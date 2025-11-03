/************************************************
 * Storage class : StorageItemData
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageItemData : DragonU3DSDK.Storage.StorageBase
    {
        
        // 序列号
        [JsonProperty]
        int id;
        [JsonIgnore]
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if(id != value)
                {
                    id = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 数量
        [JsonProperty]
        int count;
        [JsonIgnore]
        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                if(count != value)
                {
                    count = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 到期时间
        [JsonProperty]
        ulong timestamp;
        [JsonIgnore]
        public ulong Timestamp
        {
            get
            {
                return timestamp;
            }
            set
            {
                if(timestamp != value)
                {
                    timestamp = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}