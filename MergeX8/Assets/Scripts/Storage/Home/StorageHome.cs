/************************************************
 * Storage class : StorageHome
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageHome : StorageBase
    {
        
        // 当前的世界ID
        [JsonProperty]
        int currentWorldId;
        [JsonIgnore]
        public int CurrentWorldId
        {
            get
            {
                return currentWorldId;
            }
            set
            {
                if(currentWorldId != value)
                {
                    currentWorldId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
         
        // 玩家身上金币数
        [JsonProperty]
        StorageDictionary<string,StorageCurrency> currency = new StorageDictionary<string,StorageCurrency>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCurrency> Currency
        {
            get
            {
                return currency;
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        int gameMode;
        [JsonIgnore]
        public int GameMode
        {
            get
            {
                return gameMode;
            }
            set
            {
                if(gameMode != value)
                {
                    gameMode = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        int currentGameMode;
        [JsonIgnore]
        public int CurrentGameMode
        {
            get
            {
                return currentGameMode;
            }
            set
            {
                if(currentGameMode != value)
                {
                    currentGameMode = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 订单规则ID
        [JsonProperty]
        int orderRules;
        [JsonIgnore]
        public int OrderRules
        {
            get
            {
                return orderRules;
            }
            set
            {
                if(orderRules != value)
                {
                    orderRules = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // EXP
        [JsonProperty]
        int exp;
        [JsonIgnore]
        public int Exp
        {
            get
            {
                return exp;
            }
            set
            {
                if(exp != value)
                {
                    exp = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 背包解锁卷
        [JsonProperty]
        int bagToken;
        [JsonIgnore]
        public int BagToken
        {
            get
            {
                return bagToken;
            }
            set
            {
                if(bagToken != value)
                {
                    bagToken = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 等级
        [JsonProperty]
        int level;
        [JsonIgnore]
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if(level != value)
                {
                    level = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 回收记录
        [JsonProperty]
        StorageDictionary<string,bool> rcoveryRecord = new StorageDictionary<string,bool>();
        [JsonIgnore]
        public StorageDictionary<string,bool> RcoveryRecord
        {
            get
            {
                return rcoveryRecord;
            }
        }
        // ---------------------------------//
        
        // 装修币
        [JsonProperty]
        int totalDecoCoin;
        [JsonIgnore]
        public int TotalDecoCoin
        {
            get
            {
                return totalDecoCoin;
            }
            set
            {
                if(totalDecoCoin != value)
                {
                    totalDecoCoin = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 装修币
        [JsonProperty]
        int decoCoin;
        [JsonIgnore]
        public int DecoCoin
        {
            get
            {
                return decoCoin;
            }
            set
            {
                if(decoCoin != value)
                {
                    decoCoin = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 玩家体力
        [JsonProperty]
        int energy;
        [JsonIgnore]
        public int Energy
        {
            get
            {
                return energy;
            }
            set
            {
                if(energy != value)
                {
                    energy = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次增加体力时间 
        [JsonProperty]
        long lastAddEnergyTime;
        [JsonIgnore]
        public long LastAddEnergyTime
        {
            get
            {
                return lastAddEnergyTime;
            }
            set
            {
                if(lastAddEnergyTime != value)
                {
                    lastAddEnergyTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        long unlimitEnergyEndUTCTimeInSeconds;
        [JsonIgnore]
        public long UnlimitEnergyEndUTCTimeInSeconds
        {
            get
            {
                return unlimitEnergyEndUTCTimeInSeconds;
            }
            set
            {
                if(unlimitEnergyEndUTCTimeInSeconds != value)
                {
                    unlimitEnergyEndUTCTimeInSeconds = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最大充值金额
        [JsonProperty]
        float payMaxAmount;
        [JsonIgnore]
        public float PayMaxAmount
        {
            get
            {
                return payMaxAmount;
            }
            set
            {
                if(payMaxAmount != value)
                {
                    payMaxAmount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 用户的背包，存储所有资源&lt;ID,个数&gt;
        [JsonProperty]
        StorageDictionary<int,int> bag = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> Bag
        {
            get
            {
                return bag;
            }
        }
        // ---------------------------------//
        
        // 星级宝箱领取次数
        [JsonProperty]
        int starBoxCount;
        [JsonIgnore]
        public int StarBoxCount
        {
            get
            {
                return starBoxCount;
            }
            set
            {
                if(starBoxCount != value)
                {
                    starBoxCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否领取APPLE奖励
        [JsonProperty]
        bool buildReward_Apple;
        [JsonIgnore]
        public bool BuildReward_Apple
        {
            get
            {
                return buildReward_Apple;
            }
            set
            {
                if(buildReward_Apple != value)
                {
                    buildReward_Apple = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 第一次LOADING
        [JsonProperty]
        bool isFirstLoading;
        [JsonIgnore]
        public bool IsFirstLoading
        {
            get
            {
                return isFirstLoading;
            }
            set
            {
                if(isFirstLoading != value)
                {
                    isFirstLoading = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 第一次登陆
        [JsonProperty]
        bool isFirstLogin;
        [JsonIgnore]
        public bool IsFirstLogin
        {
            get
            {
                return isFirstLogin;
            }
            set
            {
                if(isFirstLogin != value)
                {
                    isFirstLogin = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否领取FB奖励
        [JsonProperty]
        bool buildReward_FB;
        [JsonIgnore]
        public bool BuildReward_FB
        {
            get
            {
                return buildReward_FB;
            }
            set
            {
                if(buildReward_FB != value)
                {
                    buildReward_FB = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 装修消耗金币 
        [JsonProperty]
        int decorateConsumeCoin;
        [JsonIgnore]
        public int DecorateConsumeCoin
        {
            get
            {
                return decorateConsumeCoin;
            }
            set
            {
                if(decorateConsumeCoin != value)
                {
                    decorateConsumeCoin = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // RATEUSFINISH
        [JsonProperty]
        bool rateUsFinish;
        [JsonIgnore]
        public bool RateUsFinish
        {
            get
            {
                return rateUsFinish;
            }
            set
            {
                if(rateUsFinish != value)
                {
                    rateUsFinish = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // RATEUS数据
        [JsonProperty]
        StorageRateUsData rateUsData = new StorageRateUsData();
        [JsonIgnore]
        public StorageRateUsData RateUsData
        {
            get
            {
                return rateUsData;
            }
        }
        // ---------------------------------//
        
        // LIKEUSFINISH
        [JsonProperty]
        bool likeUsFinish;
        [JsonIgnore]
        public bool LikeUsFinish
        {
            get
            {
                return likeUsFinish;
            }
            set
            {
                if(likeUsFinish != value)
                {
                    likeUsFinish = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 隐私弹窗是否显示
        [JsonProperty]
        bool privacy_agreed;
        [JsonIgnore]
        public bool Privacy_agreed
        {
            get
            {
                return privacy_agreed;
            }
            set
            {
                if(privacy_agreed != value)
                {
                    privacy_agreed = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否去广告
        [JsonProperty]
        bool removeAd;
        [JsonIgnore]
        public bool RemoveAd
        {
            get
            {
                return removeAd;
            }
            set
            {
                if(removeAd != value)
                {
                    removeAd = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前解锁的房间
        [JsonProperty]
        StorageDictionary<int,int> unLockRoom = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> UnLockRoom
        {
            get
            {
                return unLockRoom;
            }
        }
        // ---------------------------------//
        
        // 房间顺序分组ID
        [JsonProperty]
        int roomChapterId;
        [JsonIgnore]
        public int RoomChapterId
        {
            get
            {
                return roomChapterId;
            }
            set
            {
                if(roomChapterId != value)
                {
                    roomChapterId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前房间ID
        [JsonProperty]
        int curRoomId;
        [JsonIgnore]
        public int CurRoomId
        {
            get
            {
                return curRoomId;
            }
            set
            {
                if(curRoomId != value)
                {
                    curRoomId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前房间ID
        [JsonProperty]
        int curHappyGoRoomId;
        [JsonIgnore]
        public int CurHappyGoRoomId
        {
            get
            {
                return curHappyGoRoomId;
            }
            set
            {
                if(curHappyGoRoomId != value)
                {
                    curHappyGoRoomId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 分组和广告等数据
        [JsonProperty]
        StorageAdData adData = new StorageAdData();
        [JsonIgnore]
        public StorageAdData AdData
        {
            get
            {
                return adData;
            }
        }
        // ---------------------------------//
        
        // 消耗扩展数据
        [JsonProperty]
        StorageConsumeExtend consumeExtendData = new StorageConsumeExtend();
        [JsonIgnore]
        public StorageConsumeExtend ConsumeExtendData
        {
            get
            {
                return consumeExtendData;
            }
        }
        // ---------------------------------//
        
        // 应用第一次启动时间
        [JsonProperty]
        long localFirstRunTimeStamp;
        [JsonIgnore]
        public long LocalFirstRunTimeStamp
        {
            get
            {
                return localFirstRunTimeStamp;
            }
            set
            {
                if(localFirstRunTimeStamp != value)
                {
                    localFirstRunTimeStamp = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否关闭音乐
        [JsonProperty]
        bool musicClose;
        [JsonIgnore]
        public bool MusicClose
        {
            get
            {
                return musicClose;
            }
            set
            {
                if(musicClose != value)
                {
                    musicClose = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否关闭音效
        [JsonProperty]
        bool soundClose;
        [JsonIgnore]
        public bool SoundClose
        {
            get
            {
                return soundClose;
            }
            set
            {
                if(soundClose != value)
                {
                    soundClose = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 商店数据
        [JsonProperty]
        StorageShop shopData = new StorageShop();
        [JsonIgnore]
        public StorageShop ShopData
        {
            get
            {
                return shopData;
            }
        }
        // ---------------------------------//
        
        // 冷却时间数据
        [JsonProperty]
        StorageCoolTime coolTimeData = new StorageCoolTime();
        [JsonIgnore]
        public StorageCoolTime CoolTimeData
        {
            get
            {
                return coolTimeData;
            }
        }
        // ---------------------------------//
        
        // 统计数据
        [JsonProperty]
        StorageDictionary<string,int> statisticsIntData = new StorageDictionary<string,int>();
        [JsonIgnore]
        public StorageDictionary<string,int> StatisticsIntData
        {
            get
            {
                return statisticsIntData;
            }
        }
        // ---------------------------------//
        
        // 教程数据
        [JsonProperty]
        StorageGuide guide = new StorageGuide();
        [JsonIgnore]
        public StorageGuide Guide
        {
            get
            {
                return guide;
            }
        }
        // ---------------------------------//
        
        // 已发送的漏斗事件
        [JsonProperty]
        StorageDictionary<int,int> biEvents = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> BiEvents
        {
            get
            {
                return biEvents;
            }
        }
        // ---------------------------------//
         
        // 道具购买数据
        [JsonProperty]
        StorageDictionary<int, StorageStoreItem> storeItems = new StorageDictionary<int, StorageStoreItem>();
        [JsonIgnore]
        public StorageDictionary<int, StorageStoreItem> StoreItems
        {
            get
            {
                return storeItems;
            }
        }
        // ---------------------------------//
        
        // 商店每日刷新物品
        [JsonProperty]
        StorageList<StorageStoreItem> storeDailyItems = new StorageList<StorageStoreItem>();
        [JsonIgnore]
        public StorageList<StorageStoreItem> StoreDailyItems
        {
            get
            {
                return storeDailyItems;
            }
        }
        // ---------------------------------//
        
        // 商城每日物品
        [JsonProperty]
        StorageFlashSale flashSale = new StorageFlashSale();
        [JsonIgnore]
        public StorageFlashSale FlashSale
        {
            get
            {
                return flashSale;
            }
        }
        // ---------------------------------//
        
        // 商城小猪
        [JsonProperty]
        StoragePigSale pigSale = new StoragePigSale();
        [JsonIgnore]
        public StoragePigSale PigSale
        {
            get
            {
                return pigSale;
            }
        }
        // ---------------------------------//
         
        // 小红点
        [JsonProperty]
        StorageDictionary<int,StorageRedPoint> redPoint = new StorageDictionary<int,StorageRedPoint>();
        [JsonIgnore]
        public StorageDictionary<int,StorageRedPoint> RedPoint
        {
            get
            {
                return redPoint;
            }
        }
        // ---------------------------------//
        
        // 当前完成RVREWARDID
        [JsonProperty]
        int rvRewardId;
        [JsonIgnore]
        public int RvRewardId
        {
            get
            {
                return rvRewardId;
            }
            set
            {
                if(rvRewardId != value)
                {
                    rvRewardId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前进入游戏的关卡数
        [JsonProperty]
        int rvRewardLevel;
        [JsonIgnore]
        public int RvRewardLevel
        {
            get
            {
                return rvRewardLevel;
            }
            set
            {
                if(rvRewardLevel != value)
                {
                    rvRewardLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 电视广告弹出数
        [JsonProperty]
        int rvRewardPopCount;
        [JsonIgnore]
        public int RvRewardPopCount
        {
            get
            {
                return rvRewardPopCount;
            }
            set
            {
                if(rvRewardPopCount != value)
                {
                    rvRewardPopCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 主动点击RV
        [JsonProperty]
        bool manualTouchRv;
        [JsonIgnore]
        public bool ManualTouchRv
        {
            get
            {
                return manualTouchRv;
            }
            set
            {
                if(manualTouchRv != value)
                {
                    manualTouchRv = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否补偿过
        [JsonProperty]
        bool isCompensate;
        [JsonIgnore]
        public bool IsCompensate
        {
            get
            {
                return isCompensate;
            }
            set
            {
                if(isCompensate != value)
                {
                    isCompensate = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最后登录
        [JsonProperty]
        int lastLoginDay;
        [JsonIgnore]
        public int LastLoginDay
        {
            get
            {
                return lastLoginDay;
            }
            set
            {
                if(lastLoginDay != value)
                {
                    lastLoginDay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 连续登录
        [JsonProperty]
        int consecutiveLoginDays;
        [JsonIgnore]
        public int ConsecutiveLoginDays
        {
            get
            {
                return consecutiveLoginDays;
            }
            set
            {
                if(consecutiveLoginDays != value)
                {
                    consecutiveLoginDays = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最后领奖
        [JsonProperty]
        int lastClaimDailyBonusDay;
        [JsonIgnore]
        public int LastClaimDailyBonusDay
        {
            get
            {
                return lastClaimDailyBonusDay;
            }
            set
            {
                if(lastClaimDailyBonusDay != value)
                {
                    lastClaimDailyBonusDay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否关闭振动
        [JsonProperty]
        bool shakeClose;
        [JsonIgnore]
        public bool ShakeClose
        {
            get
            {
                return shakeClose;
            }
            set
            {
                if(shakeClose != value)
                {
                    shakeClose = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 破冰礼包
        [JsonProperty]
        StorageIceBreakPack iceBreakPackData = new StorageIceBreakPack();
        [JsonIgnore]
        public StorageIceBreakPack IceBreakPackData
        {
            get
            {
                return iceBreakPackData;
            }
        }
        // ---------------------------------//
        
        // 二阶破冰礼包
        [JsonProperty]
        StorageIceBreakPack secondIceBreakPackData = new StorageIceBreakPack();
        [JsonIgnore]
        public StorageIceBreakPack SecondIceBreakPackData
        {
            get
            {
                return secondIceBreakPackData;
            }
        }
        // ---------------------------------//
        
        // 三日破冰
        [JsonProperty]
        StorageIceBreakPack threeDayIceBreakPackData = new StorageIceBreakPack();
        [JsonIgnore]
        public StorageIceBreakPack ThreeDayIceBreakPackData
        {
            get
            {
                return threeDayIceBreakPackData;
            }
        }
        // ---------------------------------//
        
        // 七日破冰
        [JsonProperty]
        StorageIceBreakPack sevenDayIceBreakPackData = new StorageIceBreakPack();
        [JsonIgnore]
        public StorageIceBreakPack SevenDayIceBreakPackData
        {
            get
            {
                return sevenDayIceBreakPackData;
            }
        }
        // ---------------------------------//
        
        // 资源礼包
        [JsonProperty]
        StorageMaterialPack materialPackData = new StorageMaterialPack();
        [JsonIgnore]
        public StorageMaterialPack MaterialPackData
        {
            get
            {
                return materialPackData;
            }
        }
        // ---------------------------------//
        
        // 混合
        [JsonProperty]
        StorageBlendPack blendPackData = new StorageBlendPack();
        [JsonIgnore]
        public StorageBlendPack BlendPackData
        {
            get
            {
                return blendPackData;
            }
        }
        // ---------------------------------//
        
        // 闪购任务礼包
        [JsonProperty]
        StorageTaskAssistPack taskAssistPackData = new StorageTaskAssistPack();
        [JsonIgnore]
        public StorageTaskAssistPack TaskAssistPackData
        {
            get
            {
                return taskAssistPackData;
            }
        }
        // ---------------------------------//
        
        // 购买资源
        [JsonProperty]
        StorageBuyResource buyResource = new StorageBuyResource();
        [JsonIgnore]
        public StorageBuyResource BuyResource
        {
            get
            {
                return buyResource;
            }
        }
        // ---------------------------------//
        
        // 礼包链
        [JsonProperty]
        StorageGiftBagLink giftBagLink = new StorageGiftBagLink();
        [JsonIgnore]
        public StorageGiftBagLink GiftBagLink
        {
            get
            {
                return giftBagLink;
            }
        }
        // ---------------------------------//
        
        // 越买越划算礼包
        [JsonProperty]
        StorageGiftBagBuyBetter giftBagBuyBetter = new StorageGiftBagBuyBetter();
        [JsonIgnore]
        public StorageGiftBagBuyBetter GiftBagBuyBetter
        {
            get
            {
                return giftBagBuyBetter;
            }
        }
        // ---------------------------------//
        
        // 买一赠一礼包
        [JsonProperty]
        StorageGiftBagSendOne giftBagSendOne = new StorageGiftBagSendOne();
        [JsonIgnore]
        public StorageGiftBagSendOne GiftBagSendOne
        {
            get
            {
                return giftBagSendOne;
            }
        }
        // ---------------------------------//
        
        // TM礼包链
        [JsonProperty]
        StorageGiftBagLink tmGiftBagLink = new StorageGiftBagLink();
        [JsonIgnore]
        public StorageGiftBagLink TmGiftBagLink
        {
            get
            {
                return tmGiftBagLink;
            }
        }
        // ---------------------------------//
        
        // 是否返回游戏
        [JsonProperty]
        bool isBackGame;
        [JsonIgnore]
        public bool IsBackGame
        {
            get
            {
                return isBackGame;
            }
            set
            {
                if(isBackGame != value)
                {
                    isBackGame = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否完成所有任务
        [JsonProperty]
        bool isCompleteAllGuide;
        [JsonIgnore]
        public bool IsCompleteAllGuide
        {
            get
            {
                return isCompleteAllGuide;
            }
            set
            {
                if(isCompleteAllGuide != value)
                {
                    isCompleteAllGuide = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否返回HG游戏
        [JsonProperty]
        bool isBackHGGame;
        [JsonIgnore]
        public bool IsBackHGGame
        {
            get
            {
                return isBackHGGame;
            }
            set
            {
                if(isBackHGGame != value)
                {
                    isBackHGGame = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否完成所有HG任务
        [JsonProperty]
        bool isCompleteAllHGGuide;
        [JsonIgnore]
        public bool IsCompleteAllHGGuide
        {
            get
            {
                return isCompleteAllHGGuide;
            }
            set
            {
                if(isCompleteAllHGGuide != value)
                {
                    isCompleteAllHGGuide = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 发送BI 次数
        [JsonProperty]
        long sendBiIndex;
        [JsonIgnore]
        public long SendBiIndex
        {
            get
            {
                return sendBiIndex;
            }
            set
            {
                if(sendBiIndex != value)
                {
                    sendBiIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // MASTERCARD
        [JsonProperty]
        StorageMasterCard masterCard = new StorageMasterCard();
        [JsonIgnore]
        public StorageMasterCard MasterCard
        {
            get
            {
                return masterCard;
            }
        }
        // ---------------------------------//
        
        // 播放记录
        [JsonProperty]
        StorageLuckBallRecord luckBall = new StorageLuckBallRecord();
        [JsonIgnore]
        public StorageLuckBallRecord LuckBall
        {
            get
            {
                return luckBall;
            }
        }
        // ---------------------------------//
        
        // ABTEST配置
        [JsonProperty]
        StorageDictionary<string,string> abTestConfig = new StorageDictionary<string,string>();
        [JsonIgnore]
        public StorageDictionary<string,string> AbTestConfig
        {
            get
            {
                return abTestConfig;
            }
        }
        // ---------------------------------//
        
        // 小猪配置
        [JsonProperty]
        StoragePigBank pigBankData = new StoragePigBank();
        [JsonIgnore]
        public StoragePigBank PigBankData
        {
            get
            {
                return pigBankData;
            }
        }
        // ---------------------------------//
        
        // 次留奖励
        [JsonProperty]
        StorageSuperLoginGift superLoginGift = new StorageSuperLoginGift();
        [JsonIgnore]
        public StorageSuperLoginGift SuperLoginGift
        {
            get
            {
                return superLoginGift;
            }
        }
        // ---------------------------------//
        
        // 每日完成任务数量
        [JsonProperty]
        StorageList<StorageDailyCompleteTaskNum> dailyCompleteTaskNums = new StorageList<StorageDailyCompleteTaskNum>();
        [JsonIgnore]
        public StorageList<StorageDailyCompleteTaskNum> DailyCompleteTaskNums
        {
            get
            {
                return dailyCompleteTaskNums;
            }
        }
        // ---------------------------------//
        
        // 每日礼包数据
        [JsonProperty]
        StorageDailyPack dailyPackDaa = new StorageDailyPack();
        [JsonIgnore]
        public StorageDailyPack DailyPackDaa
        {
            get
            {
                return dailyPackDaa;
            }
        }
        // ---------------------------------//
        
        // 每日BUNDLE数据
        [JsonProperty]
        StorageDailyPack dailyBundleData = new StorageDailyPack();
        [JsonIgnore]
        public StorageDailyPack DailyBundleData
        {
            get
            {
                return dailyBundleData;
            }
        }
        // ---------------------------------//
        
        // 新每日BUNDLE数据
        [JsonProperty]
        StorageNewDailyPack newDailyBundleData = new StorageNewDailyPack();
        [JsonIgnore]
        public StorageNewDailyPack NewDailyBundleData
        {
            get
            {
                return newDailyBundleData;
            }
        }
        // ---------------------------------//
        
        // 大世界剧情对话
        [JsonProperty]
        StorageDecoDialog dialogData = new StorageDecoDialog();
        [JsonIgnore]
        public StorageDecoDialog DialogData
        {
            get
            {
                return dialogData;
            }
        }
        // ---------------------------------//
        
        // 剧情电影
        [JsonProperty]
        StorageStoryMovie storyMovieData = new StorageStoryMovie();
        [JsonIgnore]
        public StorageStoryMovie StoryMovieData
        {
            get
            {
                return storyMovieData;
            }
        }
        // ---------------------------------//
        
        // 装扮信息
        [JsonProperty]
        StorageAvatar avatarData = new StorageAvatar();
        [JsonIgnore]
        public StorageAvatar AvatarData
        {
            get
            {
                return avatarData;
            }
        }
        // ---------------------------------//
        
        // 签到
        [JsonProperty]
        StorageDailyBonus dailyBonus = new StorageDailyBonus();
        [JsonIgnore]
        public StorageDailyBonus DailyBonus
        {
            get
            {
                return dailyBonus;
            }
        }
        // ---------------------------------//
        
        // 每日排行活动
        [JsonProperty]
        StorageDailyRankGroup dailyRankGroup = new StorageDailyRankGroup();
        [JsonIgnore]
        public StorageDailyRankGroup DailyRankGroup
        {
            get
            {
                return dailyRankGroup;
            }
        }
        // ---------------------------------//
         
        // 小狗
        [JsonProperty]
        StorageDictionary<string,StorageDogHope> dogHopes = new StorageDictionary<string,StorageDogHope>();
        [JsonIgnore]
        public StorageDictionary<string,StorageDogHope> DogHopes
        {
            get
            {
                return dogHopes;
            }
        }
        // ---------------------------------//
         
        // 金币收集
        [JsonProperty]
        StorageDictionary<string,StorageCoinCompetition> coinCompetition = new StorageDictionary<string,StorageCoinCompetition>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCoinCompetition> CoinCompetition
        {
            get
            {
                return coinCompetition;
            }
        }
        // ---------------------------------//
        
        // 跳格子
        [JsonProperty]
        StorageCoinCompetition jumpGrid = new StorageCoinCompetition();
        [JsonIgnore]
        public StorageCoinCompetition JumpGrid
        {
            get
            {
                return jumpGrid;
            }
        }
        // ---------------------------------//
         
        // 猴子爬树
        [JsonProperty]
        StorageDictionary<string,StorageClimbTree> climbTree = new StorageDictionary<string,StorageClimbTree>();
        [JsonIgnore]
        public StorageDictionary<string,StorageClimbTree> ClimbTree
        {
            get
            {
                return climbTree;
            }
        }
        // ---------------------------------//
         
        // 金币狂潮
        [JsonProperty]
        StorageDictionary<string,StorageCoinRush> coinRush = new StorageDictionary<string,StorageCoinRush>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCoinRush> CoinRush
        {
            get
            {
                return coinRush;
            }
        }
        // ---------------------------------//
        
        // 任务中心
        [JsonProperty]
        StorageCustomTask customTask = new StorageCustomTask();
        [JsonIgnore]
        public StorageCustomTask CustomTask
        {
            get
            {
                return customTask;
            }
        }
        // ---------------------------------//
        
        // 任务统计
        [JsonProperty]
        StorageTaskStatistics taskStatistics = new StorageTaskStatistics();
        [JsonIgnore]
        public StorageTaskStatistics TaskStatistics
        {
            get
            {
                return taskStatistics;
            }
        }
        // ---------------------------------//
         
        // 充值返利
        [JsonProperty]
        StorageDictionary<string,StoragePayRebate> payRebate = new StorageDictionary<string,StoragePayRebate>();
        [JsonIgnore]
        public StorageDictionary<string,StoragePayRebate> PayRebate
        {
            get
            {
                return payRebate;
            }
        }
        // ---------------------------------//
         
        // 仓库清理
        [JsonProperty]
        StorageDictionary<string,StorageGarageCleanup> garageCleanup = new StorageDictionary<string,StorageGarageCleanup>();
        [JsonIgnore]
        public StorageDictionary<string,StorageGarageCleanup> GarageCleanup
        {
            get
            {
                return garageCleanup;
            }
        }
        // ---------------------------------//
        
        // 充值返利(本地)
        [JsonProperty]
        StoragePayRebate payRebateLocal = new StoragePayRebate();
        [JsonIgnore]
        public StoragePayRebate PayRebateLocal
        {
            get
            {
                return payRebateLocal;
            }
        }
        // ---------------------------------//
        
        // 海豹礼包
        [JsonProperty]
        StorageSealPack sealPack = new StorageSealPack();
        [JsonIgnore]
        public StorageSealPack SealPack
        {
            get
            {
                return sealPack;
            }
        }
        // ---------------------------------//
        
        // 海豚礼包 
        [JsonProperty]
        StorageDolphinPack dolphinPack = new StorageDolphinPack();
        [JsonIgnore]
        public StorageDolphinPack DolphinPack
        {
            get
            {
                return dolphinPack;
            }
        }
        // ---------------------------------//
         
        // 复活节活动
        [JsonProperty]
        StorageDictionary<string,StorageEaster> easter = new StorageDictionary<string,StorageEaster>();
        [JsonIgnore]
        public StorageDictionary<string,StorageEaster> Easter
        {
            get
            {
                return easter;
            }
        }
        // ---------------------------------//
         
        // 复活节礼包
        [JsonProperty]
        StorageDictionary<string,StorageEasterPack> easterPack = new StorageDictionary<string,StorageEasterPack>();
        [JsonIgnore]
        public StorageDictionary<string,StorageEasterPack> EasterPack
        {
            get
            {
                return easterPack;
            }
        }
        // ---------------------------------//
        
        // MAKEOVER
        [JsonProperty]
        StorageMakeOver makeOver = new StorageMakeOver();
        [JsonIgnore]
        public StorageMakeOver MakeOver
        {
            get
            {
                return makeOver;
            }
        }
        // ---------------------------------//
         
        // 复活节礼包
        [JsonProperty]
        StorageDictionary<string,StorageEasterStorePack> easterStorePack = new StorageDictionary<string,StorageEasterStorePack>();
        [JsonIgnore]
        public StorageDictionary<string,StorageEasterStorePack> EasterStorePack
        {
            get
            {
                return easterStorePack;
            }
        }
        // ---------------------------------//
         
        // 夏日挑战
        [JsonProperty]
        StorageDictionary<string,StorageBattlePass> battlePass = new StorageDictionary<string,StorageBattlePass>();
        [JsonIgnore]
        public StorageDictionary<string,StorageBattlePass> BattlePass
        {
            get
            {
                return battlePass;
            }
        }
        // ---------------------------------//
         
        // 夏日挑战2
        [JsonProperty]
        StorageDictionary<string,StorageBattlePass> battlePass2 = new StorageDictionary<string,StorageBattlePass>();
        [JsonIgnore]
        public StorageDictionary<string,StorageBattlePass> BattlePass2
        {
            get
            {
                return battlePass2;
            }
        }
        // ---------------------------------//
        
        // 能量狂潮
        [JsonProperty]
        StorageEnergyTorrent energyTorrent = new StorageEnergyTorrent();
        [JsonIgnore]
        public StorageEnergyTorrent EnergyTorrent
        {
            get
            {
                return energyTorrent;
            }
        }
        // ---------------------------------//
         
        // 主题装修
        [JsonProperty]
        StorageDictionary<string,StorageMermaid> mermaid = new StorageDictionary<string,StorageMermaid>();
        [JsonIgnore]
        public StorageDictionary<string,StorageMermaid> Mermaid
        {
            get
            {
                return mermaid;
            }
        }
        // ---------------------------------//
        
        // 体力礼包
        [JsonProperty]
        StorageEnergyPackage energyPackage = new StorageEnergyPackage();
        [JsonIgnore]
        public StorageEnergyPackage EnergyPackage
        {
            get
            {
                return energyPackage;
            }
        }
        // ---------------------------------//
        
        // 回收金币活动
        [JsonProperty]
        StorageRecoverCoin recoverCoin = new StorageRecoverCoin();
        [JsonIgnore]
        public StorageRecoverCoin RecoverCoin
        {
            get
            {
                return recoverCoin;
            }
        }
        // ---------------------------------//
        
        // 金币排行榜
        [JsonProperty]
        StorageCoinLeaderBoard coinLeaderBoard = new StorageCoinLeaderBoard();
        [JsonIgnore]
        public StorageCoinLeaderBoard CoinLeaderBoard
        {
            get
            {
                return coinLeaderBoard;
            }
        }
        // ---------------------------------//
        
        // 气球
        [JsonProperty]
        StorageBalloon balloon = new StorageBalloon();
        [JsonIgnore]
        public StorageBalloon Balloon
        {
            get
            {
                return balloon;
            }
        }
        // ---------------------------------//
         
        // 夏日西瓜
        [JsonProperty]
        StorageDictionary<string,StorageSummerWatermelon> summerWatermelon = new StorageDictionary<string,StorageSummerWatermelon>();
        [JsonIgnore]
        public StorageDictionary<string,StorageSummerWatermelon> SummerWatermelon
        {
            get
            {
                return summerWatermelon;
            }
        }
        // ---------------------------------//
         
        // 蝴蝶工坊
        [JsonProperty]
        StorageDictionary<string,StorageButterflyWorkShop> butterflyWorkShop = new StorageDictionary<string,StorageButterflyWorkShop>();
        [JsonIgnore]
        public StorageDictionary<string,StorageButterflyWorkShop> ButterflyWorkShop
        {
            get
            {
                return butterflyWorkShop;
            }
        }
        // ---------------------------------//
         
        // 夏日西瓜面包
        [JsonProperty]
        StorageDictionary<string,StorageSummerWatermelonBread> summerWatermelonBread = new StorageDictionary<string,StorageSummerWatermelonBread>();
        [JsonIgnore]
        public StorageDictionary<string,StorageSummerWatermelonBread> SummerWatermelonBread
        {
            get
            {
                return summerWatermelonBread;
            }
        }
        // ---------------------------------//
        
        // 挖沟小游戏
        [JsonProperty]
        StorageDigTrench digTrench = new StorageDigTrench();
        [JsonIgnore]
        public StorageDigTrench DigTrench
        {
            get
            {
                return digTrench;
            }
        }
        // ---------------------------------//
        
        // 默认小游戏类型
        [JsonProperty]
        int miniGameDefaultType;
        [JsonIgnore]
        public int MiniGameDefaultType
        {
            get
            {
                return miniGameDefaultType;
            }
            set
            {
                if(miniGameDefaultType != value)
                {
                    miniGameDefaultType = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 大鱼吃小鱼
        [JsonProperty]
        StorageFishEatFish fishEatFish = new StorageFishEatFish();
        [JsonIgnore]
        public StorageFishEatFish FishEatFish
        {
            get
            {
                return fishEatFish;
            }
        }
        // ---------------------------------//
        
        // 一笔画
        [JsonProperty]
        StorageOnePath onePath = new StorageOnePath();
        [JsonIgnore]
        public StorageOnePath OnePath
        {
            get
            {
                return onePath;
            }
        }
        // ---------------------------------//
        
        // 连线
        [JsonProperty]
        StorageConnectLine connectLine = new StorageConnectLine();
        [JsonIgnore]
        public StorageConnectLine ConnectLine
        {
            get
            {
                return connectLine;
            }
        }
        // ---------------------------------//
        
        // 心理学
        [JsonProperty]
        StorageConnectLine psychology = new StorageConnectLine();
        [JsonIgnore]
        public StorageConnectLine Psychology
        {
            get
            {
                return psychology;
            }
        }
        // ---------------------------------//
         
        // 海上竞速
        [JsonProperty]
        StorageDictionary<string,StorageSeaRacing> seaRacing = new StorageDictionary<string,StorageSeaRacing>();
        [JsonIgnore]
        public StorageDictionary<string,StorageSeaRacing> SeaRacing
        {
            get
            {
                return seaRacing;
            }
        }
        // ---------------------------------//
        
        // 卡册收集
        [JsonProperty]
        StorageCardCollection cardCollection = new StorageCardCollection();
        [JsonIgnore]
        public StorageCardCollection CardCollection
        {
            get
            {
                return cardCollection;
            }
        }
        // ---------------------------------//
        
        // 最大的SERVERTIME
        [JsonProperty]
        ulong lastServerTime;
        [JsonIgnore]
        public ulong LastServerTime
        {
            get
            {
                return lastServerTime;
            }
            set
            {
                if(lastServerTime != value)
                {
                    lastServerTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 非法调整时间记录
        [JsonProperty]
        StorageList<string> illegalTimeRecordList = new StorageList<string>();
        [JsonIgnore]
        public StorageList<string> IllegalTimeRecordList
        {
            get
            {
                return illegalTimeRecordList;
            }
        }
        // ---------------------------------//
        
        // 三礼包
        [JsonProperty]
        StorageThreeGift threeGift = new StorageThreeGift();
        [JsonIgnore]
        public StorageThreeGift ThreeGift
        {
            get
            {
                return threeGift;
            }
        }
        // ---------------------------------//
        
        // 三礼包2
        [JsonProperty]
        StorageThreeGift multipleGift = new StorageThreeGift();
        [JsonIgnore]
        public StorageThreeGift MultipleGift
        {
            get
            {
                return multipleGift;
            }
        }
        // ---------------------------------//
        
        // 小游戏
        [JsonProperty]
        StorageMiniGame miniGame = new StorageMiniGame();
        [JsonIgnore]
        public StorageMiniGame MiniGame
        {
            get
            {
                return miniGame;
            }
        }
        // ---------------------------------//
         
        // 复活节2024
        [JsonProperty]
        StorageDictionary<string,StorageEaster2024> easter2024 = new StorageDictionary<string,StorageEaster2024>();
        [JsonIgnore]
        public StorageDictionary<string,StorageEaster2024> Easter2024
        {
            get
            {
                return easter2024;
            }
        }
        // ---------------------------------//
        
        // 限时任务
        [JsonProperty]
        StorageTimeOrder timeOrder = new StorageTimeOrder();
        [JsonIgnore]
        public StorageTimeOrder TimeOrder
        {
            get
            {
                return timeOrder;
            }
        }
        // ---------------------------------//
        
        // 限时任务链
        [JsonProperty]
        StorageLimitOrderLine limitOrderLine = new StorageLimitOrderLine();
        [JsonIgnore]
        public StorageLimitOrderLine LimitOrderLine
        {
            get
            {
                return limitOrderLine;
            }
        }
        // ---------------------------------//
        
        // 农场限时任务
        [JsonProperty]
        StorageFarmTimeOrder farmTimeOrder = new StorageFarmTimeOrder();
        [JsonIgnore]
        public StorageFarmTimeOrder FarmTimeOrder
        {
            get
            {
                return farmTimeOrder;
            }
        }
        // ---------------------------------//
        
        // 订单狂热
        [JsonProperty]
        StorageCrazeOrder crazeOrder = new StorageCrazeOrder();
        [JsonIgnore]
        public StorageCrazeOrder CrazeOrder
        {
            get
            {
                return crazeOrder;
            }
        }
        // ---------------------------------//
        
        // 套娃
        [JsonProperty]
        StorageMatreshkas matreshkas = new StorageMatreshkas();
        [JsonIgnore]
        public StorageMatreshkas Matreshkas
        {
            get
            {
                return matreshkas;
            }
        }
        // ---------------------------------//
         
        // 卡册活动
        [JsonProperty]
        StorageDictionary<string,StorageCardCollectionActivity> cardCollectionActivity = new StorageDictionary<string,StorageCardCollectionActivity>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCardCollectionActivity> CardCollectionActivity
        {
            get
            {
                return cardCollectionActivity;
            }
        }
        // ---------------------------------//
         
        // 蛇梯子
        [JsonProperty]
        StorageDictionary<string,StorageSnakeLadder> snakeLadder = new StorageDictionary<string,StorageSnakeLadder>();
        [JsonIgnore]
        public StorageDictionary<string,StorageSnakeLadder> SnakeLadder
        {
            get
            {
                return snakeLadder;
            }
        }
        // ---------------------------------//
        
        // 任务额外奖励券
        [JsonProperty]
        StorageExtraOrderRewardCoupon extraOrderRewardCoupon = new StorageExtraOrderRewardCoupon();
        [JsonIgnore]
        public StorageExtraOrderRewardCoupon ExtraOrderRewardCoupon
        {
            get
            {
                return extraOrderRewardCoupon;
            }
        }
        // ---------------------------------//
         
        // 翻倍活动
        [JsonProperty]
        StorageDictionary<string,StorageMultipleScore> multipleScore = new StorageDictionary<string,StorageMultipleScore>();
        [JsonIgnore]
        public StorageDictionary<string,StorageMultipleScore> MultipleScore
        {
            get
            {
                return multipleScore;
            }
        }
        // ---------------------------------//
        
        // 升级礼包
        [JsonProperty]
        StorageLevelUpPackage levelUpPackage = new StorageLevelUpPackage();
        [JsonIgnore]
        public StorageLevelUpPackage LevelUpPackage
        {
            get
            {
                return levelUpPackage;
            }
        }
        // ---------------------------------//
        
        // STIMULATE
        [JsonProperty]
        StorageStimulate stimulate = new StorageStimulate();
        [JsonIgnore]
        public StorageStimulate Stimulate
        {
            get
            {
                return stimulate;
            }
        }
        // ---------------------------------//
         
        // 主题装修
        [JsonProperty]
        StorageDictionary<string,StorageThemeDecoration> themeDecoration = new StorageDictionary<string,StorageThemeDecoration>();
        [JsonIgnore]
        public StorageDictionary<string,StorageThemeDecoration> ThemeDecoration
        {
            get
            {
                return themeDecoration;
            }
        }
        // ---------------------------------//
         
        // 商店额外奖励
        [JsonProperty]
        StorageDictionary<string,StorageShopExtraReward> shopExtraReward = new StorageDictionary<string,StorageShopExtraReward>();
        [JsonIgnore]
        public StorageDictionary<string,StorageShopExtraReward> ShopExtraReward
        {
            get
            {
                return shopExtraReward;
            }
        }
        // ---------------------------------//
         
        // 老虎机
        [JsonProperty]
        StorageDictionary<string,StorageSlotMachine> slotMachine = new StorageDictionary<string,StorageSlotMachine>();
        [JsonIgnore]
        public StorageDictionary<string,StorageSlotMachine> SlotMachine
        {
            get
            {
                return slotMachine;
            }
        }
        // ---------------------------------//
         
        // 
        [JsonProperty]
        StorageDictionary<string,StorageWeeklyCard> weeklyCard = new StorageDictionary<string,StorageWeeklyCard>();
        [JsonIgnore]
        public StorageDictionary<string,StorageWeeklyCard> WeeklyCard
        {
            get
            {
                return weeklyCard;
            }
        }
        // ---------------------------------//
        
        // 消费档次
        [JsonProperty]
        StoragePayLevel payLevel = new StoragePayLevel();
        [JsonIgnore]
        public StoragePayLevel PayLevel
        {
            get
            {
                return payLevel;
            }
        }
        // ---------------------------------//
         
        // 大富翁
        [JsonProperty]
        StorageDictionary<string,StorageMonopoly> monopoly = new StorageDictionary<string,StorageMonopoly>();
        [JsonIgnore]
        public StorageDictionary<string,StorageMonopoly> Monopoly
        {
            get
            {
                return monopoly;
            }
        }
        // ---------------------------------//
         
        // 大富翁排行榜
        [JsonProperty]
        StorageDictionary<string,StorageCommonLeaderBoard> monopolyLeaderBoard = new StorageDictionary<string,StorageCommonLeaderBoard>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCommonLeaderBoard> MonopolyLeaderBoard
        {
            get
            {
                return monopolyLeaderBoard;
            }
        }
        // ---------------------------------//
         
        // 资源排行榜(KEY为排行榜关键字)
        [JsonProperty]
        StorageDictionary<string,StorageCommonResourceLeaderBoard> commonResourceLeaderBoard = new StorageDictionary<string,StorageCommonResourceLeaderBoard>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCommonResourceLeaderBoard> CommonResourceLeaderBoard
        {
            get
            {
                return commonResourceLeaderBoard;
            }
        }
        // ---------------------------------//
         
        // 祖玛
        [JsonProperty]
        StorageDictionary<string,StorageZuma> zuma = new StorageDictionary<string,StorageZuma>();
        [JsonIgnore]
        public StorageDictionary<string,StorageZuma> Zuma
        {
            get
            {
                return zuma;
            }
        }
        // ---------------------------------//
         
        // 祖玛排行榜
        [JsonProperty]
        StorageDictionary<string,StorageCommonLeaderBoard> zumaLeaderBoard = new StorageDictionary<string,StorageCommonLeaderBoard>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCommonLeaderBoard> ZumaLeaderBoard
        {
            get
            {
                return zumaLeaderBoard;
            }
        }
        // ---------------------------------//
         
        // 养宠物
        [JsonProperty]
        StorageDictionary<string,StorageKeepPet> keepPet = new StorageDictionary<string,StorageKeepPet>();
        [JsonIgnore]
        public StorageDictionary<string,StorageKeepPet> KeepPet
        {
            get
            {
                return keepPet;
            }
        }
        // ---------------------------------//
        
        // 藏宝图
        [JsonProperty]
        StorageTreasureMap treasureMap = new StorageTreasureMap();
        [JsonIgnore]
        public StorageTreasureMap TreasureMap
        {
            get
            {
                return treasureMap;
            }
        }
        // ---------------------------------//
        
        // 挖宝
        [JsonProperty]
        StorageTreasureHunt treasureHunt = new StorageTreasureHunt();
        [JsonIgnore]
        public StorageTreasureHunt TreasureHunt
        {
            get
            {
                return treasureHunt;
            }
        }
        // ---------------------------------//
        
        // 幸运金蛋
        [JsonProperty]
        StorageTreasureHunt luckyGoldenEgg = new StorageTreasureHunt();
        [JsonIgnore]
        public StorageTreasureHunt LuckyGoldenEgg
        {
            get
            {
                return luckyGoldenEgg;
            }
        }
        // ---------------------------------//
         
        // 进步礼包
        [JsonProperty]
        StorageDictionary<string,StorageGiftBagProgress> giftBagProgress = new StorageDictionary<string,StorageGiftBagProgress>();
        [JsonIgnore]
        public StorageDictionary<string,StorageGiftBagProgress> GiftBagProgress
        {
            get
            {
                return giftBagProgress;
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        StorageOptionalGift optionalGift = new StorageOptionalGift();
        [JsonIgnore]
        public StorageOptionalGift OptionalGift
        {
            get
            {
                return optionalGift;
            }
        }
        // ---------------------------------//
        
        // 转盘
        [JsonProperty]
        StorageTurntable turntable = new StorageTurntable();
        [JsonIgnore]
        public StorageTurntable Turntable
        {
            get
            {
                return turntable;
            }
        }
        // ---------------------------------//
        
        // 钻石奖励
        [JsonProperty]
        StorageDiamondReward diamondReward = new StorageDiamondReward();
        [JsonIgnore]
        public StorageDiamondReward DiamondReward
        {
            get
            {
                return diamondReward;
            }
        }
        // ---------------------------------//
         
        // 俩礼包
        [JsonProperty]
        StorageDictionary<string,StorageGiftBagDouble> giftBagDouble = new StorageDictionary<string,StorageGiftBagDouble>();
        [JsonIgnore]
        public StorageDictionary<string,StorageGiftBagDouble> GiftBagDouble
        {
            get
            {
                return giftBagDouble;
            }
        }
        // ---------------------------------//
        
        // 花园宝藏
        [JsonProperty]
        StorageGardenTreasure gardenTreasure = new StorageGardenTreasure();
        [JsonIgnore]
        public StorageGardenTreasure GardenTreasure
        {
            get
            {
                return gardenTreasure;
            }
        }
        // ---------------------------------//
         
        // 花园宝藏排行榜
        [JsonProperty]
        StorageDictionary<string,StorageCommonLeaderBoard> gardenTreasureLeaderBoard = new StorageDictionary<string,StorageCommonLeaderBoard>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCommonLeaderBoard> GardenTreasureLeaderBoard
        {
            get
            {
                return gardenTreasureLeaderBoard;
            }
        }
        // ---------------------------------//
        
        // 跳过活动预热
        [JsonProperty]
        bool skipActivityPreheating;
        [JsonIgnore]
        public bool SkipActivityPreheating
        {
            get
            {
                return skipActivityPreheating;
            }
            set
            {
                if(skipActivityPreheating != value)
                {
                    skipActivityPreheating = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 救援任务活动
        [JsonProperty]
        StorageSaveTheWhales saveTheWhales = new StorageSaveTheWhales();
        [JsonIgnore]
        public StorageSaveTheWhales SaveTheWhales
        {
            get
            {
                return saveTheWhales;
            }
        }
        // ---------------------------------//
        
        // 调制活动
        [JsonProperty]
        StorageMixMaster mixMaster = new StorageMixMaster();
        [JsonIgnore]
        public StorageMixMaster MixMaster
        {
            get
            {
                return mixMaster;
            }
        }
        // ---------------------------------//
        
        // 换ICON 是否完成
        [JsonProperty]
        bool isChangeIconSuccess;
        [JsonIgnore]
        public bool IsChangeIconSuccess
        {
            get
            {
                return isChangeIconSuccess;
            }
            set
            {
                if(isChangeIconSuccess != value)
                {
                    isChangeIconSuccess = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 乌龟对对碰
        [JsonProperty]
        StorageTurtlePang turtlePang = new StorageTurtlePang();
        [JsonIgnore]
        public StorageTurtlePang TurtlePang
        {
            get
            {
                return turtlePang;
            }
        }
        // ---------------------------------//
        
        // 星空罗盘
        [JsonProperty]
        StorageStarrySkyCompass starrySkyCompass = new StorageStarrySkyCompass();
        [JsonIgnore]
        public StorageStarrySkyCompass StarrySkyCompass
        {
            get
            {
                return starrySkyCompass;
            }
        }
        // ---------------------------------//
         
        // 盲盒
        [JsonProperty]
        StorageDictionary<int,StorageBlindBox> blindBox = new StorageDictionary<int,StorageBlindBox>();
        [JsonIgnore]
        public StorageDictionary<int,StorageBlindBox> BlindBox
        {
            get
            {
                return blindBox;
            }
        }
        // ---------------------------------//
        
        // 盲盒全局数据
        [JsonProperty]
        StorageBlindBoxGlobal blindBoxGlobal = new StorageBlindBoxGlobal();
        [JsonIgnore]
        public StorageBlindBoxGlobal BlindBoxGlobal
        {
            get
            {
                return blindBoxGlobal;
            }
        }
        // ---------------------------------//
        
        // 钻石翻倍券
        [JsonProperty]
        StorageList<StorageBuyDiamondTicket> buyDiamondTicket = new StorageList<StorageBuyDiamondTicket>();
        [JsonIgnore]
        public StorageList<StorageBuyDiamondTicket> BuyDiamondTicket
        {
            get
            {
                return buyDiamondTicket;
            }
        }
        // ---------------------------------//
        
        // 每日礼包补丁
        [JsonProperty]
        StorageNewDailyPackageExtraReward newDailyPackageExtraReward = new StorageNewDailyPackageExtraReward();
        [JsonIgnore]
        public StorageNewDailyPackageExtraReward NewDailyPackageExtraReward
        {
            get
            {
                return newDailyPackageExtraReward;
            }
        }
        // ---------------------------------//
         
        // 累计充值活动
        [JsonProperty]
        StorageDictionary<string,StorageTotalRecharge> totalRecharges = new StorageDictionary<string,StorageTotalRecharge>();
        [JsonIgnore]
        public StorageDictionary<string,StorageTotalRecharge> TotalRecharges
        {
            get
            {
                return totalRecharges;
            }
        }
        // ---------------------------------//
         
        // 建筑奖励
        [JsonProperty]
        StorageDictionary<string,StorageDecoBuildReward> decoBuildRewards = new StorageDictionary<string,StorageDecoBuildReward>();
        [JsonIgnore]
        public StorageDictionary<string,StorageDecoBuildReward> DecoBuildRewards
        {
            get
            {
                return decoBuildRewards;
            }
        }
        // ---------------------------------//
        
        // 累计充值活动
        [JsonProperty]
        StorageTotalRechargeNew totalRecharges_New = new StorageTotalRechargeNew();
        [JsonIgnore]
        public StorageTotalRechargeNew TotalRecharges_New
        {
            get
            {
                return totalRecharges_New;
            }
        }
        // ---------------------------------//
         
        // 卡册返场活动
        [JsonProperty]
        StorageDictionary<string,StorageCardCollectionReopenActivity> cardCollectionReopenActivity = new StorageDictionary<string,StorageCardCollectionReopenActivity>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCardCollectionReopenActivity> CardCollectionReopenActivity
        {
            get
            {
                return cardCollectionReopenActivity;
            }
        }
        // ---------------------------------//
        
        // 狗火鸡活动
        [JsonProperty]
        StorageKeepPetTurkey keepPetTurkey = new StorageKeepPetTurkey();
        [JsonIgnore]
        public StorageKeepPetTurkey KeepPetTurkey
        {
            get
            {
                return keepPetTurkey;
            }
        }
        // ---------------------------------//
        
        // 卡皮巴拉活动
        [JsonProperty]
        StorageKapibala kapibala = new StorageKapibala();
        [JsonIgnore]
        public StorageKapibala Kapibala
        {
            get
            {
                return kapibala;
            }
        }
        // ---------------------------------//
        
        // 圣诞盲盒礼包
        [JsonProperty]
        StorageChristmasBlindBox christmasBlindBox = new StorageChristmasBlindBox();
        [JsonIgnore]
        public StorageChristmasBlindBox ChristmasBlindBox
        {
            get
            {
                return christmasBlindBox;
            }
        }
        // ---------------------------------//
        
        // 新破冰礼包
        [JsonProperty]
        StorageNewIceBreakGiftBag newIceBreakGiftBag = new StorageNewIceBreakGiftBag();
        [JsonIgnore]
        public StorageNewIceBreakGiftBag NewIceBreakGiftBag
        {
            get
            {
                return newIceBreakGiftBag;
            }
        }
        // ---------------------------------//
        
        // 卡皮钉子活动
        [JsonProperty]
        StorageKapiScrew kapiScrew = new StorageKapiScrew();
        [JsonIgnore]
        public StorageKapiScrew KapiScrew
        {
            get
            {
                return kapiScrew;
            }
        }
        // ---------------------------------//
        
        // VIP商店
        [JsonProperty]
        StorageVipStore vipStore = new StorageVipStore();
        [JsonIgnore]
        public StorageVipStore VipStore
        {
            get
            {
                return vipStore;
            }
        }
        // ---------------------------------//
        
        // 玩狗
        [JsonProperty]
        StorageDogPlay dogPlay = new StorageDogPlay();
        [JsonIgnore]
        public StorageDogPlay DogPlay
        {
            get
            {
                return dogPlay;
            }
        }
        // ---------------------------------//
        
        // 养鱼
        [JsonProperty]
        StorageFishCulture fishCulture = new StorageFishCulture();
        [JsonIgnore]
        public StorageFishCulture FishCulture
        {
            get
            {
                return fishCulture;
            }
        }
        // ---------------------------------//
         
        // 养鱼排行榜
        [JsonProperty]
        StorageDictionary<string,StorageCommonLeaderBoard> fishCultureLeaderBoard = new StorageDictionary<string,StorageCommonLeaderBoard>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCommonLeaderBoard> FishCultureLeaderBoard
        {
            get
            {
                return fishCultureLeaderBoard;
            }
        }
        // ---------------------------------//
        
        // 新挖沟
        [JsonProperty]
        StorageDitch ditch = new StorageDitch();
        [JsonIgnore]
        public StorageDitch Ditch
        {
            get
            {
                return ditch;
            }
        }
        // ---------------------------------//
        
        // 玩狗额外奖励
        [JsonProperty]
        StorageDogPlayExtraReward dogPlayExtraReward = new StorageDogPlayExtraReward();
        [JsonIgnore]
        public StorageDogPlayExtraReward DogPlayExtraReward
        {
            get
            {
                return dogPlayExtraReward;
            }
        }
        // ---------------------------------//
        
        // 卡皮TILE活动
        [JsonProperty]
        StorageKapiTile kapiTile = new StorageKapiTile();
        [JsonIgnore]
        public StorageKapiTile KapiTile
        {
            get
            {
                return kapiTile;
            }
        }
        // ---------------------------------//
        
        // 丛林探险
        [JsonProperty]
        StorageJungleAdventure jungleAdventure = new StorageJungleAdventure();
        [JsonIgnore]
        public StorageJungleAdventure JungleAdventure
        {
            get
            {
                return jungleAdventure;
            }
        }
        // ---------------------------------//
        
        // 相册收集
        [JsonProperty]
        StoragePhotoAlbum photoAlbum = new StoragePhotoAlbum();
        [JsonIgnore]
        public StoragePhotoAlbum PhotoAlbum
        {
            get
            {
                return photoAlbum;
            }
        }
        // ---------------------------------//
         
        // 丛林排行榜
        [JsonProperty]
        StorageDictionary<string,StorageCommonLeaderBoard> jungleAdventureLeaderBoard = new StorageDictionary<string,StorageCommonLeaderBoard>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCommonLeaderBoard> JungleAdventureLeaderBoard
        {
            get
            {
                return jungleAdventureLeaderBoard;
            }
        }
        // ---------------------------------//
        
        // 本地分组
        [JsonProperty]
        StorageAdConfigLocal adConfigLocal = new StorageAdConfigLocal();
        [JsonIgnore]
        public StorageAdConfigLocal AdConfigLocal
        {
            get
            {
                return adConfigLocal;
            }
        }
        // ---------------------------------//
        
        // 绑定邮箱
        [JsonProperty]
        StorageBuildEmail buildEmail = new StorageBuildEmail();
        [JsonIgnore]
        public StorageBuildEmail BuildEmail
        {
            get
            {
                return buildEmail;
            }
        }
        // ---------------------------------//
        
        // 飞镖
        [JsonProperty]
        StorageBiuBiu biuBiu = new StorageBiuBiu();
        [JsonIgnore]
        public StorageBiuBiu BiuBiu
        {
            get
            {
                return biuBiu;
            }
        }
        // ---------------------------------//
        
        // 鹦鹉
        [JsonProperty]
        StorageParrot parrot = new StorageParrot();
        [JsonIgnore]
        public StorageParrot Parrot
        {
            get
            {
                return parrot;
            }
        }
        // ---------------------------------//
         
        // 鹦鹉排行榜
        [JsonProperty]
        StorageDictionary<string,StorageCommonLeaderBoard> parrotLeaderBoard = new StorageDictionary<string,StorageCommonLeaderBoard>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCommonLeaderBoard> ParrotLeaderBoard
        {
            get
            {
                return parrotLeaderBoard;
            }
        }
        // ---------------------------------//
         
        // 活动缓存
        [JsonProperty]
        StorageDictionary<string,StorageActivityCache> activityCache = new StorageDictionary<string,StorageActivityCache>();
        [JsonIgnore]
        public StorageDictionary<string,StorageActivityCache> ActivityCache
        {
            get
            {
                return activityCache;
            }
        }
        // ---------------------------------//
        
        // 热气球竞速
        [JsonProperty]
        StorageBalloonRacing balloonRacingDic = new StorageBalloonRacing();
        [JsonIgnore]
        public StorageBalloonRacing BalloonRacingDic
        {
            get
            {
                return balloonRacingDic;
            }
        }
        // ---------------------------------//
        
        // 兔子竞速
        [JsonProperty]
        StorageBalloonRacing rabbitRacing = new StorageBalloonRacing();
        [JsonIgnore]
        public StorageBalloonRacing RabbitRacing
        {
            get
            {
                return rabbitRacing;
            }
        }
        // ---------------------------------//
        
        // 新新破冰礼包
        [JsonProperty]
        StorageNewNewIceBreakPack newNewIceBreakPack = new StorageNewNewIceBreakPack();
        [JsonIgnore]
        public StorageNewNewIceBreakPack NewNewIceBreakPack
        {
            get
            {
                return newNewIceBreakPack;
            }
        }
        // ---------------------------------//
        
        // 花田
        [JsonProperty]
        StorageFlowerField flowerField = new StorageFlowerField();
        [JsonIgnore]
        public StorageFlowerField FlowerField
        {
            get
            {
                return flowerField;
            }
        }
        // ---------------------------------//
         
        // 花田排行榜
        [JsonProperty]
        StorageDictionary<string,StorageCommonLeaderBoard> flowerFieldLeaderBoard = new StorageDictionary<string,StorageCommonLeaderBoard>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCommonLeaderBoard> FlowerFieldLeaderBoard
        {
            get
            {
                return flowerFieldLeaderBoard;
            }
        }
        // ---------------------------------//
        
        // 爬塔
        [JsonProperty]
        StorageClimbTower climbTower = new StorageClimbTower();
        [JsonIgnore]
        public StorageClimbTower ClimbTower
        {
            get
            {
                return climbTower;
            }
        }
        // ---------------------------------//
        
        // 1+2礼包
        [JsonProperty]
        StorageGiftBagSendTwo giftBagSendTwo = new StorageGiftBagSendTwo();
        [JsonIgnore]
        public StorageGiftBagSendTwo GiftBagSendTwo
        {
            get
            {
                return giftBagSendTwo;
            }
        }
        // ---------------------------------//
        
        // 1+3礼包
        [JsonProperty]
        StorageGiftBagSendThree giftBagSendThree = new StorageGiftBagSendThree();
        [JsonIgnore]
        public StorageGiftBagSendThree GiftBagSendThree
        {
            get
            {
                return giftBagSendThree;
            }
        }
        // ---------------------------------//
        
        // 1+4礼包
        [JsonProperty]
        StorageGiftBagSend4 giftBagSend4 = new StorageGiftBagSend4();
        [JsonIgnore]
        public StorageGiftBagSend4 GiftBagSend4
        {
            get
            {
                return giftBagSend4;
            }
        }
        // ---------------------------------//
        
        // 1+6礼包
        [JsonProperty]
        StorageGiftBagSend6 giftBagSend6 = new StorageGiftBagSend6();
        [JsonIgnore]
        public StorageGiftBagSend6 GiftBagSend6
        {
            get
            {
                return giftBagSend6;
            }
        }
        // ---------------------------------//
        
        // 工会
        [JsonProperty]
        StorageTeam team = new StorageTeam();
        [JsonIgnore]
        public StorageTeam Team
        {
            get
            {
                return team;
            }
        }
        // ---------------------------------//
        
        // 收集宝石
        [JsonProperty]
        StorageCollectStone collectStone = new StorageCollectStone();
        [JsonIgnore]
        public StorageCollectStone CollectStone
        {
            get
            {
                return collectStone;
            }
        }
        // ---------------------------------//
        
        // 枕头转盘
        [JsonProperty]
        StoragePillowWheel pillowWheel = new StoragePillowWheel();
        [JsonIgnore]
        public StoragePillowWheel PillowWheel
        {
            get
            {
                return pillowWheel;
            }
        }
        // ---------------------------------//
         
        // 枕头排行榜
        [JsonProperty]
        StorageDictionary<string,StorageCommonLeaderBoard> pillowWheelLeaderBoard = new StorageDictionary<string,StorageCommonLeaderBoard>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCommonLeaderBoard> PillowWheelLeaderBoard
        {
            get
            {
                return pillowWheelLeaderBoard;
            }
        }
        // ---------------------------------//
        
        // 火车订单
        [JsonProperty]
        StorageTrainOrder trainOrder = new StorageTrainOrder();
        [JsonIgnore]
        public StorageTrainOrder TrainOrder
        {
            get
            {
                return trainOrder;
            }
        }
        // ---------------------------------//
        
        // 去广告礼包
        [JsonProperty]
        StorageNoAdsGiftBag noAdsGiftBag = new StorageNoAdsGiftBag();
        [JsonIgnore]
        public StorageNoAdsGiftBag NoAdsGiftBag
        {
            get
            {
                return noAdsGiftBag;
            }
        }
        // ---------------------------------//
        
        // 无尽体力礼包
        [JsonProperty]
        StorageEndlessEnergyGiftBag endlessEnergyGiftBag = new StorageEndlessEnergyGiftBag();
        [JsonIgnore]
        public StorageEndlessEnergyGiftBag EndlessEnergyGiftBag
        {
            get
            {
                return endlessEnergyGiftBag;
            }
        }
        // ---------------------------------//
        
        // 抓鱼
        [JsonProperty]
        StorageCatchFish catchFish = new StorageCatchFish();
        [JsonIgnore]
        public StorageCatchFish CatchFish
        {
            get
            {
                return catchFish;
            }
        }
        // ---------------------------------//
        
    }
}