using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.NewDailyPackageExtraReward;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;

public class NewDailyPackageExtraRewardModel : ActivityEntityBase
{

    public bool ShowEntrance()
    {
        return IsPrivateOpened();
    }
    private static NewDailyPackageExtraRewardModel _instance;
    public static NewDailyPackageExtraRewardModel Instance => _instance ?? (_instance = new NewDailyPackageExtraRewardModel());

    public StorageNewDailyPackageExtraReward CurStorage => StorageManager.Instance.GetStorage<StorageHome>().NewDailyPackageExtraReward;
    
    public override string Guid => "OPS_EVENT_TYPE_NEW_DAILY_PACKAGE_EXTRA_REWARD";
    public static string Guid2 => "OPS_EVENT_TYPE_NEW_DAILY_PACKAGE_EXTRA_REWARD2";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
        if (ActivityManager.Instance._activityModules.ContainsKey(Guid2))
        {
            Debug.LogError($"register {Guid2} to config hub repeated.");
            return;
        }
        ActivityManager.Instance._activityModules[Guid2] = Instance;
    }

    public NewDailyPackageExtraRewardGlobalConfig GlobalConfig => NewDailyPackageExtraRewardConfigManager.Instance
        .GetConfig<NewDailyPackageExtraRewardGlobalConfig>()[0];
    public Dictionary<int, NewDailyPackageExtraRewardConfig> ExtraRewardConfig = new Dictionary<int, NewDailyPackageExtraRewardConfig>();
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        NewDailyPackageExtraRewardConfigManager.Instance.InitConfig(configJson);
        var config = NewDailyPackageExtraRewardConfigManager.Instance.GetConfig<NewDailyPackageExtraRewardConfig>();
        for (var i = 0; i < config.Count; i++)
        {
            var shopId = config[i].Id;
            if (config[i].RewardId != null && config[i].RewardId.Count > 0)
            {
                if (!ExtraRewardConfig.ContainsKey(config[i].Id))
                {
                    ExtraRewardConfig.Add(shopId,config[i]);
                }
                else
                {
                    ExtraRewardConfig[shopId] = config[i];
                }   
            }
        }
        InitStorage();
        _lastActivityOpenState = IsOpened();
        CurStorage.AddSkinUIWindowInfo();
    }
    public NewDailyPackageExtraRewardModel()
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
            EventDispatcher.Instance.SendEventImmediately(new EventNewDailyPackageExtraRewardEnd());
        }
        else
        {
        }
        _lastActivityOpenState = currentActivityOpenState;
    }

    public void InitStorage()
    {
        if (ActivityId != CurStorage.ActivityId)
        {
            CurStorage.Clear();
            CurStorage.ActivityId = ActivityId;
            CurStorage.SkinName = GlobalConfig.SkinName;
        }
    }

    public List<ResData> GetExtraReward(int packageId)
    {
        var extraReward = new List<ResData>();
        if (!IsOpened())
            return extraReward;
        if (!ExtraRewardConfig.TryGetValue(packageId, out var tempExtraReward))
        {
            return extraReward;
        }
        extraReward = CommonUtils.FormatReward(tempExtraReward.RewardId, tempExtraReward.RewardNum);
        return extraReward;
    }

    public int GetLeftBuyTimes()
    {
        if (!IsOpened())
            return 0;
        return GlobalConfig.BuyTimes - CurStorage.BuyTimes;
    }
    public bool IsPrivateOpened()
    {
        if (!IsOpened())
            return false;
        return GetLeftBuyTimes() > 0;
    }
    public void ReceiveExtraReward(int packageId)
    {
        CurStorage.BuyTimes++;
    }

    public void PurchasePackage(int packageId)
    {
        var extraReward = GetExtraReward(packageId);
        if (extraReward.Count == 0)
            return;
        ReceiveExtraReward(packageId);
        var reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.NewDailyDealsSuccess;
        CommonRewardManager.Instance.PopCommonReward(extraReward,
            CurrencyGroupManager.Instance.GetCurrencyUseController(), true, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = reason,
            }
            , () =>
            {
            });
    }
    private bool IsUnlock
    {
        get
        {
            if (!IsInitFromServer())
                return false;
            return ExperenceModel.Instance.GetLevel() >= GlobalConfig.UnlockLevel;
            //return UnlockManager.IsOpen(UnlockManager.MergeUnlockType.NewDailyPackageExtraReward);
        }
    }

    public override bool IsOpened(bool hasLog = false)
    {
        if (!base.IsOpened(hasLog) || !IsUnlock)
        {
            return false;
        }
        if (CurStorage.BuyTimes >= GlobalConfig.BuyTimes)
            return false;
        var packageList = NewDailyPackModel.Instance.GetCanBuyPackageIdList();
        foreach (var package in packageList)
        {
            if (ExtraRewardConfig.ContainsKey(package))
            {
                return true;
            }
        }
        return false;
    }

    public override bool CanDownLoadRes()
    {
        if (!IsInitFromServer())
            return false;
        return ExperenceModel.Instance.GetLevel() >= GlobalConfig.UnlockLevel-2;
        // return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.NewDailyPack);
    }

    public const string coolTimeKey = "NewDailyPackageExtraReward";
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
            UINewDailyPackageExtraRewardController.Open(Instance.CurStorage);
            return true;
        }
        return false;
    }
    public static string[] ShowMainUIList()
    {
        return new[] {Instance.CurStorage.GetAssetPathWithSkinName(UINameConst.UINewDailyPackageExtraReward)};
    }
    
    
    
    
    #region  Entrance
    public bool ShowAuxItem()
    {
        return IsOpened();
    }

    public Aux_NewDailyPackageExtraReward GetAuxItem()
    {
        return Aux_NewDailyPackageExtraReward.Instance;
    }
    public string GetAuxItemAssetPath()
    {
        if (!IsInitFromServer())
            return null;
        return CurStorage.GetAuxItemAssetPath();
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Home/TaskList_NewDailyPackageExtraReward";
    }
    #endregion
}