/************************************************
 * Storage class : StorageShop
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageShop : StorageBase
    {
        
        // 最后一次充值金额
        [JsonProperty]
        double lastPurchase;
        [JsonIgnore]
        public double LastPurchase
        {
            get
            {
                return lastPurchase;
            }
            set
            {
                if(lastPurchase != value)
                {
                    lastPurchase = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已购买的SPECIAL礼包列表
        [JsonProperty]
        StorageList<int> purchasedSpecialList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> PurchasedSpecialList
        {
            get
            {
                return purchasedSpecialList;
            }
        }
        // ---------------------------------//
        
        // 是否购买了去广告商品
        [JsonProperty]
        bool gotNoAds;
        [JsonIgnore]
        public bool GotNoAds
        {
            get
            {
                return gotNoAds;
            }
            set
            {
                if(gotNoAds != value)
                {
                    gotNoAds = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 所有购买成功的商品记录
        [JsonProperty]
        StorageList<int> purchasedShopItemList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> PurchasedShopItemList
        {
            get
            {
                return purchasedShopItemList;
            }
        }
        // ---------------------------------//
        
    }
}