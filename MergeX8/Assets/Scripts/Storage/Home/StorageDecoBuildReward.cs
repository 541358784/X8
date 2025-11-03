/************************************************
 * Storage class : StorageDecoBuildReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDecoBuildReward : StorageBase
    {
        
        // 活动ID
        [JsonProperty]
        string decoBuildId = "";
        [JsonIgnore]
        public string DecoBuildId
        {
            get
            {
                return decoBuildId;
            }
            set
            {
                if(decoBuildId != value)
                {
                    decoBuildId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前领取进度
        [JsonProperty]
        int index;
        [JsonIgnore]
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                if(index != value)
                {
                    index = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 领取时间
        [JsonProperty]
        long getTime;
        [JsonIgnore]
        public long GetTime
        {
            get
            {
                return getTime;
            }
            set
            {
                if(getTime != value)
                {
                    getTime = value;
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
        
    }
}