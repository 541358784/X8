using System.Collections.Generic;
using System.Linq;

namespace DragonPlus.Config.Team
{
    public partial class TeamConfigManager
    {
        public TableTeamGlobalConfig LocalTeamConfig => TableTeamGlobalConfigList.First();
        
        public List<TableTeamLevelConfig> LevelConfigList =>TableTeamLevelConfigList;
        public List<TableTeamIconConfig> IconConfigList => TableTeamIconConfigList;
        public List<TableTeamShopConfig> ShopConfigList => TableTeamShopConfigList;
        
        public List<TableTeamIconFrameConfig> IconFrameConfigList => TableTeamIconFrameConfigList;

        private Dictionary<int, List<TableTeamOrder>> _teamOrders = new Dictionary<int, List<TableTeamOrder>>();
        protected override void Trim()
        {
            base.Trim();
            
            _teamOrders.Clear();
            
            foreach (var tableTeamOrder in TableTeamOrderList)
            {
                if(!_teamOrders.ContainsKey(tableTeamOrder.PayLevelGroup))
                    _teamOrders.Add(tableTeamOrder.PayLevelGroup, new List<TableTeamOrder>());
                
                _teamOrders[tableTeamOrder.PayLevelGroup].Add(tableTeamOrder);
            }
        }

        public List<TableTeamOrder> GetTeamOrdersByPayLevel(int payLevel)
        {
            if (_teamOrders.ContainsKey(payLevel))
                return _teamOrders[payLevel];

            return _teamOrders[_teamOrders.Keys.First()];
        }
        
    }
}