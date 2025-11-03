using Activity.CollectStone.Model;
using Activity.DiamondRewardModel.Model;
using Activity.GardenTreasure.Model;
using Activity.JumpGrid;
using Activity.JungleAdventure.Controller;
using Activity.LuckyGoldenEgg;
using Activity.TotalRecharge;
using Activity.TreasureHuntModel;
using Activity.Turntable.Model;
using ActivityLocal.CardCollection.Home;
using Decoration.DaysManager;
using ExtraEnergy;
using Gameplay.UI.BindEmail;
using Gameplay.UI.UpdateRewardManager;
using OptionalGift;
using Scripts.UI;
using TotalRecharge_New;

namespace AutoPopupManager
{
    public partial class AutoPopupManager
    {
        private AutoPopUI[] firstAutoPopUIArray;

        private void InitFirstAutoPopUI()
        {
            firstAutoPopUIArray = new AutoPopUI[]
            {
                //*******************优先级最高 勿动*******************
                new AutoPopUI(UIPopupBindEmailController.CanShow, new[] { UINameConst.UIPopupBindEmail }),
                //卡牌轮询收集
                new AutoPopUI(CardCollectionModel.Instance.DoAllUndoActionInAutoPopup, CardUIName.CardAllUINames),
                //居居
                new AutoPopUI(PigBankModel.Instance.TryCreateLocalGame, new[] { UINameConst.UIPopupPigBox }),
                //公会
                new AutoPopUI(TeamManager.Instance.CanShowEntranceGuide, new[] { UINameConst.UIGuidePortrait,UINameConst.UIPopupGuildJoin,UINameConst.UIPopupGuildMain }),
                //牛牛破冰
                new AutoPopUI(NewNewIceBreakPackModel.Instance.CanShowUI,new[] { UINameConst.UIPopupNewNewIceBreakPack,UINameConst.UIPopupNewNewIceBreakPackFinish, UINameConst.UIPopupReward}),
                //卡皮Tile
                new AutoPopUI(KapiTileModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait, UINameConst.UIKapiTileMain }),
                //卡皮钉子
                new AutoPopUI(KapiScrewModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait, UINameConst.UIKapiScrewMain }),
                //新破冰礼包
                new AutoPopUI(NewIceBreakGiftBagModel.CanShowNewIceBreakGiftBagOnEnterGame, new[] { UINameConst.UIPopupNewbiePack }),
                //卡皮巴拉
                new AutoPopUI(KapibalaModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait, UINameConst.UIKapibalaMain }),
                //枕头
                new AutoPopUI(PillowWheelLeaderBoardModel.Instance.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UIPillowWheelLeaderBoardMain }),
                new AutoPopUI(PillowWheelModel.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupPillowWheelPreview }),
                new AutoPopUI(PillowWheelModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait, UINameConst.UIPillowWheelMain }),
                //祖玛
                new AutoPopUI(ZumaLeaderBoardModel.Instance.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UIZumaBoardMain }),
                new AutoPopUI(ZumaModel.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupZumaPreview }),
                new AutoPopUI(ZumaModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait, UINameConst.UIPopupZumaStart, UINameConst.UIZumaMain }),
                new AutoPopUI(ZumaModel.CanShowStartPopup, new[] { UINameConst.UIPopupZumaStart, UINameConst.UIZumaMain }),
                //乌龟对对碰
                new AutoPopUI(TurtlePangModel.Instance.CanShowPreheatUI, new[] { UINameConst.UIPopupTurtlePangPreview }),
                new AutoPopUI(TurtlePangModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait }),
                //养狗引导
                new AutoPopUI(KeepPetModel.Instance.CanShowReturnPopup, new[] { UINameConst.UIPopupKeepPetReturn }),
                new AutoPopUI(KeepPetModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait }),
                new AutoPopUI(KeepPetModel.Instance.CanShowHungryGuide, new[] { UINameConst.UIGuidePortrait }),
                new AutoPopUI(KeepPetModel.Instance.CanShowUnCollectReward, new[] { UINameConst.UIPopupReward }),
                //公共资源排行榜
                new AutoPopUI(CommonResourceLeaderBoardModel.Instance.CheckUnCollectStorage, new[] { UINameConst.UIWaiting }),
                //大富翁
                new AutoPopUI(MonopolyLeaderBoardModel.Instance.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UIMonopolyLeaderBoardMain }),
                new AutoPopUI(MonopolyModel.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupMonopolyPreview }),
                new AutoPopUI(MonopolyModel.CanStartGuide, new[] { UINameConst.UIMonopolyMain }),
                new AutoPopUI(MonopolyModel.CanShowStartPopup, new[] { UINameConst.UIPopupMonopolyStart, UINameConst.UIMonopolyMain }),
                new AutoPopUI(TurntableModel.Instance.CanShowUI, new[] { UINameConst.UIPopupTurntableMain }),
                new AutoPopUI(TotalRechargeModel_New.CanShowUI, new[] { UINameConst.UIPopupTotalRecharge_New }),
                new AutoPopUI(TotalRechargeModel.CanShowUI, new[] { UINameConst.UIPopupTotalRecharge }),

                //🎰
                new AutoPopUI(SlotMachineModel.CanShowStartPopup, new[] { UINameConst.UIPopupSlotMachineMain }),
                //蛇梯子
                new AutoPopUI(SnakeLadderModel.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UISnakeLadderLeaderBoardMain }),
                new AutoPopUI(SnakeLadderModel.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupSnakeLadderPreview }),
                new AutoPopUI(SnakeLadderModel.CanShowStartPopup, new[] { UINameConst.UIPopupSnakeLadderStart, UINameConst.UISnakeLadderMain }),
                //复活节2024
                new AutoPopUI(Easter2024Model.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UIEaster2024LeaderBoardMain }),
                new AutoPopUI(Easter2024Model.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupEaster2024Preview }),
                new AutoPopUI(Easter2024Model.CanShowStartPopup, new[] { UINameConst.UIPopupEaster2024Start, UINameConst.UIEaster2024Main }),

                new AutoPopUI(UpdateRewardManager.CanShow, new[] { UINameConst.UIPopupRewardItem }),
                new AutoPopUI(DaysManager.Instance.CanShowRetrieveReward, new[] { UINameConst.UIPopupRewardItem }),
                //签到
                // new AutoPopUI(UIDailyBounsController.CanShowUI, new[] {UINameConst.UIDailyBouns}),       
                //周卡
                new AutoPopUI(UIWeeklyCardController.CanShowUI, new[] { UINameConst.UIWeeklyCard }),
                //美人鱼
                new AutoPopUI(MermaidModel.CanShowUI,
                    new[]
                    {
                        UINameConst.UIPopupMermaidStartPreview, UINameConst.UIPopupMermaidMain, UINameConst.UIPopupMermaidAddDay, UINameConst.MermaidMapPreview, UINameConst.MermaidMapBuild,
                        UINameConst.UIStory
                    }),
                //美人鱼双倍
                new AutoPopUI(UIPopupMermaidDoubleController.CanShowUI, new[] { UINameConst.UIPopupMermaidDouble }),
                //相册
                new AutoPopUI(PhotoAlbumModel.CanShowFinishPopup, new[] { UINameConst.UIPhotoAlbumShop}),
                new AutoPopUI(PhotoAlbumModel.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupPhotoAlbumPreview }),
                new AutoPopUI(PhotoAlbumModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait, UINameConst.UIPhotoAlbumShop }),
                new AutoPopUI(PhotoAlbumModel.CanShowStartPopup, new[] {  UINameConst.UIPhotoAlbumShop }),

                //主题装修
                new AutoPopUI(ThemeDecorationModel.CanShowUnCollectRewardsUI, new[] { UINameConst.UIPopupActivityUnCollectReward }),
                new AutoPopUI(ThemeDecorationLeaderBoardModel.CanShowUnCollectRewardsUI, ThemeDecorationLeaderBoardModel.ShowUnCollectRewardsUIList),
                new AutoPopUI(ThemeDecorationModel.CanShowPreheatPopupEachDay, ThemeDecorationModel.ShowPreheatPopupUIList),
                new AutoPopUI(ThemeDecorationModel.CanShowStartPopup, ThemeDecorationModel.ShowStartPopupUIList),
                new AutoPopUI(ThemeDecorationModel.CanShowBuyPreEndUIEachDay, ThemeDecorationModel.ShowBuyPreEndUIList),
                new AutoPopUI(ThemeDecorationLeaderBoardModel.CanShowMainUIPerDay, ThemeDecorationLeaderBoardModel.ShowMainUIWeekList),
                //主题装修双倍
                new AutoPopUI(ThemeDecorationModel.CanShowMultipleScore, ThemeDecorationModel.ShowMultipleScoreUIList),
                //卡牌引导
                new AutoPopUI(CardCollectionActivityModel.CanShowStart, CardUIName.CardUINames),
                //狗火鸡
                new AutoPopUI(KeepPetTurkeyModel.CanShowStart, new[] { UINameConst.UIPopupKeepPetTurkeyStart, UINameConst.UIKeepPetMain }),
                // new AutoPopUI(CardCollectionReopenActivityModel.CanShowStart,CardUIName.CardReopenUINames),
                //Happy Go
                new AutoPopUI(HappyGoModel.CanShowUI, new[] { UINameConst.UIPopupHappyGoStart, UINameConst.UIPopupHappyGoExtend, UINameConst.UIStory }),
                //猴子爬树
                new AutoPopUI(ClimbTreeLeaderBoardModel.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UIClimbTreeLeaderBoardMain }),
                new AutoPopUI(UIPopupClimbTreeEndController.CanShowUI, new[] { UINameConst.UIPopupActivityUnCollectReward }),
                new AutoPopUI(UIClimbTreeStartController.CanShowUI, new[] { UINameConst.UIClimbTreeStart, UINameConst.UIClimbTreeMain }),
                //小狗
                new AutoPopUI(DogHopeLeaderBoardModel.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UIDogHopeLeaderBoardMain }),
                new AutoPopUI(UIDogMainController.CanShowUI, new[] { UINameConst.UIDogStart, UINameConst.UIDogMain }),
                //鹦鹉
                new AutoPopUI(ParrotLeaderBoardModel.Instance.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UIParrotLeaderBoardMain }),
                new AutoPopUI(ParrotModel.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupParrotPreview}),
                new AutoPopUI(ParrotModel.CanShowStartPopup, new[] { UINameConst.UIParrotStart,UINameConst.UIParrotMain}),
                new AutoPopUI(ParrotModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait,UINameConst.UIParrotMain}),
                //花田
                new AutoPopUI(FlowerFieldLeaderBoardModel.Instance.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UIFlowerFieldLeaderBoardMain }),
                new AutoPopUI(FlowerFieldModel.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupFlowerFieldPreview}),
                // new AutoPopUI(FlowerFieldModel.CanShowStartPopup, new[] { UINameConst.UIFlowerFieldStart,UINameConst.UIFlowerFieldMain}),
                new AutoPopUI(FlowerFieldModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait,UINameConst.UIFlowerFieldMain}),
                //丛林探险
                new AutoPopUI(UIJungleAdventureMainController.CanShow, new[] { UINameConst.UIJungleAdventureMain}),
                new AutoPopUI(UIPopupJungleAdventurePreviewController.CanShow, new[] { UINameConst.UIPopupJungleAdventurePreview }),
                new AutoPopUI(JungleAdventureModel.CanShow, new[] { UINameConst.UIJungleAdventureMain }),
                new AutoPopUI(JungleAdventureLeaderBoardModel.Instance.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UIJungleAdventureBoardMain }),
                //养鱼
                new AutoPopUI(FishCultureLeaderBoardModel.Instance.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UIFishCultureBoardMain }),
                new AutoPopUI(FishCultureModel.CanShowFinishPopup, new[] { UINameConst.UIFishCultureMain}),
                new AutoPopUI(FishCultureModel.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupFishCulturePreview }),
                new AutoPopUI(FishCultureModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait, UINameConst.UIFishCultureMain }),
                new AutoPopUI(FishCultureModel.CanShowStartPopup, new[] {  UINameConst.UIFishCultureMain }),

                //花园宝藏
                //new AutoPopUI(GardenTreasureLeaderBoardModel.Instance.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UIGardenTreasureLeaderBoardMain }),
                new AutoPopUI(GardenTreasureModel.Instance.CanShowUI, new[] { UINameConst.UIGardenTreasureMain, UINameConst.UIPopupGardenTreasureStart }),
                //金币挑战
                new AutoPopUI(CoinCompetitionModel.CanShowUnCollectRewardsUI, new[] { UINameConst.UIPopupActivityUnCollectReward }),
                new AutoPopUI(CoinCompetitionModel.CanShowUI, new[] { UINameConst.UICoinCompetitionStart, UINameConst.UICoinCompetitionMain }),
                // 跳格子
                new AutoPopUI(JumpGridModel.CanShowUnCollectRewardsUI, new[] { UINameConst.UIPopupActivityUnCollectReward }),
                new AutoPopUI(JumpGridModel.CanShowUI, new[] { UINameConst.UIPopupJumpGridStart, UINameConst.UIJumpGridMain }),
                //海上竞速
                new AutoPopUI(SeaRacingModel.CanShowUnCollectRewardsUI, new[] { UINameConst.UISeaRacingReward, UINameConst.UIPopupSeaRacingEnd }),
                // new AutoPopUI(SeaRacingModel.CanShowPreheatPopupEachDay, new[] {UINameConst.UIPopupSeaRacingPreview}),
                new AutoPopUI(SeaRacingModel.CanAutoShowStartPopup, new[] { UINameConst.UIPopupSeaRacingStart, UINameConst.UISeaRacingMain }),
                //寻宝活动
                new AutoPopUI(TreasureHuntModel.CanShowUI, new[] { UINameConst.UIPopupTreasureHuntStart, UINameConst.UITreasureHuntMain, UINameConst.UIPopupReward }),
                //幸运金蛋
                new AutoPopUI(LuckyGoldenEggModel.CanShowUI, new[] { UINameConst.UIPopupLuckyGoldenEggStart, UINameConst.UILuckyGoldenEggMain, UINameConst.UIPopupReward }),
                //调制大师
                new AutoPopUI(MixMasterModel.Instance.CanShowPreheatUI, new[] { UINameConst.UIPopupMixMasterPreview }),
                new AutoPopUI(MixMasterModel.Instance.CanShowUI, new[] { UINameConst.UIMixMasterMain }),
                //星空罗盘
                new AutoPopUI(StarrySkyCompassModel.Instance.CanShowPreheatUI, new[] { UINameConst.UIPopupStarrySkyCompassPreview }),
                new AutoPopUI(StarrySkyCompassModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait }),
                //金币rush
                new AutoPopUI(CoinRushModel.CanShowUnCollectRewardsUI, new[] { UINameConst.UIPopupActivityUnCollectReward }),
                new AutoPopUI(CoinRushModel.CanShowMainPopup, CoinRushModel.ShowMainUIList),
                //金币排行榜
                new AutoPopUI(CoinLeaderBoardModel.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UICoinLeaderBoardMain }),
                new AutoPopUI(CoinLeaderBoardModel.CanShowActivityStartUI, new[] { UINameConst.UICoinLeaderBoardStart, UINameConst.UICoinLeaderBoardMain }),
                new AutoPopUI(CoinLeaderBoardModel.CanShowMainUIPerDay, new[] { UINameConst.UICoinLeaderBoardMain }),
                //星星排行榜
                new AutoPopUI(RecoverCoinModel.CanShowUnCollectRewardsUI, RecoverCoinModel.ShowUnCollectRewardsUIList),
                new AutoPopUI(RecoverCoinModel.CanShowNewNodeTipUI, RecoverCoinModel.ShowNewNodeTipUIList),
                new AutoPopUI(RecoverCoinModel.CanShowAllNodeFinishUI, RecoverCoinModel.ShowAllNodeFinishUI),
                new AutoPopUI(RecoverCoinModel.CanShowActivityStartUI, RecoverCoinModel.ShowActivityStartUIList),
                new AutoPopUI(RecoverCoinModel.CanShowMainUIPerDay, RecoverCoinModel.ShowActivityMainUIList),
                //清理鱼塘
                new AutoPopUI(UIPopupGarageCleanupStartController.CanShowUI, new[] { UINameConst.UIPopupGarageCleanupStart, UINameConst.UIGarageCleanupMain }),
                //合成西瓜
                new AutoPopUI(SummerWatermelonModel.CanShowUnCollectRewardsUI, new[] { UINameConst.UIPopupActivityUnCollectReward }),
                new AutoPopUI(SummerWatermelonModel.CanShowStartView, new[] { UINameConst.UIPopupSummerWatermelonStart, UINameConst.UIPopupSummerWatermelonMain }),
                new AutoPopUI(SummerWatermelonModel.CanShowPackagePopupEachDay,
                    new[] { SummerWatermelonModel.Instance.PackageUIPath, UINameConst.UIPopupReward, UINameConst.UIPopupSummerWatermelonMain }),
                //蝴蝶工坊
                new AutoPopUI(ButterflyWorkShopModel.CanShowUI, new[] { UINameConst.UIButterflyWorkShopMain, UINameConst.UIPopupButterflyWorkShopStart }),
                //飞镖
                new AutoPopUI(BiuBiuModel.CanShowStart, new[] { UINameConst.UIBiuBiuMain}),
                //合成面包
                new AutoPopUI(SummerWatermelonBreadModel.CanShowUnCollectRewardsUI, new[] { UINameConst.UIPopupActivityUnCollectReward }),
                new AutoPopUI(SummerWatermelonBreadModel.CanShowStartView, new[] { UINameConst.UIPopupSummerWatermelonBreadStart, UINameConst.UIPopupSummerWatermelonBreadMain }),
                new AutoPopUI(SummerWatermelonBreadModel.CanShowPackagePopupEachDay,
                    new[] { SummerWatermelonBreadModel.Instance.PackageUIPath, UINameConst.UIPopupReward, UINameConst.UIPopupSummerWatermelonBreadMain }),
                //钻石抽奖
                new AutoPopUI(DiamondRewardModel.Instance.CanShowUI, new[] { UINameConst.UIDiamondRewardMain }),
                //BP1
                new AutoPopUI(Activity.BattlePass.BattlePassModel.Instance.CanShowRefresh, new[] { UINameConst.UIPopupBattlePassRefresh }),
                new AutoPopUI(Activity.BattlePass.BattlePassModel.CheckActivityEnd,
                    new[] { UINameConst.UIPopupBattlePassEnd, UINameConst.UIPopupBattlePassEndBuy, UINameConst.UIPopupReward, UINameConst.UIEasterStart, UINameConst.UIEasterMain }),
                new AutoPopUI(Activity.BattlePass.UIPopupBattlePassRefreshController.CanShow, new[] { UINameConst.UIPopupBattlePassRefresh }),
                new AutoPopUI(Activity.BattlePass.BattlePassModel.Instance.CanShow, new[] { UINameConst.UIPopupBattlePassStart, UINameConst.UIBattlePassMain }),

                //BP2
                new AutoPopUI(Activity.BattlePass_2.BattlePassModel.Instance.CanShowRefresh, new[] { UINameConst.UIPopupBattlePass2Refresh }),
                new AutoPopUI(Activity.BattlePass_2.BattlePassModel.CheckActivityEnd,
                    new[] { UINameConst.UIPopupBattlePass2End, UINameConst.UIPopupBattlePass2EndBuy, UINameConst.UIPopupReward, UINameConst.UIEasterStart, UINameConst.UIEasterMain }),
                new AutoPopUI(Activity.BattlePass_2.UIPopupBattlePassRefreshController.CanShow, new[] { UINameConst.UIPopupBattlePass2Refresh }),
                new AutoPopUI(Activity.BattlePass_2.BattlePassModel.Instance.CanShow, new[] { UINameConst.UIPopupBattlePass2Start, UINameConst.UIBattlePass2Main }),
                //破冰礼包
                new AutoPopUI(UIPopupIcebreakingPackController.CanShowUI, new[] { UINameConst.UIPopupIcebreakingPack, UINameConst.UIPopupReward }),
                //破冰礼包2
                new AutoPopUI(UIPopupIcebreakingPackLowController.CanShowUI, new[] { UINameConst.UIPopupIcebreakingPackLow, UINameConst.UIPopupReward }),
                //钻石增殖券
                new AutoPopUI(BuyDiamondTicketModel.CanShowStartPopupEachDay, new[] { UINameConst.UIBuyDiamondTicket }),
                //每日礼包补丁
                new AutoPopUI(NewDailyPackageExtraRewardModel.CanShowStartPopupEachDay, NewDailyPackageExtraRewardModel.ShowMainUIList ),
                //本地充值返利
                new AutoPopUI(UIPopupPayRebateLocalController.CanShowUI, new[] { UINameConst.UIPopupPayRebateLocal, UINameConst.UIStore }),
                //商店额外奖励
                new AutoPopUI(ShopExtraRewardModel.CanShowStartPopupEachDay, new[] { UINameConst.UIPopupShopExtraRewardStart, UINameConst.UIStore }),
                //俩礼包
                new AutoPopUI(GiftBagDoubleModel.Instance.CanShowUI, new[] { UINameConst.UIPopupGiftBagDoubleMain }),
                //进步礼包
                new AutoPopUI(GiftBagProgressModel.Instance.CheckUnCollectTaskRewards, new[] { UINameConst.UIPopupActivityUnCollectReward }),
                new AutoPopUI(GiftBagProgressModel.Instance.CanShowUI, new[] { UINameConst.UIPopupGiftBagProgressTask }),
                //三合一礼包
                new AutoPopUI(ThreeGift.ThreeGiftModel.CanShowUI, new[] { UINameConst.UIPopupThreeGift, UINameConst.UIPopupReward }),
                //自选礼包
                new AutoPopUI(OptionGiftModel.CanShowUI, new[] { UINameConst.UIPopupOptionalGiftMain, UINameConst.UIPopupReward }),
                new AutoPopUI(MultipleGift.MultipleGiftModel.CanShowUI, new[] { UINameConst.UIPopupMultipleGift, UINameConst.UIPopupReward }),
                //越买越划算礼包
                new AutoPopUI(UIPopupGiftBagBuyBetterController.CanShowUI, new[] { UINameConst.UIPopupGiftBagBuyBetter }),
                //买一赠一礼包
                new AutoPopUI(UIPopupGiftBagSendOneController.CanShowUI, new[] { UINameConst.UIPopupGiftBagSendOne }),
                //买一赠二礼包
                new AutoPopUI(UIPopupGiftBagSendTwoController.CanShowUI, new[] { UINameConst.UIPopupGiftBagSendTwo }),
                //买一赠二加一礼包
                new AutoPopUI(UIPopupGiftBagSendThreeController.CanShowUI, new[] { UINameConst.UIPopupGiftBagSendThree }),
                //买一赠4礼包
                new AutoPopUI(UIPopupGiftBagSend4Controller.CanShowUI, new[] { UINameConst.UIPopupGiftBagSend4 }),
                //买一赠6礼包
                new AutoPopUI(UIPopupGiftBagSend6Controller.CanShowUI, new[] { UINameConst.UIPopupGiftBagSend6 }),
                //礼包链
                new AutoPopUI(UIGiftBagLinkController.CanShowUI, new[] { UINameConst.UIGiftBagLink }),
                //小猪存钱罐
                new AutoPopUI(PigBankModel.Instance.CanShow, new[] { UINameConst.UIPopupPigBox }),
                //每日礼包
                // new AutoPopUI(UIDailyPackController.CanShowUI, new[] {UINameConst.UIDailyPack}),
                new AutoPopUI(UIDailyPack2Controller.CanShowUI, new[] { UINameConst.UIDailyPack2 }),
                // new AutoPopUI(UIPopupDailyGiftController.CanShowUI, new[] {UINameConst.UIPopupDailyGift,UINameConst.UIPopupReward}),
                new AutoPopUI(UIPopupNewDailyGiftController.CanShowUI, new[] { UINameConst.UIPopupNewDailyGift, UINameConst.UIPopupReward }),
                //视频广告链
                new AutoPopUI(UIDailyRVController.CanShowUI, new[] { UINameConst.UIDailyRV }),
                //海豹礼包
                new AutoPopUI(UISealPackController.CanShowUI, new[] { UINameConst.UISealPack, UINameConst.UIPopupReward }),
                //海豚礼包
                new AutoPopUI(UIDolphinPackController.CanShowUI, new[] { UINameConst.UIDolphinPack, UINameConst.UIPopupReward }),
                //复活节
                new AutoPopUI(UIEasterMainController.CanShowUI, new[] { UINameConst.UIEasterStart, UINameConst.UIEasterMain }),
                //复活节礼包
                new AutoPopUI(UIEasterShopController.CanShowUI, new[] { UINameConst.UIEasterShop, UINameConst.UIPopupReward }),
                new AutoPopUI(UIEasterPackController.CanShowUI, new[] { UINameConst.UIEasterPack }),
                //充值返利
                new AutoPopUI(UIPopupPayRebateController.CanShowUI, new[] { UINameConst.UIPopupPayRebate, UINameConst.UIStore }),
                //合成次数排行
                new AutoPopUI(UIPopupLevelRankingShowController.CanShowUI,
                    new[] { UINameConst.UIPopupLevelRankingStart, UINameConst.UIPopupLevelRankingShow, UINameConst.UIPopupLevelRankingMain, UINameConst.UIPopupReward }),
                new AutoPopUI(ExtraEnergyModel.CanShowUI, new[] { UINameConst.UIPopupExtraEnergyStart }),
                //收集石头
                new AutoPopUI(CollectStoneModel.CanShowUI,new[] { UINameConst.UIPopupCollectStoneMain}),
                
                
            };
        }
    }
}