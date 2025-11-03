/************************************************
 * Storage class : StoragePigSale
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StoragePigSale : StorageBase
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