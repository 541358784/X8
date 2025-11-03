using System.Collections.Generic;
using DragonPlus.Config.BiuBiu;

public partial class BiuBiuModel
{
    public List<BiuBiuGlobalConfig> BiuBiuGlobalConfigList
    {
        get
        {
            var configs = new List<BiuBiuGlobalConfig>();
            var allConfigs = BiuBiuConfigManager.Instance.GetConfig<BiuBiuGlobalConfig>();
            var storage = Storage;
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
    
    public List<BiuBiuFateConfig> BiuBiuFateConfigList
    {
        get
        {
            var configs = new List<BiuBiuFateConfig>();
            var allConfigs = BiuBiuConfigManager.Instance.GetConfig<BiuBiuFateConfig>();
            var storage = Storage;
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
    
    public List<BiuBiuPackageConfig> BiuBiuPackageConfigList
    {
        get
        {
            var configs = new List<BiuBiuPackageConfig>();
            var allConfigs = BiuBiuConfigManager.Instance.GetConfig<BiuBiuPackageConfig>();
            var storage = Storage;
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