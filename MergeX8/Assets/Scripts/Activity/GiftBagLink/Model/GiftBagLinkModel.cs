using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.GiftBagLink;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
// using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class GiftBagLinkModel : ActivityEntityBase
{
    private static GiftBagLinkModel _instance;
    public static GiftBagLinkModel Instance => _instance ?? (_instance = new GiftBagLinkModel());


    private StorageGiftBagLink _storageGiftBagLink;

    public StorageGiftBagLink StorageGiftBagLink
    {
        get
        {
            if (_storageGiftBagLink == null)
                _storageGiftBagLink = StorageManager.Instance.GetStorage<StorageHome>().GiftBagLink;

            return _storageGiftBagLink;
        }
    }

    public override string Guid => "OPS_EVENT_TYPE_GIFT_BAG_LINK";

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
        GiftBagLinkConfigManager.Instance.InitConfig(configJson);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
        CleanStorage();
    }

    public void CleanStorage()
    {
        var storage = StorageManager.Instance.GetStorage<StorageHome>().GiftBagLink;
        var cleanList = new List<string>();
        foreach (var pair in storage.GiftBagLinkIds)
        {
            if (pair.Key != StorageKey)
            {
                cleanList.Add(pair.Key);
            }
        }
        foreach (var key in cleanList)
        {
            storage.GiftBagLinkIds.Remove(key);
        }
        cleanList.Clear();
        foreach (var pair in storage.GiftBagLinkiIndexs)
        {
            if (pair.Key != StorageKey)
            {
                cleanList.Add(pair.Key);
            }
        }
        foreach (var key in cleanList)
        {
            storage.GiftBagLinkiIndexs.Remove(key);
        }
        if (cleanList.Count > 0)
            Debug.LogError(Guid+"存档清理数量"+cleanList.Count);
    }

    public override void UpdateActivityState()
    {
        InitServerDataFinish();
    }

    protected override void InitServerDataFinish()
    {
        if(!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GiftBagLink))
            return;
        
        if (GetCurActiveId() > 0)
            return;

        // Common common = AdConfigHandle.Instance.GetCommon();
        // if (common == null)
        //     return;

        StorageGiftBagLink.GiftBagLinkIds[StorageKey] = PayLevelModel.Instance.GetCurPayLevelConfig().GiftBagLinkGroupId;
    }

    public bool IsOpened()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GiftBagLink))
            return false;

        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;

        List<GiftBagLinkResource> linkResources = GetGiftBagLinkResources();
        if (linkResources == null)
            return false;

        if (GetCurIndex() >= linkResources.Count)
            return false;

        return true;
    }

    public int GetCurActiveId()
    {
        if (!StorageGiftBagLink.GiftBagLinkIds.ContainsKey(StorageKey))
            StorageGiftBagLink.GiftBagLinkIds.Add(StorageKey, 0);

        return StorageGiftBagLink.GiftBagLinkIds[StorageKey];
    }

    public int GetCurIndex()
    {
        if (!StorageGiftBagLink.GiftBagLinkiIndexs.ContainsKey(StorageKey))
            StorageGiftBagLink.GiftBagLinkiIndexs.Add(StorageKey, 0);

        return StorageGiftBagLink.GiftBagLinkiIndexs[StorageKey];
    }

    public void AddCurIndex()
    {
        int index = GetCurIndex() + 1;
        SetCurIndex(index);
    }

    public void SetCurIndex(int index)
    {
        StorageGiftBagLink.GiftBagLinkiIndexs[StorageKey] = index;
    }

    public List<GiftBagLinkResource> GetGiftBagLinkResources()
    {
        int id = GetCurActiveId();
        if (id <= 0)
            return null;
        
        return GiftBagLinkUtils.GetGiftBagLinkDataList(id,GetCurIndex());
    }

    // public bool CanGetCurGift()
    // {
    //     List<GiftBagLinkResource> linkResources = GetGiftBagLinkResources();
    //     if (linkResources == null)
    //         return false;
    //
    //     int index = GetCurIndex();
    //     if (index < 0 || index + 1 >= linkResources.Count)
    //         return false;
    //
    //     GiftBagLinkResource data = linkResources[index];
    //     if (data == null)
    //         return false;
    //
    //     return data.ConsumeType == 1;
    // }

    public void GiftBagLinkGetReward(int index)
    {
        if (index < GiftBagLinkModel.Instance.GetCurIndex())
            return;
        var resourcesList = GiftBagLinkModel.Instance.GetGiftBagLinkResources();
        if (resourcesList == null)
            return;
        if (index >= resourcesList.Count)
            return;
        var _curGiftBagData = resourcesList[index];
        if (_curGiftBagData == null)
            return;
        switch (_curGiftBagData.ConsumeType)
        {
            case 1:
            {
                var ret = new List<ResData>();
                for (int i = 0; i < _curGiftBagData.RewardID.Count; i++)
                {
                    ret.Add(new ResData(_curGiftBagData.RewardID[i],
                        _curGiftBagData.Amount[i]));
                    
                    if (!UserData.Instance.IsResource(_curGiftBagData.RewardID[i]))
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonPackageLinkGet,
                            itemAId = _curGiftBagData.RewardID[i],
                            data1 = _curGiftBagData.Amount[i].ToString(),
                            isChange = true,
                        }); 
                    }
                    
                }

                var reasonArgs =
                    new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.PackageLinkGet);
                reasonArgs.data1 = _curGiftBagData.Id.ToString();
                GiftBagLinkModel.Instance.AddCurIndex();
                CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController,
                    true, reasonArgs,
                    () =>
                    {
                        EventDispatcher.Instance.DispatchEvent(EventEnum.GIFTBAGLINK_PURCHASE_REFRESH, index, _curGiftBagData);
                    });
                break;
            }
            case 2:
            {
                StoreModel.Instance.Purchase(_curGiftBagData.ConsumeAmount);
                break;
            }
        }
    }
    
    public void PurchaseSuccess(TableShop tableShop)
    {
        if (tableShop == null)
            return;

        var listData = GetGiftBagLinkResources();
        for (int index = 0; index < listData.Count; index++)
        {
            GiftBagLinkResource shopData = listData[index];

            if (shopData.ConsumeType != 2 || shopData.ConsumeAmount != tableShop.id)
                continue;

            if (index < GetCurIndex())
                continue;

            var ret = new List<ResData>();
            for (int i = 0; i < shopData.RewardID.Count; i++)
            {
                ret.Add(new ResData(shopData.RewardID[i], shopData.Amount[i]));

                if (!UserData.Instance.IsResource(shopData.RewardID[i]))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonPackageLinkGet,
                        itemAId = shopData.RewardID[i],
                        data1 = shopData.Amount[i].ToString(),
                        isChange = true,
                    }); 
                }
            }

            var reasonArgs =
                new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.PackageLinkGet);
            reasonArgs.data1 = shopData.Id.ToString();

            AddCurIndex();
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
            CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController, true,
                reasonArgs,
                () =>
                {
                    EventDispatcher.Instance.DispatchEvent(EventEnum.GIFTBAGLINK_PURCHASE_REFRESH, index, shopData);
                    PayRebateModel.Instance.OnPurchaseAniFinish();
                    PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                });

            UIWindow uiView = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIGiftBagLink);
            if (uiView != null)
            {
                CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(uiView.canvas.sortingOrder + 1);
            }

            return;
        }
    }

    // public bool CanShowUI()
    // {
    //     if (!IsOpened())
    //         return false;
    //
    //     return CanGetCurGift();
    // }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.GiftBagLink);
    }

    public bool ShowEntrance()
    {
        return IsOpened();
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/GiftBagLink/Aux_GiftBagLink";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/GiftBagLink/TaskList_GiftBagLink";
    }
}