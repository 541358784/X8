using System;
using System.Collections.Generic;
using Activity.CollectStone.View;
using Activity.FarmTimeOrder.FarmTimeLimitOrderController;
using Activity.JumpGrid;
using Activity.JungleAdventure.Controller;
using Activity.LimitTimeOrder.Controller;
using Activity.LuckyGoldenEgg;
using Activity.TimeOrder.Controller;
using ActivityLocal.CardCollection.Home;
using ActivityLocal.ClimbTower.Model;
using ActivityLocal.TipReward.UI;
using Ditch.Merge;
using Farm.View;
using Filthy.Game;
using Filthy.Game.Spine;
using Filthy.View;
using FishEatFishSpace;
using Game;
using Gameplay.UI.BindEmail;
using Gameplay.UI.MiniGame;
using Gameplay.UI.Setting;
using MiniGame.View;
using OnePath.View;
using Stimulate.Model.Merge;
using Stimulate.Model.Spine;

public partial class UIManager
{
    struct WindowInfo
    {
        public UIWindowType windowType;
        public UIWindowLayer windowLayer;
        public Type componentType;
        public bool addUIMask;
    }

    private Dictionary<string, WindowInfo> _windowsInfo = new Dictionary<string, WindowInfo>(64);

    private void allWindowMeta()
    {
        // basic
        _WindowMeta(UINameConst.UILogin, UIWindowLayer.Normal);


        //_WindowMeta(UINameConst.UIPopupSet, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupSet1, UIWindowLayer.Normal, true, typeof(UIPopupSetController));
        _WindowMeta(UINameConst.UIPopupSet2, UIWindowLayer.Normal, true, typeof(UIPopupSetOtherController));
        _WindowMeta(UINameConst.UIMainHome, UIWindowLayer.Under);
        _WindowMeta(UINameConst.UIPopupSetVip, UIWindowLayer.Normal, true, typeof(UIPopupSetVipController));
        
        _WindowMeta(UINameConst.UINotice, UIWindowLayer.Notice);
        _WindowMeta(UINameConst.UILocalNotice, UIWindowLayer.Notice);
        _WindowMeta(UINameConst.UINotice1, UIWindowLayer.Notice);
        _WindowMeta(UINameConst.UIWaiting, UIWindowLayer.Notice);
        
        _WindowMeta(UINameConst.UIStore, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIStoreGame, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupUnlockRoom, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPreview, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.ThemeGroup, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupDaily, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIGameGetReward, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIContactUs, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIGuidePortrait, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIMsgBox, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupRateUs, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupLikeUs, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupBox, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UISaveProgress, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupChooseProgress, UIWindowLayer.Max);
        _WindowMeta(UINameConst.UIPopupConfirmProgress, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.CloseRoomView, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupNoMoney, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupCompensate, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupDownLoad, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupDownLoadFail, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupDownLoadFinish, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupNewEvent, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupRVReward, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIDecorationMain, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UISpin, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIDailyBouns, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIWeeklyCard, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupBuyEnergy, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupExchangeEnergy, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupMail, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupMailList, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupIcebreakingPack, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupIcebreakingPackLow, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIDailyPack, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIDailyPack2, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIDailyRV, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UISKIPADS, UIWindowLayer.Normal);

        _WindowMeta(UINameConst.UIMainRewardItem, UIWindowLayer.Normal);

        _WindowMeta(UINameConst.UIPopupLanguage, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UICurrencyGroup, UIWindowLayer.Currency);

        _WindowMeta(UINameConst.MergeMain, UIWindowLayer.Under);
        _WindowMeta(UINameConst.HappyGoMain, UIWindowLayer.Under);

        _WindowMeta(UINameConst.UIPopupMergeInformation, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.MergeInformationTips, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupMergePackage, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupMergeWarning, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.MergeSplite, UIWindowLayer.Normal); 
        _WindowMeta(UINameConst.UIPopupMergeIncreaseLevel, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupMergeCopy, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupMergeInSplitLevel, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupMergeLevelTips, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupMergeLevelTipsShow, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupCompleteOrder, UIWindowLayer.Normal);

        _WindowMeta(UINameConst.UIPopupTask, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupTaskHelp, UIWindowLayer.Normal, true);
        
        _WindowMeta(UINameConst.UIPopupBuyCoin, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIBuyDiamond, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIBuyResources, UIWindowLayer.Normal, true);

        _WindowMeta(UINameConst.UIPopupReward, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupActivityUnCollectReward,UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupDailyChallenge, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.MergeBoardFull, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.MergeResource, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupMysteryGift, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupDiamondReward, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupLuckyBalloon, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupMergeInformationExplain, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupMergeInformationOneOutOfThree, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupMergeInformationOneOutOfFive, UIWindowLayer.Normal, true);
        
        _WindowMeta(UINameConst.DebugTmpPanel, UIWindowLayer.Normal);

        _WindowMeta(UINameConst.UIGiftBagLink, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIMasterCard, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupHappyGoReward, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupHappyGoStart, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.HappyGoUIStoreGam, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIHappyGoHelp, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupHappyGoGift1, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupHappyGoGift2, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupHappyGoExtend, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupHappyGoEnd, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupHappyGoyuilan, UIWindowLayer.Normal);

        _WindowMeta(UINameConst.UIPopupPigBox, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPigBoxHelp, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UITurntable, UIWindowLayer.Normal);
        
        _WindowMeta(UINameConst.UIStory, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupSetHead, UIWindowLayer.Normal, true);
     
        
        
        _WindowMeta(UINameConst.UIPopupLevelRankingStart, UIWindowLayer.Normal,true);
        _WindowMeta(UINameConst.UIPopupLevelRankingMain, UIWindowLayer.Normal,true);
        _WindowMeta(UINameConst.UIPopupLevelRankingShow, UIWindowLayer.Normal,true);  
        //_WindowMeta(UINameConst.UIPopupLevelRankingStartPreview, UIWindowLayer.Normal,true);         
        
        _WindowMeta(UINameConst.UIDogMain, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIDogStart, UIWindowLayer.Normal,true);
        _WindowMeta(UINameConst.UIDogStartPreview, UIWindowLayer.Normal,true);
        _WindowMeta(UINameConst.UIDogHopeLeaderBoardMain, UIWindowLayer.Normal);
        
        _WindowMeta(UINameConst.UIClimbTreeMain, UIWindowLayer.Normal);
        // _WindowMeta(UINameConst.UIClimbTreeUnSelect, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIClimbTreeStart, UIWindowLayer.Normal,true);
        // _WindowMeta(UINameConst.UIClimbTreeStartPreview, UIWindowLayer.Normal,true);
        _WindowMeta(UINameConst.UIClimbTreeLeaderBoardMain, UIWindowLayer.Normal);

        // _WindowMeta(UINameConst.UICoinRushMain, UIWindowLayer.Normal,true);
        // _WindowMeta(UINameConst.UICoinRushTaskCompleted, UIWindowLayer.Tips);
        
        _WindowMeta(UINameConst.UIPopupOneOutOfThree, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupOneOutOfFive, UIWindowLayer.Normal);
        
        _WindowMeta(UINameConst.UIPopupTaskCenter, UIWindowLayer.Normal,true);
        _WindowMeta(UINameConst.UIPopupDiamondLnsufficient, UIWindowLayer.Normal);
        
        _WindowMeta(UINameConst.UIPopupPayRebate, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupPayRebateLocal, UIWindowLayer.Normal);
        
        _WindowMeta(UINameConst.UIPopupGarageCleanupSubmit, UIWindowLayer.Normal,true);
        _WindowMeta(UINameConst.UIPopupGarageCleanupStart, UIWindowLayer.Normal,true);
        _WindowMeta(UINameConst.UIGarageCleanupMain, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupGarageCleanupHelp, UIWindowLayer.Normal,true);
        _WindowMeta(UINameConst.UIPopupGarageCleanupEnd, UIWindowLayer.Normal,true);
        
        _WindowMeta(UINameConst.UIPopupOrderReward, UIWindowLayer.Normal,true);
        
        _WindowMeta(UINameConst.UISealPack, UIWindowLayer.Normal,true);
        _WindowMeta(UINameConst.UIDolphinPack, UIWindowLayer.Normal,true);
        
        _WindowMeta(UINameConst.UIEasterHelp, UIWindowLayer.Normal,true);
        _WindowMeta(UINameConst.UIEasterMain, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIEasterEnd, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIEasterStart, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIEasterShop, UIWindowLayer.Normal,true);
        
        _WindowMeta(UINameConst.UIGameMain, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIEasterPack, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIGameFinish, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UILoadingTransition, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupGameTabulation, UIWindowLayer.Normal, false);
        
        _WindowMeta(UINameConst.UICommonTps, UIWindowLayer.Normal, true);
        
        _WindowMeta(UINameConst.UIPopupTaskNewDay, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupRewardItem, UIWindowLayer.Normal, true);
        
        _WindowMeta(UINameConst.UIPopupSyntheticChainUnlock, UIWindowLayer.Normal, false, typeof(Merge.UnlockMergeLine.UIPopupSyntheticChainUnlockController));
        
        _WindowMeta(UINameConst.UIBattlePassMain, UIWindowLayer.Normal, true, typeof(Activity.BattlePass.UIBattlePassMainController));
        _WindowMeta(UINameConst.UIBattlePassReward, UIWindowLayer.Normal, true, typeof(Activity.BattlePass.UIBattlePassRewardController));
        _WindowMeta(UINameConst.UIPopupBattlePassAddDay, UIWindowLayer.Normal, true, typeof(Activity.BattlePass.UIPopupBattlePassAddDayController));
        // _WindowMeta(UINameConst.UIPopupBattlePassBuy, UIWindowLayer.Normal, true, typeof(Activity.BattlePass.UIPopupBattlePassBuyController));
        _WindowMeta(UINameConst.UIPopupBattlePassRefresh, UIWindowLayer.Normal, true, typeof(Activity.BattlePass.UIPopupBattlePassRefreshController));
        _WindowMeta(UINameConst.UIPopupBattlePassStart, UIWindowLayer.Normal, componentType:typeof(Activity.BattlePass.UIPopupBattlePassStartController));
        _WindowMeta(UINameConst.UIPopupBattlePassEnd, UIWindowLayer.Normal, componentType:typeof(Activity.BattlePass.UIPopupBattlePassEndController));
        _WindowMeta(UINameConst.UIPopupBattlePassEndBuy, UIWindowLayer.Normal, componentType:typeof(Activity.BattlePass.UIPopupBattlePassEndBuyController));
        _WindowMeta(UINameConst.UIPopupBattlePassBuyNew1, UIWindowLayer.Normal, componentType:typeof(Activity.BattlePass.UIPopupBattlePassBuyNew));
        _WindowMeta(UINameConst.UIPopupBattlePassBuyNew2, UIWindowLayer.Normal, componentType:typeof(Activity.BattlePass.UIPopupBattlePassBuyNewUltimate));
        
              
        _WindowMeta(UINameConst.UIBattlePass2Main, UIWindowLayer.Normal, true, typeof(Activity.BattlePass_2.UIBattlePassMainController));
        _WindowMeta(UINameConst.UIBattlePass2Reward, UIWindowLayer.Normal, true, typeof(Activity.BattlePass_2.UIBattlePassRewardController));
        _WindowMeta(UINameConst.UIPopupBattlePass2AddDay, UIWindowLayer.Normal, true, typeof(Activity.BattlePass_2.UIPopupBattlePassAddDayController));
        //_WindowMeta(UINameConst.UIPopupBattlePass2Buy, UIWindowLayer.Normal, true, typeof(Activity.BattlePass_2.UIPopupBattlePassBuyController));
        _WindowMeta(UINameConst.UIPopupBattlePass2Refresh, UIWindowLayer.Normal, true, typeof(Activity.BattlePass_2.UIPopupBattlePassRefreshController));
        _WindowMeta(UINameConst.UIPopupBattlePass2Start, UIWindowLayer.Normal, componentType:typeof(Activity.BattlePass_2.UIPopupBattlePassStartController));
        _WindowMeta(UINameConst.UIPopupBattlePass2End, UIWindowLayer.Normal, componentType:typeof(Activity.BattlePass_2.UIPopupBattlePassEndController));
        _WindowMeta(UINameConst.UIPopupBattlePass2EndBuy, UIWindowLayer.Normal, componentType:typeof(Activity.BattlePass_2.UIPopupBattlePassEndBuyController));
        _WindowMeta(UINameConst.UIPopupBattlePass2BuyNew1, UIWindowLayer.Normal, componentType:typeof(Activity.BattlePass_2.UIPopupBattlePassBuyNew));
        _WindowMeta(UINameConst.UIPopupBattlePass2BuyNew2, UIWindowLayer.Normal, componentType:typeof(Activity.BattlePass_2.UIPopupBattlePassBuyNewUltimate));

        _WindowMeta(UINameConst.UIPopupThreeGift, UIWindowLayer.Normal, componentType:typeof(ThreeGift.UIPopupThreeGiftController));
        _WindowMeta(UINameConst.UIPopupMultipleGift, UIWindowLayer.Normal, componentType:typeof(MultipleGift.UIPopupThreeGiftController));
        
        _WindowMeta(UINameConst.UIPopupEnergyTorrentStart, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupEnergyTorrentMain, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupEnergyTorrentMainX8, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupThumbtack, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIEnergyTorrentTips, UIWindowLayer.Normal, false);
        
        _WindowMeta(UINameConst.MermaidHelp, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupMermaidMain, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupMermaidStart, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupMermaidStartPreview, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.UIPopupMermaidAddDay, UIWindowLayer.Normal, true);
        _WindowMeta(UINameConst.MermaidMapPreview, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.MermaidMapBuild, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.MermaidSlider, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UIPopupMermaidDouble, UIWindowLayer.Normal, true); 
        _WindowMeta(UINameConst.UIPopupEnergyBuy, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UIPopupHappyGoBuyEnergy, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UIPopupDeleteArchive1, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupDeleteArchive2, UIWindowLayer.Normal);
        // _WindowMeta(UINameConst.UIRecoverCoinMain, UIWindowLayer.Normal, false);
        // _WindowMeta(UINameConst.UIRecoverCoinStart, UIWindowLayer.Notice,false);
        // _WindowMeta(UINameConst.UIPopupRecoverCoinFinish, UIWindowLayer.Notice,false);
        // _WindowMeta(UINameConst.UIPopupRecoverCoinNewDecoArea, UIWindowLayer.Notice,false);
        // _WindowMeta(UINameConst.UIRecoverCoinBuy, UIWindowLayer.Normal, false);
        // _WindowMeta(UINameConst.UIRecoverCoinEnd, UIWindowLayer.Normal, false);
         
        _WindowMeta(UINameConst.UICoinCompetitionMain, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UICoinCompetitionStart, UIWindowLayer.Normal);
         
        _WindowMeta(UINameConst.UIPopupJumpGridStart, UIWindowLayer.Normal, componentType:typeof(UIPopupJumpGridStartController));
        _WindowMeta(UINameConst.UIJumpGridMain, UIWindowLayer.Normal, componentType:typeof(UIJumpGridMainController));
    
        _WindowMeta(UINameConst.UIPopupSummerWatermelonMain, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UIPopupSummerWatermelonStart, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UISummerWatermelonReward, UIWindowLayer.Tips, false);
        _WindowMeta(UINameConst.UIPopupSummerWatermelonGift, UIWindowLayer.Normal, false);
        
        _WindowMeta(UINameConst.UIPopupSummerWatermelonBreadMain, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UIPopupSummerWatermelonBreadStart, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UISummerWatermelonBreadReward, UIWindowLayer.Tips, false);
        _WindowMeta(UINameConst.UIPopupSummerWatermelonBreadGift, UIWindowLayer.Normal, false);


        _WindowMeta(UINameConst.UIPopupRecovery, UIWindowLayer.Normal, false);
        
        
        _WindowMeta(UINameConst.UIDigTrenchMain, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UIPopupGameChoose, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UIPopupGameTimeUp, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UIPopupGameTips, UIWindowLayer.Normal, false);
        
        #region CoinLeaderBoard
        _WindowMeta(UINameConst.UICoinLeaderBoardMain, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UICoinLeaderBoardStart, UIWindowLayer.Notice,false);
        _WindowMeta(UINameConst.UIPopupCoinLeaderBoardFinish, UIWindowLayer.Notice,false);
        _WindowMeta(UINameConst.UIPopupCoinLeaderBoardNewDecoArea, UIWindowLayer.Notice,false);
        _WindowMeta(UINameConst.UICoinLeaderBoardBuy, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UICoinLeaderBoardEnd, UIWindowLayer.Normal, false);
        #endregion

        #region FishEatFish
        _WindowMeta(UINameConst.UIFishEatFishMain, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UIFishEatFishFail, UIWindowLayer.Normal, componentType:typeof(UIFishEatFishFailController));
        #endregion
        
        #region SeaRacing
        _WindowMeta(UINameConst.UIPopupSeaRacingEnd, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UIPopupSeaRacingPreview, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UIPopupSeaRacingStart, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UISeaRacingMain, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UISeaRacingOpenBox, UIWindowLayer.Normal, false);
        _WindowMeta(UINameConst.UISeaRacingReward, UIWindowLayer.Normal, false);
        #endregion
        
        _WindowMeta(UINameConst.UIOnePathMain, UIWindowLayer.Tips, componentType:typeof(UIOnePathMainController));
        _WindowMeta(UINameConst.UIOnePathFail, UIWindowLayer.Tips, componentType:typeof(UIOnePathFailController));

        #region NewDailyPack
        _WindowMeta(UINameConst.UIPopupNewDailyGift, UIWindowLayer.Normal, componentType:typeof(UIPopupNewDailyGiftController));
        _WindowMeta(UINameConst.UINewDailyPack, UIWindowLayer.Normal, componentType:typeof(UINewDailyPackController));
        #endregion
        #region CardCollection
        
        _WindowMeta(UINameConst.UIMainCardGift, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIMainCard, UIWindowLayer.Normal, componentType:typeof(UICardController));
        
        #endregion

        
        _WindowMeta(UINameConst.UIConnectLineMain, UIWindowLayer.Normal, componentType:typeof(UIConnectLineMainController));
        _WindowMeta(UINameConst.UIPopupLimitedTimeTask, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupTimeOrderGift, UIWindowLayer.Normal, componentType:typeof(UIPopupTimeOrderGiftController));
        _WindowMeta(UINameConst.UIPopupLimitOrder, UIWindowLayer.Normal, UIWindowType.PopupTip);
        
        #region Easter2024
        _WindowMeta(UINameConst.UIEaster2024Main, UIWindowLayer.Normal, componentType:typeof(UIEaster2024MainController));
        // _WindowMeta(UINameConst.UIPopupEaster2024End, UIWindowLayer.Normal, componentType:typeof(UIPopupEaster2024EndController));
        _WindowMeta(UINameConst.UIPopupEaster2024Preview, UIWindowLayer.Normal, componentType:typeof(UIPopupEaster2024PreviewController));
        _WindowMeta(UINameConst.UIPopupEaster2024Start, UIWindowLayer.Normal, componentType:typeof(UIPopupEaster2024StartController));
        _WindowMeta(UINameConst.UIEaster2024MiniGameReward, UIWindowLayer.Normal, componentType:typeof(UIEaster2024MiniGameRewardController));
        _WindowMeta(UINameConst.UIPopupEaster2024MiniGame, UIWindowLayer.Normal, componentType:typeof(UIPopupEaster2024MiniGameController));
        _WindowMeta(UINameConst.UIEaster2024Shop, UIWindowLayer.Normal, componentType:typeof(UIEaster2024ShopController));
        _WindowMeta(UINameConst.UIEaster2024LeaderBoardMain, UIWindowLayer.Normal, componentType:typeof(UIEaster2024LeaderBoardMainController));
        _WindowMeta(UINameConst.UIEaster2024Help, UIWindowLayer.Normal, componentType:typeof(UIEaster2024HelpController));
        _WindowMeta(UINameConst.UIPopupEaster2024ShopBuy, UIWindowLayer.Normal, componentType:typeof(UIPopupEaster2024ShopBuyController),addUIMask:true);
        _WindowMeta(UINameConst.UIPopupEaster2024NoEgg, UIWindowLayer.Normal, componentType:typeof(UIPopupEaster2024NoEggController));

        #endregion
        _WindowMeta(UINameConst.UIPopupGiftBagBuyBetter, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupGiftBagSendOne, UIWindowLayer.Normal);
        
        #region SnakeLadder
        _WindowMeta(UINameConst.UISnakeLadderMain, UIWindowLayer.Normal, componentType:typeof(UISnakeLadderMainController));
        // _WindowMeta(UINameConst.UIPopupSnakeLadderEnd, UIWindowLayer.Normal, componentType:typeof(UIPopupSnakeLadderEndController));
        _WindowMeta(UINameConst.UIPopupSnakeLadderPreview, UIWindowLayer.Normal, componentType:typeof(UIPopupSnakeLadderPreviewController));
        _WindowMeta(UINameConst.UIPopupSnakeLadderStart, UIWindowLayer.Normal, componentType:typeof(UIPopupSnakeLadderStartController));
        _WindowMeta(UINameConst.UISnakeLadderShop, UIWindowLayer.Normal, componentType:typeof(UISnakeLadderShopController));
        _WindowMeta(UINameConst.UISnakeLadderLeaderBoardMain, UIWindowLayer.Normal, componentType:typeof(UISnakeLadderLeaderBoardMainController));
        _WindowMeta(UINameConst.UISnakeLadderHelp, UIWindowLayer.Normal, componentType:typeof(UISnakeLadderHelpController));
        _WindowMeta(UINameConst.UIPopupSnakeLadderShopBuy, UIWindowLayer.Normal, componentType:typeof(UIPopupSnakeLadderShopBuyController),addUIMask:true);
        _WindowMeta(UINameConst.UIPopupSnakeLadderNoTurntable, UIWindowLayer.Normal, componentType:typeof(UIPopupSnakeLadderNoTurntableController));
        _WindowMeta(UINameConst.UIPopupSnakeLadderUseCard, UIWindowLayer.Normal, componentType:typeof(UIPopupSnakeLadderUseCardController));
        #endregion
        _WindowMeta(UINameConst.UIExtraOrderRewardGet, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupShopExtraRewardStart, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupLevelUpPackage, UIWindowLayer.Normal);
        
        _WindowMeta(UINameConst.UIStimulateSpineMain, UIWindowLayer.Normal, componentType:typeof(UIStimulateSpineMainController));
        _WindowMeta(UINameConst.UIStimulateMergeMain, UIWindowLayer.Normal, componentType:typeof(UIStimulateMergeMainController));

        #region SlotMachine
        _WindowMeta(UINameConst.UIPopupSlotMachineStart, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupSlotMachineMain, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupSlotMachineNoSpin, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UISlotMachineHelp, UIWindowLayer.Normal);
        #endregion
        
        #region Monopoly
        _WindowMeta(UINameConst.UIMonopolyMain, UIWindowLayer.Normal, componentType:typeof(UIMonopolyMainController));
        // _WindowMeta(UINameConst.UIPopupMonopolyEnd, UIWindowLayer.Normal, componentType:typeof(UIPopupMonopolyEndController));
        _WindowMeta(UINameConst.UIPopupMonopolyPreview, UIWindowLayer.Normal, componentType:typeof(UIPopupMonopolyPreviewController));
        _WindowMeta(UINameConst.UIPopupMonopolyStart, UIWindowLayer.Normal, componentType:typeof(UIPopupMonopolyStartController));
        _WindowMeta(UINameConst.UIMonopolyShop, UIWindowLayer.Normal, componentType:typeof(UIMonopolyShopController));
        _WindowMeta(UINameConst.UIMonopolyLeaderBoardMain, UIWindowLayer.Normal, componentType:typeof(UIMonopolyLeaderBoardMainController));
        _WindowMeta(UINameConst.UIMonopolyHelp, UIWindowLayer.Normal, componentType:typeof(UIMonopolyHelpController));
        _WindowMeta(UINameConst.UIPopupMonopolyShopBuy, UIWindowLayer.Normal, componentType:typeof(UIPopupMonopolyShopBuyController),addUIMask:true);
        _WindowMeta(UINameConst.UIPopupMonopolyNoDice, UIWindowLayer.Normal, componentType:typeof(UIPopupMonopolyNoDiceController));
        _WindowMeta(UINameConst.UIPopupMonopolyUseCard, UIWindowLayer.Normal, componentType:typeof(UIPopupMonopolyUseCardController));
        _WindowMeta(UINameConst.UIMonopolyMiniGameReward, UIWindowLayer.Normal, componentType:typeof(UIMonopolyMiniGameRewardController));
        _WindowMeta(UINameConst.UIPopupMonopolyMiniGame, UIWindowLayer.Normal, componentType:typeof(UIPopupMonopolyMiniGameController));
        _WindowMeta(UINameConst.UIPopupMonopolyBuyBlock, UIWindowLayer.Normal, componentType:typeof(UIPopupMonopolyBuyBlockController));
        #endregion
        _WindowMeta(UINameConst.UIDecorationPreview, UIWindowLayer.Normal);
        
        _WindowMeta(UINameConst.UICrazeOrderMain, UIWindowLayer.Normal);
        
        #region KeepPet
        _WindowMeta(UINameConst.UIKeepPetMain, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupKeepPetBag, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupKeepPetPatrol, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupKeepPetPatrolReward, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupKeepPetTask, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupKeepPetTaskCompleted, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupKeepPetGiftNoDrumsticks, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupKeepPetGiftNoPower, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupKeepPetStart, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIKeepPetHelp, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupKeepPetClue, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupKeepPetGift, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupKeepPetThreeOneGift, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIKeepPetLevelUp, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupKeepPetReturn, UIWindowLayer.Normal);
        #endregion
        #region TreasureMap
        _WindowMeta(UINameConst.UIPopupTreasureMap, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UITreasureMapReward, UIWindowLayer.Normal);
        #endregion     
        
        #region TreasureHunt
        _WindowMeta(UINameConst.UITreasureHuntMain, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UITreasureHuntHelp, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupTreasureHuntGift, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupTreasureHuntStart, UIWindowLayer.Normal);
        _WindowMeta(UINameConst.UIPopupMonopolyNoHammer, UIWindowLayer.Normal);
        #endregion
        
        #region LuckyGoldenEgg
        _WindowMeta(UINameConst.UILuckyGoldenEggMain, UIWindowLayer.Normal, componentType:typeof(UILuckyGoldenEggMainController));
        _WindowMeta(UINameConst.UILuckyGoldenEggHelp, UIWindowLayer.Normal, componentType:typeof(UILuckyGoldenEggHelpController));
        _WindowMeta(UINameConst.UIPopupLuckyGoldenEggGift, UIWindowLayer.Normal, componentType:typeof(UIPopupLuckyGoldenEggGiftController));
        _WindowMeta(UINameConst.UIPopupLuckyGoldenEggStart, UIWindowLayer.Normal, componentType:typeof(UIPopupLuckyGoldenEggStartController));
        _WindowMeta(UINameConst.UIPopupLuckyGoldenEggNoItem, UIWindowLayer.Normal, componentType:typeof(UIPopupLuckyGoldenEggNoItemController));
        #endregion
        
        _WindowMeta(UINameConst.UIPopupMiniGame, UIWindowLayer.Normal, componentType:typeof(UIPopupMiniGameController));
        _WindowMeta(UINameConst.UIPsychologyMain, UIWindowLayer.Normal);
        
        _WindowMeta(UINameConst.UIPopupExtraEnergyStart, UIWindowLayer.Normal);
        
        #region GiftBagProgress
        _WindowMeta(UINameConst.UIPopupGiftBagProgressTask, UIWindowLayer.Normal, componentType:typeof(UIPopupGiftBagProgressTaskController));
        _WindowMeta(UINameConst.UIPopupGiftBagProgressTaskCompleted, UIWindowLayer.Normal,componentType:typeof(UIPopupGiftBagProgressTaskCompletedController));
        _WindowMeta(UINameConst.UIGiftBagProgressHelp, UIWindowLayer.Normal, componentType:typeof(UIGiftBagProgressHelpController));
       #endregion
       
       #region OptionalGift
       _WindowMeta(UINameConst.UIPopupOptionalGiftMain, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupOptionalGiftSelect, UIWindowLayer.Normal);
       #endregion
       
       _WindowMeta(UINameConst.UIPopupMatreshkas, UIWindowLayer.Normal, componentType:typeof(Activity.Matreshkas.View.UIPopupMatreshkasController));
        
       _WindowMeta(UINameConst.UIButterflyWorkShopMain, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupButterflyWorkShopStart, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupButterflyWorkShopEnd, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIButterflyWorkShopHelp, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupButterflyWorkShop, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupButterflyWorkTip, UIWindowLayer.Normal);

       _WindowMeta(UINameConst.UIPopupTurntableMain, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UITurntableHelp, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupTurntableNoTurntable, UIWindowLayer.Normal);
       
       _WindowMeta(UINameConst.UIDiamondRewardMain, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupDiamondRewardBuy, UIWindowLayer.Normal, componentType:typeof(BuyDiamondRewardController));
       
       _WindowMeta(UINameConst.UIPopupGiftBagDoubleMain, UIWindowLayer.Normal);
       
       _WindowMeta(UINameConst.UIGardenTreasureLeaderBoardMain, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupGardenTreasureStart, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIGardenTreasureMain, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIGardenTreasureHelp, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupGardenTreasureGift, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupGardenTreasureNoItem, UIWindowLayer.Normal);
       
       _WindowMeta(UINameConst.UIBlueBlockMain, UIWindowLayer.Normal);
       
       _WindowMeta(UINameConst.UISaveTheWhalesReward, UIWindowLayer.Normal);
       
       _WindowMeta(UINameConst.UIMixMasterMain, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIMixMasterUnlock, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupMixMasterTaskCompleted, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupMixMasterShop, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupMixMasterList, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIMixMasterMakeSuccess, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupMixMasterPreview, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIMixMasterMakeFailed, UIWindowLayer.Normal);
       
       _WindowMeta(UINameConst.UITurtlePangMain, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UITurtlePangShop, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupTurtlePangPreview, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupTurtlePangShopBuy, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UITurtlePangGiftBag, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupTurtlePangNoItem, UIWindowLayer.Normal);
       
       _WindowMeta(UINameConst.UIStarrySkyCompassMain, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIStarrySkyCompassShop, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupStarrySkyCompassPreview, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupStarrySkyCompassNoItem, UIWindowLayer.Normal);
       
       _WindowMeta(UINameConst.UIPopupBlindBoxRecycleTip, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIBlindBoxRecycle, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIBlindBoxMain, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIBlindBox1, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxTheme1));
       _WindowMeta(UINameConst.UIBlindBoxReward1, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxReward1));
       _WindowMeta(UINameConst.UIBlindBoxOpen1, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxOpenBox1));
       _WindowMeta(UINameConst.UIBlindBoxPreview1, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxPreview1));
       _WindowMeta(UINameConst.UIBlindBox2, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxTheme2));
       _WindowMeta(UINameConst.UIBlindBoxReward2, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxReward2));
       _WindowMeta(UINameConst.UIBlindBoxOpen2, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxOpenBox2));
       _WindowMeta(UINameConst.UIBlindBoxPreview2, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxPreview2));
       _WindowMeta(UINameConst.UIBlindBox3, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxTheme3));
       _WindowMeta(UINameConst.UIBlindBoxReward3, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxReward3));
       _WindowMeta(UINameConst.UIBlindBoxOpen3, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxOpenBox3));
       _WindowMeta(UINameConst.UIBlindBoxPreview3, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxPreview3));
       _WindowMeta(UINameConst.UIBlindBox4, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxTheme4));
       _WindowMeta(UINameConst.UIBlindBoxReward4, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxReward4));
       _WindowMeta(UINameConst.UIBlindBoxOpen4, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxOpenBox4));
       _WindowMeta(UINameConst.UIBlindBoxPreview4, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxPreview4));
       _WindowMeta(UINameConst.UIBlindBox5, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxTheme5));
       _WindowMeta(UINameConst.UIBlindBoxReward5, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxReward5));
       _WindowMeta(UINameConst.UIBlindBoxOpen5, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxOpenBox5));
       _WindowMeta(UINameConst.UIBlindBoxPreview5, UIWindowLayer.Normal,componentType:typeof(UIBlindBoxPreview5));
       
       // _WindowMeta(UINameConst.UINewDailyPackageExtraReward, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIBuyDiamondTicket, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupTotalRecharge, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupTotalRecharge_New, UIWindowLayer.Normal);
       
       _WindowMeta(UINameConst.UIZumaMain, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIZumaNoItem, UIWindowLayer.Normal,componentType:typeof(UIPopupZumaNoDiceController));
       _WindowMeta(UINameConst.UIPopupZumaPreview, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupZumaStart, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIZumaHelp, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIZumaBoardMain, UIWindowLayer.Normal,componentType:typeof(UIZumaLeaderBoardMainController));
       _WindowMeta(UINameConst.UIZumaShop, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupZumaShopBuy, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupZumaGift, UIWindowLayer.Normal,componentType:typeof(UIZumaGiftBagController));
       _WindowMeta(UINameConst.UIPopupZumaReward, UIWindowLayer.Normal,componentType:typeof(UIPopupRewardController));
       _WindowMeta(UINameConst.UIPopupZumaReward1, UIWindowLayer.Normal,componentType:typeof(UIPopupRewardController));
       
       _WindowMeta(UINameConst.UIPopupKeepPetTurkeyStart, UIWindowLayer.Normal,componentType:typeof(UIPopupKeepPetTurkeyStartController));
       _WindowMeta(UINameConst.UIKeepPetTurkeyShop, UIWindowLayer.Normal,componentType:typeof(UIKeepPetTurkeyShopController));
       _WindowMeta(UINameConst.UIPopupKeepPetTurkeyShopBuy, UIWindowLayer.Normal,componentType:typeof(UIPopupKeepPetTurkeyShopBuyController));
       
       _WindowMeta(UINameConst.UIPopupKapibalaPreview, UIWindowLayer.Normal,componentType:typeof(UIPopupKapibalaPreviewController));
       _WindowMeta(UINameConst.UIPopupKapibalaStart, UIWindowLayer.Normal,componentType:typeof(UIPopupKapibalaStartController));
       _WindowMeta(UINameConst.UIKapibalaMain, UIWindowLayer.Normal,componentType:typeof(UIKapibalaMainController));
       _WindowMeta(UINameConst.UIPopupKapibalaGiftBag, UIWindowLayer.Normal,componentType:typeof(UIPopupKapibalaGiftBagController));


       _WindowMeta(UINameConst.UIFilthySpineMain, UIWindowLayer.Normal, componentType: typeof(UISpineMainController));
       _WindowMeta(UINameConst.UIFilthyMergeMain, UIWindowLayer.Normal,componentType:typeof(UIFilthyMergeMainController));
       
       _WindowMeta(UINameConst.UIFarmMain, UIWindowLayer.Under,componentType:typeof(UIFarmMainController));
       _WindowMeta(UINameConst.UIPopupFarmBag, UIWindowLayer.Normal,componentType:typeof(UIPopupFarmBagController));
       _WindowMeta(UINameConst.FarmSellUI, UIWindowLayer.Normal,componentType:typeof(FarmSellUIController), addUIMask:true);
       _WindowMeta(UINameConst.UIPopupWarehouseUpgrade, UIWindowLayer.Normal,componentType:typeof(UIPopupWarehouseUpgradeController));
       _WindowMeta(UINameConst.UIPopupFarmLevelTipsShow, UIWindowLayer.Normal, componentType: typeof(UIPopupFarmLevelTipsShowController));
       _WindowMeta(UINameConst.UIPopupFarmNotice, UIWindowLayer.Normal,componentType:typeof(UIPopupFarmNoticeController));
       _WindowMeta(UINameConst.UIPopupFarmLevelTips, UIWindowLayer.Normal,componentType:typeof(UIPopupFarmLevelTipsController));
       _WindowMeta(UINameConst.UIFarmLoading, UIWindowLayer.Max,componentType:typeof(UIFarmLoadingController));
       _WindowMeta(UINameConst.UIPopupFarmItemInformation, UIWindowLayer.Max,componentType:typeof(UIPopupFarmItemInformationController));
       
       _WindowMeta(UINameConst.UIPopupChristmasBlindBox, UIWindowLayer.Normal,componentType:typeof(UIPopupChristmasBlindBoxController));
       _WindowMeta(UINameConst.UIChristmasBlindBoxOpen, UIWindowLayer.Normal,componentType:typeof(UIChristmasBlindBoxOpenController));

       _WindowMeta(UINameConst.UIPopupScrewGameSelect, UIWindowLayer.Normal,componentType:typeof(UIFilthyGameSelectController));
       _WindowMeta(UINameConst.UIPopupNewbiePack, UIWindowLayer.Normal,componentType:typeof(UIPopupNewbiePackController));
       
       
       _WindowMeta(UINameConst.UIPopupKapiScrewPreview, UIWindowLayer.Normal,componentType:typeof(UIPopupKapiScrewPreviewController));
       _WindowMeta(UINameConst.UIPopupKapiScrewStart, UIWindowLayer.Normal,componentType:typeof(UIPopupKapiScrewStartController));
       _WindowMeta(UINameConst.UIKapiScrewMain, UIWindowLayer.Normal,componentType:typeof(UIKapiScrewMainController));
       _WindowMeta(UINameConst.UIPopupKapiScrewShop, UIWindowLayer.Normal,componentType:typeof(UIPopupKapiScrewShopController));
       _WindowMeta(UINameConst.UIPopupKapiScrewOptionalGiftSelect, UIWindowLayer.Normal,componentType:typeof(UIPopupKapiScrewOptionalGiftSelectController));
       
       _WindowMeta(UINameConst.UIPopupDogPlay, UIWindowLayer.Normal,componentType:typeof(UIPopupDogPlayController));
       
       _WindowMeta(UINameConst.UIFishCultureBoardMain, UIWindowLayer.Normal,componentType:typeof(UIFishCultureLeaderBoardMainController));
       _WindowMeta(UINameConst.UIPopupFishCulturePreview, UIWindowLayer.Normal,componentType:typeof(UIPopupFishCulturePreviewController));
       _WindowMeta(UINameConst.UIFishCultureMain, UIWindowLayer.Normal,componentType:typeof(UIFishCultureMainController));
       _WindowMeta(UINameConst.UIPopupFishCultureNoItem, UIWindowLayer.Normal,componentType:typeof(UIPopupFishCultureNoDiceController));
       _WindowMeta(UINameConst.UIPopupFishCultureShopBuy, UIWindowLayer.Normal,componentType:typeof(UIPopupFishCultureShopBuyController));
       _WindowMeta(UINameConst.UIPopupFishExit, UIWindowLayer.Normal,componentType:typeof(UIPopupFishExitController));
       
       _WindowMeta(UINameConst.UIDitchMergeMain, UIWindowLayer.Normal,componentType:typeof(UIDitchMergeMainController));
       _WindowMeta(UINameConst.UIDigTrenchNewMain, UIWindowLayer.Normal,componentType:typeof(UIDigTrenchNewMainController));
       _WindowMeta(UINameConst.UIPopupDigTrenchNewGameTips, UIWindowLayer.Normal, componentType: typeof(UIPopupDigTrenchNewGameTipsController));

       _WindowMeta(UINameConst.UIDigTrenchNewStory, UIWindowLayer.Normal, componentType: typeof(UIDigTrenchNewStoryController));
       _WindowMeta(UINameConst.UIPopupDogPlayExtraReward, UIWindowLayer.Normal, componentType: typeof(UIPopupDogPlayExtraRewardController));

       _WindowMeta(UINameConst.UIPopupFarmTimeLimitOrder, UIWindowLayer.Normal, componentType: typeof(FarmTimeLimitOrderController));
       
       _WindowMeta(UINameConst.UIPopupKapiTilePreview, UIWindowLayer.Normal, componentType: typeof(UIPopupKapiTilePreviewController));
       _WindowMeta(UINameConst.UIKapiTileMain, UIWindowLayer.Normal, componentType: typeof(UIKapiTileMainController));
       _WindowMeta(UINameConst.UIPopupKapiTileGiftBag, UIWindowLayer.Normal, componentType: typeof(UIPopupKapiTileGiftBagController));
       #region TileMatch
       _WindowMeta(UINameConst.TileMatchMain, UIWindowLayer.Under);
       _WindowMeta(UINameConst.UIPopupTileMatchFail, UIWindowLayer.Under);
       _WindowMeta(UINameConst.UITileMatchSuccess, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupBuyItem, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupBuyHp, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupRemoveGuide, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIItemsUnlocked, UIWindowLayer.Normal);
        
       #endregion
       
       _WindowMeta(UINameConst.UIPhotoAlbumShop, UIWindowLayer.Normal, componentType: typeof(UIPhotoAlbumShopController));
       _WindowMeta(UINameConst.UIPopupPhotoAlbumExit, UIWindowLayer.Normal, componentType: typeof(UIPopupPhotoAlbumExitController));
       _WindowMeta(UINameConst.UIPopupPhotoAlbumPreview, UIWindowLayer.Normal, componentType: typeof(UIPopupPhotoAlbumPreviewController));
       _WindowMeta(UINameConst.UIPopupPhotoAlbumShopBuy, UIWindowLayer.Normal, componentType: typeof(UIPopupPhotoAlbumShopBuyController));
       _WindowMeta(UINameConst.UIJungleAdventureMain, UIWindowLayer.Normal, componentType: typeof(UIJungleAdventureMainController));
       _WindowMeta(UINameConst.UIPopupJungleAdventurePreview, UIWindowLayer.Normal, componentType: typeof(UIPopupJungleAdventurePreviewController));
       _WindowMeta(UINameConst.UIPopupJungleAdventureReward, UIWindowLayer.Normal, componentType: typeof(UIPopupJungleAdventureRewardController));
       
       _WindowMeta(UINameConst.UIPopupPhotoAlbumProgress, UIWindowLayer.Normal, componentType: typeof(UIPopupPhotoAlbumProgressController));
       _WindowMeta(UINameConst.UIPopupPhotoAlbumSpine, UIWindowLayer.Normal, componentType: typeof(UIPopupPhotoAlbumSpineController));
       _WindowMeta(UINameConst.UIJungleAdventureBoardMain, UIWindowLayer.Normal, componentType: typeof(UIJungleAdventureLeaderBoardMainController));
       
       _WindowMeta(UINameConst.UIPopupBindEmail, UIWindowLayer.Normal, componentType: typeof(UIPopupBindEmailController));
       _WindowMeta(UINameConst.UIPopupShopRv, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupLuckBubbleRv, UIWindowLayer.Normal);
       
       _WindowMeta(UINameConst.UIBiuBiuMain, UIWindowLayer.Normal,componentType: typeof(UIBiuBiuMainController));
       _WindowMeta(UINameConst.UIBiuBiuHelp, UIWindowLayer.Normal,componentType: typeof(UIBiuBiuHelpController));
       _WindowMeta(UINameConst.UIPopupBiuBiuPackage, UIWindowLayer.Normal,componentType: typeof(UIPopupBiuBiuPackageController));
       _WindowMeta(UINameConst.UIPopupBiuBiuTip, UIWindowLayer.Normal,componentType: typeof(UIPopupBiuBiuTipController));
       
       _WindowMeta(UINameConst.UIParrotStart, UIWindowLayer.Normal,componentType: typeof(UIParrotStartController));
       _WindowMeta(UINameConst.UIParrotMain, UIWindowLayer.Normal,componentType: typeof(UIParrotMainController));
       _WindowMeta(UINameConst.UIPopupParrotPreview, UIWindowLayer.Normal,componentType: typeof(UIPopupParrotPreviewController));
       _WindowMeta(UINameConst.UIParrotLeaderBoardMain, UIWindowLayer.Normal,componentType: typeof(UIParrotLeaderBoardMainController));
       _WindowMeta(UINameConst.UIParrotHelp, UIWindowLayer.Normal,componentType: typeof(UIParrotHelpController));
       
       
       _WindowMeta(UINameConst.UIBalloonRacingStart, UIWindowLayer.Normal, UIWindowType.PopupTip,componentType:typeof(Activity.BalloonRacing.UI.UIPopupSpeedRaceStartController));
       _WindowMeta(UINameConst.UIBalloonRacingMain, UIWindowLayer.Normal, UIWindowType.PopupTip,componentType:typeof(Activity.BalloonRacing.UI.UISpeedRaceMainController));
       _WindowMeta(UINameConst.UIBalloonRacingFail, UIWindowLayer.Normal, UIWindowType.PopupTip,componentType:typeof(Activity.BalloonRacing.UI.UIPopupSpeedRaceFailController));
       _WindowMeta(UINameConst.UIBalloonRacingReward, UIWindowLayer.Normal, UIWindowType.PopupTip,componentType:typeof(Activity.BalloonRacing.UI.UIBalloonRacingRewardController));
       _WindowMeta(UINameConst.UIBalloonRacingOpenBox, UIWindowLayer.Normal, UIWindowType.Normal,componentType:typeof(Activity.BalloonRacing.UI.UIBalloonRacingOpenBoxController));
       
       
       _WindowMeta(UINameConst.UIRabbitRacingMain, UIWindowLayer.Normal, UIWindowType.PopupTip, componentType:typeof(Activity.RabbitRacing.UI.UISpeedRaceMainController));
       _WindowMeta(UINameConst.UIPopupRabbitRacingStart, UIWindowLayer.Normal, UIWindowType.PopupTip,componentType:typeof(Activity.RabbitRacing.UI.UIPopupSpeedRaceStartController));
       _WindowMeta(UINameConst.UIPopupRabbitRacingFail, UIWindowLayer.Normal, UIWindowType.PopupTip,componentType:typeof(Activity.RabbitRacing.UI.UIPopupSpeedRaceFailController));
       _WindowMeta(UINameConst.UIRabbitRacingOpenBox, UIWindowLayer.Normal, UIWindowType.Normal,componentType:typeof(Activity.RabbitRacing.UI.UIRabbitRacingOpenBoxController));
       _WindowMeta(UINameConst.UIRabbitRacingReward, UIWindowLayer.Normal, UIWindowType.PopupTip,componentType:typeof(Activity.RabbitRacing.UI.UIRabbitRacingRewardController));
    
    
       _WindowMeta(UINameConst.UICommonNormalBox, UIWindowLayer.Normal);
       
       _WindowMeta(UINameConst.UIPopupNewNewIceBreakPack, UIWindowLayer.Normal);
       _WindowMeta(UINameConst.UIPopupNewNewIceBreakPackFinish, UIWindowLayer.Normal);
       
       _WindowMeta(UINameConst.UIFlowerFieldStart, UIWindowLayer.Normal,componentType: typeof(UIFlowerFieldStartController));
       _WindowMeta(UINameConst.UIFlowerFieldMain, UIWindowLayer.Normal,componentType: typeof(UIFlowerFieldMainController));
       _WindowMeta(UINameConst.UIPopupFlowerFieldPreview, UIWindowLayer.Normal,componentType: typeof(UIPopupFlowerFieldPreviewController));
       _WindowMeta(UINameConst.UIFlowerFieldLeaderBoardMain, UIWindowLayer.Normal,componentType: typeof(UIFlowerFieldLeaderBoardMainController));
       _WindowMeta(UINameConst.UIFlowerFieldHelp, UIWindowLayer.Normal,componentType: typeof(UIFlowerFieldHelpController));
       
       _WindowMeta(UINameConst.UIClimbTowerMain, UIWindowLayer.Normal,componentType: typeof(UIClimbTowerMainController));
       _WindowMeta(UINameConst.UIPopupClimbTowerQuit, UIWindowLayer.Normal,componentType: typeof(UIPopupClimbTowerQuitController));
       _WindowMeta(UINameConst.UIPopupClimbTowerPay, UIWindowLayer.Normal,componentType: typeof(UIPopupClimbTowerPayController));
       _WindowMeta(UINameConst.UIClimbTowerMainPay, UIWindowLayer.Normal,componentType: typeof(UIClimbTowerMainController));
       _WindowMeta(UINameConst.UIPopupClimbTowerCollect, UIWindowLayer.Normal,componentType: typeof(UIPopupClimbTowerQuitController));
       
       _WindowMeta(UINameConst.UIPopupGiftBagSendTwo, UIWindowLayer.Normal,componentType: typeof(UIPopupGiftBagSendTwoController));
       _WindowMeta(UINameConst.UIPopupGiftBagSendThree, UIWindowLayer.Normal,componentType: typeof(UIPopupGiftBagSendThreeController));
       _WindowMeta(UINameConst.UIPopupGiftBagSend4, UIWindowLayer.Normal,componentType: typeof(UIPopupGiftBagSend4Controller));
       _WindowMeta(UINameConst.UIPopupGiftBagSend6, UIWindowLayer.Normal,componentType: typeof(UIPopupGiftBagSend6Controller));
       
       _WindowMeta(UINameConst.UIPopupTipRewardMain, UIWindowLayer.Normal, UIWindowType.PopupTip,componentType:typeof(UIPopupTipRewardMainController));
       
       _WindowMeta(UINameConst.UIPopupCollectStoneMain, UIWindowLayer.Normal, componentType:typeof(UIPopupCollectStoneMainController));
       
       _WindowMeta(UINameConst.UIPopupGuildJoin, UIWindowLayer.Normal, componentType:typeof(UIPopupGuildJoinController));
       _WindowMeta(UINameConst.UIPopupGuildSetIcon, UIWindowLayer.Normal, componentType:typeof(UIPopupGuildSetIconController));
       _WindowMeta(UINameConst.UIPopupGuildJoinPreview, UIWindowLayer.Normal, componentType:typeof(UIPopupGuildJoinPreviewController));
       _WindowMeta(UINameConst.UIPopupGuildMain, UIWindowLayer.Normal, componentType:typeof(UIPopupGuildMainController));
       _WindowMeta(UINameConst.UIPopupGuildSet, UIWindowLayer.Normal, componentType:typeof(UIPopupGuildSetController));
       _WindowMeta(UINameConst.UIPopupGuildCardGet, UIWindowLayer.Normal, componentType:typeof(UIPopupGuildCardGetController));
       _WindowMeta(UINameConst.UIPopupGuildCardOpen, UIWindowLayer.Normal, componentType:typeof(UIPopupGuildCardOpenController));
       _WindowMeta(UINameConst.UIPopupTeamMemberInfo, UIWindowLayer.Normal, componentType:typeof(UIPopupTeamMemberInfoController));
       _WindowMeta(UINameConst.UIPopupGuildExitTip, UIWindowLayer.Normal, componentType:typeof(UIPopupGuildExitTipController));
       _WindowMeta(UINameConst.UIPopupGuildLevelUp, UIWindowLayer.Normal, componentType:typeof(UIPopupGuildLevelUpController));
       _WindowMeta(UINameConst.UIPopupGuildJoinTip, UIWindowLayer.Normal, componentType:typeof(UIPopupGuildJoinTipController));
       _WindowMeta(UINameConst.UIPopupGuildKickTip, UIWindowLayer.Normal, componentType:typeof(UIPopupGuildKickTipController));
       _WindowMeta(UINameConst.UIPopupGuildDisbandTip, UIWindowLayer.Normal, componentType:typeof(UIPopupGuildDisbandTipController));
       
       _WindowMeta(UINameConst.UIPopupLimitOrderGift, UIWindowLayer.Normal, componentType:typeof(UIPopupLimitOrderGiftController));
       
       
       
       _WindowMeta(UINameConst.UIMiniGameLoading, UIWindowLayer.Max,false, componentType:typeof(UIUIMiniGameLoadingController));   
       
       _WindowMeta(UINameConst.UITrainOrderMain, UIWindowLayer.Normal, componentType:typeof(Activity.TrainOrder.UITrainOrderMainController));
       _WindowMeta(UINameConst.UIPopupTrainOrderStart, UIWindowLayer.Normal, componentType:typeof(Activity.TrainOrder.UIPopupTrainOrderStartController));
       
       
       _WindowMeta(UINameConst.UIPillowWheelMain, UIWindowLayer.Normal, componentType:typeof(UIPillowWheelMainController));
       _WindowMeta(UINameConst.UIPopupPillowWheelPreview, UIWindowLayer.Normal, componentType:typeof(UIPopupPillowWheelPreviewController));
       _WindowMeta(UINameConst.UIPillowWheelLeaderBoardMain, UIWindowLayer.Normal, componentType:typeof(UIPillowWheelLeaderBoardMainController));
       _WindowMeta(UINameConst.UIPopupPillowWheelGift, UIWindowLayer.Normal, componentType:typeof(UIPopupPillowWheelGiftController));
       _WindowMeta(UINameConst.UIPillowWheelHelp, UIWindowLayer.Normal, componentType:typeof(UIPillowWheelHelpController));
       _WindowMeta(UINameConst.UIPopupPillowWheelNoItem, UIWindowLayer.Normal, componentType:typeof(UIPopupPillowWheelNoItemController));
       
       _WindowMeta(UINameConst.UIPopupNoADS, UIWindowLayer.Normal, componentType:typeof(UIPopupNoADSController));
       
       _WindowMeta(UINameConst.UIEndlessEnergyGiftBag, UIWindowLayer.Normal, componentType:typeof(UIEndlessEnergyGiftBagController));
       
       _WindowMeta(UINameConst.UICatchFishMain, UIWindowLayer.Normal, componentType:typeof(UICatchFishMainController));
       _WindowMeta(UINameConst.UIPopupCatchFishPreview, UIWindowLayer.Normal, componentType:typeof(UIPopupCatchFishPreviewController));
    }


    private void _WindowMeta(string name, UIWindowLayer windowLayer, UIWindowType windowType = UIWindowType.Normal,
        bool addUIMask = false, Type componentType = null)
    {
        _windowsInfo.Add(name,
            new WindowInfo {windowType = windowType, windowLayer = windowLayer, addUIMask = addUIMask, componentType = componentType});
    }

    private void _WindowMeta(string name, UIWindowLayer windowLayer, bool addUIMask, Type componentType = null)
    {
        _windowsInfo.Add(name,
            new WindowInfo {windowType = UIWindowType.Normal, windowLayer = windowLayer, addUIMask = addUIMask, componentType = componentType});
    }
    public void _WindowMetaPublic(string name, UIWindowLayer windowLayer, bool addUIMask)
    {
        if (_windowsInfo.ContainsKey(name))
            return;
        _windowsInfo.Add(name,
            new WindowInfo {windowType = UIWindowType.Normal, windowLayer = windowLayer, addUIMask = addUIMask});
    }
}

