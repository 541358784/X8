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
using DragonPlus.Config.SeaRacing;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class SeaRacingModel : ActivityEntityBase,I_ActivityStatus
{
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/SeaRacing/Aux_SeaRacing";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/SeaRacing/TaskList_SeaRacing";
    }
    public static bool InGuideChain = false;
    private static SeaRacingModel _instance;
    public static SeaRacingModel Instance => _instance ?? (_instance = new SeaRacingModel());

    public override string Guid => "OPS_EVENT_TYPE_SEA_RACING";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
        EventDispatcher.Instance.AddEventListener(EventEnum.AddCoin,Instance.OnCollectCoin);
        EventDispatcher.Instance.AddEventListener(EventEnum.AddRecoverCoinStar,Instance.OnCollectCoin);
    }
    public void OnCollectCoin(BaseEvent e)
    {
        var collectCount = (int) e.datas[0];
        if (collectCount <= 0)
            return;
        AddStar(collectCount);
    }

    public static StorageDictionary<string,StorageSeaRacing> StorageSeaRacing => StorageManager.Instance.GetStorage<StorageHome>().SeaRacing;

    public StorageSeaRacing CurStorageSeaRacingWeek
    {
        get
        {
            if (ActivityId == null)
                return null;
            StorageSeaRacing.TryGetValue(ActivityId, out StorageSeaRacing curWeek);
            return curWeek;
        }
    }

    public bool CreateStorage()
    {
        DebugUtil.LogError("CreateStorage3");
        if (CurStorageSeaRacingWeek == null && IsInitFromServer()
            && SeaRacingModel.GetFirstWeekCanGetReward() == null
            && IsOpened())
        {
            DebugUtil.LogError("CreateStorage4");
            var newWeek = new StorageSeaRacing()
            {
                EndTime = (long) EndTime,
                StartTime = (long) StartTime,
                PreheatCompleteTime = (long)StartTime+PreheatTime,
                CurrencyRoundIndex = 0,
                IsFinish = false,
                IsCompletedAll = false,
                ActivityId = ActivityId,
            };
            var resMd5List = ActivityManager.Instance.GetActivityMd5List(ActivityId);
            newWeek.ActivityResList.Clear();
            foreach (var resMd5 in resMd5List)
            {
                var resPath = ActivityResHotUpdate.GetFilePath(resMd5);
                newWeek.ActivityResList.Add(resPath);
            }
            StorageSeaRacing.Add(ActivityId, newWeek);
            newWeek.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().SeaRacingGroupId;
            InitTable(RoundConfig,SeaRacingRoundConfigList);
            InitTable(RewardConfig,SeaRacingRewardConfigList);
            InitTable(RobotConfig,SeaRacingRobotConfigList);
            InitTable(RobotRandomConfig,SeaRacingRobotRandomConfigList);
            if (IsStart())
            {
                CurStorageSeaRacingWeek.CreateRound();
            }
            return true;
        }

        return false;
    }
    public long PreheatTime=> SeaRacingConfigManager.Instance.GetConfig<SeaRacingPreheatConfig>()[0].PreheatTime * (long)XUtility.Hour;
    private static void InitTable<T>(Dictionary<int, T> config,List<T> tableData = null) where T : TableBase
    {
        if (config == null)
            return;
        if (tableData == null)
            tableData = SeaRacingConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }
    public Dictionary<int, SeaRacingRoundConfig> RoundConfig = new Dictionary<int, SeaRacingRoundConfig>();
    public Dictionary<int, SeaRacingRewardConfig> RewardConfig = new Dictionary<int, SeaRacingRewardConfig>();
    public Dictionary<int, SeaRacingRobotConfig> RobotConfig = new Dictionary<int, SeaRacingRobotConfig>();
    public Dictionary<int, SeaRacingRobotRandomConfig> RobotRandomConfig = new Dictionary<int, SeaRacingRobotRandomConfig>();

    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DebugUtil.LogError("1");
        SeaRacingConfigManager.Instance.InitConfig(configJson);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");

        TryReleaseUselessStorage();

        if (CurStorageSeaRacingWeek != null)
        {
            InitTable(RoundConfig,SeaRacingRoundConfigList);
            InitTable(RewardConfig,SeaRacingRewardConfigList);
            InitTable(RobotConfig,SeaRacingRobotConfigList); 
            InitTable(RobotRandomConfig,SeaRacingRobotRandomConfigList); 
        }
        XUtility.WaitFrames(1, () =>
        {
            DebugUtil.LogError("2");
            if (CurStorageSeaRacingWeek == null && SeaRacingModel.GetFirstWeekCanGetReward() == null)
            {
                if (!CreateStorage())
                {
                    LoopCreateStorage = true;
                }
            }
            _lastActivityOpenState = IsStart();
            if (IsStart() && CurStorageSeaRacingWeek.CurRound() == null)
            {
                CurStorageSeaRacingWeek.CreateRound();
            }
        });
    }

    public void TryReleaseUselessStorage()
    {
        var releaseWeekList = new List<StorageSeaRacing>();
        foreach (var pair in StorageSeaRacing)
        {
            if (pair.Value.IsTimeOut() && pair.Value.IsFinish)
            {
                releaseWeekList.Add(pair.Value);
            }
        }

        foreach (var releaseWeek in releaseWeekList)
        {
            releaseWeek.TryRelease();
        }
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.SeaRacing);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() && CurStorageSeaRacingWeek != null && !CurStorageSeaRacingWeek.IsTimeOut();
    }

    public bool IsStart()
    {
        return IsPrivateOpened() && APIManager.Instance.GetServerTime() > (IsSkipActivityPreheating()?(ulong)CurStorageSeaRacingWeek.StartTime:(ulong)CurStorageSeaRacingWeek.PreheatCompleteTime);
    }

    public void AddStar(int addCount)
    {
        if (!IsStart())
            return;
        var curRound = CurStorageSeaRacingWeek.CurRound();
        if (curRound == null)
            return;
        if(!curRound.IsStart)
            return;
        if(curRound.IsFinish)
            return;
        curRound.CollectStar(addCount);
        curRound.SortController().UpdateMe();
    }

    public int GetStar()
    {
        if (!IsStart())
            return 0;
        var curRound = CurStorageSeaRacingWeek.CurRound();
        if (curRound == null)
            return 0;
        return curRound.Score;
    }

    private bool _lastActivityOpenState;//记录上一帧的活动开启状态，在轮询中判断是否触发开启活动或者关闭活动

    public SeaRacingModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }

    private bool LoopCreateStorage;
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;
        if (LoopCreateStorage && IsOpened())
        {
            LoopCreateStorage = false;
            CreateStorage();
        }
        
        if(CurStorageSeaRacingWeek != null && CurStorageSeaRacingWeek.CurRound() != null && CurStorageSeaRacingWeek.CurRound().IsStart)
            CurStorageSeaRacingWeek?.CurRound()?.SortController().UpdateAll();
        
        var currentActivityOpenState = IsStart();
        if (_lastActivityOpenState == currentActivityOpenState)
            return;
        if (!currentActivityOpenState)
        {
            CanShowUnCollectRewardsUI();
        }
        else
        {
            CurStorageSeaRacingWeek.CreateRound();
            // CanShowStartPopup();
        }
        _lastActivityOpenState = currentActivityOpenState;
    }

    public static StorageSeaRacing GetFirstWeekCanGetReward()
    {
        foreach (var storageWeekPair in StorageSeaRacing)
        {
            var storageWeek = storageWeekPair.Value;
            if (storageWeek.IsTimeOut() && !storageWeek.IsFinish)
                return storageWeek;
        }
        return null;
    }

    public static bool CanShowUnCollectRewardsUI()
    {
        var weekCanGetReward = GetFirstWeekCanGetReward();
        if (weekCanGetReward == null)
            return false;
        if (!weekCanGetReward.IsResExist())
            return false;
        foreach (var round in weekCanGetReward.SeaRacingRoundList)
        {
            if (round.Value.UnCollectRewards.Count > 0)
            {
                OpenRoundRewardPopup(round.Value);
                return true;
            }
        }
        OpenFinishPopup(weekCanGetReward);
        return true;
    }

    public static UIPopupSeaRacingEndController OpenFinishPopup(StorageSeaRacing storageWeek)
    {
        
        return UIPopupSeaRacingEndController.Open(storageWeek);
    }
    public static bool CanShowMainPopup()
    {
        if (Instance.IsStart() && Instance.CurStorageSeaRacingWeek.CurRound()!=null && Instance.CurStorageSeaRacingWeek.CurRound().IsStart)
        {
            Instance.CurStorageSeaRacingWeek.CurRound().SortController().UpdateAll();
            UISeaRacingMainController.Open(Instance.CurStorageSeaRacingWeek.CurRound());
            return true;
        }
        return false;
    }
    public static UISeaRacingRewardController OpenRoundRewardPopup(StorageSeaRacingRound storageWeek)
    {
        var mainWindow =
            UIManager.Instance.OpenUI(UINameConst.UISeaRacingReward) as UISeaRacingRewardController;
        if (!mainWindow)
            return null;
        mainWindow.BindStorage(storageWeek);
        return mainWindow;
    }

    public static bool CanShowPreheatPopup()
    {
        if (Instance.IsPrivateOpened() && !Instance.IsStart())
        {
            UIPopupSeaRacingPreviewController.Open(Instance.CurStorageSeaRacingWeek);
            return true;
        }
        return false;
    }

    public const string coolTimeKey = "SeaRacing";
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
        if (Instance.IsStart() && Instance.CurStorageSeaRacingWeek.CurRound()!=null && !Instance.CurStorageSeaRacingWeek.CurRound().IsStart)
        {
            if ((SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome || SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home) && 
                !GuideSubSystem.Instance.IsShowingGuide() && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SeaRacingHomeEntrance))
            {
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SeaRacingHomeEntrance, null);
                return true;
            }
            
            UIPopupSeaRacingStartController.Open(Instance.CurStorageSeaRacingWeek.CurRound());
            return true;
        }
        return false;
    }
    
    
    public static bool CanAutoShowStartPopup()
    {
        if (Instance.IsStart() && Instance.CurStorageSeaRacingWeek.CurRound()!=null && !Instance.CurStorageSeaRacingWeek.CurRound().IsStart)
        {
            if ((SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome || SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home) && 
                !GuideSubSystem.Instance.IsShowingGuide() && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SeaRacingHomeEntrance))
            {
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SeaRacingHomeEntrance, null);
                return true;
            }
            
            string _coolTimeKey = "SeaRacing"+Instance.CurStorageSeaRacingWeek.CurrencyRoundIndex;
            if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, _coolTimeKey))
                return false;

            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, _coolTimeKey,CommonUtils.GetTimeStamp());
            
            UIPopupSeaRacingStartController.Open(Instance.CurStorageSeaRacingWeek.CurRound());
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
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.SeaRacing);
    }

    public I_ActivityStatus.ActivityStatus GetActivityStatus()
    {
        if (StorageSeaRacing.Count == 0)
            return I_ActivityStatus.ActivityStatus.None;
        if (StorageSeaRacing.Last().Value.IsCompletedAll)
            return I_ActivityStatus.ActivityStatus.Completed;
        var storage = StorageSeaRacing.Last().Value;
        if (storage.SeaRacingRoundList.Count > 0 && storage.SeaRacingRoundList[1].IsStart)
            return I_ActivityStatus.ActivityStatus.Incomplete;
        return I_ActivityStatus.ActivityStatus.None;
    }
}