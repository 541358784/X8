using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deco.Node;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThemeDecorationLeaderBoardModel
{
    public static bool InGuideChain = false;
    public static string LeadBoardAPITypeName = "ThemeDecorationLeaderBoard";
    private static ThemeDecorationLeaderBoardModel _instance;
    public static ThemeDecorationLeaderBoardModel Instance => _instance ?? (_instance = new ThemeDecorationLeaderBoardModel());
    
    

    public int MaxPlayerCount => ThemeDecorationModel.Instance.GlobalConfig.MaxPlayerCount;
    public StorageThemeDecoration CurMainStorage => ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek;
    public StorageThemeDecorationLeaderBoard CurStorageThemeDecorationLeaderBoardWeek
    {
        get
        {
            if (CurMainStorage == null)
                return null;
            if (CurMainStorage.GetUnCollectReward() != null)
                return null;
            if (CurMainStorage.GetUnCollectLeaderBoard() != null)
                return null;
            var activeStorage = CurMainStorage.LeaderBoardStorageList.Find(a => a.IsActive());
            return activeStorage;
        }
    }
    
    public void CreateStorage(StorageThemeDecoration mainStorage)
    {
        foreach (var config in ThemeDecorationModel.Instance.LeaderBoardScheduleConfig)
        {
            var activityId = mainStorage.ActivityId + "_" +config.Id;
            var newWeek = mainStorage.LeaderBoardStorageList.Find(a => a.ActivityId == activityId);
            var rewardConfig = new List<ThemeDecorationLeaderBoardRewardConfig>();
            var startTime = mainStorage.StartTime + (long)((ulong)config.StartTime * XUtility.Min);
            var endTime = mainStorage.StartTime + (long)((ulong)config.EndTime * XUtility.Min);
            for (var i = 0; i < config.RewardConfigList.Count; i++)
            {
                rewardConfig.Add(ThemeDecorationModel.Instance.LeaderBoardRewardConfig[config.RewardConfigList[i]]);
            }
            if (newWeek != null)
            {
                newWeek.JsonRewardConfig = JsonConvert.SerializeObject(rewardConfig);
                newWeek.EndTime = endTime;
                newWeek.StartTime = startTime;
            }
            else
            {
                newWeek = new StorageThemeDecorationLeaderBoard();
                newWeek.JsonRewardConfig = JsonConvert.SerializeObject(rewardConfig);
                newWeek.EndTime = endTime;
                newWeek.StartTime = startTime;
                newWeek.StarCount = 0;
                newWeek.IsFinish = false;
                newWeek.IsStart = true;
                newWeek.MaxPlayerCount = MaxPlayerCount;
                newWeek.ActivityId = activityId;
                newWeek.StarUpdateTime = APIManager.Instance.GetServerTime();
                newWeek.IsUpdateFinalData = false;
                newWeek.ActivityResList.Clear();
                foreach (var res in mainStorage.ActivityResList)
                {
                    newWeek.ActivityResList.Add(res);
                }
                newWeek.LeastStarCount = config.LeastEnterBoardScore;
                newWeek.SkinName = mainStorage.SkinName;
                mainStorage.LeaderBoardStorageList.Add(newWeek);
                ThemeDecorationLeaderBoardUtils.StorageWeekInitStateDictionary.Add(newWeek.ActivityId,false);
            }
        }
    }
    private bool InEnterLeaderBoardLoop;
    public async void EnterLeaderBoard(StorageThemeDecorationLeaderBoard newWeek,Action callback = null)
    {
        if (InEnterLeaderBoardLoop)
            return;
        InEnterLeaderBoardLoop = true;
        Debug.LogError("新周期创建并与后端通信");
        Action OnSuccess = () =>
        {
            WaitingManager.Instance.CloseWindow();
            if (callback != null)
                callback();
        };
        WaitingManager.Instance.OpenWindow(5f);
        var maxRepeatCount = 10;
        for (var i = 0; i < maxRepeatCount; i++)
        {
            var success = await newWeek.ForceUpdateLeaderBoardFromServer();
            if (success)
            {
                Debug.LogError("新周期创建成功");
                OnSuccess();
                break;
            }
            else
            {
                Debug.LogError("新周期创建失败");
                if (i == maxRepeatCount - 1)
                {
                    WaitingManager.Instance.CloseWindow();
                }
                else
                {
                    Debug.LogError("再次创建新周期");
                }
            }
        }
        Debug.LogError("新周期创建循环退出");
        InEnterLeaderBoardLoop = false;
    }
    

    public void InitFromServerData()
    {
        ThemeDecorationLeaderBoardUtils.StorageWeekInitStateDictionary.Clear();
        if (CurStorageThemeDecorationLeaderBoardWeek != null)
            ThemeDecorationLeaderBoardUtils.StorageWeekInitStateDictionary.Add(CurStorageThemeDecorationLeaderBoardWeek.ActivityId,false);
        XUtility.WaitFrames(1, () =>
        {
            if (IsStart())
            {
                CurStorageThemeDecorationLeaderBoardWeek.ForceUpdateLeaderBoardFromServer().WrapErrors();
            }
        });
        UpdateTime();
    }

    public bool IsStart()
    {
        return CurStorageThemeDecorationLeaderBoardWeek != null;
    }

    public bool IsStartAndStorageInitFromServer()
    {
        return IsStart() && CurStorageThemeDecorationLeaderBoardWeek.IsStorageWeekInitFromServer();
    }

    public void AddStar(int addCount)
    {
        if (!IsStart())
            return;
        CurStorageThemeDecorationLeaderBoardWeek.CollectStar(addCount);
    }

    public int GetStar()
    {
        if (!IsStart())
            return 0;
        return CurStorageThemeDecorationLeaderBoardWeek.StarCount;
    }

    public static StorageThemeDecorationLeaderBoard GetFirstWeekCanGetReward()
    {
        var ThemeDecorationStorage = ThemeDecorationModel.GetFirstWeekCanGetLeaderBoardReward();
        if (ThemeDecorationStorage == null)
            return null;
        foreach (var leaderBoard in ThemeDecorationStorage.LeaderBoardStorageList)
        {
            if (leaderBoard.CanStorageThemeDecorationLeaderBoardGetReward())
                return leaderBoard;   
        }
        return null;
    }

    public static StorageThemeDecorationLeaderBoard ShowUnCollectRewardsUIWeekStorage = null;
    public static string[] ShowUnCollectRewardsUIList()
    {
        return new[] {UINameConst.UIWaiting, ShowUnCollectRewardsUIWeekStorage.GetAssetPathWithSkinName(UINameConst.UIThemeDecorationLeaderBoardMain)};
    }
    public static bool CanShowUnCollectRewardsUI()
    {
        var weekCanGetReward = GetFirstWeekCanGetReward();
        if (weekCanGetReward == null)
            return false;
        if (!weekCanGetReward.IsResExist())
            return false;

        WaitingManager.Instance.OpenWindow(5f);
        weekCanGetReward.ForceUpdateLeaderBoardFromServer().AddBoolCallBack((success) =>
        {
            WaitingManager.Instance.CloseWindow();
            if (!success)
                return;
            weekCanGetReward.IsUpdateFinalData = true;
            OpenMainPopup(weekCanGetReward);
        }).WrapErrors();
        ShowUnCollectRewardsUIWeekStorage = weekCanGetReward;
        return true;
    }

    public static UIThemeDecorationLeaderBoardMainController OpenMainPopup(StorageThemeDecorationLeaderBoard storageWeek)
    {
        if (!storageWeek.IsUpdateFinalData)
        {
            storageWeek.TryUpdateLeaderBoardFromServer().WrapErrors();
        }
        return UIThemeDecorationLeaderBoardMainController.Open(storageWeek);
    }
    private const string coolTimeKey = "ThemeDecorationLeaderBoard";
    public static bool CanShowMainUIPerDay()
    {
        if (!Instance.IsStart())
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            ShowMainUIWeekStorage = Instance.CurStorageThemeDecorationLeaderBoardWeek;
            OpenMainPopup(Instance.CurStorageThemeDecorationLeaderBoardWeek);
            return true;
        }
        return false;
    }
    public static StorageThemeDecorationLeaderBoard ShowMainUIWeekStorage = null;
    public static string[] ShowMainUIWeekList()
    {
        return new[] {ShowMainUIWeekStorage.GetAssetPathWithSkinName(UINameConst.UIThemeDecorationLeaderBoardMain)};
    }
    //目前不需要排行榜开始弹窗
    // public static bool CanShowActivityStartUI()
    // {
    //     if (Instance.IsPrivateOpened() && !Instance.CurStorageThemeDecorationLeaderBoardWeek.IsStart)
    //     {
    //         if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIThemeDecorationLeaderBoardStart) == null)
    //         {
    //             var startWindow =
    //                 UIManager.Instance.OpenUI(UINameConst.UIThemeDecorationLeaderBoardStart) as
    //                     UIThemeDecorationLeaderBoardStartController;
    //         }
    //         CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
    //         return true;
    //     }
    //
    //     return false;
    // }
    
    
    public ThemeDecorationLeaderBoardModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }

    private StorageThemeDecorationLeaderBoard _lastActivityOpenState;
    public void UpdateTime()
    {
        if (!ThemeDecorationModel.Instance.IsInitFromServer())
            return;
        var currentActivityOpenState = CurStorageThemeDecorationLeaderBoardWeek;
        if (_lastActivityOpenState == currentActivityOpenState)
            return;
        if (currentActivityOpenState == null)
        {
            CanShowUnCollectRewardsUI();
        }
        _lastActivityOpenState = currentActivityOpenState;
    }
    public void InitAux()
    {
    }
    public string GetAuxItemAssetPath()
    {
        if (CurStorageThemeDecorationLeaderBoardWeek == null)
            return null;
        
        return ThemeDecorationLeaderBoardUtils.GetAuxItemAssetPath(CurStorageThemeDecorationLeaderBoardWeek);
    }

    public bool ShowEntrance()
    {
        if (CurStorageThemeDecorationLeaderBoardWeek == null)
            return false;

        return ThemeDecorationLeaderBoardUtils.ShowEntrance(CurStorageThemeDecorationLeaderBoardWeek);
    }
    
    
    public bool ShowMergeEntrance()
    {
        if (CurStorageThemeDecorationLeaderBoardWeek == null)
            return false;

        return CurStorageThemeDecorationLeaderBoardWeek.ShowEntrance();
    }

    public string GetMergeItemAssetPath()
    {
        if (CurStorageThemeDecorationLeaderBoardWeek == null)
            return null;

        return CurStorageThemeDecorationLeaderBoardWeek.GetTaskItemAssetPath();
    }
}