public static class UIAnimationConst
{
    public static readonly string Appear = "appear";
    public static readonly string DisAppear = "disappear";
}

public static partial class UINameConst
{
    public static readonly string UILogin = "Home/UILogin";
    public static readonly string LoadingTransition = "Home/LoadingTransition";
    public static readonly string UISaveProgress = "Common/UIPopupSaveProgress";
    public static readonly string UINotice = "Common/UIPopupNotice";
    public static readonly string UILocalNotice = "UIPopupNotice";
    public static readonly string UINotice1 = "Common/UIPopupNotice1";
    public static readonly string UIMsgBox = "Common/UIPopupMsgBox";
    public static readonly string UIPopupChooseProgress = "Common/UIPopupChooseProgress";
    public static readonly string UIPopupConfirmProgress = "Common/UIPopupConfirmProgress";

    //public static readonly string UIPopupSet = "Home/UIPopupSet";
    public static readonly string UIPopupSet1 = "Home/UIPopupSet1";
    public static readonly string UIPopupSet2 = "Home/UIPopupSet2";
    public static readonly string UIPopupSetVip = "Home/UIPopupSetVip";
    
    public static readonly string UIGameGetReward = "UIGame/UIGameGetReward";

    public static readonly string UIMainHome = "Home/UIHomeMain";
    public static readonly string UIPopupUnlockRoom = "Home/UIUnlockRoom";
    public static readonly string UIMainGroup = "Home/UIMainGroup";
    public static readonly string UIPreview = "Home/UIPreview";
    public static readonly string ThemeGroup = "Home/ThemeGroup";
    public static readonly string UIPopupDaily = "Home/UIPopupDaily";
    public static readonly string UIPopupDiamondReward = "Common/UIPopupDiamondReward";
    public static readonly string UIPopupLuckyBalloon = "Common/UIPopupLuckyBalloon";
    public static readonly string UIContactUs = "Home/UIPopupContactUs";
    public static readonly string UIPopupLanguage = "Home/UIPopupLanguage";
    public static readonly string UIGuidePortrait = "Common/UIGuidePortrait";
    public static readonly string UIPopupRateUs = "Home/UIPopupRateUs";
    public static readonly string UIPopupLikeUs = "Home/UIPopupLikeUs";
    public static readonly string UIPopupBox = "Home/UIPopupBox";
    public static readonly string CloseRoomView = "Home/CloseRoomView";
    public static readonly string UIPopupNoMoney = "Home/UIPopupNoMoney";
    public static readonly string UIPopupDownLoad = "Home/UIPopupDownLoad";
    public static readonly string UIPopupDownLoadFail = "Home/UIPopupDownLoadFail";
    public static readonly string UIPopupDownLoadFinish = "Home/UIPopupDownLoadFinish";
    public static readonly string UIPopupNewEvent = "Home/UIPopupNewEvent";
    public static readonly string UIPopupRVReward = "Home/UIPopupRVReward";
    public static readonly string UISpin = "Home/UISpin";
    public static readonly string UIDailyBouns = "Home/UIDailyBouns";
    public static readonly string UIWeeklyCard = "Home/UIWeeklyCard";
    public static readonly string UIPopupMail = "Home/UIPopupMail";
    public static readonly string UIPopupMailList = "Home/UIPopupMailList";
    public static readonly string UIStore = "Shop/UIStore";
    public static readonly string UIStoreGame = "Shop/UIStoreGame";

