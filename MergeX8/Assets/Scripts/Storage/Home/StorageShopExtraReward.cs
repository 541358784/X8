/************************************************
 * Storage class : StorageShopExtraReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageShopExtraReward : StorageBase
    {
        
        // 购买次数
        [JsonProperty]
        StorageDictionary<int,int> buyState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> BuyState
        {
            get
            {
                return buyState;
            }
        }
        // ---------------------------------//
        
    }
}