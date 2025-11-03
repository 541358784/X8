/************************************************
 * Storage class : StorageTotalRecharge
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTotalRecharge : StorageBase
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
        
        // 新手累计充值是否开启
        [JsonProperty]
        bool isOpenNewbie;
        [JsonIgnore]
        public bool IsOpenNewbie
        {
            get
            {
                return isOpenNewbie;
            }
            set
            {
                if(isOpenNewbie != value)
                {
                    isOpenNewbie = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}