using System.Collections.Generic;
using System.Linq;

namespace DragonPlus.Config.CollectStone
{
    public partial class CollectStoneConfigManager
    {        
        private Dictionary<int, TableCollectSetting> _configMap = new Dictionary<int, TableCollectSetting>();

        protected override void Trim()
        {
            base.Trim();
            
            _configMap.Clear();
            foreach (var config in TableCollectSettingList)
            {
               _configMap[config.PayLevel] = config;
            }
        }

        public TableCollectSetting GetCollectSetting(int payLevel)
        {
            if (_configMap.ContainsKey(payLevel))
                return _configMap[payLevel];

            return _configMap[_configMap.Keys.First()];
        }
    }
}