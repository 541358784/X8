using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.DogPlayExtraReward;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public class DogPlayExtraRewardModel :ActivityEntityBase
{
    private static DogPlayExtraRewardModel _instance;
    public static DogPlayExtraRewardModel Instance => _instance ?? (_instance = new DogPlayExtraRewardModel());
    
    public override string Guid => "OPS_EVENT_TYPE_DOG_PLAY_EXTRA_REWARD";
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    private List<DogPlayExtraRewardRewardConfig> RewardConfig =>
        DogPlayExtraRewardConfigManager.Instance.GetConfig<DogPlayExtraRewardRewardConfig>();
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DogPlayExtraRewardConfigManager.Instance.InitConfig(configJson);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
    }
    public override bool IsOpened(bool hasLog = false)
    {
        return UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DogPlayExtraReward) //已解锁
               && DogPlayModel.Instance.IsOpen()
               && base.IsOpened(hasLog);
    }

    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.DogPlayExtraReward);
    }

    public StorageDogPlayExtraReward Storage =>
        StorageManager.Instance.GetStorage<StorageHome>().DogPlayExtraReward;

    public bool CanShowStart()
    {
        if (IsOpened() && 
            GetExtraRewards(DogPlayModel.Instance.Storage).Count>0 &&
            !Storage.IsStart)
        {
            Storage.IsStart = true;
            UIPopupDogPlayExtraRewardController.Open();
            return true;
        }
        return false;
    }

    public List<ResData> GetExtraRewards(StorageDogPlay storage)
    {
        var rewards = new List<ResData>();
        if (storage.CurCount >= storage.MaxCount)
        {
            if (Storage.DogPlayOrderId == storage.CurConfigId)
            {
                foreach (var reward in Storage.Rewards)
                {
                    rewards.Add(new ResData(reward.Id,reward.Count));
                }
            }
        }
        else if(IsOpened())
        {
            var rewardConfig = RewardConfig.Find(a => a.DogPlayOrderId == storage.CurConfigId);
            if (rewardConfig != null)
            {
                rewards = CommonUtils.FormatReward(rewardConfig.RewardId, rewardConfig.RewardNum);
            }
        }
        return rewards;
    }

    public void InitStorage()
    {
        if (Storage.ActivityId != ActivityId)
        {
            Storage.ActivityId = ActivityId;
            Storage.IsStart = false;
            Storage.Rewards.Clear();
            Storage.DogPlayOrderId = 0;
        }
    }
    public void OnDogPlayOrderCompleted(int dogPlayOrderId)
    {
        if (!IsInitFromServer())
            return;
        if (!IsOpened())
            return;
        var rewardConfig = RewardConfig.Find(a => a.DogPlayOrderId == dogPlayOrderId);
        if (rewardConfig != null)
        {
            var rewards = CommonUtils.FormatReward(rewardConfig.RewardId, rewardConfig.RewardNum);
            foreach (var reward in rewards)
            {
                Storage.Rewards.Add(new StorageResData()
                {
                    Id = reward.id,
                    Count = reward.count,
                });
            }
            Storage.DogPlayOrderId = dogPlayOrderId;
        }
    }

    public void OnCollectExtraRewards()
    {
        Storage.DogPlayOrderId = 0;
        Storage.Rewards.Clear();
    }
}