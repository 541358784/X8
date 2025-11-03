/************************************************
 * Storage class : StorageRvShop
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageRvShop : StorageBase
    {
        
        // 上次RVSHOP的启动时间
        [JsonProperty]
        long rVShopOpenTime;
        [JsonIgnore]
        public long RVShopOpenTime
        {
            get
            {
                return rVShopOpenTime;
            }
            set
            {
                if(rVShopOpenTime != value)
                {
                    rVShopOpenTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前RVSHOP LIST领取过的记录
        [JsonProperty]
        StorageList<int> curRVShopGotRecord = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CurRVShopGotRecord
        {
            get
            {
                return curRVShopGotRecord;
            }
        }
        // ---------------------------------//
        
        // 上次RVSHOP主动弹出的时间
        [JsonProperty]
        long lastRVShopPopupTime;
        [JsonIgnore]
        public long LastRVShopPopupTime
        {
            get
            {
                return lastRVShopPopupTime;
            }
            set
            {
                if(lastRVShopPopupTime != value)
                {
                    lastRVShopPopupTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前RVSHOPLIST INDEX
        [JsonProperty]
        int curRVShopListIndex;
        [JsonIgnore]
        public int CurRVShopListIndex
        {
            get
            {
                return curRVShopListIndex;
            }
            set
            {
                if(curRVShopListIndex != value)
                {
                    curRVShopListIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前RVSHOPLIST INDEX
        [JsonProperty]
        int curRVShopListID;
        [JsonIgnore]
        public int CurRVShopListID
        {
            get
            {
                return curRVShopListID;
            }
            set
            {
                if(curRVShopListID != value)
                {
                    curRVShopListID = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已购项（充值）
        [JsonProperty]
        StorageList<int> gotShopItem = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> GotShopItem
        {
            get
            {
                return gotShopItem;
            }
        }
        // ---------------------------------//
        
        // 上次价格变化 
        [JsonProperty]
        int lastPriceChange;
        [JsonIgnore]
        public int LastPriceChange
        {
            get
            {
                return lastPriceChange;
            }
            set
            {
                if(lastPriceChange != value)
                {
                    lastPriceChange = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次支付时间
        [JsonProperty]
        long lastPayTime;
        [JsonIgnore]
        public long LastPayTime
        {
            get
            {
                return lastPayTime;
            }
            set
            {
                if(lastPayTime != value)
                {
                    lastPayTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前累计次数
        [JsonProperty]
        int payTime;
        [JsonIgnore]
        public int PayTime
        {
            get
            {
                return payTime;
            }
            set
            {
                if(payTime != value)
                {
                    payTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前价格
        [JsonProperty]
        int currentPrice;
        [JsonIgnore]
        public int CurrentPrice
        {
            get
            {
                return currentPrice;
            }
            set
            {
                if(currentPrice != value)
                {
                    currentPrice = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前类型
        [JsonProperty]
        int currentType;
        [JsonIgnore]
        public int CurrentType
        {
            get
            {
                return currentType;
            }
            set
            {
                if(currentType != value)
                {
                    currentType = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 连续多少个礼包未购买
        [JsonProperty]
        int unPayDays;
        [JsonIgnore]
        public int UnPayDays
        {
            get
            {
                return unPayDays;
            }
            set
            {
                if(unPayDays != value)
                {
                    unPayDays = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}