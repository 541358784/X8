/************************************************
 * Storage class : StorageAutoClaimItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageAutoClaimItem : DragonU3DSDK.Storage.StorageBase
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
        
        // 上次获取时间
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
        
        // 是否开始CD(针对满体力被扣时的开始CD时间添加)
        [JsonProperty]
        bool isCD;
        [JsonIgnore]
        public bool IsCD
        {
            get
            {
                return isCD;
            }
            set
            {
                if(isCD != value)
                {
                    isCD = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}