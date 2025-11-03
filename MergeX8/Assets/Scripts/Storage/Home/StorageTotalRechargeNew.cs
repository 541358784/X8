/************************************************
 * Storage class : StorageTotalRechargeNew
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTotalRechargeNew : StorageBase
    {
        
        // 累充（美分）
        [JsonProperty]
        int totalRecharge;
        [JsonIgnore]
        public int TotalRecharge
        {
            get
            {
                return totalRecharge;
            }
            set
            {
                if(totalRecharge != value)
                {
                    totalRecharge = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 分组
        [JsonProperty]
        int payLevelGroup;
        [JsonIgnore]
        public int PayLevelGroup
        {
            get
            {
                return payLevelGroup;
            }
            set
            {
                if(payLevelGroup != value)
                {
                    payLevelGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 领奖状态
        [JsonProperty]
        StorageList<int> collectGroups = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CollectGroups
        {
            get
            {
                return collectGroups;
            }
        }
        // ---------------------------------//
        
        // 付费分组
        [JsonProperty]
        int gruopId;
        [JsonIgnore]
        public int GruopId
        {
            get
            {
                return gruopId;
            }
            set
            {
                if(gruopId != value)
                {
                    gruopId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否完成
        [JsonProperty]
        bool isFinish;
        [JsonIgnore]
        public bool IsFinish
        {
            get
            {
                return isFinish;
            }
            set
            {
                if(isFinish != value)
                {
                    isFinish = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 加入时间
        [JsonProperty]
        long joinTime;
        [JsonIgnore]
        public long JoinTime
        {
            get
            {
                return joinTime;
            }
            set
            {
                if(joinTime != value)
                {
                    joinTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 配置ID
        [JsonProperty]
        StorageList<StorageTotalRechargeReward> configs = new StorageList<StorageTotalRechargeReward>();
        [JsonIgnore]
        public StorageList<StorageTotalRechargeReward> Configs
        {
            get
            {
                return configs;
            }
        }
        // ---------------------------------//
        
    }
}