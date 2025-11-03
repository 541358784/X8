using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.EasterGift;
using DragonPlus.Config.PayRebate;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class EasterGiftModel : ActivityEntityBase
{
    private static EasterGiftModel _instance;
    public static EasterGiftModel Instance => _instance ?? (_instance = new EasterGiftModel());


    private StorageEasterStorePack _storage;

    public StorageEasterStorePack StorageEasterStore
    {
        get
        {
            var storage = StorageManager.Instance.GetStorage<StorageHome>().EasterStorePack;
            if (_storage == null)
            {
                if (!storage.ContainsKey(StorageKey))
                    storage.Add(StorageKey, new StorageEasterStorePack());
                _storage = storage[StorageKey];
            }
           
            return _storage;
        }
    }

    public override string Guid => "OPS_EVENT_TYPE_EASTER_GIFT";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        EasterGiftConfigManager.Instance.InitConfig(configJson);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
    }

    public override void UpdateActivityState()
    {
        InitServerDataFinish();
    }

    protected override void InitServerDataFinish()
    {
        _storage = null;
    }

    public bool IsOpened()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Easter))
            return false;
        if (PayRebateLocalModel.Instance.IsOpened())
            return false;
        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;
        if (StorageEasterStore.IsFinish)
            return false;

        return true;
    }

    public bool CanShowUI()
    {
        if (!IsOpened())
            return false;
        return true;
    }
 
    public bool IsCanBuyItem(int shopID)
    {
        return !StorageEasterStore.GotShopItem.Contains(shopID);
    }

    public void RecordBuyItem(int shopID)
    {
        if (!StorageEasterStore.GotShopItem.Contains(shopID))
        {
            StorageEasterStore.GotShopItem.Add(shopID);
            EventDispatcher.Instance.DispatchEvent(EventEnum.EASTER_PACK_REFRESH, shopID);
        }

        
        if (StorageEasterStore.GotShopItem.Count >= GetEasterBundleConfig().Count)
        {
            StorageEasterStore.IsFinish = true;
            EventDispatcher.Instance.DispatchEvent(EventEnum.EASTER_PACK_Finish);
        }
    }

    public void PurchaseSuccess(TableShop cfg,string openSrc)
    {
        List<ResData> listResData = new List<ResData>();
        var packinfo = GetEasterBundleConfig();
        if(packinfo==null)
            return;
        
        foreach (var item in packinfo)
        {
            if (item.ShopItemId == cfg.id)
            {
                for (int i = 0; i < item.BundleItemCountList.Count; i++)
                {
                    ResData res = new ResData(item.BundleItemList[i], item.BundleItemCountList[i]);
                    listResData.Add(res);
                    if (!UserData.Instance.IsResource(res.id))
                    {
                        TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig(res.id);
                        if (mergeItemConfig != null)
                        {
                            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                            {
                                MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonEasterPackageGet,
                                itemAId = mergeItemConfig.id,
                                ItemALevel = mergeItemConfig.level,
                                isChange = true,
                            });
                        }
                    }
                }

                break;
            }
        }

        if (listResData == null || listResData.Count == 0)
            return;
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, listResData);

        var reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.EasterPackageGet;
        CommonRewardManager.Instance.PopCommonReward(listResData,
            CurrencyGroupManager.Instance.GetCurrencyUseController(), true, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = reason,
            }
            , () =>
            {
                PayRebateModel.Instance.OnPurchaseAniFinish();
                PayRebateLocalModel.Instance.OnPurchaseAniFinish();
            });
        
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterPackageBuySuccess);

        RecordBuyItem(cfg.id);
    }

    public void ClearPack()
    {
        StorageEasterStore.Clear();

        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, "EasterPack");
    }

    public List<EasterBundle> GetEasterBundleConfig()
    {
        return EasterGiftConfigManager.Instance.GetConfig<EasterBundle>();
    }
 
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.Easter);
    }
    
}