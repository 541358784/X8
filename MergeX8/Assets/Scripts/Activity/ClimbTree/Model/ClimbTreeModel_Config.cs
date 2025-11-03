using System;
using System.Collections.Generic;
using DragonPlus.Config.ClimbTree;

public partial class ClimbTreeModel
{
    public Dictionary<int, ClimbTreeConfig> ClimbTreeDictionary;
    public List<ClimbTreeConfig> ClimbTreeList =>ClimbTreeConfigManager.Instance.ClimbTreeConfigList;
    public List<ClimbTreeProductConfig> ClimbTreeProductList =>ClimbTreeConfigManager.Instance.ClimbTreeProductConfigList;
    public ClimbTreeGlobalConfig GlobalConfig => ClimbTreeConfigManager.Instance.GetConfig<ClimbTreeGlobalConfig>()[0];
    public List<ClimbTreeLeaderBoardRewardConfig> LeaderBoardRewardConfig=> ClimbTreeConfigManager.Instance.GetConfig<ClimbTreeLeaderBoardRewardConfig>();
    public int MaxLevel;
    public int MinLevel = 0;
    public int MaxScore;

    public void InitConfig()
    {
        ClimbTreeDictionary = new Dictionary<int, ClimbTreeConfig>();
        for (var i = 0; i < ClimbTreeList.Count; i++)
        {
            var climbTree = ClimbTreeList[i];
            ClimbTreeDictionary.Add(climbTree.Level,climbTree);
        }
        MaxLevel = GetMaxLevel();
        MaxScore = GetMaxScore();
    }
    public int GetMaxLevel()
    {
        var maxLevel = -1;
        for (var i = 0; i < ClimbTreeList.Count; i++)
        {
            maxLevel = Math.Max(ClimbTreeList[i].Level, maxLevel);
        }

        return maxLevel;
    }

    public int GetMaxScore()
    {
        var maxScore = -1;
        for (var i = 0; i < ClimbTreeList.Count; i++)
        {
            maxScore = Math.Max(ClimbTreeList[i].Score, maxScore);
        }

        return maxScore;
    }

    public int GetLevelByScore(int score)
    {
        for (var i = MinLevel; i < MaxLevel; i++)
        {
            var nextLevelScore = ClimbTreeDictionary[i + 1].Score;
            if (score < nextLevelScore)
                return i;
        }

        return MaxLevel;
    }

    public int GetLevelBaseScore(int level)
    {
        if (level <= MinLevel)
            return 0;
        if (level > MaxLevel)
            return int.MaxValue;
        return ClimbTreeDictionary[level].Score;
    }

    public int GetLevelStageScore(int level)
    {
        var nextLevel = level + 1;
        var nextLevelScore = GetLevelBaseScore(nextLevel);
        var curLevelScore = GetLevelBaseScore(level);
        return nextLevelScore - curLevelScore;
    }

    public List<ResData> GetLevelRewards(int level)
    {
        if (level > MaxLevel)
            level = MaxLevel;
        if (level <= MinLevel || level > MaxLevel)
            throw new Exception("等级" + level + "没奖励");
        var result = new List<ResData>();
        for (var i = 0; i < ClimbTreeDictionary[level].RewardId.Count; i++)
        {
            result.Add(new ResData(id: ClimbTreeDictionary[level].RewardId[i],
                count: ClimbTreeDictionary[level].RewardNum[i]));
        }

        return result;
    }

    public int GetLevelRewardsShowId(int level)
    {
        if (level > MaxLevel)
            level = MaxLevel;
        if (level <= MinLevel || level > MaxLevel)
            throw new Exception("等级" + level + "没奖励");
        return ClimbTreeDictionary[level].RewardShowIndex;
    }
}