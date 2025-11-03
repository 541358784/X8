using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deco.Node;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonPlus.Config.SnakeLadder;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class SnakeLadderModel : ActivityEntityBase
{
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/SnakeLadder/Aux_SnakeLadder";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/SnakeLadder/TaskList_SnakeLadder";
    }

    public bool ShowEntrance()
    {
        return IsStart();
    }
    private static SnakeLadderModel _instance;
    public static SnakeLadderModel Instance => _instance ?? (_instance = new SnakeLadderModel());

    public override string Guid => "OPS_EVENT_TYPE_SNAKE_LADDER";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public static StorageDictionary<string,StorageSnakeLadder> StorageSnakeLadder => StorageManager.Instance.GetStorage<StorageHome>().SnakeLadder;

    public StorageSnakeLadder CurStorageSnakeLadderWeek
    {
        get
        {
            if (ActivityId == null)
                return null;
            StorageSnakeLadder.TryGetValue(ActivityId, out StorageSnakeLadder curWeek);
            return curWeek;
        }
    }

    public bool CreateStorage()
    {
        DebugUtil.LogError("CreateStorage3");
        if (!SceneFsm.mInstance.ClientInited)
            return false;
        if (CurStorageSnakeLadderWeek == null && IsInitFromServer()
            && SnakeLadderModel.GetFirstWeekCanGetReward() == null
            && IsOpened())
        {
            DebugUtil.LogError("CreateStorage4");
            var newWeek = new StorageSnakeLadder()
            {
                ActivityId = ActivityId,
                TurntableCount = 0,
                ScoreMultiValue = 1,
                StepMultiValue = 1,
                WildCardCount = 0,
                DefenseCardCount = 0,
                StartTime = (long) StartTime,
                PreheatCompleteTime = (long)StartTime+PreheatTime,
                EndTime = (long) EndTime,
                ActivityResList = {},
                FinishStoreItemList = {},
                Score = 0,
                IsStart = false,
                TotalScore = 0,
                TurntableRandomPool = {},
                UnLockStoreLevel = {1},
                CompleteTimes = 0,
                CurLevelId = 1,
                CurBlockIndex = 0,
            };
            var resMd5List = ActivityManager.Instance.GetActivityMd5List(ActivityId);
            newWeek.ActivityResList.Clear();
            foreach (var resMd5 in resMd5List)
            {
                var resPath = ActivityResHotUpdate.GetFilePath(resMd5);
                newWeek.ActivityResList.Add(resPath);
            }

            // for (var i = 0; i < StoreLevelConfig.Count; i++)
            // {
            //     var levelCfg = StoreLevelConfig[i];
            //     for (var i1 = 0; i1 < levelCfg.StoreItemList.Count; i1++)
            //     {
            //         var storeItemCfg = StoreItemConfig[levelCfg.StoreItemList[i1]];
            //         if ((SnakeLadderStoreItemType)storeItemCfg.Type == SnakeLadderStoreItemType.BuildItem)
            //         {
            //             var isOwned = true;
            //             for (var i2 = 0; i2 < storeItemCfg.RewardId.Count; i2++)
            //             {
            //                 var decoItem = DecoWorld.ItemLib[storeItemCfg.RewardId[i2]];
            //                 if (!decoItem.IsOwned)
            //                 {
            //                     isOwned = false;
            //                     break;
            //                 }
            //             }
            //             if (isOwned)
            //             {
            //                 newWeek.FinishStoreItemList.Add(storeItemCfg.Id);
            //             }
            //         }
            //     }
            // }
            newWeek.TurntableRandomPool.Clear();
            var curLevel = newWeek.GetCurLevel();
            for (var i = 0; i < curLevel.CardPool().Count; i++)
            {
                newWeek.TurntableRandomPool.Add(curLevel.CardPool()[i]);
            }
            StorageSnakeLadder.Add(ActivityId, newWeek);
            SnakeLadderLeaderBoardModel.Instance.CreateStorage(newWeek.LeaderBoardStorage);
            return true;
        }

        return false;
    }
    public SnakeLadderGlobalConfig GlobalConfig => SnakeLadderConfigManager.Instance.GetConfig<SnakeLadderGlobalConfig>()[0];
    public long PreheatTime=> (long)(GlobalConfig.PreheatTime * XUtility.Hour);
    private static void InitTable<T>(Dictionary<int, T> config) where T : TableBase
    {
        if (config == null)
            return;

        List<T> tableData = SnakeLadderConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }
    public List<SnakeLadderLevelConfig> LevelConfig => SnakeLadderConfigManager.Instance.GetConfig<SnakeLadderLevelConfig>();
    public Dictionary<int, SnakeLadderBlockConfig> BlockConfig = new Dictionary<int, SnakeLadderBlockConfig>();
    public List<SnakeLadderStoreLevelConfig> StoreLevelConfig => SnakeLadderConfigManager.Instance.GetConfig<SnakeLadderStoreLevelConfig>();
    public Dictionary<int, SnakeLadderStoreItemConfig> StoreItemConfig = new Dictionary<int, SnakeLadderStoreItemConfig>();
    public Dictionary<int, SnakeLadderCardConfig> CardConfig = new Dictionary<int, SnakeLadderCardConfig>();
    public List<SnakeLadderLeaderBoardRewardConfig> LeaderBoardRewardConfig=> SnakeLadderConfigManager.Instance.GetConfig<SnakeLadderLeaderBoardRewardConfig>();
    public List<SnakeLadderTaskRewardConfig> TaskRewardConfig => SnakeLadderConfigManager.Instance.GetConfig<SnakeLadderTaskRewardConfig>();
    public List<SnakeLadderBuyTurntableConfig> BuyTurntableConfig => SnakeLadderConfigManager.Instance.GetConfig<SnakeLadderBuyTurntableConfig>();
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DebugUtil.LogError("1");
        SnakeLadderConfigManager.Instance.InitConfig(configJson);
        InitTable(StoreItemConfig);
        InitTable(BlockConfig);
        InitTable(CardConfig);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");

        if (CurStorageSnakeLadderWeek != null)
        {
            CurStorageSnakeLadderWeek.StartTime = (long)StartTime;
            CurStorageSnakeLadderWeek.PreheatCompleteTime = (long)StartTime + PreheatTime;
            CurStorageSnakeLadderWeek.EndTime = (long)EndTime;
            CurStorageSnakeLadderWeek.LeaderBoardStorage.EndTime = CurStorageSnakeLadderWeek.EndTime;
            CurStorageSnakeLadderWeek.LeaderBoardStorage.StartTime = CurStorageSnakeLadderWeek.StartTime;
        }
        if (CurStorageSnakeLadderWeek == null && SnakeLadderModel.GetFirstWeekCanGetReward() == null)
        {
            if (!CreateStorage())
            {
                LoopCreateStorage = true;
            }
        }
        _lastActivityOpenState = IsStart();
        
        SnakeLadderLeaderBoardModel.Instance.InitFromServerData();
        TryReleaseUselessStorage();
    }

    public void TryReleaseUselessStorage()
    {
        var releaseWeekList = new List<StorageSnakeLadder>();
        foreach (var pair in StorageSnakeLadder)
        {
            if (pair.Value.IsTimeOut() && (!pair.Value.LeaderBoardStorage.IsInitFromServer() || pair.Value.LeaderBoardStorage.IsFinish))
            {
                releaseWeekList.Add(pair.Value);
            }
        }

        foreach (var releaseWeek in releaseWeekList)
        {
            releaseWeek.TryRelease();
        }
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.SnakeLadder);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() && CurStorageSnakeLadderWeek != null && !CurStorageSnakeLadderWeek.IsTimeOut();
    }

    public bool IsStart()
    {
        return IsPrivateOpened() && APIManager.Instance.GetServerTime() > (IsSkipActivityPreheating()?(ulong)CurStorageSnakeLadderWeek.StartTime:(ulong)CurStorageSnakeLadderWeek.PreheatCompleteTime);
    }

    public void AddTurntable(int addCount,string reason)
    {
        if (!IsStart())
            return;
        CurStorageSnakeLadderWeek.TurntableCount += addCount;
        EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderTurntableCountChange(addCount,CurStorageSnakeLadderWeek.TurntableCount));
        
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSnakeLadderTurntableChange,
            addCount.ToString(),CurStorageSnakeLadderWeek.TurntableCount.ToString(),reason);
    }

    public int GetTurntableCount()
    {
        if (!IsStart())
            return 0;
        return CurStorageSnakeLadderWeek.TurntableCount;
    }

    public bool ReduceTurntableCount(int reduceCount)
    {
        if (!IsStart())
            return false;
        if (CurStorageSnakeLadderWeek.TurntableCount < reduceCount)
            return false;
        CurStorageSnakeLadderWeek.TurntableCount -= reduceCount;
        EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderTurntableCountChange(-reduceCount,CurStorageSnakeLadderWeek.TurntableCount));
        return true;
    }

    public void AddStep(int stepCount, bool triggerStepMulti,bool sendEvent)
    {
        if (!IsStart())
            return;
        if (triggerStepMulti && CurStorageSnakeLadderWeek.StepMultiValue > 1)
        {
            stepCount *= CurStorageSnakeLadderWeek.StepMultiValue;
            ReduceMultiStepCard();
        }

        if (sendEvent)
        {
            EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderUIMoveStep(CurStorageSnakeLadderWeek.CurBlockIndex,stepCount));   
        }
        var curLevel = CurStorageSnakeLadderWeek.GetCurLevel();
        var targetBlockIndex = CurStorageSnakeLadderWeek.CurBlockIndex + stepCount;
        if (targetBlockIndex >= curLevel.GetBlockConfigList().Count-1)
        {
            //通关
            var block = curLevel.GetBlockConfigList().Last();
            if ((SnakeLadderBlockType)block.BlockType != SnakeLadderBlockType.End)
            {
                Debug.LogError("最后一个地块类型错误");
            }
            var reward = CommonUtils.FormatReward(block.RewardId, block.RewardNum);
            var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.SnakeLadderGet);
            UserData.Instance.AddRes(reward,reason);
            var lastCurLevel = CurStorageSnakeLadderWeek.GetCurLevel();
            CurStorageSnakeLadderWeek.CompleteTimes++;
            CurStorageSnakeLadderWeek.CurBlockIndex = 0;
            var newCurLevel = CurStorageSnakeLadderWeek.GetCurLevel();
            if (lastCurLevel != newCurLevel)
            {
                for (var i = 0; i < CurStorageSnakeLadderWeek.TurntableRandomPool.Count; i++)
                {
                    var cardId = CurStorageSnakeLadderWeek.TurntableRandomPool[i];
                    if (!newCurLevel.CardPool().Contains(cardId))
                    {
                        if (cardId > 100)
                        {
                            for (var j = 0; j < newCurLevel.CardPool().Count; j++)
                            {
                                var newCardId = newCurLevel.CardPool()[j];
                                if (newCardId > 100 && newCardId % 100 == cardId % 100)
                                {
                                    CurStorageSnakeLadderWeek.TurntableRandomPool[i] = newCardId;
                                    break;
                                }
                            }   
                        }
                        else
                        {
                            newCurLevel.CardPool().RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderUIGetBlockReward(block,reward));
            EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderLevelUp(CurStorageSnakeLadderWeek.GetCurLevel()));
        }
        else
        {
            CurStorageSnakeLadderWeek.CurBlockIndex = targetBlockIndex;
            var blockIndex = CurStorageSnakeLadderWeek.CurBlockIndex;
            var block = curLevel.GetBlockConfigList()[blockIndex];
            var blockType = (SnakeLadderBlockType) block.BlockType;
            if (blockType == SnakeLadderBlockType.Ladder)
            {
                EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderUIMoveLadder(CurStorageSnakeLadderWeek.CurBlockIndex,block.MoveStep));
                AddStep(block.MoveStep, false,false);
            }
            else if (blockType == SnakeLadderBlockType.Snake)
            {
                if (!ReduceDefenseCard())
                {
                    EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderUIMoveSnake(CurStorageSnakeLadderWeek.CurBlockIndex,block.MoveStep));
                    AddStep(block.MoveStep, false,false);   
                }
            }
            else if (blockType == SnakeLadderBlockType.Score)
            {
                var addScore = block.Score;
                if (CurStorageSnakeLadderWeek.ScoreMultiValue > 1)
                {
                    addScore *= CurStorageSnakeLadderWeek.ScoreMultiValue;
                    ReduceMultiScoreCard();   
                }
                AddScore(addScore,"Block", true);
                EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderUIGetBlockScore(block,addScore,blockIndex));
            }
            else if (blockType == SnakeLadderBlockType.Reward)
            {
                var reward = CommonUtils.FormatReward(block.RewardId, block.RewardNum);
                if (CurStorageSnakeLadderWeek.ScoreMultiValue > 1)
                {
                    foreach (var res in reward)
                    {
                        res.count *= CurStorageSnakeLadderWeek.ScoreMultiValue;
                    }
                    ReduceMultiScoreCard();   
                }
                var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.SnakeLadderGet);
                UserData.Instance.AddRes(reward,reason);
                EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderUIGetBlockReward(block,reward));
            }
        }
    }
    public void AddScore(int addCount,string reason,bool needWait = false )
    {
        if (!IsStart())
            return;
        CurStorageSnakeLadderWeek.Score += addCount;
        CurStorageSnakeLadderWeek.TotalScore += addCount;
        SnakeLadderLeaderBoardModel.Instance.SetStar(CurStorageSnakeLadderWeek.TotalScore);
        EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderScoreChange(addCount,needWait));
        
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSnakeLadderRadishChange,
            addCount.ToString(),CurStorageSnakeLadderWeek.Score.ToString(),reason);
    }

    public int GetScore()
    {
        if (!IsStart())
            return 0;
        return CurStorageSnakeLadderWeek.Score;
    }
    public bool ReduceScore(int reduceCount,string reason)
    {
        if (!IsStart())
            return false;
        if (CurStorageSnakeLadderWeek.Score < reduceCount)
            return false;
        CurStorageSnakeLadderWeek.Score -= reduceCount;
        EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderScoreChange(-reduceCount));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSnakeLadderRadishChange,
            (-reduceCount).ToString(),CurStorageSnakeLadderWeek.Score.ToString(),reason);
        return true;
    }

    public void DebugAddCard(int cardId)
    {
        if (!IsInitFromServer())
            return;
        if (!CardConfig.ContainsKey(cardId))
            return;
        var config = CardConfig[cardId];
        var state = new SnakeLadderCardState(config);
        if (state.CardType == SnakeLadderCardType.Score)
        {
            AddScore(state.Score,"Debug");
        }
        else if (state.CardType == SnakeLadderCardType.Step)
        {
            AddStep(state.Step,true,true);
        }
        else if (state.CardType == SnakeLadderCardType.MultiScore)
        {
            AddMultiScoreCard(state.MultiScore);
        }
        else if (state.CardType == SnakeLadderCardType.MultiStep)
        {
            AddMultiStepCard(state.MultiStep);
        }
        else if (state.CardType == SnakeLadderCardType.Wild)
        {
            AddWildCard();
        }
        else if (state.CardType == SnakeLadderCardType.Defense)
        {
            AddDefenseCard();
        }
    }

    public void AddWildCard(bool autoSendEvent = true)
    {
        if (!IsStart())
            return;
        CurStorageSnakeLadderWeek.WildCardCount++;
        if (autoSendEvent)
        {
            var cardState = new SnakeLadderCardState(SnakeLadderCardType.Wild,0);
            EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderCardCountChange(cardState,1,CurStorageSnakeLadderWeek.WildCardCount));
        }
    }
    public bool ReduceWildCard()
    {
        if (!IsStart())
            return false;
        if (CurStorageSnakeLadderWeek.WildCardCount == 0)
            return false;
        CurStorageSnakeLadderWeek.WildCardCount--;
        var cardState = new SnakeLadderCardState(SnakeLadderCardType.Wild, 0);
        EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderCardCountChange(cardState,-1,CurStorageSnakeLadderWeek.WildCardCount));
        return true;
    }
    
    public void AddDefenseCard(bool autoSendEvent = true)
    {
        if (!IsStart())
            return;
        CurStorageSnakeLadderWeek.DefenseCardCount++;
        if (autoSendEvent)
        {
            var cardState = new SnakeLadderCardState(SnakeLadderCardType.Defense,0);
            EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderCardCountChange(cardState,1,CurStorageSnakeLadderWeek.DefenseCardCount));
        }
    }
    public bool ReduceDefenseCard()
    {
        if (!IsStart())
            return false;
        if (CurStorageSnakeLadderWeek.DefenseCardCount == 0)
            return false;
        CurStorageSnakeLadderWeek.DefenseCardCount--;
        var cardState = new SnakeLadderCardState(SnakeLadderCardType.Defense, 0);
        EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderCardCountChange(cardState,-1,CurStorageSnakeLadderWeek.DefenseCardCount));
        return true;
    }
    
    public void AddMultiStepCard(int multiValue, bool autoSendEvent = true)
    {
        if (!IsStart())
            return;
        var oldValue = CurStorageSnakeLadderWeek.StepMultiValue;
        CurStorageSnakeLadderWeek.StepMultiList.Add(multiValue);
        CurStorageSnakeLadderWeek.StepMultiValue = CurStorageSnakeLadderWeek.StepMultiList[0];
        var newValue = CurStorageSnakeLadderWeek.StepMultiValue;
        if (autoSendEvent)
        {
            EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderStepMultiChange(oldValue,newValue));
        }
    }
    public void ReduceMultiStepCard()
    {
        if (!IsStart())
            return;
        var oldValue = CurStorageSnakeLadderWeek.StepMultiValue;
        CurStorageSnakeLadderWeek.StepMultiList.RemoveAt(0);
        if (CurStorageSnakeLadderWeek.StepMultiList.Count > 0)
        {
            CurStorageSnakeLadderWeek.StepMultiValue = CurStorageSnakeLadderWeek.StepMultiList[0];
        }
        else
        {
            CurStorageSnakeLadderWeek.StepMultiValue = 1;
        }
        var newValue = CurStorageSnakeLadderWeek.StepMultiValue;
        EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderStepMultiChange(oldValue,newValue));
    }
    public void AddMultiScoreCard(int multiValue, bool autoSendEvent = true)
    {
        if (!IsStart())
            return;
        var oldValue = CurStorageSnakeLadderWeek.ScoreMultiValue;
        CurStorageSnakeLadderWeek.ScoreMultiValue += (multiValue-1);
        var newValue = CurStorageSnakeLadderWeek.ScoreMultiValue;
        if (autoSendEvent)
        {
            EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderScoreMultiChange(oldValue,newValue));
        }
    }
    public void ReduceMultiScoreCard()
    {
        if (!IsStart())
            return;
        var oldValue = CurStorageSnakeLadderWeek.ScoreMultiValue;
        CurStorageSnakeLadderWeek.ScoreMultiValue = 1;
        var newValue = CurStorageSnakeLadderWeek.ScoreMultiValue;
        EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderScoreMultiChange(oldValue,newValue));
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
        
        var configs = TaskRewardConfig;
        if (configs != null && configs.Count > 0)
        {
            int value = 0;
            foreach (var config in configs)
            {
                if (tempPrice <= config.Max_value)
                {
                    value = config.Output;
                    break;
                }
            }
            return value;   
        }
        else
        {
            int coin = ((tempPrice/20)+1);
            coin = Math.Min(coin, 8);
            return coin;
        }
    }
    
    private bool _lastActivityOpenState;//记录上一帧的活动开启状态，在轮询中判断是否触发开启活动或者关闭活动

    public SnakeLadderModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }

    private bool LoopCreateStorage;
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;
        if (LoopCreateStorage && IsOpened() && CurStorageSnakeLadderWeek == null)
        {
            if (CreateStorage())
            {
                LoopCreateStorage = false;       
            }
        }

        var currentActivityOpenState = IsStart();
        if (_lastActivityOpenState == currentActivityOpenState)
            return;
        if (!currentActivityOpenState)
        {
            var mainUI = UIManager.Instance.GetOpenedUIByPath<UISnakeLadderMainController>(UINameConst.UISnakeLadderMain);
            if (mainUI)
                mainUI.AnimCloseWindow();
            var previewUI = UIManager.Instance.GetOpenedUIByPath<UIPopupSnakeLadderPreviewController>(UINameConst.UIPopupSnakeLadderPreview);
            if (previewUI)
                previewUI.AnimCloseWindow();
            var shopUI = UIManager.Instance.GetOpenedUIByPath<UISnakeLadderShopController>(UINameConst.UISnakeLadderShop);
            if (shopUI)
                shopUI.AnimCloseWindow();
            var startUI = UIManager.Instance.GetOpenedUIByPath<UIPopupSnakeLadderStartController>(UINameConst.UIPopupSnakeLadderStart);
            if (startUI)
                startUI.AnimCloseWindow();
            CanShowUnCollectRewardsUI();
        }
        else
        {
            var preheatUI = UIManager.Instance.GetOpenedUIByPath<UIPopupSnakeLadderPreviewController>(UINameConst.UIPopupSnakeLadderPreview);
            if (preheatUI)
                preheatUI.AnimCloseWindow();
            // if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home ||
            //     SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome)
            // {
            //     BackHomeControl.PushExtraPopup(new BackHomeControl.AutoPopUI(SnakeLadderModel.CanShowStartPopup,new[] {UINameConst.UIPopupSnakeLadderStart,UINameConst.UISnakeLadderMain}));
            // }
        }
        _lastActivityOpenState = currentActivityOpenState;
    }

    public static StorageSnakeLadder GetFirstWeekCanGetReward()
    {
        foreach (var storageWeekPair in StorageSnakeLadder)
        {
            var storageWeek = storageWeekPair.Value;
            if (storageWeek.IsTimeOut() && storageWeek.LeaderBoardStorage.IsInitFromServer() && !storageWeek.LeaderBoardStorage.IsFinish)
                return storageWeek;
        }
        return null;
    }

    public static bool CanShowUnCollectRewardsUI()
    {
        return SnakeLadderLeaderBoardModel.CanShowUnCollectRewardsUI();
    }
    public static bool CanShowMainPopup()
    {
        if (Instance.IsStart())
        {
            UISnakeLadderMainController.Open(Instance.CurStorageSnakeLadderWeek);
            return true;
        }
        return false;
    }

    public static bool CanShowPreheatPopup()
    {
        if (Instance.IsPrivateOpened() && !Instance.IsStart())
        {
            UIPopupSnakeLadderPreviewController.Open(Instance.CurStorageSnakeLadderWeek);
            return true;
        }
        return false;
    }

    public const string coolTimeKey = "SnakeLadder";
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

    public static bool CanShowStartPopup()
    {
        if (Instance.IsStart() && !Instance.CurStorageSnakeLadderWeek.IsStart)
        {
            // if ((SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome || SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home) && 
            //     !GuideSubSystem.Instance.IsShowingGuide() && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SnakeLadderHomeEntrance))
            // {
            //     GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SnakeLadderHomeEntrance, null);
            //     return true;
            // }
            Instance.CurStorageSnakeLadderWeek.IsStart = true;
            UIPopupSnakeLadderStartController.Open(Instance.CurStorageSnakeLadderWeek);
            return true;
        }
        return false;
    }



    public static void FlyStar(int rewardNum, Vector2 srcPos, Transform starTransform, float time, bool showEffect,
        Action action = null)
    {
        Transform target = starTransform;
        int count = Math.Min(rewardNum, 10);
        float delayTime = 0.3f;
        if (count >= 5)
            delayTime = 0.1f;
        for (int i = 0; i < count; i++)
        {
            int index = i;

            Vector3 position = target.position;

            FlyGameObjectManager.Instance.FlyObject(target.gameObject, srcPos, position, showEffect, time,
                delayTime * i, () =>
                {
                    FlyGameObjectManager.Instance.PlayHintStarsEffect(position);
                    ShakeManager.Instance.ShakeLight();
                    if (index == count - 1)
                    {
                        action?.Invoke();
                    }
                });
        }
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.SnakeLadder);
    }
}