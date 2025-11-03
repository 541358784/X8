/************************************************
 * Storage class : StorageMultipleScoreSingleRule
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMultipleScoreSingleRule : StorageBase
    {
        
        // 乘倍系数
        [JsonProperty]
        float multiValue;
        [JsonIgnore]
        public float MultiValue
        {
            get
            {
                return multiValue;
            }
            set
            {
                if(multiValue != value)
                {
                    multiValue = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 开始时间
        [JsonProperty]
        ulong startTime;
        [JsonIgnore]
        public ulong StartTime
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
        
        // 结束时间
        [JsonProperty]
        ulong endTime;
        [JsonIgnore]
        public ulong EndTime
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
        
    }
}