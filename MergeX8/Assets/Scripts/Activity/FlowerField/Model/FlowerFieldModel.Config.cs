using System.Collections.Generic;
using DragonPlus.Config.FlowerField;

public partial class FlowerFieldModel
{
    public List<FlowerFieldRewardConfig> FlowerFieldRewardConfigList
    {
        get
        {
            var configs = new List<FlowerFieldRewardConfig>();
            var allConfigs = FlowerFieldConfigManager.Instance.GetConfig<FlowerFieldRewardConfig>();
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
    public List<FlowerFieldTaskRewardConfig> FlowerFieldTaskRewardConfigList
    {
        get
        {
            var configs = new List<FlowerFieldTaskRewardConfig>();
            var allConfigs = FlowerFieldConfigManager.Instance.GetConfig<FlowerFieldTaskRewardConfig>();
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
    public List<FlowerFieldGlobalConfig> FlowerFieldGlobalConfigList
    {
        get
        {
            var configs = new List<FlowerFieldGlobalConfig>();
            var allConfigs = FlowerFieldConfigManager.Instance.GetConfig<FlowerFieldGlobalConfig>();
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

    public List<FlowerFieldLeaderBoardRewardConfig> FlowerFieldLeaderBoardRewardConfigList
    {
        get
        {
            var configs = new List<FlowerFieldLeaderBoardRewardConfig>();
            var allConfigs = FlowerFieldConfigManager.Instance.GetConfig<FlowerFieldLeaderBoardRewardConfig>();
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