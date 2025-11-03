/************************************************
 * Storage class : StorageGiftBagBuyBetter
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageGiftBagBuyBetter : StorageBase
    {
        
        // 礼包链ID
        [JsonProperty]
        StorageDictionary<string,int> giftBagBuyBetterIds = new StorageDictionary<string,int>();
        [JsonIgnore]
        public StorageDictionary<string,int> GiftBagBuyBetterIds
        {
            get
            {
                return giftBagBuyBetterIds;
            }
        }
        // ---------------------------------//
        
        // 当前领取的索引
        [JsonProperty]
        StorageDictionary<string,int> giftBagBuyBetteriIndexs = new StorageDictionary<string,int>();
        [JsonIgnore]
        public StorageDictionary<string,int> GiftBagBuyBetteriIndexs
        {
            get
            {
                return giftBagBuyBetteriIndexs;
            }
        }
        // ---------------------------------//
        
    }
}