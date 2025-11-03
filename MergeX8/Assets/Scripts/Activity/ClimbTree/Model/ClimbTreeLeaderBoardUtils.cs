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

public static class ClimbTreeLeaderBoardUtils
{
    private static readonly Dictionary<StorageClimbTreeLeaderBoard, List<ClimbTreeLeaderBoardRewardConfig>> RewardConfigPool =
        new Dictionary<StorageClimbTreeLeaderBoard, List<ClimbTreeLeaderBoardRewardConfig>>();
    public static List<ClimbTreeLeaderBoardRewardConfig> RewardConfig(this StorageClimbTreeLeaderBoard storageWeek)
    {
        if (storageWeek.JsonRecoverCoinRewardConfig.IsEmptyString())
            return null;
        if (!RewardConfigPool.ContainsKey(storageWeek))
        {
            var newRewardConfig = JsonConvert.DeserializeObject<List<ClimbTreeLeaderBoardRewardConfig>>(storageWeek.JsonRecoverCoinRewardConfig);
            RewardConfigPool.Add(storageWeek,newRewardConfig);
        }
        return RewardConfigPool[storageWeek];
    }

    public static bool CanStorageClimbTreeLeaderBoardGetReward(this StorageClimbTreeLeaderBoard storageWeek)//是否可以领奖
    {
        return !storageWeek.IsFinish &&//未领奖
               storageWeek.IsTimeOut() && //时间已经结束
               storageWeek.IsInitFromServer();//进入过榜单
    }

