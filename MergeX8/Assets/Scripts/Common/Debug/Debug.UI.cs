using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Decoration;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Game;
using Gameplay;
using Gameplay.UI.Capybara;
using Gameplay.UI.EnergyTorrent;
using Merge.Order;
using TMatch;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
using File = DragonU3DSDK.Network.API.Protocol.File;

/// <summary>
/// UI，弹窗，界面相关
/// </summary>
public partial class SROptions
{
    private const string categoryUI = "1界面";
    private const string categoryAd = "广告";
    private const string PiggyBank = "小猪";
    private const string Pack = "礼包";
    private const string Task = "2任务";
    private const string Merge = "3Merge";
    private const string Story = "Story";
    private const string SignIn = "签到";
    private const string DailyRank = "每日排行";
    private const string DogHope = "小狗";
    private const string ClimbTree = "猴子爬树";
    private const string RecoverCoin = "回收金币";
    private const string SummerWatermelon = "夏日西瓜";
    private const string SummerWatermelonBread = "夏日西瓜(面包换皮)";
    private const string TMatch = "捡垃圾";
    private const string CoinRush = "金币rush";
    private const string Decoration = "装修挂点";
    private const string MysteryGift = "神秘礼物";
    private const string PayRebate = "充值返利";
    private const string GarageCleanup = "仓库整理";
    private const string Easter = "复活节";
    private const string CoinCompetition = "金币挑战";
    private const string MergePackageGuide = "背包引导";
    private const string HappyGo = "HappyGo";
    private const string KeepPet = "KeepPet";
    private const string TreasureHunt = "TreasureHunt";
    private const string ButterflyWorkShop = "胡蝶工坊";
    private const string SaveTheWhales = "拯救鲸鱼";
    private const string Adapt = "限时限时适配";
    public void CleanGuideList(List<int> guideIdList)
    {
        var guideFinish = StorageManager.Instance.GetStorage<StorageHome>().Guide.GuideFinished;
        var cacheGuideFinished = GuideSubSystem.Instance.CacheGuideFinished;
        for (var i = 0; i < guideIdList.Count; i++)
        {
            var guideId = guideIdList[i];
            if (guideFinish.ContainsKey(guideId))
            {
                guideFinish.Remove(guideId);
            }
            if (cacheGuideFinished.ContainsKey(guideId))
            {
                cacheGuideFinished.Remove(guideId);
            }
        }
    }
    [Category(MergePackageGuide)]
    [DisplayName("背包引导清空")]
    public void CleanMergePackageClean()
    {
        var guideIdList = new List<int>() {568,410,411};
        CleanGuideList(guideIdList);
    }
    private void HideDebugPanel()
    {
        SRDebug.Instance.HideDebugPanel();
    }
    