    public static readonly string UIWaiting = "Common/Waiting";

    public static readonly string UIPopupCompensate = "Home/UIPopupCompensate";
    public static readonly string UIPopupBuyEnergy = "Home/UIPopupBuyEnergy";
    public static readonly string UIPopupExchangeEnergy = "Home/UIPopupExchangeEnergy";

    public static readonly string UIDecorationMain = "Home/UIDecorationMain";

    public static readonly string UIDailyRV = "Home/UIDailyRV";
    public static readonly string UIPopupIcebreakingPack = "Home/UIPopupIcebreakingPack";
    public static readonly string UIPopupIcebreakingPackLow = "Home/UIPopupIcebreakingPackLow";
    public static readonly string UIDailyPack = "Home/UIDailyPack";
    public static readonly string UIDailyPack2 = "Home/UIDailyPack2";
    public static readonly string UIPopupBuyCoin = "Home/UIPopupBuyCoin";
    public static readonly string UIBuyDiamond = "Home/UIBuyDiamond";
    public static readonly string UIPopupMysteryGift = "Common/UIPopupMysteryGift";
    public static readonly string UIBuyResources = "Home/UIBuyResources";
    public static readonly string UIMasterCard = "Home/UIMasterCard";


    #region 去广告IAP

    public const string UISKIPADS = "Home/UISkipAds";

