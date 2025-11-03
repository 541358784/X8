using System.Collections.Generic;
using DragonPlus.Config.Parrot;

public partial class ParrotModel
{
    public List<ParrotRewardConfig> ParrotRewardConfigList
    {
        get
        {
            var configs = new List<ParrotRewardConfig>();
            var allConfigs = ParrotConfigManager.Instance.GetConfig<ParrotRewardConfig>();
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == Storage.PayLevelGroup)
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
    public List<ParrotTaskRewardConfig> ParrotTaskRewardConfigList
    {
        get
        {
            var configs = new List<ParrotTaskRewardConfig>();
            var allConfigs = ParrotConfigManager.Instance.GetConfig<ParrotTaskRewardConfig>();
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == Storage.PayLevelGroup)
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
    public List<ParrotGlobalConfig> ParrotGlobalConfigList
    {
        get
        {
            var configs = new List<ParrotGlobalConfig>();
            var allConfigs = ParrotConfigManager.Instance.GetConfig<ParrotGlobalConfig>();
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == Storage.PayLevelGroup)
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

    public List<ParrotLeaderBoardRewardConfig> ParrotLeaderBoardRewardConfigList
    {
        get
        {
            var configs = new List<ParrotLeaderBoardRewardConfig>();
            var allConfigs = ParrotConfigManager.Instance.GetConfig<ParrotLeaderBoardRewardConfig>();
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == Storage.PayLevelGroup)
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