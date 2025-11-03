/************************************************
 * Storage class : StorageActiveData
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageActiveData : StorageBase
    {
        
        // 上次活跃时间
        [JsonProperty]
        ulong lastActiveTime;
        [JsonIgnore]
        public ulong LastActiveTime
        {
            get
            {
                return lastActiveTime;
            }
            set
            {
                if(lastActiveTime != value)
                {
                    lastActiveTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 总活跃天数
        [JsonProperty]
        uint totalActiveDays;
        [JsonIgnore]
        public uint TotalActiveDays
        {
            get
            {
                return totalActiveDays;
            }
            set
            {
                if(totalActiveDays != value)
                {
                    totalActiveDays = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最近连续活跃天数
        [JsonProperty]
        uint continuousActiveDays;
        [JsonIgnore]
        public uint ContinuousActiveDays
        {
            get
            {
                return continuousActiveDays;
            }
            set
            {
                if(continuousActiveDays != value)
                {
                    continuousActiveDays = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活跃期间付费总额（美分）
        [JsonProperty]
        ulong activeRevenueUSDCents;
        [JsonIgnore]
        public ulong ActiveRevenueUSDCents
        {
            get
            {
                return activeRevenueUSDCents;
            }
            set
            {
                if(activeRevenueUSDCents != value)
                {
                    activeRevenueUSDCents = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}