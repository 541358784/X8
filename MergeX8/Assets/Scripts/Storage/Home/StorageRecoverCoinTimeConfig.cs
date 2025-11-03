/************************************************
 * Storage class : StorageRecoverCoinTimeConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageRecoverCoinTimeConfig : StorageBase
    {
        
        // ID
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
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 周ID
        [JsonProperty]
        int week;
        [JsonIgnore]
        public int Week
        {
            get
            {
                return week;
            }
            set
            {
                if(week != value)
                {
                    week = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 开始时间
        [JsonProperty]
        string starTimeSec = "";
        [JsonIgnore]
        public string StarTimeSec
        {
            get
            {
                return starTimeSec;
            }
            set
            {
                if(starTimeSec != value)
                {
                    starTimeSec = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 结束时间
        [JsonProperty]
        string endTimeSec = "";
        [JsonIgnore]
        public string EndTimeSec
        {
            get
            {
                return endTimeSec;
            }
            set
            {
                if(endTimeSec != value)
                {
                    endTimeSec = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}