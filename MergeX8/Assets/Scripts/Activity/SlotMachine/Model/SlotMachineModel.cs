using System.Collections.Generic;
using System.Linq;
using Activity.SlotMachine.View;
using DragonPlus;
using DragonPlus.Config.SlotMachine;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using Gameplay;
using UnityEngine;

public class SlotMachineModel : ActivityEntityBase
{
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/SlotMachine/Aux_SlotMachine";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/SlotMachine/TaskList_SlotMachine";
    }

    private static SlotMachineModel _instance;
    public static SlotMachineModel Instance => _instance ?? (_instance = new SlotMachineModel());

    public Dictionary<string, StorageSlotMachine> StorageDic => StorageManager.Instance.GetStorage<StorageHome>().SlotMachine;
    public StorageSlotMachine CurStorage
    {
        get
        {
            if (!IsInitFromServer())
                return null;
            if (!StorageDic.TryGetValue(ActivityId,out var storage))
            {
                storage = new StorageSlotMachine()
                {
                    HasUnCollectResult = false,
                    IsStart = false,
                };
                foreach (var a in GlobalConfig.ResultConfigList)
                {
                    storage.ElementIndexList.Add(4);
                }
                StorageDic.Add(ActivityId,storage);
            }
            return storage;
        }
    }
    public override string Guid => "OPS_EVENT_TYPE_SLOT_MACHINE";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }
    private static void InitTable<T>(Dictionary<int, T> config) where T : TableBase
    {
        if (config == null)
            return;

        List<T> tableData = SlotMachineConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }
    public SlotMachineGlobalConfig GlobalConfig => SlotMachineConfigManager.Instance.GetConfig<SlotMachineGlobalConfig>()[0];
    public Dictionary<int, SlotMachineResultConfig> ResultConfigDic = new Dictionary<int, SlotMachineResultConfig>();
    public List<SlotMachineReSpinConfig> ReSpinConfigList => SlotMachineConfigManager.Instance.GetConfig<SlotMachineReSpinConfig>();
    public List<SlotMachineRewardConfig> RewardConfigList => SlotMachineConfigManager.Instance.GetConfig<SlotMachineRewardConfig>();
    public List<SlotMachineTaskRewardConfig> TaskRewardConfigList => SlotMachineConfigManager.Instance.GetConfig<SlotMachineTaskRewardConfig>();
    
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        SlotMachineConfigManager.Instance.InitConfig(configJson);
        InitTable(ResultConfigDic);
        _lastActivityOpenState = IsOpened();
        InitServerDataFinish();
    }
    public SlotMachineModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }

    public int GetScore()
    {
        if (!IsOpened())
            return 0;
        return CurStorage.SpinCount;
    }

    public void AddScore(int addCount,string reason)
    {
        if (!IsOpened())
            return;
        CurStorage.SpinCount += addCount;
        EventDispatcher.Instance.SendEventImmediately(new EventSlotMachineScoreChange(addCount));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSlotMachineRadishChange,
            addCount.ToString(),CurStorage.SpinCount.ToString(),reason);
    }
    private bool _lastActivityOpenState;//记录上一帧的活动开启状态，在轮询中判断是否触发开启活动或者关闭活动
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;

        var currentActivityOpenState = IsOpened();
        if (_lastActivityOpenState == currentActivityOpenState)
            return;
        if (!currentActivityOpenState)
        {
            EventDispatcher.Instance.SendEventImmediately(new EventSlotMachineEnd());
        }
        else
        {
        }
        _lastActivityOpenState = currentActivityOpenState;
    }

    private bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.SlotMachine);
    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock;
    }

    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.SlotMachine);
    }

    // public const string coolTimeKey = "SlotMachine";
    // public static bool CanShowStartPopupEachDay()
    // {
    //     if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
    //         return false;
    //     if (CanShowStartPopup())
    //     {
    //         CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,CommonUtils.GetTimeStamp());
    //         return true;
    //     }
    //     return false;
    // }

    public static bool CanShowStartPopup()
    {
        // if (Instance.IsOpened() && !Instance.CurStorage.IsStart)
        // {
        //     Instance.CurStorage.IsStart = true;
        //     UIPopupSlotMachineStartController.Open(Instance.CurStorage);
        //     return true;
        // }
        if (Instance.IsOpened() && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SlotMachineAuxItem))
        {
            var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_SlotMachine>();
            if (auxItem != null)
            {
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(auxItem.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SlotMachineAuxItem, auxItem.transform as RectTransform,
                    topLayer: topLayer);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SlotMachineAuxItem, null))
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    public int GetTaskValue(StorageTaskItem taskItem, bool isMul)
    {
        int tempPrice = 0;
        for (var i = 0; i < taskItem.RewardTypes.Count; i++)
        {
            if (taskItem.RewardTypes[i] == (int)UserData.ResourceId.Coin || taskItem.RewardTypes[i] == (int)UserData.ResourceId.RareDecoCoin)
            {
                if(taskItem.RewardNums.Count > i)
                    tempPrice = taskItem.RewardNums[i];
                
                break;
            }
        }

        if (tempPrice == 0)
        {
            foreach (var itemId in taskItem.ItemIds)
            {
                tempPrice += OrderConfigManager.Instance.GetItemPrice(itemId);
            }
        }
        
        var configs = TaskRewardConfigList;
        var value = 0;
        if (configs != null && configs.Count > 0)
        {
            foreach (var config in configs)
            {
                if (tempPrice <= config.Max_value)
                {
                    value = config.Output;
                    break;
                }
            }
        }

        // if (isMul && MultipleScoreModel.Instance.IsOpenActivity())
        //     value = (int)(value * MultipleScoreModel.Instance.GetMultiple(MultipleScoreModel.InfluenceFuncType.SlotMachine));
        return value;
    }
    

    public bool ShowEntrance()
    {
        if (CurStorage == null)
            return false;

        return SlotMachineUtils.ShowEntrance(CurStorage);
    }
}