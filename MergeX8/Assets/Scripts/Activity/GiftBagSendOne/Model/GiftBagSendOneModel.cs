using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.GiftBagSendOne;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class GiftBagSendOneModel : ActivityEntityBase
{
    private static GiftBagSendOneModel _instance;
    public static GiftBagSendOneModel Instance => _instance ?? (_instance = new GiftBagSendOneModel());


    private StorageGiftBagSendOne _storageGiftBagSendOne;

    public StorageGiftBagSendOne StorageGiftBagSendOne
    {
        get
        {
            if (_storageGiftBagSendOne == null)
                _storageGiftBagSendOne = StorageManager.Instance.GetStorage<StorageHome>().GiftBagSendOne;

            return _storageGiftBagSendOne;
        }
    }

    public GiftBagSendOneList GlobalConfig => GiftBagSendOneConfigManager.Instance.GetConfig<GiftBagSendOneList>()
        .Find(a => a.UserGroup == StorageGiftBagSendOne.PayLevelGroup);

    private List<GiftBagSendOneResource> RewardConfig =>
        GiftBagSendOneConfigManager.Instance.GetConfig<GiftBagSendOneResource>();
    public override string Guid => "OPS_EVENT_TYPE_GIFT_BAG_SEND_ONE";

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
        GiftBagSendOneConfigManager.Instance.InitConfig(configJson);
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
    }

    public void InitStorage()
    {
        if (StorageGiftBagSendOne.ActivityId != ActivityId)
        {
            StorageGiftBagSendOne.Clear();
            StorageGiftBagSendOne.ActivityId = ActivityId;
            StorageGiftBagSendOne.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().GiftBagSendOneGroupId;
        }
    }
    public bool IsOpened()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GiftBagSendOne))
            return false;

        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;

        List<GiftBagSendOneResource> linkResources = GetGiftBagSendOneResources();
        if (linkResources == null)
            return false;

        var collectAll = true;
        foreach (var config in linkResources)
        {
            if (!StorageGiftBagSendOne.CollectState.Contains(config.Id))
                collectAll = false;
        }
        if (collectAll)
            return false;

        return true;
    }

    public List<GiftBagSendOneResource> GetGiftBagSendOneResources()
    {
        var configList = GlobalConfig.ListData;
        var list = new List<GiftBagSendOneResource>();
        foreach (var config in configList)
        {
            list.Add(RewardConfig.Find(a => a.Id == config));
        }
        return list;
    }

    public bool CanCollect(GiftBagSendOneResource config)
    {
        if (StorageGiftBagSendOne.CollectState.Contains(config.Id))
            return false;
        if (StorageGiftBagSendOne.BuyState)
            return true;
        return false;
    }
    public void GiftBagSendOneGetReward(GiftBagSendOneResource config)
    {
        if (!CanCollect(config))
            return;
        StorageGiftBagSendOne.CollectState.Add(config.Id);
        var rewards = CommonUtils.FormatReward(config.RewardID, config.Amount);
        var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap);
        UserData.Instance.AddRes(rewards,reason);
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
            false, reason);
    }

    public void OnRvSuccess(GiftBagSendOneResource config)
    {
        if (config.ConsumeType != 2 || config.ConsumeAmount > 0)
            return;
        if (!CanBuy())
            return;
        StorageGiftBagSendOne.RvTimes++;
        if (StorageGiftBagSendOne.RvTimes < -config.ConsumeAmount)
        {
            if (UIPopupGiftBagSendOneController.Instance)
            {
                UIPopupGiftBagSendOneController.Instance.OnBuy();
            }
            return;   
        }
        GiftBagSendOneResource shopData = config;
        StorageGiftBagSendOne.BuyState = true;
        GiftBagSendOneGetReward(shopData);
        var ret = CommonUtils.FormatReward(shopData.RewardID, shopData.Amount);
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
        if (UIPopupGiftBagSendOneController.Instance)
        {
            UIPopupGiftBagSendOneController.Instance.OnBuy();
        }
    }
    public void PurchaseSuccess(TableShop tableShop)
    {
        if (tableShop == null)
            return;
        if (!CanBuy())
            return;
        var listData = GetGiftBagSendOneResources();
        for (int index = 0; index < listData.Count; index++)
        {
            GiftBagSendOneResource shopData = listData[index];

            if (shopData.ConsumeType != 2 || shopData.ConsumeAmount != tableShop.id)
                continue;

            StorageGiftBagSendOne.BuyState = true;
            GiftBagSendOneGetReward(shopData);
            var ret = CommonUtils.FormatReward(shopData.RewardID, shopData.Amount);
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
            if (UIPopupGiftBagSendOneController.Instance)
            {
                UIPopupGiftBagSendOneController.Instance.OnBuy();
            }
            return;
        }
    }

    public bool CanShowUI()
    {
        if (!IsOpened())
            return false;

        var configs = GetGiftBagSendOneResources();
        foreach (var config in configs)
        {
            if (CanCollect(config))
                return true;
        }
        return false;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.GiftBagSendOne);
    }
    
    public bool ShowEntrance()
    {
        return IsOpened();
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/GiftBagSendOne/Aux_GiftBagSendOne";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/GiftBagSendOne/TaskList_GiftBagSendOne";
    }

    public static bool CanBuy()
    {
        return Instance.IsOpened() && !Instance.StorageGiftBagSendOne.BuyState;
    }
}