using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.ClimbTree;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;

public partial class ClimbTreeModel :ActivityEntityBase
{
    public bool ShowEntrance()
    {
        return ClimbTreeModel.Instance.IsPrivateOpened() && ClimbTreeModel.Instance.CurStorageClimbTreeWeek.IsShowStartView;
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/ClimbTree/Aux_ClimbTree";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/ClimbTree/TaskList_ClimbTree";
    }
    private static ClimbTreeModel _instance;
    public static ClimbTreeModel Instance => _instance ?? (_instance = new ClimbTreeModel());
    
    public override string Guid => "OPS_EVENT_TYPE_CLIMB_TREE";

    int _lastCurLevel = 0;
    public int CurScore
    {
        get
        { 
            return CurStorageClimbTreeWeek.LastScore;
        }
        set
        {
            CurStorageClimbTreeWeek.LastScore = value;
            _curLevel = ClimbTreeModel.Instance.GetLevelByScore(CurScore);
            if (_lastCurLevel != CurLevel)
            {
                for (var i = _lastCurLevel + 1; i <= CurLevel; i++)
                {
                    CurStorageClimbTreeWeek.UnCollectLevels.Remove(i);
                    var rewards = ClimbTreeModel.Instance.GetLevelRewards(i);
                    foreach (var reward in rewards)
                    {
                        if (CurStorageClimbTreeWeek.UnCollectRewards.ContainsKey(reward.id))
                        {
                            CurStorageClimbTreeWeek.UnCollectRewards[reward.id] -= reward.count;
                            if (CurStorageClimbTreeWeek.UnCollectRewards[reward.id] == 0)
                            {
                                CurStorageClimbTreeWeek.UnCollectRewards.Remove(reward.id);
                            }
                        }
                        else
                        {
                            throw new Exception("领取奖励时未找到保存到待领取列表中的奖励");
                        }
                    }
                }
                _lastCurLevel = CurLevel;
            }
        }
    }
    int _lastTotalLevel = 0;
    public int TotalScore
    {
        get
        { 
            return CurStorageClimbTreeWeek.TotalScore;
        }
        set
        {
            CurStorageClimbTreeWeek.TotalScore = value;
            _totalLevel = ClimbTreeModel.Instance.GetLevelByScore(TotalScore);
            if (_lastTotalLevel != TotalLevel)
            {
                for (var i = _lastTotalLevel + 1; i <= TotalLevel; i++)
                {
                    CurStorageClimbTreeWeek.UnCollectLevels.Add(i);
                    var rewards = ClimbTreeModel.Instance.GetLevelRewards(i);
                    foreach (var reward in rewards)
                    {
                        if (!CurStorageClimbTreeWeek.UnCollectRewards.ContainsKey(reward.id))
                        {
                            CurStorageClimbTreeWeek.UnCollectRewards.Add(reward.id,0);
                        }
                        CurStorageClimbTreeWeek.UnCollectRewards[reward.id] += reward.count;
                    }
                }
                _lastTotalLevel = TotalLevel;
            }
        }
    }

