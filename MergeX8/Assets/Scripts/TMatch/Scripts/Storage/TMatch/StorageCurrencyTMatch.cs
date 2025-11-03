/************************************************
 * Storage class : StorageCurrencyTMatch
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCurrencyTMatch : DragonU3DSDK.Storage.StorageBase
    {
        
        // 去广告
        [JsonProperty]
        bool removeAd;
        [JsonIgnore]
        public bool RemoveAd
        {
            get
            {
                return removeAd;
            }
            set
            {
                if(removeAd != value)
                {
                    removeAd = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
         
        // 玩家拥有的所有资源
        [JsonProperty]
        StorageDictionary<int,StorageSafeCount> userResourceDic = new StorageDictionary<int,StorageSafeCount>();
        [JsonIgnore]
        public StorageDictionary<int,StorageSafeCount> UserResourceDic
        {
            get
            {
                return userResourceDic;
            }
        }
        // ---------------------------------//
        
        // 限时道具   
        [JsonProperty]
        StorageDictionary<int,long> unlimitBuffEndUTCTimeInSecondsDict = new StorageDictionary<int,long>();
        [JsonIgnore]
        public StorageDictionary<int,long> UnlimitBuffEndUTCTimeInSecondsDict
        {
            get
            {
                return unlimitBuffEndUTCTimeInSecondsDict;
            }
        }
        // ---------------------------------//
        
        // 能量值
        [JsonProperty]
        int energy;
        [JsonIgnore]
        public int Energy
        {
            get
            {
                return energy;
            }
            set
            {
                if(energy != value)
                {
                    energy = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次自动回复能量的时间
        [JsonProperty]
        long lastAddEnergyTime;
        [JsonIgnore]
        public long LastAddEnergyTime
        {
            get
            {
                return lastAddEnergyTime;
            }
            set
            {
                if(lastAddEnergyTime != value)
                {
                    lastAddEnergyTime = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 无限体力结束时间
        [JsonProperty]
        long unlimitEnergyEndUTCTimeInSeconds;
        [JsonIgnore]
        public long UnlimitEnergyEndUTCTimeInSeconds
        {
            get
            {
                return unlimitEnergyEndUTCTimeInSeconds;
            }
            set
            {
                if(unlimitEnergyEndUTCTimeInSeconds != value)
                {
                    unlimitEnergyEndUTCTimeInSeconds = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否初始化
        [JsonProperty]
        bool isInit;
        [JsonIgnore]
        public bool IsInit
        {
            get
            {
                return isInit;
            }
            set
            {
                if(isInit != value)
                {
                    isInit = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
         
        // 物品数据
        [JsonProperty]
        StorageDictionary<int,StorageItemData> items = new StorageDictionary<int,StorageItemData>();
        [JsonIgnore]
        public StorageDictionary<int,StorageItemData> Items
        {
            get
            {
                return items;
            }
        }
        // ---------------------------------//
         
        // 自动获取物品数据
        [JsonProperty]
        StorageDictionary<int, StorageAutoClaimItem> autoClaimItems = new StorageDictionary<int, StorageAutoClaimItem>();
        [JsonIgnore]
        public StorageDictionary<int, StorageAutoClaimItem> AutoClaimItems
        {
            get
            {
                return autoClaimItems;
            }
        }
        // ---------------------------------//
        
        // 1000000
        [JsonProperty]
        int sid;
        [JsonIgnore]
        public int Sid
        {
            get
            {
                return sid;
            }
            set
            {
                if(sid != value)
                {
                    sid = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}