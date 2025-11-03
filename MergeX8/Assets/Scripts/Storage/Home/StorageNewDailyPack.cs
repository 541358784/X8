/************************************************
 * Storage class : StorageNewDailyPack
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageNewDailyPack : StorageBase
    {
        
        // 弹出次数
        [JsonProperty]
        int popTimes;
        [JsonIgnore]
        public int PopTimes
        {
            get
            {
                return popTimes;
            }
            set
            {
                if(popTimes != value)
                {
                    popTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次弹出时间 
        [JsonProperty]
        long lastPopUpTime;
        [JsonIgnore]
        public long LastPopUpTime
        {
            get
            {
                return lastPopUpTime;
            }
            set
            {
                if(lastPopUpTime != value)
                {
                    lastPopUpTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否完成
        [JsonProperty]
        bool isFinish;
        [JsonIgnore]
        public bool IsFinish
        {
            get
            {
                return isFinish;
            }
            set
            {
                if(isFinish != value)
                {
                    isFinish = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 礼包开始时间
        [JsonProperty]
        long startTime;
        [JsonIgnore]
        public long StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                if(startTime != value)
                {
                    startTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已购项
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
        
        // 当前取奖励ID
        [JsonProperty]
        int currentTaskRecouseID;
        [JsonIgnore]
        public int CurrentTaskRecouseID
        {
            get
            {
                return currentTaskRecouseID;
            }
            set
            {
                if(currentTaskRecouseID != value)
                {
                    currentTaskRecouseID = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前配置组
        [JsonProperty]
        int curConfigGroup;
        [JsonIgnore]
        public int CurConfigGroup
        {
            get
            {
                return curConfigGroup;
            }
            set
            {
                if(curConfigGroup != value)
                {
                    curConfigGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
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
        
        // 上次折扣变化
        [JsonProperty]
        int lastDisCountChange;
        [JsonIgnore]
        public int LastDisCountChange
        {
            get
            {
                return lastDisCountChange;
            }
            set
            {
                if(lastDisCountChange != value)
                {
                    lastDisCountChange = value;
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
        
        // 当前折扣
        [JsonProperty]
        int currentDisCount;
        [JsonIgnore]
        public int CurrentDisCount
        {
            get
            {
                return currentDisCount;
            }
            set
            {
                if(currentDisCount != value)
                {
                    currentDisCount = value;
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
        
        // 礼包内容
        [JsonProperty]
        StorageList<StorageNewDailyPackItem> packInfo = new StorageList<StorageNewDailyPackItem>();
        [JsonIgnore]
        public StorageList<StorageNewDailyPackItem> PackInfo
        {
            get
            {
                return packInfo;
            }
        }
        // ---------------------------------//
        
        // 初始化标识
        [JsonProperty]
        bool isInit;
        [JsonIgnore]
        public bool IsInit
        {
            get
            {
                return isInit;
            }
            set
            {
                if(isInit != value)
                {
                    isInit = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否购买过
        [JsonProperty]
        bool buyFlag;
        [JsonIgnore]
        public bool BuyFlag
        {
            get
            {
                return buyFlag;
            }
            set
            {
                if(buyFlag != value)
                {
                    buyFlag = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}