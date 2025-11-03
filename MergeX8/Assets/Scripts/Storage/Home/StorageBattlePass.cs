/************************************************
 * Storage class : StorageBattlePass
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageBattlePass : StorageBase
    {
        
        // 当前获得的积分
        [JsonProperty]
        int activityScore;
        [JsonIgnore]
        public int ActivityScore
        {
            get
            {
                return activityScore;
            }
            set
            {
                if(activityScore != value)
                {
                    activityScore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 支付组
        [JsonProperty]
        int payLevelGroup;
        [JsonIgnore]
        public int PayLevelGroup
        {
            get
            {
                return payLevelGroup;
            }
            set
            {
                if(payLevelGroup != value)
                {
                    payLevelGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 购买类型
        [JsonProperty]
        int buyType;
        [JsonIgnore]
        public int BuyType
        {
            get
            {
                return buyType;
            }
            set
            {
                if(buyType != value)
                {
                    buyType = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否显示了开始界面
        [JsonProperty]
        bool isShowStart;
        [JsonIgnore]
        public bool IsShowStart
        {
            get
            {
                return isShowStart;
            }
            set
            {
                if(isShowStart != value)
                {
                    isShowStart = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否购买奖励
        [JsonProperty]
        bool isPurchase;
        [JsonIgnore]
        public bool IsPurchase
        {
            get
            {
                return isPurchase;
            }
            set
            {
                if(isPurchase != value)
                {
                    isPurchase = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否购买彩券
        [JsonProperty]
        bool isGoldPurchase;
        [JsonIgnore]
        public bool IsGoldPurchase
        {
            get
            {
                return isGoldPurchase;
            }
            set
            {
                if(isGoldPurchase != value)
                {
                    isGoldPurchase = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否购买奖励
        [JsonProperty]
        bool isUltimatePurchase;
        [JsonIgnore]
        public bool IsUltimatePurchase
        {
            get
            {
                return isUltimatePurchase;
            }
            set
            {
                if(isUltimatePurchase != value)
                {
                    isUltimatePurchase = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 手动结束
        [JsonProperty]
        bool manualEnd;
        [JsonIgnore]
        public bool ManualEnd
        {
            get
            {
                return manualEnd;
            }
            set
            {
                if(manualEnd != value)
                {
                    manualEnd = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动开始时间
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
        
        // 活动结束时间
        [JsonProperty]
        long endTime;
        [JsonIgnore]
        public long EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                if(endTime != value)
                {
                    endTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否领取了所有奖励
        [JsonProperty]
        bool isGetAllReward;
        [JsonIgnore]
        public bool IsGetAllReward
        {
            get
            {
                return isGetAllReward;
            }
            set
            {
                if(isGetAllReward != value)
                {
                    isGetAllReward = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // SHOPID
        [JsonProperty]
        int shopId;
        [JsonIgnore]
        public int ShopId
        {
            get
            {
                return shopId;
            }
            set
            {
                if(shopId != value)
                {
                    shopId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 积分系数
        [JsonProperty]
        int scoreMultiple;
        [JsonIgnore]
        public int ScoreMultiple
        {
            get
            {
                return scoreMultiple;
            }
            set
            {
                if(scoreMultiple != value)
                {
                    scoreMultiple = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 购买时间
        [JsonProperty]
        long purchaseTime;
        [JsonIgnore]
        public long PurchaseTime
        {
            get
            {
                return purchaseTime;
            }
            set
            {
                if(purchaseTime != value)
                {
                    purchaseTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动配置表奖励
        [JsonProperty]
        StorageList<StorageBattlePassRewardConfig> reward = new StorageList<StorageBattlePassRewardConfig>();
        [JsonIgnore]
        public StorageList<StorageBattlePassRewardConfig> Reward
        {
            get
            {
                return reward;
            }
        }
        // ---------------------------------//
        
        // N次合成没有酒杯
        [JsonProperty]
        int noProductCount;
        [JsonIgnore]
        public int NoProductCount
        {
            get
            {
                return noProductCount;
            }
            set
            {
                if(noProductCount != value)
                {
                    noProductCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 任务
        [JsonProperty]
        StorageBattlePassTask battlePassTask = new StorageBattlePassTask();
        [JsonIgnore]
        public StorageBattlePassTask BattlePassTask
        {
            get
            {
                return battlePassTask;
            }
        }
        // ---------------------------------//
        
        // 是否购买活动延期
        [JsonProperty]
        bool isBuyDays;
        [JsonIgnore]
        public bool IsBuyDays
        {
            get
            {
                return isBuyDays;
            }
            set
            {
                if(isBuyDays != value)
                {
                    isBuyDays = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 延期购买时间 
        [JsonProperty]
        long extraDaysBuyTime;
        [JsonIgnore]
        public long ExtraDaysBuyTime
        {
            get
            {
                return extraDaysBuyTime;
            }
            set
            {
                if(extraDaysBuyTime != value)
                {
                    extraDaysBuyTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 购买弹出次数
        [JsonProperty]
        int purchasePopCount;
        [JsonIgnore]
        public int PurchasePopCount
        {
            get
            {
                return purchasePopCount;
            }
            set
            {
                if(purchasePopCount != value)
                {
                    purchasePopCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否显示结束
        [JsonProperty]
        bool isShowEnd;
        [JsonIgnore]
        public bool IsShowEnd
        {
            get
            {
                return isShowEnd;
            }
            set
            {
                if(isShowEnd != value)
                {
                    isShowEnd = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 循环宝箱分数
        [JsonProperty]
        int loopRewardScore;
        [JsonIgnore]
        public int LoopRewardScore
        {
            get
            {
                return loopRewardScore;
            }
            set
            {
                if(loopRewardScore != value)
                {
                    loopRewardScore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 循环宝箱ID
        [JsonProperty]
        StorageList<int> loopRewardList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> LoopRewardList
        {
            get
            {
                return loopRewardList;
            }
        }
        // ---------------------------------//
        
        // 循环宝箱领取次数
        [JsonProperty]
        int loopRewardCollectTimes;
        [JsonIgnore]
        public int LoopRewardCollectTimes
        {
            get
            {
                return loopRewardCollectTimes;
            }
            set
            {
                if(loopRewardCollectTimes != value)
                {
                    loopRewardCollectTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 循环宝箱配置表
        [JsonProperty]
        string loopRewardConfigToJson = "";
        [JsonIgnore]
        public string LoopRewardConfigToJson
        {
            get
            {
                return loopRewardConfigToJson;
            }
            set
            {
                if(loopRewardConfigToJson != value)
                {
                    loopRewardConfigToJson = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}