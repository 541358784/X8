using System.Collections.Generic;
using DragonPlus.Config.FishCulture;

public partial class FishCultureModel
{
    public List<FishCultureRewardConfig> FishCultureRewardConfigList
    {
        get
        {
            var configs = new List<FishCultureRewardConfig>();
            var allConfigs = FishCultureConfigManager.Instance.GetConfig<FishCultureRewardConfig>();
            var storage = CurStorageFishCultureWeek;
            var payLevelGroup = storage != null ? storage.PayLevelGroup : 0;
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == payLevelGroup)
                    configs.Add(config);
            }
            if (configs.Count == 0)
            {
                foreach (var config in allConfigs)
                {
                    if (config.PayLevelGroup == 0)
                        configs.Add(config);
                }
            }
            return configs;
        }
    }
    
    public List<FishCultureLeaderBoardRewardConfig> FishCultureLeaderBoardRewardConfigList
    {
        get
        {
            var configs = new List<FishCultureLeaderBoardRewardConfig>();
            var allConfigs = FishCultureConfigManager.Instance.GetConfig<FishCultureLeaderBoardRewardConfig>();
            var storage = CurStorageFishCultureWeek;
            var payLevelGroup = storage != null ? storage.PayLevelGroup : 0;
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == payLevelGroup)
                    configs.Add(config);
            }
            if (configs.Count == 0)
            {
                foreach (var config in allConfigs)
                {
                    if (config.PayLevelGroup == 0)
                        configs.Add(config);
                }
            }
            return configs;
        }
    }
    
    public List<FishCultureTaskRewardConfig> FishCultureTaskRewardConfigList
    {
        get
        {
            var configs = new List<FishCultureTaskRewardConfig>();
            var allConfigs = FishCultureConfigManager.Instance.GetConfig<FishCultureTaskRewardConfig>();
            var storage = CurStorageFishCultureWeek;
            var payLevelGroup = storage != null ? storage.PayLevelGroup : 0;
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == payLevelGroup)
                    configs.Add(config);
            }
            if (configs.Count == 0)
            {
                foreach (var config in allConfigs)
                {
                    if (config.PayLevelGroup == 0)
                        configs.Add(config);
                }
            }
            return configs;
        }
    }
}