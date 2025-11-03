using System.Collections.Generic;
using System.Linq;
using Activity.CrazeOrder.Model;

namespace DragonPlus.Config.CrazeOrder
{
    public partial class CrazeOrderConfigManager : Manager<CrazeOrderConfigManager>
    {
        public void Trim()
        {
            
        }

        public Dictionary<int, List<CrazeOrderConfig>> GetCrazeOrderConfigsByPayLevel()
        {

            var payLevelGroup = CrazeOrderModel.Instance.CrazeOrder.PayLevelGroup;
            var configs = new List<CrazeOrderConfig>();
            var allConfigs = CrazeOrderConfigList;
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

            var configDic = new Dictionary<int, List<CrazeOrderConfig>>();
            foreach (var config in configs)
            {
                if (!configDic.ContainsKey(config.Level))
                    configDic.Add(config.Level,new List<CrazeOrderConfig>());
                configDic[config.Level].Add(config);
            }
            return configDic;
        }
        
        public List<CrazeOrderConfig> GetOrderConfig( int level)
        {
            var configs = GetCrazeOrderConfigsByPayLevel();
            if (!configs.ContainsKey(level))
                return null;
            
            return configs[level];
        }

        public int AdaptOrderLevel(int level)
        {
            var configs = GetCrazeOrderConfigsByPayLevel();
            foreach (var kv in configs)
            {
                if (level <= kv.Key)
                    return kv.Key;
            }

            return configs.Last().Key;
        }

        public List<CrazeStageConfig> GetStageConfigs()
        {
            var payLevelGroup = CrazeOrderModel.Instance.CrazeOrder.PayLevelGroup;
            var configs = new List<CrazeStageConfig>();
            var allConfigs = CrazeStageConfigList;
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