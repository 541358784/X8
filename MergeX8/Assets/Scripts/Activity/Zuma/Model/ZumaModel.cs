using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dynamic;
using Activity.Zuma.View;
using Deco.Node;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonPlus.Config.Zuma;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class ZumaModel : ActivityEntityBase
{
    public bool ShowEntrance()
    {
        return IsStart();
    }
    private static ZumaModel _instance;
    public static ZumaModel Instance => _instance ?? (_instance = new ZumaModel());

    public override string Guid => "OPS_EVENT_TYPE_ZUMA";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public static StorageDictionary<string,StorageZuma> StorageZuma => StorageManager.Instance.GetStorage<StorageHome>().Zuma;

    public StorageZuma CurStorageZumaWeek
    {
        get
        {
            if (ActivityId == null)
                return null;
            StorageZuma.TryGetValue(ActivityId, out StorageZuma curWeek);
            return curWeek;
        }
    }

    public bool CreateStorage()
    {
        if (!SceneFsm.mInstance.ClientInited)
            return false;
        if (CurStorageZumaWeek == null && IsInitFromServer()
            && ZumaModel.GetFirstWeekCanGetReward() == null
            && IsOpened())
        {
            var newWeek = new StorageZuma()
            {
                ActivityId = ActivityId,
                BallCount = GlobalConfig.StartDice,
                StartTime = (long) StartTime,
                PreheatCompleteTime = (long)StartTime+PreheatTime,
                EndTime = (long) EndTime,
                ActivityResList = {},
                FinishStoreItemList = {},
                Score = 0,
                IsStart = false,
                TotalScore = 0,
                UnLockStoreLevel = {1},
                CompleteTimes = 0,
                LevelId = 0,
            };
            var resMd5List = ActivityManager.Instance.GetActivityMd5List(ActivityId);
            newWeek.ActivityResList.Clear();
            newWeek.ActivityResMd5List.Clear();
            foreach (var resMd5 in resMd5List)
            {
                newWeek.ActivityResMd5List.Add(resMd5);
                var resPath = ActivityResHotUpdate.GetFilePath(resMd5);
                newWeek.ActivityResList.Add(resPath);
            }


            var firstLevel = newWeek.GetNextLevel();
            newWeek.StartLevel(firstLevel);
            
            // for (var i = 0; i < StoreLevelConfig.Count; i++)
            // {
            //     var levelCfg = StoreLevelConfig[i];
            //     for (var i1 = 0; i1 < levelCfg.StoreItemList.Count; i1++)
            //     {
            //         var storeItemCfg = StoreItemConfig[levelCfg.StoreItemList[i1]];
            //         if ((ZumaStoreItemType)storeItemCfg.Type == ZumaStoreItemType.BuildItem)
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
            StorageZuma.Add(ActivityId, newWeek);
            ZumaLeaderBoardModel.Instance.CreateStorage(newWeek);
            return true;
        }

        return false;
    }
    
    
    public ZumaGlobalConfig GlobalConfig => ZumaConfigManager.Instance.GetConfig<ZumaGlobalConfig>()[0];
    public long PreheatTime=> IsSkipActivityPreheating()?0:(long)((ulong)GlobalConfig.PreheatTime * XUtility.Hour);
    private static void InitTable<T>(Dictionary<int, T> config) where T : TableBase
    {
        if (config == null)
            return;

        List<T> tableData = ZumaConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }

    public Dictionary<int, ZumaLevelConfig> LevelConfigs = new Dictionary<int, ZumaLevelConfig>();
    public ZumaLevelConfig LoopLevelConfig;
    public List<ZumaLevelConfig> NormalLevelConfig = new List<ZumaLevelConfig>();
    public List<ZumaStoreLevelConfig> StoreLevelConfig => ZumaConfigManager.Instance.GetConfig<ZumaStoreLevelConfig>();
    public Dictionary<int, ZumaStoreItemConfig> StoreItemConfig = new Dictionary<int, ZumaStoreItemConfig>();
    public List<ZumaLeaderBoardRewardConfig> LeaderBoardRewardConfig=> ZumaConfigManager.Instance.GetConfig<ZumaLeaderBoardRewardConfig>();
    public List<ZumaTaskRewardConfig> TaskRewardConfig => ZumaConfigManager.Instance.GetConfig<ZumaTaskRewardConfig>();
    public List<ZumaGiftBagConfig> GiftBagConfig => ZumaConfigManager.Instance.GetConfig<ZumaGiftBagConfig>();
    
    
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DebugUtil.LogError("1");
        ZumaConfigManager.Instance.InitConfig(configJson);
        InitTable(StoreItemConfig);
        InitTable(LevelConfigs);
        var levelConfigs = ZumaConfigManager.Instance.GetConfig<ZumaLevelConfig>();
        for (var i = 0; i < levelConfigs.Count; i++)
        {
            var levelConfig = levelConfigs[i];
            if (levelConfig.IsLoopLevel)
            {
                LoopLevelConfig = levelConfig;
            }
            else
            {
                NormalLevelConfig.Add(levelConfig);
            }
        }
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
        TryReleaseUselessStorage();
        
        if (CurStorageZumaWeek != null)
        {
            CurStorageZumaWeek.StartTime = (long) StartTime;
            CurStorageZumaWeek.PreheatCompleteTime = (long) StartTime + PreheatTime;
            CurStorageZumaWeek.EndTime = (long) EndTime;
            ZumaLeaderBoardModel.Instance.CreateStorage(CurStorageZumaWeek);
        }
        if (CurStorageZumaWeek == null && ZumaModel.GetFirstWeekCanGetReward() == null)
        {
            if (!CreateStorage())
            {
                LoopCreateStorage = true;
            }
        }
        _lastActivityOpenState = IsStart();
        
        ZumaLeaderBoardModel.Instance.InitFromServerData();
    }

    public void TryReleaseUselessStorage()
    {
        var releaseWeekList = new List<StorageZuma>();
        foreach (var pair in StorageZuma)
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

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Zuma);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() && CurStorageZumaWeek != null && !CurStorageZumaWeek.IsTimeOut();
    }

    public bool IsStart()
    {
        return IsPrivateOpened() && APIManager.Instance.GetServerTime() > (ulong)CurStorageZumaWeek.PreheatCompleteTime;
    }

    public void AddBall(int addCount,string reason)
    {
        if (!IsStart())
            return;
        CurStorageZumaWeek.BallCount += addCount;
        EventDispatcher.Instance.SendEventImmediately(new EventZumaDiceCountChange(addCount,CurStorageZumaWeek.BallCount));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventZumaDiceChange,
            addCount.ToString(),CurStorageZumaWeek.BallCount.ToString(),reason);
    }

    public int GetBallCount()
    {
        if (!IsStart())
            return 0;
        return CurStorageZumaWeek.BallCount;
    }

    public bool ReduceBall(int reduceCount)
    {
        if (!IsStart())
            return false;
        if (CurStorageZumaWeek.BallCount < reduceCount)
            return false;
        CurStorageZumaWeek.BallCount -= reduceCount;
        EventDispatcher.Instance.SendEventImmediately(new EventZumaDiceCountChange(-reduceCount,CurStorageZumaWeek.BallCount));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventZumaDiceChange,
            (-reduceCount).ToString(),CurStorageZumaWeek.BallCount.ToString(),"Use");
        return true;
    }
    public void AddBomb(int addCount,string reason)
    {
        if (!IsStart())
            return;
        CurStorageZumaWeek.BombCount += addCount;
        EventDispatcher.Instance.SendEventImmediately(new EventZumaBombCountChange(addCount,CurStorageZumaWeek.BombCount));
    }

    public int GetBombCount()
    {
        if (!IsStart())
            return 0;
        return CurStorageZumaWeek.BombCount;
    }

    public bool ReduceBomb(int reduceCount)
    {
        if (!IsStart())
            return false;
        if (CurStorageZumaWeek.BombCount < reduceCount)
            return false;
        CurStorageZumaWeek.BombCount -= reduceCount;
        EventDispatcher.Instance.SendEventImmediately(new EventZumaBombCountChange(-reduceCount,CurStorageZumaWeek.BombCount));
        return true;
    }
    public void AddLine(int addCount,string reason)
    {
        if (!IsStart())
            return;
        CurStorageZumaWeek.WildCount += addCount;
        EventDispatcher.Instance.SendEventImmediately(new EventZumaLineCountChange(addCount,CurStorageZumaWeek.WildCount));
    }

    public int GetLineCount()
    {
        if (!IsStart())
            return 0;
        return CurStorageZumaWeek.WildCount;
    }

    public bool ReduceLine(int reduceCount)
    {
        if (!IsStart())
            return false;
        if (CurStorageZumaWeek.WildCount < reduceCount)
            return false;
        CurStorageZumaWeek.WildCount -= reduceCount;
        EventDispatcher.Instance.SendEventImmediately(new EventZumaLineCountChange(-reduceCount,CurStorageZumaWeek.WildCount));
        return true;
    }
    

    public int GetScore()
    {
        if (!IsStart())
            return 0;
        return CurStorageZumaWeek.Score;
    }
    public bool ReduceScore(int reduceCount,string reason)
    {
        if (!IsStart())
            return false;
        if (CurStorageZumaWeek.Score < reduceCount)
            return false;
        CurStorageZumaWeek.Score -= reduceCount;
        EventDispatcher.Instance.SendEventImmediately(new EventZumaScoreChange(-reduceCount));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventZumaRadishChange,
            (-reduceCount).ToString(),CurStorageZumaWeek.Score.ToString(),reason);
        return true;
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

    public ZumaModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }

    private bool LoopCreateStorage;
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;
        if (LoopCreateStorage && IsOpened() && CurStorageZumaWeek == null)
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
            if (UIZumaMainController.Instance)
                UIZumaMainController.Instance.AnimCloseWindow();
            if (UIZumaShopController.Instance)
                UIZumaShopController.Instance.AnimCloseWindow();
            CanShowUnCollectRewardsUI();
        }
        else
        {
            var preheatUI = UIManager.Instance.GetOpenedUIByPath<UIPopupZumaPreviewController>(UINameConst.UIPopupZumaPreview);
            if (preheatUI)
                preheatUI.AnimCloseWindow();
            // if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home ||
            //     SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome)
            // {
            //     BackHomeControl.PushExtraPopup(new BackHomeControl.AutoPopUI(ZumaModel.CanShowStartPopup,new[] {UINameConst.UIPopupZumaStart,UINameConst.UIZumaMain}));
            // }
        }
        _lastActivityOpenState = currentActivityOpenState;
    }

    public static StorageZuma GetFirstWeekCanGetReward()
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
            UIZumaMainController.Open(Instance.CurStorageZumaWeek);
            return true;
        }
        return false;
    }

    public static bool CanShowPreheatPopup()
    {
        if (Instance.IsPrivateOpened() && !Instance.IsStart())
        {
            UIPopupZumaPreviewController.Open(Instance.CurStorageZumaWeek);
            return true;
        }
        return false;
    }

    public const string preheatCoolTimeKey = "ZumaPreheat";
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
        // if (Instance.IsStart() &&
        //     (SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome || SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home) && 
        //     !GuideSubSystem.Instance.IsShowingGuide() && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ZumaAuxItem))
        // {
        //     var auxItem = Instance.CurStorageZumaWeek.GetAuxItem();
        //     if (auxItem != null)
        //     {
        //         List<Transform> topLayer = new List<Transform>();
        //         topLayer.Add(auxItem.transform);
        //         GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ZumaAuxItem, auxItem.transform as RectTransform,
        //             topLayer: topLayer);
        //         if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ZumaAuxItem, null))
        //         {
        //             return true;
        //         }
        //     }
        // }
        return false;
    }
    public static bool CanShowStartPopup()
    {
        if (Instance.IsStart() && !Instance.CurStorageZumaWeek.IsStart)
        {
            Instance.CurStorageZumaWeek.IsStart = true;
            UIPopupZumaStartController.Open(Instance.CurStorageZumaWeek);
            return true;
        }
        return false;
    }
    
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.Zuma);
    }

    public Transform GetCommonFlyTarget()
    {
        var storage = CurStorageZumaWeek;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_Zuma>();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_Zuma>();
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }
    
    public void OnPurchase(TableShop shopConfig)
    {
        var cfg = GiftBagConfig.Find(a => a.ShopId == shopConfig.id);
        if (cfg == null)
            return;
        var rewards = CommonUtils.FormatReward(cfg.RewardId, cfg.RewardNum);
        var reason = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap
        };
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);

        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, true,
            reason, () =>
            {
                // if (UIZumaGiftBagController.Instance)
                //     UIZumaGiftBagController.Instance.AnimCloseWindow();
            });
    }
    public ZumaLevelConfig GetLevel(int levelId)
    {
        if (!LevelConfigs.ContainsKey(levelId))
            return null;
        return LevelConfigs[levelId];
    }

    public bool ShowAuxItem()
    {
        if (CurStorageZumaWeek == null)
            return false;

        return ZumaUtils.ShowAuxItem(CurStorageZumaWeek);
    }
    
    public bool ShowTaskEntrance()
    {
        if (CurStorageZumaWeek == null)
            return false;

        return CurStorageZumaWeek.ShowTaskEntrance();
    }
    
}