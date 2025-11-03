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
using DragonPlus.Config.ThemeDecoration;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class ThemeDecorationModel : ActivityEntityBase
{
    private static ThemeDecorationModel _instance;
    public static ThemeDecorationModel Instance => _instance ?? (_instance = new ThemeDecorationModel());

    public override string Guid => "OPS_EVENT_TYPE_THEME_DECORATION";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public static StorageDictionary<string,StorageThemeDecoration> StorageThemeDecoration => StorageManager.Instance.GetStorage<StorageHome>().ThemeDecoration;

    public StorageThemeDecoration CurStorageThemeDecorationWeek
    {
        get
        {
            if (ActivityId == null)
                return null;
            StorageThemeDecoration.TryGetValue(ActivityId, out StorageThemeDecoration curWeek);
            return curWeek;
        }
    }

    public bool CreateStorage()
    {
        if (!SceneFsm.mInstance.ClientInited)
            return false;
        if (CurStorageThemeDecorationWeek == null && IsInitFromServer()
            && ThemeDecorationModel.GetFirstWeekCanGetReward() == null
            && ThemeDecorationModel.GetFirstWeekCanGetLeaderBoardReward() == null
            && IsOpened())
        {
            var newWeek = new StorageThemeDecoration()
            {
                ActivityId = ActivityId,
                ActivityResList = {},
                FinishStoreItemList = {},
                Score = 0,
                IsStart = false,
                TotalScore = 0,
                UnLockStoreLevel = {1},
                IsBuyPreEnd = false,
            };
            StorageThemeDecoration.Add(ActivityId, newWeek);
            newWeek.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().ThemeDecorationGroupId;
            InitTable(StoreItemConfig,ThemeDecorationStoreItemConfigList);
            InitTable(LeaderBoardRewardConfig,ThemeDecorationLeaderBoardRewardConfigList);   
            newWeek.SkinName = GlobalConfig.SkinName;
            newWeek.StartTime = (long)StartTime;
            newWeek.PreheatCompleteTime = (long)StartTime + PreheatTime;
            newWeek.PreEndTime = (long)EndTime - PreEndTime;
            newWeek.PreEndBuyTime = (long)EndTime - PreEndTime + PreEndBuyTime;
            newWeek.EndTime = (long)EndTime;
            
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
            //         if ((ThemeDecorationStoreItemType)storeItemCfg.Type == ThemeDecorationStoreItemType.BuildItem)
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
            ThemeDecorationLeaderBoardModel.Instance.CreateStorage(newWeek);
            newWeek.AddSkinUIWindowInfo();
            UpdateActivityUsingResList(ActivityId);
            
            return true;
        }

        return false;
    }
    public ThemeDecorationGlobalConfig GlobalConfig => ThemeDecorationGlobalConfigList[0];
    public long PreheatTime=> IsSkipActivityPreheating()?0:(long)((ulong)GlobalConfig.PreheatTime * XUtility.Min);
    public long PreEndTime=> (long)((ulong)GlobalConfig.ExtendBuyTime * XUtility.Min);
    public long PreEndBuyTime=> (long)((ulong)GlobalConfig.ExtendBuyWaitTime * XUtility.Min);
    private static void InitTable<T>(Dictionary<int, T> config,List<T> tableData = null) where T : TableBase
    {
        if (config == null)
            return;

        if (tableData == null)
            tableData = ThemeDecorationConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }
    public List<ThemeDecorationStoreLevelConfig> StoreLevelConfig => ThemeDecorationStoreLevelConfigList;
    public Dictionary<int, ThemeDecorationStoreItemConfig> StoreItemConfig = new Dictionary<int, ThemeDecorationStoreItemConfig>();
    public Dictionary<int,ThemeDecorationLeaderBoardRewardConfig> LeaderBoardRewardConfig= new Dictionary<int, ThemeDecorationLeaderBoardRewardConfig>();
    public List<ThemeDecorationTaskRewardConfig> TaskRewardConfig => ThemeDecorationTaskRewardConfigList;
    public List<ThmeDecorationLeaderBoardScheduleConfig> LeaderBoardScheduleConfig => ThmeDecorationLeaderBoardScheduleConfigList;
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        ThemeDecorationConfigManager.Instance.InitConfig(configJson);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
        TryReleaseUselessStorage();

        if (CurStorageThemeDecorationWeek != null)
        {
            InitTable(StoreItemConfig,ThemeDecorationStoreItemConfigList);
            InitTable(LeaderBoardRewardConfig,ThemeDecorationLeaderBoardRewardConfigList);   
        }
        if (CurStorageThemeDecorationWeek != null)
        {
            CurStorageThemeDecorationWeek.StartTime = (long) StartTime;
            CurStorageThemeDecorationWeek.PreheatCompleteTime = (long) StartTime + PreheatTime;
            CurStorageThemeDecorationWeek.PreEndTime = (long) EndTime - PreEndTime;
            CurStorageThemeDecorationWeek.PreEndBuyTime = (long) EndTime - PreEndTime + PreEndBuyTime;
            CurStorageThemeDecorationWeek.EndTime = (long) EndTime;
            ThemeDecorationLeaderBoardModel.Instance.CreateStorage(CurStorageThemeDecorationWeek);
        }
        if (CurStorageThemeDecorationWeek == null 
            && ThemeDecorationModel.GetFirstWeekCanGetReward() == null 
            && ThemeDecorationModel.GetFirstWeekCanGetLeaderBoardReward() == null)
        {
            if (!CreateStorage())
            {
                LoopCreateStorage = true;
            }
        }
        
        _lastActivityOpenState = IsStart();
        
        ThemeDecorationLeaderBoardModel.Instance.InitFromServerData();
        AddSkinUIWindowInfo();
    }
    
    public static void AddSkinUIWindowInfo()
    {
        if (Instance.CurStorageThemeDecorationWeek != null)
            Instance.CurStorageThemeDecorationWeek.AddSkinUIWindowInfo();
        var weekList = GetAllWeekCanGetReward();
        foreach (var week in weekList)
        {
            week.AddSkinUIWindowInfo();
        }
    }

    public void TryReleaseUselessStorage()
    {
        var releaseWeekList = new List<StorageThemeDecoration>();
        foreach (var pair in StorageThemeDecoration)
        {
            releaseWeekList.Add(pair.Value);
        }
        foreach (var releaseWeek in releaseWeekList)
        {
            releaseWeek.TryRelease();
        }
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.ThemeDecoration);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() && CurStorageThemeDecorationWeek != null && !CurStorageThemeDecorationWeek.IsTimeOut();
    }
    
    public bool IsStart()
    {
        return IsPrivateOpened() && 
               CurStorageThemeDecorationWeek.IsPreheat() && 
               !CurStorageThemeDecorationWeek.IsTotalTimeOut();
    }
    
    public void AddScore(int addCount,string reason)
    {
        if (!IsStart())
            return;
        CurStorageThemeDecorationWeek.Score += addCount;
        CurStorageThemeDecorationWeek.TotalScore += addCount;
        ThemeDecorationLeaderBoardModel.Instance.AddStar(addCount);
        EventDispatcher.Instance.SendEventImmediately(new EventThemeDecorationScoreChange(addCount));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventThemeDecorationRadishChange,
            addCount.ToString(),CurStorageThemeDecorationWeek.Score.ToString(),reason);
    }

    public int GetScore()
    {
        if (!IsPrivateOpened())
            return 0;
        return CurStorageThemeDecorationWeek.Score;
    }
    public bool ReduceScore(int reduceCount,string reason)
    {
        if (!IsStart())
            return false;
        if (CurStorageThemeDecorationWeek.Score < reduceCount)
            return false;
        CurStorageThemeDecorationWeek.Score -= reduceCount;
        EventDispatcher.Instance.SendEventImmediately(new EventThemeDecorationScoreChange(-reduceCount));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventThemeDecorationRadishChange,
            (-reduceCount).ToString(),CurStorageThemeDecorationWeek.Score.ToString(),reason);
        return true;
    }

    public void PurchaseSuccess(TableShop shopConfig)
    {
        StorageThemeDecoration buySuccessStorage = null;
        if (IsPrivateOpened() && !CurStorageThemeDecorationWeek.IsBuyPreEnd)
        {
            buySuccessStorage = CurStorageThemeDecorationWeek;
        }
        else
        {
            foreach (var pair in StorageThemeDecoration)
            {
                var storage = pair.Value;
                if (!storage.IsTimeOut() && !storage.IsBuyPreEnd)
                {
                    buySuccessStorage = storage;
                    break;
                }
            }
        }

        if (IsInitFromServer())
        {
            var rewards = CommonUtils.FormatReward(GlobalConfig.ExtendBuyRewardId, GlobalConfig.ExtendBuyRewardNum);
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);
            CommonRewardManager.Instance.PopCommonReward(rewards,CurrencyGroupManager.Instance.GetCurrencyUseController(), 
                true, new GameBIManager.ItemChangeReasonArgs()
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ThemeDecorationGet,
                }
                , () =>
                {
                    PayRebateModel.Instance.OnPurchaseAniFinish();
                    PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                });
        }
        if (buySuccessStorage != null)
        {
            buySuccessStorage.IsBuyPreEnd = true;
            EventDispatcher.Instance.SendEventImmediately(new EventThemeDecorationBuySuccess(buySuccessStorage));
        }
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
        else
        {
            value = ((tempPrice/20)+1);
            value = Math.Min(value, 8);
        }
        
        if (isMul && MultipleScoreModel.Instance.IsOpenActivity())
            value = (int)(value * MultipleScoreModel.Instance.GetMultiple(MultipleScoreModel.InfluenceFuncType.ThemeDecoration));
        return value;
    }
    
    private bool _lastActivityOpenState;//记录上一帧的活动开启状态，在轮询中判断是否触发开启活动或者关闭活动

    public ThemeDecorationModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }

    private bool LoopCreateStorage;
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;
        if (LoopCreateStorage && IsOpened() && CurStorageThemeDecorationWeek == null)
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
            CanShowBuyPreEndUIEachDay();
        }
        else
        {
            var preheatUI = UIManager.Instance.GetOpenedUIByPath<UIPopupThemeDecorationPreviewController>(CurStorageThemeDecorationWeek.GetAssetPathWithSkinName(UINameConst.UIPopupThemeDecorationPreview));
            if (preheatUI)
                preheatUI.AnimCloseWindow();
            // if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home ||
            //     SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome)
            // {
            //     BackHomeControl.PushExtraPopup(new BackHomeControl.AutoPopUI(ThemeDecorationModel.CanShowStartPopup,new[] {UINameConst.UIPopupThemeDecorationStart,UINameConst.UIThemeDecorationMain}));
            // }
        }
        _lastActivityOpenState = currentActivityOpenState;
    }

    public static StorageThemeDecoration GetFirstWeekCanGetReward()
    {
        foreach (var storageWeekPair in StorageThemeDecoration)
        {
            var storageWeek = storageWeekPair.Value;
            if (storageWeek.GetUnCollectReward() != null)
                return storageWeek;
        }
        return null;
    }
    public static StorageThemeDecoration GetFirstWeekCanGetLeaderBoardReward()
    {
        foreach (var storageWeekPair in StorageThemeDecoration)
        {
            var storageWeek = storageWeekPair.Value;
            if (storageWeek.GetUnCollectLeaderBoard() != null)
                return storageWeek;
        }
        return null;
    }

    public static bool OpenMapPreviewUI()
    {
        var storage = Instance.CurStorageThemeDecorationWeek;
        if (storage == null)
        {
            storage = StorageThemeDecoration.Last().Value;
        }
        if (storage == null)
            return false;
        UIThemeDecorationMapPreviewController.Open(storage);
        return true;
    }
    public static bool CanShowShopUI()
    {
        if (Instance.IsStart())
        {
            UIThemeDecorationShopController.Open(Instance.CurStorageThemeDecorationWeek);
            return true;
        }
        return false;
    }
    public static bool CanShowBuyPreEndUI()
    {
        var storage = Instance.CurStorageThemeDecorationWeek;
        if (Instance.IsPrivateOpened() && storage.IsPreEnd() && !storage.IsBuyPreEnd)
        {
            UIPopupThemeDecorationBuyPreEndController.Open(storage);
            return true;
        }
        return false;
    }
    public const string BuyPreEndUICoolTimeKey = "ThemeDecorationBuyPreEndUI";
    public static bool CanShowBuyPreEndUIEachDay()
    {
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, BuyPreEndUICoolTimeKey))
            return false;
        if (CanShowBuyPreEndUI())
        {
            ShowBuyPreEndUIStorage = Instance.CurStorageThemeDecorationWeek;
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, BuyPreEndUICoolTimeKey,CommonUtils.GetTimeStamp());
            return true;
        }
        return false;
    }
    private static StorageThemeDecoration ShowBuyPreEndUIStorage;
    public static string[] ShowBuyPreEndUIList()
    {
        return new[]
        {
            ShowBuyPreEndUIStorage.GetAssetPathWithSkinName(UINameConst.UIPopupThemeDecorationBuyPreEnd),
            UINameConst.UIPopupReward,
        };
    }

    public static bool CanShowUnCollectRewardsUI()
    {
        var weekCanGetReward = GetFirstWeekCanGetReward();
        if (weekCanGetReward == null)
            return false;
        var rewards = weekCanGetReward.GetUnCollectReward();
        var reasonArgs = new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ThemeDecorationGet};
        weekCanGetReward.UnCollectRewards.Clear();
        CommonRewardManager.Instance.PopActivityUnCollectReward(rewards, reasonArgs, null);
        return true;
    }
    public static bool CanShowPreheatPopup()
    {
        if (Instance.IsPrivateOpened() && !Instance.CurStorageThemeDecorationWeek.IsPreheat())
        {
            UIPopupThemeDecorationPreviewController.Open(Instance.CurStorageThemeDecorationWeek);
            ShowPreheatPopupUIStorage = Instance.CurStorageThemeDecorationWeek;
            return true;
        }
        return false;
    }
    private static StorageThemeDecoration ShowPreheatPopupUIStorage;
    public static string[] ShowPreheatPopupUIList()
    {
        return new[] {ShowPreheatPopupUIStorage.GetAssetPathWithSkinName(UINameConst.UIPopupThemeDecorationPreview)};
    }

    public const string coolTimeKey = "ThemeDecoration";
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
    
    private static StorageThemeDecoration ShowStartPopupUIStorage;
    public static string[] ShowStartPopupUIList()
    {
        return new[]
        {
            ShowStartPopupUIStorage.GetAssetPathWithSkinName(UINameConst.UIPopupThemeDecorationStart),
            Instance.CurStorageThemeDecorationWeek.GetAssetPathWithSkinName(UINameConst.UIThemeDecorationShop),
        };
    }
    public static bool CanShowStartPopup()
    {
        if (Instance.IsStart() && !Instance.CurStorageThemeDecorationWeek.IsStart)
        {
            // if ((SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome || SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home) && 
            //     !GuideSubSystem.Instance.IsShowingGuide() && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ThemeDecorationHomeEntrance))
            // {
            //     GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ThemeDecorationHomeEntrance, null);
            //     return true;
            // }
            Instance.CurStorageThemeDecorationWeek.IsStart = true;
            UIPopupThemeDecorationStartController.Open(Instance.CurStorageThemeDecorationWeek);
            ShowStartPopupUIStorage = Instance.CurStorageThemeDecorationWeek;
            return true;
        }
        return false;
    }

    public static string[] ShowMultipleScoreUIList()
    {
        return new []
        {
            ShowMultipleScoreStorage.GetAssetPathWithSkinName(UINameConst.UIPopupThemeDecorationMultipleScore),
        };
    }

    private static StorageThemeDecoration ShowMultipleScoreStorage;
    public const string MultipleScoreCoolTimeKey = "ThemeDecorationMultipleScore"; 
    public static bool CanShowMultipleScore()
    {
        if (!Instance.IsStart())
            return false;
        if (ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.ThemeDecoration) > 1)
            return false;
        if (MultipleScoreModel.Instance.GetMultiple(MultipleScoreModel.InfluenceFuncType.ThemeDecoration) <= 1)
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, MultipleScoreCoolTimeKey))
        {
            ShowMultipleScoreStorage = Instance.CurStorageThemeDecorationWeek;
            UIPopupThemeDecorationMultipleScoreController.Open(Instance.CurStorageThemeDecorationWeek);
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, MultipleScoreCoolTimeKey, CommonUtils.GetTimeStamp());
            return true;
        }
        return false;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.ThemeDecoration);
    }
    
    public override List<string> GetNeedResList(string activityId,List<string> allResList)
    {
        var skinNameList = new List<string>();
        if (IsInitFromServer())
            skinNameList.Add(GlobalConfig.SkinName.ToLower());
        if (CurStorageThemeDecorationWeek != null)
        {
            var skinName = CurStorageThemeDecorationWeek.GetSkinName();
            if (!skinNameList.Contains(skinName.ToLower()))
                skinNameList.Add(skinName.ToLower());
        }
        var weekList = GetAllWeekCanGetReward();
        foreach (var week in weekList)
        {
            var skinName = week.GetSkinName();
            if (!skinNameList.Contains(skinName.ToLower()))
                skinNameList.Add(skinName.ToLower());
        }
        var resList = new List<string>();
        foreach (var path in allResList)
        {
            foreach (var skinName in skinNameList)
            {
                if (path.Contains(skinName))
                {
                    DebugUtil.Log("ActivityManager -> 活动资源 : " + path);
                    resList.Add(path);
                    break;
                }
            }
        }
        return resList;
    }
    
    public static List<StorageThemeDecoration> GetAllWeekCanGetReward()
    {
        var weekList = new List<StorageThemeDecoration>();
        foreach (var storageWeekPair in StorageThemeDecoration)
        {
            var storageWeek = storageWeekPair.Value;
            if (storageWeek.GetUnCollectReward() != null || storageWeek.GetUnCollectLeaderBoard() != null)
                weekList.Add(storageWeek);
        }
        return weekList;
    }
    
    public string GetAuxItemAssetPath()
    {
        if (CurStorageThemeDecorationWeek == null)
            return null;
        
        return ThemeDecorationUtils.GetAuxItemAssetPath(CurStorageThemeDecorationWeek);
    }

    public bool ShowEntrance()
    {
        if (CurStorageThemeDecorationWeek == null)
            return false;

        return ThemeDecorationUtils.ShowEntrance(CurStorageThemeDecorationWeek);
    }
    
    public bool ShowMergeEntrance()
    {
        if (CurStorageThemeDecorationWeek == null)
            return false;

        return CurStorageThemeDecorationWeek.ShowEntrance();
    }

    public string GetMergeItemAssetPath()
    {
        if (CurStorageThemeDecorationWeek == null)
            return null;

        return CurStorageThemeDecorationWeek.GetTaskItemAssetPath();
    }
}