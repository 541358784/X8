/************************************************
 * Storage class : StorageTotalRechargeReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTotalRechargeReward : StorageBase
    {
        
        // 配置ID
        [JsonProperty]
        int ids;
        [JsonIgnore]
        public int Ids
        {
            get
            {
                return ids;
            }
            set
            {
                if(ids != value)
                {
                    ids = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 奖励ID
        [JsonProperty]
        StorageList<int> rewardIds = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> RewardIds
        {
            get
            {
                return rewardIds;
            }
        }
        // ---------------------------------//
        
        // 奖励数量 
        [JsonProperty]
        StorageList<int> rewardCounts = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> RewardCounts
        {
            get
            {
                return rewardCounts;
            }
        }
        // ---------------------------------//
        
        // 分数
        [JsonProperty]
        int store;
        [JsonIgnore]
        public int Store
        {
            get
            {
                return store;
            }
            set
            {
                if(store != value)
                {
                    store = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}