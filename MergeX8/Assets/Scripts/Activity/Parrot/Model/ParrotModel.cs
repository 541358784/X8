using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Activity.Base;
using Deco.Node;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonPlus.Config.Parrot;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using SomeWhere;
using SRF;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class ParrotModel : ActivityEntityBase, I_ActivityStatus
{
    public const int ParrotMergeItemId = 20201;
    public bool ShowAuxItem()
    {
        if (!ParrotModel.Instance.IsOpened())
            return false;
        return true;
    }
    public bool ShowTaskEntrance()
    {
        if (!IsOpened())
            return false;
        if (Storage.GetPreheatLeftTime() > 0)
            return false;
        return true;
    }
    private static ParrotModel _instance;
    public static ParrotModel Instance => _instance ?? (_instance = new ParrotModel());

    public override string Guid => "OPS_EVENT_TYPE_PARROT";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }
    public ParrotGlobalConfig GlobalConfig => ParrotGlobalConfigList[0];
    public List<ParrotTaskRewardConfig> TaskRewardConfig => ParrotTaskRewardConfigList;
    
    public List<ParrotRewardConfig> RewardConfig => ParrotRewardConfigList;
    public long PreheatTime=> IsSkipActivityPreheating()?0:(long)((ulong)GlobalConfig.PreheatTime * XUtility.Hour);
    public StorageParrot Storage => StorageManager.Instance.GetStorage<StorageHome>().Parrot;

    public void InitStorage()
    {
        if (!IsInitFromServer())
            return;
        if (Storage.ActivityId != ActivityId)
        {
            RemoveAllParrotItem();
            Storage.Clear();
            Storage.ActivityId = ActivityId;
            Storage.TotalScore = 0;
            Storage.IsStart = false;
            Storage.IsEnd = false;
            Storage.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().ParrotGroupId;
        }
        Storage.StartTime = (long)StartTime;
        Storage.PreheatCompleteTime = (long)StartTime + PreheatTime;
        Storage.EndTime = (long)EndTime;
    }
    // private static void InitTable<T>(Dictionary<int, T> config) where T : TableBase
    // {
    //     if (config == null)
    //         return;
    //
    //     List<T> tableData = ParrotConfigManager.Instance.GetConfig<T>();
    //     if (tableData == null)
    //         return;
    //
    //     config.Clear();
    //     foreach (T kv in tableData)
    //     {
    //         config.Add(kv.GetID(), kv);
    //     }
    // }
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DebugUtil.LogError("1");
        ParrotConfigManager.Instance.InitConfig(configJson);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
        ParrotLeaderBoardModel.Instance.InitFromServerData();
        XUtility.WaitFrames(1).AddCallBack(() =>
        {
            ParrotLeaderBoardModel.Instance.CreateStorage(Storage); 
        }).WrapErrors();
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Parrot);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() &&!Storage.IsTimeOut();
    }

    public int GetScore()
    {
        return Storage.TotalScore;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.Parrot);
    }

    public Transform GetCommonFlyTarget()
    {
        var storage = Storage;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = MergeParrot.Instance;
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = Aux_Parrot.Instance;
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }
    public bool IsStart()
    {
        return IsPrivateOpened() && APIManager.Instance.GetServerTime() > (ulong)Storage.PreheatCompleteTime;
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
        var configs = TaskRewardConfig;
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
    public void AddScore(int addCount,string reason)
    {
        var oldValue = Storage.TotalScore;
        var oldState = GetLevelState(oldValue);
        Storage.TotalScore += addCount;
        if (addCount > 0)
        {
            ParrotLeaderBoardModel.Instance.GetLeaderBoardStorage(Storage.ActivityId)?.CollectStar(addCount, () =>
            {
                CanShowLeaderBoardGuide();
            });
        }
        var newValue = Storage.TotalScore;
        var newState = GetLevelState(newValue);
        var rewards = new List<ResData>();
        for (var i = oldState.Level; i < newState.Level; i++)
        {
            var rewardConfig = RewardConfig.Find(a => a.Id == i);
            rewards.AddRange(CommonUtils.FormatReward(rewardConfig.RewardId, rewardConfig.RewardNum));
        }
        if (rewards.Count > 0)
            UserData.Instance.AddRes(rewards,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.ParrotGet));
        EventDispatcher.Instance.SendEventImmediately(new EventParrotScoreChange(addCount));
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventParrotRadishChange,
            addCount.ToString(), newValue.ToString(), reason);
        if (oldState.Level != newState.Level)
        {
            UIParrotMainController.Open().PerformJump(oldState,newState).WrapErrors();
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventParrotPass,
                oldState.Level.ToString());
        }
    }

    public ParrotLevelState GetLevelState(int score)
    {
        var state = new ParrotLevelState();
        var lastLevelScore = 0;
        for (var i=0;i<RewardConfig.Count;i++)
        {
            var config = RewardConfig[i];
            if (score < config.Score)
            {
                state.Level = config.Id;
                state.Group = config.Group;
                state.GroupInnerIndex = config.GroupInnerIndex;
                state.CurScore = score - lastLevelScore;
                state.MaxScore = config.Score - lastLevelScore;
                state.Rewards = CommonUtils.FormatReward(config.RewardId, config.RewardNum);
                return state;
            }
            else
            {
                if (i == RewardConfig.Count - 1)
                {
                    state.Level = config.Id+1;
                    state.Group = config.Group;
                    state.GroupInnerIndex = config.GroupInnerIndex+1;
                    state.CurScore = score;
                    state.MaxScore = config.Score;
                    state.Rewards = new List<ResData>();
                    Storage.IsEnd = true;//完成标志
                    return state;
                }
                else
                {
                    lastLevelScore = config.Score;   
                }
            }
        }
        return state;
    }
    public const string preheatCoolTimeKey = "ParrotPreheat";
    public static bool CanShowPreheatPopupEachDay()
    {
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, preheatCoolTimeKey))
            return false;
        if (CanShowPreheatPopup())
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, preheatCoolTimeKey,CommonUtils.GetTimeStamp());
            return true;
        }
        return false;
    }
    public static bool CanShowPreheatPopup()
    {
        if (Instance.IsPrivateOpened() && Instance.Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupParrotPreviewController.Open();
            return true;
        }
        return false;
    }
    public static bool CanShowStartPopup()
    {
        if (Instance.IsStart() && !Instance.Storage.IsStart)
        {
            Instance.Storage.IsStart = true;
            UIParrotStartController.Open();
            return true;
        }
        return false;
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/Parrot/Aux_Parrot";
    }

    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/Parrot/TaskList_Parrot";
    }
    public void RemoveAllParrotItem()
    {
        MergeManager.Instance.RemoveAllItemByType(MergeItemType.Parrot,MergeBoardEnum.Main,"ParrotRemove");
    }

    public bool IsCanClearParrotItem()
    {
        return Storage.IsTimeOut();
    }
    
    public async void AddParrot(MergeBoardItem mergeItem,TableMergeItem tableMerge, int index)
    {
        if (!IsStart())
            return;
        if (tableMerge == null)
            return;

        mergeItem.SendHarvestBi();
        MergeManager.Instance.RemoveBoardItem(index,MergeBoardEnum.Main,"Parrot");
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonParrotGet,
            itemAId = tableMerge.id,
            isChange = true,
                   
        });
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID, Vector2Int.zero,MergeBoardEnum.Main);
        ShakeManager.Instance.ShakeLight();
        
        AddScore(tableMerge.value,"UseMergeItem");
        if (MergeTaskTipsController.Instance.MergeParrot && 
            (MergeTaskTipsController.Instance.contentRect.anchoredPosition.x <
            -MergeTaskTipsController.Instance.MergeParrot.transform.localPosition.x + 220 ||
            MergeTaskTipsController.Instance.contentRect.anchoredPosition.x - Screen.width >
            -MergeTaskTipsController.Instance.MergeParrot.transform.localPosition.x + 220))
        {
            var moveTask = new TaskCompletionSource<bool>();
            MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-MergeTaskTipsController.Instance.MergeParrot.transform.localPosition.x+220, 0).OnComplete(
                () =>
                {
                    moveTask.SetResult(true);   
                });
            await moveTask.Task;   
        }

        if (MergeTaskTipsController.Instance.MergeParrot)
        {
            Transform target = MergeTaskTipsController.Instance.MergeParrot.transform;
            FlyGameObjectManager.Instance.FlyObject(index, tableMerge.id, mergeItem.transform.position, target, 0.8f, () =>
            {
                ShakeManager.Instance.ShakeLight();
                FlyGameObjectManager.Instance.PlayHintStarsEffect(target.position);
                MergeTaskTipsController.Instance.MergeParrot.PerformAddValue( tableMerge.value);
            });   
        }
    }

    public I_ActivityStatus.ActivityStatus GetActivityStatus()
    {
        if (Storage.IsEnd)
        {
            return I_ActivityStatus.ActivityStatus.Completed;
        }
        else if (Storage.TotalScore > 0)
        {
            return I_ActivityStatus.ActivityStatus.Incomplete;
        }
        else if (!Storage.ActivityId.IsEmptyString())
        {
            return I_ActivityStatus.ActivityStatus.NotParticipated;
        }
        return I_ActivityStatus.ActivityStatus.None;
    }
}