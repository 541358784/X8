using System.Collections.Generic;
using System.Linq;
using Activity.LuckyGoldenEgg;
using JetBrains.Annotations;

namespace DragonPlus.Config.LuckyGoldenEgg
{
    public partial class LuckyGoldenEggConfigManager
    {
        private TableLuckyGoldenEggActivityConfig _luckyGoldenEggActivityConfig;

        private Dictionary<int, TableLuckyGoldenEggSetting> _settingMap = new Dictionary<int, TableLuckyGoldenEggSetting>();

        protected override void Trim()
        {
            base.Trim();

            _settingMap.Clear();
            foreach (var config in TableLuckyGoldenEggSettingList)
            {
                _settingMap[config.PayLevel] = config;
            }
        }

        public TableLuckyGoldenEggSetting GetSettingConfig()
        {
            var group = LuckyGoldenEggModel.Instance.PayLevelGroup();

            if (_settingMap.ContainsKey(group))
                return _settingMap[group];

            return _settingMap[_settingMap.Keys.First()];
        }
        
        public TableLuckyGoldenEggActivityConfig luckyGoldenEggActivityConfig
        {
            get
            {
                if (_luckyGoldenEggActivityConfig == null)
                    _luckyGoldenEggActivityConfig = TableLuckyGoldenEggActivityConfigList[0];
                
                return _luckyGoldenEggActivityConfig;
            }
        }

        public List<TableLuckyGoldenEggLevelConfig> TableLuckyGoldenEggLevelConfigListByPayLevel
        {
            get
            {
                var payLevelGroup = LuckyGoldenEggModel.Instance.PayLevelGroup();
                var configs = new List<TableLuckyGoldenEggLevelConfig>();
                var allConfigs = TableLuckyGoldenEggLevelConfigList;
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
        public TableLuckyGoldenEggLevelConfig GetLuckyGoldenEggLevelConfig(int level)
        {
            if (level >= TableLuckyGoldenEggLevelConfigListByPayLevel.Count)
                return TableLuckyGoldenEggLevelConfigListByPayLevel.LastOrDefault();
            
            return TableLuckyGoldenEggLevelConfigListByPayLevel[level];
        }
        
        public TableLuckyGoldenEggStoreConfig GetLuckyGoldenEggStoreConfig(int shopId)
        {
            return TableLuckyGoldenEggStoreConfigList.Find(a=>a.ShopId==shopId);
        }
    }
}