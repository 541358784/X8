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
using DragonPlus.Config.Easter2024;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class Easter2024Model : ActivityEntityBase
{
    public int DebugBigBall = 1;
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/Donut/Aux_Donut";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/Donut/TaskList_Donut";
    }

    public bool ShowEntrance()
    {
        return IsStart();
    }
    private static Easter2024Model _instance;
    public static Easter2024Model Instance => _instance ?? (_instance = new Easter2024Model());

    public override string Guid => "OPS_EVENT_TYPE_EASTER_2024";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public static StorageDictionary<string,StorageEaster2024> StorageEaster2024 => StorageManager.Instance.GetStorage<StorageHome>().Easter2024;

    public StorageEaster2024 CurStorageEaster2024Week
    {
        get
        {
            if (ActivityId == null)
                return null;
            StorageEaster2024.TryGetValue(ActivityId, out StorageEaster2024 curWeek);
            return curWeek;
        }
    }

    public bool CreateStorage()
    {
        DebugUtil.LogError("CreateStorage3");
        if (!SceneFsm.mInstance.ClientInited)
            return false;
        if (CurStorageEaster2024Week == null && IsInitFromServer()
            && Easter2024Model.GetFirstWeekCanGetReward() == null
            && IsOpened())
        {
            DebugUtil.LogError("CreateStorage4");
            var newWeek = new StorageEaster2024()
            {
                ActivityId = ActivityId,
                BallCount = 0,
                ExtraBallList = {},
                MultiBallList = {},
                StartTime = (long) StartTime,
                PreheatCompleteTime = (long)StartTime+PreheatTime,
                EndTime = (long) EndTime,
                ActivityResList = {},
                FinishStoreItemList = {},
                LuckyPointCount = 0,
                Score = 0,
                IsFinish = false,
            };
            var resMd5List = ActivityManager.Instance.GetActivityMd5List(ActivityId);
            newWeek.ActivityResList.Clear();
            foreach (var resMd5 in resMd5List)
            {
                var resPath = ActivityResHotUpdate.GetFilePath(resMd5);
                newWeek.ActivityResList.Add(resPath);
            }

            for (var i = 0; i < StoreLevelConfig.Count; i++)
            {
                var levelCfg = StoreLevelConfig[i];
                for (var i1 = 0; i1 < levelCfg.StoreItemList.Count; i1++)
                {
                    var storeItemCfg = StoreItemConfig[levelCfg.StoreItemList[i1]];
                    if ((Easter2024StoreItemType)storeItemCfg.Type == Easter2024StoreItemType.BuildItem)
                    {
                        var isOwned = true;
                        for (var i2 = 0; i2 < storeItemCfg.RewardId.Count; i2++)
                        {
                            var decoItem = DecoWorld.ItemLib[storeItemCfg.RewardId[i2]];
                            if (!decoItem.IsOwned)
                            {
                                isOwned = false;
                                break;
                            }
                        }
                        if (isOwned)
                        {
                            newWeek.FinishStoreItemList.Add(storeItemCfg.Id);
                        }
                    }
                }
            }
            newWeek.CardRandomPool.Clear();
            var curLevel = newWeek.GetCurLevel();
            for (var i = 0; i < curLevel.CardPool.Count; i++)
            {
                newWeek.CardRandomPool.Add(curLevel.CardPool[i]);
            }
            newWeek.UnLockStoreLevel.Add(1);
            StorageEaster2024.Add(ActivityId, newWeek);
            Easter2024LeaderBoardModel.Instance.CreateStorage(newWeek.LeaderBoardStorage);
            return true;
        }

        return false;
    }
    public Easter2024GlobalConfig GlobalConfig => Easter2024ConfigManager.Instance.GetConfig<Easter2024GlobalConfig>()[0];
    public long PreheatTime=> (long)(GlobalConfig.PreheatTime * XUtility.Hour);
    private static void InitTable<T>(Dictionary<int, T> config) where T : TableBase
    {
        if (config == null)
            return;

        List<T> tableData = Easter2024ConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }
    public List<Easter2024LevelConfig> LevelConfig => Easter2024ConfigManager.Instance.GetConfig<Easter2024LevelConfig>();
    public List<Easter2024StoreLevelConfig> StoreLevelConfig => Easter2024ConfigManager.Instance.GetConfig<Easter2024StoreLevelConfig>();
    public Dictionary<int, Easter2024StoreItemConfig> StoreItemConfig = new Dictionary<int, Easter2024StoreItemConfig>();
    public Dictionary<int, Easter2024MiniGameConfig> MiniGameConfig = new Dictionary<int, Easter2024MiniGameConfig>();
    public Dictionary<int, Easter2024CardConfig> CardConfig = new Dictionary<int, Easter2024CardConfig>();
    public List<Easter2024LeaderBoardRewardConfig> LeaderBoardRewardConfig=> Easter2024ConfigManager.Instance.GetConfig<Easter2024LeaderBoardRewardConfig>();
    public List<Easter2024TaskRewardConfig> TaskRewardConfig => Easter2024ConfigManager.Instance.GetConfig<Easter2024TaskRewardConfig>();

    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DebugUtil.LogError("1");
        Easter2024ConfigManager.Instance.InitConfig(configJson);
        InitTable(StoreItemConfig);
        InitTable(MiniGameConfig);
        InitTable(CardConfig);
        DropBallGame.BallSpeedXValue = GlobalConfig.BallMoveSpeed;
        DropBallGame.LuckyNodeSpeedXValue = GlobalConfig.LuckyMoveSpeed;
        DropBallGame.GravityScale = GlobalConfig.GravityScale;
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
        TryReleaseUselessStorage();
        
        if (CurStorageEaster2024Week == null && Easter2024Model.GetFirstWeekCanGetReward() == null)
        {
            if (!CreateStorage())
            {
                LoopCreateStorage = true;
            }
        }
        _lastActivityOpenState = IsStart();

        if (CurStorageEaster2024Week != null)
        {
            CurStorageEaster2024Week.StartTime = (long)StartTime;
            CurStorageEaster2024Week.PreheatCompleteTime = (long)StartTime + PreheatTime;
            CurStorageEaster2024Week.EndTime = (long)EndTime;
            if (CurStorageEaster2024Week.LeaderBoardStorage != null)
            {
                CurStorageEaster2024Week.LeaderBoardStorage.StartTime = (long)StartTime;
                CurStorageEaster2024Week.LeaderBoardStorage.EndTime = (long)EndTime;
            }
        }
        Easter2024LeaderBoardModel.Instance.InitFromServerData();
    }

    public void TryReleaseUselessStorage()
    {
        var releaseWeekList = new List<StorageEaster2024>();
        foreach (var pair in StorageEaster2024)
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

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Easter2024);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() && CurStorageEaster2024Week != null && !CurStorageEaster2024Week.IsTimeOut();
    }

    public bool IsStart()
    {
        return IsPrivateOpened() && APIManager.Instance.GetServerTime() > (IsSkipActivityPreheating()?(ulong)CurStorageEaster2024Week.StartTime:(ulong)CurStorageEaster2024Week.PreheatCompleteTime);
    }

    public void AddEgg(int addCount,string reason)
    {
        if (!IsStart())
            return;
        CurStorageEaster2024Week.BallCount += addCount;
        EventDispatcher.Instance.SendEvent(new EventEaster2024EggCountChange(addCount));
        
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterEggChange,
            addCount.ToString(),CurStorageEaster2024Week.BallCount.ToString(),reason);
    }

    public int GetEgg()
    {
        if (!IsStart())
            return 0;
        return CurStorageEaster2024Week.BallCount;
    }

    public Dictionary<DropBallGame.Ball, int> BILeftBallCount = new Dictionary<DropBallGame.Ball, int>();
    public bool ReduceEgg(int reduceCount)
    {
        if (!IsStart())
            return false;
        if (CurStorageEaster2024Week.BallCount < reduceCount)
            return false;
        CurStorageEaster2024Week.BallCount -= reduceCount;
        EventDispatcher.Instance.SendEvent(new EventEaster2024EggCountChange(-reduceCount));
        return true;
    }
    public void AddScore(int addCount,string reason,bool needWait = false )
    {
        if (!IsStart())
            return;
        CurStorageEaster2024Week.Score += addCount;
        var lastCurLevel = CurStorageEaster2024Week.GetCurLevel();
        CurStorageEaster2024Week.TotalScore += addCount;
        var newCurLevel = CurStorageEaster2024Week.GetCurLevel();
        if (lastCurLevel != newCurLevel)
        {
            for (var i = 0; i < CurStorageEaster2024Week.CardRandomPool.Count; i++)
            {
                var cardId = CurStorageEaster2024Week.CardRandomPool[i];
                if (!newCurLevel.CardPool.Contains(cardId))
                {
                    if (cardId > 100)
                    {
                        for (var j = 0; j < newCurLevel.CardPool.Count; j++)
                        {
                            var newCardId = newCurLevel.CardPool[j];
                            if (newCardId > 100 && newCardId % 100 == cardId % 100)
                            {
                                CurStorageEaster2024Week.CardRandomPool[i] = newCardId;
                                break;
                            }
                        }   
                    }
                    else
                    {
                        newCurLevel.CardPool.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        Easter2024LeaderBoardModel.Instance.SetStar(CurStorageEaster2024Week.TotalScore);
        EventDispatcher.Instance.SendEvent(new EventEaster2024ScoreChange(addCount,needWait));
        
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterRadishChange,
            addCount.ToString(),CurStorageEaster2024Week.Score.ToString(),reason);
    }

    public int GetScore()
    {
        if (!IsStart())
            return 0;
        return CurStorageEaster2024Week.Score;
    }
    public bool ReduceScore(int reduceCount,string reason)
    {
        if (!IsStart())
            return false;
        if (CurStorageEaster2024Week.Score < reduceCount)
            return false;
        CurStorageEaster2024Week.Score -= reduceCount;
        EventDispatcher.Instance.SendEvent(new EventEaster2024ScoreChange(-reduceCount));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterRadishChange,
            (-reduceCount).ToString(),CurStorageEaster2024Week.Score.ToString(),reason);
        return true;
    }

    public void DebugAddCard(int cardId)
    {
        if (!IsInitFromServer())
            return;
        if (!CardConfig.ContainsKey(cardId))
            return;
        var config = CardConfig[cardId];
        var state = new Easter2024CardState(config);
        if (state.CardType == Easter2024CardType.Score)
        {
            AddScore(state.Score,"Debug");
        }
        else if (state.CardType == Easter2024CardType.ExtraBall)
        {
            AddExtraBallCard(state.BallCount);
        }
        else if (state.CardType == Easter2024CardType.MultiScore)
        {
            AddMultiScoreCard(state.MultiValue);
        }
    }

    public void AddExtraBallCard(int ballCount, bool autoSendEvent = true)
    {
        if (!IsStart())
            return;
        CurStorageEaster2024Week.ExtraBallList.Add(ballCount);

        if (autoSendEvent)
        {
            var cardState = new Easter2024CardState(Easter2024CardType.ExtraBall, ballCount);
            EventDispatcher.Instance.SendEvent(new EventEaster2024CardCountChange(cardState,1));
        }
    }
    public bool ReduceExtraBallCard(int ballCount)
    {
        if (!IsStart())
            return false;
        for (var i = 0; i < CurStorageEaster2024Week.ExtraBallList.Count; i++)
        {
            if (CurStorageEaster2024Week.ExtraBallList[i] == ballCount)
            {
                CurStorageEaster2024Week.ExtraBallList.RemoveAt(i);
                var cardState = new Easter2024CardState(Easter2024CardType.ExtraBall, ballCount);
                EventDispatcher.Instance.SendEvent(new EventEaster2024CardCountChange(cardState,-1));
                return true;
            }
        }
        return false;
    }
    public void AddMultiScoreCard(int multiValue, bool autoSendEvent = true)
    {
        if (!IsStart())
            return;
        CurStorageEaster2024Week.MultiBallList.Add(multiValue);
        
        if (autoSendEvent)
        {
            var cardState = new Easter2024CardState(Easter2024CardType.MultiScore, multiValue);
            EventDispatcher.Instance.SendEvent(new EventEaster2024CardCountChange(cardState,1));
        }
    }
    public bool ReduceMultiScoreCard(int multiValue)
    {
        if (!IsStart())
            return false;
        for (var i = 0; i < CurStorageEaster2024Week.MultiBallList.Count; i++)
        {
            if (CurStorageEaster2024Week.MultiBallList[i] == multiValue)
            {
                CurStorageEaster2024Week.MultiBallList.RemoveAt(i);
                var cardState = new Easter2024CardState(Easter2024CardType.MultiScore, multiValue);
                EventDispatcher.Instance.SendEvent(new EventEaster2024CardCountChange(cardState,-1));
                return true;
            }
        }
        return false;
    }

    public int MiniGameNeedLuckyPointCount => GlobalConfig.LuckyPointCount;
    public bool AddLuckyPoint(int luckyPointCount)
    {
        if (!IsStart())
            return false;
        var oldValue = CurStorageEaster2024Week.LuckyPointCount;
        CurStorageEaster2024Week.LuckyPointCount += luckyPointCount;
        if (CurStorageEaster2024Week.LuckyPointCount > MiniGameNeedLuckyPointCount)
        {
            CurStorageEaster2024Week.LuckyPointCount = MiniGameNeedLuckyPointCount;
        }
        EventDispatcher.Instance.SendEvent(new EventEaster2024LuckyPointCountChange(luckyPointCount));
        return CurStorageEaster2024Week.LuckyPointCount != oldValue;
    }
    public void ReduceLuckyPoint(int luckyPointCount)
    {
        if (!IsStart())
            return;
        CurStorageEaster2024Week.LuckyPointCount -= luckyPointCount;
        EventDispatcher.Instance.SendEvent(new EventEaster2024LuckyPointCountChange(-luckyPointCount));
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

    public Easter2024Model()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }

    private bool LoopCreateStorage;
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;
        if (LoopCreateStorage && IsOpened() && CurStorageEaster2024Week == null)
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
            var mainUI = UIManager.Instance.GetOpenedUIByPath<UIEaster2024MainController>(UINameConst.UIEaster2024Main);
            if (mainUI)
                mainUI.AnimCloseWindow();
            var getCardUI = UIManager.Instance.GetOpenedUIByPath<UIEaster2024MiniGameRewardController>(UINameConst.UIEaster2024MiniGameReward);
            if (getCardUI)
                getCardUI.AnimCloseWindow();
            var miniGameUI = UIManager.Instance.GetOpenedUIByPath<UIPopupEaster2024MiniGameController>(UINameConst.UIPopupEaster2024MiniGame);
            if (miniGameUI)
                miniGameUI.AnimCloseWindow();
            var previewUI = UIManager.Instance.GetOpenedUIByPath<UIPopupEaster2024PreviewController>(UINameConst.UIPopupEaster2024Preview);
            if (previewUI)
                previewUI.AnimCloseWindow();
            var shopUI = UIManager.Instance.GetOpenedUIByPath<UIEaster2024ShopController>(UINameConst.UIEaster2024Shop);
            if (shopUI)
                shopUI.AnimCloseWindow();
            var startUI = UIManager.Instance.GetOpenedUIByPath<UIPopupEaster2024StartController>(UINameConst.UIPopupEaster2024Start);
            if (startUI)
                startUI.AnimCloseWindow();
            CanShowUnCollectRewardsUI();
        }
        else
        {
            var preheatUI = UIManager.Instance.GetOpenedUIByPath<UIPopupEaster2024PreviewController>(UINameConst.UIPopupEaster2024Preview);
            if (preheatUI)
                preheatUI.AnimCloseWindow();
            // if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home ||
            //     SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome)
            // {
            //     BackHomeControl.PushExtraPopup(new BackHomeControl.AutoPopUI(Easter2024Model.CanShowStartPopup,new[] {UINameConst.UIPopupEaster2024Start,UINameConst.UIEaster2024Main}));
            // }
        }
        _lastActivityOpenState = currentActivityOpenState;
    }

    public static StorageEaster2024 GetFirstWeekCanGetReward()
    {
        foreach (var storageWeekPair in StorageEaster2024)
        {
            var storageWeek = storageWeekPair.Value;
            if (storageWeek.IsTimeOut() && storageWeek.LeaderBoardStorage.IsInitFromServer() && !storageWeek.LeaderBoardStorage.IsFinish)
                return storageWeek;
        }
        return null;
    }

    public static bool CanShowUnCollectRewardsUI()
    {
        return Easter2024LeaderBoardModel.CanShowUnCollectRewardsUI();
    }
    public static bool CanShowMainPopup()
    {
        if (Instance.IsStart())
        {
            UIEaster2024MainController.Open(Instance.CurStorageEaster2024Week);
            return true;
        }
        return false;
    }

    public static bool CanShowPreheatPopup()
    {
        if (Instance.IsPrivateOpened() && !Instance.IsStart())
        {
            UIPopupEaster2024PreviewController.Open(Instance.CurStorageEaster2024Week);
            return true;
        }
        return false;
    }

    public const string coolTimeKey = "Easter2024";
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
        if (Instance.IsStart() &&
            (SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome ||
                               SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home) &&
            !GuideSubSystem.Instance.IsShowingGuide() &&
            !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.Easter2024GuideStart))
        {
            var auxItem = Aux_Easter2024.Instance;
            if (!auxItem)
                return false;
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(auxItem.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Easter2024GuideStart, auxItem.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Easter2024GuideStart, null))
            {
                Instance.CurStorageEaster2024Week.IsStart = true;
                Easter2024Model.Instance.AddEgg(5,"GuideSendBall");
                return true;
            }
        }
        else
        {
            if (Instance.IsStart() && !Instance.CurStorageEaster2024Week.IsStart)
            {
                Instance.CurStorageEaster2024Week.IsStart = true;
                UIPopupEaster2024StartController.Open(Instance.CurStorageEaster2024Week);
                return true;
            }
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
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.Easter2024);
    }
}