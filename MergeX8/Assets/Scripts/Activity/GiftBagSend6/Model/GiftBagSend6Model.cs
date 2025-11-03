using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.GiftBagSend6;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class GiftBagSend6Model : ActivityEntityBase
{
    private static GiftBagSend6Model _instance;
    public static GiftBagSend6Model Instance => _instance ?? (_instance = new GiftBagSend6Model());


    private StorageGiftBagSend6 _storageGiftBagSend6;

    public StorageGiftBagSend6 StorageGiftBagSend6
    {
        get
        {
            if (_storageGiftBagSend6 == null)
                _storageGiftBagSend6 = StorageManager.Instance.GetStorage<StorageHome>().GiftBagSend6;

            return _storageGiftBagSend6;
        }
    }

    private GiftBagSend6List GlobalConfig => GiftBagSend6ConfigManager.Instance.GetConfig<GiftBagSend6List>()
        .Find(a => a.UserGroup == StorageGiftBagSend6.PayLevelGroup);

    private List<GiftBagSend6Resource> RewardConfig =>
        GiftBagSend6ConfigManager.Instance.GetConfig<GiftBagSend6Resource>();
    public override string Guid => "OPS_EVENT_TYPE_GIFT_BAG_SEND_6";

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
        GiftBagSend6ConfigManager.Instance.InitConfig(configJson);
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
    }

    public void InitStorage()
    {
        if (StorageGiftBagSend6.ActivityId != ActivityId)
        {
            StorageGiftBagSend6.Clear();
            StorageGiftBagSend6.ActivityId = ActivityId;
            StorageGiftBagSend6.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().GiftBagSend6GroupId;
        }
    }
    public bool IsOpened()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GiftBagSend6))
            return false;

        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;

        List<GiftBagSend6Resource> linkResources = GetGiftBagSend6Resources();
        if (linkResources == null)
            return false;

        var collectAll = true;
        foreach (var config in linkResources)
        {
            if (!StorageGiftBagSend6.CollectState.Contains(config.Id))
                collectAll = false;
        }
        if (collectAll)
            return false;

        return true;
    }

    public List<GiftBagSend6Resource> GetGiftBagSend6Resources()
    {
        var configList = GlobalConfig.ListData;
        var list = new List<GiftBagSend6Resource>();
        foreach (var config in configList)
        {
            list.Add(RewardConfig.Find(a => a.Id == config));
        }
        return list;
    }

    public bool CanCollect(GiftBagSend6Resource config)
    {
        if (StorageGiftBagSend6.CollectState.Contains(config.Id))
            return false;
        if (!StorageGiftBagSend6.BuyState)
            return false;
        var configList = GetGiftBagSend6Resources();
        var canCollect = true;
        for (var i = 0; i < configList.Count; i++)
        {
            if (configList[i] == config)
            {
                break;
            }
            if (!StorageGiftBagSend6.CollectState.Contains(configList[i].Id))
            {
                canCollect = false;
                break;
            }
        }
        return canCollect;
    }
    public void GiftBagSend6GetReward(GiftBagSend6Resource config)
    {
        if (!CanCollect(config))
            return;
        StorageGiftBagSend6.CollectState.Add(config.Id);
        var rewards = CommonUtils.FormatReward(config.RewardID, config.Amount);
        var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap);
        UserData.Instance.AddRes(rewards,reason);
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
            false, reason);
    }
    public void OnRvSuccess(GiftBagSend6Resource config)
    {
        if (config.ConsumeType != 2 || config.ConsumeAmount > 0)
            return;
        if (!CanBuy())
            return;
        StorageGiftBagSend6.RvTimes++;
        if (StorageGiftBagSend6.RvTimes < -config.ConsumeAmount)
        {
            if (UIPopupGiftBagSend6Controller.Instance)
            {
                UIPopupGiftBagSend6Controller.Instance.OnBuy();
            }
            return;   
        }
        GiftBagSend6Resource shopData = config;
        StorageGiftBagSend6.BuyState = true;
        GiftBagSend6GetReward(shopData);
        var ret = CommonUtils.FormatReward(shopData.RewardID, shopData.Amount);
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
        if (UIPopupGiftBagSend6Controller.Instance)
        {
            UIPopupGiftBagSend6Controller.Instance.OnBuy();
        }
    }
    public void PurchaseSuccess(TableShop tableShop)
    {
        if (tableShop == null)
            return;
        if (!CanBuy())
            return;
        var listData = GetGiftBagSend6Resources();
        for (int index = 0; index < listData.Count; index++)
        {
            GiftBagSend6Resource shopData = listData[index];

            if (shopData.ConsumeType != 2 || shopData.ConsumeAmount != tableShop.id)
                continue;

            StorageGiftBagSend6.BuyState = true;
            GiftBagSend6GetReward(shopData);
            var ret = CommonUtils.FormatReward(shopData.RewardID, shopData.Amount);
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
            if (UIPopupGiftBagSend6Controller.Instance)
            {
                UIPopupGiftBagSend6Controller.Instance.OnBuy();
            }
            return;
        }
    }

    public bool CanShowUI()
    {
        if (!IsOpened())
            return false;

        var configs = GetGiftBagSend6Resources();
        foreach (var config in configs)
        {
            if (CanCollect(config))
                return true;
        }
        return false;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.GiftBagSend6);
    }
    
    public bool ShowEntrance()
    {
        return IsOpened();
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/GiftBagSend6/Aux_GiftBagSend6";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/GiftBagSend6/TaskList_GiftBagSend6";
    }

    public static bool CanBuy()
    {
        return Instance.IsOpened() && !Instance.StorageGiftBagSend6.BuyState;
    }
}