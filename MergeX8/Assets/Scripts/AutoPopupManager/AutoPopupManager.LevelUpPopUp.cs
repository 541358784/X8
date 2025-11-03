using Activity.JumpGrid;
using Activity.JungleAdventure.Controller;
using ActivityLocal.CardCollection.Home;
using ExtraEnergy;
using Gameplay.UI.BindEmail;
using Scripts.UI;

namespace AutoPopupManager
{
    public partial class AutoPopupManager
    {
        private AutoPopUI[] levelUpAutoPopUIArray;

        private void InitLevelUpPopUI()
        {
            levelUpAutoPopUIArray = new AutoPopUI[]
            {
                //*******************优先级最高 勿动*******************
                new AutoPopUI(UIPopupBindEmailController.CanShow, new[] { UINameConst.UIPopupBindEmail }),
                //居居
                new AutoPopUI(PigBankModel.Instance.TryCreateLocalGame, new[] { UINameConst.UIPopupPigBox }),
                //公会
                new AutoPopUI(TeamManager.Instance.CanShowEntranceGuide, new[] { UINameConst.UIGuidePortrait,UINameConst.UIPopupGuildJoin,UINameConst.UIPopupGuildMain }),
                //枕头
                new AutoPopUI(PillowWheelModel.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupPillowWheelPreview }),
                new AutoPopUI(PillowWheelModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait, UINameConst.UIPillowWheelMain }),
                //牛牛破冰
                new AutoPopUI(NewNewIceBreakPackModel.Instance.CanShowUI,new[] { UINameConst.UIPopupNewNewIceBreakPack,UINameConst.UIPopupNewNewIceBreakPackFinish, UINameConst.UIPopupReward}),
                //卡皮Tile
                new AutoPopUI(KapiTileModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait, UINameConst.UIKapiTileMain }),
                //卡皮钉子
                new AutoPopUI(KapiScrewModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait, UINameConst.UIKapiScrewMain }),
                //新破冰礼包
                new AutoPopUI(NewIceBreakGiftBagModel.CanShowNewIceBreakGiftBagOnLevelUp, new[] { UINameConst.UIPopupNewbiePack }),
                //养狗引导
                new AutoPopUI(KeepPetModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait }),
                new AutoPopUI(KeepPetModel.Instance.CanShowHungryGuide, new[] { UINameConst.UIGuidePortrait }),
                //大富翁
                new AutoPopUI(MonopolyModel.CanStartGuide, new[] { UINameConst.UIMonopolyMain }),
                //升级礼包
                new AutoPopUI(LevelUpPackageModel.CanShowLevelUpPackage, new[] { UINameConst.UIPopupLevelUpPackage }),
                //相册
                new AutoPopUI(PhotoAlbumModel.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupPhotoAlbumPreview }),
                new AutoPopUI(PhotoAlbumModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait, UINameConst.UIPhotoAlbumShop }),
                new AutoPopUI(PhotoAlbumModel.CanShowStartPopup, new[] {  UINameConst.UIPhotoAlbumShop }),
                //蛇梯子
                new AutoPopUI(SnakeLadderModel.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UISnakeLadderLeaderBoardMain }),
                new AutoPopUI(SnakeLadderModel.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupSnakeLadderPreview }),
                new AutoPopUI(SnakeLadderModel.CanShowStartPopup, new[] { UINameConst.UIPopupSnakeLadderStart, UINameConst.UISnakeLadderMain }),
                //复活节2024
                new AutoPopUI(Easter2024Model.CanShowUnCollectRewardsUI, new[] { UINameConst.UIWaiting, UINameConst.UIEaster2024LeaderBoardMain }),
                new AutoPopUI(Easter2024Model.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupEaster2024Preview }),
                new AutoPopUI(Easter2024Model.CanShowStartPopup, new[] { UINameConst.UIPopupEaster2024Start, UINameConst.UIEaster2024Main }),

                //破冰礼包
                new AutoPopUI(UIPopupIcebreakingPackController.CanShowUILevelUp, new[] { UINameConst.UIPopupIcebreakingPack, UINameConst.UIPopupReward }),

                //每日礼包
                // new AutoPopUI(UIDailyPackController.CanShowUILevelUp, new[] {UINameConst.UIDailyPack, UINameConst.UIPopupReward}),

                // new AutoPopUI(UIPopupDailyGiftController.CanShowUILevelUp, new[] {UINameConst.UIPopupDailyGift,UINameConst.UIPopupReward}),
                new AutoPopUI(UIPopupNewDailyGiftController.CanShowUILevelUp, new[] { UINameConst.UIPopupNewDailyGift, UINameConst.UIPopupReward }),
                //卡牌引导
                new AutoPopUI(CardCollectionActivityModel.CanShowStart, CardUIName.CardUINames),
                // new AutoPopUI(CardCollectionReopenActivityModel.CanShowStart,CardUIName.CardReopenUINames),
                //BP1
                new AutoPopUI(Activity.BattlePass.BattlePassModel.Instance.CanShowRefresh, new[] { UINameConst.UIPopupBattlePassRefresh }),
                new AutoPopUI(Activity.BattlePass.BattlePassModel.Instance.CanShow, new[] { UINameConst.UIPopupBattlePassStart, UINameConst.UIBattlePassMain }),
                //BP2
                new AutoPopUI(Activity.BattlePass_2.BattlePassModel.Instance.CanShowRefresh, new[] { UINameConst.UIPopupBattlePass2Refresh }),
                new AutoPopUI(Activity.BattlePass_2.BattlePassModel.Instance.CanShow, new[] { UINameConst.UIPopupBattlePass2Start, UINameConst.UIBattlePass2Main }),
                //养鱼
                new AutoPopUI(FishCultureModel.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupFishCulturePreview }),
                new AutoPopUI(FishCultureModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait,UINameConst.UIFishCultureMain }),
                new AutoPopUI(FishCultureModel.CanShowStartPopup, new[] { UINameConst.UIFishCultureMain }),
                //猴子爬树
                new AutoPopUI(UIClimbTreeStartController.CanShowUI, new[] { UINameConst.UIClimbTreeStart, UINameConst.UIClimbTreeMain }),
                //小狗
                new AutoPopUI(UIDogMainController.CanShowUI, new[] { UINameConst.UIDogStart, UINameConst.UIDogMain }),
                //鹦鹉
                new AutoPopUI(ParrotModel.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupParrotPreview}),
                new AutoPopUI(ParrotModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait,UINameConst.UIParrotMain}),
                new AutoPopUI(ParrotModel.CanShowStartPopup, new[] { UINameConst.UIParrotStart,UINameConst.UIParrotMain}),
                //花田
                new AutoPopUI(FlowerFieldModel.CanShowPreheatPopupEachDay, new[] { UINameConst.UIPopupFlowerFieldPreview}),
                new AutoPopUI(FlowerFieldModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait,UINameConst.UIFlowerFieldMain}),
                // new AutoPopUI(FlowerFieldModel.CanShowStartPopup, new[] { UINameConst.UIFlowerFieldStart,UINameConst.UIFlowerFieldMain}),
                //丛林探险
                new AutoPopUI(UIJungleAdventureMainController.CanShow, new[] { UINameConst.UIJungleAdventureMain}),
                new AutoPopUI(UIPopupJungleAdventurePreviewController.CanShow, new[] { UINameConst.UIPopupJungleAdventurePreview }),
                new AutoPopUI(JungleAdventureModel.CanShow, new[] { UINameConst.UIJungleAdventureMain }),
                //金币挑战
                new AutoPopUI(CoinCompetitionModel.CanShowUI, new[] { UINameConst.UICoinCompetitionStart, UINameConst.UICoinCompetitionMain }),
                // 跳格子
                new AutoPopUI(JumpGridModel.CanShowUI, new[] { UINameConst.UIPopupJumpGridStart, UINameConst.UIJumpGridMain }),
                //海上竞速
                new AutoPopUI(SeaRacingModel.CanAutoShowStartPopup, new[] { UINameConst.UIPopupSeaRacingStart, UINameConst.UISeaRacingMain }),
                //金币rush
                new AutoPopUI(CoinRushModel.CanShowMainPopup, CoinRushModel.ShowMainUIList),
                //蝴蝶工坊
                new AutoPopUI(ButterflyWorkShopModel.CanShowUI, new[] { UINameConst.UIButterflyWorkShopMain, UINameConst.UIPopupButterflyWorkShopStart }),
                //调制大师
                new AutoPopUI(MixMasterModel.Instance.CanShowPreheatUI, new[] { UINameConst.UIPopupMixMasterPreview }),
                new AutoPopUI(MixMasterModel.Instance.CanShowUI, new[] { UINameConst.UIMixMasterMain }),
                //星空罗盘
                new AutoPopUI(StarrySkyCompassModel.Instance.CanShowPreheatUI, new[] { UINameConst.UIPopupStarrySkyCompassPreview }),
                new AutoPopUI(StarrySkyCompassModel.Instance.CanShowGuide, new[] { UINameConst.UIGuidePortrait }),
                //金币排行榜
                new AutoPopUI(CoinLeaderBoardModel.CanShowActivityStartUI, new[] { UINameConst.UICoinLeaderBoardStart, UINameConst.UICoinLeaderBoardMain }),
                //清理鱼塘
                new AutoPopUI(UIPopupGarageCleanupStartController.CanShowUI, new[] { UINameConst.UIPopupGarageCleanupStart, UINameConst.UIGarageCleanupMain }),
                //合成西瓜
                new AutoPopUI(SummerWatermelonModel.CanShowStartView, new[] { UINameConst.UIPopupSummerWatermelonStart, UINameConst.UIPopupSummerWatermelonMain }),
                //合成面包
                new AutoPopUI(SummerWatermelonBreadModel.CanShowStartView, new[] { UINameConst.UIPopupSummerWatermelonBreadStart, UINameConst.UIPopupSummerWatermelonBreadMain }),
                new AutoPopUI(ExtraEnergyModel.CanShowUI, new[] { UINameConst.UIPopupExtraEnergyStart }),
            };
        }
    }
}