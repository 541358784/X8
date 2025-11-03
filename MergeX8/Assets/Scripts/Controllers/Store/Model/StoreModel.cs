using System;
using System.Collections.Generic;
using Activity.GardenTreasure.Model;
using Activity.LimitTimeOrder;
using Activity.LuckyGoldenEgg;
using Activity.TimeOrder;
using Activity.TotalRecharge;
using Activity.TreasureHuntModel;
using ActivityLocal.ClimbTower.Model;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Network.BI;
using DragonU3DSDK.Storage;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using Merge.Order;
using OptionalGift;
using TMatch;
using TotalRecharge_New;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPBIPara
{
    public BiEventCommon.Types.CommonMonetizationAdEventPlacement placement =
        BiEventCommon.Types.CommonMonetizationAdEventPlacement.CommonMonetizationEventPlacementNone;

    public String data1 = "";
    public String data2 = "";
    public String data3 = "";
    public String productId = "";
    public String transactionId = "";
}

public partial class StoreModel : Manager<StoreModel>
{
    public enum eProductType
    {
        None,
        Diamond = 0,
        Bundle = 1,
        IceBreakPack = 2,
        IceBreakPackSecond = 21,
        DailyPack = 3,
        PigBank = 4,
        TaskAssistBundle = 5,
        Gold = 111,
        GarageCleanBundle = 12,
        RvShop = 7,
        BuyResources = 25,
        GiftBagLink = 8,
        MasterCard = 26,
        SealPack = 9,
        EasterPack = 10,
        EasterGift = 11,
        BattlePass = 13,
        MermaidExtend = 14,
        DolphinPack = 15,
        EnergyPack = 16,
        BallPack = 17,
        MysteryPack = 18,
        TMatchBundle = 32,
        BattlePass_2 = 19, //bp 二期
        TmGiftBagLink = 36, //Tm 礼包链
        HappyBundle = 30, //hg bundle
        HappyBundleExtend = 31, //hg bundle
        SummerWatermelonBreadPackage = 37,//蓝莓礼包
        NewDailyPack = 38,//新每日礼包
        ThreeGift = 39,//
        SummerWatermelonBread = 40,//西瓜礼包
        MultipleGift = 41,//三连礼包2
        TMBP = 42,
        GiftBagBuyBetter = 43,//越买越划算礼包
        GiftBagSendOne = 44,//买一赠一礼包
        LevelUpPackage = 45,//升级礼包
        ThemeDecoration = 46,//主题装修延时
        WeekCard = 47,//主题装修延时
        KeepPetGift = 48,//牛排礼包
        TreasureHuntGift = 49,
        GiftBagProgress = 50,//进步礼包
        OptionalGift = 51,//自选礼包
        ButterflyWorkShop = 52,//蝴蝶工坊
        GiftBagDouble = 53,//俩礼包
        GardenTreasure = 54,//花园礼包 
        MixMaster = 55,//调制大师礼包
        TurtlePang = 56,//乌龟对对碰礼包
        Zuma = 57,//乌龟对对碰礼包
        KeepPetThreeOneGift = 58,//牛排礼包
        Kapibala = 59,//卡皮巴拉
        Screw = 60,//钉子
        ChristmasBlindBox=61,//圣诞盲盒礼包
        KapiScrew = 62,//卡皮钉子
        NewIceBreakGiftBag = 63,//新破冰礼包
        KapiTile = 64,//卡皮钉子
        BiuBiu = 65,//飞镖
        NewNewIceBreakPack = 66,//新新破冰礼包
        ClimbTower = 67,//爬塔
        LuckyGoldenEgg = 68,//幸运金蛋
        TimeOrderGift = 69,//TimeOrderGift
        GiftBagSendTwo = 70,//送俩
        GiftBagSendThree = 71,//送仨
        TimeLimitOrderGift = 72,//TimeLimitOrderGift
        GiftBagSend4 = 73,//送4
        GiftBagSend6 = 74,//送6
        PillowWheel = 75,//枕头转盘礼包
        NoAdsGiftBag = 76,//去广告礼包
    }

    public class BIIAPExtra
    {
        public string data1 = "";
        public string data2 = "";

        public String data3 = "";

        // public MonetizationIAPEventPlacement placement = MonetizationIAPEventPlacement.PlacementUnknown;
        public String productId = "";
        public String transactionId = "";
    }

    /// <summary>
    /// 购买商品成功回调 [productType, callbackList] <para>
    /// -1 = 所有类型</para>
    /// </summary>
    private readonly Dictionary<int, List<Action<TableShop, string>>> m_onGetIPAReward =
        new Dictionary<int, List<Action<TableShop, string>>>();

    /// <summary>
    /// 购买商品失败回调 [productType, callbackList] <para>
    /// -1 = 所有类型</para>
    /// </summary>
    private readonly Dictionary<int, List<Action<TableShop, string>>> m_onIPARewardFailed =
        new Dictionary<int, List<Action<TableShop, string>>>();