    public static bool IsTimeOut(this StorageClimbTreeLeaderBoard storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageClimbTreeLeaderBoard storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageClimbTreeLeaderBoard storageWeek,long leftTime)
    {
        storageWeek.EndTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static string GetLeftTimeText(this StorageClimbTreeLeaderBoard storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }

    private static readonly Dictionary<StorageClimbTreeLeaderBoard, ClimbTreeLeaderBoardPlayerSortController> SortControllerPool =
        new Dictionary<StorageClimbTreeLeaderBoard, ClimbTreeLeaderBoardPlayerSortController>();
    public static ClimbTreeLeaderBoardPlayerSortController SortController(this StorageClimbTreeLeaderBoard storageWeek)
    {
        if (!SortControllerPool.ContainsKey(storageWeek))
        {
            var newSortController = new ClimbTreeLeaderBoardPlayerSortController(storageWeek);
            SortControllerPool.Add(storageWeek,newSortController);
        }
        return SortControllerPool[storageWeek];
    }

    public static List<ResData> GetRewardsByRank(this List<ClimbTreeLeaderBoardRewardConfig> rewardConfig, int rank)
    {
        var rewardList = new List<ResData>();
        if (rewardConfig == null || rewardConfig.Count == 0)
        {
            return rewardList;
        }
        ClimbTreeLeaderBoardRewardConfig targetConfig = null;
        for (var i = 0; i < rewardConfig.Count; i++)
        {
            var cfg = rewardConfig[i];
            if (cfg.RankMin <= rank && cfg.RankMax >= rank)
            {
                targetConfig = cfg;
                break;
            }
        }
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
    

    public static void CollectStar(this StorageClimbTreeLeaderBoard storage,int currentCount)
    {
        if (storage.IsTimeOut())
            return;
        // if (collectCount <= 0)
        //     return;
        storage.StarCount = currentCount;
        storage.StarUpdateTime = APIManager.Instance.GetServerTime();
        
        if (storage.StarCount >= ClimbTreeLeaderBoardModel.Instance.LeastEnterBoardScore)
        {
            if (storage.IsInitFromServer())
                // storage.UploadLeaderBoardToServer().WrapErrors();
                storage.TryUpdateLeaderBoardFromServer().WrapErrors();
            else
                ClimbTreeLeaderBoardModel.Instance.EnterLeaderBoard(storage,() =>
                {
                    if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                    {
                        MergeGuideLogic.Instance.CheckClimbTreeLogic();
                    }
                });
        }
    }

    private static ulong _lastUpdateWeekValueTime = 0;
    private const ulong updateInterval = 1000*10;
    public static async Task<bool> TryUpdateLeaderBoardFromServer(this StorageClimbTreeLeaderBoard weekStorage)
    {
        if (ClimbTreeLeaderBoardModel.GetFirstWeekCanGetReward() != null)
            return false;
        var serverTime = APIManager.Instance.GetServerTime();
        if (serverTime - _lastUpdateWeekValueTime < updateInterval)
            return false;
        _lastUpdateWeekValueTime = serverTime;
        return await weekStorage.ForceUpdateLeaderBoardFromServer();
    }

    private static int _forceUpdateWeekValueAck = 0;

    public static Dictionary<string, string> GetUniqueArgs(this StorageClimbTreeLeaderBoard weekStorage)
    {
        var uniqueArgs = new Dictionary<string, string>()
        {
            {"ActivityId", weekStorage.ActivityId.ToString()},
            {"Version","1"},
        };
        return uniqueArgs;
    }
    public static async Task<bool> ForceUpdateLeaderBoardFromServer(this StorageClimbTreeLeaderBoard weekStorage)
    {
        // WaitingManager.Instance.OpenWindow();
        if (!weekStorage.IsInitFromServer() && weekStorage.StarCount < ClimbTreeLeaderBoardModel.Instance.LeastEnterBoardScore)
            return false;
        _forceUpdateWeekValueAck++;
        int tempAck = _forceUpdateWeekValueAck;
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
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.CreateOrGet, ClimbTreeLeaderBoardModel.LeadBoardAPITypeName,
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
                    LeaderBoardModel.Request(LeaderBoardModel.RequestType.Logout,ClimbTreeLeaderBoardModel.LeadBoardAPITypeName,null,null,
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

    public static async Task<bool> QuitLeaderBoardFromServer(this StorageClimbTreeLeaderBoard weekStorage)
    {
        if (!weekStorage.IsInitFromServer())
            return false;
        WaitingManager.Instance.OpenWindow();
        var uniqueArgs = weekStorage.GetUniqueArgs();
        var globalData = JsonConvert.SerializeObject(uniqueArgs);
        var taskCallback = new TaskCompletionSource<bool>();
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.Logout,ClimbTreeLeaderBoardModel.LeadBoardAPITypeName,uniqueArgs,globalData,
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

    public static async Task<bool> UploadLeaderBoardToServer(this StorageClimbTreeLeaderBoard weekStorage)
    {
        if (!weekStorage.IsInitFromServer())
            return false;
        if (ClimbTreeLeaderBoardModel.GetFirstWeekCanGetReward() != null)
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
        };
        var extraInfo = JsonConvert.SerializeObject(myPlayerStorage);
        var taskCallback = new TaskCompletionSource<bool>();
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.Update,ClimbTreeLeaderBoardModel.LeadBoardAPITypeName,uniqueArgs,globalData,
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

    public static void SetValue(this CommonLeaderBoardPlayerServerStruct srcPlayer,CommonLeaderBoardPlayerServerStruct dstPlayer)
    {
        srcPlayer.PlayerId = dstPlayer.PlayerId;
        srcPlayer.PlayerName = dstPlayer.PlayerName;
        srcPlayer.StarCount = dstPlayer.StarCount;
        srcPlayer.AvatarIconId = dstPlayer.AvatarIconId;
        srcPlayer.AvatarIconFrameId = dstPlayer.AvatarIconFrameId;
        srcPlayer.StarUpdateTime = srcPlayer.StarUpdateTime;
    }

    public static bool IsInitFromServer(this StorageClimbTreeLeaderBoard weekStorage)
    {
        return !weekStorage.LeaderBoardId.IsEmptyString();
    }

    public static Dictionary<string, bool> StorageWeekInitStateDictionary = new Dictionary<string, bool>();
    public static bool IsStorageWeekInitFromServer(this StorageClimbTreeLeaderBoard weekStorage)
    {
        return StorageWeekInitStateDictionary[weekStorage.ActivityId];
    }
    public static bool IsResExist(this StorageClimbTreeLeaderBoard weekStorage)
    {
        return ActivityManager.Instance.CheckResExist(weekStorage.ActivityResList) || ActivityManager.Instance.IsActivityResourcesDownloaded(ClimbTreeModel.Instance.ActivityId);
    }
}