using System.Collections.Generic;
using DragonPlus.Config.ButterflyWorkShop;

public partial class ButterflyWorkShopModel
{
    public ButterflyWorkShopConfig ButterflyWorkShopConfig
    {
        get
        {
            var configs = new List<ButterflyWorkShopConfig>();
            var allConfigs = ButterflyWorkShopConfigManager.Instance.GetConfig<ButterflyWorkShopConfig>();
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == StorageButterflyWorkShop.PayLevelGroup)
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
            return configs[0];
        }
    }
    
    public List<ButterflyWorkShopProductAttenuationConfig> ProductAttenuationConfig
    {
        get
        {
            var configs = new List<ButterflyWorkShopProductAttenuationConfig>();
            var allConfigs = ButterflyWorkShopConfigManager.Instance.GetConfig<ButterflyWorkShopProductAttenuationConfig>();
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == StorageButterflyWorkShop.PayLevelGroup)
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

    public List<ButterflyWorkShopPackageConfig> ButterflyWorkShopPackageConfigList
    {
        get
        {
            var configs = new List<ButterflyWorkShopPackageConfig>();
            var allConfigs = ButterflyWorkShopConfigManager.Instance.GetConfig<ButterflyWorkShopPackageConfig>();
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == StorageButterflyWorkShop.PayLevelGroup)
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

    public List<StageRewardConfig> StageRewardConfigList
    {
        get
        {
            var configs = new List<StageRewardConfig>();
            var allConfigs = ButterflyWorkShopConfigManager.Instance.GetConfig<StageRewardConfig>();
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == StorageButterflyWorkShop.PayLevelGroup)
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

    public List<ButterflyWorkShopRewardConfig> ButterflyWorkShopRewardConfigList
    {
        get
        {
            var configs = new List<ButterflyWorkShopRewardConfig>();
            var allConfigs = ButterflyWorkShopConfigManager.Instance.GetConfig<ButterflyWorkShopRewardConfig>();
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == StorageButterflyWorkShop.PayLevelGroup)
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

    public List<ButterflyRandomConfig> ButterflyRandomConfigList
    {
        get
        {
            var configs = new List<ButterflyRandomConfig>();
            var allConfigs = ButterflyWorkShopConfigManager.Instance.GetConfig<ButterflyRandomConfig>();
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == StorageButterflyWorkShop.PayLevelGroup)
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