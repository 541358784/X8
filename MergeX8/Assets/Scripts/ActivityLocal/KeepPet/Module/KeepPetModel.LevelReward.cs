using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;

public partial class KeepPetModel
{
    public bool CollectLevelReward(int level)
    {
        var levelConfig = LevelConfig.Find(a => a.Id == level);
        if (Storage.Exp < levelConfig.Exp)
        {
            return false;
        }
        if (Storage.LevelRewardCollectState.TryGetValue(level, out var collectState) && collectState)
        {
            return false;
        }
        Storage.LevelRewardCollectState.TryAdd(level, true);
        EventDispatcher.Instance.SendEventImmediately(new EventKeepPetCollectLevelReward(levelConfig));
        if (levelConfig.RewardId != null && levelConfig.RewardId.Count > 0)
        {
            var rewards = CommonUtils.FormatReward(levelConfig.RewardId, levelConfig.RewardNum);
            var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason
                    .KeepPetLevelRewardGet);
            UserData.Instance.AddRes(rewards,reason);
        }

        if (levelConfig.RewardBuildingItem != 0)
        {
            GetBuilding(levelConfig.RewardBuildingItem);
        }
        return true;
    }

    public int GetLevel()
    {
        if (LevelConfig == null)
            return 0;
        
        for (var i = LevelConfig.Count-1; i >=0 ; i--)
        {
            if (Storage.Exp >= LevelConfig[i].Exp)
            {
                return LevelConfig[i].Id;
            }
        }
        return 0;
    }

    public void CollectAllLevelReward()
    {
        var rewards = new List<ResData>();
        foreach (var level in LevelConfig)
        {
            if (CollectLevelReward(level.Id))
            {
                rewards.AddRange(CommonUtils.FormatReward(level.RewardId,level.RewardNum));
            }
        }

        if (rewards.Count > 0)
        {
            CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.GetCurrencyUseController(),
                false);   
        }
    }
}