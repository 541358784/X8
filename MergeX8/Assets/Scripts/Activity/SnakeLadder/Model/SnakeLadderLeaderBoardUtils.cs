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

public static class SnakeLadderLeaderBoardUtils
{
    private static readonly Dictionary<StorageSnakeLadderLeaderBoard, List<SnakeLadderLeaderBoardRewardConfig>> RewardConfigPool =
        new Dictionary<StorageSnakeLadderLeaderBoard, List<SnakeLadderLeaderBoardRewardConfig>>();
    public static List<SnakeLadderLeaderBoardRewardConfig> RewardConfig(this StorageSnakeLadderLeaderBoard storageWeek)
    {
        if (storageWeek.JsonRecoverCoinRewardConfig.IsEmptyString())
            return null;
        if (!RewardConfigPool.ContainsKey(storageWeek))
        {
            var newRewardConfig = JsonConvert.DeserializeObject<List<SnakeLadderLeaderBoardRewardConfig>>(storageWeek.JsonRecoverCoinRewardConfig);
            RewardConfigPool.Add(storageWeek,newRewardConfig);
        }
        return RewardConfigPool[storageWeek];
    }

    public static bool CanStorageSnakeLadderLeaderBoardGetReward(this StorageSnakeLadderLeaderBoard storageWeek)//是否可以领奖
    {
        return !storageWeek.IsFinish &&//未领奖
               storageWeek.IsTimeOut() && //时间已经结束
               storageWeek.IsInitFromServer();//进入过榜单
    }

    public static bool IsTimeOut(this StorageSnakeLadderLeaderBoard storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageSnakeLadderLeaderBoard storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageSnakeLadderLeaderBoard storageWeek,long leftTime)
    {
        storageWeek.EndTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static string GetLeftTimeText(this StorageSnakeLadderLeaderBoard storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }

    private static readonly Dictionary<StorageSnakeLadderLeaderBoard, SnakeLadderLeaderBoardPlayerSortController> SortControllerPool =
        new Dictionary<StorageSnakeLadderLeaderBoard, SnakeLadderLeaderBoardPlayerSortController>();
    public static SnakeLadderLeaderBoardPlayerSortController SortController(this StorageSnakeLadderLeaderBoard storageWeek)
    {
        if (!SortControllerPool.ContainsKey(storageWeek))
        {
            var newSortController = new SnakeLadderLeaderBoardPlayerSortController(storageWeek);
            SortControllerPool.Add(storageWeek,newSortController);
        }
        return SortControllerPool[storageWeek];
    }

    public static List<ResData> GetRewardsByRank(this List<SnakeLadderLeaderBoardRewardConfig> rewardConfig, int rank)
    {
        SnakeLadderLeaderBoardRewardConfig targetConfig = null;
        for (var i = 0; i < rewardConfig.Count; i++)
        {
            var cfg = rewardConfig[i];
            if (cfg.RankMin <= rank && cfg.RankMax >= rank)
            {
                targetConfig = cfg;
                break;
            }
        }

        var rewardList = new List<ResData>();
        if (targetConfig == null)
            return rewardList;
        if (targetConfig.RewardId == null)
            return rewardList;
        for (var i = 0; i < targetConfig.RewardId.Count; i++)
        {
            rewardList.Add(new ResData(targetConfig.RewardId[i],targetConfig.RewardNum[i]));
        }
        return rewardList;
    }
    

    public static void CollectStar(this StorageSnakeLadderLeaderBoard storage,int currentCount)
    {
        if (storage.IsTimeOut())
            return;
        // if (collectCount <= 0)
        //     return;
        storage.StarCount = currentCount;
        storage.StarUpdateTime = APIManager.Instance.GetServerTime();
        
        if (storage.StarCount >= SnakeLadderLeaderBoardModel.Instance.LeastEnterBoardScore)
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
                SnakeLadderLeaderBoardModel.Instance.EnterLeaderBoard(storage);
        }
    }

    private static ulong _lastUpdateWeekValueTime = 0;
    private const ulong updateInterval = 1000*10;
    public static async Task<bool> TryUpdateLeaderBoardFromServer(this StorageSnakeLadderLeaderBoard weekStorage)
    {
        if (SnakeLadderLeaderBoardModel.GetFirstWeekCanGetReward() != null)
            return false;
        var serverTime = APIManager.Instance.GetServerTime();
        if (serverTime - _lastUpdateWeekValueTime < updateInterval)
            return false;
        _lastUpdateWeekValueTime = serverTime;
        return await weekStorage.ForceUpdateLeaderBoardFromServer();
    }

    private static int _forceUpdateWeekValueAck = 0;

