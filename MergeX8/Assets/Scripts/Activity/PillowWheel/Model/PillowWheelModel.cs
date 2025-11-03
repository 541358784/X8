using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.PillowWheel;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using SomeWhere;
using UnityEngine;

public partial class PillowWheelModel : ActivityEntityBase
{
    #region 基础属性
    public override string Guid => "OPS_EVENT_TYPE_PILLOW_WHEEL";

    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.PillowWheel);
    }

    private static PillowWheelModel _instance;
    public static PillowWheelModel Instance => _instance ?? (_instance = new PillowWheelModel());

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime, ulong rewardEndTime,
        bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson, activitySubType);
        PillowWheelConfigManager.Instance.InitConfig(configJson);
        InitStorage();
        PillowWheelLeaderBoardModel.Instance.InitFromServerData();
        XUtility.WaitFrames(1).AddCallBack(() =>
        {
            PillowWheelLeaderBoardModel.Instance.CreateStorage(Storage); 
        }).WrapErrors();
    }
    public StoragePillowWheel Storage => StorageManager.Instance.GetStorage<StorageHome>().PillowWheel;
    public void InitStorage()
    {
        if (Storage.ActivityId != ActivityId)
        {
            Storage.Clear();
            Storage.ActivityId = ActivityId;
            Storage.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().PillowWheel;
        }
        Storage.StartTime = (long)StartTime;
        Storage.PreheatCompleteTime = (long)StartTime + (long)((ulong)GlobalConfig.PreheatTime * XUtility.Hour);;
        Storage.EndTime = (long)EndTime;
    }
    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.PillowWheel);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() &&!Storage.IsTimeOut();
    }
    public bool IsStart()
    {
        return IsPrivateOpened() && Storage.GetPreheatLeftTime() == 0;
    }
    public Transform GetCommonFlyTarget()
    {
        var storage = Storage;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = MergePillowWheel.Instance;
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = Aux_PillowWheel.Instance;
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/PillowWheel/Aux_PillowWheel";
    }

    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/PillowWheel/TaskList_PillowWheel";
    }
    public bool ShowAuxItem()
    {
        if (!IsOpened())
            return false;
        return true;
    }
    public bool ShowTaskEntrance()
    {
        if (!IsStart())
            return false;
        return true;
    }
    #endregion

    public int GetItem()
    {
        return Storage.ItemCount;
    }

    public void AddItem(int count, GameBIManager.ItemChangeReasonArgs reason)
    {
        Storage.ItemCount += count;
        if (count > 0)
        {
            Storage.TotalItemCount += count;
            PillowWheelLeaderBoardModel.Instance.GetLeaderBoardStorage(Storage.ActivityId)?.CollectStar(count, () =>
            {
                CanShowLeaderBoardGuide();
            });
        }
        GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.PillowWheel, count, (ulong)Storage.ItemCount,
            reason);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventPillowWheelRadishChange,count.ToString(),Storage.ItemCount.ToString(),reason.ToString());
        EventDispatcher.Instance.SendEventImmediately(new EventPillowWheelItemChange(count));
    }
    public void PurchaseSuccess(TableShop tableShop)
    {
        if (tableShop == null)
            return;
        var shopConfig = ShopConfigList[0];
        if (tableShop.id != shopConfig.ShopId)
            return;
        var rewards = CommonUtils.FormatReward(shopConfig.RewardId, shopConfig.RewardNum);
        var itemChangeReason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap);
        UserData.Instance.AddRes(rewards,itemChangeReason);
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, false,
            itemChangeReason,animEndCall: () =>
            {
                if (UIPillowWheelMainController.Instance)
                    UIPillowWheelMainController.Instance.UpdateItemCountText();
            });
    }
    public int GetTaskValue(StorageTaskItem taskItem)
    {
        if (!IsInitFromServer())
            return 0;
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
        var value = 0;
        var configs = TaskRewardConfigList;
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
        return value;
    }

    public List<PillowWheelResultConfig> ResultList()
    {
        var resultList = new List<PillowWheelResultConfig>();
        foreach (var config in ResultConfigList)
        {
            if (!Storage.CollectState.Contains(config.Id) && Storage.CollectState.Count >= config.UnlockSpinTimes)
            {
                resultList.Add(config);
            }
        }

        return resultList;
    }
    
    public PillowWheelResultConfig Spin()
    {
        var resultList = ResultList();
        if (resultList.Count == 0)
            return null;

        var weightList = new List<int>();
        foreach (var config in resultList)
        {
            weightList.Add(config.Weight);
        }

        var rewards = new List<ResData>();
        var randomIndex = weightList.RandomIndexByWeight();
        var randomResult = resultList[randomIndex];
        Storage.CollectState.Add(randomResult.Id);
        if (randomResult.RewardId < 0)
        {
            var specialKey = -randomResult.RewardId;
            Storage.SpecialCollectState.TryAdd(specialKey, 0);
            Storage.SpecialCollectState[specialKey]++;
            var specialConfig = SpecialRewardConfigList.Find(a => a.Id == specialKey);
            if (Storage.SpecialCollectState[specialKey] == specialConfig.Count)
            {
                rewards.Add(new ResData(specialConfig.RewardId,specialConfig.RewardNum));
            }
        }
        else
        {
            rewards.Add(new ResData(randomResult.RewardId,randomResult.RewardNum));
        }
        
        if (rewards.Count > 0 && !UserData.Instance.IsResource(rewards[0].id))
        {
            TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig(rewards[0].id);
            if (mergeItemConfig != null)
            {
                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                {
                    MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonPillowWheelGet,
                    itemAId = mergeItemConfig.id,
                    ItemALevel = mergeItemConfig.level,
                    isChange = true,
                });
            }
        }

        var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.PillowWheelGet);
        UserData.Instance.AddRes(rewards,reason);
        EventDispatcher.Instance.SendEventImmediately(new EventPillowWheelCollectStateChange(randomResult.Id));

        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventPillowWheelPass,
            randomResult.Id.ToString());
        return randomResult;
    }
    
    public static bool CanShowPreheatPopup()
    {
        if (Instance.IsPrivateOpened() && Instance.Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupPillowWheelPreviewController.Open();
            return true;
        }
        return false;
    }
    public const string coolTimeKey = "PillowWheel";
    public static bool CanShowPreheatPopupEachDay()
    {
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            return false;
        if (CanShowPreheatPopup())
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,CommonUtils.GetTimeStamp());
            return true;
        }
        return false;
    }
}