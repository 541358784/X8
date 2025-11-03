/************************************************
 * Storage class : StorageActivityCache
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageActivityCache : StorageBase
    {
        
        // 活动ID
        [JsonProperty]
        string activityId = "";
        [JsonIgnore]
        public string ActivityId
        {
            get
            {
                return activityId;
            }
            set
            {
                if(activityId != value)
                {
                    activityId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动开始时间
        [JsonProperty]
        long startTime;
        [JsonIgnore]
        public long StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                if(startTime != value)
                {
                    startTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动结束时间
        [JsonProperty]
        long endTime;
        [JsonIgnore]
        public long EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                if(endTime != value)
                {
                    endTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动配置
        [JsonProperty]
        string configJson = "";
        [JsonIgnore]
        public string ConfigJson
        {
            get
            {
                return configJson;
            }
            set
            {
                if(configJson != value)
                {
                    configJson = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 资源MD5
        [JsonProperty]
        StorageList<string> resMd5 = new StorageList<string>();
        [JsonIgnore]
        public StorageList<string> ResMd5
        {
            get
            {
                return resMd5;
            }
        }
        // ---------------------------------//
        
        // 资源路径
        [JsonProperty]
        StorageList<string> resPath = new StorageList<string>();
        [JsonIgnore]
        public StorageList<string> ResPath
        {
            get
            {
                return resPath;
            }
        }
        // ---------------------------------//
        
    }
}