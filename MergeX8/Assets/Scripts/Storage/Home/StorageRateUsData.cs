/************************************************
 * Storage class : StorageRateUsData
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageRateUsData : StorageBase
    {
        
        // 上次IOS弹出时间
        [JsonProperty]
        long lastPopiOSTime;
        [JsonIgnore]
        public long LastPopiOSTime
        {
            get
            {
                return lastPopiOSTime;
            }
            set
            {
                if(lastPopiOSTime != value)
                {
                    lastPopiOSTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 弹出次数
        [JsonProperty]
        int popTimesiOS;
        [JsonIgnore]
        public int PopTimesiOS
        {
            get
            {
                return popTimesiOS;
            }
            set
            {
                if(popTimesiOS != value)
                {
                    popTimesiOS = value;
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
        
    }
}