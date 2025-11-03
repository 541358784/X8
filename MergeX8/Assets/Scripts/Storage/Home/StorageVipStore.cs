/************************************************
 * Storage class : StorageVipStore
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageVipStore : StorageBase
    {
        
        // VIP等级
        [JsonProperty]
        int vipLevel;
        [JsonIgnore]
        public int VipLevel
        {
            get
            {
                return vipLevel;
            }
            set
            {
                if(vipLevel != value)
                {
                    vipLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否适配过
        [JsonProperty]
        bool isAdapt;
        [JsonIgnore]
        public bool IsAdapt
        {
            get
            {
                return isAdapt;
            }
            set
            {
                if(isAdapt != value)
                {
                    isAdapt = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否是VIP商店
        [JsonProperty]
        bool isVipStore;
        [JsonIgnore]
        public bool IsVipStore
        {
            get
            {
                return isVipStore;
            }
            set
            {
                if(isVipStore != value)
                {
                    isVipStore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 充值金额
        [JsonProperty]
        int purchasePrice;
        [JsonIgnore]
        public int PurchasePrice
        {
            get
            {
                return purchasePrice;
            }
            set
            {
                if(purchasePrice != value)
                {
                    purchasePrice = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 解锁时间
        [JsonProperty]
        long unlockTime;
        [JsonIgnore]
        public long UnlockTime
        {
            get
            {
                return unlockTime;
            }
            set
            {
                if(unlockTime != value)
                {
                    unlockTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 刷新结束时间
        [JsonProperty]
        long refreshTime;
        [JsonIgnore]
        public long RefreshTime
        {
            get
            {
                return refreshTime;
            }
            set
            {
                if(refreshTime != value)
                {
                    refreshTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 降级结束时间
        [JsonProperty]
        long cycleTime;
        [JsonIgnore]
        public long CycleTime
        {
            get
            {
                return cycleTime;
            }
            set
            {
                if(cycleTime != value)
                {
                    cycleTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 购买记录
        [JsonProperty]
        StorageDictionary<string,int> buyRecord = new StorageDictionary<string,int>();
        [JsonIgnore]
        public StorageDictionary<string,int> BuyRecord
        {
            get
            {
                return buyRecord;
            }
        }
        // ---------------------------------//
        
        // 是否开启过VIP商店
        [JsonProperty]
        bool hasOpenedVipStore;
        [JsonIgnore]
        public bool HasOpenedVipStore
        {
            get
            {
                return hasOpenedVipStore;
            }
            set
            {
                if(hasOpenedVipStore != value)
                {
                    hasOpenedVipStore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}