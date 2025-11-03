/************************************************
 * Storage class : StorageHGFlashSale
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageHGFlashSale : StorageBase
    {
        
        // 刷新时间
        [JsonProperty]
        ulong refreshTime;
        [JsonIgnore]
        public ulong RefreshTime
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
        
        // 钻石刷新时间
        [JsonProperty]
        ulong diamongs_refresh_time;
        [JsonIgnore]
        public ulong Diamongs_refresh_time
        {
            get
            {
                return diamongs_refresh_time;
            }
            set
            {
                if(diamongs_refresh_time != value)
                {
                    diamongs_refresh_time = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 钻石刷新次数
        [JsonProperty]
        int refreshCount_diamonds;
        [JsonIgnore]
        public int RefreshCount_diamonds
        {
            get
            {
                return refreshCount_diamonds;
            }
            set
            {
                if(refreshCount_diamonds != value)
                {
                    refreshCount_diamonds = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 广告刷新时间
        [JsonProperty]
        ulong rv_refresh_time;
        [JsonIgnore]
        public ulong Rv_refresh_time
        {
            get
            {
                return rv_refresh_time;
            }
            set
            {
                if(rv_refresh_time != value)
                {
                    rv_refresh_time = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 广告刷新次数
        [JsonProperty]
        int refreshCount_Rv;
        [JsonIgnore]
        public int RefreshCount_Rv
        {
            get
            {
                return refreshCount_Rv;
            }
            set
            {
                if(refreshCount_Rv != value)
                {
                    refreshCount_Rv = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 物品
        [JsonProperty]
        StorageList<StorageStoreItem> items = new StorageList<StorageStoreItem>();
        [JsonIgnore]
        public StorageList<StorageStoreItem> Items
        {
            get
            {
                return items;
            }
        }
        // ---------------------------------//
        
        // 自动刷新次数
        [JsonProperty]
        int refreshCount;
        [JsonIgnore]
        public int RefreshCount
        {
            get
            {
                return refreshCount;
            }
            set
            {
                if(refreshCount != value)
                {
                    refreshCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 版本跟新数据之后强制刷新
        [JsonProperty]
        int forceRefresh;
        [JsonIgnore]
        public int ForceRefresh
        {
            get
            {
                return forceRefresh;
            }
            set
            {
                if(forceRefresh != value)
                {
                    forceRefresh = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 总刷新次数
        [JsonProperty]
        int totalRefreshCount;
        [JsonIgnore]
        public int TotalRefreshCount
        {
            get
            {
                return totalRefreshCount;
            }
            set
            {
                if(totalRefreshCount != value)
                {
                    totalRefreshCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前配置组
        [JsonProperty]
        int curConfigGroup;
        [JsonIgnore]
        public int CurConfigGroup
        {
            get
            {
                return curConfigGroup;
            }
            set
            {
                if(curConfigGroup != value)
                {
                    curConfigGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}