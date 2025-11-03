using System.Collections.Generic;
using DragonPlus.Config.PillowWheel;

public partial class PillowWheelModel
{
    public PillowWheelGlobalConfig GlobalConfig => GlobalConfigList[0];
    public List<PillowWheelGlobalConfig> GlobalConfigList
    {
        get
        {
            var configs = new List<PillowWheelGlobalConfig>();
            var allConfigs = PillowWheelConfigManager.Instance.GetConfig<PillowWheelGlobalConfig>();
            var payLevelGroup = Storage.PayLevelGroup;
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
    public List<PillowWheelLeaderBoardRewardConfig> LeaderBoardRewardConfigList
    {
        get
        {
            return PillowWheelConfigManager.Instance.GetConfig<PillowWheelLeaderBoardRewardConfig>();
        }
    }
    public List<PillowWheelResultConfig> ResultConfigList
    {
        get
        {
            var configs = new List<PillowWheelResultConfig>();
            var allConfigs = PillowWheelConfigManager.Instance.GetConfig<PillowWheelResultConfig>();
            var payLevelGroup = Storage.PayLevelGroup;
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
    public List<PillowWheelShopConfig> ShopConfigList
    {
        get
        {
            return PillowWheelConfigManager.Instance.GetConfig<PillowWheelShopConfig>();
        }
    }
    
    public List<PillowWheelSpecialRewardConfig> SpecialRewardConfigList
    {
        get
        {
            var configs = new List<PillowWheelSpecialRewardConfig>();
            var allConfigs = PillowWheelConfigManager.Instance.GetConfig<PillowWheelSpecialRewardConfig>();
            var payLevelGroup = Storage.PayLevelGroup;
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
    public List<PillowWheelTaskRewardConfig> TaskRewardConfigList
    {
        get
        {
            return PillowWheelConfigManager.Instance.GetConfig<PillowWheelTaskRewardConfig>();
        }
    }
    public List<PillowWheelTurntableConfig> TurntableConfigList
    {
        get
        {
            return PillowWheelConfigManager.Instance.GetConfig<PillowWheelTurntableConfig>();
        }
    }
}