    [Sort(-40)]
    [Category(categoryUI)]
    [DisplayName("是否使用低清图集")]
    public bool IsUseSdAtlas
    {
        get
        {
            int value = PlayerPrefs.GetInt("useSdAtlas");
            return value == 1;
        }
        set
        {
            PlayerPrefs.SetInt("useSdAtlas", value ? 1 : 0);
        }
        
    }
    
    
    [Sort(-30)]
    [Category(categoryUI)]
    [DisplayName("删档并退出")]
    public void DeleteAllStorage()
    {
        DebugCmdExecute.ClearStorage();
        PlayerPrefs.DeleteAll();
        DebugCmdExecute.QuitApp();
    }
    [Category(categoryUI)]
    [DisplayName("重置m周卡")]
    public void ClearWeeklyCard()
    {
        HideDebugPanel();
        StorageHome storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        storageHome.WeeklyCard.Clear();
    }  
    [Category(categoryUI)]
    [DisplayName("重置累计充值")]
    public void ClearTotalRecharge()
    {
        HideDebugPanel();
        StorageHome storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        storageHome.TotalRecharges.Clear();
    }
    [Category(SignIn)]
    [DisplayName("签到")]
    public void ShowDailyBonus()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UIDailyBouns, 9);
    }
   
    [Category(SignIn)]
    [DisplayName("重置签到")]
    public void ClearDailyBonus()
    {
        HideDebugPanel();
        StorageHome storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        storageHome.DailyBonus.Clear();
    }
    
    [Category(SignIn)]
    [DisplayName("签到总天数")]
    public int TotalClaimDailyBonus
    {
        get
        {
            StorageHome storageHome = StorageManager.Instance.GetStorage<StorageHome>();
            return storageHome.DailyBonus.TotalClaimDay;
        }
        set
        {
            StorageHome storageHome = StorageManager.Instance.GetStorage<StorageHome>();
            storageHome.DailyBonus.TotalClaimDay=value;
        }
    }

    
    [Sort(-20)]
    [Category(categoryUI)]
    [DisplayName("关闭")]
    public void CloseDebug()
    {
        HideDebugPanel();
    }

    [Sort(-10)]
    [Category(categoryUI)]
    [DisplayName("修改装修币")]
    public int coin
    {
        get { return UserData.Instance.GetRes(UserData.ResourceId.Coin); }
        set
        {
            GameBIManager.ItemChangeReasonArgs reason = new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
            UserData.Instance.SetRes(UserData.ResourceId.Coin, value, reason);
        }
    }
    [Sort(-10)]
    [Category(categoryUI)]
    [DisplayName("任务获得总装修币")]
    public int TotalCoin
    {
        get { return StorageManager.Instance.GetStorage<StorageHome>().TotalDecoCoin; }
    }


    [Sort(-9)]
    [Category(categoryUI)]
    [DisplayName("修改特殊装修币")]
    public int key
    {
        get { return UserData.Instance.GetRes(UserData.ResourceId.RareDecoCoin); }
        set
        {
            GameBIManager.ItemChangeReasonArgs reason = new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
            UserData.Instance.SetRes(UserData.ResourceId.RareDecoCoin, value, reason);
        }
    }

    [Sort(-9)]
    [Category(categoryUI)]
    [DisplayName("修改海豹装修币")]
    public int sealKey
    {
        get { return UserData.Instance.GetRes(UserData.ResourceId.Seal); }
        set
        {
            GameBIManager.ItemChangeReasonArgs reason = new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
            UserData.Instance.SetRes(UserData.ResourceId.Seal, value, reason);
        }
    }
    
    [Sort(-9)]
    [Category(categoryUI)]
    [DisplayName("修改海豚装修币")]
    public int DolphinKey
    {
        get { return UserData.Instance.GetRes(UserData.ResourceId.Dolphin); }
        set
        {
            GameBIManager.ItemChangeReasonArgs reason = new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
            UserData.Instance.SetRes(UserData.ResourceId.Dolphin, value, reason);
        }
    }
    [Sort(-9)]
    [Category(categoryUI)]
    [DisplayName("修改水豚装修币")]
    public int CapybaraKey
    {
        get { return UserData.Instance.GetRes(UserData.ResourceId.Capybara); }
        set
        {
            GameBIManager.ItemChangeReasonArgs reason = new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
            UserData.Instance.SetRes(UserData.ResourceId.Capybara, value, reason);
        }
    }
    
    
    
    [Sort(-8)]
    [Category(categoryUI)]
    [DisplayName("修改美人鱼活动币")]
    public int MermaidNum
    {
        get { return UserData.Instance.GetRes(UserData.ResourceId.Mermaid); }
        set
        {
            GameBIManager.ItemChangeReasonArgs reason = new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
            UserData.Instance.SetRes(UserData.ResourceId.Mermaid, value, reason);
        }
    }
    
    
    [Sort(-8)]
    [Category(categoryUI)]
    [DisplayName("修改钻石")]
    public int DiamondNum
    {
        get { return UserData.Instance.GetRes(UserData.ResourceId.Diamond); }
        set
        {
            GameBIManager.ItemChangeReasonArgs reason = new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
            UserData.Instance.SetRes(UserData.ResourceId.Diamond, value, reason);
        }
    }

    [Sort(-7)]
    [Category(categoryUI)]
    [DisplayName("修改体力")]
    public int EnergyNum
    {
        get { return UserData.Instance.GetRes(UserData.ResourceId.Energy); }
        set
        {
            GameBIManager.ItemChangeReasonArgs reason = new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
            UserData.Instance.SetRes(UserData.ResourceId.Energy, value, reason);
        }
    }
    [Sort(-7)]
    [Category(categoryUI)]
    [DisplayName("修改背包卷")]
    public int BagTokenNum
    {
        get { return UserData.Instance.GetRes(UserData.ResourceId.BagToken); }
        set
        {
            GameBIManager.ItemChangeReasonArgs reason = new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
            UserData.Instance.SetRes(UserData.ResourceId.BagToken, value, reason);
        }
    }

    [Sort(-6)]
    [Category(categoryUI)]
    [DisplayName("修改经验")]
    public int ExpNum
    {
        get { return UserData.Instance.GetRes(UserData.ResourceId.Exp); }
        set
        {
            GameBIManager.ItemChangeReasonArgs reason = new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
            UserData.Instance.SetRes(UserData.ResourceId.Exp, value, reason);
        }
    }

    [Sort(-6)]
    [Category(categoryUI)]
    [DisplayName("修改等级")]
    public int UserLevel
    {
        get { return StorageManager.Instance.GetStorage<StorageHome>().Level; }
        set { StorageManager.Instance.GetStorage<StorageHome>().Level = value; }
    }
  
    [Sort(-4)]
    [Category(categoryUI)]
    [DisplayName("开启debug支付")]
    public bool DebugPay
    {
        get
        {
            if (ConfigurationController.Instance.version == VersionStatus.DEBUG)
                return StoreModel.Instance.isDebugPay;
            return false;
        }
        set
        {
            if (ConfigurationController.Instance.version == VersionStatus.DEBUG)
                StoreModel.Instance.isDebugPay = value;
        }
    }

    [Sort(-4)]
    [Category(categoryUI)]
    [DisplayName("开启Capybara")]
    public bool Capybara
    {
        get
        {
            return CapybaraManager.Instance.IsOpenCapybara();
        }
        set
        {
            if (value)
            {
                StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord[CapybaraManager.recordKey] = true;
            }
            else
            {
                if (StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey(CapybaraManager.recordKey))
                    StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.Remove(CapybaraManager.recordKey);
            }
        }
    }
    
    
    [Sort(-2)]
    [Category(categoryUI)]
    [DisplayName("清空棋盘")]
    public void ClearMergeBoard()
    {
        if (StorageManager.Instance.GetStorage<StorageGame>().MergeBoards
            .TryGetValue(0, out StorageMergeBoard storageMergeBoard))
        {
            foreach (var item in storageMergeBoard.Items)
            {
                item.Id = -1;
            }
        }

        DebugCmdExecute.QuitApp();
    }

    [Category(categoryUI)]
    [DisplayName("显示气球")]
    public void DoMoveBalloon()
    {
        HideDebugPanel();
        MergeMainBalloon.Instance.Check(true);
    }

    private int ballPackId;
    [Category(categoryUI)]
    [DisplayName("气球礼包 ID")]
    public int BallPackID
    {
        get
        {
            return ballPackId;
        }
        set
        {
            ballPackId = value;
        }
    }
    
    [Category(categoryUI)]
    [DisplayName("显示气球礼包")]
    public void DoMoveBalloonPack()
    {
        HideDebugPanel();
        MergeMainBalloon.Instance.DebugShowPay(BallPackID);
    }

    
    [Category(categoryUI)]
    [DisplayName("清狗粮")]
    public void ClearDogCookies()
    {
        HideDebugPanel();
        MergeManager.Instance.RemoveAllItemByType(MergeItemType.dogCookies,MergeBoardEnum.Main,"Debug");
    }
    
    [Category(categoryUI)]
    [DisplayName("清香蕉")]
    public void ClearClimbTreeBanana()
    {
        HideDebugPanel();
        MergeManager.Instance.RemoveAllItemByType(MergeItemType.climbTreeBanana,MergeBoardEnum.Main,"Debug");
    }

    [Category(categoryUI)]
    [DisplayName("重置TM礼包链")]
    public void ResetTMGiftLink()
    {
        HideDebugPanel();
        StorageGiftBagLink _storageGiftBagLink = StorageManager.Instance.GetStorage<StorageHome>().TmGiftBagLink;
        _storageGiftBagLink.GiftBagLinkiIndexs[TMGiftBagLinkModel.Instance.StorageKey] = 0;
    }
    
    [Category(categoryUI)]
    [DisplayName("弹出奖励测试")]
    public void TestRewawrdTips()
    {
        List<ResData> resDatas = new List<ResData>();
        resDatas.Add(new ResData(102,10));
        resDatas.Add(new ResData(101204,1));
        RewardTipsManager.Instance.ShowRewardTips(Vector3.zero, resDatas);
    }
    [Category(categoryUI)]
    [DisplayName("拉取AdConfig")]
    public void UpdateRemoteConfig()
    {
        DragonPlus.ConfigHub.ConfigHubManager.Instance.UpdateRemoteConfig(true);
    }

    [Category(categoryUI)]
    [DisplayName("重置MasterCard")]
    public void ResetMasterCard()
    {
        HideDebugPanel();
        MasterCardModel.Instance.MasterCard.Clear();
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, "masterCard");
    }

    [Category(categoryUI)]
    [DisplayName("显示SaveProgress")]
    public void ShowUISaveProgress()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UISaveProgress, 9);
    }

    [Category(categoryUI)]
    [DisplayName("清理临时背包")]
    public void ClearTempBag()
    {
        HideDebugPanel();
        MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Rewards.Clear();
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
    }

    [Category(categoryUI)]
    [DisplayName("测试选档")]
    public void TestSeleclStorage()
    {
        HideDebugPanel();
        Profile ServerProfile = new Profile();
        ServerProfile.Json = StorageManager.Instance.ToJson();
        UIManager.Instance.OpenUI(UINameConst.UIPopupChooseProgress)
            .GetComponent<UIPopupChooseProgressController>().SetData(ServerProfile);
    }

    [Category(categoryUI)]
    [DisplayName("清闪购数据")]
    public void RefreshFlashShop()
    {
        StorageManager.Instance.GetStorage<StorageHome>().FlashSale.Clear();
        StorageManager.Instance.GetStorage<StorageHome>().PigSale.Clear();
    }

    [Category(categoryUI)]
    [DisplayName("清闪购礼包数据")]
    public void RefreshFlashPackShop()
    {
        StorageManager.Instance.GetStorage<StorageHome>().TaskAssistPackData.Clear();
    }

    [Category(categoryUI)]
    [DisplayName("清体力")]
    public void ClearEnergy()
    {
        EnergyModel.Instance.CostEnergy(EnergyModel.Instance.EnergyNumber(),
            new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug});
    }

    [Category(categoryUI)]
    [DisplayName("刷新购买体力，金币 ")]
    public void RefreshDailyShop()
    {
        StoreModel.Instance.RefreshDailyShop();
        HideDebugPanel();
    }
    [Category(categoryUI)]
    [DisplayName("注册推送 ")]
    public void RegistLocalNotifications()
    {
        NotificationManager.Instance.RegistLocalNotifications();
        HideDebugPanel();
    }
    [Category(categoryUI)]
    [DisplayName("测试推送 ")]
    public void TestLocalNotifications()
    {
        string TitleStr = "发送一条测试推送";

#if DEBUG || DEVELOPMENT_BUILD
        NotificationManager.Instance.UseDebug = true;
#endif
       
        DebugUtil.LogError ("MobileNotificationManager.inited->"+MobileNotificationManager.inited);
        MobileNotificationManager.SendNotification(99,10*1000,
                "title",
                "test !!!!", new Color32(0xff, 0x44, 0x44, 255));
        HideDebugPanel();
    }
    [Category(categoryUI)]
    [DisplayName("重置能量狂潮引导")]
    public void ResetEn()
    {
        CoinCompetitionModel.Instance.Clear();
       
        var guideIdList = new List<int>() {505, 506,500, 501,502};
        CleanGuideList(guideIdList);
    }
    [Category(categoryUI)]
    [DisplayName("强制服务器档界面")]
    public void OpenForceServerWindow()
    {
        CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
        {
            DescString =
                LocalizationManager.Instance.GetLocalizedString("&key.UI_profile_force_use_server_profile_desc"),
            OKCallback = () =>
            {
                // 覆盖存档,重新初始化
                SceneFsm.mInstance.ClientInited = false;
                SceneFsm.mInstance.BackToLogin();
                LocalizationManager.Instance.MatchLanguage();
            },
            HasCloseButton = false,
            IsLockSystemBack = true,
            IsHighSortingOrder = true
        });
        HideDebugPanel();
    }

    [Category(categoryUI)]
    [DisplayName("评论")]
    public void ShowRate()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UIPopupRateUs);
    }

    [Category(categoryUI)]
    [DisplayName("隐私协议")]
    public void ShowPrivacy()
    {
        HideDebugPanel();     
        
        UIManager.Instance.OpenUI("PrivacyPolicy", windowType:UIWindowType.Normal, windowLayer:UIWindowLayer.Max, type:typeof(PrivacyPolicyController), false);
    }
    [Category(categoryUI)]
    [DisplayName("能量狂潮重置")]
    public void ResetEnergyTorrent()
    {
        HideDebugPanel();
        EnergyTorrentModel.Instance.StorageEnergyTorrent.Clear();
    }
   [Category(categoryUI)]
    [DisplayName("Cg")]
    public void ShowCg()
    {
        HideDebugPanel();
       CGVideoManager.Instance.DebugPlayCG();
    }

    [Category(categoryUI)]
    [DisplayName("LikeUs")]
    public void ShowLikeUs()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UIPopupLikeUs);
    } 
    [Category(categoryUI)]
    [DisplayName("重置LikeUs")]
    public void ClearShowLikeUs()
    {
       var storageHome= StorageManager.Instance.GetStorage<StorageHome>();
       storageHome.LikeUsFinish = false;
    }


    [Category(categoryUI)]
    [DisplayName("完成所有教程")]
    public void CompleteGuide()
    {
        StorageManager.Instance.GetStorage<StorageHome>().IsCompleteAllGuide = true;
    }


    [Category(categoryUI)]
    [DisplayName("每日奖励")]
    public void DailyBonus()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UIPopupDaily);
    }
    [Category(categoryUI)]
    [DisplayName("拉取活动")]
    public void UpdateActivity()
    {
        ActivityManager.Instance.m_ActivityDataFetchedTime = -1000;
        ActivityManager.Instance.RequestActivityInfosFromServer();
    }
    [Category(categoryUI)]
    [DisplayName("DailyRv")]
    public void ShowDailyRv()
    {
        HideDebugPanel();
        UIDailyRVController.Open("Debug");
    }

    [Category(categoryUI)]
    [DisplayName("ResetDailyRv")]
    public void ResetDailyRv()
    {
        HideDebugPanel();
        DailyRVModel.Instance.DEBUG_RefreshRVShopList();
    } 
    [Category(categoryUI)]
    [DisplayName("清理海豹")]
    public void ReplaceSeal()
    {
        HideDebugPanel();
       MergeManager.Instance.ReplaceSeal(MergeBoardEnum.Main);
    }

    [Category(categoryUI)] 
    [DisplayName("清理海豚")]
    public void ReplaceDolphin()
    {
        HideDebugPanel();
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, "DolphinPack");

        DolphinPackModel.Instance.storageDolphinPack.Clear();
    }
    [Category(PayRebate)]
    [DisplayName("重置充值返利")]
    public void ResetPayRebate()
    {
        HideDebugPanel();
        PayRebateModel.Instance.StoragePayRebate.Clear();
    }
    [Category(PayRebate)]
    [DisplayName("重置三礼包")]
    public void ResetThreeGift()
    {
        HideDebugPanel();
        ThreeGift.ThreeGiftModel.Instance.StorageThreeGift.Clear();
    }
    
    [Category(categoryUI)]
    [DisplayName("是否跳过活动预热")]
    public bool SkipActivityPreheating
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().SkipActivityPreheating;
        }
        set
        {
            StorageManager.Instance.GetStorage<StorageHome>().SkipActivityPreheating = value;
        }
    }
    
    [Category(categoryUI)]
    [DisplayName("重置所有弹窗冷却时间")]
    public void RestAllCoolTime()
    {
        StorageManager.Instance.GetStorage<StorageHome>().CoolTimeData.IntervalTime.Clear();
        StorageManager.Instance.GetStorage<StorageHome>().CoolTimeData.LossTime.Clear();
        StorageManager.Instance.GetStorage<StorageHome>().CoolTimeData.OtherDay.Clear();
    }
    
        
    [Category(Adapt)]
    [DisplayName("NONE")]
    public void AdaptTest()
    {
    }
    
        
    [Category(PayRebate)]
    [DisplayName("重置三礼包2")]
    public void ResetMultipleGift()
    {
        HideDebugPanel();
        MultipleGift.MultipleGiftModel.Instance.StorageMultipleGift.Clear();
    }
    
    [Category(PayRebate)]
    [DisplayName("重置充值返利(本地)")]
    public void ResetPayRebateLocal()
    {
        HideDebugPanel();
        PayRebateLocalModel.Instance.StoragePayRebate.Clear();
    }
    [Category(PayRebate)]
    [DisplayName("EnergyPackage")]
    public void ResetEnergyPackage()
    {
        HideDebugPanel();
        EnergyPackageModel.Instance.StorageEnergyPackage.Clear();
    }
    
    [Category(PayRebate)]
    [DisplayName("EnergyPackage")]
    public void EnergyPackageUI()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyBuy);
    }
    
    
    [Category("Common")]
    [DisplayName("保存动态图集到电脑")]
    public void saveRuntimeAtlas()
    {

        var textureCount = 0;
        void exportFile(Dictionary<int, RuntimeAtlas> kvDic)
        {
            foreach (var kv in kvDic)
            {
                var textureList = kv.Value.TextureList;
                if (textureList != null && textureList.Count > 0)
                {
                    for (var index = 0; index < textureList.Count; index++)
                    {
                        var tex = textureList[index];
                        var data = tex.EncodeToPNG();

                        using (var file = System.IO.File.Open($"/Users/shouyu.liu/Downloads/RuntimeAtlas_{textureCount++}.png", FileMode.Create))
                        {
                            using (var writer = new BinaryWriter(file))
                            {
                                writer.Write(data);
                            }
                        }
                    }
                }
            }
        }
        
        exportFile(RuntimeAtlasManager.Instance._bilinearFilterAtlasDic);
    }
}