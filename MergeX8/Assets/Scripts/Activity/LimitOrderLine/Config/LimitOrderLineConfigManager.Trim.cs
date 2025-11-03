using System.Collections.Generic;
using System.Linq;
using Activity.LimitTimeOrder;

namespace DragonPlus.Config.LimitOrderLine
{
    public partial class LimitOrderLineConfigManager
    {
        
        protected override void Trim()
        {
           
        }

        public List<TableTimeOrderLineGroup> GetTimeOrderConfigsByPayLevel()
        {
            var payLevelGroup = LimitTimeOrderModel.Instance.LimitOrderLine.PayLevelGroup;
            var configs = new List<TableTimeOrderLineGroup>();
            var allConfigs = TableTimeOrderLineGroupList;
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
        
        
        public List<TableTimeOrderLineGift> GetTimeOrderGiftConfigsByPayLevel()
        {
            var payLevelGroup = LimitTimeOrderModel.Instance.LimitOrderLine.PayLevelGroup;
            var configs = new List<TableTimeOrderLineGift>();
            var allConfigs = TableTimeOrderLineGiftList;
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