/************************************************
 * Storage class : StorageReviveGiftPack
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageReviveGiftPack : DragonU3DSDK.Storage.StorageBase
    {
        
        // 记录当前礼包等级
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
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 未购买次数
        [JsonProperty]
        int notBuyTimes;
        [JsonIgnore]
        public int NotBuyTimes
        {
            get
            {
                return notBuyTimes;
            }
            set
            {
                if(notBuyTimes != value)
                {
                    notBuyTimes = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次购买时间
        [JsonProperty]
        long lastBuyTime;
        [JsonIgnore]
        public long LastBuyTime
        {
            get
            {
                return lastBuyTime;
            }
            set
            {
                if(lastBuyTime != value)
                {
                    lastBuyTime = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次弹出时间
        [JsonProperty]
        long lastShowTime;
        [JsonIgnore]
        public long LastShowTime
        {
            get
            {
                return lastShowTime;
            }
            set
            {
                if(lastShowTime != value)
                {
                    lastShowTime = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}