/************************************************
 * Storage class : StorageSealPack
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageSealPack : StorageBase
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
        
        // 礼包结束时间
        [JsonProperty]
        long finishTime;
        [JsonIgnore]
        public long FinishTime
        {
            get
            {
                return finishTime;
            }
            set
            {
                if(finishTime != value)
                {
                    finishTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}