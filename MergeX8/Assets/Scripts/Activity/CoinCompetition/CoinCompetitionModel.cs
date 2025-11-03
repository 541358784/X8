using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinCompetition;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;

public class CoinCompetitionModel : ActivityEntityBase
{
    public bool ShowEntrance()
    {
        return CoinCompetitionModel.Instance.IsOpened() && CoinCompetitionModel.Instance.IsShowStart() &&
            !CoinCompetitionModel.Instance.StorageCompetition.IsShowEndView;
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/CoinCompetition/Aux_CoinCompetition";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/CoinCompetition/TaskList_CoinCompetition";
    }
    private static CoinCompetitionModel _instance;
    public static CoinCompetitionModel Instance => _instance ?? (_instance = new CoinCompetitionModel());

    private CoinCompetitionConfig _coinCompetitionConfig;
    public CoinCompetitionConfig CoinCompetitionConfig
    {
        get
        {
            if (_coinCompetitionConfig == null)
            {
                _coinCompetitionConfig = GetCoinCompetitionConfig();
            }

            return _coinCompetitionConfig;
        }
    }
    public CoinCompetitionConfig GetCoinCompetitionConfig()
    {
        var configs=CoinCompetitionConfigManager.Instance.GetConfig<CoinCompetitionConfig>();
        if (configs == null || configs.Count <= 0)
            return null;
        return configs[0];
    }
    private StorageCoinCompetition _storageCompetition;

    public StorageCoinCompetition StorageCompetition
    {
        get
        {
            var storage = StorageManager.Instance.GetStorage<StorageHome>().CoinCompetition;
            if (_storageCompetition == null)
            {
                if (!storage.ContainsKey(StorageKey))
                    storage.Add(StorageKey, new StorageCoinCompetition()
                    {
                        StartActivityTime = (long)StartTime,
                        ActivityEndTime = (long)EndTime
                    });
                _storageCompetition = storage[StorageKey];
            }
           
            return _storageCompetition;
        }
    }

