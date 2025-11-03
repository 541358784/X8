/************************************************
 * Storage class : StorageDailyRankRewardGroup
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDailyRankRewardGroup : StorageBase
    {
        
        // 奖励
        [JsonProperty]
        StorageList<StorageDailyRankReward> rewards = new StorageList<StorageDailyRankReward>();
        [JsonIgnore]
        public StorageList<StorageDailyRankReward> Rewards
        {
            get
            {
                return rewards;
            }
        }
        // ---------------------------------//
        
    }
}