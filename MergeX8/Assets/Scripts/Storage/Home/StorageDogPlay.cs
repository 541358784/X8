/************************************************
 * Storage class : StorageDogPlay
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDogPlay : StorageBase
    {
        
        // 当前配置ID
        [JsonProperty]
        int curConfigId;
        [JsonIgnore]
        public int CurConfigId
        {
            get
            {
                return curConfigId;
            }
            set
            {
                if(curConfigId != value)
                {
                    curConfigId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 循环次数
        [JsonProperty]
        int rounds;
        [JsonIgnore]
        public int Rounds
        {
            get
            {
                return rounds;
            }
            set
            {
                if(rounds != value)
                {
                    rounds = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 不同类型任务可生成道具数量
        [JsonProperty]
        StorageDictionary<int,int> orderCanProductCount = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> OrderCanProductCount
        {
            get
            {
                return orderCanProductCount;
            }
        }
        // ---------------------------------//
        
        // 最大所需任务数量状态表
        [JsonProperty]
        StorageDictionary<int,int> maxTaskCountDic = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> MaxTaskCountDic
        {
            get
            {
                return maxTaskCountDic;
            }
        }
        // ---------------------------------//
        
        // 已收集任务数量状态表
        [JsonProperty]
        StorageDictionary<int,int> curTaskCountDic = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> CurTaskCountDic
        {
            get
            {
                return curTaskCountDic;
            }
        }
        // ---------------------------------//
        
        // 任务挂载道具状态(任务唯一ID，挂载数量)
        [JsonProperty]
        StorageDictionary<int,int> orderActiveState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> OrderActiveState
        {
            get
            {
                return orderActiveState;
            }
        }
        // ---------------------------------//
        
        // 最大所需数量
        [JsonProperty]
        int maxCount;
        [JsonIgnore]
        public int MaxCount
        {
            get
            {
                return maxCount;
            }
            set
            {
                if(maxCount != value)
                {
                    maxCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已收集数量
        [JsonProperty]
        int curCount;
        [JsonIgnore]
        public int CurCount
        {
            get
            {
                return curCount;
            }
            set
            {
                if(curCount != value)
                {
                    curCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 奖励
        [JsonProperty]
        StorageDictionary<int,int> rewards = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> Rewards
        {
            get
            {
                return rewards;
            }
        }
        // ---------------------------------//
        
    }
}