    // 是否处于支付中，防止局内触发支付导致弹出暂停弹框
    public bool IsPaying { private set; get; }

    public bool isDebugPay = false;
    public string openSrc = "";
    public int PackID = 0;

    private void InvokeIPARewardCallback(bool success, int productType, TableShop shopConfig,
        string transactionID)
    {
        if (success)
        {
            PlayerPrefs.SetInt("RFMWeight", PlayerPrefs.GetInt("RFMWeight", 0) + 1);
        }

        var s = success ? m_onGetIPAReward : m_onIPARewardFailed;
        if (!s.TryGetValue(productType, out var callbacks)) return;

        foreach (var callbak in callbacks)
        {
            try
            {
                callbak(shopConfig, transactionID);
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }
    }

    /// <summary>
    /// 添加特定商品类型购买成功回调<para>
    /// 特别当 producType = -1 时，表示所有商品</para>
    /// </summary>
    /// <param name="callback">回调</param>
    /// <param name="productType">商品类型 默认所有商品</param>
    public void AddOnGetIPARewardCallback(System.Action<TableShop, string> callback, int productType = -1)
    {
        if (callback == null) return;
        if (!m_onGetIPAReward.TryGetValue(productType, out var callbacks))
        {
            callbacks = new List<System.Action<TableShop, string>>();
            m_onGetIPAReward.Add(productType, callbacks);
        }

        callbacks.Remove(callback);
        callbacks.Add(callback);
    }

    /// <summary>
    /// 添加特定商品类型购买失败回调<para>
    /// 特别当 producType = -1 时，表示所有商品</para>
    /// </summary>
    /// <param name="callback">回调</param>
    /// <param name="productType">商品类型 默认所有商品</param>
    public void AddOnIPARewardFailedCallback(Action<TableShop, string> callback, int productType = -1)
    {
        if (callback == null) return;
        if (!m_onIPARewardFailed.TryGetValue(productType, out var callbacks))
        {
            callbacks = new List<Action<TableShop, string>>();
            m_onIPARewardFailed.Add(productType, callbacks);
        }

        callbacks.Remove(callback);
        callbacks.Add(callback);
    }

    public TableShop GetShopConfigById(int shopConfigId)
    {
        return GlobalConfigManager.Instance.GetTableShopByID(shopConfigId);
    }

    public IAPBIPara m_IAPBIPara = new IAPBIPara();
    private object purchaseParam1;
    public void Purchase(int purchaseId, string src = "", int packId = 0,object param1 = null)
    {
        purchaseParam1 = param1;
        openSrc = src;
        IsPaying = true;
        PackID = packId;
        var shopConfig = GlobalConfigManager.Instance.GetTableShopByID(purchaseId);

        if (ConfigurationController.Instance.version == VersionStatus.DEBUG && isDebugPay)
        {
            GetIPAReward(shopConfig, null);
            return;
        }

        string productId = "";
#if UNITY_IOS
        productId = shopConfig.product_id_ios;
#elif UNITY_ANDROID
        productId = shopConfig.product_id;
#endif
        var products = Dlugin.SDK.GetInstance().iapManager.GetAllProductInfo();
        if (products == null || products.Length <= 0)
        {
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
#if UNITY_ANDROID
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_cannot_connect_to_google_play")
#elif UNITY_IOS
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_cannot_connect_to_itunes_store")
#else
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_purchase_failed")
#endif
            });
            return;
        }

        if (purchaseId <= 0)
        {
            return;
        }

        //foreach (var p in products)
        //{
        //    DebugUtil.Log(" 商品 : " + p.definition.id + "  ,hasReceipt :" + p.hasReceipt + "  ,receipt: " + p.receipt + " , transactionID : " + p.transactionID);
        //}

        WaitingManager.Instance.OpenWindow();

        
        AppIconChangerSystem.Instance.IsPaying = true;
#if UNITY_ANDROID
        if (Dlugin.SDK.GetInstance().iapManager.IsProductAlreadyOwned(productId))
        {
            // 该商品有未完成订单，尝试一次补单
            DebugUtil.Log("该商品为未完成状态，尝试补单，取消本次付费行为!");
            RequestUnfulfilledPaymentsAndTryVerify(productId);
            return;
        }
#endif

        //用户点击付费内购按钮

        BIManager.Instance.SendCommonMonetizationIAPEvent(
            BiEventCommon.Types.CommonMonetizationIAPEventType.CommonMonetizationEventIapPurchaseClick
            , m_IAPBIPara.placement.ToString()
            , BiEventCommon.Types.CommonMonetizationIAPEventFailedReason.CommonMonetizationEventReasonIapNone,
            m_IAPBIPara.productId, "", m_IAPBIPara.data1, m_IAPBIPara.data2, m_IAPBIPara.data3,
            shopConfig.id.ToString());
        AdLogicManager.Instance.specialOrder = true;
        Dlugin.SDK.GetInstance().iapManager.PurchaseProduct(productId, OnPurchased, "", purchaseId.ToString());

