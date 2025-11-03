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

public static class ThemeDecorationLeaderBoardUtils
{
    public static void CompletedStorageActivity(this StorageThemeDecorationLeaderBoard leadBoardStorage)
    {
        leadBoardStorage.IsFinish = true;
        // leadBoardStorage.TryRelease();
    }

    public static bool TryRelease(this StorageThemeDecorationLeaderBoard storage)
    {
        var mainStorage = storage.MainStorage();
        if (storage.IsTimeOut() && (!storage.IsInitFromServer() || storage.IsFinish))
        {
            Debug.LogError("删除ActivityId = " + storage.ActivityId + "排行榜数据");
            if (mainStorage != null)
            {
                mainStorage.LeaderBoardStorageList.Remove(storage);   
            }
            ThemeDecorationLeaderBoardUtils.StorageWeekInitStateDictionary.Remove(storage.ActivityId);
            return true;
        }

        return false;
    }
    public static StorageThemeDecoration MainStorage(this StorageThemeDecorationLeaderBoard storageWeek)
    {
        foreach (var pair in ThemeDecorationModel.StorageThemeDecoration)
        {
            var storage = pair.Value;
            if (storage.LeaderBoardStorageList.Find(a => a == storageWeek) != null)
            {
                return storage;
            }
        }
        return null;
    }
    private static readonly Dictionary<StorageThemeDecorationLeaderBoard, List<ThemeDecorationLeaderBoardRewardConfig>> RewardConfigPool =
        new Dictionary<StorageThemeDecorationLeaderBoard, List<ThemeDecorationLeaderBoardRewardConfig>>();
    public static List<ThemeDecorationLeaderBoardRewardConfig> RewardConfig(this StorageThemeDecorationLeaderBoard storageWeek)
    {
        if (storageWeek.JsonRewardConfig.IsEmptyString())
            return null;
        if (!RewardConfigPool.ContainsKey(storageWeek))
        {
            var newRewardConfig = JsonConvert.DeserializeObject<List<ThemeDecorationLeaderBoardRewardConfig>>(storageWeek.JsonRewardConfig);
            RewardConfigPool.Add(storageWeek,newRewardConfig);
        }
        return RewardConfigPool[storageWeek];
    }

    public static bool CanStorageThemeDecorationLeaderBoardGetReward(this StorageThemeDecorationLeaderBoard storageWeek)//是否可以领奖
    {
        return !storageWeek.IsFinish &&//未领奖
               storageWeek.IsTimeOut() && //时间已经结束
               storageWeek.IsInitFromServer();//进入过榜单
    }

