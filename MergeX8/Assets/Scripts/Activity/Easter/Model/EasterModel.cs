using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.Easter;
using DragonPlus.Config.PayRebate;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class EasterModel : ActivityEntityBase
{
    private static EasterModel _instance;
    public static EasterModel Instance => _instance ?? (_instance = new EasterModel());


    private StorageEaster _storageEaster;

    public StorageEaster StorageEaster
    {
        get
        {
            var storage = StorageManager.Instance.GetStorage<StorageHome>().Easter;
            if (_storageEaster == null)
            {
                if (!storage.ContainsKey(StorageKey))
                    storage.Add(StorageKey, new StorageEaster());
                _storageEaster = storage[StorageKey];
            }
           
            return _storageEaster;
        }
    }

    public override string Guid => "OPS_EVENT_TYPE_EASTER";

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
        EasterConfigManager.Instance.InitConfig(configJson);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
    }

    public override void UpdateActivityState()
    {
        InitServerDataFinish();
    }
    public bool IsCanClearEaster()
    {
        if (IsInitFromServer())
            return !IsOpened();

        return IsActivityEnd();
    }
    
    public bool IsActivityEnd()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Easter))
            return false;
        
        List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().Easter.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            if (StorageManager.Instance.GetStorage<StorageHome>().Easter[keys[i]].EndTime > 0)
                return (long)APIManager.Instance.GetServerTime() >= StorageManager.Instance.GetStorage<StorageHome>().Easter[keys[i]].EndTime;
        }

        return true;
    }
    
    protected override void InitServerDataFinish()
    {
        _storageEaster = null;
        StorageEaster.EndTime = (long)EndTime;
    }

    public bool IsOpened()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Easter))
            return false;
        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;
      
        if (StorageEaster.IsShowEndView)
            return false;

        return true;
    }
    public bool IsStart()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Easter))
            return false;
        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;
      
        if (StorageEaster.IsShowEndView)
            return false;
    
        if (!StorageEaster.IsShowStartView)
            return false;

        return true;
    }
    
    // 活动剩余预热时间的字符串显示
    public virtual string GetActivityPreheatLeftTimeString()
    {
        return CommonUtils.FormatLongToTimeStr((long) GetActivityPreheatLeftTime());
    }

    public ulong GetActivityPreheatLeftTime()
    {
        var easterConfig = GetEasterConfig();
        if (easterConfig == null)
            return 0;
        var left =(ulong)easterConfig.PreheatTime * 3600 * 1000- (APIManager.Instance.GetServerTime()-  StartTime);
        if (left < 0)
            left = 0;
        return left;
    }
    public bool IsPreheating()
    {
        if (IsSkipActivityPreheating())
            return false;
        ulong serverTime =APIManager.Instance.GetServerTime();
        var easterConfig = GetEasterConfig();
        if (easterConfig == null)
            return false;
        if ( serverTime-StartTime <=(ulong)easterConfig.PreheatTime * 3600 * 1000)
            return true;
        
        return false;
    }
    public bool IsShowStart()
    {
        return StorageEaster.IsShowStartView;
    }
    public bool IsComplete()
    {
        var lastData = GetMaxIndexData();
        return GetScore() >= lastData.Score;
    }
    public EasterReward GetMaxIndexData()
    {
        var reward = GetEasterReward();
        return reward[reward.Count-1];
    }
    public void AddScore(int score)
    {
        StorageEaster.TotalScore += score;
        
        var config = GetCurIndexData();
        if(StorageEaster.TotalScore >= config.Score && StorageEaster.CurIndex < GetEasterReward().Count-1)
            StorageEaster.CurIndex++;

        if (StorageEaster.CurIndex == GetEasterReward().Count - 1)
            StorageEaster.TotalScore = Math.Min(config.Score, StorageEaster.TotalScore);
       
    }
    
    public bool CanManualActivity()
    {
        if (!IsOpened())
            return false;

        if (StorageEaster.IsManualActivity)
            return false;
        
        var config = GetCurIndexData();
        if (StorageEaster.CurIndex ==GetEasterReward().Count - 1 &&
            StorageEaster.TotalScore >= config.Score && !IsHaveCanClaim())
            return true;

        return false;
    }

    public bool IsMax()
    {
        var config = GetCurIndexData();
        if (StorageEaster.CurIndex ==GetEasterReward().Count - 1 &&
            StorageEaster.TotalScore >= config.Score)
            return true;

        return false;
    }
    public List<EasterReward> GetCanClaimRewards(StorageEaster storage)
    {
        List<EasterReward> tempList = new List<EasterReward>();
        var rewards = GetEasterReward();
        if(rewards==null )
            return tempList;
        for (int i = 0; i < rewards.Count; i++)
        {
            if (storage.TotalScore >= rewards[i].Score)
            {
                if (!storage.Reward.ContainsKey(rewards[i].Id))
                    tempList.Add(rewards[i]);
            }
        }

        return tempList;
    }
    public bool IsHaveCanClaim()
    {
        var rewards = GetEasterReward();
        for (int i = 0; i < rewards.Count; i++)
        {
            EasterMainCell.RewardStatus status = EasterMainCell.RewardStatus.None;
            if (GetScore() >= rewards[i].Score)
            {
                if (!IsClaimed(rewards[i].Id))
                    return true;
            }
        }
        return false;
    }
    public int GetCanClaimIndex()
    {
        var rewards = GetEasterReward();
        for (int i = 0; i < rewards.Count; i++)
        {
            if (GetScore() >= rewards[i].Score)
            {
                if (!IsClaimed(rewards[i].Id))
                    return i;
            }
        }
        return -1;
    }
    public bool IsClaimed(int rID)
    {
        return StorageEaster.Reward.ContainsKey(rID);
    }
    
    public void Claim(int rID)
    {
        
        if(!StorageEaster.Reward.ContainsKey(rID))
            StorageEaster.Reward.Add(rID,true);
        if (CanManualActivity())
            StorageEaster.IsManualActivity = true;
        EventDispatcher.Instance.DispatchEvent(EventEnum.EASTER_CLAIM);

    }

    public int GetCurIndex()
    {
        return StorageEaster.CurIndex;
    }
    public void StartActivity()
    {
        StorageEaster.IsShowStartView = true;
        
    }
    
    public void EndActivity(bool isManual = false)
    {
        StorageEaster.IsShowEndView = false;
        StorageEaster.IsManualActivity = isManual;
    }
    public int GetScore()
    {
        return StorageEaster.TotalScore;
    }
    public int GetCurStageScore()
    {
        var preData = GetPreIndexData();
        return GetScore() - (preData == null ? 0 : preData.Score);
    }
    public int GetIndexStageScore(int index = -1)
    {
        index = index < 0 ? StorageEaster.CurIndex : index;
        var curData = GetCurIndexData(index);
        if (curData == null)
            return 0;
        
        var preData = index == 0 ? null : GetPreIndexData(index);

        int preStateScore = preData == null ? 0 : preData.Score;

        return curData.Score - preStateScore;
    }
    public EasterReward GetCurIndexData(int index = -1)
    {
        index = index < 0 ? StorageEaster.CurIndex : index;
        
        return GetGetEasterRewardByIndex(index);
    }
    public EasterReward GetPreIndexData(int index = -1)
    {
        index = index < 0 ? StorageEaster.CurIndex-1 : index - 1;
        
        return GetGetEasterRewardByIndex(index);
    }

    public EasterReward GetGetEasterRewardByIndex(int index)
    {
        var configs=  EasterConfigManager.Instance.GetConfig<EasterReward>();
        if (configs==null || index < 0 || index >= configs.Count)
            return null;

        return configs[index];
    }

    public EasterConfig GetEasterConfig()
    {
        var configs=EasterConfigManager.Instance.GetConfig<EasterConfig>();
        if (configs == null || configs.Count <= 0)
            return null;
        return configs[0];
    }

    public List<EasterReward> GetEasterReward()
    {
        return  EasterConfigManager.Instance.GetConfig<EasterReward>();
    }
    public StorageEaster CheckActivityEnd()
    {
        var easter = StorageManager.Instance.GetStorage<StorageHome>().Easter;
        List<string> keys = easter.Keys.ToList();
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            StorageEaster lastActivity = easter[keys[i]];
            if(lastActivity.IsShowEndView)
                continue;
           
            if(!lastActivity.IsShowStartView)
                continue;

            if(lastActivity.EndTime == 0)
                continue;
            
            if ((long)APIManager.Instance.GetServerTime() > lastActivity.EndTime)
                return lastActivity;
        }

        return null;
    }

    public void UpdateActivity()
    {
        StorageEaster cleanupData = CheckActivityEnd();
        if (cleanupData == null)
            return;
    
        UIEasterEndController mainController = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIEasterEnd) as UIEasterEndController;
        if(mainController != null)
            return;
        
        var end =UIManager.Instance.OpenUI(UINameConst.UIEasterEnd, cleanupData);
        if (end == null)
            cleanupData.IsShowEndView = true;
    }
    
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.Easter);
    }
}