/************************************************
 * Storage class : StorageTMShop
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTMShop : DragonU3DSDK.Storage.StorageBase
    {
        
        // 购买次数
        [JsonProperty]
        StorageDictionary<int,int> purchasedTimes = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> PurchasedTimes
        {
            get
            {
                return purchasedTimes;
            }
        }
        // ---------------------------------//
        
    }
}