        DebugUtil.Log("Purchase Id : " + purchaseId);
    }

    public void GetIPAReward(TableShop cfg, string transID)
    {
        if (cfg == null)
            return;
        
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventIapShopId, cfg.id.ToString(),cfg.price.ToString());

        VipStoreModel.Instance.Purchase( VipStoreModel.Instance.GetVipScore(cfg.price));
        PayRebateModel.Instance.OnPurchase();
        PayRebateLocalModel.Instance.OnPurchase();
        InvokeIPARewardCallback(true, cfg.productType, cfg, transID);
        InvokeIPARewardCallback(true, -1, cfg, transID);
        eProductType ptype = (eProductType) cfg.productType;
        PayLevelModel.Instance.OnPaySuccess(cfg);
        AdLocalConfigHandle.Instance.OnPaySuccess(cfg);
        ForRewards(cfg);

        IsPaying = false;
        // TotalRechargeModel.Instance.OnPaySuccess(cfg);
        //TotalRechargeModel_New.Instance.OnPaySuccess(cfg);
        float payMax = StorageManager.Instance.GetStorage<StorageHome>().PayMaxAmount;
        StorageManager.Instance.GetStorage<StorageHome>().PayMaxAmount = payMax > cfg.price ? payMax : cfg.price;
        EventDispatcher.Instance.DispatchEvent(EventEnum.OnIAPItemPaid, cfg.id);

    }

    private void ForRewards(TableShop cfg)
    {
        eProductType ptype = (eProductType) cfg.productType;
        Transform endTrans = null;
        var reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap;
        List<ResData> listResData = new List<ResData>();
        var extraReward = ShopExtraRewardModel.Instance.GetExtraReward(cfg.id);
        if (extraReward.Count > 0)
        {
            ShopExtraRewardModel.Instance.ReceiveExtraReward(cfg.id);
        }
        switch (ptype)
        {
            case eProductType.Diamond:
            {
                var ticketRewards = BuyDiamondTicketModel.Instance.PurchaseDiamond(cfg.id);
                var extras = new Dictionary<string, string>();
                extras.Clear();
                extras.Add("type", "iap");
                extras.Add("amount", cfg.price.ToString());
                if (MasterCardModel.Instance.IsBuyMasterCard && MasterCardModel.Instance.GetPayDouble() > 0)
                {
                    ResData resData = new ResData((int) UserData.ResourceId.Diamond, cfg.amount);
                    ResData resDataExtra = new ResData((int) UserData.ResourceId.Diamond,
                        cfg.amount * MasterCardModel.Instance.GetPayDoublePre() / 100, true);
                    listResData.Add(resData);
                    listResData.Add(resDataExtra);
                    EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, listResData);
                    MasterCardModel.Instance.SetPayDouble(MasterCardModel.Instance.GetPayDouble() - 1);
                    if (extraReward.Count > 0)
                    {
                        UserData.Instance.AddRes(extraReward,
                            new GameBIManager.ItemChangeReasonArgs()
                            {
                                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopExtraRewardGet
                            });
                        var diamondReward = extraReward.Find(a => a.id == (int) UserData.ResourceId.Diamond);
                        if (diamondReward != null)
                        {
                            var flyCount = diamondReward.count;
                            FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(),
                                UserData.ResourceId.Diamond, flyCount, nowItemPos, 0.8f, true, true, 0.15f, () =>
                                {
                                    PayRebateModel.Instance.OnPurchaseAniFinish();
                                    PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                                });
                        }
                        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, extraReward);

                    }
                    if (ticketRewards!=null)
                    {
                        UserData.Instance.AddRes(ticketRewards,
                            new GameBIManager.ItemChangeReasonArgs()
                            {
                                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.BuyDiamondTicketGet
                            });
                        var diamondReward = ticketRewards.Find(a => a.id == (int) UserData.ResourceId.Diamond);
                        if (diamondReward != null)
                        {
                            var flyCount = diamondReward.count;
                            FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(),
                                UserData.ResourceId.Diamond, flyCount, nowItemPos, 0.8f, true, true, 0.15f, () =>
                                {
                                    PayRebateModel.Instance.OnPurchaseAniFinish();
                                    PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                                });
                        }
                    }

                    EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ticketRewards);
                }
                else
                {
                    UserData.Instance.AddRes((int) UserData.ResourceId.Diamond, cfg.amount,
                        new GameBIManager.ItemChangeReasonArgs()
                        {
                            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap,
                            data1 = cfg.id.ToString(),
                        }, false);
                    ResData resData = new ResData((int) UserData.ResourceId.Diamond, cfg.amount);
                    List<ResData> resDatas = new List<ResData>();
                    resDatas.Add(resData);
                    EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, resDatas);

                    var flyCount = cfg.amount;
                    if (extraReward.Count > 0)
                    {
                        UserData.Instance.AddRes(extraReward,
                            new GameBIManager.ItemChangeReasonArgs()
                            {
                                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopExtraRewardGet
                            });
                        var diamondReward = extraReward.Find(a => a.id == (int) UserData.ResourceId.Diamond);
                        if (diamondReward != null)
                        {
                            flyCount += diamondReward.count;
                        }
                        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, extraReward);

                    }
                    if (ticketRewards!=null)
                    {
                        UserData.Instance.AddRes(ticketRewards,
                            new GameBIManager.ItemChangeReasonArgs()
                            {
                                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.BuyDiamondTicketGet
                            });
                        var diamondReward = ticketRewards.Find(a => a.id == (int) UserData.ResourceId.Diamond);
                        if (diamondReward != null)
                        {
                            flyCount += diamondReward.count;
                        }
                        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ticketRewards);

                    }
                    FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(),
                        UserData.ResourceId.Diamond, flyCount, nowItemPos, 0.8f, true, true, 0.15f, () =>
                        {
                            PayRebateModel.Instance.OnPurchaseAniFinish();
                            PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                        });
                    
                }

                AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Shop,AdLocalOperate.Operate);
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "diamond", extras);
                break;
            }
            case eProductType.GarageCleanBundle:
            {
                InitBundles(cfg.id, ref listResData,BiEventAdventureIslandMerge.Types.ItemChangeReason.FishBoosts,BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonFishBoosts);
                GarageCleanupModel.Instance.BuyCleanupPack();
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFishBoostsSuccess);
                break;
            }
            case eProductType.Bundle:
            {
                InitBundles(cfg.id, ref listResData,BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap,BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemChangeIapBundle);
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "bundle", extras2);
                break;
            } 
            
            case eProductType.IceBreakPack:
            {
                var bundleCfg = AdConfigHandle.Instance.GetIceBreakPackByShopID(cfg.id);
                if (bundleCfg == null)
                    return;

                if (bundleCfg.Content == null)
                    return;

                if (bundleCfg.Count == null)
                    return;

                for (int i = 0; i < bundleCfg.Content.Count; ++i)
                {
                    int id = bundleCfg.Content[i];
                    int count = bundleCfg.Count[i];
                    ResData resData = new ResData(id, count);
                    listResData.Add(resData);
                    if (!UserData.Instance.IsResource(id))
                    {
                        TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig(id);
                        if (mergeItemConfig != null)
                        {
                            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                            {
                                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonIceBreakerBuy,
                                itemAId = mergeItemConfig.id,
                                ItemALevel = mergeItemConfig.level,
                                isChange = true,
                            });
                        }
                    }
                }

                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.IceBreakerBuy;
                IcebreakingPackModel.Instance.RecordBuyItem(cfg.id);
                break;
            }
            case eProductType.IceBreakPackSecond:
            {
                var bundleCfg = AdConfigHandle.Instance.GetIceBreakSecondPackByShopID(cfg.id);
                if (bundleCfg == null)
                    return;

                if (bundleCfg.Content == null)
                    return;

                if (bundleCfg.Count == null)
                    return;

                for (int i = 0; i < bundleCfg.Content.Count; ++i)
                {
                    int id = bundleCfg.Content[i];
                    int count = bundleCfg.Count[i];
                    ResData resData = new ResData(id, count);
                    listResData.Add(resData);
                    if (!UserData.Instance.IsResource(id))
                    {
                        TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig(id);
                        if (mergeItemConfig != null)
                        {
                            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                            {
                                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonIceBreakerBuy,
                                itemAId = mergeItemConfig.id,
                                ItemALevel = mergeItemConfig.level,
                                isChange = true,
                            });
                        }
                    }
                }

                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.IceBreakerBuy;
                IcebreakingPackSecondModel.Instance.RecordBuyItem(cfg.id);
                break;
            }
            case eProductType.TaskAssistBundle:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                extras2.Add("packID", PackID.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "MasterCard", extras2);

                TaskAssistPackModel.Instance.PurchaseSuccess(cfg, PackID);
                break;
            }
            case eProductType.RvShop:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "rvshop", extras2);

                DailyRVModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.BuyResources:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "buyResources", extras2);

                BuyResourceManager.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.GiftBagLink:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "GiftGagLink", extras2);

                GiftBagLinkModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.PigBank:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "PigBank", extras2);

                PigBankModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.DailyPack:
            {
          
                DailyPackModel.Instance.PurchaseSuccess(cfg,openSrc);
                break;
            }
            case eProductType.HappyBundle:
            {
                HappyGoPackModel.Instance.PurchaseSuccess(cfg);
                break;
            }case eProductType.HappyBundleExtend:
            {
                HappyGoModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.MysteryPack:
            {
               UIPopupMysteryGiftController.PurchaseSuccess();
                
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "MySteryPack", extras2);

                break;
            }    
            case eProductType.BallPack:
            {
               UIPopupLuckyBalloonController.PurchaseSuccess();
                
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "BalloonPack", extras2);

                break;
                
            }
            case eProductType.EasterGift:
            {
          
                EasterGiftModel.Instance.PurchaseSuccess(cfg,openSrc);
                break;
            }
            case eProductType.MasterCard:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "MasterCard", extras2);

                MasterCardModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.SealPack:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSealPackageSuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "seal", extras2);

                SealPackModel.Instance.PurchaseSuccess();
                break;
            }   
            case eProductType.DolphinPack:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDolphinPackageSuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "dolphin", extras2);

                DolphinPackModel.Instance.PurchaseSuccess();
                break;
            }         
            case eProductType.EnergyPack:
            {
        
                EnergyPackageModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.EasterPack:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterPackageSuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "easter", extras2);

                EasterPackModel.Instance.PurchaseSuccess();
                break;
            }

            case eProductType.BattlePass:
            {
                // var extras2 = new Dictionary<string, string>();
                // extras2.Clear();
                // extras2.Add("type","iap");       
                // extras2.Add("amount",cfg.price.ToString());
                // GameBIManager.Instance.SendGameEvent(BiEventMergeX2.Types.GameEventType.GameEventStoreBuySuccess, openSrc,TaskModuleManager.Instance.GetCurMaxTaskID().ToString(),"BattlePass",extras2);

                Activity.BattlePass.BattlePassModel.Instance.PurchaseSuccess(cfg);

                break;
            }
            case eProductType.BattlePass_2:
            {
                Activity.BattlePass_2.BattlePassModel.Instance.PurchaseSuccess(cfg);
             break;   
            }
            case eProductType.MermaidExtend:
            {
                MermaidModel.Instance.PurchaseSuccess();
                break;
            }
            case eProductType.TMatchBundle:
            {
                TMatch.TMatchModel.Instance.OnPurchase(cfg,  purchaseParam1 as Transform,false);
                break;
            }
            case eProductType.TMBP:
            {
                TMatch.TMBPModel.Instance.OnPurchase(cfg, false);
                break;
            }
            case eProductType.TmGiftBagLink:
            {
                TMGiftBagLinkModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.SummerWatermelonBreadPackage:
            {
                SummerWatermelonBreadModel.Instance.PurchasePackageSuccess(cfg);
                break;
            }
            case eProductType.SummerWatermelonBread:
            {
                SummerWatermelonModel.Instance.PurchasePackageSuccess(cfg);
                break;
            }
            case eProductType.NewDailyPack:
            {
                NewDailyPackModel.Instance.PurchaseSuccess(cfg,openSrc);
                break;
            } 
            case eProductType.ThreeGift:
            {
                ThreeGift.ThreeGiftModel.Instance.PurchaseSuccess(cfg.id);
                break;
            }
            case eProductType.MultipleGift:
            {
                MultipleGift.MultipleGiftModel.Instance.PurchaseSuccess(cfg.id);
                break;
            }
            case eProductType.GiftBagBuyBetter:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "GiftGagBuyBetter", extras2);
                GiftBagBuyBetterModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.GiftBagSendOne:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "GiftGagSendOne", extras2);
                GiftBagSendOneModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.LevelUpPackage:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "LevelUpPackage", extras2);
                LevelUpPackageModel.Instance.PurchaseSuccess(cfg,purchaseParam1 as StorageLevelUpPackageSinglePackage);
                break;
            }
            case eProductType.ThemeDecoration:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "ThemeDecorationMoreTime", extras2);
                ThemeDecorationModel.Instance.PurchaseSuccess(cfg);
                break;
            }     
            case eProductType.WeekCard:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "ThemeDecorationMoreTime", extras2);
                WeeklyCardModel.Instance.PurchaseSuccess(cfg);
                break;
            }
           case eProductType.KeepPetGift:
            {
                KeepPetModel.Instance.PurchaseSuccess(cfg);
                break;
            }    
            case eProductType.KeepPetThreeOneGift:
            {
                KeepPetModel.Instance.PurchaseThreeOneSuccess(cfg);
                break;
            }    
            case eProductType.LuckyGoldenEgg:
            {
                LuckyGoldenEggModel.Instance.PurchaseSuccess(cfg);
                break;
            }   
            case eProductType.TimeOrderGift:
            {
                TimeOrderModel.Instance.PurchaseSuccess(cfg.id, purchaseParam1 == null ? -1 : (int)purchaseParam1);
                break;
            }   
            case eProductType.TimeLimitOrderGift:
            {
                LimitTimeOrderModel.Instance.PurchaseSuccess(cfg.id, purchaseParam1 == null ? -1 : (int)purchaseParam1);
                break;
            }   
           case eProductType.TreasureHuntGift:
            {
                TreasureHuntModel.Instance.PurchaseSuccess(cfg);
                break;
            }   
            case eProductType.OptionalGift:
            {
                OptionGiftModel.Instance.PurchaseSuccess(cfg);
                break;
            }   
            case eProductType.ButterflyWorkShop:
            {
                ButterflyWorkShopModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.GiftBagProgress:
            {
                GiftBagProgressModel.Instance.OnPurchase(cfg);
                break;
            }
            case eProductType.GiftBagDouble:
            {
                GiftBagDoubleModel.Instance.OnPurchase(cfg);
                break;
            }
            case eProductType.GardenTreasure:
            {
                GardenTreasureModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.MixMaster:
            {
                MixMasterModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.TurtlePang:
            {
                TurtlePangModel.Instance.OnPurchase(cfg);
                break;
            }
            case eProductType.Zuma:
            {
                ZumaModel.Instance.OnPurchase(cfg);
                break;
            }
            case eProductType.Kapibala:
            {
                KapibalaModel.Instance.OnPurchase(cfg);
                break;
            }
            case eProductType.KapiScrew:
            {
                KapiScrewModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.Screw:
            {
                Screw.StoreModel.Instance.OnPurchase(cfg);
                break;
            }
            case eProductType.ChristmasBlindBox:
            {
                ChristmasBlindBoxModel.Instance.OnPurchase(cfg);
                break;
            }
            case eProductType.NewIceBreakGiftBag:
            {
                NewIceBreakGiftBagModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.NewNewIceBreakPack:
            {
                NewNewIceBreakPackModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.ClimbTower:
            {
                ClimbTowerModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.KapiTile:
            {
                KapiTileModel.Instance.OnPurchase(cfg);
                break;
            }
            case eProductType.BiuBiu:
            {
                BiuBiuModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.GiftBagSendTwo:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "GiftGagSendTwo", extras2);
                GiftBagSendTwoModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.GiftBagSendThree:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "GiftGagSendThree", extras2);
                GiftBagSendThreeModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.GiftBagSend4:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "GiftGagSend4", extras2);
                GiftBagSend4Model.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.GiftBagSend6:
            {
                var extras2 = new Dictionary<string, string>();
                extras2.Clear();
                extras2.Add("type", "iap");
                extras2.Add("amount", cfg.price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    openSrc, MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "GiftGagSend6", extras2);
                GiftBagSend6Model.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.PillowWheel:
            {
                PillowWheelModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            case eProductType.NoAdsGiftBag:
            {
                NoAdsGiftBagModel.Instance.PurchaseSuccess(cfg);
                break;
            }
            default:
                break;
        }

        if (listResData == null || listResData.Count == 0)
            return;
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, listResData);
        CommonRewardManager.Instance.PopCommonReward(listResData,
            CurrencyGroupManager.Instance.GetCurrencyUseController(), true, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = reason,
            }, () =>
            {
                PayRebateModel.Instance.OnPurchaseAniFinish();
                PayRebateLocalModel.Instance.OnPurchaseAniFinish();
            });
    }

    private void InitBundles(int shopId, ref List<ResData> listResData,
        BiEventAdventureIslandMerge.Types.ItemChangeReason reason,BiEventAdventureIslandMerge.Types.MergeEventType mergeReson)
    {
        TableBundle bundleCfg = GlobalConfigManager.Instance.GetTableBundleByShopID(shopId);
        if (bundleCfg == null)
            return;

        if (bundleCfg.bundleItemList == null)
            return;

        if (bundleCfg.bundleItemCountList == null)
            return;

        for (int i = 0; i < bundleCfg.bundleItemList.Length; ++i)
        {
            int id = bundleCfg.bundleItemList[i];
            int count = bundleCfg.bundleItemCountList[i];
            ResData resData = new ResData(id, count);
            listResData.Add(resData);
        }

        if (bundleCfg == null)
        {
            return;
        }

        List<int> itemIds = new List<int>(bundleCfg.bundleItemList);
        if (itemIds == null)
        {
            return;
        }

        List<int> itemCounts = new List<int>(bundleCfg.bundleItemCountList);
        if (itemCounts == null)
        {
            return;
        }

        List<int> itemIdsType = new List<int>(bundleCfg.bundleItemType);
        for (int i = 0; i < itemIds.Count; ++i)
        {
            int id = itemIds[i];
            int count = itemCounts[i];
            if (itemIdsType[i] == 1)
            {
                GameBIManager.ItemChangeReasonArgs reasonArgs = new GameBIManager.ItemChangeReasonArgs();
                reasonArgs.reason = reason;
            }
            else if (itemIdsType[i] == 2)
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(id);
                if (itemConfig == null)
                {
                    DebugUtil.LogError("----------商店表配置错误------------------ID->" + id);
                    continue;
                }
                
                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                {
                    MergeEventType = mergeReson,
                    itemAId = itemConfig.id,
                    ItemALevel = itemConfig.level,
                    isChange = true,
                    extras = new Dictionary<string, string>
                    {
                    }
                });
            }
        }
    }

    //false, productId, null, PurchaseFailureReason.DuplicateTransaction
    public void OnPurchased(bool success, string productId, UnityEngine.Purchasing.Product product,
        UnityEngine.Purchasing.PurchaseFailureReason failureReason)
    {
        WaitingManager.Instance.CloseWindow();
        if (string.IsNullOrEmpty(productId) || product == null)
            return;

        // WaitingManager.Instance.CloseUI();
        m_IAPBIPara.productId = productId;
        m_IAPBIPara.transactionId = product.transactionID;
        var storageHome = DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageHome>();
        var storageGame = DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageGame>();
        m_IAPBIPara.data1 = storageHome?.CurRoomId.ToString();
        m_IAPBIPara.data2 = storageGame?.TaskGroups.CompleteOrderNum.ToString();
        m_IAPBIPara.data3 = AdConfigHandle.Instance.GetCommonID().ToString();
        TableShop shopConfig = null;
        // 尝试使用发起支付时传入的shop id来获取当前商品信息
        var userData = Dlugin.SDK.GetInstance().iapManager.GetUserData(productId);
        if (!string.IsNullOrEmpty(userData) && int.TryParse(userData, out var shopId))
            shopConfig = GlobalConfigManager.Instance.GetTableShopByID(shopId);

        // 若没有传入shop id，或者有错误，则使用product id来匹配
        if (shopConfig == null)
        {
            shopConfig = GlobalConfigManager.Instance.GetTableShop().Find((sc) =>
            {
#if UNITY_IOS
                return productId == sc.product_id_ios;
#else
                return productId == sc.product_id;
#endif
            });
        }

        if (shopConfig == null)
        {
            DebugUtil.LogWarning("shop Config not found in OnPurchased");
            return;
        }

        if (success && product != null)
        {
            GetIPAReward(shopConfig, product.transactionID);
            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
            BIManager.Instance.SendCommonMonetizationIAPEvent(
                BiEventCommon.Types.CommonMonetizationIAPEventType.CommonMonetizationEventIapPurchaseSuccess
                , m_IAPBIPara.placement.ToString(), new BiEventCommon.Types.CommonMonetizationIAPEventFailedReason(),
                m_IAPBIPara.productId, "", m_IAPBIPara.data1, m_IAPBIPara.data2, m_IAPBIPara.data3,
                shopConfig.id.ToString(), storageCommon.RevenueCount.ToString());
            GameBIManager.Instance.SendPurchaseCompleteThirdBI(shopConfig.price);

        }
        else
        {
            DebugUtil.Log("OnPurchased failed with reason = " + failureReason.ToString());

            if (failureReason == PurchaseFailureReason.UserCancelled)
            {
                string text = LocalizationManager.Instance.GetLocalizedString($"UI_purchase_failed_desc_05");
                DebugUtil.LogError($"PurchaseFailedController: [:{text}]");
                CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                {
                    DescString = text,
                });
            }

            var reason = BiEventCommon.Types.CommonMonetizationIAPEventFailedReason
                .CommonMonetizationEventReasonIapVerifyFailed;
            if (failureReason == PurchaseFailureReason.UserCancelled)
            {
                reason = BiEventCommon.Types.CommonMonetizationIAPEventFailedReason
                    .CommonMonetizationEventReasonIapCancel;
            }

            BIManager.Instance.SendCommonMonetizationIAPEvent(
                BiEventCommon.Types.CommonMonetizationIAPEventType.CommonMonetizationEventIapPurchaseNotSuccess
                , m_IAPBIPara.placement.ToString(),
                reason, m_IAPBIPara.productId, "", m_IAPBIPara.data1, m_IAPBIPara.data2, m_IAPBIPara.data3,
                shopConfig.id.ToString());
            InvokeIPARewardCallback(false, shopConfig.productType, shopConfig, product?.transactionID);
            InvokeIPARewardCallback(false, -1, shopConfig, product?.transactionID);
        }
    }

    public string GetProductId(TableShop shopConfig)
    {
#if UNITY_IOS
        return shopConfig.product_id_ios;
#else
        return shopConfig.product_id;
#endif
    }

    // 补单处理
    public void CheckVerifyUnfulfilledPayments()
    {
        DebugUtil.Log("--------------CheckVerifyUnfulfilledPayments----------------------");
        RequestUnfulfilledPaymentsAndTryVerify();
    }

    public void RequestUnfulfilledPaymentsAndTryVerify(string checkProductId = "")
    {
        APIManager.Instance.Send(new CListUnfulfilledPayments(),
            (Google.Protobuf.IMessage obj) =>
            {
                DebugUtil.Log("CListUnfulfilledPayments success !");
                var payments = (obj as SListUnfulfilledPayments);
                foreach (var payment in payments.Payments)
                {
                    Dlugin.SDK.GetInstance().iapManager.SetUnfulfilledPaymentId(payment.ProductId, payment.PaymentId);
                }

                Dlugin.SDK.GetInstance().iapManager.VerifyUnfulfilledPayment(OnPurchased, checkProductId);
            },
            (arg1, arg2, arg3) =>
            {
                DebugUtil.Log("CListUnfulfilledPayments error : " + arg1 + "  " + arg2 + "  " + arg3);
            });
    }

    /// 恢复购买
    /// </summary>
    public void RestorePurchase()
    {
        if (!Dlugin.SDK.GetInstance().iapManager.IsInitialized())
            return;

        Dlugin.SDK.GetInstance().iapManager.RestorePurchases(productIds =>
        {
            DebugUtil.Log($"RestorePurchases count {productIds.Count}");
            foreach (var obj in productIds) DebugUtil.Log($"Restored {obj}");

            if (productIds.Count == 0)
            {
                CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                {
                    DescString =
                        LocalizationManager.Instance.GetLocalizedString(
                            "&key.UI_store_common_restore_purchase_notice1"),
                    OKButtonText = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_ok")
                });
                return;
            }

            foreach (var productId in productIds)
            {
                TableShop shopConfig = GlobalConfigManager.Instance.GetTableShop().Find((sc) =>
                {
#if UNITY_IOS
                    return productId == sc.product_id_ios;
#else
                    return productId == sc.product_id;
#endif
                });

                if (shopConfig == null)
                {
                    DebugUtil.Log($"Can't find purchase item for pid {productId}");
                }
                else
                {
                    if (shopConfig.productType.Equals((int) StoreProductType.NoAds))
                    {
                        StorageManager.Instance.GetStorage<StorageHome>().ShopData.GotNoAds = true;
                    }
                }
            }

            EventDispatcher.Instance.DispatchEvent(EventEnum.OnIAPItemPaid);
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString =
                    LocalizationManager.Instance.GetLocalizedString("&key.UI_store_common_restore_purchase_notice"),
                OKButtonText = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_ok")
            });
        });
    }

    /// <summary>
    /// 获得价格
    /// </summary>
    public string GetPrice(int shopId)
    {
        var shopConfig = GlobalConfigManager.Instance.GetTableShopByID(shopId);
        if (shopConfig == null)
            return "";
        var products = Dlugin.SDK.GetInstance().iapManager.GetAllProductInfo();
        if (products != null && products.Length > 0)
        {
            var product = GetProductInfo(shopConfig.id);
            if (product == null || product.metadata == null)
            {
                DebugUtil.Log("价格从配置获取-1" + shopConfig?.price);
                return "$" + shopConfig.price.ToString();
            }
            DebugUtil.Log("价格从平台获取" + product.metadata.localizedPriceString);
            return product.metadata.localizedPriceString;
        }

        DebugUtil.Log("价格从配置获取" + shopConfig?.price);
        return "$" + shopConfig.price.ToString();
    }

    public Product GetProductInfo(int shopItemId)
    {
        var products = Dlugin.SDK.GetInstance().iapManager.GetAllProductInfo();
        if (products == null)
        {
            DebugUtil.LogWarning("products is null");
            return null;
        }

        var shopConfig = GlobalConfigManager.Instance.GetTableShopByID(shopItemId);
        if (shopConfig == null)
        {
            DebugUtil.LogWarning("shopConfig is null " + shopItemId);
            return null;
        }

        foreach (var p in products)
        {
            // Debug.LogError("p.definition.storeSpecificId="+p.definition.storeSpecificId+" shopConfig.product_id="+shopConfig.product_id+" p.metadata.localizedPriceString="+p.metadata.localizedPriceString);
            // DragonU3DSDK.DebugUtil.LogError("Product storeSpecificId : " + p.definition.storeSpecificId);
#if UNITY_IOS
            if (p.definition.storeSpecificId == shopConfig.product_id_ios)
            {
                return p;
            }
#elif UNITY_ANDROID
            if (p.definition.storeSpecificId == shopConfig.product_id)
            {
                return p;
            }
#endif
        }

        return null;
    }

    public int GetTodayBuyCount(int shopID)
    {
        StorageStoreItem item = GetStorageItem(shopID);
        if (null == item)
            return 0;

        return item.BuyCount;
    }

    public void AddItemCount(int shopID, DailyShop dailyShop)
    {
        StorageStoreItem item = GetStorageItem(shopID);
        if (dailyShop != null)
        {
            int priceAdd = 0;
            priceAdd = item.BuyCount >= dailyShop.Price_idex.Count
                ? 0
                : dailyShop.Price_idex[item.BuyCount];
            item.PriceAdd += priceAdd;
        }

        long time = Utils.GetTimeStamp();
        ++item.BuyCount;
        item.BuyTime = time;
    }

    public StorageStoreItem GetStorageItem(int shopID)
    {
        StorageHome storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        StorageStoreItem item = null;
        if (!storageHome.StoreItems.ContainsKey(shopID))
        {
            item = new StorageStoreItem();
            storageHome.StoreItems.Add(shopID, item);
            DebugUtil.Log("新增商品存档 " + shopID);
        }
        else
        {
            item = storageHome.StoreItems[shopID];
        }

        long time = Utils.GetTimeStamp();
        if (!Utils.IsSameDay(item.BuyTime / 1000, time / 1000))
        {
            item.BuyCount = 0;
            item.PriceAdd = 0;
            item.RvWatched = 0;
            // DebugUtil.Log("商品存档隔天数量重置 " + shopID);
        }

        return item;
    }
    public Transform GetCommonFlyTarget()
    {
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = UIHomeMainController.mainController.ShopTransform;
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }
}
