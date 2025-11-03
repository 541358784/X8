using Activity.BalloonRacing;
using Activity.CrazeOrder.Model;
using Activity.FarmTimeOrder;
using Activity.LimitTimeOrder;
using Activity.LuckyGoldenEgg;
using Activity.Matreshkas.Model;
using Activity.RabbitRacing.Dynamic;
using Activity.SaveTheWhales;
using Activity.TimeOrder;
using Activity.TotalRecharge;
using Activity.TreasureHuntModel;
using ExtraEnergy;
using Gameplay.UI.EnergyTorrent;
using Merge.UnlockMergeLine;
using TotalRecharge_New;

namespace AutoPopupManager
{
    public partial class AutoPopupManager
    {
        private AutoPopUI[] enterGameAutoPopUIArray;

        private void InitGameAutoPopUI()
        {
            enterGameAutoPopUIArray = new AutoPopUI[]
            {
                //玩狗
                new AutoPopUI(DogPlayModel.Instance.CanShowStartGuide, new[] { UINameConst.UIGuidePortrait,UINameConst.UIPopupDogPlay }),
                new AutoPopUI(DogPlayExtraRewardModel.Instance.CanShowStart, new[] { UINameConst.UIPopupDogPlayExtraReward}),
                new AutoPopUI(MermaidModel.CanShowUI, new[] {UINameConst.UIPopupMermaidStartPreview,UINameConst.UIPopupMermaidMain,UINameConst.UIPopupMermaidAddDay}),
                new AutoPopUI(EnergyTorrentModel.Instance.CanShowUI, new[] {UINameConst.UIPopupEnergyTorrentMain,UINameConst.UIPopupEnergyTorrentMainX8,UINameConst.UIPopupEnergyTorrentStart}),
                new AutoPopUI(UnlockMergeLineManager.Instance.UnLockMergeLine, new[] {UINameConst.UIPopupSyntheticChainUnlock}),
                new AutoPopUI(TimeOrderModel.Instance.CanShowJoin, new[] {UINameConst.UIPopupLimitedTimeTask}),
                new AutoPopUI(TimeOrderModel.Instance.CanShowGift, new[] {UINameConst.UIPopupTimeOrderGift}),
                new AutoPopUI(LimitTimeOrderModel.Instance.CanShowGift, new[] {UINameConst.UIPopupLimitOrderGift}),
                new AutoPopUI(SaveTheWhalesModel.Instance.CanShowJoin, new[] {UINameConst.UISaveTheWhalesReward}),
                new AutoPopUI(LimitTimeOrderModel.Instance.CanShowJoin, new[] {UINameConst.UIPopupLimitOrder}),
                new AutoPopUI(CrazeOrderModel.Instance.CanShowJoin, new[] {UINameConst.UICrazeOrderMain}),
                new AutoPopUI(MatreshkasModel.Instance.CanShowJoin, new[] {UINameConst.UIPopupMatreshkas}),
                new AutoPopUI(TreasureHuntModel.CanShowUI, new[] {UINameConst.UIPopupTreasureHuntStart,UINameConst.UITreasureHuntMain,UINameConst.UIPopupReward}),
                new AutoPopUI(LuckyGoldenEggModel.CanShowUI, new[] {UINameConst.UIPopupLuckyGoldenEggStart,UINameConst.UILuckyGoldenEggMain,UINameConst.UIPopupReward}),
                new AutoPopUI(ExtraEnergyModel.CanShowUI, new[] {UINameConst.UIPopupExtraEnergyStart}),
        
                new AutoPopUI(TotalRechargeModel_New.CanShowUI, new[] {UINameConst.UIPopupTotalRecharge_New}),
                new AutoPopUI(TotalRechargeModel.CanShowUI, new[] {UINameConst.UIPopupTotalRecharge}),

                new AutoPopUI(FarmTimeLimitOrderModel.Instance.CanShowJoin, new[] { UINameConst.UIPopupFarmTimeLimitOrder}),
                //气球竞速
                new AutoPopUI(BalloonRacingModel.Instance.CanShowJoinRacing,
                    new[]{
                        UINameConst.UIBalloonRacingStart, UINameConst.UIBalloonRacingMain, UINameConst.UIBalloonRacingReward, UINameConst.UIBalloonRacingOpenBox
                    }),
                //气球竞速
                new AutoPopUI(RabbitRacingModel.Instance.CanShowJoinRacing,
                    new[]{
                        UINameConst.UIPopupRabbitRacingStart, UINameConst.UIRabbitRacingMain, UINameConst.UIRabbitRacingReward, UINameConst.UIRabbitRacingOpenBox
                    }),
                
                new AutoPopUI(Activity.TrainOrder.TrainOrderModel.Instance.CanShowStart,
                    new[]
                    {
                        UINameConst.UIPopupTrainOrderStart
                    }),
            };
        }
    }
}