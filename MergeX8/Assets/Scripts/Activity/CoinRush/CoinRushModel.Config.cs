using System.Collections.Generic;
using DragonPlus.Config.CoinRush;

public partial class CoinRushModel
{
    public List<CoinRushTaskConfig> CoinRushTaskConfigList
    {
        get
        {
            var configs = new List<CoinRushTaskConfig>();
            var allConfigs = CoinRushConfigManager.Instance.GetConfig<CoinRushTaskConfig>();
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == StorageCoinRush.PayLevelGroup)
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

    public List<PreheatConfig> PreheatConfigList
    {
        get
        {
            var configs = new List<PreheatConfig>();
            var allConfigs = CoinRushConfigManager.Instance.GetConfig<PreheatConfig>();
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == StorageCoinRush.PayLevelGroup)
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

    public List<LastRewardConfig> LastRewardConfigList
    {
        get
        {
            var configs = new List<LastRewardConfig>();
            var allConfigs = CoinRushConfigManager.Instance.GetConfig<LastRewardConfig>();
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == StorageCoinRush.PayLevelGroup)
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