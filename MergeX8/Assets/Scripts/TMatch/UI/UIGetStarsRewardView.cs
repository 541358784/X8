

namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UICommonStarsReward")]
    public class UIGetStarsRewardView : UIGetRewardView
    {
        public override bool hasBox => true;
        protected override int RewardAddType => 2;
    }
}