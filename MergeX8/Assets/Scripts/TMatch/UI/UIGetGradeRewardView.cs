

namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UICommonGradeReward")]
    public class UIGetGradeRewardView : UIGetRewardView
    {
        public override bool hasBox => true;
        protected override int RewardAddType => 2;
    }
}