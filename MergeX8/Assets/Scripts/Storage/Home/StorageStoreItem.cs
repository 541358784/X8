/************************************************
 * Storage class : StorageStoreItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageStoreItem : StorageBase
    {
        
        // 购买时间
        [JsonProperty]
        long buyTime;
        [JsonIgnore]
        public long BuyTime
        {
            get
            {
                return buyTime;
            }
            set
            {
                if(buyTime != value)
                {
                    buyTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 购买数量
        [JsonProperty]
        int buyCount;
        [JsonIgnore]
        public int BuyCount
        {
            get
            {
                return buyCount;
            }
            set
            {
                if(buyCount != value)
                {
                    buyCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 对应的ID
        [JsonProperty]
        int itemId;
        [JsonIgnore]
        public int ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                if(itemId != value)
                {
                    itemId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 价格
        [JsonProperty]
        int price;
        [JsonIgnore]
        public int Price
        {
            get
            {
                return price;
            }
            set
            {
                if(price != value)
                {
                    price = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 价格递增总和(不计本身价格)
        [JsonProperty]
        int priceAdd;
        [JsonIgnore]
        public int PriceAdd
        {
            get
            {
                return priceAdd;
            }
            set
            {
                if(priceAdd != value)
                {
                    priceAdd = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // RV观看次数
        [JsonProperty]
        int rvWatched;
        [JsonIgnore]
        public int RvWatched
        {
            get
            {
                return rvWatched;
            }
            set
            {
                if(rvWatched != value)
                {
                    rvWatched = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 点位
        [JsonProperty]
        int site;
        [JsonIgnore]
        public int Site
        {
            get
            {
                return site;
            }
            set
            {
                if(site != value)
                {
                    site = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 可购买次数
        [JsonProperty]
        int canBuyCount;
        [JsonIgnore]
        public int CanBuyCount
        {
            get
            {
                return canBuyCount;
            }
            set
            {
                if(canBuyCount != value)
                {
                    canBuyCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 1:钻石2：金币
        [JsonProperty]
        int priceType;
        [JsonIgnore]
        public int PriceType
        {
            get
            {
                return priceType;
            }
            set
            {
                if(priceType != value)
                {
                    priceType = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 价格递增系数
        [JsonProperty]
        StorageList<int> priceIdex = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> PriceIdex
        {
            get
            {
                return priceIdex;
            }
        }
        // ---------------------------------//
        
    }
}