/************************************************
 * Storage class : StorageGiftBagLink
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageGiftBagLink : StorageBase
    {
        
        // 礼包链ID
        [JsonProperty]
        StorageDictionary<string,int> giftBagLinkIds = new StorageDictionary<string,int>();
        [JsonIgnore]
        public StorageDictionary<string,int> GiftBagLinkIds
        {
            get
            {
                return giftBagLinkIds;
            }
        }
        // ---------------------------------//
        
        // 当前领取的索引
        [JsonProperty]
        StorageDictionary<string,int> giftBagLinkiIndexs = new StorageDictionary<string,int>();
        [JsonIgnore]
        public StorageDictionary<string,int> GiftBagLinkiIndexs
        {
            get
            {
                return giftBagLinkiIndexs;
            }
        }
        // ---------------------------------//
        
    }
}