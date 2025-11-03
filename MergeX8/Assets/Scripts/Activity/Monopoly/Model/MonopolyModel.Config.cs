using System.Collections.Generic;
using DragonPlus.Config.Monopoly;

public partial class MonopolyModel
{
    public List<MonopolyGlobalConfig> MonopolyGlobalConfigList
    {
        get
        {
            var configs = new List<MonopolyGlobalConfig>();
            var allConfigs = MonopolyConfigManager.Instance.GetConfig<MonopolyGlobalConfig>();
            var storage = CurStorageMonopolyWeek;
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
    
    public List<MonopolyBlockConfig> MonopolyBlockConfigList
    {
        get
        {
            var configs = new List<MonopolyBlockConfig>();
            var allConfigs = MonopolyConfigManager.Instance.GetConfig<MonopolyBlockConfig>();
            var storage = CurStorageMonopolyWeek;
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
    
    public List<MonopolyRewardBoxConfig> MonopolyRewardBoxConfigList
    {
        get
        {
            var configs = new List<MonopolyRewardBoxConfig>();
            var allConfigs = MonopolyConfigManager.Instance.GetConfig<MonopolyRewardBoxConfig>();
            var storage = CurStorageMonopolyWeek;
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
    
    public List<MonopolyCardConfig> MonopolyCardConfigList
    {
        get
        {
            var configs = new List<MonopolyCardConfig>();
            var allConfigs = MonopolyConfigManager.Instance.GetConfig<MonopolyCardConfig>();
            var storage = CurStorageMonopolyWeek;
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
    
    public List<MonopolyDiceConfig> MonopolyDiceConfigList
    {
        get
        {
            var configs = new List<MonopolyDiceConfig>();
            var allConfigs = MonopolyConfigManager.Instance.GetConfig<MonopolyDiceConfig>();
            var storage = CurStorageMonopolyWeek;
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
    
    public List<MonopolyMiniGameConfig> MonopolyMiniGameConfigList
    {
        get
        {
            var configs = new List<MonopolyMiniGameConfig>();
            var allConfigs = MonopolyConfigManager.Instance.GetConfig<MonopolyMiniGameConfig>();
            var storage = CurStorageMonopolyWeek;
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
    
    public List<MonopolyStoreLevelConfig> MonopolyStoreLevelConfigList
    {
        get
        {
            var configs = new List<MonopolyStoreLevelConfig>();
            var allConfigs = MonopolyConfigManager.Instance.GetConfig<MonopolyStoreLevelConfig>();
            var storage = CurStorageMonopolyWeek;
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
    
    public List<MonopolyStoreItemConfig> MonopolyStoreItemConfigList
    {
        get
        {
            var configs = new List<MonopolyStoreItemConfig>();
            var allConfigs = MonopolyConfigManager.Instance.GetConfig<MonopolyStoreItemConfig>();
            var storage = CurStorageMonopolyWeek;
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
    
    public List<MonopolyTaskRewardConfig> MonopolyTaskRewardConfigList
    {
        get
        {
            var configs = new List<MonopolyTaskRewardConfig>();
            var allConfigs = MonopolyConfigManager.Instance.GetConfig<MonopolyTaskRewardConfig>();
            var storage = CurStorageMonopolyWeek;
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
    
    public List<MonopolyLeaderBoardRewardConfig> MonopolyLeaderBoardRewardConfigList
    {
        get
        {
            var configs = new List<MonopolyLeaderBoardRewardConfig>();
            var allConfigs = MonopolyConfigManager.Instance.GetConfig<MonopolyLeaderBoardRewardConfig>();
            var storage = CurStorageMonopolyWeek;
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
    
    public List<MonopolyBuyDiceConfig> MonopolyBuyDiceConfigList
    {
        get
        {
            var configs = new List<MonopolyBuyDiceConfig>();
            var allConfigs = MonopolyConfigManager.Instance.GetConfig<MonopolyBuyDiceConfig>();
            var storage = CurStorageMonopolyWeek;
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