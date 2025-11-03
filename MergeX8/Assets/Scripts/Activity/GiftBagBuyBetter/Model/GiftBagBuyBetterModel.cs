using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.GiftBagBuyBetter;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class GiftBagBuyBetterModel : ActivityEntityBase
{
    private static GiftBagBuyBetterModel _instance;
    public static GiftBagBuyBetterModel Instance => _instance ?? (_instance = new GiftBagBuyBetterModel());


    private StorageGiftBagBuyBetter _storageGiftBagBuyBetter;

    public StorageGiftBagBuyBetter StorageGiftBagBuyBetter
    {
        get
        {
            if (_storageGiftBagBuyBetter == null)
                _storageGiftBagBuyBetter = StorageManager.Instance.GetStorage<StorageHome>().GiftBagBuyBetter;

            return _storageGiftBagBuyBetter;
        }
    }

    public override string Guid => "OPS_EVENT_TYPE_GIFT_BAG_BUY_BETTER";

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
        GiftBagBuyBetterConfigManager.Instance.InitConfig(configJson);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
        CleanStorage();
    }
    public void CleanStorage()
    {
        var storage = StorageManager.Instance.GetStorage<StorageHome>().GiftBagBuyBetter;
        var cleanList = new List<string>();
        foreach (var pair in storage.GiftBagBuyBetterIds)
        {
            if (pair.Key != StorageKey)
            {
                cleanList.Add(pair.Key);
            }
        }
        foreach (var key in cleanList)
        {
            storage.GiftBagBuyBetterIds.Remove(key);
        }
        cleanList.Clear();
        foreach (var pair in storage.GiftBagBuyBetteriIndexs)
        {
            if (pair.Key != StorageKey)
            {
                cleanList.Add(pair.Key);
            }
        }
        foreach (var key in cleanList)
        {
            storage.GiftBagBuyBetteriIndexs.Remove(key);
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
        if(!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GiftBagBuyBetter))
            return;
        
        if (GetCurActiveId() > 0)
            return;

        // Common common = AdConfigHandle.Instance.GetCommon();
        // if (common == null)
        //     return;

        StorageGiftBagBuyBetter.GiftBagBuyBetterIds[StorageKey] = PayLevelModel.Instance.GetCurPayLevelConfig().GiftBagBuyBetterGroupId;
    }

    public bool IsOpened()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GiftBagBuyBetter))
            return false;

        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;

        List<GiftBagBuyBetterResource> linkResources = GetGiftBagBuyBetterResources();
        if (linkResources == null)
            return false;

        if (GetCurIndex() >= linkResources.Count)
            return false;

        return true;
    }

    public int GetCurActiveId()
    {
        if (!StorageGiftBagBuyBetter.GiftBagBuyBetterIds.ContainsKey(StorageKey))
            StorageGiftBagBuyBetter.GiftBagBuyBetterIds.Add(StorageKey, 0);

        return StorageGiftBagBuyBetter.GiftBagBuyBetterIds[StorageKey];
    }

    public int GetCurIndex()
    {
        if (!StorageGiftBagBuyBetter.GiftBagBuyBetteriIndexs.ContainsKey(StorageKey))
            StorageGiftBagBuyBetter.GiftBagBuyBetteriIndexs.Add(StorageKey, 0);

        return StorageGiftBagBuyBetter.GiftBagBuyBetteriIndexs[StorageKey];
    }

    public void AddCurIndex()
    {
        int index = GetCurIndex() + 1;
        SetCurIndex(index);
    }

    public void SetCurIndex(int index)
    {
        StorageGiftBagBuyBetter.GiftBagBuyBetteriIndexs[StorageKey] = index;
    }

    public List<GiftBagBuyBetterResource> GetGiftBagBuyBetterResources()
    {
        int id = GetCurActiveId();
        if (id <= 0)
            return null;
        
        return GiftBagBuyBetterUtils.GetGiftBagBuyBetterDataList(id,GetCurIndex());
    }

    // public bool CanGetCurGift()
    // {
    //     List<GiftBagBuyBetterResource> linkResources = GetGiftBagBuyBetterResources();
    //     if (linkResources == null)
    //         return false;
    //
    //     int index = GetCurIndex();
    //     if (index < 0 || index + 1 >= linkResources.Count)
    //         return false;
    //
    //     GiftBagBuyBetterResource data = linkResources[index];
    //     if (data == null)
    //         return false;
    //
    //     return data.ConsumeType == 1;
    // }

    public void GiftBagBuyBetterGetReward(int index)
    {
        if (index < GiftBagBuyBetterModel.Instance.GetCurIndex())
            return;
        var resourcesList = GiftBagBuyBetterModel.Instance.GetGiftBagBuyBetterResources();
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
                            MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonGiftBagBuyBetterGet,
                            itemAId = _curGiftBagData.RewardID[i],
                            data1 = _curGiftBagData.Amount[i].ToString(),
                            isChange = true,
                        }); 
                    }
                    
                }

                var reasonArgs =
                    new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.GiftBagBuyButterGet);
                reasonArgs.data1 = _curGiftBagData.Id.ToString();
                GiftBagBuyBetterModel.Instance.AddCurIndex();
                CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController,
                    true, reasonArgs,
                    () =>
                    {
                        EventDispatcher.Instance.DispatchEvent(EventEnum.GIFT_BAG_BUY_BETTER_PURCHASE_REFRESH, index, _curGiftBagData);
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

        var listData = GetGiftBagBuyBetterResources();
        for (int index = 0; index < listData.Count; index++)
        {
            GiftBagBuyBetterResource shopData = listData[index];

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
                        MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonGiftBagBuyBetterGet,
                        itemAId = shopData.RewardID[i],
                        data1 = shopData.Amount[i].ToString(),
                        isChange = true,
                    }); 
                }
            }

            var reasonArgs =
                new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.GiftBagBuyButterGet);
            reasonArgs.data1 = shopData.Id.ToString();

            AddCurIndex();
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
            CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController, true,
                reasonArgs,
                () =>
                {
                    EventDispatcher.Instance.DispatchEvent(EventEnum.GIFT_BAG_BUY_BETTER_PURCHASE_REFRESH, index, shopData);
                    PayRebateModel.Instance.OnPurchaseAniFinish();
                    PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                });

            // UIWindow uiView = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupGiftBagBuyBetter);
            // if (uiView != null)
            // {
            //     CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(uiView.canvas.sortingOrder + 1);
            // }

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
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.GiftBagBuyBetter);
    }
    public bool ShowEntrance()
    {
        return IsOpened();
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/GiftBagBuyBetter/Aux_GiftBagBuyBetter";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/GiftBagBuyBetter/TaskList_GiftBagBuyBetter";
    }
}