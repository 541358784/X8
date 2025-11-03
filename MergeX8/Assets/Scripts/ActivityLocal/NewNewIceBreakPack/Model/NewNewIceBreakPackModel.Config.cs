using System.Collections.Generic;

public partial class NewNewIceBreakPackModel
{
    public List<TableNewNewIceBreakPackGlobal> NewNewIceBreakPackGlobalList
    {
        get
        {
            var configs = new List<TableNewNewIceBreakPackGlobal>();
            var allConfigs = GlobalConfigManager.Instance.NewNewIceBreakPackGlobalList;
            foreach (var config in allConfigs)
            {
                if (config.payLevelGroup == Storage.PayLevelGroup)
                    configs.Add(config);
            }
            if (configs.Count == 0)
            {
                foreach (var config in allConfigs)
                {
                    if (config.payLevelGroup == 0)
                        configs.Add(config);
                }
            }
            return configs;
        }
    }

    public List<TableNewNewIceBreakPackReward> NewNewIceBreakPackRewardList
    {
        get
        {
            var configs = new List<TableNewNewIceBreakPackReward>();
            var allConfigs = GlobalConfigManager.Instance.NewNewIceBreakPackRewardList;
            foreach (var config in allConfigs)
            {
                if (config.payLevelGroup == Storage.PayLevelGroup)
                    configs.Add(config);
            }
            if (configs.Count == 0)
            {
                foreach (var config in allConfigs)
                {
                    if (config.payLevelGroup == 0)
                        configs.Add(config);
                }
            }
            return configs;
        }
    }
}