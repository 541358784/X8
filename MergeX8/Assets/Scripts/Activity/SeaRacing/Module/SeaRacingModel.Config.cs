using System.Collections.Generic;
using DragonPlus.Config.SeaRacing;

public partial class SeaRacingModel
{
    public List<SeaRacingRoundConfig> SeaRacingRoundConfigList
    {
        get
        {
            var configs = new List<SeaRacingRoundConfig>();
            var allConfigs = SeaRacingConfigManager.Instance.GetConfig<SeaRacingRoundConfig>();
            var storage = CurStorageSeaRacingWeek;
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
    
    public List<SeaRacingRewardConfig> SeaRacingRewardConfigList
    {
        get
        {
            var configs = new List<SeaRacingRewardConfig>();
            var allConfigs = SeaRacingConfigManager.Instance.GetConfig<SeaRacingRewardConfig>();
            var storage = CurStorageSeaRacingWeek;
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
    
    public List<SeaRacingRobotConfig> SeaRacingRobotConfigList
    {
        get
        {
            var configs = new List<SeaRacingRobotConfig>();
            var allConfigs = SeaRacingConfigManager.Instance.GetConfig<SeaRacingRobotConfig>();
            var storage = CurStorageSeaRacingWeek;
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
    public List<SeaRacingRobotRandomConfig> SeaRacingRobotRandomConfigList
    {
        get
        {
            var configs = new List<SeaRacingRobotRandomConfig>();
            var allConfigs = SeaRacingConfigManager.Instance.GetConfig<SeaRacingRobotRandomConfig>();
            var storage = CurStorageSeaRacingWeek;
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