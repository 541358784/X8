using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public static class CommonLeaderBoardUtils
{
    public static void CompletedStorageActivity(this StorageCommonLeaderBoard leadBoardStorage)
    {
        leadBoardStorage.IsFinish = true;
        leadBoardStorage.GetModelInstance().TryRelease(leadBoardStorage);
    }

    public static bool CanStorageCommonLeaderBoardGetReward(this StorageCommonLeaderBoard storageWeek)//是否可以领奖
    {
        return !storageWeek.IsFinish &&//未领奖
               storageWeek.IsTimeOut() && //时间已经结束
               storageWeek.IsInitFromServer();//进入过榜单
    }

    public static bool IsActive(this StorageCommonLeaderBoard storageWeek)
    {
        return storageWeek.GetStartTime() <= 0 && storageWeek.GetLeftTime() > 0;
    }
    public static bool IsTimeOut(this StorageCommonLeaderBoard storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageCommonLeaderBoard storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageCommonLeaderBoard storageWeek,long leftTime)
    {
        storageWeek.EndTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static string GetLeftTimeText(this StorageCommonLeaderBoard storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    
    public static long GetStartTime(this StorageCommonLeaderBoard storageWeek)
    {
        return Math.Max(storageWeek.StartTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetStartTime(this StorageCommonLeaderBoard storageWeek,long leftTime)
    {
        storageWeek.StartTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }

    private static readonly Dictionary<StorageCommonLeaderBoard, CommonLeaderBoardPlayerSortController> SortControllerPool =
        new Dictionary<StorageCommonLeaderBoard, CommonLeaderBoardPlayerSortController>();
    public static CommonLeaderBoardPlayerSortController SortController(this StorageCommonLeaderBoard storageWeek)
    {
        if (!SortControllerPool.ContainsKey(storageWeek))
        {
            var newSortController = new CommonLeaderBoardPlayerSortController(storageWeek);
            SortControllerPool.Add(storageWeek,newSortController);
        }
        return SortControllerPool[storageWeek];
    }


    public static void CollectStar(this StorageCommonLeaderBoard storage,int addCount,Action callback = null)
    {
        if (storage.IsTimeOut())
            return;
        // if (collectCount <= 0)
        //     return;
        storage.StarCount += addCount;
        storage.StarUpdateTime = APIManager.Instance.GetServerTime();
        EventDispatcher.Instance.SendEventImmediately(new EventCommonLeaderBoardScoreChange(storage));
        if (storage.StarCount >= storage.LeastStarCount)
        {
            if (storage.IsInitFromServer())
            {
                storage.TryUpdateLeaderBoardFromServer().AddBoolCallBack((success) =>
                {
                    if (!success)
                        storage.UploadLeaderBoardToServer().WrapErrors();
                }).WrapErrors();
            }
            else
                storage.EnterLeaderBoard(callback);
        }
    }

    public static List<ResData> GetRewardsByRank(this StorageCommonLeaderBoard storageWeek, int rank)
    {
        var rewardList = new List<ResData>();
        var model = storageWeek.GetModelInstance();
        if (model == null)
            return rewardList;
        return model.GetRewardsByRank(storageWeek, rank);
    }
    public static bool CanStoragePushToServer(this StorageCommonLeaderBoard weekStorage)
    {
        var model = weekStorage.GetModelInstance();
        if (model == null)
            return false;
        var unCollectStorage = model.GetFirstWeekCanGetReward();
        if (unCollectStorage != null && unCollectStorage != weekStorage)
            return false;
        return true;
    }

    public static CommonLeaderBoardModel GetModelInstance(this StorageCommonLeaderBoard weekStorage)
    {
        return CommonLeaderBoardModel.GetInstance(weekStorage.LeaderBoardKeyWord);
    }
    
    private static bool InEnterLeaderBoardLoop = false;
    public static async void EnterLeaderBoard(this StorageCommonLeaderBoard newWeek,Action callback = null)
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
    private static ulong _lastUpdateWeekValueTime = 0;
    private const ulong updateInterval = 1000*10;
    public static async Task<bool> TryUpdateLeaderBoardFromServer(this StorageCommonLeaderBoard weekStorage)
    {
        var serverTime = APIManager.Instance.GetServerTime();
        if (serverTime - _lastUpdateWeekValueTime < updateInterval)
            return false;
        _lastUpdateWeekValueTime = serverTime;
        return await weekStorage.ForceUpdateLeaderBoardFromServer();
    }

    private static Dictionary<StorageCommonLeaderBoard, int> ForceUpdateWeekValueAckDic =
        new Dictionary<StorageCommonLeaderBoard, int>();

    public static async Task<bool> ForceUpdateLeaderBoardFromServer(this StorageCommonLeaderBoard weekStorage)
    {
        // WaitingManager.Instance.OpenWindow();
        
        if (!weekStorage.CanStoragePushToServer())
            return false;
        if (!weekStorage.IsInitFromServer() && weekStorage.StarCount < weekStorage.LeastStarCount)
            return false;
        ForceUpdateWeekValueAckDic.TryAdd(weekStorage, 0);
        ForceUpdateWeekValueAckDic[weekStorage]++;
        int tempAck = ForceUpdateWeekValueAckDic[weekStorage];
        var uniqueArgs = weekStorage.GetUniqueArgs();
        var globalData = JsonConvert.SerializeObject(uniqueArgs);
        var myPlayerStorage = new CommonLeaderBoardPlayerServerStruct()
        {
            AvatarIconId = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.AvatarIconId,
            AvatarIconFrameId = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.GetUserAvatarFrame().id,
            PlayerId = StorageManager.Instance.GetStorage<StorageCommon>().PlayerId,
            PlayerName = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName,
            StarCount = weekStorage.StarCount,
            StarUpdateTime = weekStorage.StarUpdateTime,
            ViewState = HeadIconUtils.GetMyViewState(),
        };
        var extraInfo = JsonConvert.SerializeObject(myPlayerStorage);
        var taskCallback = new TaskCompletionSource<bool>();
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.CreateOrGet, weekStorage.LeaderBoardKeyWord,
            uniqueArgs, globalData,
            (success, requestType, leaderBoardListItem) =>
            {
                // WaitingManager.Instance.CloseUI();
                if (ForceUpdateWeekValueAckDic[weekStorage] != tempAck)
                {
                    taskCallback.SetResult(false);
                    return;
                }
                if (!success)
                {
                    taskCallback.SetResult(false);
                    return;
                }

                if (leaderBoardListItem.Info.Extra != globalData)
                {
                    LeaderBoardModel.Request(LeaderBoardModel.RequestType.Logout,weekStorage.LeaderBoardKeyWord,null,null,
                        (success, requestType, leaderBoardListItem) =>
                        {
                            
                        });
                    taskCallback.SetResult(false);
                    return;
                }
                weekStorage.LeaderBoardId = leaderBoardListItem.Me.LeaderboardId;
                CommonLeaderBoardUtils.StorageWeekInitStateDictionary[weekStorage.ActivityId] = true;
                weekStorage.SortController().UpdateAllPlayerState(leaderBoardListItem.LeaderboardEntries);
                weekStorage.SortController().UpdateMe(leaderBoardListItem.Me);
                taskCallback.SetResult(true);
            }, weekStorage.StarCount, extraInfo, (uint) weekStorage.MaxPlayerCount);
        return await taskCallback.Task;
    }

    public static async Task<bool> QuitLeaderBoardFromServer(this StorageCommonLeaderBoard weekStorage)
    {
        if (!weekStorage.CanStoragePushToServer())
            return false;
        if (!weekStorage.IsInitFromServer())
            return false;
        WaitingManager.Instance.OpenWindow();
        var uniqueArgs = weekStorage.GetUniqueArgs();
        var globalData = JsonConvert.SerializeObject(uniqueArgs);
        var taskCallback = new TaskCompletionSource<bool>();
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.Logout,weekStorage.LeaderBoardKeyWord,uniqueArgs,globalData,
            (success, requestType, leaderBoardListItem) =>
            {
                WaitingManager.Instance.CloseWindow();
                if (!success)
                {
                    taskCallback.SetResult(false);
                    return;
                }
                taskCallback.SetResult(true);
            });
        return await taskCallback.Task;
    }

    public static async Task<bool> UploadLeaderBoardToServer(this StorageCommonLeaderBoard weekStorage)
    {
        if (!weekStorage.CanStoragePushToServer())
            return false;
        if (!weekStorage.IsInitFromServer())
            return false;
        var uniqueArgs = weekStorage.GetUniqueArgs();
        var globalData = JsonConvert.SerializeObject(uniqueArgs);
        var myPlayerStorage = new CommonLeaderBoardPlayerServerStruct()
        {
            AvatarIconId = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.AvatarIconId,
            AvatarIconFrameId = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.GetUserAvatarFrame().id,
            PlayerId = StorageManager.Instance.GetStorage<StorageCommon>().PlayerId,
            PlayerName = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName,
            StarCount = weekStorage.StarCount,
            StarUpdateTime = weekStorage.StarUpdateTime,
            ViewState = HeadIconUtils.GetMyViewState(),
        };
        var extraInfo = JsonConvert.SerializeObject(myPlayerStorage);
        var taskCallback = new TaskCompletionSource<bool>();
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.Update,weekStorage.LeaderBoardKeyWord,uniqueArgs,globalData,
            (success, requestType, leaderBoardListItem) =>
            {
                if (!success)
                {
                    taskCallback.SetResult(false);
                    return;
                }
                taskCallback.SetResult(true);
            },weekStorage.StarCount, extraInfo);
        return await taskCallback.Task;
    }

    public static Dictionary<string, string> GetUniqueArgs(this StorageCommonLeaderBoard weekStorage)
    {
        var uniqueArgs = new Dictionary<string, string>()
        {
            {"ActivityId", weekStorage.ActivityId.ToString()}
        };
        return uniqueArgs;
    }

    public static bool IsInitFromServer(this StorageCommonLeaderBoard weekStorage)
    {
        return !weekStorage.LeaderBoardId.IsEmptyString();
    }

    public static Dictionary<string, bool> StorageWeekInitStateDictionary = new Dictionary<string, bool>();
    public static bool IsStorageWeekInitFromServer(this StorageCommonLeaderBoard weekStorage)
    {
        return StorageWeekInitStateDictionary.TryGetValue(weekStorage.ActivityId, out var value) ? value:false;
    }
    public static bool IsResExist(this StorageCommonLeaderBoard weekStorage)
    {
        return ActivityManager.Instance.CheckResExist(weekStorage.ActivityResList);
    }
}