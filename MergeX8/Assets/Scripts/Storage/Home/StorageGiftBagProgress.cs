/************************************************
 * Storage class : StorageGiftBagProgress
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageGiftBagProgress : StorageBase
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
        
        // 是否购买了礼包
        [JsonProperty]
        bool buyState;
        [JsonIgnore]
        public bool BuyState
        {
            get
            {
                return buyState;
            }
            set
            {
                if(buyState != value)
                {
                    buyState = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 开始时间戳(如果活动开启则进行更新，否则用本地存的)
        [JsonProperty]
        long startTime;
        [JsonIgnore]
        public long StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                if(startTime != value)
                {
                    startTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 结束时间戳(同STARTTIME)
        [JsonProperty]
        long endTime;
        [JsonIgnore]
        public long EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                if(endTime != value)
                {
                    endTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 分组
        [JsonProperty]
        int globalConfig;
        [JsonIgnore]
        public int GlobalConfig
        {
            get
            {
                return globalConfig;
            }
            set
            {
                if(globalConfig != value)
                {
                    globalConfig = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}