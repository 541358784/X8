/************************************************
 * Storage class : StorageASMRLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageASMRLevel : StorageBase
    {
        
        // ASMR关卡ID
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
        
        // 是否完成
        [JsonProperty]
        bool isFinished;
        [JsonIgnore]
        public bool IsFinished
        {
            get
            {
                return isFinished;
            }
            set
            {
                if(isFinished != value)
                {
                    isFinished = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最大完成度(0-100)
        [JsonProperty]
        int maxComplete;
        [JsonIgnore]
        public int MaxComplete
        {
            get
            {
                return maxComplete;
            }
            set
            {
                if(maxComplete != value)
                {
                    maxComplete = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已领取的奖励ID
        [JsonProperty]
        StorageList<int> getRewardId = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> GetRewardId
        {
            get
            {
                return getRewardId;
            }
        }
        // ---------------------------------//
        
        // 通关次数
        [JsonProperty]
        int passTime;
        [JsonIgnore]
        public int PassTime
        {
            get
            {
                return passTime;
            }
            set
            {
                if(passTime != value)
                {
                    passTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}