    #endregion

    public static readonly string UIMainRewardItem = "Home/UIMainRewardItem";
    public static readonly string UICurrencyGroup = "Common/UICurrencyGroup";

    //---------------------------Merge----------------------------------------
    public static readonly string MergeMain = "Merge/MergeMain";
    public static readonly string UIPopupMergeInformation = "Merge/UIPopupMergeInformation";
    public static readonly string UIPopupMergeInformationExplain = "Merge/UIPopupMergeInformationExplain";
    public static readonly string UIPopupMergeInformationOneOutOfThree = "Merge/UIPopupMergeInformationOneOutOfThree";
    public static readonly string UIPopupMergeInformationOneOutOfFive = "Merge/UIPopupMergeInformationOneOutOfFive";
    
    public static readonly string MergeInformationTips = "Merge/MergeInformationTips";
    public static readonly string UIPopupMergePackage = "Merge/UIPopupMergePackage";
    public static readonly string UIPopupMergeWarning = "Merge/UIPopupMergeWarning";
    public static readonly string MergeSplite = "Merge/MergeSplite";
    public static readonly string UIPopupMergeLevelTips = "Merge/UIPopupMergeLevelTips";
    public static readonly string UIPopupMergeLevelTipsShow = "Merge/UIPopupMergeLevelTipsShow";
    
