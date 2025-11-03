using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.GiftBagSendThree;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class GiftBagSendThreeModel : ActivityEntityBase
{
    private static GiftBagSendThreeModel _instance;
    public static GiftBagSendThreeModel Instance => _instance ?? (_instance = new GiftBagSendThreeModel());


    private StorageGiftBagSendThree _storageGiftBagSendThree;

    public StorageGiftBagSendThree StorageGiftBagSendThree
    {
        get
        {
            if (_storageGiftBagSendThree == null)
                _storageGiftBagSendThree = StorageManager.Instance.GetStorage<StorageHome>().GiftBagSendThree;

            return _storageGiftBagSendThree;
        }
    }

    private GiftBagSendThreeList GlobalConfig => GiftBagSendThreeConfigManager.Instance.GetConfig<GiftBagSendThreeList>()
        .Find(a => a.UserGroup == StorageGiftBagSendThree.PayLevelGroup);

    private List<GiftBagSendThreeResource> RewardConfig =>
        GiftBagSendThreeConfigManager.Instance.GetConfig<GiftBagSendThreeResource>();
    public override string Guid => "OPS_EVENT_TYPE_GIFT_BAG_SEND_THREE";

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
        GiftBagSendThreeConfigManager.Instance.InitConfig(configJson);
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
    }

    public void InitStorage()
    {
        if (StorageGiftBagSendThree.ActivityId != ActivityId)
        {
            StorageGiftBagSendThree.Clear();
            StorageGiftBagSendThree.ActivityId = ActivityId;
            StorageGiftBagSendThree.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().GiftBagSendThreeGroupId;
        }
    }
    public bool IsOpened()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GiftBagSendThree))
            return false;

        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;

        List<GiftBagSendThreeResource> linkResources = GetGiftBagSendThreeResources();
        if (linkResources == null)
            return false;

        var collectAll = true;
        foreach (var config in linkResources)
        {
            if (!StorageGiftBagSendThree.CollectState.Contains(config.Id))
                collectAll = false;
        }
        if (collectAll)
            return false;

        return true;
    }

    public List<GiftBagSendThreeResource> GetGiftBagSendThreeResources()
    {
        var configList = GlobalConfig.ListData;
        var list = new List<GiftBagSendThreeResource>();
        foreach (var config in configList)
        {
            list.Add(RewardConfig.Find(a => a.Id == config));
        }
        return list;
    }

    public bool CanCollect(GiftBagSendThreeResource config)
    {
        if (StorageGiftBagSendThree.CollectState.Contains(config.Id))
            return false;
        if (StorageGiftBagSendThree.BuyState)
            return true;
        return false;
    }
    public void GiftBagSendThreeGetReward(GiftBagSendThreeResource config)
    {
        if (!CanCollect(config))
            return;
        StorageGiftBagSendThree.CollectState.Add(config.Id);
        var rewards = CommonUtils.FormatReward(config.RewardID, config.Amount);
        var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap);
        UserData.Instance.AddRes(rewards,reason);
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
            false, reason);
    }
    public void OnRvSuccess(GiftBagSendThreeResource config)
    {
        if (config.ConsumeType != 2 || config.ConsumeAmount > 0)
            return;
        if (!CanBuy())
            return;
        StorageGiftBagSendThree.RvTimes++;
        if (StorageGiftBagSendThree.RvTimes < -config.ConsumeAmount)
        {
            if (UIPopupGiftBagSendThreeController.Instance)
            {
                UIPopupGiftBagSendThreeController.Instance.OnBuy();
            }
            return;   
        }
        GiftBagSendThreeResource shopData = config;
        StorageGiftBagSendThree.BuyState = true;
        GiftBagSendThreeGetReward(shopData);
        var ret = CommonUtils.FormatReward(shopData.RewardID, shopData.Amount);
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
        if (UIPopupGiftBagSendThreeController.Instance)
        {
            UIPopupGiftBagSendThreeController.Instance.OnBuy();
        }
    }
    public void PurchaseSuccess(TableShop tableShop)
    {
        if (tableShop == null)
            return;
        if (!CanBuy())
            return;
        var listData = GetGiftBagSendThreeResources();
        for (int index = 0; index < listData.Count; index++)
        {
            GiftBagSendThreeResource shopData = listData[index];

            if (shopData.ConsumeType != 2 || shopData.ConsumeAmount != tableShop.id)
                continue;

            StorageGiftBagSendThree.BuyState = true;
            GiftBagSendThreeGetReward(shopData);
            var ret = CommonUtils.FormatReward(shopData.RewardID, shopData.Amount);
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
            if (UIPopupGiftBagSendThreeController.Instance)
            {
                UIPopupGiftBagSendThreeController.Instance.OnBuy();
            }
            return;
        }
    }

    public bool CanShowUI()
    {
        if (!IsOpened())
            return false;

        var configs = GetGiftBagSendThreeResources();
        foreach (var config in configs)
        {
            if (CanCollect(config))
                return true;
        }
        return false;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.GiftBagSendThree);
    }
    
    public bool ShowEntrance()
    {
        return IsOpened();
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/GiftBagSendThree/Aux_GiftBagSendThree";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/GiftBagSendThree/TaskList_GiftBagSendThree";
    }

    public static bool CanBuy()
    {
        return Instance.IsOpened() && !Instance.StorageGiftBagSendThree.BuyState;
    }
}