/************************************************
 * Storage class : StorageEnergyPackage
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageEnergyPackage : StorageBase
    {
        
        // 弹出次数
        [JsonProperty]
        int popTimes;
        [JsonIgnore]
        public int PopTimes
        {
            get
            {
                return popTimes;
            }
            set
            {
                if(popTimes != value)
                {
                    popTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 单个礼包弹出次数
        [JsonProperty]
        int onPackPopTimes;
        [JsonIgnore]
        public int OnPackPopTimes
        {
            get
            {
                return onPackPopTimes;
            }
            set
            {
                if(onPackPopTimes != value)
                {
                    onPackPopTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次CD时间
        [JsonProperty]
        long lastInCdTime;
        [JsonIgnore]
        public long LastInCdTime
        {
            get
            {
                return lastInCdTime;
            }
            set
            {
                if(lastInCdTime != value)
                {
                    lastInCdTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否购买
        [JsonProperty]
        bool isPurchase;
        [JsonIgnore]
        public bool IsPurchase
        {
            get
            {
                return isPurchase;
            }
            set
            {
                if(isPurchase != value)
                {
                    isPurchase = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 组ID
        [JsonProperty]
        StorageList<int> groupId = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> GroupId
        {
            get
            {
                return groupId;
            }
        }
        // ---------------------------------//
        
    }
}