/************************************************
 * Storage class : StorageEasterStorePack
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageEasterStorePack : StorageBase
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
        
        // 上次弹出时间 
        [JsonProperty]
        long lastPopUpTime;
        [JsonIgnore]
        public long LastPopUpTime
        {
            get
            {
                return lastPopUpTime;
            }
            set
            {
                if(lastPopUpTime != value)
                {
                    lastPopUpTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否完成
        [JsonProperty]
        bool isFinish;
        [JsonIgnore]
        public bool IsFinish
        {
            get
            {
                return isFinish;
            }
            set
            {
                if(isFinish != value)
                {
                    isFinish = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已购项
        [JsonProperty]
        StorageList<int> gotShopItem = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> GotShopItem
        {
            get
            {
                return gotShopItem;
            }
        }
        // ---------------------------------//
        
    }
}