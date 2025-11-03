// #define RECOVERCOIN_USE_ROBOT
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deco.Node;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonPlus.Config.RecoverCoin;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
public class RecoverCoinModel :ActivityEntityBase
{
    public static bool InGuideChain = false;
    public static string LeadBoardAPITypeName = "RecoverCoin";
    private static RecoverCoinModel _instance;
    public static RecoverCoinModel Instance => _instance ?? (_instance = new RecoverCoinModel());
    
    public override string Guid => "OPS_EVENT_TYPE_RECOVER_COIN";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public void ClearUselessStorage()
    {
        StorageRecoverCoin.WeekTimeConfig.Clear();
        // Instance._weekTimeConfig = new Dictionary<int, RecoverCoinTimeConfig>();
        // foreach (var pair in StorageRecoverCoin.WeekTimeConfig)
        // {
        //     var storageWeekCfg = pair.Value;
        //     var serverCfg = new RecoverCoinTimeConfig()
        //     {
        //         Id = storageWeekCfg.Id,
        //         Week = storageWeekCfg.Week,
        //         StarTimeSec = storageWeekCfg.StarTimeSec,
        //         EndTimeSec = storageWeekCfg.EndTimeSec,
        //     };
        //     Instance._weekTimeConfig.Add(serverCfg.Week,serverCfg);
        // }

        var releaseWeekList = new List<StorageRecoverCoinWeek>();
        foreach (var pair in StorageRecoverCoin.StorageByWeek)
        {
            if (pair.Value.IsFinish)
            {
                releaseWeekList.Add(pair.Value);
            }
        }
        foreach (var releaseWeek in releaseWeekList)
        {
            releaseWeek.TryRelease();
        }
    }
    public static StorageRecoverCoin StorageRecoverCoin => StorageManager.Instance.GetStorage<StorageHome>().RecoverCoin;

    public Dictionary<int, RecoverCoinTimeConfig> _weekTimeConfig;
    public void InitWeekTimeConfig()
    {
        // StorageRecoverCoin.WeekTimeConfig.Clear();
        _weekTimeConfig = new Dictionary<int, RecoverCoinTimeConfig>();
        var timeConfig = RecoverCoinConfigManager.Instance.GetConfig<RecoverCoinTimeConfig>();
        foreach (var cfg in timeConfig)
        {
            // if (CurTime > cfg.EndTimeSec.ToLong())
            //     continue;
            // StorageRecoverCoin.WeekTimeConfig.Add(cfg.Week,new StorageRecoverCoinTimeConfig()
            // {
            //     Id = cfg.Id,
            //     Week = cfg.Week,
            //     StarTimeSec = cfg.StarTimeSec,
            //     EndTimeSec = cfg.EndTimeSec,
            // });
            _weekTimeConfig.Add(cfg.Week,cfg);
        }
    }

    private long CurTime => (long) APIManager.Instance.GetServerTime();
    private int _curWeekId = -1;
    private int CurWeekId
    {
        get
        {
            if (_weekTimeConfig == null)
                return -1;
            if (!_weekTimeConfig.ContainsKey(_curWeekId) ||
                _weekTimeConfig[_curWeekId].StarTimeSec.ToLong() > CurTime ||
                _weekTimeConfig[_curWeekId].EndTimeSec.ToLong() < CurTime)
            {
                _curWeekId = -1;
                foreach (var cfg in _weekTimeConfig)//重新定位weekId
                {
                    if (cfg.Value.StarTimeSec.ToLong() > CurTime)
                        break;
                    if (cfg.Value.StarTimeSec.ToLong() <= CurTime && cfg.Value.EndTimeSec.ToLong() > CurTime)
                    {
                        _curWeekId = cfg.Key;
                        break;
                    }
                }
            }
            return _curWeekId;
        }
    }

