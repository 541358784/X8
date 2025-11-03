using System.Collections.Generic;
using System.Linq;
using Activity.TimeOrder;

namespace DragonPlus.Config.TimeOrder
{
    public partial class TimeOrderConfigManager
    {
        
        protected override void Trim()
        {
        }

        public List<TableTimeOrderConfig> GetTimeOrderConfigsByPayLevel()
        {
            var payLevelGroup = TimeOrderModel.Instance.TimeOrder.PayLevelGroup;
            var configs = new List<TableTimeOrderConfig>();
            var allConfigs = TableTimeOrderConfigList;
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
        
        public List<TableTimeOrderGift> GetTimeOrderGiftConfigsByPayLevel()
        {
            var payLevelGroup = TimeOrderModel.Instance.TimeOrder.PayLevelGroup;
            var configs = new List<TableTimeOrderGift>();
            var allConfigs = TableTimeOrderGiftList;
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