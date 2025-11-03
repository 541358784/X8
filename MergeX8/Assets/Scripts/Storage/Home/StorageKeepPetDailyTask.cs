/************************************************
 * Storage class : StorageKeepPetDailyTask
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageKeepPetDailyTask : StorageBase
    {
        
        // 目标收集状态
        [JsonProperty]
        StorageDictionary<int,int> targetCollectState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> TargetCollectState
        {
            get
            {
                return targetCollectState;
            }
        }
        // ---------------------------------//
        
        // 目标消耗状态
        [JsonProperty]
        StorageDictionary<int,int> targetConsumeState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> TargetConsumeState
        {
            get
            {
                return targetConsumeState;
            }
        }
        // ---------------------------------//
        
        // 特殊目标状态
        [JsonProperty]
        StorageDictionary<int,int> specialTargetState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> SpecialTargetState
        {
            get
            {
                return specialTargetState;
            }
        }
        // ---------------------------------//
        
        // 是否领取最终奖励
        [JsonProperty]
        bool isCollectFinalReward;
        [JsonIgnore]
        public bool IsCollectFinalReward
        {
            get
            {
                return isCollectFinalReward;
            }
            set
            {
                if(isCollectFinalReward != value)
                {
                    isCollectFinalReward = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已经领奖的等级
        [JsonProperty]
        StorageList<int> alreadyCollectLevels = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> AlreadyCollectLevels
        {
            get
            {
                return alreadyCollectLevels;
            }
        }
        // ---------------------------------//
        
        // 可领奖的等级
        [JsonProperty]
        StorageList<int> canCollectLevels = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CanCollectLevels
        {
            get
            {
                return canCollectLevels;
            }
        }
        // ---------------------------------//
        
        // 未领取的奖励
        [JsonProperty]
        StorageDictionary<int,int> unCollectRewards = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> UnCollectRewards
        {
            get
            {
                return unCollectRewards;
            }
        }
        // ---------------------------------//
        
        // 新解锁任务当天被锁数值差
        [JsonProperty]
        StorageDictionary<int,int> unlockReduceValue = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> UnlockReduceValue
        {
            get
            {
                return unlockReduceValue;
            }
        }
        // ---------------------------------//
        
    }
}