using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.GiftBagSendTwo;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class GiftBagSendTwoModel : ActivityEntityBase
{
    private static GiftBagSendTwoModel _instance;
    public static GiftBagSendTwoModel Instance => _instance ?? (_instance = new GiftBagSendTwoModel());


    private StorageGiftBagSendTwo _storageGiftBagSendTwo;

    public StorageGiftBagSendTwo StorageGiftBagSendTwo
    {
        get
        {
            if (_storageGiftBagSendTwo == null)
                _storageGiftBagSendTwo = StorageManager.Instance.GetStorage<StorageHome>().GiftBagSendTwo;

            return _storageGiftBagSendTwo;
        }
    }

    private GiftBagSendTwoList GlobalConfig => GiftBagSendTwoConfigManager.Instance.GetConfig<GiftBagSendTwoList>()
        .Find(a => a.UserGroup == StorageGiftBagSendTwo.PayLevelGroup);

    private List<GiftBagSendTwoResource> RewardConfig =>
        GiftBagSendTwoConfigManager.Instance.GetConfig<GiftBagSendTwoResource>();
    public override string Guid => "OPS_EVENT_TYPE_GIFT_BAG_SEND_TWO";

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
        GiftBagSendTwoConfigManager.Instance.InitConfig(configJson);
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
    }

    public void InitStorage()
    {
        if (StorageGiftBagSendTwo.ActivityId != ActivityId)
        {
            StorageGiftBagSendTwo.Clear();
            StorageGiftBagSendTwo.ActivityId = ActivityId;
            StorageGiftBagSendTwo.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().GiftBagSendTwoGroupId;
        }
    }
    public bool IsOpened()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GiftBagSendTwo))
            return false;

        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;

        List<GiftBagSendTwoResource> linkResources = GetGiftBagSendTwoResources();
        if (linkResources == null)
            return false;

        var collectAll = true;
        foreach (var config in linkResources)
        {
            if (!StorageGiftBagSendTwo.CollectState.Contains(config.Id))
                collectAll = false;
        }
        if (collectAll)
            return false;

        return true;
    }

    public List<GiftBagSendTwoResource> GetGiftBagSendTwoResources()
    {
        var configList = GlobalConfig.ListData;
        var list = new List<GiftBagSendTwoResource>();
        foreach (var config in configList)
        {
            list.Add(RewardConfig.Find(a => a.Id == config));
        }
        return list;
    }

    public bool CanCollect(GiftBagSendTwoResource config)
    {
        if (StorageGiftBagSendTwo.CollectState.Contains(config.Id))
            return false;
        if (StorageGiftBagSendTwo.BuyState)
            return true;
        return false;
    }
    public void GiftBagSendTwoGetReward(GiftBagSendTwoResource config)
    {
        if (!CanCollect(config))
            return;
        StorageGiftBagSendTwo.CollectState.Add(config.Id);
        var rewards = CommonUtils.FormatReward(config.RewardID, config.Amount);
        var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap);
        UserData.Instance.AddRes(rewards,reason);
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
            false, reason);
    }
    public void OnRvSuccess(GiftBagSendTwoResource config)
    {
        if (config.ConsumeType != 2 || config.ConsumeAmount > 0)
            return;
        if (!CanBuy())
            return;
        StorageGiftBagSendTwo.RvTimes++;
        if (StorageGiftBagSendTwo.RvTimes < -config.ConsumeAmount)
        {
            if (UIPopupGiftBagSendTwoController.Instance)
            {
                UIPopupGiftBagSendTwoController.Instance.OnBuy();
            }
            return;   
        }
        GiftBagSendTwoResource shopData = config;
        StorageGiftBagSendTwo.BuyState = true;
        GiftBagSendTwoGetReward(shopData);
        var ret = CommonUtils.FormatReward(shopData.RewardID, shopData.Amount);
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
        if (UIPopupGiftBagSendTwoController.Instance)
        {
            UIPopupGiftBagSendTwoController.Instance.OnBuy();
        }
    }
    public void PurchaseSuccess(TableShop tableShop)
    {
        if (tableShop == null)
            return;
        if (!CanBuy())
            return;
        var listData = GetGiftBagSendTwoResources();
        for (int index = 0; index < listData.Count; index++)
        {
            GiftBagSendTwoResource shopData = listData[index];

            if (shopData.ConsumeType != 2 || shopData.ConsumeAmount != tableShop.id)
                continue;

            StorageGiftBagSendTwo.BuyState = true;
            GiftBagSendTwoGetReward(shopData);
            var ret = CommonUtils.FormatReward(shopData.RewardID, shopData.Amount);
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
            if (UIPopupGiftBagSendTwoController.Instance)
            {
                UIPopupGiftBagSendTwoController.Instance.OnBuy();
            }
            return;
        }
    }

    public bool CanShowUI()
    {
        if (!IsOpened())
            return false;

        var configs = GetGiftBagSendTwoResources();
        foreach (var config in configs)
        {
            if (CanCollect(config))
                return true;
        }
        return false;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.GiftBagSendTwo);
    }
    
    public bool ShowEntrance()
    {
        return IsOpened();
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/GiftBagSendTwo/Aux_GiftBagSendTwo";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/GiftBagSendTwo/TaskList_GiftBagSendTwo";
    }

    public static bool CanBuy()
    {
        return Instance.IsOpened() && !Instance.StorageGiftBagSendTwo.BuyState;
    }
}