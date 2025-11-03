/************************************************
 * Storage class : StorageLevelUpPackageSinglePackage
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageLevelUpPackageSinglePackage : StorageBase
    {
        
        // 礼包ID
        [JsonProperty]
        int packageId;
        [JsonIgnore]
        public int PackageId
        {
            get
            {
                return packageId;
            }
            set
            {
                if(packageId != value)
                {
                    packageId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 开始时间
        [JsonProperty]
        ulong startTime;
        [JsonIgnore]
        public ulong StartTime
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
        
        // 结束时间
        [JsonProperty]
        ulong endTime;
        [JsonIgnore]
        public ulong EndTime
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
        
        // 购买次数
        [JsonProperty]
        int buyTimes;
        [JsonIgnore]
        public int BuyTimes
        {
            get
            {
                return buyTimes;
            }
            set
            {
                if(buyTimes != value)
                {
                    buyTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 礼包等级
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
        
    }
}