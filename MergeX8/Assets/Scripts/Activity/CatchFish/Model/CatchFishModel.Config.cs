using System.Collections.Generic;
using DragonPlus.Config.CatchFish;

public partial class CatchFishModel
{
    public CatchFishGlobalConfig GlobalConfig => GlobalConfigList[0];
    public List<CatchFishGlobalConfig> GlobalConfigList
    {
        get
        {
            var configs = new List<CatchFishGlobalConfig>();
            var allConfigs = CatchFishConfigManager.Instance.GetConfig<CatchFishGlobalConfig>();
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
    public List<CatchFishShopConfig> ShopConfigList
    {
        get
        {
            return CatchFishConfigManager.Instance.GetConfig<CatchFishShopConfig>();
        }
    }
    
    public List<CatchFishTaskRewardConfig> TaskRewardConfigList
    {
        get
        {
            return CatchFishConfigManager.Instance.GetConfig<CatchFishTaskRewardConfig>();
        }
    }
}