    public static readonly string UIPopupMergeCopy = "Merge/UIPopupMergeCopy";
    public static readonly string UIPopupMergeIncreaseLevel = "Merge/UIPopupMergeIncreaseLevel";
    public static readonly string UIPopupMergeInSplitLevel = "Merge/UIPopupMergeInSplitLevel";
    public static readonly string UIPopupCompleteOrder = "Merge/UIPopupCompleteOrder";


    public static readonly string UIPopupTask = "Home/UIPopupTask";
    public static readonly string UIPopupTaskHelp = "Home/UIPopupTaskHelp";
    
    public static readonly string UIPopupReward = "Home/UIPopupReward";
    public static readonly string UIPopupActivityUnCollectReward = "Home/UIPopupActivityUnCollectReward";
    public static readonly string UIPopupDailyChallenge = "Home/UIPopupDailyChallenge";

    public static readonly string MergeBoardFull = "Merge/MergeBoardFull";
    public static readonly string MergeResource = "Merge/MergeResource";
    public static readonly string DebugTmpPanel = "Common/DebugTmpPanel";

    public static readonly string UIGiftBagLink = "Activity/GiftBagLink/UIGiftBagLink";
    
    public static readonly string UIPopupHappyGoReward = "Activity/HappyGo/UIPopupHappyGoReward";
    public static readonly string UIPopupHappyGoStart = "Activity/HappyGo/UIPopupHappyGoStart";
    public static readonly string UIHappyGoHelp = "Activity/HappyGo/UIHappyGoHelp";
    public static readonly string HappyGoMain = "Activity/HappyGo/HappyGoMain";
    public static readonly string UIPopupHappyGoGift1 = "Activity/HappyGo/UIPopupHappyGoGift1";
    public static readonly string UIPopupHappyGoGift2 = "Activity/HappyGo/UIPopupHappyGoGift2";
    public static readonly string HappyGoUIStoreGam = "Activity/HappyGo/HappyGoUIStoreGame";
    public static readonly string UIPopupHappyGoBuyEnergy = "Activity/HappyGo/UIPopupHappyGoBuyEnergy";
    public static readonly string UIPopupHappyGoExtend= "Activity/HappyGo/UIPopupHappyGoExtend";
    public static readonly string UIPopupHappyGoEnd= "Activity/HappyGo/UIPopupHappyGoEnd";
    public static readonly string UIPopupHappyGoyuilan= "Activity/HappyGo/UIPopupHappyGoyuilan";
    
    public static readonly string UIPopupPigBox = "Activity/PigBox/UIPopupPigBox";
    public static readonly string UIPigBoxHelp = "Activity/PigBox/UIPigBoxHelp";

    public static readonly string UITurntable = "Home/UITurntable";
    
    public static readonly string UIStory = "UIStory/UIStory";
    
    public static readonly string UIPopupSetHead = "Home/UIPopupSetHead";
    
    
    
    public static readonly string UIPopupLevelRankingStart = "Activity/LevelRanking/UIPopupLevelRankingStart";
    public static readonly string UIPopupLevelRankingMain = "Activity/LevelRanking/UIPopupLevelRankingMain";
    public static readonly string UIPopupLevelRankingShow = "Activity/LevelRanking/UIPopupLevelRankingShow";   
    //public static readonly string UIPopupLevelRankingStartPreview = "Home/UIPopupLevelRankingStartPreview";   
        
    public static readonly string UIDogStart = "Activity/Dog/UIDogStart";
    public static readonly string UIDogMain = "Activity/Dog/UIDogMain";
    public static readonly string UIDogStartPreview = "Activity/Dog/UIDogStartPreview";
    public static readonly string UIDogHopeLeaderBoardMain = "Activity/Dog/UIDogHopeLeaderBoardMain";//排行榜界面
    
    public static readonly string UIClimbTreeStart = "Activity/ClimbTree/UIClimbTreeStart";
    public static readonly string UIClimbTreeMain = "Activity/ClimbTree/UIClimbTreeMain";
    // public static readonly string UIClimbTreeUnSelect = "Activity/ClimbTree/UIPopupClimbTreeEnd";
    // public static readonly string UIClimbTreeStartPreview = "Activity/ClimbTree/UIClimbTreeStartPreview";
    public static readonly string UIClimbTreeLeaderBoardMain = "Activity/ClimbTree/UIClimbTreeLeaderBoardMain";//排行榜界面

    public static readonly string UICoinRushMain = "Activity/CoinRush/UICoinRushMain";
    public static readonly string UICoinRushTaskCompleted = "Activity/CoinRush/UICoinRushTaskCompleted";
    
    public static readonly string UIPopupOneOutOfThree = "Merge/UIPopupOneOutOfThree";
    public static readonly string UIPopupOneOutOfFive = "Merge/UIPopupOneOutOfFive";
    
    public static readonly string UIPopupTaskCenter = "Home/UIPopupTaskCenter";
    
    public static readonly string UIPopupDiamondLnsufficient = "Home/UIPopupDiamondLnsufficient";
    
    public static readonly string UIPopupPayRebate = "Activity/PayRebate/UIPopupPayRebate";
    public static readonly string UIPopupPayRebateLocal = "Home/UIPopupPayRebateLocal";
    
    public static readonly string UIPopupGarageCleanupSubmit = "Activity/GarageCleanup/UIPopupGarageCleanupSubmit";
    public static readonly string UIPopupGarageCleanupStart = "Activity/GarageCleanup/UIPopupGarageCleanupStart";
    public static readonly string UIGarageCleanupMain = "Activity/GarageCleanup/UIGarageCleanupMain";
    public static readonly string UIPopupGarageCleanupHelp = "Activity/GarageCleanup/UIPopupGarageCleanupHelp";
    public static readonly string UIPopupGarageCleanupEnd = "Activity/GarageCleanup/UIPopupGarageCleanupEnd";
    
    public static readonly string UIPopupOrderReward = "Merge/UIPopupOrderReward";
    
    public static readonly string UISealPack = "Home/UISealPack";
    public static readonly string UIDolphinPack = "Home/UIDolphinPack";
    
    public static readonly string UIEasterHelp = "Activity/Easter/UIEasterHelp";
    public static readonly string UIEasterMain = "Activity/Easter/UIEasterMain";
    public static readonly string UIEasterEnd = "Activity/Easter/UIEasterEnd";
    public static readonly string UIEasterStart = "Activity/Easter/UIEasterStart";
    public static readonly string UIEasterShop = "Activity/Easter/UIEasterShop";

    public static readonly string UIGameMain = "UIMakeover/UIGameMain";
    public static readonly string UIPopupGameSelect = "UIMakeover/UIPopupGameSelect";
    public static readonly string UIPopupGameTabulation = "Home/UIPopupGameTabulation";
    public static readonly string UIGameFinish = "UIMakeover/UIGameFinish";
    
        
    public static readonly string UIEasterPack = "Activity/EasterPack/UIEasterPack";

    public static readonly string UICommonTps = "Common/UICommonTps";
    
    public static readonly string UIPopupTaskNewDay = "Home/UIPopupTaskNewDay";
    
    public static readonly string UIPopupRewardItem = "Home/UIPopupRewardItem";
    
    public static readonly string UIBattlePassMain = "Activity/BattlePass/UIBattlePassMain";
    public static readonly string UIBattlePassReward = "Activity/BattlePass/UIBattlePassReward";
    public static readonly string UIPopupBattlePassAddDay = "Activity/BattlePass/UIPopupBattlePassAddDay";
    //public static readonly string UIPopupBattlePassBuy = "Activity/BattlePass/UIPopupBattlePassBuy";
    public static readonly string UIPopupBattlePassBuyNew1 = "Activity/BattlePass/UIPopupBattlePassBuyNew1";
    public static readonly string UIPopupBattlePassBuyNew2 = "Activity/BattlePass/UIPopupBattlePassBuyNew2";
    public static readonly string UIPopupBattlePassRefresh = "Activity/BattlePass/UIPopupBattlePassRefresh";
    public static readonly string UIPopupBattlePassStart = "Activity/BattlePass/UIPopupBattlePassStart";
    public static readonly string UIPopupBattlePassEnd = "Activity/BattlePass/UIPopupBattlePassEnd";
    public static readonly string UIPopupBattlePassEndBuy = "Activity/BattlePass/UIPopupBattlePassEndBuy";

    
    
    public static readonly string UIBattlePass2Main = "Activity/BPTwo/UIBattlePassMain";
    public static readonly string UIBattlePass2Reward = "Activity/BPTwo/UIBattlePassReward";
    public static readonly string UIPopupBattlePass2AddDay = "Activity/BPTwo/UIPopupBattlePassAddDay";
    //public static readonly string UIPopupBattlePass2Buy = "Activity/BPTwo/UIPopupBattlePassBuy";
    public static readonly string UIPopupBattlePass2Refresh = "Activity/BPTwo/UIPopupBattlePassRefresh";
    public static readonly string UIPopupBattlePass2Start = "Activity/BPTwo/UIPopupBattlePassStart";
    public static readonly string UIPopupBattlePass2End = "Activity/BPTwo/UIPopupBattlePassEnd";
    public static readonly string UIPopupBattlePass2EndBuy = "Activity/BPTwo/UIPopupBattlePassEndBuy";
    public static readonly string UIPopupBattlePass2BuyNew1 = "Activity/BPTwo/UIPopupBattlePassBuyNew1";
    public static readonly string UIPopupBattlePass2BuyNew2 = "Activity/BPTwo/UIPopupBattlePassBuyNew2";
    
    
    public static readonly string UIPopupThreeGift = "Activity/ThreeGift/UIPopupThreeGift";
    public static readonly string UIPopupMultipleGift = "Activity/MultipleGift/UIPopupThreeGift";
    
    public static readonly string UIPopupEnergyTorrentStart = "Merge/UIPopupEnergyTorrentStart";
    public static readonly string UIEnergyTorrentTips = "Merge/UIEnergyTorrentTips";
    public static readonly string UIPopupEnergyTorrentMain = "Merge/UIPopupEnergyTorrentMain";
    public static readonly string UIPopupEnergyTorrentMainX8 = "Merge/UIPopupEnergyTorrentMainX8";
    public static readonly string UIPopupThumbtack = "Merge/UIPopupThumbtack";
    public static readonly string UIPopupSyntheticChainUnlock = "Merge/UIPopupSyntheticChainUnlock";
    
    
    public static readonly string MermaidHelp = "Activity/Mermaid/MermaidHelp";
    public static readonly string UIPopupMermaidMain = "Activity/Mermaid/UIPopupMermaidMain";
    public static readonly string UIPopupMermaidStart = "Activity/Mermaid/UIPopupMermaidStart";
    public static readonly string UIPopupMermaidStartPreview = "Activity/Mermaid/UIPopupMermaidStartPreview";
    public static readonly string UIPopupMermaidAddDay = "Activity/Mermaid/UIPopupMermaidAddDay";
    public static readonly string MermaidMapPreview = "Activity/Mermaid/MermaidMapPreview";
    public static readonly string MermaidMapBuild = "Activity/Mermaid/MermaidMapBuild";
    public static readonly string MermaidSlider = "Activity/Mermaid/MermaidSlider";
    public static readonly string UIPopupMermaidDouble = "Activity/Mermaid/UIPopupMermaidDouble";
    
    
    public static readonly string UIPopupEnergyBuy = "Merge/UIPopupEnergyBuy";
    
