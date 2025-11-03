using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonPlus;
using DragonPlus.Config.RecoverCoin;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public static class RecoverCoinUtils
{
    private static readonly Dictionary<StorageRecoverCoinWeek, List<RecoverCoinRewardConfig>> RewardConfigPool =
        new Dictionary<StorageRecoverCoinWeek, List<RecoverCoinRewardConfig>>();
    public static List<RecoverCoinRewardConfig> RewardConfig(this StorageRecoverCoinWeek storageWeek)
    {
        // if (storageWeek.JsonRecoverCoinRewardConfig.IsEmptyString())
        //     return null;
        // if (!RewardConfigPool.ContainsKey(storageWeek))
        // {
        //     var newRewardConfig = JsonConvert.DeserializeObject<List<RecoverCoinRewardConfig>>(storageWeek.JsonRecoverCoinRewardConfig);
        //     RewardConfigPool.Add(storageWeek,newRewardConfig);
        // }
        // return RewardConfigPool[storageWeek];
        return RecoverCoinConfigManager.Instance.GetConfig<RecoverCoinRewardConfig>();
    }
    
    private static readonly Dictionary<StorageRecoverCoinWeek, List<RecoverCoinExchangeStarConfig>> ExchangeStarConfigPool =
        new Dictionary<StorageRecoverCoinWeek, List<RecoverCoinExchangeStarConfig>>();
    public static List<RecoverCoinExchangeStarConfig> ExchangeStarConfig(this StorageRecoverCoinWeek storageWeek)
    {
        // if (storageWeek.JsonRecoverCoinExchangeStarConfig.IsEmptyString())
        //     return null;
        // if (!ExchangeStarConfigPool.ContainsKey(storageWeek))
        // {
        //     var newExchangeStarConfig = JsonConvert.DeserializeObject<List<RecoverCoinExchangeStarConfig>>(storageWeek.JsonRecoverCoinExchangeStarConfig);
        //     ExchangeStarConfigPool.Add(storageWeek,newExchangeStarConfig);
        // }
        // return ExchangeStarConfigPool[storageWeek];
        return RecoverCoinConfigManager.Instance.GetConfig<RecoverCoinExchangeStarConfig>();
    }
    
    private static readonly Dictionary<StorageRecoverCoinWeek, List<RecoverCoinRobotMinStarUpdateIntervalConfig>> RobotMinStarUpdateIntervalConfigPool =
        new Dictionary<StorageRecoverCoinWeek, List<RecoverCoinRobotMinStarUpdateIntervalConfig>>();
    public static List<RecoverCoinRobotMinStarUpdateIntervalConfig> RobotMinStarUpdateIntervalConfig(this StorageRecoverCoinWeek storageWeek)
    {
        // if (storageWeek.JsonRecoverCoinRobotMinStarUpdateIntervalConfig.IsEmptyString())
        //     return null;
        // if (!RobotMinStarUpdateIntervalConfigPool.ContainsKey(storageWeek))
        // {
        //     var newRobotMinStarUpdateIntervalConfig = JsonConvert.DeserializeObject<List<RecoverCoinRobotMinStarUpdateIntervalConfig>>(storageWeek.JsonRecoverCoinRobotMinStarUpdateIntervalConfig);
        //     RobotMinStarUpdateIntervalConfigPool.Add(storageWeek,newRobotMinStarUpdateIntervalConfig);
        // }
        // return RobotMinStarUpdateIntervalConfigPool[storageWeek];
        return RecoverCoinConfigManager.Instance.GetConfig<RecoverCoinRobotMinStarUpdateIntervalConfig>();
    }

    class RobotConfigGroup
    {
        public int Id;
        public int Weight;
        public int StarMin;
        public int StarMax;
    }
    public static int GetNewRobotMaxStarCount(this RecoverCoinRobotGrowSpeedConfig growSpeedConfig,int robotId)
    {
        var groupCount = growSpeedConfig.Weight.Count;
        var groupList = new List<RobotConfigGroup>();
        // var maxWeight = 0;
        for (var i = 0; i < groupCount; i++)
        {
            // maxWeight += growSpeedConfig.Weight[i];
            groupList.Add(new RobotConfigGroup()
            {
                Id = i,
                Weight = growSpeedConfig.Weight[i],
                StarMin = growSpeedConfig.GrowSpeedMin[i],
                StarMax = growSpeedConfig.GrowSpeedMax[i]
            });
        }
        // var randomWeight = Random.Range(0, maxWeight);
        var randomWeight = robotId;
        var weightCount = 0;
        RobotConfigGroup cfgGroup = null;
        for (var i = 0; i < groupCount; i++)
        {
            weightCount += groupList[i].Weight;
            if (weightCount > randomWeight)
            {
                cfgGroup = groupList[i];
                break;
            }
        }
        return Random.Range(cfgGroup.StarMin, cfgGroup.StarMax);
    }
    
    public static bool CanStorageRecoverCoinWeekGetReward(this StorageRecoverCoinWeek storageWeek)//是否可以领奖
    {
        return !storageWeek.IsFinish &&//未领奖
               storageWeek.IsTimeOut();//时间已经结束
    }

    public static bool IsTimeOut(this StorageRecoverCoinWeek storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageRecoverCoinWeek storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageRecoverCoinWeek storageWeek,long leftTime)
    {
        storageWeek.EndTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        RecoverCoinModel.Instance._weekTimeConfig[storageWeek.WeekId].EndTimeSec = storageWeek.EndTime.ToString();
    }
    public static string GetLeftTimeText(this StorageRecoverCoinWeek storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }

    private static readonly Dictionary<StorageRecoverCoinWeek, RecoverCoinPlayerSortController> SortControllerPool =
        new Dictionary<StorageRecoverCoinWeek, RecoverCoinPlayerSortController>();
    public static RecoverCoinPlayerSortController SortController(this StorageRecoverCoinWeek storageWeek)
    {
        if (!SortControllerPool.ContainsKey(storageWeek))
        {
            var newSortController = new RecoverCoinPlayerSortController(storageWeek);
            SortControllerPool.Add(storageWeek,newSortController);
        }
        return SortControllerPool[storageWeek];
    }

    public static List<ResData> GetRewardsByRank(this List<RecoverCoinRewardConfig> rewardConfig, int rank)
    {
        RecoverCoinRewardConfig targetConfig = null;
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
            return null;
        var rewardList = new List<ResData>();
        for (var i = 0; i < targetConfig.RewardId.Count; i++)
        {
            rewardList.Add(new ResData(targetConfig.RewardId[i],targetConfig.RewardNum[i]));
        }
        return rewardList;
    }
    public static void CompletedStorageActivity(this StorageRecoverCoinWeek storage)
    {
        storage.IsFinish = true;
        GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.RecoverCoinStar, storage.StarCount, (ulong) storage.StarCount,
            new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.CoinoverUse} );
        DragonPlus.GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventCoinoverLeaderboard,
            data1: UserData.Instance.GetRes(UserData.ResourceId.Coin).ToString(),
            data2: storage.StarCount.ToString());
        storage.TryRelease();
        RecoverCoinModel.Instance.InvokeActionAfterWeekEnd();//有被忽略的挂点需要在周结束时进行更新
        RecoverCoinModel.Instance.UpdateActivityUsingResList(RecoverCoinModel.Instance.ActivityId);
    }

    public static void TryRelease(this StorageRecoverCoinWeek storage)
    {
        if (storage.IsFinish)
        {
            Debug.LogError("删除第"+storage.WeekId+"周数据");
            RecoverCoinModel.StorageRecoverCoin.StorageByWeek.Remove(storage.WeekId);
        }
    }
    
    public static void CollectStar(this StorageRecoverCoinWeek storage,int collectCount)
    {
        if (storage.IsTimeOut())
            return;
        if (collectCount <= 0)
            return;
        storage.StarCount += collectCount;
        var time = APIManager.Instance.GetServerTime();
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventCoinoverGetstar2,
            collectCount.ToString(),storage.StarCount.ToString(), time.ToString());
        storage.StarUpdateTime = time;
        storage.UploadLeaderBoardToServer().WrapErrors();
    }
    
    public static int GetRobotUpdateInterval(this StorageRecoverCoinWeek weekStorage)
    {
        var robotMinStarUpdateIntervalConfig = weekStorage.RobotMinStarUpdateIntervalConfig();
        if (robotMinStarUpdateIntervalConfig == null)
            return 7;
        var totalWeight = 0;
        foreach (var cfg in robotMinStarUpdateIntervalConfig)
        {
            totalWeight += cfg.Weight;
        }
        if (totalWeight <= 0)
            return 7;
        var randomWeight = Random.Range(0, totalWeight);
        var curWeight = 0;
        foreach (var cfg in robotMinStarUpdateIntervalConfig)
        {
            curWeight += cfg.Weight;
            if (curWeight > randomWeight)
            {
                return cfg.MinStarUpdateInterval;
            }
        }
        return 7;
    }

    // public static string GetActivityId(this StorageRecoverCoinWeek weekStorage)
    // {
    //     if (weekStorage.ActivityId.IsEmptyString())
    //     {
    //         weekStorage.ActivityId = RecoverCoinModel.Instance.ActivityId;
    //     }
    //     return weekStorage.ActivityId;
    // }

    private static ulong _lastUpdateWeekValueTime = 0;
    private const ulong updateInterval = 1000*10;
    public static async Task<bool> TryUpdateLeaderBoardFromServer(this StorageRecoverCoinWeek weekStorage)
    {
        if (!weekStorage.IsRealPeopleLeaderBoard())
            return false;
        if (RecoverCoinModel.GetFirstWeekCanGetReward() != null)
            return false;
        var serverTime = APIManager.Instance.GetServerTime();
        if (serverTime - _lastUpdateWeekValueTime < updateInterval)
            return false;
        _lastUpdateWeekValueTime = serverTime;
        return await weekStorage.ForceUpdateLeaderBoardFromServer();
    }

    private static int _forceUpdateWeekValueAck = 0;

    public static Dictionary<string, string> GetUniqueArgs(this StorageRecoverCoinWeek weekStorage)
    {
        var uniqueArgs = new Dictionary<string, string>()
        {
            {"Week", weekStorage.WeekId.ToString()},
            {"Version","1"},
        };
        if (weekStorage.PlayerCoinCountGroupIndex > 0)//已经开启的未分组榜单保持未分组状态
            uniqueArgs.Add("CoinCountGroup",weekStorage.PlayerCoinCountGroupIndex.ToString());
        return uniqueArgs;
    }
    public static async Task<bool> ForceUpdateLeaderBoardFromServer(this StorageRecoverCoinWeek weekStorage)
    {
        if (!weekStorage.IsRealPeopleLeaderBoard())
            return false;
        // WaitingManager.Instance.OpenWindow();
        _forceUpdateWeekValueAck++;
        int tempAck = _forceUpdateWeekValueAck;
        var uniqueArgs = weekStorage.GetUniqueArgs();
        var globalData = JsonConvert.SerializeObject(uniqueArgs);
        var myPlayerStorage = new RecoverCoinPlayerServerStruct()
        {
            AvatarIconId = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.AvatarIconId,
            AvatarIconFrameId = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.GetUserAvatarFrame().id,
            BuyTimes = weekStorage.BuyTimes,
            CompletedTaskCount = weekStorage.CompletedTaskCount,
            PlayerId = StorageManager.Instance.GetStorage<StorageCommon>().PlayerId,
            PlayerName = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName,
            StarCount = weekStorage.StarCount,
            StarUpdateTime = weekStorage.StarUpdateTime,
        };
        var extraInfo = JsonConvert.SerializeObject(myPlayerStorage);
        var taskCallback = new TaskCompletionSource<bool>();
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.CreateOrGet, RecoverCoinModel.LeadBoardAPITypeName,
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
                    LeaderBoardModel.Request(LeaderBoardModel.RequestType.Logout,RecoverCoinModel.LeadBoardAPITypeName,null,null,
                        (success, requestType, leaderBoardListItem) =>
                        {
                            
                        });
                    taskCallback.SetResult(false);
                    return;
                }
                weekStorage.LeaderBoardId = leaderBoardListItem.Me.LeaderboardId;
                foreach (var leadBoardEntry in leaderBoardListItem.LeaderboardEntries)
                {
                    var playerData = JsonConvert.DeserializeObject<RecoverCoinPlayerServerStruct>(leadBoardEntry.Extra);
                    if (playerData.PlayerId == StorageManager.Instance.GetStorage<StorageCommon>().PlayerId)
                        continue;
                    weekStorage.SortController().AddNewPlayer(playerData);
                    // if (!weekStorage.PlayerList.ContainsKey(playerData.PlayerId))
                    // {
                    //     weekStorage.SortController().AddNewPlayer(playerData);
                    //     weekStorage.PlayerList.Add(playerData.PlayerId,playerData);
                    // }
                    // else
                    // {
                    //     weekStorage.PlayerList[playerData.PlayerId].SetValue(playerData);
                    // }
                }
                taskCallback.SetResult(true);
            }, weekStorage.StarCount, extraInfo, (uint) weekStorage.MaxPlayerCount);
        return await taskCallback.Task;
    }

    public static async Task<bool> QuitLeaderBoardFromServer(this StorageRecoverCoinWeek weekStorage)
    {
        if (!weekStorage.IsRealPeopleLeaderBoard() || !weekStorage.IsInitFromServer())
            return false;
        WaitingManager.Instance.OpenWindow();
        var uniqueArgs = weekStorage.GetUniqueArgs();
        var globalData = JsonConvert.SerializeObject(uniqueArgs);
        var taskCallback = new TaskCompletionSource<bool>();
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.Logout,RecoverCoinModel.LeadBoardAPITypeName,uniqueArgs,globalData,
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

    public static async Task<bool> UploadLeaderBoardToServer(this StorageRecoverCoinWeek weekStorage)
    {
        if (!weekStorage.IsRealPeopleLeaderBoard() || !weekStorage.IsInitFromServer())
            return false;
        if (RecoverCoinModel.GetFirstWeekCanGetReward() != null)
            return false;
        var uniqueArgs = weekStorage.GetUniqueArgs();
        var globalData = JsonConvert.SerializeObject(uniqueArgs);
        var myPlayerStorage = new RecoverCoinPlayerServerStruct()
        {
            AvatarIconId = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.AvatarIconId,
            AvatarIconFrameId = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.GetUserAvatarFrame().id,
            BuyTimes = weekStorage.BuyTimes,
            CompletedTaskCount = weekStorage.CompletedTaskCount,
            PlayerId = StorageManager.Instance.GetStorage<StorageCommon>().PlayerId,
            PlayerName = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName,
            StarCount = weekStorage.StarCount,
            StarUpdateTime = weekStorage.StarUpdateTime,
        };
        var extraInfo = JsonConvert.SerializeObject(myPlayerStorage);
        var taskCallback = new TaskCompletionSource<bool>();
        LeaderBoardModel.Request(LeaderBoardModel.RequestType.Update,RecoverCoinModel.LeadBoardAPITypeName,uniqueArgs,globalData,
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

    public static void SetValue(this RecoverCoinPlayerServerStruct srcPlayer,RecoverCoinPlayerServerStruct dstPlayer)
    {
        srcPlayer.PlayerId = dstPlayer.PlayerId;
        srcPlayer.PlayerName = dstPlayer.PlayerName;
        srcPlayer.BuyTimes = dstPlayer.BuyTimes;
        srcPlayer.CompletedTaskCount = dstPlayer.CompletedTaskCount;
        srcPlayer.StarCount = dstPlayer.StarCount;
        srcPlayer.AvatarIconId = dstPlayer.AvatarIconId;
        srcPlayer.AvatarIconFrameId = dstPlayer.AvatarIconFrameId;
        srcPlayer.StarUpdateTime = srcPlayer.StarUpdateTime;
    }

    public static bool IsInitFromServer(this StorageRecoverCoinWeek weekStorage)
    {
        return !weekStorage.LeaderBoardId.IsEmptyString();
    }

    public static bool IsRealPeopleLeaderBoard(this StorageRecoverCoinWeek weekStorage)
    {
        return weekStorage.RobotList.Count == 0;
    }

    public static string GetSkinName(this StorageRecoverCoinWeek weekStorage)
    {
        if (weekStorage.SkinName == "")
            weekStorage.SkinName = "Default";
        return weekStorage.SkinName;
    }
    public static string GetAssetPathWithSkinName(this StorageRecoverCoinWeek weekStorage,string assetBasePath)
    {
        // Debug.LogError("周期storage.SkinName="+weekStorage.GetSkinName());
        return assetBasePath.Replace("/RecoverCoin/", "/RecoverCoin"+RecoverCoinModel.ConnectKeyWord + weekStorage.GetSkinName() + "/");
    }
    public static void AddSkinUIWindowInfo(this StorageRecoverCoinWeek weekStorage)
    {
        UIManager.Instance._WindowMetaPublic(weekStorage.GetAssetPathWithSkinName(UINameConst.UIRecoverCoinMain), UIWindowLayer.Normal, false);
        UIManager.Instance._WindowMetaPublic(weekStorage.GetAssetPathWithSkinName(UINameConst.UIRecoverCoinStart), UIWindowLayer.Notice,false);
        UIManager.Instance._WindowMetaPublic(weekStorage.GetAssetPathWithSkinName(UINameConst.UIPopupRecoverCoinFinish), UIWindowLayer.Notice,false);
        UIManager.Instance._WindowMetaPublic(weekStorage.GetAssetPathWithSkinName(UINameConst.UIPopupRecoverCoinNewDecoArea), UIWindowLayer.Notice,false);
        UIManager.Instance._WindowMetaPublic(weekStorage.GetAssetPathWithSkinName(UINameConst.UIRecoverCoinBuy), UIWindowLayer.Normal, false);
        UIManager.Instance._WindowMetaPublic(weekStorage.GetAssetPathWithSkinName(UINameConst.UIRecoverCoinEnd), UIWindowLayer.Normal, false);
    }
}