    public static Dictionary<string, string> GetUniqueArgs(this StorageSnakeLadderLeaderBoard weekStorage)
    {
        var uniqueArgs = new Dictionary<string, string>()
        {
            {"ActivityId", weekStorage.ActivityId.ToString()},
            {"Version","1"},
        };
        return uniqueArgs;
    }
    public static async Task<bool> ForceUpdateLeaderBoardFromServer(this StorageSnakeLadderLeaderBoard weekStorage)
    {
        // WaitingManager.Instance.OpenWindow();
        if (!weekStorage.IsInitFromServer() && weekStorage.StarCount < SnakeLadderLeaderBoardModel.Instance.LeastEnterBoardScore)
            return false;
        _forceUpdateWeekValueAck++;
        int tempAck = _forceUpdateWeekValueAck;
        var uniqueArgs = weekStorage.GetUniqueArgs();
        var globalData = JsonConvert.SerializeObject(uniqueArgs);
        var myPlayerStorage = new SnakeLadderLeaderBoardPlayerServerStruct()
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
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.CreateOrGet, SnakeLadderLeaderBoardModel.LeadBoardAPITypeName,
            uniqueArgs, globalData,
            (success, requestType, leaderBoardListItem) =>
            {
                // WaitingManager.Instance.CloseUI();
                if (_forceUpdateWeekValueAck != tempAck)
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
                    LeaderBoardModel.Request(LeaderBoardModel.RequestType.Logout,SnakeLadderLeaderBoardModel.LeadBoardAPITypeName,null,null,
                        (success, requestType, leaderBoardListItem) =>
                        {
                            
                        });
                    taskCallback.SetResult(false);
                    return;
                }
                weekStorage.LeaderBoardId = leaderBoardListItem.Me.LeaderboardId;
                StorageWeekInitStateDictionary[weekStorage.ActivityId] = true;
                weekStorage.SortController().UpdateAllPlayerState(leaderBoardListItem.LeaderboardEntries);
                weekStorage.SortController().UpdateMe(leaderBoardListItem.Me);
                taskCallback.SetResult(true);
            }, weekStorage.StarCount, extraInfo, (uint) weekStorage.MaxPlayerCount);
        return await taskCallback.Task;
    }

    public static async Task<bool> QuitLeaderBoardFromServer(this StorageSnakeLadderLeaderBoard weekStorage)
    {
        if (!weekStorage.IsInitFromServer())
            return false;
        WaitingManager.Instance.OpenWindow();
        var uniqueArgs = weekStorage.GetUniqueArgs();
        var globalData = JsonConvert.SerializeObject(uniqueArgs);
        var taskCallback = new TaskCompletionSource<bool>();
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.Logout,SnakeLadderLeaderBoardModel.LeadBoardAPITypeName,uniqueArgs,globalData,
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

    public static async Task<bool> UploadLeaderBoardToServer(this StorageSnakeLadderLeaderBoard weekStorage)
    {
        if (!weekStorage.IsInitFromServer())
            return false;
        if (SnakeLadderLeaderBoardModel.GetFirstWeekCanGetReward() != null)
            return false;
        var uniqueArgs = weekStorage.GetUniqueArgs();
        var globalData = JsonConvert.SerializeObject(uniqueArgs);
        var myPlayerStorage = new SnakeLadderLeaderBoardPlayerServerStruct()
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
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.Update,SnakeLadderLeaderBoardModel.LeadBoardAPITypeName,uniqueArgs,globalData,
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

    public static void SetValue(this SnakeLadderLeaderBoardPlayerServerStruct srcPlayer,SnakeLadderLeaderBoardPlayerServerStruct dstPlayer)
    {
        srcPlayer.PlayerId = dstPlayer.PlayerId;
        srcPlayer.PlayerName = dstPlayer.PlayerName;
        srcPlayer.StarCount = dstPlayer.StarCount;
        srcPlayer.AvatarIconId = dstPlayer.AvatarIconId;
        srcPlayer.AvatarIconFrameId = dstPlayer.AvatarIconFrameId;
        srcPlayer.StarUpdateTime = srcPlayer.StarUpdateTime;
    }

    public static bool IsInitFromServer(this StorageSnakeLadderLeaderBoard weekStorage)
    {
        return !weekStorage.LeaderBoardId.IsEmptyString();
    }

    public static Dictionary<string, bool> StorageWeekInitStateDictionary = new Dictionary<string, bool>();
    public static bool IsStorageWeekInitFromServer(this StorageSnakeLadderLeaderBoard weekStorage)
    {
        return StorageWeekInitStateDictionary[weekStorage.ActivityId];
    }
    public static bool IsResExist(this StorageSnakeLadderLeaderBoard weekStorage)
    {
        return ActivityManager.Instance.CheckResExist(weekStorage.ActivityResList) || ActivityManager.Instance.IsActivityResourcesDownloaded(SnakeLadderModel.Instance.ActivityId);
    }
}