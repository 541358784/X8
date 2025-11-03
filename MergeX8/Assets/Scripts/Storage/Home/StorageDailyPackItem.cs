/************************************************
 * Storage class : StorageDailyPackItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDailyPackItem : StorageBase
    {
        
        // 物品ID
        [JsonProperty]
        StorageList<int> id = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> Id
        {
            get
            {
                return id;
            }
        }
        // ---------------------------------//
        
        // 数量
        [JsonProperty]
        StorageList<int> count = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> Count
        {
            get
            {
                return count;
            }
        }
        // ---------------------------------//
        
        // 对应SHOPID
        [JsonProperty]
        int shopid;
        [JsonIgnore]
        public int Shopid
        {
            get
            {
                return shopid;
            }
            set
            {
                if(shopid != value)
                {
                    shopid = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前折扣
        [JsonProperty]
        int discount;
        [JsonIgnore]
        public int Discount
        {
            get
            {
                return discount;
            }
            set
            {
                if(discount != value)
                {
                    discount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 礼包ID
        [JsonProperty]
        int packId;
        [JsonIgnore]
        public int PackId
        {
            get
            {
                return packId;
            }
            set
            {
                if(packId != value)
                {
                    packId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 礼包内容ID
        [JsonProperty]
        int packInfoId;
        [JsonIgnore]
        public int PackInfoId
        {
            get
            {
                return packInfoId;
            }
            set
            {
                if(packInfoId != value)
                {
                    packInfoId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}