using System.Collections.Generic;
using DragonPlus.Config.ThemeDecoration;

public partial class ThemeDecorationModel
{
    public List<ThemeDecorationGlobalConfig> ThemeDecorationGlobalConfigList
    {
        get
        {
            var configs = new List<ThemeDecorationGlobalConfig>();
            var allConfigs = ThemeDecorationConfigManager.Instance.GetConfig<ThemeDecorationGlobalConfig>();
            var storage = CurStorageThemeDecorationWeek;
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

    public List<ThemeDecorationStoreLevelConfig> ThemeDecorationStoreLevelConfigList
    {
        get
        {
            var configs = new List<ThemeDecorationStoreLevelConfig>();
            var allConfigs = ThemeDecorationConfigManager.Instance.GetConfig<ThemeDecorationStoreLevelConfig>();
            var storage = CurStorageThemeDecorationWeek;
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

    public List<ThemeDecorationStoreItemConfig> ThemeDecorationStoreItemConfigList
    {
        get
        {
            var configs = new List<ThemeDecorationStoreItemConfig>();
            var allConfigs = ThemeDecorationConfigManager.Instance.GetConfig<ThemeDecorationStoreItemConfig>();
            var storage = CurStorageThemeDecorationWeek;
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

    public List<ThemeDecorationTaskRewardConfig> ThemeDecorationTaskRewardConfigList
    {
        get
        {
            if (!IsOpened())
                return null;
            
            var configs = new List<ThemeDecorationTaskRewardConfig>();
            var allConfigs = ThemeDecorationConfigManager.Instance.GetConfig<ThemeDecorationTaskRewardConfig>();
            if (allConfigs == null)
                return null;
            
            var storage = CurStorageThemeDecorationWeek;
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

    public List<ThemeDecorationLeaderBoardRewardConfig> ThemeDecorationLeaderBoardRewardConfigList
    {
        get
        {
            var configs = new List<ThemeDecorationLeaderBoardRewardConfig>();
            var allConfigs = ThemeDecorationConfigManager.Instance.GetConfig<ThemeDecorationLeaderBoardRewardConfig>();
            var storage = CurStorageThemeDecorationWeek;
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

    public List<ThmeDecorationLeaderBoardScheduleConfig> ThmeDecorationLeaderBoardScheduleConfigList
    {
        get
        {
            var configs = new List<ThmeDecorationLeaderBoardScheduleConfig>();
            var allConfigs = ThemeDecorationConfigManager.Instance.GetConfig<ThmeDecorationLeaderBoardScheduleConfig>();
            var storage = CurStorageThemeDecorationWeek;
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