    public static List<int> LastFinishNodeList => StorageRecoverCoin.LastFinishNodeList;
    public RecoverCoinRobotGrowSpeedConfig RobotGrowSpeedConfig
    {
        get
        {
            var curCoin = UserData.Instance.GetRes(UserData.ResourceId.Coin);
            var cfgList = RecoverCoinConfigManager.Instance.GetConfig<RecoverCoinRobotGrowSpeedConfig>();
            foreach (var cfg in cfgList)
            {
                if (cfg.CoinMin <= curCoin && cfg.CoinMax >= curCoin)
                {
                    return cfg;
                }
            }
            return null;
        }
    }
    public RecoverCoinPlayerCoinCountGroupConfig  PlayerCoinCountGroupConfig
    {
        get
        {
            var curCoin = UserData.Instance.GetRes(UserData.ResourceId.Coin);
            var cfgList = RecoverCoinConfigManager.Instance.GetConfig<RecoverCoinPlayerCoinCountGroupConfig>();
            foreach (var cfg in cfgList)
            {
                if (cfg.CoinMin <= curCoin && cfg.CoinMax >= curCoin)
                {
                    return cfg;
                }
            }
            return null;
        }
    }
    public int MaxPlayerCount => RecoverCoinConfigManager.Instance.GetConfig<RecoverCoinRobotCountConfig>()[0].MaxPlayerCount;

    public bool IsCurWeekExist()
    {
        if (CurWeekId < 1)
            return false;
        return StorageRecoverCoin.StorageByWeek.ContainsKey(CurWeekId);
    }

    private List<Action> _actionAfterWeekEnd = new List<Action>();

    public void PushActionAfterWeekEnd(Action action)
    {
        _actionAfterWeekEnd.Add(action);
    }

    public void InvokeActionAfterWeekEnd()
    {
        foreach (var action in _actionAfterWeekEnd)
        {
            action?.Invoke();
        }
        _actionAfterWeekEnd = new List<Action>();
    }
    public bool IsCurWeekExistByStorage()//如果存储中存在未结束的week数据，则视为在排行赛活动中
    {
        foreach (var week in StorageRecoverCoin.StorageByWeek)
        {
            if (!week.Value.IsFinish && week.Value.StartTime < CurTime)
            {
                return true;
            }
        }
        return false;
    }
    
