using System.Collections.Generic;
using System.Linq;
using Activity.JumpGrid;

namespace DragonPlus.Config.JumpGrid
{
    public partial class JumpGridConfigManager
    {
        private Dictionary<int, List<TableJumpGridReward>> _rewardMap = new Dictionary<int, List<TableJumpGridReward>>();
        
        protected override void Trim()
        {
            base.Trim();

            _rewardMap.Clear();
            foreach (var config in TableJumpGridRewardList)
            {
                if (!_rewardMap.ContainsKey(config.PayLevelGroup))
                    _rewardMap[config.PayLevelGroup] = new List<TableJumpGridReward>();
                
                _rewardMap[config.PayLevelGroup].Add(config);
            }
        }

        public TableJumpGridConfig GetSettingConfig()
        {
            return TableJumpGridConfigList.First();
        }

        public List<TableJumpGridReward> GetRewards(int payLevelGroup)
        {
            if (_rewardMap.ContainsKey(payLevelGroup))
                return _rewardMap[payLevelGroup];

            return _rewardMap[_rewardMap.Keys.First()];
        }
        
        public List<TableJumpGridReward> GetRewards()
        {
            return GetRewards(JumpGridModel.Instance.PayLevelGroup());
        }
    }
}