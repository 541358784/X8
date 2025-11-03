using System.Linq;

namespace DragonPlus.Config.TipReward
{
    public partial class TipRewardConfigManager
    {
        public TableTipRewardSetting TipRewardSetting
        {
            get { return TableTipRewardSettingList.First(); }
        }
        
        protected override void Trim()
        {
            base.Trim();
        }
    }
}