    public StorageRecoverCoinWeek CurStorageRecoverCoinWeek
    {
        get
        {
            StorageRecoverCoin.StorageByWeek.TryGetValue(CurWeekId,out StorageRecoverCoinWeek curWeek);
            if (curWeek == null && IsFinishedAllTarget() && CurWeekId > 0 && IsInitFromServer())
            {
#if RECOVERCOIN_USE_ROBOT
                var robotGrowSpeedConfig = RobotGrowSpeedConfig;
#else
                var userCoinCountGroupConfig = PlayerCoinCountGroupConfig;
#endif
                var newWeek = new StorageRecoverCoinWeek()
                {

                    // JsonRecoverCoinRewardConfig = JsonConvert.SerializeObject(RecoverCoinConfigManager.Instance.GetConfig<RecoverCoinRewardConfig>()),
                    // JsonRecoverCoinExchangeStarConfig = JsonConvert.SerializeObject(RecoverCoinConfigManager.Instance.GetConfig<RecoverCoinExchangeStarConfig>()),
                    // JsonRecoverCoinRobotMinStarUpdateIntervalConfig = JsonConvert.SerializeObject(RecoverCoinConfigManager.Instance.GetConfig<RecoverCoinRobotMinStarUpdateIntervalConfig>()),
                    BuyTimes = 0,
                    EndTime = _weekTimeConfig[_curWeekId].EndTimeSec.ToLong(),
                    StartTime = _weekTimeConfig[_curWeekId].StarTimeSec.ToLong(),
                    StarCount = 0,
                    CompletedTaskCount = 0,
                    IsFinish = false,
                    IsStart = false,
                    WeekId = CurWeekId,
                    MaxPlayerCount = MaxPlayerCount,
#if RECOVERCOIN_USE_ROBOT
                    RobotIndex = robotGrowSpeedConfig.Id,
#else
                    PlayerCoinCountGroupIndex = userCoinCountGroupConfig.Id,
#endif
                    // ActivityId = ActivityId,
                    StarUpdateTime = APIManager.Instance.GetServerTime(),
                    IsUpdateFinalData = false,
                    SkinName = RecoverCoinConfigManager.Instance.GetConfig<RecoverCoinSkinConfig>()[0].SkinName,
                };
#if RECOVERCOIN_USE_ROBOT
                var namePool = RecoverCoinConfigManager.Instance.GetConfig<RecoverCoinRobotNameConfig>();
                string GetRobotName()
                {
                    var index = Random.Range(0, namePool.Count);
                    var result = namePool[index];
                    namePool.RemoveAt(index);
                    return result.Name;
                }
                newWeek.RobotList.Clear();
                for (var i = 0; i < newWeek.MaxPlayerCount-1; i++)
                {
                    var robot = new StorageRecoverCoinRobot()
                    {
                        Id = i + 1,
                        MaxStarCount = robotGrowSpeedConfig.GetNewRobotMaxStarCount(i),
                        PlayerName = GetRobotName(),
                        AvatarIconId = Random.Range(0,6),
                        LastUpdateStarCount = 0,
                        NextUpdateInterval = newWeek.GetRobotUpdateInterval(),
                    };
                    newWeek.RobotList.Add(robot);
                }
#endif
                StorageRecoverCoin.StorageByWeek.Add(CurWeekId,newWeek);
                newWeek.AddSkinUIWindowInfo();
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventCoinoverLeadertopgroup,
                    data1:newWeek.PlayerCoinCountGroupIndex.ToString());
                curWeek = newWeek;
                UpdateActivityUsingResList(ActivityId);
                if (RecoverCoinModel.GetFirstWeekCanGetReward() == null)
                {
                    newWeek.ForceUpdateLeaderBoardFromServer().WrapErrors();
                }
            }
            return curWeek;
        }
    }

    private List<StorageRecoverCoinRobot> _robotSortList;

    public bool IsFinishedAllTarget()
    {
        return !CoinLeaderBoardModel.Instance.IsCurWeekExistByStorage() && DecoManager.Instance.IsFinishLastNode();
    }

    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        EventDispatcher.Instance.AddEventListener(EventEnum.OwnDecoNode,(e)=>
        {
            if(FarmModel.Instance.IsFarmModel())
                return;

            var decoNodeData = (DecoNodeData)e.datas[0];
            if (decoNodeData._config.costId == (int)UserData.ResourceId.Coin)
                _lastFinishNodeId = decoNodeData._config.id;
        });
        RecoverCoinConfigManager.Instance.InitConfig(configJson);
        ClearUselessStorage();
        InitWeekTimeConfig();
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        _lastWeekState = CurStorageRecoverCoinWeek;
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
        if (RecoverCoinModel.GetFirstWeekCanGetReward() == null && 
            IsCurWeekExist())
        {
            CurStorageRecoverCoinWeek.ForceUpdateLeaderBoardFromServer().WrapErrors();
        }
        AddSkinUIWindowInfo();
    }


    public override void UpdateActivityState()
    {
        // InitServerDataFinish();
    }

    protected override void InitServerDataFinish()
    {
        foreach (var weekCfg in _weekTimeConfig)
        {
            if (StorageRecoverCoin.StorageByWeek.ContainsKey(weekCfg.Value.Week))
            {
                StorageRecoverCoin.StorageByWeek[weekCfg.Value.Week].StartTime = weekCfg.Value.StarTimeSec.ToLong();
                StorageRecoverCoin.StorageByWeek[weekCfg.Value.Week].EndTime = weekCfg.Value.EndTimeSec.ToLong();
            }
        }
    }

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) &&
               // Instance.IsFinishedAllTarget() && 
               CurStorageRecoverCoinWeek != null; //当前当前周的配置;
    }

    public bool IsStart()
    {
        return IsCurWeekExist() && CurStorageRecoverCoinWeek.IsStart && ActivityManager.Instance.IsActivityResourcesDownloaded(ActivityId);
    }

    public void AddStar(int addCount)
    {
        if (!IsStart())
            return;
        CurStorageRecoverCoinWeek.CollectStar(addCount);
        CurStorageRecoverCoinWeek.SortController().UpdateMe();
    }

    public int GetStar()
    {
        if (!IsStart())
            return 0;
        return CurStorageRecoverCoinWeek.StarCount;
    }

    private StorageRecoverCoinWeek _lastWeekState;
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;
        // CurStorageRecoverCoinWeek?.SortController().Update();
        var currentActivityOpenState = CurStorageRecoverCoinWeek;
        if (_lastWeekState == currentActivityOpenState)
            return;
        CanShowUnCollectRewardsUI();
        
        _lastWeekState = currentActivityOpenState;
    }

    public static StorageRecoverCoinWeek GetFirstWeekCanGetReward()
    {
        foreach (var storageWeekPair in StorageRecoverCoin.StorageByWeek)
        {
            var storageWeek = storageWeekPair.Value;
            if (storageWeek.CanStorageRecoverCoinWeekGetReward())
            {
                return storageWeek;
            }
        }
        return null;
    }

    public static List<StorageRecoverCoinWeek> GetAllWeekCanGetReward()
    {
        var weekList = new List<StorageRecoverCoinWeek>();
        foreach (var storageWeekPair in StorageRecoverCoin.StorageByWeek)
        {
            var storageWeek = storageWeekPair.Value;
            if (storageWeek.CanStorageRecoverCoinWeekGetReward())
            {
                weekList.Add(storageWeek);
            }
        }
        return weekList;
    }
    public static string GetSkinName()
    {
        if (Instance.CurStorageRecoverCoinWeek != null)
        {
            // Debug.LogError("周期storage.SkinName="+Instance.CurStorageRecoverCoinWeek.GetSkinName());
            return Instance.CurStorageRecoverCoinWeek.GetSkinName();
        }
        else if (Instance.IsInitFromServer())
        {
            // Debug.LogError("配置SkinName="+RecoverCoinConfigManager.Instance.GetConfig<RecoverCoinSkinConfig>()[0].SkinName);
            return RecoverCoinConfigManager.Instance.GetConfig<RecoverCoinSkinConfig>()[0].SkinName;
        }
        // Debug.LogError("默认SkinName=Default");
        return "Default";
    }

    public const string ConnectKeyWord = "llllll";
    public static string GetAssetPathWithSkinName(string assetBasePath)
    {
        return assetBasePath.Replace("/RecoverCoin/", "/RecoverCoin"+RecoverCoinModel.ConnectKeyWord + GetSkinName() + "/");
    }
    public static string[] ShowUnCollectRewardsUIList()
    {
        return new[] {UINameConst.UIWaiting, ShowUnCollectRewardsUIWeekStorage.GetAssetPathWithSkinName(UINameConst.UIRecoverCoinMain)};
    }

    public void CleanStorageOnce()
    {
        if (StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey("1.0.69"))
            return;
        var keyStorage = StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig;
        var key = "RecoverCleanStorageOnce";
        if (!keyStorage.ContainsKey(key))
        {
            
            keyStorage.Add(key,"true");
            StorageRecoverCoin.Clear();
        }
    }
    private static StorageRecoverCoinWeek ShowUnCollectRewardsUIWeekStorage;
    public static bool CanShowUnCollectRewardsUI()
    {
        Instance.CleanStorageOnce();
        var weekCanGetReward = GetFirstWeekCanGetReward();
        if (weekCanGetReward == null)
            return false;
        if (!ActivityManager.Instance.IsActivityResourcesDownloaded(Instance.ActivityId))
            return false;
        
        var buyPopup = UIManager.Instance.GetOpenedUIByPath(weekCanGetReward.GetAssetPathWithSkinName(UINameConst.UIRecoverCoinBuy));
        if (buyPopup != null)
        {
            buyPopup.CloseWindowWithinUIMgr();
        }
        if (!weekCanGetReward.IsRealPeopleLeaderBoard())
        {
            OpenMainPopup(weekCanGetReward);
        }
        else
        {
            WaitingManager.Instance.OpenWindow(5f);
            weekCanGetReward.ForceUpdateLeaderBoardFromServer().AddBoolCallBack((success) =>
            {
                WaitingManager.Instance.CloseWindow();
                if (!success)
                    return;
                weekCanGetReward.IsUpdateFinalData = true;
                OpenMainPopup(weekCanGetReward);
            }).WrapErrors();
        }
        ShowUnCollectRewardsUIWeekStorage = weekCanGetReward;
        return true;
    }

    public static UIRecoverCoinMainController OpenMainPopup(StorageRecoverCoinWeek storageWeek)
    {
        if (!storageWeek.IsUpdateFinalData)
        {
            storageWeek.TryUpdateLeaderBoardFromServer().WrapErrors();
        }
        var mainWindow = UIManager.Instance.OpenUI(storageWeek.GetAssetPathWithSkinName(UINameConst.UIRecoverCoinMain)) as UIRecoverCoinMainController;
        storageWeek.SortController().UpdateAll();
        mainWindow.BindStorageWeek(storageWeek);
        return mainWindow;
    }

    public static UIRecoverCoinBuyController OpenBuyStarPopup(StorageRecoverCoinWeek storageWeek)
    {
        var buyStarWindow = UIManager.Instance.OpenUI(storageWeek.GetAssetPathWithSkinName(UINameConst.UIRecoverCoinBuy)) as UIRecoverCoinBuyController;
        buyStarWindow.BindStorageWeek(storageWeek);
        return buyStarWindow;
    }

    
    private static StorageRecoverCoinWeek ShowActivityMainUIWeekStorage;
    public static string[] ShowActivityMainUIList()
    {
        return new[]
        {
            ShowActivityMainUIWeekStorage.GetAssetPathWithSkinName(UINameConst.UIRecoverCoinMain)
        };
    }
    
    private const string coolTimeKey = "RecoverCoin";
    public static bool CanShowMainUIPerDay()
    {
        if (!Instance.IsStart())
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            ShowActivityMainUIWeekStorage = Instance.CurStorageRecoverCoinWeek;
            OpenMainPopup(Instance.CurStorageRecoverCoinWeek);
            return true;
        }
        return false;
    }
    
    public static bool CanShowActivityStartUI()
    {
        if (Instance.IsOpened() && !Instance.CurStorageRecoverCoinWeek.IsStart && GetFirstWeekCanGetReward()==null)
        {
            if (UIManager.Instance.GetOpenedUIByPath(GetAssetPathWithSkinName(UINameConst.UIRecoverCoinStart)) == null)
            {
                var startWindow = UIManager.Instance.OpenUI(GetAssetPathWithSkinName(UINameConst.UIRecoverCoinStart)) as UIRecoverCoinStartController;   
            }
            ShowActivityStartUIWeekStorage = Instance.CurStorageRecoverCoinWeek;
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            return true;
        }
        return false;
    }
    

    private static StorageRecoverCoinWeek ShowActivityStartUIWeekStorage;
    public static string[] ShowActivityStartUIList()
    {
        return new[]
        {
            ShowActivityStartUIWeekStorage.GetAssetPathWithSkinName(UINameConst.UIRecoverCoinStart),
            ShowActivityStartUIWeekStorage.GetAssetPathWithSkinName(UINameConst.UIRecoverCoinMain)
        };
    }

    private static int _lastFinishNodeId = 0;
    public static bool CanShowAllNodeFinishUI()
    {
        if (FarmModel.Instance.IsFarmModel())
            return false;
        
        if (Instance.IsFinishedAllTarget() && !LastFinishNodeList.Contains(_lastFinishNodeId))
        {
            LastFinishNodeList.Clear();
            LastFinishNodeList.Add(_lastFinishNodeId);
            if (!LastFinishNodeList.Contains(0))
            {
                LastFinishNodeList.Add(0);
            }
            var finishAllNodeWindow = UIManager.Instance.OpenUI(GetAssetPathWithSkinName(UINameConst.UIPopupRecoverCoinFinish)) as UIPopupRecoverCoinFinishController;
            return true;
        }
        return false;
    }

    public static string[] ShowAllNodeFinishUI()
    {
        return new[] {GetAssetPathWithSkinName(UINameConst.UIPopupRecoverCoinFinish)};
    }
    public static bool CanShowNewNodeTipUI()
    {
        if (Instance.IsCurWeekExistByStorage() && !Instance.IsFinishedAllTarget() &&
            StorageRecoverCoin.LastDecoNodeId != DecoManager.LastNodeId)
        {
            StorageRecoverCoin.LastDecoNodeId = DecoManager.LastNodeId;
            var newDecoNodeTip = UIManager.Instance.OpenUI(GetAssetPathWithSkinName(UINameConst.UIPopupRecoverCoinNewDecoArea)) as UIPopupRecoverCoinNewDecoAreaController;
            return true;
        }
        return false;
    }
    
    public static string[] ShowNewNodeTipUIList()
    {
        return new[] {GetAssetPathWithSkinName(UINameConst.UIPopupRecoverCoinNewDecoArea)};
    }

    private static int ExistStarCount = 0;
    public static void FlyStar(int rewardNum, Vector2 srcPos,Transform starTransform, float time, bool showEffect, Action action = null)
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
            if (ExistStarCount < 10)
            {
                ExistStarCount++;
                FlyGameObjectManager.Instance.FlyObject(target.gameObject, srcPos, position, showEffect, time, delayTime * i, () =>
                {
                    ExistStarCount--;
                    FlyGameObjectManager.Instance.PlayHintStarsEffect(position);
                    ShakeManager.Instance.ShakeLight();
                    if (index == count - 1)
                    {
                        action?.Invoke();
                    }
                });   
            }
            else
            {
                if (index == count - 1)
                {
                    action?.Invoke();
                }
            }
        }
    }

    public void SetNextWeekStartTime(long startTime)
    {
        if (CurWeekId > 0)
            return;
        foreach (var pair in _weekTimeConfig)
        {
            var weekConfig = pair.Value;
            if (weekConfig.StarTimeSec.ToLong() > CurTime)
            {
                weekConfig.StarTimeSec = (CurTime + startTime).ToString();
                return;
            }
        }
    }
    public long GetNextWeekStartTime()
    {
        if (CurWeekId > 0)
            return 0;
        if (_weekTimeConfig == null)
            return long.MaxValue;
        foreach (var pair in _weekTimeConfig)
        {
            var weekConfig = pair.Value;
            if (weekConfig.StarTimeSec.ToLong() > CurTime)
            {
                return weekConfig.StarTimeSec.ToLong() - CurTime;
            }
        }
        return long.MaxValue;
    }

    public override List<string> GetNeedResList(string activityId,List<string> allResList)
    {
        var skinNameList = new List<string>();
        skinNameList.Add(GetSkinName().ToLower());
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

    public static void AddSkinUIWindowInfo()
    {
        UIManager.Instance._WindowMetaPublic(GetAssetPathWithSkinName(UINameConst.UIRecoverCoinMain), UIWindowLayer.Normal, false);
        UIManager.Instance._WindowMetaPublic(GetAssetPathWithSkinName(UINameConst.UIRecoverCoinStart), UIWindowLayer.Notice,false);
        UIManager.Instance._WindowMetaPublic(GetAssetPathWithSkinName(UINameConst.UIPopupRecoverCoinFinish), UIWindowLayer.Notice,false);
        UIManager.Instance._WindowMetaPublic(GetAssetPathWithSkinName(UINameConst.UIPopupRecoverCoinNewDecoArea), UIWindowLayer.Notice,false);
        UIManager.Instance._WindowMetaPublic(GetAssetPathWithSkinName(UINameConst.UIRecoverCoinBuy), UIWindowLayer.Normal, false);
        UIManager.Instance._WindowMetaPublic(GetAssetPathWithSkinName(UINameConst.UIRecoverCoinEnd), UIWindowLayer.Normal, false);
        var weekList = GetAllWeekCanGetReward();
        foreach (var week in weekList)
        {
            week.AddSkinUIWindowInfo();
        }
    }
    public override bool CanDownLoadRes()
    {
        return true;
    }
}