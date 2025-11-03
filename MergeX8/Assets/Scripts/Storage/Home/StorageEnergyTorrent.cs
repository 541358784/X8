/************************************************
 * Storage class : StorageEnergyTorrent
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageEnergyTorrent : StorageBase
    {
        
        // 是否开启
        [JsonProperty]
        bool isOpen;
        [JsonIgnore]
        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                if(isOpen != value)
                {
                    isOpen = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否展示过
        [JsonProperty]
        bool isShowStart;
        [JsonIgnore]
        public bool IsShowStart
        {
            get
            {
                return isShowStart;
            }
            set
            {
                if(isShowStart != value)
                {
                    isShowStart = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否开启过
        [JsonProperty]
        bool isOpened;
        [JsonIgnore]
        public bool IsOpened
        {
            get
            {
                return isOpened;
            }
            set
            {
                if(isOpened != value)
                {
                    isOpened = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前倍率
        [JsonProperty]
        int multiply;
        [JsonIgnore]
        public int Multiply
        {
            get
            {
                return multiply;
            }
            set
            {
                if(multiply != value)
                {
                    multiply = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 8倍开始时间
        [JsonProperty]
        long maxStartTime;
        [JsonIgnore]
        public long MaxStartTime
        {
            get
            {
                return maxStartTime;
            }
            set
            {
                if(maxStartTime != value)
                {
                    maxStartTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}