using System;
using System.Collections.Generic;
using System.Linq;
using Decoration;
using DragonPlus;
using DragonPlus.Config.Easter;
using DragonPlus.Config.Mermaid;
using DragonPlus.Config.PayRebate;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class MermaidModel : ActivityEntityBase
{
    private static MermaidModel _instance;
    public static MermaidModel Instance => _instance ?? (_instance = new MermaidModel());

    private MermaidConfig _mermaidConfig;
    public MermaidConfig MermaidConfig
    {
        get
        {
            if (_mermaidConfig == null)
            {
                _mermaidConfig = GetMermaidConfig();
            }

            return _mermaidConfig;
        }
    }
    
    private StorageMermaid _storageMermaid;

    public StorageMermaid StorageMermaid
    {
        get
        {
            var storage = StorageManager.Instance.GetStorage<StorageHome>().Mermaid;
            if (_storageMermaid == null)
            {
                if (!storage.ContainsKey(StorageKey))
                    storage.Add(StorageKey, new StorageMermaid());
                _storageMermaid = storage[StorageKey];
            }
           
            return _storageMermaid;
        }
    }

    public override string Guid => "OPS_EVENT_TYPE_MERMAID";

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
        MermaidConfigManager.Instance.InitConfig(configJson);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
    }

    public override void UpdateActivityState()
    {
        InitServerDataFinish();
    }

    protected override void InitServerDataFinish()
    {
        _storageMermaid = null;
        if(MermaidConfig == null)
            return;
        
        if (!StorageMermaid.IsBuyEntendDay)
        {
            StorageMermaid.EndTime = (long) EndTime - MermaidConfig.ExtendBuyWaitTime * 3600 * 1000 -
                                     MermaidConfig.ExtendBuyTime * 3600 * 1000;
        }
        else
        {
            StorageMermaid.EndTime = StorageMermaid.BuyExtendTime + MermaidConfig.ExtendBuyTime * 3600 * 1000;
        }
        
        //继承上次兑换最多的存档~
        if (!StorageMermaid.IsExtend && !IsFinished())
        {
            var storage = StorageManager.Instance.GetStorage<StorageHome>().Mermaid;
            StorageMermaid mermaid=null;
            foreach (var key in storage.Keys)
            {
                if (key != StorageKey && storage[key].ExchangeCount>0)
                {
                    if (mermaid == null)
                    {
                        mermaid = storage[key];
                    }
                    else  if ( storage[key].ExchangeCount > mermaid.ExchangeCount)
                    {
                        mermaid = storage[key];

                    }
                }
            }

            if (mermaid != null)
            {
                StorageMermaid.ExchangeCount = mermaid.ExchangeCount;
                foreach (var key in mermaid.Reward.Keys)
                {
                    StorageMermaid.Reward.Add(key,true);
                }   
                foreach (var key in mermaid.ExchangeReward.Keys)
                {
                    StorageMermaid.ExchangeReward.Add(key,true);
                }

                StorageMermaid.IsExtend = true;
            }

        }
 
    }

    public bool IsExtend()
    {
        return StorageMermaid.IsExtend;
    }

    public int GetExtendMultiple()
    {
        if (IsExtend())
            return 3;
        return 1;
    }
    public void PurchaseSuccess()
    {
        StorageMermaid.BuyExtendTime = (long)APIManager.Instance.GetServerTime();
        StorageMermaid.IsBuyEntendDay = true;
        StorageMermaid.EndTime = StorageMermaid.BuyExtendTime+MermaidConfig.ExtendBuyTime * 3600 * 1000;
        EventDispatcher.Instance.DispatchEvent(EventEnum.MERMAID_PURCHASE_SUCCESS);
    }
    public bool IsOpened()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Mermaid))
            return false;
        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;
        if(IsFinished())
           return false;
        var isWait = IsWaitBuyExtendDay();
        if (GetActivityLeftTime() <= 0 && StorageMermaid.IsBuyEntendDay)
            return false;      
        if (GetActivityLeftTime() <= 0 && !isWait)
            return false;
        if (isWait && !IsShowStart())
            return false;
        if (isWait && IsExchangeAll())
            return false;
        return true;
    }

    public bool IsFinished()
    {
        var storage = StorageManager.Instance.GetStorage<StorageHome>().Mermaid;
        var rewards = GetExchangeRewards();
        foreach (var key in storage.Keys)
        {
            if (storage[key].Score > 0)
            {
                if (rewards != null && storage[key].ExchangeCount >= rewards.Count)
                    return true;
            }
        }

        return false;
    }
    
    public bool IsStart()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Mermaid))
            return false;
        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;
      
        if (!StorageMermaid.IsShowStartView)
            return false;
        if (GetActivityLeftTime() <= 0)
            return false;
        if (IsWaitBuyExtendDay() && IsExchangeAll())
            return false;
        return true;
    }
    public bool IsPreheating()
    {
        if (IsSkipActivityPreheating())
            return false;
        ulong serverTime =APIManager.Instance.GetServerTime();
        var config = GetMermaidConfig();
        if (config == null)
            return false;
        if ( serverTime-StartTime <=(ulong)config.PreheatTime * 3600 * 1000)
            return true;
        
        return false;
    }
    // 活动剩余预热时间的字符串显示
    public virtual string GetActivityPreheatLeftTimeString()
    {
        return CommonUtils.FormatLongToTimeStr((long) GetActivityPreheatLeftTime());
    }

    public ulong GetActivityPreheatLeftTime()
    {
        var config = GetMermaidConfig();
        if (config == null)
            return 0;
        var left =(ulong)config.PreheatTime * 3600 * 1000- (APIManager.Instance.GetServerTime()-  StartTime);
        if (left < 0)
            left = 0;
        return left;
    }
    public ulong GetActivityLeftTime()
    {
        var left = StorageMermaid.EndTime - (long) APIManager.Instance.GetServerTime();
        if (left < 0)
            left = 0;
        return (ulong) left;
    }

    public void CheckSendOverBI()
    {
        var storage = StorageManager.Instance.GetStorage<StorageHome>().Mermaid;
        foreach (var keyValue in storage)
        {
            var st = keyValue.Value;
            var left = st.EndTime- (long) APIManager.Instance.GetServerTime();
            if (left < 0 && !st.IsSendOverBi)
            {
                // string data2 = "";
                foreach (var kv in st.Reward)
                {
                    // data2 += kv.Key + ",";
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMermaidOverState,
                        st.TotalScore.ToString(),kv.Key.ToString(),/*st.Score.ToString()*/st.Reward.Count.ToString());
                }

                st.IsSendOverBi = true;
            }
        }

    
    }
    public string GetActivityLeftTimeString()
    {
        return CommonUtils.FormatLongToTimeStr((long) GetActivityLeftTime());
    }

    public bool IsWaitBuyExtendDay()
    {
        if (StorageMermaid.IsBuyEntendDay)
            return false;
        
        return GetActivityExtendBuyWaitLeftTime() > 0;
    }
    public ulong GetActivityExtendBuyWaitLeftTime()
    {
        if ( StorageMermaid.EndTime > (long)APIManager.Instance.GetServerTime())
            return 0;
        var left = MermaidConfig.ExtendBuyWaitTime* 3600 * 1000- ((long) APIManager.Instance.GetServerTime()-(long) StorageMermaid.EndTime) ;
        if (left < 0)
            left = 0;
        return (ulong) left;
    }
    public string GetActivityExtendBuyWaitLeftTimeString()
    {
        return CommonUtils.FormatLongToTimeStr((long) GetActivityExtendBuyWaitLeftTime());
    }

    public bool IsShowStart()
    {
        return StorageMermaid.IsShowStartView;
    }

    public void AddScore(int score)
    {
        StorageMermaid.Score += score;
        StorageMermaid.TotalScore += score;
    }
    public int GetScore()
    {
        return StorageMermaid.Score;
    } 
    public bool IsClaimed(int rID)
    {
        return StorageMermaid.Reward.ContainsKey(rID);
    }
    public void RecordExchange(int rID)
    {
        StorageMermaid.ExchangeCount++;
        if(!StorageMermaid.Reward.ContainsKey(rID))
            StorageMermaid.Reward.Add(rID,true);
    }

    public bool IsCanClaim()
    {
        var configs = GetExchangeRewards();
        if (configs == null)
            return false;
        foreach (var config in configs)
        {
            if(StorageMermaid.Score>= config.ExchangeScore*GetExtendMultiple() && !StorageMermaid.Reward.ContainsKey(config.RewardId))
                return true;
        }
        return false;
    }
    
    public bool ClaimStateReward(Action cb)
    {
        var configs = GetStageRewards();
        bool flag=false;
        foreach (var config in configs)
        {
            if (config.ExchangeTimes == StorageMermaid.ExchangeCount && !StorageMermaid.ExchangeReward.ContainsKey(config.Id))
            {
                flag = true;
                var itemConfig= GameConfigManager.Instance.GetItemConfig(config.RewardId);
                if (itemConfig != null)
                {
                    List<ResData> listResData = new List<ResData>();
                    listResData.Add(new ResData(config.RewardId,config.RewardNum));
                    var reason = BiEventCooking.Types.ItemChangeReason.MermaidDeco;
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonMermaidChestGet,
                        itemAId =config.RewardId,
                        isChange = true,
                    }); 
                    CommonRewardManager.Instance.PopCommonReward(listResData, 
                        CurrencyGroupManager.Instance.GetCurrencyUseController(), true, new GameBIManager.ItemChangeReasonArgs()
                        {
                            reason = reason,
                        }
                        , () =>
                        {
                            cb?.Invoke();
                        });
                }
                else
                {
                    List<ResData> listResData = new List<ResData>();
                    listResData.Add(new ResData(config.RewardId,config.RewardNum));
                    var reason = BiEventCooking.Types.ItemChangeReason.MermaidDeco;
                    CommonRewardManager.Instance.PopCommonReward(listResData, 
                        CurrencyGroupManager.Instance.GetCurrencyUseController(), false, new GameBIManager.ItemChangeReasonArgs()
                        {
                            reason = reason,
                        }
                        , () =>
                        {
                            List<int> temp = new List<int>();
                            temp.Add(config.RewardId);
                            DecoManager.Instance.InstallItem(temp);
                        });
                    // cb?.Invoke();
                }
                StorageMermaid.ExchangeReward.Add(config.Id,true);
                return true;
            }
        }
        if(!flag)
            cb?.Invoke();
        return false;
    }
    

    
    public void StartActivity()
    {
        StorageMermaid.IsShowStartView = true;
        
    }

    public bool IsExchangeAll()
    {
        var rewards = GetExchangeRewards();
        if (rewards != null && StorageMermaid.ExchangeCount >= GetExchangeRewards().Count)
            return true;
        return false;
    }
    public int GetExchangeCount()
    {
        return StorageMermaid.ExchangeCount;
    }   
    public void AddExchangeCount()
    {
        StorageMermaid.ExchangeCount++;
    }

    public MermaidConfig GetMermaidConfig()
    {
        var configs=MermaidConfigManager.Instance.GetConfig<MermaidConfig>();
        if (configs == null || configs.Count <= 0)
            return null;
        return configs[0];
    }

    public int GetTaskValue(StorageTaskItem taskItem, bool isMul)
    {
        var configs = GetTaskRewards();
        if (configs == null)
            return 0;
        int tempPrice = 0;
        foreach (var itemId in taskItem.ItemIds)
        {
            var config = GameConfigManager.Instance.GetItemConfig(itemId);
            if (config == null)
                continue;
            tempPrice += config.price;
        }


        int value = 0;
        foreach (var config in configs)
        {
            if (tempPrice <= config.Max_value)
            {
                value = config.Output;
                break;
            }
        }

        if (isMul && MultipleScoreModel.Instance.IsOpenActivity())
            value = (int)(value * MultipleScoreModel.Instance.GetMultiple(MultipleScoreModel.InfluenceFuncType.Mermaid));
        
        return value;
    }
    
    public List<ExchangeReward> GetExchangeRewards()
    {
        return MermaidConfigManager.Instance.GetConfig<ExchangeReward>();
    }
    
    public List<StageReward> GetStageRewards()
    {
        return MermaidConfigManager.Instance.GetConfig<StageReward>();
    }

    public List<TaskReward> GetTaskRewards()
    {
        return MermaidConfigManager.Instance.GetConfig<TaskReward>();
    }

    private static string MermaidPreheating = "MermaidPreheating";
    private static string coolTimeKey = "Mermaid";
    private static string coolTimeKeyExtend = "MermaidExtend";
    public static bool CanShowUI()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Mermaid))
            return false;

        MermaidModel.Instance.CheckSendOverBI();
        if (!MermaidModel.Instance.IsOpened())
            return false;

        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKeyExtend))
        {
            if (MermaidModel.Instance.IsWaitBuyExtendDay())
            {
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKeyExtend, CommonUtils.GetTimeStamp());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMermaidExtendPop);
                UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidAddDay);
                return true;
            }
        }

        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, MermaidPreheating))
        {
            if (MermaidModel.Instance.IsPreheating())
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidStartPreview);
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, MermaidPreheating, CommonUtils.GetTimeStamp());
                return true;
            }
        }

        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            if (!MermaidModel.Instance.IsPreheating())
            {
                if (!MermaidModel.Instance.IsShowStart())
                {
                    if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidStartPreview);
                        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                        return true;
                    }
                }
                // else
                // {
                //     CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                //     UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidMain);
                //     return true;
                // }
         
            }
        }
        return false;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.Mermaid);
    }
}