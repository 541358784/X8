/************************************************
 * Storage class : StorageDiamondReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDiamondReward : StorageBase
    {
        
        // 活动ID
        [JsonProperty]
        string activityId = "";
        [JsonIgnore]
        public string ActivityId
        {
            get
            {
                return activityId;
            }
            set
            {
                if(activityId != value)
                {
                    activityId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 池状态
        [JsonProperty]
        StorageDictionary<int,int> poolStatus = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> PoolStatus
        {
            get
            {
                return poolStatus;
            }
        }
        // ---------------------------------//
        
        // 池ID
        [JsonProperty]
        int poolId;
        [JsonIgnore]
        public int PoolId
        {
            get
            {
                return poolId;
            }
            set
            {
                if(poolId != value)
                {
                    poolId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 池索引
        [JsonProperty]
        int poolIndex;
        [JsonIgnore]
        public int PoolIndex
        {
            get
            {
                return poolIndex;
            }
            set
            {
                if(poolIndex != value)
                {
                    poolIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动开始时间
        [JsonProperty]
        long startActivityTime;
        [JsonIgnore]
        public long StartActivityTime
        {
            get
            {
                return startActivityTime;
            }
            set
            {
                if(startActivityTime != value)
                {
                    startActivityTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动结束时间
        [JsonProperty]
        long activityEndTime;
        [JsonIgnore]
        public long ActivityEndTime
        {
            get
            {
                return activityEndTime;
            }
            set
            {
                if(activityEndTime != value)
                {
                    activityEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是领取所有奖励
        [JsonProperty]
        bool isGetAllReward;
        [JsonIgnore]
        public bool IsGetAllReward
        {
            get
            {
                return isGetAllReward;
            }
            set
            {
                if(isGetAllReward != value)
                {
                    isGetAllReward = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否忽略弹窗
        [JsonProperty]
        bool isIgnorePopUI;
        [JsonIgnore]
        public bool IsIgnorePopUI
        {
            get
            {
                return isIgnorePopUI;
            }
            set
            {
                if(isIgnorePopUI != value)
                {
                    isIgnorePopUI = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 等级
        [JsonProperty]
        int level;
        [JsonIgnore]
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if(level != value)
                {
                    level = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 可以升级
        [JsonProperty]
        bool canUpgrade;
        [JsonIgnore]
        public bool CanUpgrade
        {
            get
            {
                return canUpgrade;
            }
            set
            {
                if(canUpgrade != value)
                {
                    canUpgrade = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}