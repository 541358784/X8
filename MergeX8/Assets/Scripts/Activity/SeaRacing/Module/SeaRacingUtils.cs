using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonPlus;
using DragonPlus.Config.SeaRacing;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using SomeWhere;
using UnityEngine;
using Random = UnityEngine.Random;

public static class SeaRacingUtils
{
    public static bool IsTimeOut(this StorageSeaRacingRound storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageSeaRacingRound storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageSeaRacingRound storageWeek,long leftTime)
    {
        storageWeek.EndTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static string GetLeftTimeText(this StorageSeaRacingRound storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }

    public static long GetPreheatLeftTime(this StorageSeaRacing storage)
    {
        var heatTime = (SeaRacingModel.Instance.IsSkipActivityPreheating()?storage.StartTime: storage.PreheatCompleteTime) - (long)APIManager.Instance.GetServerTime();
        heatTime = Math.Max(heatTime, 0);
        return heatTime;
    }
    public static void SetPreheatLeftTime(this StorageSeaRacing storageWeek,long leftTime)
    {
        storageWeek.PreheatCompleteTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }

    public static string GetPreheatLeftTimeText(this StorageSeaRacing storage)
    {
        return CommonUtils.FormatLongToTimeStr(storage.GetPreheatLeftTime());
    }
    public static bool IsTimeOut(this StorageSeaRacing storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageSeaRacing storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageSeaRacing storageWeek,long leftTime)
    {
        storageWeek.EndTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        foreach (var pair in storageWeek.SeaRacingRoundList)
        {
            pair.Value.EndTime = storageWeek.EndTime;
        }
    }
    public static string GetLeftTimeText(this StorageSeaRacing storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }

    private static readonly Dictionary<StorageSeaRacingRound, SeaRacingPlayerSortController> SortControllerPool =
        new Dictionary<StorageSeaRacingRound, SeaRacingPlayerSortController>();
    public static SeaRacingPlayerSortController SortController(this StorageSeaRacingRound storageWeek)
    {
        if (!SortControllerPool.ContainsKey(storageWeek))
        {
            var newSortController = new SeaRacingPlayerSortController(storageWeek);
            SortControllerPool.Add(storageWeek,newSortController);
        }
        return SortControllerPool[storageWeek];
    }

    public static List<ResData> GetRewardsByRank(this StorageSeaRacingRound storageWeek, int rank)
    {
        var rewardConfigIdList = SeaRacingModel.Instance.RoundConfig[storageWeek.RoundConfigId].RewardConfigId;
        if (rank > rewardConfigIdList.Count)
            return null;
        var rewardConfigId = rewardConfigIdList[rank - 1];
        var targetConfig = SeaRacingModel.Instance.RewardConfig[rewardConfigId];
        if (targetConfig == null)
            return null;
        var rewardList = new List<ResData>();
        for (var i = 0; i < targetConfig.RewardId.Count; i++)
        {
            rewardList.Add(new ResData(targetConfig.RewardId[i],targetConfig.RewardNum[i]));
        }
        return rewardList;
    }
    public static void CompletedStorageActivity(this StorageSeaRacing storage)
    {
        storage.IsFinish = true;
        if (storage.TryRelease())
            SeaRacingModel.Instance.CreateStorage();
    }

    public static bool TryRelease(this StorageSeaRacing storage)
    {
        if (storage.IsFinish && storage.IsTimeOut())
        {
            Debug.LogError("删除ActivityId = "+storage.ActivityId+"排行榜数据");
            SeaRacingModel.StorageSeaRacing.Remove(storage.ActivityId);
            return true;
        }
        return false;
    }
    
    public static void CollectStar(this StorageSeaRacingRound storage,int collectCount)
    {
        if (storage.IsTimeOut())
            return;
        // if (collectCount <= 0)
        //     return;
        var oldScore = storage.Score;
        storage.Score += collectCount;
        if (storage.Score > storage.MaxScore)
            storage.Score = storage.MaxScore;
        if (storage.Score != oldScore)
        {
            storage.ScoreUpdateTime = APIManager.Instance.GetServerTime();
            if (storage.Score >= storage.MaxScore)
            {
                storage.SortController().UpdateAll();
                storage.UnCollectRewards.Clear();
                var rewardList = storage.GetRewardsByRank(storage.SortController().MyRank);
                if (rewardList != null)
                {
                    foreach (var reward in rewardList)
                    {
                        if (storage.UnCollectRewards.ContainsKey(reward.id))
                        {
                            storage.UnCollectRewards[reward.id] += reward.count;
                        }
                        else
                        {
                            storage.UnCollectRewards.Add(reward.id,reward.count);
                        }
                    }
                }
            }
        }
    }

    public static StorageSeaRacingRound CurRound(this StorageSeaRacing storage)
    {
        storage.SeaRacingRoundList.TryGetValue(storage.CurrencyRoundIndex,out var curRound);
        return curRound;
    }
    public static bool IsResExist(this StorageSeaRacing weekStorage)
    {
        return ActivityManager.Instance.CheckResExist(weekStorage.ActivityResList) || ActivityManager.Instance.IsActivityResourcesDownloaded(SeaRacingModel.Instance.ActivityId);
    }

    public static bool CreateRound(this StorageSeaRacing weekStorage)
    {
        if (weekStorage == null)
            return false;
        var curRound = weekStorage.CurRound();
        Debug.LogError("CreateRound3");
        if (!weekStorage.IsTimeOut() && (curRound == null || curRound.IsFinish))
        {
            var nextIndex = weekStorage.CurrencyRoundIndex + 1;
            SeaRacingModel.Instance.RoundConfig.TryGetValue(nextIndex, out var nextRoundConfig);
            if (nextRoundConfig == null)//没有下一轮的配置了，活动结束
            {
                SeaRacingModel.Instance.CurStorageSeaRacingWeek.IsCompletedAll = true;
                SeaRacingModel.OpenFinishPopup(SeaRacingModel.Instance.CurStorageSeaRacingWeek);
                return false;
            }
            Debug.LogError("CreateRound4");
            
            var newWeek = new StorageSeaRacingRound()
            {
                RoundConfigId = nextIndex,
                EndTime = weekStorage.EndTime,
                StartTime = (long) APIManager.Instance.GetServerTime(),
                Score = 0,
                ScoreUpdateTime = 0,
                MaxScore = nextRoundConfig.MaxScore,
                IsFinish = false,
                IsStart = false,
            };
            var namePool = SeaRacingConfigManager.Instance.GetConfig<SeaRacingRobotNameConfig>();
            string GetRobotName()
            {
                var index = Random.Range(0, namePool.Count);
                var result = namePool[index];
                namePool.RemoveAt(index);
                return result.Name;
            }
            newWeek.RobotList.Clear();
            var curTime = (long)APIManager.Instance.GetServerTime();
            foreach (var robotRandomConfigId in nextRoundConfig.RobotRandomConfigId)
            {
                if (SeaRacingModel.Instance.RobotRandomConfig.TryGetValue(robotRandomConfigId,
                        out var robotRandomConfig))
                {
                    var robotStorage = new StorageSeaRacingRobot()
                    {
                        RandomConfigId = robotRandomConfig.Id,
                        // RobotConfigId = robotConfigId,
                        Score = 0,
                        PlayerName = GetRobotName(),
                        AvatarIconId = Random.Range(0, 6),
                        UpdateScoreTime = curTime,
                        // RobotType = robotConfig.RobotType,
                        // ScoreLimit = robotConfig.ScoreLimit,
                    };
                    newWeek.RobotList.Add(robotStorage);
                    robotStorage.RandomRobotConfig();
                }
            }
            weekStorage.SeaRacingRoundList.Add(nextIndex, newWeek);
            weekStorage.CurrencyRoundIndex = nextIndex;
            // newWeek.IsStart = true;
            // newWeek.StartTime = (long)APIManager.Instance.GetServerTime();
            return true;
        }

        return false;
    }

    public static void RandomRobotState(this StorageSeaRacingRobot robotStorage)//随机机器人下次加分
    {
        var robotConfig = SeaRacingModel.Instance.RobotConfig[robotStorage.RobotConfigId];   
        int randomCount;
        {
            var countList = new List<int>();
            var countWeights = new List<int>();
            foreach (var count in robotConfig.CoutRange)
            {
                countList.Add(count);
            }
            foreach (var weight in robotConfig.CoutWeight)
            {
                countWeights.Add(weight);
            }
            var randomWeightIndex = countWeights.RandomIndexByWeight();
            randomCount = countList[randomWeightIndex];
        }
        robotStorage.UpdateScoreMaxCount = randomCount;
        robotStorage.UpdateScoreInterval = Random.Range(robotConfig.TimeRange[0], robotConfig.TimeRange[1]) * (long)XUtility.Second;
        robotStorage.UpdateScoreValue = Random.Range(robotConfig.AddRange[0], robotConfig.AddRange[1]);
    }
    public static void RandomRobotConfig(this StorageSeaRacingRobot robotStorage)//随机机器人配置
    {
        var randomConfig = SeaRacingModel.Instance.RobotRandomConfig[robotStorage.RandomConfigId];
        SeaRacingRobotConfig robotConfig;
        {
            var robots = new List<int>();
            var weights = new List<int>();
            foreach (var robotId in randomConfig.Robot)
            {
                robots.Add(robotId);
            }
            foreach (var weight in randomConfig.Weight)
            {
                weights.Add(weight);
            }

            var randomWeightIndex = weights.RandomIndexByWeight();
            var robotConfigId = robots[randomWeightIndex];
            robotConfig = SeaRacingModel.Instance.RobotConfig[robotConfigId];   
        }

        robotStorage.RobotConfigId = robotConfig.Id;
        robotStorage.RobotType = robotConfig.RobotType;
        robotStorage.ScoreLimit = robotConfig.ScoreLimit;
        robotStorage.UpdateScoreCount = 0;
        // Debug.LogError("刷新机器人配置 随机组"+robotStorage.RandomConfigId+" 机器人配置"+robotStorage.RobotConfigId);
        robotStorage.RandomRobotState();

    }
    
    public static string GetRoundString(this StorageSeaRacingRound storage)
    {
        return storage.RoundConfigId + "/" + SeaRacingModel.Instance.RoundConfig.Count;
    }
}