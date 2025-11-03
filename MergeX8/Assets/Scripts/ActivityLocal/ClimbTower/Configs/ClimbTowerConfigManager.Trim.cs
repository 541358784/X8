using System.Collections.Generic;
using System.Linq;
using ActivityLocal.ClimbTower.Model;

namespace DragonPlus.Config.ClimbTower
{
    public partial class ClimbTowerConfigManager
    {
        private Dictionary<int, TableClimbTowerSetting> _settingMap = new Dictionary<int, TableClimbTowerSetting>();
        private Dictionary<int, List<TableClimbTowerReward>> _rewardMap = new Dictionary<int, List<TableClimbTowerReward>>();
        private Dictionary<int, List<TableClimbTowerReward>> _payRewardMap = new Dictionary<int, List<TableClimbTowerReward>>();
        
        protected override void Trim()
        {
            base.Trim();
            
            
            _settingMap.Clear();
            foreach (var config in TableClimbTowerSettingList)
            {
                _settingMap[config.PlayLevel] = config;
            }
            
            
            _rewardMap.Clear();
            _payRewardMap.Clear();
            foreach (var config in TableClimbTowerRewardList)
            {
                Dictionary<int, List<TableClimbTowerReward>> map = _rewardMap;
                if (config.IsPay)
                    map = _payRewardMap;
                
                if (!map.ContainsKey(config.PayLevel))
                    map[config.PayLevel] = new List<TableClimbTowerReward>();
                
                map[config.PayLevel].Add(config);
            }
        }

        public TableClimbTowerSetting GetSettingConfig()
        {
            var group = ClimbTowerModel.Instance.PlayLevel();
            if (_settingMap.ContainsKey(group))
                return _settingMap[group];

            return _settingMap[_settingMap.Keys.First()];
        }
        
        public List<TableClimbTowerReward> GetRewardConfig(bool isPay)
        {
            var group = ClimbTowerModel.Instance.PlayLevel();
            
            Dictionary<int, List<TableClimbTowerReward>> map = _rewardMap;
            if (isPay)
                map = _payRewardMap;
            
            if (map.ContainsKey(group))
                return map[group];

            return map[map.Keys.First()];
        }
    }
}