    public static readonly string UILoadingTransition = "UITransition/UILoadingTransition";
    public static readonly string UIPopupDeleteArchive1 = "Home/UIPopupDeleteArchive1";
    public static readonly string UIPopupDeleteArchive2 = "Home/UIPopupDeleteArchive2";

    public static readonly string UIRecoverCoinMain = "Activity/RecoverCoin/UIRecoverCoinMain";//金币回收主界面，领奖界面也在里面
    public static readonly string UIRecoverCoinStart = "Activity/RecoverCoin/UIRecoverCoinStart";//每周金币回收活动开始
    public static readonly string UIPopupRecoverCoinFinish = "Activity/RecoverCoin/UIPopupRecoverCoinFinish";//所有挂点完成
    public static readonly string UIRecoverCoinBuy = "Activity/RecoverCoin/UIRecoverCoinBuy";//金币回收金币兑换星星界面
    public static readonly string UIRecoverCoinEnd = "Activity/RecoverCoin/UIRecoverCoinEnd";//每周金币活动结束状态（暂时不需要用这个）
    public static readonly string UIPopupRecoverCoinNewDecoArea = "Activity/RecoverCoin/UIPopupRecoverCoinNewDecoArea";//已开启排行塞的玩家在新装修节点更新后的提示
    
    public static readonly string UIPopupLimitedTimeTask = "Activity/TimeOrder/UIPopupLimitedTimeTask";
    public static readonly string UIPopupTimeOrderGift = "Activity/TimeOrder/UIPopupTimeOrderGift";
    public static readonly string UIPopupLimitOrder = "Activity/LimitOrder/UIPopupLimitOrder";
    
    public static readonly string UIPopupFarmTimeLimitOrder = "Activity/FarmTimeLimit/UIPopupFarmTimeLimitOrder";
    
    public static readonly string UICoinCompetitionMain = "Activity/CoinCompetition/UICoinCompetitionMain";
    public static readonly string UICoinCompetitionStart = "Activity/CoinCompetition/UICoinCompetitionStart";

    public static readonly string UIPopupJumpGridStart = "Activity/JumpGrid/UIPopupJumpGridStart";
    public static readonly string UIJumpGridMain = "Activity/JumpGrid/UIJumpGridMain";
    
    public static readonly string UIPopupSummerWatermelonMain = "Activity/SummerWatermelonNormal/UIPopupSummerWatermelonMain";//夏日西瓜主界面
    public static readonly string UIPopupSummerWatermelonStart = "Activity/SummerWatermelonNormal/UIPopupSummerWatermelonStart";//夏日西瓜开始弹窗
    public static readonly string UISummerWatermelonReward = "Activity/SummerWatermelonNormal/UISummerWatermelonReward";//夏日西瓜合成新等级活动棋子时的提示弹窗
    public static readonly string UIPopupSummerWatermelonGift = "Activity/SummerWatermelonNormal/UIPopupSummerWatermelonGift";//夏日西瓜面包礼包
    
    public static readonly string UIPopupSummerWatermelonBreadMain = "Activity/SummerWatermelonBread/UIPopupSummerWatermelonBreadMain";//夏日西瓜面包主界面
    public static readonly string UIPopupSummerWatermelonBreadStart = "Activity/SummerWatermelonBread/UIPopupSummerWatermelonBreadStart";//夏日西瓜面包开始弹窗
    public static readonly string UISummerWatermelonBreadReward = "Activity/SummerWatermelonBread/UISummerWatermelonBreadReward";//夏日西瓜面包合成新等级活动棋子时的提示弹窗
    public static readonly string UIPopupSummerWatermelonBreadGift = "Activity/SummerWatermelonBread/UIPopupSummerWatermelonBreadGift";//夏日西瓜面包礼包
    
    public static readonly string UIPopupRecovery = "Common/UIPopupRecovery";
    
    public static readonly string UIDigTrenchMain = "UIDigTrench/UIDigTrenchMain";
    public static readonly string UIPopupGameChoose = "UIDigTrench/UIPopupGameChoose";
    public static readonly string UIPopupGameTimeUp = "UIDigTrench/UIPopupGameTimeUp";
    public static readonly string UIPopupGameTips = "UIDigTrench/UIPopupGameTips";
    
    #region CoinLeaderBoard
    public static readonly string UICoinLeaderBoardMain = "Activity/CoinLeaderBoard/UICoinLeaderBoardMain";//金币回收主界面，领奖界面也在里面
    public static readonly string UICoinLeaderBoardStart = "Activity/CoinLeaderBoard/UICoinLeaderBoardStart";//每周金币回收活动开始
    public static readonly string UIPopupCoinLeaderBoardFinish = "Activity/CoinLeaderBoard/UIPopupCoinLeaderBoardFinish";//所有挂点完成
    public static readonly string UICoinLeaderBoardBuy = "Activity/CoinLeaderBoard/UICoinLeaderBoardBuy";//金币回收金币兑换星星界面
    public static readonly string UICoinLeaderBoardEnd = "Activity/CoinLeaderBoard/UICoinLeaderBoardEnd";//每周金币活动结束状态（暂时不需要用这个）
    public static readonly string UIPopupCoinLeaderBoardNewDecoArea = "Activity/CoinLeaderBoard/UIPopupCoinLeaderBoardNewDecoArea";//已开启排行塞的玩家在新装修节点更新后的提示
    #endregion

    #region FishEatFish
    public static readonly string UIFishEatFishMain = "FishEatFish/FishDynamic/Prefabs/UI/UIFishEatFishMain";
    public static readonly string UIFishEatFishFail = "FishEatFish/FishDynamic/Prefabs/UI/UIFishEatFishFail";
    #endregion
    
    #region SeaRacing
    public static readonly string UIPopupSeaRacingEnd = "Activity/SeaRacing/UIPopupSeaRacingEnd";
    public static readonly string UIPopupSeaRacingPreview = "Activity/SeaRacing/UIPopupSeaRacingPreview";
    public static readonly string UIPopupSeaRacingStart = "Activity/SeaRacing/UIPopupSeaRacingStart";
    public static readonly string UISeaRacingMain = "Activity/SeaRacing/UISeaRacingMain";
    public static readonly string UISeaRacingOpenBox = "Activity/SeaRacing/UISeaRacingOpenBox";
    public static readonly string UISeaRacingReward = "Activity/SeaRacing/UISeaRacingReward";

    #endregion

    public static readonly string UIOnePathMain = "UIOnePath/UIOnePathMain";
    public static readonly string UIOnePathFail = "UIOnePath/UIOnePathFail";

    #region NewDailyPack
    public static readonly string UIPopupNewDailyGift = "Home/UIPopupNewDailyGift";
    public static readonly string UINewDailyPack = "Home/UINewDailyPack";
    #endregion

    #region CardCollection
    public static readonly string UIMainCard = "CardMain/UICard";
    public static readonly string UIMainCardGift = "CardMain/UICardGift";
    
    #endregion
    public static readonly string UIConnectLineMain = "ConnectLine/UIConnectLineMain";

    #region Easter2024
    public static readonly string UIEaster2024Main = "Activity/Donut/UIDonutMain";//扔球主界面
    // public static readonly string UIPopupEaster2024End = "Activity/Donut/UIPopupDonutEnd";//活动结束界面
    public static readonly string UIPopupEaster2024Preview = "Activity/Donut/UIPopupDonutPreview";//预热界面
    public static readonly string UIPopupEaster2024Start = "Activity/Donut/UIPopupDonutStart";//活动开始界面
    public static readonly string UIEaster2024MiniGameReward = "Activity/Donut/UIDonutMiniGameReward";//九宫格小游戏获奖弹窗
    public static readonly string UIPopupEaster2024MiniGame = "Activity/Donut/UIPopupDonutMiniGame";//九宫格小游戏界面
    public static readonly string UIEaster2024Shop = "Activity/Donut/UIDonutShop";//商店界面
    public static readonly string UIEaster2024LeaderBoardMain = "Activity/Donut/UIDonutLeaderBoardMain";//排行榜界面
    public static readonly string UIEaster2024Help = "Activity/Donut/UIDonutHelp"; //帮助界面
    public static readonly string UIPopupEaster2024ShopBuy = "Activity/Donut/UIPopupDonutShopBuy"; //购买确认界面
    public static readonly string UIPopupEaster2024NoEgg = "Activity/Donut/UIPopupDonutNoEgg";//没蛋

    #endregion

    public static readonly string UIPopupGiftBagBuyBetter = "Activity/GiftBagBuyBetter/UIPopupGiftBagBuyBetter";//越买越划算礼包主界面
    public static readonly string UIPopupGiftBagSendOne = "Activity/GiftBagSendOne/UIPopupGiftBagSendOne";//买一赠一礼包主界面
    
    #region SnakeLadder
    public static readonly string UISnakeLadderMain = "Activity/SnakeLadder/UISnakeLadderMain";//蛇梯子主界面
    // public static readonly string UIPopupSnakeLadderEnd = "Activity/SnakeLadder/UIPopupSnakeLadderEnd";//活动结束界面
    public static readonly string UIPopupSnakeLadderPreview = "Activity/SnakeLadder/UIPopupSnakeLadderPreview";//预热界面
    public static readonly string UIPopupSnakeLadderStart = "Activity/SnakeLadder/UIPopupSnakeLadderStart";//活动开始界面
    public static readonly string UISnakeLadderShop = "Activity/SnakeLadder/UISnakeLadderShop";//商店界面
    public static readonly string UISnakeLadderLeaderBoardMain = "Activity/SnakeLadder/UISnakeLadderLeaderBoardMain";//排行榜界面
    public static readonly string UISnakeLadderHelp = "Activity/SnakeLadder/UISnakeLadderHelp"; //帮助界面
    public static readonly string UIPopupSnakeLadderShopBuy = "Activity/SnakeLadder/UIPopupSnakeLadderShopBuy"; //购买确认界面
    public static readonly string UIPopupSnakeLadderNoTurntable = "Activity/SnakeLadder/UIPopupSnakeLadderNoTurntable";//没蛋
    public static readonly string UIPopupSnakeLadderUseCard = "Activity/SnakeLadder/UIPopupSnakeLadderUseCard";//任意卡
    #endregion

    public static readonly string UIExtraOrderRewardGet = "Activity/ExtraOrderRewardCoupon/UIExtraOrderRewardGet";
    public static readonly string UIPopupShopExtraRewardStart = "Activity/ShopExtraReward/UIPopupShopExtraRewardStart";
    public static readonly string UIPopupLevelUpPackage = "Home/UIPopupLevelUpPackage";
    
    #region ThemeDecoration
    // public static readonly string UIPopupThemeDecorationEnd = "Activity/ThemeDecoration/UIPopupThemeDecorationEnd";//活动结束界面
    public static readonly string UIPopupThemeDecorationPreview = "Activity/ThemeDecoration/UIPopupThemeDecorationPreview";//预热界面
    public static readonly string UIPopupThemeDecorationStart = "Activity/ThemeDecoration/UIPopupThemeDecorationStart";//活动开始界面
    public static readonly string UIThemeDecorationShop = "Activity/ThemeDecoration/UIThemeDecorationShop";//商店界面
    public static readonly string UIThemeDecorationLeaderBoardMain = "Activity/ThemeDecoration/UIThemeDecorationLeaderBoardMain";//排行榜界面
    public static readonly string UIThemeDecorationHelp = "Activity/ThemeDecoration/UIThemeDecorationHelp"; //帮助界面
    public static readonly string UIPopupThemeDecorationShopBuy = "Activity/ThemeDecoration/UIPopupThemeDecorationShopBuy"; //购买确认界面
    public static readonly string UIPopupThemeDecorationBuyPreEnd = "Activity/ThemeDecoration/UIPopupThemeDecorationBuyPreEnd";//额外时间购买界面
    public static readonly string UIPopupThemeDecorationMultipleScore = "Activity/ThemeDecoration/UIPopupThemeDecorationMultipleScore";//乘倍活动弹窗
    public static readonly string UIThemeDecorationMapPreview = "Activity/ThemeDecoration/UIThemeDecorationMapPreview";//预览界面
    #endregion
    public static readonly string UIStimulateSpineMain = "Stimulate/UIStimulateSpineMain";
    public static readonly string UIStimulateMergeMain = "Stimulate/UIStimulateMergeMain";

    #region SlotMachine
    public static readonly string UIPopupSlotMachineStart = "Activity/SlotMachine/UIPopupSlotMachineStart";
    public static readonly string UIPopupSlotMachineMain = "Activity/SlotMachine/UIPopupSlotMachineMain";
    public static readonly string UIPopupSlotMachineNoSpin = "Activity/SlotMachine/UIPopupSlotMachineNoSpin";
    public static readonly string UISlotMachineHelp = "Activity/SlotMachine/UISlotMachineHelp";
    #endregion
    
    #region Monopoly
    public static readonly string UIMonopolyMain = "Activity/Monopoly/UIMonopolyMain";//主界面
    // public static readonly string UIPopupMonopolyEnd = "Activity/Monopoly/UIPopupMonopolyEnd";//活动结束界面
    public static readonly string UIPopupMonopolyPreview = "Activity/Monopoly/UIPopupMonopolyPreview";//预热界面
    public static readonly string UIPopupMonopolyStart = "Activity/Monopoly/UIPopupMonopolyStart";//活动开始界面
    public static readonly string UIMonopolyShop = "Activity/Monopoly/UIMonopolyShop";//商店界面
    public static readonly string UIMonopolyLeaderBoardMain = "Activity/Monopoly/UIMonopolyLeaderBoardMain";//排行榜界面
    public static readonly string UIMonopolyHelp = "Activity/Monopoly/UIMonopolyHelp"; //帮助界面
    public static readonly string UIPopupMonopolyShopBuy = "Activity/Monopoly/UIPopupMonopolyShopBuy"; //购买确认界面
    public static readonly string UIPopupMonopolyNoDice = "Activity/Monopoly/UIPopupMonopolyNoDice";//没蛋
    public static readonly string UIPopupMonopolyUseCard = "Activity/Monopoly/UIPopupMonopolyUseCard";//任意卡
    public static readonly string UIMonopolyMiniGameReward = "Activity/Monopoly/UIMonopolyMiniGameReward";//九宫格小游戏获奖弹窗
    public static readonly string UIPopupMonopolyMiniGame = "Activity/Monopoly/UIPopupMonopolyMiniGame";//九宫格小游戏界面
    public static readonly string UIPopupMonopolyBuyBlock = "Activity/Monopoly/UIPopupMonopolyBuyBlock";//买地块弹板
    #endregion
    public static readonly string UIDecorationPreview = "Home/UIDecorationPreview";//预览界面

