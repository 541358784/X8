using Activity.DiamondRewardModel.Model;
using Activity.FarmTimeOrder;
using Activity.GardenTreasure.Model;
using Activity.TotalRecharge;
using Activity.TreasureHuntModel;
using Activity.Turntable.Model;
using ActivityLocal.CardCollection.Home;
using ExtraEnergy;
using OptionalGift;
using TotalRecharge_New;

namespace AutoPopupManager
{
    public partial class AutoPopupManager
    {
        private AutoPopUI[] _farmAutoPopUI;
        
        private void InitFarmPopUI()
        {
            _farmAutoPopUI = new AutoPopUI[]
            {
                //农场限时任务
                new AutoPopUI(FarmTimeLimitOrderModel.Instance.CanShowJoin, new[] { UINameConst.UIPopupFarmTimeLimitOrder}),
            };
        }
    }
}