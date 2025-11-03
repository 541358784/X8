using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.GiftBagProgress;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class GiftBagProgressModel: ActivityEntityBase
{
    private static GiftBagProgressModel _instance;
    public static GiftBagProgressModel Instance => _instance ?? (_instance = new GiftBagProgressModel());
    public override string Guid => "OPS_EVENT_TYPE_GIFT_BAG_PROGRESS";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public GiftBagProgressModel()
    {
        RegisterDailyTaskEvent();
        EventDispatcher.Instance.AddEventListener(EventEnum.BackLogin,InitEntranceAgain);
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GiftBagProgress);
    public bool IsFinish => Storage.IsFinish();

    public bool IsOpenPrivate()
    {
        return IsUnlock && IsOpened() && !IsFinish;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.GiftBagProgress);
    }

    public GiftBagProgressGlobalConfig GlobalConfig =>
        GiftBagProgressConfigManager.Instance.GetConfig<GiftBagProgressGlobalConfig>().Find(a=>a.Id == Storage.GlobalConfig);
    
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        MergeGiftBagProgressBubble_Creator.CreatorDic.Clear();
        GiftBagProgressConfigManager.Instance.InitConfig(configJson);
        DebugUtil.Log($"InitConfig:{Guid}");
        CleanUselessStorage();
        InitStorage();
        InitDailyTaskConfig();
        Storage.CreateMergeBubble();
    }

    public void InitEntranceAgain(BaseEvent e)
    {
        if (!IsInitFromServer())
            return;
        MergeGiftBagProgressBubble_Creator.CreatorDic.Clear();
        Storage.CreateMergeBubble();
    }

    public Dictionary<string, StorageGiftBagProgress> StorageDic =>
        StorageManager.Instance.GetStorage<StorageHome>().GiftBagProgress;

    public void InitStorage()
    {
        if (!IsInitFromServer())
            return;
        if (!StorageDic.TryGetValue(ActivityId, out var storage))
        {
            storage = new StorageGiftBagProgress();
            storage.BuyState = false;
            StorageDic.Add(ActivityId,storage);
            storage.GlobalConfig = PayLevelModel.Instance.GetCurPayLevelConfig().GiftBagProgressGroupId;
        }
        storage.StartTime = (long) StartTime;
        storage.EndTime = (long) EndTime;
    }

    public StorageGiftBagProgress Storage
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
            if (storage.IsTimeOut() && 
                (!storage.BuyState || storage.UnCollectRewards.Count == 0))//没购买或者已购买但没有未领取奖励
            {
                cleanStorageKeyList.Add(pair.Key);
            }
        }
        foreach (var storageKey in cleanStorageKeyList)
        {
            StorageDic.Remove(storageKey);
        }
    }
    public bool CheckUnCollectTaskRewards()//获取所有未及时领取的最终奖励,并清理存档
    {
        var unCollectRewards = new List<ResData>();
        var clearStorageList = new List<StorageGiftBagProgress>();
        foreach (var pair in StorageDic)
        {
            var storage = pair.Value;
            if (storage.IsTimeOut() && 
                storage.BuyState && storage.UnCollectRewards.Count > 0)//超时，已购买并且有未领取奖励
            {
                unCollectRewards.AddRange(CommonUtils.FormatReward(storage.UnCollectRewards));
                clearStorageList.Add(storage);
            }
        }

        if (unCollectRewards.Count == 0)
            return false;
        var reason = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.GiftBagProgressGet
        };
        CommonRewardManager.Instance.PopActivityUnCollectReward(unCollectRewards, reason, null, () =>
        {
            foreach (var storage in clearStorageList)
            {
                storage.UnCollectRewards.Clear();
            }
            CleanUselessStorage();
        });
        return true;
    }

    public void OnPurchase(TableShop shopConfig)
    {
        if (!IsInitFromServer())
            return;
        if (shopConfig.id != GlobalConfig.ShopId)
            return;
        var rewards = CommonUtils.FormatReward(GlobalConfig.RewardId, GlobalConfig.RewardNum);
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);
        var reason = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.GiftBagProgressGet
        };
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.GetCurrencyUseController(),
            true,
            reason, animEndCall: () =>
            {
                EventDispatcher.Instance.SendEventImmediately(new EventGiftBagProgressBuyStateChange(Storage));
            });
        if (Storage != null && !Storage.BuyState)
        {
            Storage.BuyState = true;
            // EventDispatcher.Instance.SendEventImmediately(new EventGiftBagProgressBuyStateChange(Storage));
        }
        else
        {
            foreach (var pair in StorageDic)
            {
                var storage = pair.Value;
                if (storage.IsActive() && !storage.BuyState)
                {
                    storage.BuyState = true;
                    // EventDispatcher.Instance.SendEventImmediately(new EventGiftBagProgressBuyStateChange(storage));
                    break;
                }
            }
        }
    }
    private static string CanShowUICoolTimeKey = "GiftBagProgress_CanShowUI";
    public bool CanShowUI()
    {
        if (!IsOpenPrivate())
            return false;
        if (Storage.BuyState)
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, CanShowUICoolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, CanShowUICoolTimeKey, CommonUtils.GetTimeStamp());
            UIPopupGiftBagProgressTaskController.Open(Storage);
            return true;
        }
        return false;
    }
    
    public bool ShowAuxItem()
    {
        if (Storage == null)
            return false;
        
        return GiftBagProgressUtils.ShowAuxItem(Storage);
    }
    
    public bool ShowTaskEntrance()
    {
        if (Storage == null)
            return false;
        
        return Storage.ShowTaskEntrance();
    }
}