    public static readonly string UICrazeOrderMain = "Activity/CrazeOrder/UICrazeOrderMain";
    
    #region KeepPet
    public static readonly string UIKeepPetMain = "KeepPet/UIKeepPetMain";//主界面
    public static readonly string UIPopupKeepPetBag = "KeepPet/UIPopupKeepPetBag";//选巡逻奖励界面
    public static readonly string UIPopupKeepPetPatrol = "KeepPet/UIPopupKeepPetPatrol";//巡逻任务界面
    public static readonly string UIPopupKeepPetPatrolReward = "KeepPet/UIPopupKeepPetPatrolReward";//巡逻任务奖励介绍
    public static readonly string UIPopupKeepPetTask = "KeepPet/UIPopupKeepPetTask"; //每日任务界面
    public static readonly string UIPopupKeepPetTaskCompleted = "KeepPet/UIPopupKeepPetTaskCompleted";//每日任务完成
    public static readonly string UIPopupKeepPetGiftNoDrumsticks = "KeepPet/UIPopupKeepPetGiftNoDrumsticks";//没鸡腿弹窗
    public static readonly string UIPopupKeepPetGiftNoPower = "KeepPet/UIPopupKeepPetGiftNoPower";//没能量弹窗
    public static readonly string UIPopupKeepPetStart = "KeepPet/UIPopupKeepPetStart";//开始弹窗
    public static readonly string UIKeepPetHelp = "KeepPet/UIKeepPetHelp";//帮助弹窗
    public static readonly string UIPopupKeepPetClue = "KeepPet/UIPopupKeepPetClue";
    public static readonly string UIPopupKeepPetGift = "KeepPet/UIPopupKeepPetGift";
    public static readonly string UIPopupKeepPetThreeOneGift = "KeepPet/UIPopupKeepPetThreeOneGift";
    public static readonly string UIKeepPetLevelUp = "KeepPet/UIKeepPetLevelUp";//升级奖励弹窗
    public static readonly string UIPopupKeepPetReturn = "Home/UIPopupKeepPetReturn";//奖励返还弹窗
    #endregion
    
    #region TreasureMap
    public static readonly string UIPopupTreasureMap= "Activity/TreasureMap/UIPopupTreasureMap";
    public static readonly string UITreasureMapReward= "Activity/TreasureMap/UITreasureMapReward";
    #endregion
     
    #region TreasureHunt
    public static readonly string UITreasureHuntMain= "Activity/TreasureHunt/UITreasureHuntMain";
    public static readonly string UITreasureHuntHelp= "Activity/TreasureHunt/UITreasureHuntHelp";
    public static readonly string UIPopupTreasureHuntGift= "Activity/TreasureHunt/UIPopupTreasureHuntGift";
    public static readonly string UIPopupTreasureHuntStart= "Activity/TreasureHunt/UIPopupTreasureHuntStart";
    public static readonly string UIPopupMonopolyNoHammer= "Activity/TreasureHunt/UIPopupMonopolyNoHammer";
    #endregion
    
    #region LuckyGoldenEgg
    public static readonly string UILuckyGoldenEggMain= "Activity/LuckyGoldenEgg/UILuckyGoldenEggMain";
    public static readonly string UILuckyGoldenEggHelp= "Activity/LuckyGoldenEgg/UILuckyGoldenEggHelp";
    public static readonly string UIPopupLuckyGoldenEggGift= "Activity/LuckyGoldenEgg/UIPopupLuckyGoldenEggGift";
    public static readonly string UIPopupLuckyGoldenEggStart= "Activity/LuckyGoldenEgg/UIPopupLuckyGoldenEggStart";
    public static readonly string UIPopupLuckyGoldenEggNoItem= "Activity/LuckyGoldenEgg/UIPopupLuckyGoldenEggNoItem";
    #endregion
    
    public static readonly string UIPopupMiniGame = "Home/UIPopupMiniGame";   
    public static readonly string UIPsychologyMain = "Psychology/UIPsychologyMain";   
    
    public static readonly string UIPopupExtraEnergyStart = "Activity/ExtraEnergy/UIPopupExtraEnergyStart";

    #region GiftBagProgress
    public static readonly string UIPopupGiftBagProgressTask = "Activity/GiftBagProgress/UIPopupGiftBagProgressTask";
    public static readonly string UIPopupGiftBagProgressTaskCompleted = "Activity/GiftBagProgress/UIPopupGiftBagProgressTaskCompleted";
    public static readonly string UIGiftBagProgressHelp = "Activity/GiftBagProgress/UIGiftBagProgressHelp";
    #endregion
    
    #region OptionalGift
    public static readonly string UIPopupOptionalGiftMain = "Activity/OptionalGift/UIPopupOptionalGiftMain";
    public static readonly string UIPopupOptionalGiftSelect = "Activity/OptionalGift/UIPopupOptionalGiftSelect";
    #endregion
    
    public static readonly string UIPopupMatreshkas = "Activity/Matreshkas/UIPopupMatreshkas";
    
    //蝴蝶工坊
    public static readonly string UIButterflyWorkShopMain = "Activity/ButterflyWorkShop/UIButterflyWorkShopMain";
    public static readonly string UIPopupButterflyWorkShopStart = "Activity/ButterflyWorkShop/UIPopupButterflyWorkShopStart";
    public static readonly string UIPopupButterflyWorkShopEnd = "Activity/ButterflyWorkShop/UIPopupButterflyWorkShopEnd";
    public static readonly string UIButterflyWorkShopHelp = "Activity/ButterflyWorkShop/UIButterflyWorkShopHelp";
    public static readonly string UIPopupButterflyWorkShop = "Activity/ButterflyWorkShop/UIPopupButterflyWorkShop";
    public static readonly string UIPopupButterflyWorkTip = "Activity/ButterflyWorkShop/UIPopupButterflyWorkTip";
    
    public static readonly string UIPopupTurntableMain = "Activity/Turntable/UIPopupTurntableMain";
    public static readonly string UITurntableHelp = "Activity/Turntable/UITurntableHelp";
    public static readonly string UIPopupTurntableNoTurntable = "Activity/Turntable/UIPopupTurntableNoTurntable";

    public static readonly string UIDiamondRewardMain = "Activity/DiamondReward/UIDiamondRewardMain";
    public static readonly string UIPopupDiamondRewardBuy = "Activity/DiamondReward/UIPopupDiamondRewardBuy";

    public static readonly string UIPopupGiftBagDoubleMain = "Activity/GiftBagDouble/UIPopupGiftBagDoubleMain";
    
    public static readonly string UIGardenTreasureLeaderBoardMain = "Activity/GardenTreasure/UIGardenTreasureLeaderBoardMain";
    public static readonly string UIPopupGardenTreasureStart = "Activity/GardenTreasure/UIPopupGardenTreasureStart";
    public static readonly string UIGardenTreasureMain = "Activity/GardenTreasure/UIGardenTreasureMain";
    public static readonly string UIGardenTreasureHelp = "Activity/GardenTreasure/UIGardenTreasureHelp";
    public static readonly string UIPopupGardenTreasureGift = "Activity/GardenTreasure/UIPopupGardenTreasureGift";
    public static readonly string UIBlueBlockMain = "MiniGame/BlueBlock/UI/UIBlueBlockMain";
    public static readonly string UIPopupGardenTreasureNoItem = "Activity/GardenTreasure/UIPopupGardenTreasureNoItem";
    
    
    public static readonly string UISaveTheWhalesReward = "Activity/SaveTheWhales/UISaveTheWhalesReward";

    
    public static readonly string UIMixMasterMain = "Activity/MixMaster/UIMixMasterMain";
    public static readonly string UIMixMasterUnlock = "Activity/MixMaster/UIMixMasterUnlock";
    public static readonly string UIPopupMixMasterTaskCompleted = "Activity/MixMaster/UIPopupMixMasterTaskCompleted";
    public static readonly string UIPopupMixMasterShop = "Activity/MixMaster/UIPopupMixMasterShop";
    public static readonly string UIPopupMixMasterList = "Activity/MixMaster/UIPopupMixMasterList";
    public static readonly string UIMixMasterMakeSuccess = "Activity/MixMaster/UIMixMasterMakeSuccess";
    public static readonly string UIPopupMixMasterPreview = "Activity/MixMaster/UIPopupMixMasterPreview";
    public static readonly string UIMixMasterMakeFailed = "Activity/MixMaster/UIMixMasterMakeFailed";

    public static readonly string UITurtlePangMain = "Activity/TurtlePang/UITurtlePangMain";
    public static readonly string UITurtlePangShop = "Activity/TurtlePang/UITurtlePangShop";
    public static readonly string UIPopupTurtlePangPreview = "Activity/TurtlePang/UIPopupTurtlePangPreview";
    public static readonly string UIPopupTurtlePangShopBuy = "Activity/TurtlePang/UIPopupTurtlePangShopBuy";
    public static readonly string UITurtlePangGiftBag = "Activity/TurtlePang/UITurtlePangGiftBag";
    public static readonly string UIPopupTurtlePangNoItem = "Activity/TurtlePang/UIPopupTurtlePangNoItem";

    public static readonly string UIStarrySkyCompassMain = "Activity/StarrySkyCompass/UIStarrySkyCompassMain";
    public static readonly string UIStarrySkyCompassShop = "Activity/StarrySkyCompass/UIStarrySkyCompassShop";
    public static readonly string UIPopupStarrySkyCompassPreview = "Activity/StarrySkyCompass/UIPopupStarrySkyCompassPreview";
    public static readonly string UIPopupStarrySkyCompassNoItem = "Activity/StarrySkyCompass/UIPopupStarrySkyCompassNoItem";

    
    public static readonly string UIPopupBlindBoxRecycleTip = "BlindBox/Common/UIPopupBlindBoxRecycleTip";
    public static readonly string UIBlindBoxRecycle = "BlindBox/Common/UIBlindBoxRecycle";
    public static readonly string UIBlindBoxMain = "BlindBox/Common/UIBlindBoxMain";
    public static readonly string UIBlindBox1 = "BlindBox/Theme1/UIBlindBox1";
    public static readonly string UIBlindBoxReward1 = "BlindBox/Theme1/UIPopupBlindBoxReward";
    public static readonly string UIBlindBoxOpen1 = "BlindBox/Theme1/UIBlindBoxOpen";
    public static readonly string UIBlindBoxPreview1 = "BlindBox/Theme1/UIBlindBoxPreview";
    public static readonly string UIBlindBox2 = "BlindBox/Theme2/UIBlindBox2";
    public static readonly string UIBlindBoxReward2 = "BlindBox/Theme2/UIPopupBlindBoxReward";
    public static readonly string UIBlindBoxOpen2 = "BlindBox/Theme2/UIBlindBoxOpen";
    public static readonly string UIBlindBoxPreview2 = "BlindBox/Theme2/UIBlindBoxPreview";
    public static readonly string UIBlindBox3 = "BlindBox/Theme3/UIBlindBox3";
    public static readonly string UIBlindBoxReward3 = "BlindBox/Theme3/UIPopupBlindBoxReward";
    public static readonly string UIBlindBoxOpen3 = "BlindBox/Theme3/UIBlindBoxOpen";
    public static readonly string UIBlindBoxPreview3 = "BlindBox/Theme3/UIBlindBoxPreview";
    public static readonly string UIBlindBox4 = "BlindBox/Theme4/UIBlindBox4";
    public static readonly string UIBlindBoxReward4 = "BlindBox/Theme4/UIPopupBlindBoxReward";
    public static readonly string UIBlindBoxOpen4 = "BlindBox/Theme4/UIBlindBoxOpen";
    public static readonly string UIBlindBoxPreview4 = "BlindBox/Theme4/UIBlindBoxPreview";
    public static readonly string UIBlindBox5 = "BlindBox/Theme5/UIBlindBox5";
    public static readonly string UIBlindBoxReward5 = "BlindBox/Theme5/UIPopupBlindBoxReward";
    public static readonly string UIBlindBoxOpen5 = "BlindBox/Theme5/UIBlindBoxOpen";
    public static readonly string UIBlindBoxPreview5 = "BlindBox/Theme5/UIBlindBoxPreview";

    public static readonly string UINewDailyPackageExtraReward = "Activity/NewDailyPackageExtraReward/UINewDailyPackageExtraReward";

    public static readonly string UIBuyDiamondTicket = "Home/UIBuyDiamondTicket";
    
    public static readonly string UIPopupTotalRecharge = "Activity/TotalRecharge/UIPopupTotalRecharge";
    public static readonly string UIPopupTotalRecharge_New = "Home/UIPopupTotalRecharge_New";

    public static readonly string UIZumaMain = "Activity/Zuma/UIZumaMain";
    public static readonly string UIZumaNoItem = "Activity/Zuma/UIZumaNoItem";
    public static readonly string UIPopupZumaPreview = "Activity/Zuma/UIPopupZumaPreview";
    public static readonly string UIPopupZumaShopBuy = "Activity/Zuma/UIPopupZumaShopBuy";
    public static readonly string UIPopupZumaStart = "Activity/Zuma/UIPopupZumaStart";
    public static readonly string UIZumaHelp = "Activity/Zuma/UIZumaHelp";
    public static readonly string UIZumaBoardMain = "Activity/Zuma/UIZumaBoardMain";
    public static readonly string UIZumaShop = "Activity/Zuma/UIZumaShop";
    public static readonly string UIPopupZumaGift = "Activity/Zuma/UIPopupZumaGift";
    public static readonly string UIPopupZumaReward = "Activity/Zuma/UIPopupZumaReward";
    public static readonly string UIPopupZumaReward1 = "Activity/Zuma/UIPopupZumaReward1";
    