    public override string Guid => "OPS_EVENT_TYPE_COIN_COMPETITION";

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
        CoinCompetitionConfigManager.Instance.InitConfig(configJson);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
        IsInited = true;
        StorageCompetition.StartActivityTime = (long)StartTime;
        StorageCompetition.ActivityEndTime = (long)EndTime;
    }

    public bool GetIsInit()
    {
        return IsInited;
    }

    public override void UpdateActivityState()
    {
        InitServerDataFinish();
    }

    protected override void InitServerDataFinish()
    {
  
    }
    
    public bool IsOpened()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CoinCompetition))
            return false;
        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;
      
        return true;
    }
    public bool IsStart()
    {
        if (!IsOpened())
            return false;
      
        if (!StorageCompetition.IsShowStartView)
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
        var config = GetCoinCompetitionConfig();
        if (config == null)
            return 0;
        var left =(ulong)config.PreheatTime * 3600 * 1000- (APIManager.Instance.GetServerTime()-  StartTime);
        if (left < 0)
            left = 0;
        return left;
    }

    public string GetActivityLeftTimeString()
    {
        return CommonUtils.FormatLongToTimeStr((long) GetActivityLeftTime());
    }

  
    public bool IsShowStart()
    {
        return StorageCompetition.IsShowStartView;
    }

    public void AddScore(int score)
    {
        StorageCompetition.TotalScore += score;
        // if (MergeTaskTipsController.Instance.mergeCoinCompetition && 
        //     (MergeTaskTipsController.Instance.contentRect.anchoredPosition.x <
        //     -MergeTaskTipsController.Instance.mergeCoinCompetition.transform.localPosition.x + 220 ||
        //     MergeTaskTipsController.Instance.contentRect.anchoredPosition.x - Screen.width >
        //     -MergeTaskTipsController.Instance.mergeCoinCompetition.transform.localPosition.x + 220))
        // {
        //     MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-MergeTaskTipsController.Instance.mergeCoinCompetition.transform.localPosition.x+220, 0).OnComplete(
        //         () =>
        //         {
        //             EventDispatcher.Instance.DispatchEventImmediately(EventEnum.ADD_COIN,score);
        //         });
        // }
        // else
        // {
        //     EventDispatcher.Instance.DispatchEventImmediately(EventEnum.ADD_COIN,score);
        // }
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.ADD_COIN,score);

        
        var configs = GetCoinCompetitionRewards();
        if (configs != null)
        {
            foreach (var config in configs)
            {
                if (StorageCompetition.TotalScore >= config.Score &&
                    !StorageCompetition.CollectRewardsLevelList.ContainsKey(config.Id))
                {
                    StorageCompetition.CollectRewardsLevelList.Add(config.Id,true);
                    var rewardList = CommonUtils.FormatReward(config.RewardId, config.RewardNum);
                    foreach (var finialReward in rewardList)
                    {
                        if (!StorageCompetition.UnCollectRewards.ContainsKey(finialReward.id))
                        {
                            StorageCompetition.UnCollectRewards.Add(finialReward.id, 0);   
                        }
                        StorageCompetition.UnCollectRewards[finialReward.id] += finialReward.count;
                    }
                }
            }   
        }
    }
    public int GetScore()
    {
        return StorageCompetition.TotalScore;
    } 
    public bool IsClaimed(int rID)
    {
        return StorageCompetition.Reward.ContainsKey(rID);
    }
    public void RecordExchange(CoinCompetitionReward rewardConfig)
    {
        var rID = rewardConfig.Id;
        StorageCompetition.CurIndex=rID;
        if(!StorageCompetition.Reward.ContainsKey(rID))
            StorageCompetition.Reward.Add(rID,true);
        if (StorageCompetition.CollectRewardsLevelList.ContainsKey(rID))
        {
            var rewardList = CommonUtils.FormatReward(rewardConfig.RewardId, rewardConfig.RewardNum);
            foreach (var reward in rewardList)
            {
                if (StorageCompetition.UnCollectRewards.ContainsKey(reward.id))
                {
                    StorageCompetition.UnCollectRewards[reward.id] -= reward.count;
                    if (StorageCompetition.UnCollectRewards[reward.id] <= 0)
                    {
                        StorageCompetition.UnCollectRewards.Remove(reward.id);
                    }
                }
            }
        }
        
    }

    public bool IsCanClaim()
    {
        var configs = GetCoinCompetitionRewards();
        if (configs == null)
            return false;
        foreach (var config in configs)
        {
            if(StorageCompetition.TotalScore>= config.Score && !StorageCompetition.Reward.ContainsKey(config.Id))
                return true;
        }
        return false;
    }

    public CoinCompetitionReward GetFontReward()
    {
        var rewards = GetCoinCompetitionRewards();
        var currentReward = GetCurrentReward();
        if (currentReward == null)
            return null;
        if (currentReward.Id == 1)
            return null;
        return rewards[currentReward.Id - 2];
    }

    public CoinCompetitionReward GetCurrentReward()
    {
        var rewards = GetCoinCompetitionRewards();
        if (rewards == null || rewards.Count <= 0)
            return null;
        for (int i = 0; i < rewards.Count; i++)
        {
            if (!StorageCompetition.Reward.ContainsKey(rewards[i].Id))
                return rewards[i];
        }
        return rewards[rewards.Count - 1];
    }
    
    public CoinCompetitionReward GetNextReward()
    {
        var rewards = GetCoinCompetitionRewards();
        var currentReward = GetCurrentReward();
        if (currentReward == null)
            return null;
        if (currentReward.Id == rewards.Count)
            return null;
        return rewards[currentReward.Id];
    }

    public int GetLevelHaveStore(int level)
    {
        var font = GetRewardStoreByLevel(level - 1);
        return Mathf.Max(0, StorageCompetition.TotalScore - font);
    }
    public int GetLevelNeedStore(int level)
    {
        var cur = GetRewardStoreByLevel(level);
        var font = GetRewardStoreByLevel(level - 1);
        return cur - font;
    }

    public int GetRewardStoreByLevel(int level)
    {
        var rewards = GetCoinCompetitionRewards();
        if (level < 1)
            return 0;
        if (level > rewards.Count)
            level = rewards.Count;
        return rewards[level - 1].Score;
    }    
    public CoinCompetitionReward GetRewardConfigByLevel(int level)
    {
        var rewards = GetCoinCompetitionRewards();
        if (level < 1)
            level = 1;
        if (level > rewards.Count)
            level = rewards.Count;
        return rewards[level - 1];
    }
    
    public float GetCurrentProgress()
    {
       var cur= GetCurrentReward();
       if (cur == null)
           return 0;
       var font = GetFontReward();
       int fontStore = font == null ? 0 : font.Score;

       return (float)(StorageCompetition.TotalScore - fontStore) / ( cur.Score - fontStore);
    }   
    
    public string GetCurrentProgressStr()
    {
       var cur= GetCurrentReward();
       if (cur == null)
           return "";
       var font = GetFontReward();
       int fontStore = font == null ? 0 : font.Score;

       return (StorageCompetition.TotalScore - fontStore) +"/"+ ( cur.Score - fontStore);
    }
    public void StartActivity()
    {
        StorageCompetition.IsShowStartView = true;
        
    }

    public bool IsExchangeAll()
    {
        var rewards = GetCoinCompetitionRewards();
        if (rewards != null && StorageCompetition.CurIndex >= GetCoinCompetitionRewards().Count)
            return true;
        return false;
    }

    public List<CoinCompetitionReward> GetCoinCompetitionRewards()
    {
        return CoinCompetitionConfigManager.Instance.GetConfig<CoinCompetitionReward>();
    }
    public bool IsPreheating()
    {
        if (IsSkipActivityPreheating())
            return false;
        ulong serverTime =APIManager.Instance.GetServerTime();
        var config = GetCoinCompetitionConfig();
        if (config == null)
            return false;
        if ( serverTime-StartTime <=(ulong)config.PreheatTime * 3600 * 1000)
            return true;
        
        return false;
    }

    public void Claim(Action cb,bool isInMain=false)
    {
        List<ResData> list = new List<ResData>();
        var currentReward = GetCurrentReward();
        if (StorageCompetition.TotalScore < currentReward.Score)
            return;
        RecordExchange(currentReward);
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.MERGE_COINCOMPETITION_REFRESH);
        if (currentReward.Id < 11)
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGoldcwarsMissionNormal,currentReward.Id.ToString());
        }
        else
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGoldcwarsMissionChallenge,currentReward.Id.ToString());

        }
        for (int i = 0; i < currentReward.RewardId.Count; i++)
        {
            list.Add(new ResData(currentReward.RewardId[i],currentReward.RewardNum[i]));
            var mergeEventType =currentReward.Id==11? BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonMissionChallenge: BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonMissionNormal;
            if (!UserData.Instance.IsResource(currentReward.RewardId[i]))
            {
                var itemConfig=GameConfigManager.Instance.GetItemConfig(currentReward.RewardId[i]);
                if (itemConfig != null)
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = mergeEventType,
                        itemAId =itemConfig.id,
                        isChange = true,
                    });
                }
            }
        }
        var reason =currentReward.Id==11?  BiEventAdventureIslandMerge.Types.ItemChangeReason.MissionChallenge: BiEventAdventureIslandMerge.Types.ItemChangeReason.MissionNormal;
        CommonRewardManager.Instance.PopCommonReward(list,
            CurrencyGroupManager.Instance.GetCurrencyUseController(), true, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = reason,
            }
            , () =>
            {
                if(isInMain)
                    cb?.Invoke();
                else
                {
                    UIManager.Instance.OpenUI(UINameConst.UICoinCompetitionMain);
                }
            });
     
    }

    private static string Preheating = "CoinCompetitionPreheating";
    private static string coolTimeKey = "CoinCOmpetition";
    
    public static bool CanShowUI()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CoinCompetition))
            return false;

        if (!CoinCompetitionModel.Instance.IsOpened())
            return false;


        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, Preheating))
        {
            if (CoinCompetitionModel.Instance.IsPreheating())
            {
                UIManager.Instance.OpenUI(UINameConst.UICoinCompetitionStart);
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, Preheating, CommonUtils.GetTimeStamp());
                return true;
            }
        }

        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            if (!CoinCompetitionModel.Instance.IsPreheating())
            {
                if (!CoinCompetitionModel.Instance.IsShowStart())
                {
                    UIManager.Instance.OpenUI(UINameConst.UICoinCompetitionStart);
                    CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                    return true;
                
                }
                else
                {
                    CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                    UIManager.Instance.OpenUI(UINameConst.UICoinCompetitionMain);
                    return true;
                }
         
            }
        }
        return false;
    }

    public void Clear()
    {
        var guideIdList = new List<int>() {510, 511, 512, 513, 514};
        var guideFinish = StorageManager.Instance.GetStorage<StorageHome>().Guide.GuideFinished;
        var cacheGuideFinished = GuideSubSystem.Instance.CacheGuideFinished;
        for (var i = 0; i < guideIdList.Count; i++)
        {
            var guideId = guideIdList[i];
            if (guideFinish.ContainsKey(guideId))
            {
                guideFinish.Remove(guideId);
            }
            if (cacheGuideFinished.ContainsKey(guideId))
            {
                cacheGuideFinished.Remove(guideId);
            }
        }
        StorageCompetition.Clear();
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey);
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, Preheating);
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, "LikeUsTime");
        StorageHome storage = StorageManager.Instance.GetStorage<StorageHome>();
        storage.LikeUsFinish = false;
       
    }

    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.CoinCompetition);
    }
    #region UnCollectRewards
    public List<ResData> GetUnCollectRewards()
    {
        var unCollectRewardsList = new List<ResData>();
        List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().CoinCompetition.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var storageCoinCompetition = StorageManager.Instance.GetStorage<StorageHome>().CoinCompetition[keys[i]];
            if (IsCoinCompetitionStorageEnd(storageCoinCompetition))
            {
                foreach (var pair in storageCoinCompetition.UnCollectRewards)
                {
                    if (pair.Value > 0)
                    {
                        unCollectRewardsList.Add(new ResData(pair.Key,pair.Value));
                    }
                }
            }
        }
        return unCollectRewardsList;
    }
    static bool IsCoinCompetitionStorageEnd(StorageCoinCompetition storageCoinCompetition)
    {
        return (long) APIManager.Instance.GetServerTime() >= storageCoinCompetition.ActivityEndTime ||
               (long) APIManager.Instance.GetServerTime() < storageCoinCompetition.StartActivityTime;
    }
    public static bool CanShowUnCollectRewardsUI()
    {
        if (CoinCompetitionModel.Instance.GetUnCollectRewards().Count > 0)
        {
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MissionNormal};
            var unCollectRewards = CoinCompetitionModel.Instance.GetUnCollectRewards();
            CommonRewardManager.Instance.PopActivityUnCollectReward(unCollectRewards, reasonArgs, null, () =>
            {
                for (int i = 0; i < unCollectRewards.Count; i++)
                {
                    if (!UserData.Instance.IsResource(unCollectRewards[i].id))
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonMissionNormal,
                            isChange = false,
                            itemAId = unCollectRewards[i].id
                        });
                    }
                }
                CoinCompetitionModel.Instance.CleanUnCollectRewards();
            });
            // UIManager.Instance.OpenUI(UINameConst.UIClimbTreeUnSelect);
            return true;
        }
        else
        {
            CoinCompetitionModel.Instance.CleanUnCollectRewards();
        }
        return false;
    }
    public void CleanUnCollectRewards()
    {
        List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().CoinCompetition.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var storageCoinCompetition = StorageManager.Instance.GetStorage<StorageHome>().CoinCompetition[keys[i]];
            if (IsCoinCompetitionStorageEnd(storageCoinCompetition))
            {
                CompletedStorageActivity(storageCoinCompetition);
                StorageManager.Instance.GetStorage<StorageHome>().CoinCompetition.Remove(keys[i]);
            }
        }
    }
    public void CompletedStorageActivity(StorageCoinCompetition storage)
    {
        storage.UnCollectRewards.Clear();
    }
    #endregion
}