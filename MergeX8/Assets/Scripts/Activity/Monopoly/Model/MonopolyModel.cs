using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dynamic;
using Activity.Monopoly.View;
using Deco.Node;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonPlus.Config.Monopoly;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class MonopolyModel : ActivityEntityBase
{
    private static MonopolyModel _instance;
    public static MonopolyModel Instance => _instance ?? (_instance = new MonopolyModel());

    public override string Guid => "OPS_EVENT_TYPE_MONOPOLY";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public static StorageDictionary<string,StorageMonopoly> StorageMonopoly => StorageManager.Instance.GetStorage<StorageHome>().Monopoly;

    public StorageMonopoly CurStorageMonopolyWeek
    {
        get
        {
            if (ActivityId == null)
                return null;
            StorageMonopoly.TryGetValue(ActivityId, out StorageMonopoly curWeek);
            return curWeek;
        }
    }

    public bool CreateStorage()
    {
        if (!SceneFsm.mInstance.ClientInited)
            return false;
        if (CurStorageMonopolyWeek == null && IsInitFromServer()
            && MonopolyModel.GetFirstWeekCanGetReward() == null
            && IsOpened())
        {
            var newWeek = new StorageMonopoly()
            {
                ActivityId = ActivityId,
                PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().MonopolyGroupId,
                ScoreMultiList = {  },
                StepMultiList = {  },
                WildCardCount = 0,
                StartTime = (long) StartTime,
                PreheatCompleteTime = (long)StartTime+PreheatTime,
                EndTime = (long) EndTime,
                ActivityResList = {},
                FinishStoreItemList = {},
                Score = 0,
                IsStart = false,
                TotalScore = 0,
                DiceRandomPool = {},
                CardRandomPool = {},
                UnLockStoreLevel = {1},
                CompleteTimes = 0,
                RewardBoxCompleteTimes = 0,
                RewardBoxCollectNum = 0,
                CurBlockIndex = 0,
                DiceBuyState = {  },
                BlockBuyState = {  },
                CurBlockBuyState = false,
                UnFinishedMiniGameConfigId = 0,
            };
            StorageMonopoly.Add(ActivityId, newWeek);
            InitConfigAfterInitStorage();
            newWeek.DiceCount = GlobalConfig.StartDice;
            var resMd5List = ActivityManager.Instance.GetActivityMd5List(ActivityId);
            newWeek.ActivityResList.Clear();
            newWeek.ActivityResMd5List.Clear();
            foreach (var resMd5 in resMd5List)
            {
                newWeek.ActivityResMd5List.Add(resMd5);
                var resPath = ActivityResHotUpdate.GetFilePath(resMd5);
                newWeek.ActivityResList.Add(resPath);
            }

            // for (var i = 0; i < StoreLevelConfig.Count; i++)
            // {
            //     var levelCfg = StoreLevelConfig[i];
            //     for (var i1 = 0; i1 < levelCfg.StoreItemList.Count; i1++)
            //     {
            //         var storeItemCfg = StoreItemConfig[levelCfg.StoreItemList[i1]];
            //         if ((MonopolyStoreItemType)storeItemCfg.Type == MonopolyStoreItemType.BuildItem)
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
            newWeek.DiceRandomPool.Clear();
            for (var i = 0; i < DicePool.Count; i++)
            {
                newWeek.DiceRandomPool.Add(DicePool[i]);
            }
            newWeek.CardRandomPool.Clear();
            for (var i = 0; i < CardPool.Count; i++)
            {
                newWeek.CardRandomPool.Add(CardPool[i]);
            }
            MonopolyLeaderBoardModel.Instance.CreateStorage(newWeek);
            return true;
        }

        return false;
    }

    public List<int> CardPool;
    public List<int> DicePool;
    public List<MonopolyBlockConfig> BlockConfigList;
    public MonopolyGlobalConfig GlobalConfig => MonopolyGlobalConfigList[0];
    public long PreheatTime=> IsSkipActivityPreheating()?0:(long)(GlobalConfig.PreheatTime * XUtility.Hour);
    private static void InitTable<T>(Dictionary<int, T> config,List<T> tableData = null) where T : TableBase
    {
        if (config == null)
            return;
        if (tableData == null)
            tableData = MonopolyConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }
    
    public Dictionary<int, MonopolyBlockConfig> BlockConfig = new Dictionary<int, MonopolyBlockConfig>();
    public List<MonopolyRewardBoxConfig> RewardBoxConfig => MonopolyRewardBoxConfigList;
    public Dictionary<int, MonopolyMiniGameConfig> MiniGameConfig = new Dictionary<int, MonopolyMiniGameConfig>();
    public List<MonopolyStoreLevelConfig> StoreLevelConfig => MonopolyStoreLevelConfigList;
    public Dictionary<int, MonopolyStoreItemConfig> StoreItemConfig = new Dictionary<int, MonopolyStoreItemConfig>();
    public Dictionary<int, MonopolyCardConfig> CardConfig = new Dictionary<int, MonopolyCardConfig>();
    public Dictionary<int, MonopolyDiceConfig> DiceConfig = new Dictionary<int, MonopolyDiceConfig>();
    public List<MonopolyLeaderBoardRewardConfig> LeaderBoardRewardConfig=> MonopolyLeaderBoardRewardConfigList;
    public List<MonopolyTaskRewardConfig> TaskRewardConfig => MonopolyTaskRewardConfigList;
    public List<MonopolyBuyDiceConfig> BuyDiceConfig => MonopolyBuyDiceConfigList;
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DebugUtil.LogError("1");
        MonopolyConfigManager.Instance.InitConfig(configJson);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
        TryReleaseUselessStorage();
        
        if (CurStorageMonopolyWeek != null)
        {
            CurStorageMonopolyWeek.StartTime = (long) StartTime;
            CurStorageMonopolyWeek.PreheatCompleteTime = (long) StartTime + PreheatTime;
            CurStorageMonopolyWeek.EndTime = (long) EndTime;
            MonopolyLeaderBoardModel.Instance.CreateStorage(CurStorageMonopolyWeek);
            InitConfigAfterInitStorage();
        }
        if (CurStorageMonopolyWeek == null && MonopolyModel.GetFirstWeekCanGetReward() == null)
        {
            if (!CreateStorage())
            {
                LoopCreateStorage = true;
            }
        }
        _lastActivityOpenState = IsStart();
        
        MonopolyLeaderBoardModel.Instance.InitFromServerData();
    }

    public void InitConfigAfterInitStorage()
    {
        InitTable(StoreItemConfig,MonopolyStoreItemConfigList);
        InitTable(MiniGameConfig,MonopolyMiniGameConfigList);
        InitTable(BlockConfig,MonopolyBlockConfigList);
        InitTable(CardConfig,MonopolyCardConfigList);
        InitTable(DiceConfig,MonopolyDiceConfigList);
        CardPool = new List<int>();
        for (var j=0;j<GlobalConfig.CardPoolId.Count;j++)
        {
            var cardId = GlobalConfig.CardPoolId[j];
            var count = GlobalConfig.CardPoolNum[j];
            for (var i = 0; i < count; i++)
            {
                CardPool.Add(cardId);
            }
        }
        DicePool = new List<int>();
        for (var j=0;j<GlobalConfig.DicePoolId.Count;j++)
        {
            var cardId = GlobalConfig.DicePoolId[j];
            var count = GlobalConfig.DicePoolNum[j];
            for (var i = 0; i < count; i++)
            {
                DicePool.Add(cardId);
            }
        }
        BlockConfigList = new List<MonopolyBlockConfig>();
        for (var i = 0; i < GlobalConfig.BlockList.Count; i++)
        {
            BlockConfigList.Add(BlockConfig[GlobalConfig.BlockList[i]]);
        }
    }
    public void TryReleaseUselessStorage()
    {
        var releaseWeekList = new List<StorageMonopoly>();
        foreach (var pair in StorageMonopoly)
        {
            if (pair.Value.IsTimeOut())
            {
                releaseWeekList.Add(pair.Value);
            }
        }
        foreach (var releaseWeek in releaseWeekList)
        {
            releaseWeek.TryRelease();
        }
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Monopoly);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() && CurStorageMonopolyWeek != null && !CurStorageMonopolyWeek.IsTimeOut();
    }

    public bool IsStart()
    {
        return IsPrivateOpened() && APIManager.Instance.GetServerTime() > (ulong)CurStorageMonopolyWeek.PreheatCompleteTime;
    }

    public void AddDice(int addCount,string reason)
    {
        if (!IsStart())
            return;
        CurStorageMonopolyWeek.DiceCount += addCount;
        EventDispatcher.Instance.SendEventImmediately(new EventMonopolyDiceCountChange(addCount,CurStorageMonopolyWeek.DiceCount));
        
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonopolyDiceChange,
            addCount.ToString(),CurStorageMonopolyWeek.DiceCount.ToString(),reason);
    }

    public int GetDiceCount()
    {
        if (!IsStart())
            return 0;
        return CurStorageMonopolyWeek.DiceCount;
    }

    public bool ReduceDiceCount(int reduceCount)
    {
        if (!IsStart())
            return false;
        if (CurStorageMonopolyWeek.DiceCount < reduceCount)
            return false;
        CurStorageMonopolyWeek.DiceCount -= reduceCount;
        EventDispatcher.Instance.SendEventImmediately(new EventMonopolyDiceCountChange(-reduceCount,CurStorageMonopolyWeek.DiceCount));
        return true;
    }
    

    public int GetScore()
    {
        if (!IsStart())
            return 0;
        return CurStorageMonopolyWeek.Score;
    }
    public bool ReduceScore(int reduceCount,string reason)
    {
        if (!IsStart())
            return false;
        if (CurStorageMonopolyWeek.Score < reduceCount)
            return false;
        CurStorageMonopolyWeek.Score -= reduceCount;
        EventDispatcher.Instance.SendEventImmediately(new EventMonopolyScoreChange(-reduceCount));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonopolyRadishChange,
            (-reduceCount).ToString(),CurStorageMonopolyWeek.Score.ToString(),reason);
        return true;
    }

    public void DebugAddCard(int cardId)
    {
        if (!IsInitFromServer())
            return;
        if (!IsStart())
            return;
        if (!CardConfig.ContainsKey(cardId))
            return;
        var config = CardConfig[cardId];
        var state = new MonopolyCardState(config);
        if (state.CardType == MonopolyCardType.Score)
        {
            CurStorageMonopolyWeek.AddScore(state.Score,"Debug");
        }
        else if (state.CardType == MonopolyCardType.MultiScore)
        {
            CurStorageMonopolyWeek.AddMultiScoreCard(state.MultiScore);
        }
        else if (state.CardType == MonopolyCardType.MultiStep)
        {
            CurStorageMonopolyWeek.AddMultiStepCard(state.MultiStep);
        }
        else if (state.CardType == MonopolyCardType.Wild)
        {
            CurStorageMonopolyWeek.AddWildCard();
        }
    }
    

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
    
    private bool _lastActivityOpenState;//记录上一帧的活动开启状态，在轮询中判断是否触发开启活动或者关闭活动

    public MonopolyModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }

    private bool LoopCreateStorage;
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;
        if (LoopCreateStorage && IsOpened() && CurStorageMonopolyWeek == null)
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
            if (UIMonopolyMainController.Instance)
                UIMonopolyMainController.Instance.AnimCloseWindow();
            if (UIMonopolyShopController.Instance)
                UIMonopolyShopController.Instance.AnimCloseWindow();
            CanShowUnCollectRewardsUI();
        }
        else
        {
            var preheatUI = UIManager.Instance.GetOpenedUIByPath<UIPopupMonopolyPreviewController>(UINameConst.UIPopupMonopolyPreview);
            if (preheatUI)
                preheatUI.AnimCloseWindow();
            // if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home ||
            //     SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome)
            // {
            //     BackHomeControl.PushExtraPopup(new BackHomeControl.AutoPopUI(MonopolyModel.CanShowStartPopup,new[] {UINameConst.UIPopupMonopolyStart,UINameConst.UIMonopolyMain}));
            // }
        }
        _lastActivityOpenState = currentActivityOpenState;
    }

    public static StorageMonopoly GetFirstWeekCanGetReward()
    {
        return null;
    }

    public static bool CanShowUnCollectRewardsUI()
    {
        return false;
    }
    public static bool CanShowMainPopup()
    {
        if (Instance.IsStart())
        {
            UIMonopolyMainController.Open(Instance.CurStorageMonopolyWeek);
            return true;
        }
        return false;
    }

    public static bool CanShowPreheatPopup()
    {
        if (Instance.IsPrivateOpened() && !Instance.IsStart())
        {
            UIPopupMonopolyPreviewController.Open(Instance.CurStorageMonopolyWeek);
            return true;
        }
        return false;
    }

    public const string preheatCoolTimeKey = "MonopolyPreheat";
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

    public static bool CanStartGuide()
    {
        if (Instance.IsStart() &&
            (SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome || SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home) && 
            !GuideSubSystem.Instance.IsShowingGuide() && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MonopolyAuxItem))
        {
            var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_Monopoly>();
            if (auxItem != null)
            {
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(auxItem.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MonopolyAuxItem, auxItem.transform as RectTransform,
                    topLayer: topLayer);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MonopolyAuxItem, null))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public static bool CanShowStartPopup()
    {
        if (Instance.IsStart() && !Instance.CurStorageMonopolyWeek.IsStart)
        {
            Instance.CurStorageMonopolyWeek.IsStart = true;
            // UIPopupMonopolyStartController.Open(Instance.CurStorageMonopolyWeek);
            // return true;
        }
        return false;
    }
    
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.Monopoly);
    }

    public Transform GetFlyTarget()
    {
        var storage = CurStorageMonopolyWeek;
        if (UIMonopolyMainController.Instance)
        {
            return UIMonopolyMainController.Instance.Roller.transform;
        }
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_Monopoly>();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_Monopoly>();
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }

    public bool ShowEntrance()
    {
        if (CurStorageMonopolyWeek == null)
            return false;

        return MonopolyUtils.ShowEntrance(CurStorageMonopolyWeek);
    }
}