    public static readonly string UIPopupKeepPetTurkeyStart = "Activity/KeepPetTurkey/UIPopupKeepPetTurkeyStart";
    public static readonly string UIKeepPetTurkeyShop = "Activity/KeepPetTurkey/UIKeepPetTurkeyShop";
    public static readonly string UIPopupKeepPetTurkeyShopBuy = "Activity/KeepPetTurkey/UIPopupKeepPetTurkeyShopBuy";

    public static readonly string UIPopupKapibalaStart = "Activity/Kapibala/UIPopupKapibalaStart";
    public static readonly string UIPopupKapibalaPreview = "Activity/Kapibala/UIPopupKapibalaPreview";
    public static readonly string UIKapibalaMain = "Activity/Kapibala/UIKapibalaMain";
    public static readonly string UIPopupKapibalaGiftBag = "Activity/Kapibala/UIPopupKapibalaGiftBag";
    
    public static readonly string UIFilthySpineMain = "Filthy/UIFilthySpineMain";
    public static readonly string UIFilthyMergeMain = "Filthy/UIFilthyMergeMain";

    public static readonly string UIFarmMain = "Farm/Prefabs/UI/UIFarmMain";
    public static readonly string UIPopupFarmBag = "Farm/Prefabs/UI/UIPopupFarmBag";
    public static readonly string FarmSellUI = "Farm/Prefabs/UI/FarmSellUI";
    public static readonly string UIPopupWarehouseUpgrade = "Farm/Prefabs/UI/UIPopupWarehouseUpgrade";
    public static readonly string UIPopupFarmLevelTipsShow = "Farm/Prefabs/UI/UIPopupFarmLevelTipsShow";
    public static readonly string UIPopupFarmLevelTips = "Farm/Prefabs/UI/UIPopupFarmLevelTips";
    public static readonly string UIPopupFarmNotice = "Farm/Prefabs/UI/UIPopupFarmNotice";
    public static readonly string UIFarmLoading = "Farm/Prefabs/UI/UIFarmLoading";
    public static readonly string UIPopupFarmItemInformation = "Farm/Prefabs/UI/UIPopupFarmItemInformation";
    
    
    public static readonly string UIPopupChristmasBlindBox = "Activity/ChristmasBlindBox/UIPopupChristmasBlindBox";
    public static readonly string UIChristmasBlindBoxOpen = "Activity/ChristmasBlindBox/UIChristmasBlindBoxOpen";

    public static readonly string UIPopupScrewGameSelect = "Home/UIPopupGameSelect";
    public static readonly string UIPopupNewbiePack = "Home/UIPopupNewbiePack";
    
    public static readonly string UIKapiScrewMain = "Activity/KapiScrew/UIKapibalaContestMain";
    public static readonly string UIPopupKapiScrewPreview = "Activity/KapiScrew/UIPopupKapiScrewPreview";
    public static readonly string UIPopupKapiScrewShop = "Activity/KapiScrew/UIPopupKapibalaOptionalGift";
    public static readonly string UIPopupKapiScrewStart = "Activity/KapiScrew/UIPopupKapiScrewStart";

    public static readonly string UIPopupKapiScrewOptionalGiftSelect = "Activity/KapiScrew/UIPopupKapibalaOptionalGiftSelect";
    public static readonly string UIPopupDogPlay = "Prefabs/Home/UIPopupDogPlay";

    public static readonly string UIFishCultureBoardMain = "Activity/FishCulture/UIFishCultureBoardMain";
    public static readonly string UIPopupFishCulturePreview = "Activity/FishCulture/UIPopupFishCulturePreview";
    public static readonly string UIFishCultureMain = "Activity/FishCulture/UIFishCultureMain";
    public static readonly string UIPopupFishCultureNoItem = "Activity/FishCulture/UIPopupFishCultureNoItem";
    public static readonly string UIPopupFishCultureShopBuy = "Activity/FishCulture/UIPopupFishCultureShopBuy";
    public static readonly string UIPopupFishExit = "Activity/FishCulture/UIPopupFishExit";
    
    public static readonly string UIDitchMergeMain = "DigTrenchNew/Prefabs/UIDitchMergeMain";
    public static readonly string UIDigTrenchNewMain = "DigTrenchNew/Prefabs/UIDigTrenchNewMain";
    public static readonly string UIPopupDigTrenchNewGameTips = "DigTrenchNew/Prefabs/UIPopupDigTrenchNewGameTips";

    public static readonly string UIDigTrenchNewStory = "Prefabs/UIStory/UIDigTrenchNewStory";
    public static readonly string UIPopupDogPlayExtraReward = "Activity/DogPlayExtraReward/UIPopupDogPlayExtraReward";
    
    public static readonly string UIPopupKapiTilePreview = "Activity/KapiTile/UIPopupKapiTilePreview";
    public static readonly string UIKapiTileMain = "Activity/KapiTile/UIKapiTileMain";
    public static readonly string UIPopupKapiTileGiftBag = "Activity/KapiTile/UIPopupKapiTileGiftBag";
    
    public static readonly string UIPhotoAlbumShop = "Activity/PhotoAlbum/UIPopupPhotoAlbumMain";
    public static readonly string UIPopupPhotoAlbumExit = "Activity/PhotoAlbum/UIPopupPhotoAlbumExit";
    public static readonly string UIPopupPhotoAlbumPreview = "Activity/PhotoAlbum/UIPopupPhotoAlbumPreview";
    public static readonly string UIPopupPhotoAlbumShopBuy = "Activity/PhotoAlbum/UIPopupPhotoAlbumShopBuy";
    public static readonly string UIPopupPhotoAlbumProgress = "Activity/PhotoAlbum/UIPopupPhotoAlbumProgress";
    public static readonly string UIPopupPhotoAlbumSpine = "Activity/PhotoAlbum/UIPopupPhotoAlbumSpine";


    public static readonly string UIJungleAdventureMain = "Activity/JungleAdventure/UIJungleAdventureMain";
    public static readonly string UIPopupJungleAdventurePreview = "Activity/JungleAdventure/UIPopupJungleAdventurePreview";
    public static readonly string UIJungleAdventureBoardMain = "Activity/JungleAdventure/UIJungleAdventureBoardMain";
    public static readonly string UIPopupJungleAdventureReward = "Activity/JungleAdventure/UIPopupJungleAdventureReward";
    
    public static readonly string UIPopupBindEmail = "Prefabs/Home/UIPopupBindEmail";
    public static readonly string UIPopupShopRv = "Merge/UIPopupShopRv";
    public static readonly string UIPopupLuckBubbleRv = "Merge/UIPopupLuckBubbleRv";
    
    public static readonly string UIBiuBiuMain = "Activity/BiuBiu/UIBiuBiuMain";
    public static readonly string UIBiuBiuHelp = "Activity/BiuBiu/UIBiuBiuHelp";
    public static readonly string UIPopupBiuBiuPackage = "Activity/BiuBiu/UIPopupBiuBiuPackage";
    public static readonly string UIPopupBiuBiuTip = "Activity/BiuBiu/UIPopupBiuBiuTip";
    
    public static readonly string UIParrotStart = "Activity/Parrot/UIPopupParrotStart";
    public static readonly string UIParrotMain = "Activity/Parrot/UIParrotMain";
    public static readonly string UIPopupParrotPreview = "Activity/Parrot/UIPopupParrotPreview";
    public static readonly string UIParrotLeaderBoardMain = "Activity/Parrot/UIParrotBoardMain";
    public static readonly string UIParrotHelp = "Activity/Parrot/UIParrotHelp";
    
    
    public static readonly string UIBalloonRacingMain = "Activity/BalloonRacing/UISpeedRaceMain";//竞速主界面
    public static readonly string UIBalloonRacingStart = "Activity/BalloonRacing/UIPopupSpeedRaceStart";//竞速加入
    public static readonly string UIBalloonRacingFail = "Activity/BalloonRacing/UIPopupSpeedRaceFail";//竞速失败
    public static readonly string UIBalloonRacingOpenBox = "Activity/BalloonRacing/UIBalloonRacingOpenBox";//打开奖励
    public static readonly string UIBalloonRacingReward = "Activity/BalloonRacing/UIBalloonRacingReward";//竞速奖励
    
    
    
    public static readonly string UIRabbitRacingMain = "Activity/RabbitRacing/UIRabbitRacingMain";//竞速主界面
    public static readonly string UIPopupRabbitRacingStart = "Activity/RabbitRacing/UIPopupRabbitRacingStart";//竞速加入
    public static readonly string UIPopupRabbitRacingFail = "Activity/RabbitRacing/UIPopupRabbitRacingFail";//竞速失败
    public static readonly string UIRabbitRacingOpenBox = "Activity/RabbitRacing/UIRabbitRacingOpenBox";//打开奖励
    public static readonly string UIRabbitRacingReward = "Activity/RabbitRacing/UIRabbitRacingReward";//竞速奖励
    
    
    public static readonly string UICommonNormalBox = "Common/RewardTip";

    public static readonly string UIPopupNewNewIceBreakPack = "Prefabs/Home/UIPopupNewNewIceBreakPack";
    public static readonly string UIPopupNewNewIceBreakPackFinish = "Prefabs/Home/UIPopupNewNewIceBreakPackFinish";
    
    public static readonly string UIFlowerFieldStart = "Activity/FlowerField/UIPopupFlowerFieldStart";
    public static readonly string UIFlowerFieldMain = "Activity/FlowerField/UIFlowerFieldMain";
    public static readonly string UIPopupFlowerFieldPreview = "Activity/FlowerField/UIPopupFlowerFieldPreview";
    public static readonly string UIFlowerFieldLeaderBoardMain = "Activity/FlowerField/UIPopupFlowerFieldBoardMain";
    public static readonly string UIFlowerFieldHelp = "Activity/FlowerField/UIFlowerFieldHelp";

    public static readonly string UIClimbTowerMain = "ActivityLocal/ClimbTower/UIClimbTowerMain";
    public static readonly string UIPopupClimbTowerQuit = "ActivityLocal/ClimbTower/UIPopupClimbTowerQuit";
    public static readonly string UIPopupClimbTowerPay = "ActivityLocal/ClimbTower/UIPopupClimbTowerPay";
    public static readonly string UIClimbTowerMainPay = "ActivityLocal/ClimbTower/UIClimbTowerMainPay";
    public static readonly string UIPopupClimbTowerCollect = "ActivityLocal/ClimbTower/UIPopupClimbTowerCollect";
    
    public static readonly string UIPopupGiftBagSendTwo = "Activity/GiftBagSendTwo/UIPopupGiftBagSendTwo";//买一赠2礼包主界面
    public static readonly string UIPopupGiftBagSendThree = "Activity/GiftBagSendThree/UIPopupGiftBagSendThree";//买一赠3礼包主界面
    public static readonly string UIPopupGiftBagSend4 = "Activity/GiftBagSend4/UIPopupGiftBagSend4";//买一赠4礼包主界面
    public static readonly string UIPopupGiftBagSend6 = "Activity/GiftBagSend6/UIPopupGiftBagSend6";//买一赠6礼包主界面

    public static readonly string UIPopupTipRewardMain = "Merge/UIPopupTipRewardMain";

    public static readonly string UIPopupCollectStoneMain = "Activity/CollectStone/UIPopupCollectStoneMain";
    
    public static readonly string UIPopupGuildJoin = "Prefabs/Home/UIPopupGuildJoin";//公会选择或创建
    public static readonly string UIPopupGuildSetIcon = "Prefabs/Home/UIPopupGuildSetIcon";//公会选图标
    public static readonly string UIPopupGuildJoinPreview = "Prefabs/Home/UIPopupGuildJoinPreview";//公会预览
    public static readonly string UIPopupGuildMain = "Prefabs/Home/UIPopupGuildMain";//公会主界面
    public static readonly string UIPopupGuildSet = "Prefabs/Home/UIPopupGuildSet";//公会设置界面
    public static readonly string UIPopupGuildCardGet = "Prefabs/Home/UIPopupGuildCardGet";//公会卡包详情
    public static readonly string UIPopupGuildCardOpen = "Prefabs/Home/UIPopupGuildCardOpen";//公会开卡包
    public static readonly string UIPopupTeamMemberInfo =  "Prefabs/Home/UIPopupTeamMemberInfo";//其他玩家的详细信息
    public static readonly string UIPopupGuildExitTip = "Prefabs/Home/UIPopupGuildExitTip";//公会退出确认
    public static readonly string UIPopupGuildLevelUp = "Prefabs/Home/UIPopupGuildLvUp";//公会升级详情
    public static readonly string UIPopupGuildJoinTip = "Prefabs/Home/UIPopupGuildJoinTip";//公会加入确认
    public static readonly string UIPopupGuildKickTip = "Prefabs/Home/UIPopupGuildKickTip";//公会踢出确认
    public static readonly string UIPopupGuildDisbandTip = "Prefabs/Home/UIPopupGuildDisbandTip";//公会解散确认
    
    public static readonly string UIPopupLimitOrderGift = "Activity/LimitOrder/UIPopupLimitOrderGift";
    
    public static readonly string UIMiniGameLoading = "NewMiniGame/UIMiniGame/Prefab/UIUIMiniGameLoading";
    
    
    public static readonly string UITrainOrderMain = "Activity/TrainOrder/UITrainOrderMain";
    public static readonly string UIPopupTrainOrderStart = "Activity/TrainOrder/UIPopupTrainOrderStart";
    
    public static readonly string UIPillowWheelMain = "Activity/PillowWheel/UIPillowWheelMain";
    public static readonly string UIPopupPillowWheelPreview = "Activity/PillowWheel/UIPopupPillowWheelPreview";
    public static readonly string UIPillowWheelLeaderBoardMain = "Activity/PillowWheel/UIPopupPillowWheelBoardMain";
    public static readonly string UIPopupPillowWheelGift = "Activity/PillowWheel/UIPopupPillowWheelGift";
    public static readonly string UIPillowWheelHelp = "Activity/PillowWheel/UIPillowWheelHelp";
    public static readonly string UIPopupPillowWheelNoItem = "Activity/PillowWheel/UIPopupPillowWheelNoItem";
    
    public static readonly string UIPopupNoADS = "Prefabs/Home/UIPopupNoADS";
    public static readonly string UIEndlessEnergyGiftBag = "Prefabs/Home/UIEndlessGift";
    
    public static readonly string UICatchFishMain = "Prefabs/Home/UICatchFishMain";
    public static readonly string UIPopupCatchFishPreview = "Prefabs/Home/UIPopupCatchFishPreview";
}
