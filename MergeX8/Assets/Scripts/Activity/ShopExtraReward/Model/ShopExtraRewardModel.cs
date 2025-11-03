using System.Collections.Generic;
using System.Linq;
using DragonPlus.Config.ShopExtraReward;
using DragonU3DSDK.Storage;
using UnityEngine;

public class ShopExtraRewardModel : ActivityEntityBase
{
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/ShopExtraReward/Aux_ShopExtraReward";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/ShopExtraReward/TaskList_ShopExtraReward";
    }

    public bool ShowEntrance()
    {
        return IsPrivateOpened();
    }
    private static ShopExtraRewardModel _instance;
    public static ShopExtraRewardModel Instance => _instance ?? (_instance = new ShopExtraRewardModel());

    public Dictionary<string, StorageShopExtraReward> StorageDic => StorageManager.Instance.GetStorage<StorageHome>().ShopExtraReward;
    public StorageShopExtraReward CurStorage
    {
        get
        {
            if (!IsInitFromServer())
                return null;
            if (!StorageDic.TryGetValue(ActivityId,out var storage))
            {
                storage = new StorageShopExtraReward();
                StorageDic.Add(ActivityId,storage);
            }
            return storage;
        }
    }
    public override string Guid => "OPS_EVENT_TYPE_SHOP_EXTRA_REWARD";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public Dictionary<int, ShopExtraRewardConfig> ExtraRewardConfig = new Dictionary<int, ShopExtraRewardConfig>();
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        ShopExtraRewardConfigManager.Instance.InitConfig(configJson);
        var config = ShopExtraRewardConfigManager.Instance.GetConfig<ShopExtraRewardConfig>();
        for (var i = 0; i < config.Count; i++)
        {
            var shopId = config[i].Id;
            if (!ExtraRewardConfig.ContainsKey(config[i].Id))
            {
                ExtraRewardConfig.Add(shopId,config[i]);
            }
            else
            {
                ExtraRewardConfig[shopId] = config[i];
            }
        }
        _lastActivityOpenState = IsOpened();
        InitServerDataFinish();
    }
    public ShopExtraRewardModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }
    private bool _lastActivityOpenState;//记录上一帧的活动开启状态，在轮询中判断是否触发开启活动或者关闭活动
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;

        var currentActivityOpenState = IsOpened();
        if (_lastActivityOpenState == currentActivityOpenState)
            return;
        if (!currentActivityOpenState)
        {
            EventDispatcher.Instance.SendEventImmediately(new EventShopExtraRewardEnd());
        }
        else
        {
        }
        _lastActivityOpenState = currentActivityOpenState;
    }

    public List<ResData> GetExtraReward(int shopId)
    {
        var extraReward = new List<ResData>();
        if (!IsOpened())
            return extraReward;
        if (!ExtraRewardConfig.TryGetValue(shopId, out var tempExtraReward))
        {
            return extraReward;
        }
        if (CurStorage.BuyState.TryGetValue(shopId,out var count))
        {
            if (count >= tempExtraReward.BuyTimes)
                return extraReward;
        }
        extraReward = CommonUtils.FormatReward(tempExtraReward.RewardId, tempExtraReward.RewardNum);
        return extraReward;
    }

    public int GetLeftBuyTimes()
    {
        if (!IsOpened())
            return 0;
        var leftCount = 0;
        foreach (var pair in ExtraRewardConfig)
        {
            leftCount += pair.Value.BuyTimes;
            if (CurStorage.BuyState.TryGetValue(pair.Key, out var count))
            {
                leftCount -= count;
            }
        }
        return leftCount;
    }
    public bool IsPrivateOpened()
    {
        if (!IsOpened())
            return false;
        return GetLeftBuyTimes() > 0;
    }
    public void ReceiveExtraReward(int shopId)
    {
        if (!CurStorage.BuyState.ContainsKey(shopId))
        {
            CurStorage.BuyState.Add(shopId,0);
        }
        CurStorage.BuyState[shopId]++;
    }

    private bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.ShopExtraReward);
    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock;
    }

    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.ShopExtraReward);
    }

    public const string coolTimeKey = "ShopExtraReward";
    public static bool CanShowStartPopupEachDay()
    {
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            return false;
        if (CanShowStartPopup())
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,CommonUtils.GetTimeStamp());
            return true;
        }
        return false;
    }

    public static bool CanShowStartPopup()
    {
        if (Instance.IsPrivateOpened())
        {
            UIPopupShopExtraRewardStartController.Open();
            return true;
        }
        return false;
    }
}