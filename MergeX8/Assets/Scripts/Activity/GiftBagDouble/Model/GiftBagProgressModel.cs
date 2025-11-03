using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.GiftBagDouble;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class GiftBagDoubleModel: ActivityEntityBase
{
    private static GiftBagDoubleModel _instance;
    public static GiftBagDoubleModel Instance => _instance ?? (_instance = new GiftBagDoubleModel());
    public override string Guid => "OPS_EVENT_TYPE_GIFT_BAG_DOUBLE";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public GiftBagDoubleModel()
    {
        EventDispatcher.Instance.AddEventListener(EventEnum.BackLogin,InitEntranceAgain);
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GiftBagDouble);
    public bool IsFinish => Storage.IsFinish();

    public bool IsOpenPrivate()
    {
        return IsUnlock && IsOpened() && !IsFinish;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.GiftBagDouble);
    }

    public GiftBagDoubleGlobalConfig GlobalConfig =>
        GiftBagDoubleConfigManager.Instance.GetConfig<GiftBagDoubleGlobalConfig>()[0];

    public List<GiftBagDoubleGroupConfig> GroupConfig =>
        GiftBagDoubleConfigManager.Instance.GetConfig<GiftBagDoubleGroupConfig>();
    public List<GiftBagDoubleProductConfig> ProductConfig =>
        GiftBagDoubleConfigManager.Instance.GetConfig<GiftBagDoubleProductConfig>();
    
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        GiftBagDoubleConfigManager.Instance.InitConfig(configJson);
        DebugUtil.Log($"InitConfig:{Guid}");
        CleanUselessStorage();
        InitStorage();
    }

    public void InitEntranceAgain(BaseEvent e)
    {
        if (!IsInitFromServer())
            return;
    }

    public Dictionary<string, StorageGiftBagDouble> StorageDic =>
        StorageManager.Instance.GetStorage<StorageHome>().GiftBagDouble;

    public void InitStorage()
    {
        if (!IsInitFromServer())
            return;
        if (!StorageDic.TryGetValue(ActivityId, out var storage))
        {
            storage = new StorageGiftBagDouble();
            var groupId = PayLevelModel.Instance.GetCurPayLevelConfig().GiftBagDoubleGroupId;
            storage.GroupId = groupId;
            StorageDic.Add(ActivityId,storage);
        }
        storage.StartTime = (long) StartTime;
        storage.EndTime = (long) EndTime;
    }

    public StorageGiftBagDouble Storage
    {
        get
        {
            if (ActivityId.IsEmptyString())
                return null;
            
            return StorageDic.TryGetValue(ActivityId, out var storage) ? storage : null;
        }
    }

    public void CleanUselessStorage()
    {
        var cleanStorageKeyList = new List<string>();
        foreach (var pair in StorageDic)
        {
            var storage = pair.Value;
            if (storage.IsTimeOut())//没购买或者已购买但没有未领取奖励
            {
                cleanStorageKeyList.Add(pair.Key);
            }
        }
        foreach (var storageKey in cleanStorageKeyList)
        {
            StorageDic.Remove(storageKey);
        }
    }

    public void OnPurchase(TableShop shopConfig)
    {
        if (!IsInitFromServer())
            return;
        if (Storage == null)
            return;
        var productList = Storage.GetProductList();
        var reason = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.GiftBagDoubleGet
        };
        for (var i = 0; i < GroupConfig.Count; i++)
        {
            var group = GroupConfig[i];
            if (GroupConfig[i].BuyAllShopId == shopConfig.id)
            {
                var products = group.ProductList;
                var totalProducts = new List<GiftBagDoubleProductConfig>();
                var allRewards = new List<ResData>();
                foreach (var productId in products)
                {
                    var singleProduct = ProductConfig.Find(a => a.Id == productId);
                    if (singleProduct == null)
                        continue;
                    totalProducts.Add(singleProduct);
                    var singleRewards = CommonUtils.FormatReward(singleProduct.RewardId, singleProduct.RewardNum);
                    allRewards.AddRange(singleRewards);
                    Storage.BuyState.Add(singleProduct.Id);
                }
                foreach (var resData in allRewards)
                {
                    if(UserData.Instance.IsResource(resData.id))
                        continue;
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonGiftBagDoubleGet,
                        itemAId =resData.id,
                        isChange = true,
                    });
                }
                EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, allRewards);
                CommonRewardManager.Instance.PopCommonReward(allRewards, CurrencyGroupManager.Instance.GetCurrencyUseController(),
                    true,
                    reason, animEndCall: () =>
                    {
                        foreach (var product in totalProducts)
                        {
                            EventDispatcher.Instance.SendEventImmediately(new EventGiftBagDoubleBuyStateChange(Storage,product));
                        }
                    });
                var mainUI = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupGiftBagDoubleMain);
                if (mainUI)
                    mainUI.AnimCloseWindow();
                return;
            }
        }
        var product = productList.Find(a => a.ShopId == shopConfig.id);
        if (product == null)
            return;
        var rewards = CommonUtils.FormatReward(product.RewardId, product.RewardNum);
        Storage.BuyState.Add(product.Id);
        foreach (var resData in rewards)
        {
            if(UserData.Instance.IsResource(resData.id))
                continue;
            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
            {
                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonGiftBagDoubleGet,
                itemAId =resData.id,
                isChange = true,
            });
        }
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);

        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.GetCurrencyUseController(),
            true,
            reason, animEndCall: () =>
            {
                EventDispatcher.Instance.SendEventImmediately(new EventGiftBagDoubleBuyStateChange(Storage,product));
            });
    }
    private static string CanShowUICoolTimeKey = "GiftBagDouble_CanShowUI";
    public bool CanShowUI()
    {
        if (!IsOpenPrivate())
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, CanShowUICoolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, CanShowUICoolTimeKey, CommonUtils.GetTimeStamp());
            UIPopupGiftBagDoubleMainController.Open(Storage);
            return true;
        }
        return false;
    }

    public bool ShowAuxItem()
    {
        if (Storage == null)
            return false;

        return Storage.ShowAuxItem();
    }
}