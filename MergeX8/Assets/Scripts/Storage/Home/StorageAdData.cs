/************************************************
 * Storage class : StorageAdData
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageAdData : StorageBase
    {
        
        // 用户组
        [JsonProperty]
        int userGroup;
        [JsonIgnore]
        public int UserGroup
        {
            get
            {
                return userGroup;
            }
            set
            {
                if(userGroup != value)
                {
                    userGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 小用户组
        [JsonProperty]
        int subUserGroup;
        [JsonIgnore]
        public int SubUserGroup
        {
            get
            {
                return subUserGroup;
            }
            set
            {
                if(subUserGroup != value)
                {
                    subUserGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // ADJUST
        [JsonProperty]
        double costAmount;
        [JsonIgnore]
        public double CostAmount
        {
            get
            {
                return costAmount;
            }
            set
            {
                if(costAmount != value)
                {
                    costAmount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // ADJUST
        [JsonProperty]
        string costCurrency = "";
        [JsonIgnore]
        public string CostCurrency
        {
            get
            {
                return costCurrency;
            }
            set
            {
                if(costCurrency != value)
                {
                    costCurrency = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // ADJUST
        [JsonProperty]
        string costType = "";
        [JsonIgnore]
        public string CostType
        {
            get
            {
                return costType;
            }
            set
            {
                if(costType != value)
                {
                    costType = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 无广告时间
        [JsonProperty]
        long noADEndTime;
        [JsonIgnore]
        public long NoADEndTime
        {
            get
            {
                return noADEndTime;
            }
            set
            {
                if(noADEndTime != value)
                {
                    noADEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        //  用户登陆时间
        [JsonProperty]
        long loginTime;
        [JsonIgnore]
        public long LoginTime
        {
            get
            {
                return loginTime;
            }
            set
            {
                if(loginTime != value)
                {
                    loginTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
         
        // 广告播放记录
        [JsonProperty]
        StorageDictionary<string,StorageAdPlayRecord> adPlayRecords = new StorageDictionary<string,StorageAdPlayRecord>();
        [JsonIgnore]
        public StorageDictionary<string,StorageAdPlayRecord> AdPlayRecords
        {
            get
            {
                return adPlayRecords;
            }
        }
        // ---------------------------------//
         
        // 插屏播放记录
        [JsonProperty]
        StorageDictionary<string,StorageAdPlayRecord> interstitalPlayRecords = new StorageDictionary<string,StorageAdPlayRecord>();
        [JsonIgnore]
        public StorageDictionary<string,StorageAdPlayRecord> InterstitalPlayRecords
        {
            get
            {
                return interstitalPlayRecords;
            }
        }
        // ---------------------------------//
        
        // 插屏播放的当天时间戳
        [JsonProperty]
        long interstitalDayTimestamp;
        [JsonIgnore]
        public long InterstitalDayTimestamp
        {
            get
            {
                return interstitalDayTimestamp;
            }
            set
            {
                if(interstitalDayTimestamp != value)
                {
                    interstitalDayTimestamp = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当天插屏总播放成功次数
        [JsonProperty]
        int playedInterstitalToday;
        [JsonIgnore]
        public int PlayedInterstitalToday
        {
            get
            {
                return playedInterstitalToday;
            }
            set
            {
                if(playedInterstitalToday != value)
                {
                    playedInterstitalToday = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 神秘礼包展示次数
        [JsonProperty]
        int misteryGiftCount;
        [JsonIgnore]
        public int MisteryGiftCount
        {
            get
            {
                return misteryGiftCount;
            }
            set
            {
                if(misteryGiftCount != value)
                {
                    misteryGiftCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 视频广告 观看次数
        [JsonProperty]
        StorageDictionary<string,int> adVideoWatchCount = new StorageDictionary<string,int>();
        [JsonIgnore]
        public StorageDictionary<string,int> AdVideoWatchCount
        {
            get
            {
                return adVideoWatchCount;
            }
        }
        // ---------------------------------//
        
        // 视频广告 观看时间点(秒)
        [JsonProperty]
        StorageDictionary<string,long> adVideoWatchTime = new StorageDictionary<string,long>();
        [JsonIgnore]
        public StorageDictionary<string,long> AdVideoWatchTime
        {
            get
            {
                return adVideoWatchTime;
            }
        }
        // ---------------------------------//
        
        // 存档时间为
        [JsonProperty]
        long adResetTime;
        [JsonIgnore]
        public long AdResetTime
        {
            get
            {
                return adResetTime;
            }
            set
            {
                if(adResetTime != value)
                {
                    adResetTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 插屏广告 观看次数
        [JsonProperty]
        int adSettingWatchCount;
        [JsonIgnore]
        public int AdSettingWatchCount
        {
            get
            {
                return adSettingWatchCount;
            }
            set
            {
                if(adSettingWatchCount != value)
                {
                    adSettingWatchCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 记录每个MAP玩家看RV的次数
        [JsonProperty]
        StorageDictionary<int,int> rvTotalWatchCount = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> RvTotalWatchCount
        {
            get
            {
                return rvTotalWatchCount;
            }
        }
        // ---------------------------------//
        
        // 记录每个MAP玩家看插屏的次数
        [JsonProperty]
        StorageDictionary<int,int> interstitialTotalWatchCount = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> InterstitialTotalWatchCount
        {
            get
            {
                return interstitialTotalWatchCount;
            }
        }
        // ---------------------------------//
        
        // 广告奖励索引表
        [JsonProperty]
        StorageDictionary<string,int> adRewardIndex = new StorageDictionary<string,int>();
        [JsonIgnore]
        public StorageDictionary<string,int> AdRewardIndex
        {
            get
            {
                return adRewardIndex;
            }
        }
        // ---------------------------------//
        
        // HAPPY GO RVSHOP
        [JsonProperty]
        StorageRvShop happyGoRvShopData = new StorageRvShop();
        [JsonIgnore]
        public StorageRvShop HappyGoRvShopData
        {
            get
            {
                return happyGoRvShopData;
            }
        }
        // ---------------------------------//
        
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
        
        // 当天被动插屏播放成功次数
        [JsonProperty]
        int playPassiveInterTodayNum;
        [JsonIgnore]
        public int PlayPassiveInterTodayNum
        {
            get
            {
                return playPassiveInterTodayNum;
            }
            set
            {
                if(playPassiveInterTodayNum != value)
                {
                    playPassiveInterTodayNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当天主动动插屏播放成功次数
        [JsonProperty]
        int playActiveInterTodayNum;
        [JsonIgnore]
        public int PlayActiveInterTodayNum
        {
            get
            {
                return playActiveInterTodayNum;
            }
            set
            {
                if(playActiveInterTodayNum != value)
                {
                    playActiveInterTodayNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当天被动插屏播放成功时间
        [JsonProperty]
        long playPassiveInterTodayTime;
        [JsonIgnore]
        public long PlayPassiveInterTodayTime
        {
            get
            {
                return playPassiveInterTodayTime;
            }
            set
            {
                if(playPassiveInterTodayTime != value)
                {
                    playPassiveInterTodayTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当天主动动插屏播放成功时间
        [JsonProperty]
        long playActiveInterTodayTime;
        [JsonIgnore]
        public long PlayActiveInterTodayTime
        {
            get
            {
                return playActiveInterTodayTime;
            }
            set
            {
                if(playActiveInterTodayTime != value)
                {
                    playActiveInterTodayTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当天RV播放成功时间
        [JsonProperty]
        long playRvTodayTime;
        [JsonIgnore]
        public long PlayRvTodayTime
        {
            get
            {
                return playRvTodayTime;
            }
            set
            {
                if(playRvTodayTime != value)
                {
                    playRvTodayTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前完成任务次数(被动插屏用)
        [JsonProperty]
        int passiveInterCompleteOrderNum;
        [JsonIgnore]
        public int PassiveInterCompleteOrderNum
        {
            get
            {
                return passiveInterCompleteOrderNum;
            }
            set
            {
                if(passiveInterCompleteOrderNum != value)
                {
                    passiveInterCompleteOrderNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前播放RV次数(主动插屏用)
        [JsonProperty]
        int activeInterPlayRvNum;
        [JsonIgnore]
        public int ActiveInterPlayRvNum
        {
            get
            {
                return activeInterPlayRvNum;
            }
            set
            {
                if(activeInterPlayRvNum != value)
                {
                    activeInterPlayRvNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}