    public static bool IsActive(this StorageThemeDecorationLeaderBoard storageWeek)
    {
        var curTime = (long)APIManager.Instance.GetServerTime();
        return curTime > storageWeek.StartTime && curTime < storageWeek.EndTime;
    }
    public static bool IsTimeOut(this StorageThemeDecorationLeaderBoard storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageThemeDecorationLeaderBoard storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageThemeDecorationLeaderBoard storageWeek,long leftTime)
    {
        storageWeek.EndTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static string GetLeftTimeText(this StorageThemeDecorationLeaderBoard storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    
    public static long GetStartTime(this StorageThemeDecorationLeaderBoard storageWeek)
    {
        return Math.Max(storageWeek.StartTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetStartTime(this StorageThemeDecorationLeaderBoard storageWeek,long leftTime)
    {
        storageWeek.StartTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }

    private static readonly Dictionary<StorageThemeDecorationLeaderBoard, ThemeDecorationLeaderBoardPlayerSortController> SortControllerPool =
        new Dictionary<StorageThemeDecorationLeaderBoard, ThemeDecorationLeaderBoardPlayerSortController>();
    public static ThemeDecorationLeaderBoardPlayerSortController SortController(this StorageThemeDecorationLeaderBoard storageWeek)
    {
        if (!SortControllerPool.ContainsKey(storageWeek))
        {
            var newSortController = new ThemeDecorationLeaderBoardPlayerSortController(storageWeek);
            SortControllerPool.Add(storageWeek,newSortController);
        }
        return SortControllerPool[storageWeek];
    }

    public static List<ResData> GetRewardsByRank(this List<ThemeDecorationLeaderBoardRewardConfig> rewardConfig, int rank)
    {
        ThemeDecorationLeaderBoardRewardConfig targetConfig = null;
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
    

    public static void CollectStar(this StorageThemeDecorationLeaderBoard storage,int addCount)
    {
        if (storage.IsTimeOut())
            return;
        // if (collectCount <= 0)
        //     return;
        storage.StarCount += addCount;
        storage.StarUpdateTime = APIManager.Instance.GetServerTime();
        EventDispatcher.Instance.SendEventImmediately(new EventThemeDecorationLeaderBoardScoreChange(storage));
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
                ThemeDecorationLeaderBoardModel.Instance.EnterLeaderBoard(storage);
        }
    }

    private static ulong _lastUpdateWeekValueTime = 0;
    private const ulong updateInterval = 1000*10;
    public static async Task<bool> TryUpdateLeaderBoardFromServer(this StorageThemeDecorationLeaderBoard weekStorage)
    {
        if (ThemeDecorationLeaderBoardModel.GetFirstWeekCanGetReward() != null)
            return false;
        var serverTime = APIManager.Instance.GetServerTime();
        if (serverTime - _lastUpdateWeekValueTime < updateInterval)
            return false;
        _lastUpdateWeekValueTime = serverTime;
        return await weekStorage.ForceUpdateLeaderBoardFromServer();
    }

    private static int _forceUpdateWeekValueAck = 0;

    public static Dictionary<string, string> GetUniqueArgs(this StorageThemeDecorationLeaderBoard weekStorage)
    {
        var uniqueArgs = new Dictionary<string, string>()
        {
            {"ActivityId", weekStorage.ActivityId.ToString()},
            {"Version","1"},
        };
        return uniqueArgs;
    }
    public static async Task<bool> ForceUpdateLeaderBoardFromServer(this StorageThemeDecorationLeaderBoard weekStorage)
    {
        // WaitingManager.Instance.OpenWindow();
        if (!weekStorage.IsInitFromServer() && weekStorage.StarCount < weekStorage.LeastStarCount)
            return false;
        _forceUpdateWeekValueAck++;
        int tempAck = _forceUpdateWeekValueAck;
        var uniqueArgs = weekStorage.GetUniqueArgs();
        var globalData = JsonConvert.SerializeObject(uniqueArgs);
        var myPlayerStorage = new ThemeDecorationLeaderBoardPlayerServerStruct()
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
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.CreateOrGet, ThemeDecorationLeaderBoardModel.LeadBoardAPITypeName,
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
                    LeaderBoardModel.Request(LeaderBoardModel.RequestType.Logout,ThemeDecorationLeaderBoardModel.LeadBoardAPITypeName,null,null,
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

    public static async Task<bool> QuitLeaderBoardFromServer(this StorageThemeDecorationLeaderBoard weekStorage)
    {
        if (!weekStorage.IsInitFromServer())
            return false;
        WaitingManager.Instance.OpenWindow();
        var uniqueArgs = weekStorage.GetUniqueArgs();
        var globalData = JsonConvert.SerializeObject(uniqueArgs);
        var taskCallback = new TaskCompletionSource<bool>();
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.Logout,ThemeDecorationLeaderBoardModel.LeadBoardAPITypeName,uniqueArgs,globalData,
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

    public static async Task<bool> UploadLeaderBoardToServer(this StorageThemeDecorationLeaderBoard weekStorage)
    {
        if (!weekStorage.IsInitFromServer())
            return false;
        if (ThemeDecorationLeaderBoardModel.GetFirstWeekCanGetReward() != null)
            return false;
        var uniqueArgs = weekStorage.GetUniqueArgs();
        var globalData = JsonConvert.SerializeObject(uniqueArgs);
        var myPlayerStorage = new ThemeDecorationLeaderBoardPlayerServerStruct()
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
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.Update,ThemeDecorationLeaderBoardModel.LeadBoardAPITypeName,uniqueArgs,globalData,
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

    public static void SetValue(this ThemeDecorationLeaderBoardPlayerServerStruct srcPlayer,ThemeDecorationLeaderBoardPlayerServerStruct dstPlayer)
    {
        srcPlayer.PlayerId = dstPlayer.PlayerId;
        srcPlayer.PlayerName = dstPlayer.PlayerName;
        srcPlayer.StarCount = dstPlayer.StarCount;
        srcPlayer.AvatarIconId = dstPlayer.AvatarIconId;
        srcPlayer.AvatarIconFrameId = dstPlayer.AvatarIconFrameId;
        srcPlayer.StarUpdateTime = srcPlayer.StarUpdateTime;
    }

    public static bool IsInitFromServer(this StorageThemeDecorationLeaderBoard weekStorage)
    {
        return !weekStorage.LeaderBoardId.IsEmptyString();
    }

    public static Dictionary<string, bool> StorageWeekInitStateDictionary = new Dictionary<string, bool>();
    public static bool IsStorageWeekInitFromServer(this StorageThemeDecorationLeaderBoard weekStorage)
    {
        return StorageWeekInitStateDictionary.TryGetValue(weekStorage.ActivityId, out var value) ? value:false;
    }
    public static bool IsResExist(this StorageThemeDecorationLeaderBoard weekStorage)
    {
        return ActivityManager.Instance.CheckResExist(weekStorage.ActivityResList) || ActivityManager.Instance.IsActivityResourcesDownloaded(ThemeDecorationModel.Instance.ActivityId);
    }
    public static string GetSkinName(this StorageThemeDecorationLeaderBoard weekStorage)
    {
        if (weekStorage.SkinName == "")
            weekStorage.SkinName = "Default";
        return weekStorage.SkinName;
    }
    public static string GetAssetPathWithSkinName(this StorageThemeDecorationLeaderBoard weekStorage,string assetBasePath)
    {
        // Debug.LogError("周期storage.SkinName="+weekStorage.GetSkinName());
        return assetBasePath.Replace("/ThemeDecoration/", "/ThemeDecoration"+ThemeDecorationUtils.ConnectKeyWord + weekStorage.GetSkinName() + "/");
    }
    
    public static string GetAuxItemAssetPath(this StorageThemeDecorationLeaderBoard storage)
    {
        return storage.GetAssetPathWithSkinName("Prefabs/Activity/ThemeDecoration/Aux_ThemeDecorationLeaderBoard");
    }
    public static string GetTaskItemAssetPath(this StorageThemeDecorationLeaderBoard storage)
    {
        return storage.GetAssetPathWithSkinName("Prefabs/Activity/ThemeDecoration/TaskList_ThemeDecorationLeaderBoard");
    }
    public static bool ShowEntrance(this StorageThemeDecorationLeaderBoard storage)
    {
        if (!storage.IsResExist())
            return false;
        // if (!storage.IsStorageWeekInitFromServer())
        //     return false;
        if (!storage.IsActive())
            return false;
        return true;
    }
    public static void AddSkinUIWindowInfo(this StorageThemeDecorationLeaderBoard weekStorage)
    {
        UIManager.Instance._WindowMetaPublic(weekStorage.GetAssetPathWithSkinName(UINameConst.UIThemeDecorationLeaderBoardMain), UIWindowLayer.Normal, false);
    }
}