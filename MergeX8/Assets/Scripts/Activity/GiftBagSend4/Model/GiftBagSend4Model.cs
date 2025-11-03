using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.GiftBagSend4;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class GiftBagSend4Model : ActivityEntityBase
{
    private static GiftBagSend4Model _instance;
    public static GiftBagSend4Model Instance => _instance ?? (_instance = new GiftBagSend4Model());


    private StorageGiftBagSend4 _storageGiftBagSend4;

    public StorageGiftBagSend4 StorageGiftBagSend4
    {
        get
        {
            if (_storageGiftBagSend4 == null)
                _storageGiftBagSend4 = StorageManager.Instance.GetStorage<StorageHome>().GiftBagSend4;

            return _storageGiftBagSend4;
        }
    }

    private GiftBagSend4List GlobalConfig => GiftBagSend4ConfigManager.Instance.GetConfig<GiftBagSend4List>()
        .Find(a => a.UserGroup == StorageGiftBagSend4.PayLevelGroup);

    private List<GiftBagSend4Resource> RewardConfig =>
        GiftBagSend4ConfigManager.Instance.GetConfig<GiftBagSend4Resource>();
    public override string Guid => "OPS_EVENT_TYPE_GIFT_BAG_SEND_4";

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
        GiftBagSend4ConfigManager.Instance.InitConfig(configJson);
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
    }

    public void InitStorage()
    {
        if (StorageGiftBagSend4.ActivityId != ActivityId)
        {
            StorageGiftBagSend4.Clear();
            StorageGiftBagSend4.ActivityId = ActivityId;
            StorageGiftBagSend4.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().GiftBagSend4GroupId;
        }
    }
    public bool IsOpened()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GiftBagSend4))
            return false;

        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;

        List<GiftBagSend4Resource> linkResources = GetGiftBagSend4Resources();
        if (linkResources == null)
            return false;

        var collectAll = true;
        foreach (var config in linkResources)
        {
            if (!StorageGiftBagSend4.CollectState.Contains(config.Id))
                collectAll = false;
        }
        if (collectAll)
            return false;

        return true;
    }

    public List<GiftBagSend4Resource> GetGiftBagSend4Resources()
    {
        var configList = GlobalConfig.ListData;
        var list = new List<GiftBagSend4Resource>();
        foreach (var config in configList)
        {
            list.Add(RewardConfig.Find(a => a.Id == config));
        }
        return list;
    }

    public bool CanCollect(GiftBagSend4Resource config)
    {
        if (StorageGiftBagSend4.CollectState.Contains(config.Id))
            return false;
        if (StorageGiftBagSend4.BuyState)
            return true;
        return false;
    }
    public void GiftBagSend4GetReward(GiftBagSend4Resource config)
    {
        if (!CanCollect(config))
            return;
        StorageGiftBagSend4.CollectState.Add(config.Id);
        var rewards = CommonUtils.FormatReward(config.RewardID, config.Amount);
        var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap);
        UserData.Instance.AddRes(rewards,reason);
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
            false, reason);
    }
    public void OnRvSuccess(GiftBagSend4Resource config)
    {
        if (config.ConsumeType != 2 || config.ConsumeAmount > 0)
            return;
        if (!CanBuy())
            return;
        StorageGiftBagSend4.RvTimes++;
        if (StorageGiftBagSend4.RvTimes < -config.ConsumeAmount)
        {
            if (UIPopupGiftBagSend4Controller.Instance)
            {
                UIPopupGiftBagSend4Controller.Instance.OnBuy();
            }
            return;   
        }
        GiftBagSend4Resource shopData = config;
        StorageGiftBagSend4.BuyState = true;
        GiftBagSend4GetReward(shopData);
        var ret = CommonUtils.FormatReward(shopData.RewardID, shopData.Amount);
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
        if (UIPopupGiftBagSend4Controller.Instance)
        {
            UIPopupGiftBagSend4Controller.Instance.OnBuy();
        }
    }
    public void PurchaseSuccess(TableShop tableShop)
    {
        if (tableShop == null)
            return;
        if (!CanBuy())
            return;
        var listData = GetGiftBagSend4Resources();
        for (int index = 0; index < listData.Count; index++)
        {
            GiftBagSend4Resource shopData = listData[index];

            if (shopData.ConsumeType != 2 || shopData.ConsumeAmount != tableShop.id)
                continue;

            StorageGiftBagSend4.BuyState = true;
            GiftBagSend4GetReward(shopData);
            var ret = CommonUtils.FormatReward(shopData.RewardID, shopData.Amount);
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
            if (UIPopupGiftBagSend4Controller.Instance)
            {
                UIPopupGiftBagSend4Controller.Instance.OnBuy();
            }
            return;
        }
    }

    public bool CanShowUI()
    {
        if (!IsOpened())
            return false;

        var configs = GetGiftBagSend4Resources();
        foreach (var config in configs)
        {
            if (CanCollect(config))
                return true;
        }
        return false;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.GiftBagSend4);
    }
    
    public bool ShowEntrance()
    {
        return IsOpened();
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/GiftBagSend4/Aux_GiftBagSend4";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/GiftBagSend4/TaskList_GiftBagSend4";
    }

    public static bool CanBuy()
    {
        return Instance.IsOpened() && !Instance.StorageGiftBagSend4.BuyState;
    }
}