using System.Collections.Generic;
using System.Linq;

namespace DragonPlus.Config.JungleAdventure
{
    public partial class JungleAdventureConfigManager
    {
        private Dictionary<int, List<TableJungleAdventureConfig>> _configMap = new Dictionary<int, List<TableJungleAdventureConfig>>();
        protected override void Trim()
        {
            base.Trim();
            _configMap.Clear();
            foreach (var config in TableJungleAdventureConfigList)
            {
                if (!_configMap.ContainsKey(config.PayLevelGroup))
                    _configMap[config.PayLevelGroup] = new List<TableJungleAdventureConfig>();
                
                _configMap[config.PayLevelGroup].Add(config);
            }
        }

        public TableJungleAdventureConfig GetConfigByStage(int stage)
        {
            var config = GetConfigs().Find(a => a.Id == stage);
            if (config != null)
                return config;

            return GetConfigs().Last();
        }

        public List<TableJungleAdventureConfig> GetConfigs()
        {
            int group = JungleAdventureModel.Instance.PayLevelGroup();

            if (_configMap.ContainsKey(group))
                return _configMap[group];

            return _configMap[_configMap.Keys.First()];
        }
    }
}