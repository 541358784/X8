using System.Collections.Generic;
using DragonPlus.Config.DiamondReward;

namespace Activity.DiamondRewardModel.Model
{
    public partial class DiamondRewardModel
    {
        public List<DiamondSettingConfig> DiamondSettingConfigList
        {
            get
            {
                return DiamondRewardConfigManager.Instance.GetConfig<DiamondSettingConfig>();
            }
        }
        public List<DiamondResultConfig> DiamondResultConfigList
        {
            get
            {
                return DiamondRewardConfigManager.Instance.GetConfig<DiamondResultConfig>();
            }
        }
        public List<DiamondPoolConfig> DiamondPoolConfigList
        {
            get
            {
                return DiamondRewardConfigManager.Instance.GetConfig<DiamondPoolConfig>();
            }
        }
        public DiamondSettingConfig DiamondSettingConfigLevel
        {
            get
            {
                return DiamondSettingConfigList.Find(a=>a.Id == DiamondReward.Level);
            }
        }
        public Dictionary<int,DiamondResultConfig> DiamondResultConfigLevelDic
        {
            get
            {
                var configList = DiamondResultConfigList;
                var configDic = new Dictionary<int, DiamondResultConfig>();
                foreach (var config in configList)
                {
                    if (config.Level == DiamondReward.Level)
                        configDic.Add(config.Id,config);
                }
                return configDic;
            }
        }
        
        public List<DiamondPoolConfig> DiamondPoolConfigLevelList
        {
            get
            {
                var configList = DiamondPoolConfigList;
                var configDic = new List<DiamondPoolConfig>();
                foreach (var config in configList)
                {
                    if (config.Level == DiamondReward.Level)
                        configDic.Add(config);
                }
                return configDic;
            }
        }
        public DiamondPoolConfig DiamondPoolConfigLevelPool
        {
            get
            {
                var configList = DiamondPoolConfigLevelList;
                foreach (var config in configList)
                {
                    if (config.Id == DiamondReward.PoolId)
                        return config;
                }
                return null;
            }
        }
    }
}