    public List<int> GetUnCollectLevels()
    {
        var result = new List<int>();
        List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().ClimbTree.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var storageClimbTree = StorageManager.Instance.GetStorage<StorageHome>().ClimbTree[keys[i]];
            if (IsClimbTreeStorageEnd(storageClimbTree))
            {
                result.AddRange(storageClimbTree.UnCollectLevels);
            }
        }
        return result;
    }

    static bool IsClimbTreeStorageEnd(StorageClimbTree storageClimbTree)
    {
        return storageClimbTree.IsManualActivity || storageClimbTree.IsTimeOut();
    }
    public List<ResData> GetUnCollectRewards()
    {
        var unCollectRewardsList = new List<ResData>();
        List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().ClimbTree.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var storageClimbTree = StorageManager.Instance.GetStorage<StorageHome>().ClimbTree[keys[i]];
            if (IsClimbTreeStorageEnd(storageClimbTree))
            {
                foreach (var pair in storageClimbTree.UnCollectRewards)
                {
                    if (pair.Value > 0)
                    {
                        unCollectRewardsList.Add(new ResData(pair.Key,pair.Value));
                    }
                }
            }
        }
        return unCollectRewardsList;
    }

    public List<ResData> GetCurrentUnCollectRewards()
    {
        var unCollectRewardsList = new List<ResData>();
        foreach (var pair in CurStorageClimbTreeWeek.UnCollectRewards)
        {
            if (pair.Value > 0)
            {
                unCollectRewardsList.Add(new ResData(pair.Key,pair.Value));
            }
        }
        return unCollectRewardsList;
    }

    public void CleanUnCollectRewardsList()
    {
        List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().ClimbTree.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var storageClimbTree = StorageManager.Instance.GetStorage<StorageHome>().ClimbTree[keys[i]];
            if (IsClimbTreeStorageEnd(storageClimbTree))
            {
                storageClimbTree.LastScore = storageClimbTree.TotalScore;
                storageClimbTree.UnCollectRewards.Clear();
                storageClimbTree.UnCollectLevels.Clear();
            }
        }
    }

    private int _curLevel;
    public int CurLevel => _curLevel;

    private int _totalLevel;
    public int TotalLevel => _totalLevel;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    // public bool IsFirstPopUp()
    // {
    //     var value = CurStorageClimbTreeWeek.IsPreOpen;
    //     CurStorageClimbTreeWeek.IsPreOpen = false;
    //     return value;
    // }

    public const int _climbTreeBananaId = 30101;
    public const int _climbTreeBananaLineId = 30201;

    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        ClimbTreeConfigManager.Instance.InitConfig(configJson);
        InitConfig();
        InitServerDataFinish();
        if (CurStorageClimbTreeWeek != null)
        {
            CurStorageClimbTreeWeek.StartTime = (long) StartTime;
            CurStorageClimbTreeWeek.EndTime = (long) EndTime;
            CurStorageClimbTreeWeek.LeaderBoardStorage.JsonRecoverCoinRewardConfig = JsonConvert.SerializeObject(ClimbTreeModel.Instance.LeaderBoardRewardConfig);
            CurStorageClimbTreeWeek.LeaderBoardStorage.EndTime = CurStorageClimbTreeWeek.EndTime;
            CurStorageClimbTreeWeek.LeaderBoardStorage.StartTime = CurStorageClimbTreeWeek.StartTime;
            _curLevel = ClimbTreeModel.Instance.GetLevelByScore(CurScore);
            _totalLevel = ClimbTreeModel.Instance.GetLevelByScore(TotalScore);
            _lastTotalLevel = TotalLevel;
            _lastCurLevel = CurLevel;
        }
        if (CurStorageClimbTreeWeek == null && ClimbTreeModel.GetFirstWeekCanGetReward() == null)
        {
            if (!CreateStorage())
            {
                LoopCreateStorage = true;
            }
        }
        _lastActivityOpenState = IsPrivateOpened();
        DebugUtil.Log($"InitConfig:{Guid}");
        ClimbTreeLeaderBoardModel.Instance.InitFromServerData();
        CleanUselessStorage();
    }

    

    private bool LoopCreateStorage;
    public bool IsPrivateOpened()
    {
        return IsOpened() && CurStorageClimbTreeWeek != null && !CurStorageClimbTreeWeek.IsTimeOut() /*&& !CurStorageClimbTreeWeek.IsManualActivity*/;
    }
    
    public override void UpdateActivityState()
    {
        InitServerDataFinish();
    }

    protected override void InitServerDataFinish()
    {
    }

    public override bool IsOpened(bool hasLog = false)
    {
        return UnlockManager.IsOpen(UnlockManager.MergeUnlockType.ClimbTree) //已解锁
               && base.IsOpened(hasLog);
    }

    private List<TaskCompletionSource<bool>> AddScoreAsyncLock = new List<TaskCompletionSource<bool>>();
    private bool AddScoreInAsync = false;
    public async void AddScore(int score,List<TaskCompletionSource<bool>> addValueTask)
    {
        TotalScore += score;
        ClimbTreeLeaderBoardModel.Instance.SetStar(TotalScore);
        var totalLevel = TotalLevel;
        if (AddScoreInAsync)
        {
            var asyncLock = new TaskCompletionSource<bool>();
            AddScoreAsyncLock.Add(asyncLock);
            Debug.LogError("Model加分 入队列");
            await asyncLock.Task;
        }
        Debug.LogError("Model加分 开始");
        AddScoreInAsync = true;
        while (CurLevel < totalLevel)
        {
            var task = new TaskCompletionSource<bool>();
            addValueTask.Add(task);
            await task.Task;
            addValueTask.Remove(task);
            var nextLevelScore =
                ClimbTreeModel.Instance.GetLevelBaseScore(ClimbTreeModel.Instance.CurLevel + 1);
            var addScore = nextLevelScore - CurScore;
            CurScore += addScore;
            score -= addScore;
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonkeyReward,ClimbTreeModel.Instance.CurLevel.ToString());
            if (MergeTaskTipsController.Instance._mergeClimbTree)
                MergeTaskTipsController.Instance._mergeClimbTree.PerformLevelUp();
            var rewards = ClimbTreeModel.Instance.GetLevelRewards(CurLevel);
            for (int i = 0; i < rewards.Count; i++)
            {
                var reward = rewards[i];
            
                if (!UserData.Instance.IsResource(reward.id))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonMonkeyReward,
                        itemAId = reward.id,
                        isChange = true,
                    });
                }
                UserData.Instance.AddRes(reward.id, reward.count,
                    new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MonkeyReward}, true);
            }
            if (IsMaxLevel())
            {
                CompletedActivity();
            }
        }
        CurScore += score;
        Debug.LogError("Model加分 结束");
        AddScoreInAsync = false;
        if (AddScoreAsyncLock.Count > 0)
        {
            Debug.LogError("Model加分 传递");
            var asyncLock = AddScoreAsyncLock[0];
            AddScoreAsyncLock.RemoveAt(0);
            asyncLock.SetResult(true);
        }
    }

    public bool IsMaxLevel()
    {
        return CurLevel >= ClimbTreeModel.Instance.MaxLevel;
    }

    public void RemoveAllClimbTreeBanana()
    {
        MergeManager.Instance.RemoveAllItemByType(MergeItemType.climbTreeBanana,MergeBoardEnum.Main,"ClimbTreeRemoveAll");
    }
    public bool CanManualActivity()
    {
        if (!IsOpened())
            return false;
        if (IsMaxLevel())
            return true;
        return false;
    }

    public int GetUpperScore(int score,int level)
    {
        return score - ClimbTreeModel.Instance.GetLevelBaseScore(level);
    }


    public void CompletedActivity()
    {
        // CurStorageClimbTreeWeek.IsManualActivity = true;
    }
 
    // public bool IsComplete()
    // {
    //     return IsOpened() && TotalScore >= ClimbTreeModel.Instance.MaxScore;
    // }

    public void EndActivity()
    {
        if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIClimbTreeMain)==null)
        {
            UIPopupClimbTreeEndController.CanShowUI();
        }
        ClimbTreeLeaderBoardModel.CanShowUnCollectRewardsUI();
        if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
        {
            if(ClimbTreeModel.Instance.IsCanClearBananas())
                ClimbTreeModel.Instance.RemoveAllClimbTreeBanana();
        }
    }

    public void StartActivity()
    {
        // CanShowUI();
    }

    // public int CreateBananaOnMerge(TableMergeItem mergeNewConfig = null)
    // {
    //     if (!IsOpened())
    //         return -1;
    //     if (CanShowStartView())
    //         return -1;
    //     if (mergeNewConfig?.in_line == ClimbTreeModel._climbTreeBananaLineId)
    //         return -1;
    //     if (!ClimbTreeModel.Instance.TryCreateBanana())
    //         return -1;
    //     return _climbTreeBananaId;
    // }
    
    // private Dictionary<int, ClimbTreeProductConfig> _climbTreeProductConfig;
    // public Dictionary<int,ClimbTreeProductConfig> ClimbTreeProductConfig
    // {
    //     get
    //     {
    //         if (_climbTreeProductConfig == null)
    //         {
    //             _climbTreeProductConfig = new Dictionary<int, ClimbTreeProductConfig>();
    //             var serverCfg = ClimbTreeModel.Instance.ClimbTreeProductList;
    //             foreach (var cfg in serverCfg)
    //             {
    //                 _climbTreeProductConfig.Add(cfg.ItemId,cfg);
    //             }
    //         }
    //         return _climbTreeProductConfig;
    //     }
    // }
    // public void TryProductClimbTree(int index,int id,int doubleEnergyTimes,MergeBoard board,out bool ignoreUse)//尝试生成活动棋子
    // {
    //     ignoreUse = false;
    //     if (!IsOpened())
    //         return;
    //     if (CanShowStartView())
    //         return;
    //     //缺判断能否生成棋子的条件
    //     if (!ClimbTreeProductConfig.ContainsKey(id))
    //         return;
    //     var cfg = ClimbTreeProductConfig[id];
    //     var randomWeight = UnityEngine.Random.Range(0, cfg.MaxWeight);
    //     var tempWeight = 0;
    //     var newItemId = -1;
    //     for (var i = 0; i < cfg.Weight.Count; i++)
    //     {
    //         tempWeight += cfg.Weight[i];
    //         if (tempWeight > randomWeight)
    //         {
    //             newItemId = cfg.OutPut[i];
    //             break;
    //         }
    //     }
    //     if (newItemId < 0)
    //         return;
    //     //缺判断能否生成棋子的条件
    //     var newItemConfig = GameConfigManager.Instance.GetItemConfig(newItemId);
    //     if (doubleEnergyTimes > 0)
    //     {
    //         for (var i = 0; i < doubleEnergyTimes; i++)
    //         {
    //             if (newItemConfig.next_level > 0)
    //             {
    //                 newItemId = newItemConfig.next_level;
    //                 newItemConfig = GameConfigManager.Instance.GetItemConfig(newItemId);
    //             }   
    //         }
    //     }
    //
    //     var count = 1;
    //     var multi = ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.ClimbTree);
    //     count = (int)(count*multi);
    //     for (var i = 0; i < count; i++)
    //     {
    //         int emptyIndexForClimbTree = MergeManager.Instance.FindEmptyGrid(index,(MergeBoardEnum)board._boardID,true);
    //         GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonkeyBubble);
    //         GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
    //         {
    //             MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonMonkeyBubble,
    //             itemAId = newItemConfig.id,
    //             isChange = true,
    //             data1 = multi.ToString(),
    //         });
    //         if (emptyIndexForClimbTree != -1)
    //         {
    //             // MergeMainController.Instance.MergeBoard.ProductOneItem(productItem.index, emptyIndexForClimbTree, newItemConfig.id, false, RefreshItemSource.product);
    //             MergeManager.Instance.SetNewBoardItem(emptyIndexForClimbTree, newItemConfig.id, 1, RefreshItemSource.product,MergeBoardEnum.Main, index);
    //             if (index == emptyIndexForClimbTree)
    //             {
    //                 ignoreUse = true;
    //             }
    //         }
    //         else
    //         {
    //             var mergeItem = MergeManager.Instance.GetEmptyItem();
    //             mergeItem.Id = newItemConfig.id;
    //             mergeItem.State = 1;
    //             MergeManager.Instance.AddRewardItem(mergeItem,(MergeBoardEnum)board._boardID, 1);
    //             FlyGameObjectManager.Instance.FlyCurrency(newItemConfig.id, 1, board.GetGridPosition(index), 1, false,
    //                 action:
    //                 () => { });
    //         }   
    //     }
    // }
    
    
    public async void AddClimbTreeBanana(MergeBoardItem mergeItem,TableMergeItem tableMerge, int index)
    {
        if (!ClimbTreeModel.Instance.IsPrivateOpened())
            return;
        if (tableMerge == null)
            return;

        mergeItem.SendHarvestBi();
        MergeManager.Instance.RemoveBoardItem(index,MergeBoardEnum.Main,"ClimbTree");
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonMonkeyBanana,
            itemAId = tableMerge.id,
            isChange = true,
                   
        });
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID, Vector2Int.zero,MergeBoardEnum.Main);

        ShakeManager.Instance.ShakeLight();
        // bool isComplete = ClimbTreeModel.Instance.IsComplete();
        var addValueTask = new List<TaskCompletionSource<bool>>();
        ClimbTreeModel.Instance.AddScore(tableMerge.value,addValueTask);
        if (MergeTaskTipsController.Instance._mergeClimbTree && 
            (MergeTaskTipsController.Instance.contentRect.anchoredPosition.x <
            -MergeTaskTipsController.Instance._mergeClimbTree.transform.localPosition.x + 220 ||
            MergeTaskTipsController.Instance.contentRect.anchoredPosition.x - Screen.width >
            -MergeTaskTipsController.Instance._mergeClimbTree.transform.localPosition.x + 220))
        {
            var moveTask = new TaskCompletionSource<bool>();
            MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-MergeTaskTipsController.Instance._mergeClimbTree.transform.localPosition.x+220, 0).OnComplete(
                () =>
                {
                    moveTask.SetResult(true);   
                });
            await moveTask.Task;   
        }

        if (MergeTaskTipsController.Instance._mergeClimbTree)
        {
            MergeTaskTipsController.Instance._mergeClimbTree.PreBananaFly();
            Transform target = MergeTaskTipsController.Instance._mergeClimbTree.transform;
            FlyGameObjectManager.Instance.FlyObject(index, tableMerge.id, mergeItem.transform.position, target, 0.8f, () =>
            {
                ShakeManager.Instance.ShakeLight();
                FlyGameObjectManager.Instance.PlayHintStarsEffect(target.position);
                MergeTaskTipsController.Instance._mergeClimbTree.PerformAddValue( tableMerge.value, 0.3f, addValueTask);
            });   
        }
        else
        {
            while (addValueTask.Count > 0)
            {
                addValueTask[0].SetResult(true);
            }
        }
    }
    public bool CanShowStartView()
    {
        return IsOpened() && !CurStorageClimbTreeWeek.IsShowStartView;
    }
    public void ShowStartView()
    {
        CurStorageClimbTreeWeek.IsShowStartView = true;
    }
    
    public bool CanShowPreStartView()
    {
        return IsOpened() && !CurStorageClimbTreeWeek.IsPreOpen;
    }
    
    public void ShowPreStartView()
    {
        CurStorageClimbTreeWeek.IsPreOpen = true;
    }
    public ClimbTreeModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }
    private bool _lastActivityOpenState;
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;
        if (LoopCreateStorage && IsOpened() && CurStorageClimbTreeWeek == null)
        {
            if (CreateStorage())
            {
                LoopCreateStorage = false;
            }
        }
        var currentActivityOpenState = ClimbTreeModel.Instance.IsPrivateOpened();
        if (_lastActivityOpenState == currentActivityOpenState)
            return;
        if (_lastActivityOpenState && !currentActivityOpenState)
        {
            ClimbTreeModel.Instance.EndActivity();
        }
        else if(!_lastActivityOpenState && currentActivityOpenState)
        {
            ClimbTreeModel.Instance.StartActivity();
        }

        _lastActivityOpenState = currentActivityOpenState;
    }
    public bool IsCanClearBananas()
    {
        if (IsInitFromServer())
            return !IsOpened();

        return IsActivityEnd();
    }
    
    public bool IsActivityEnd()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.ClimbTree))
            return false;
        List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().ClimbTree.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var storageClimbTree = StorageManager.Instance.GetStorage<StorageHome>().ClimbTree[keys[i]];
            if (!IsClimbTreeStorageEnd(storageClimbTree))
                return false;
        }
        return true;
    }
    
    
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.ClimbTree);
    }
    // private static string coolTimeKey = "ClimbTree";
    // public static bool CanShowUI()
    // {
    //     if (!ClimbTreeModel.Instance.IsPrivateOpened())
    //         return false;
    //
    //     if (ClimbTreeModel.Instance.CanShowPreStartView())
    //     {
    //         UIManager.Instance.OpenUI(UINameConst.UIClimbTreeStart);
    //         return true;
    //     }
    //
    //     // if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
    //     // {
    //     //     CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
    //     //     UIManager.Instance.OpenUI(UINameConst.UIClimbTreeMain);
    //     //     return true;
    //     // }
    //     return false;
    // }
    public void CleanUselessStorage()
    {
        var cleanStorageKeyList = StorageClimbTree.Keys.ToList();
        foreach (var key in cleanStorageKeyList)
        {
            var storage = StorageClimbTree[key];
            storage.TryRelease();
        }
    }
    public List<ClimbTreeTaskRewardConfig> TaskRewardConfig => ClimbTreeConfigManager.Instance.GetConfig<ClimbTreeTaskRewardConfig>();
    public int GetTaskValue(StorageTaskItem taskItem, bool isMul)
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
}