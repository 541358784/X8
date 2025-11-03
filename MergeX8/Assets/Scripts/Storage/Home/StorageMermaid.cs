/************************************************
 * Storage class : StorageMermaid
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMermaid : StorageBase
    {
        
        // 累计分数
        [JsonProperty]
        int totalScore;
        [JsonIgnore]
        public int TotalScore
        {
            get
            {
                return totalScore;
            }
            set
            {
                if(totalScore != value)
                {
                    totalScore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 获取的分数
        [JsonProperty]
        int score;
        [JsonIgnore]
        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                if(score != value)
                {
                    score = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 兑换总数
        [JsonProperty]
        int exchangeCount;
        [JsonIgnore]
        public int ExchangeCount
        {
            get
            {
                return exchangeCount;
            }
            set
            {
                if(exchangeCount != value)
                {
                    exchangeCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否显示活动开始界面
        [JsonProperty]
        bool isShowStartView;
        [JsonIgnore]
        public bool IsShowStartView
        {
            get
            {
                return isShowStartView;
            }
            set
            {
                if(isShowStartView != value)
                {
                    isShowStartView = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 奖励领取情况
        [JsonProperty]
        StorageDictionary<int,bool> reward = new StorageDictionary<int,bool>();
        [JsonIgnore]
        public StorageDictionary<int,bool> Reward
        {
            get
            {
                return reward;
            }
        }
        // ---------------------------------//
        
        // 兑换奖励领取情况
        [JsonProperty]
        StorageDictionary<int,bool> exchangeReward = new StorageDictionary<int,bool>();
        [JsonIgnore]
        public StorageDictionary<int,bool> ExchangeReward
        {
            get
            {
                return exchangeReward;
            }
        }
        // ---------------------------------//
        
        // 结束时间
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
        
        // 是否购买
        [JsonProperty]
        bool isBuyEntendDay;
        [JsonIgnore]
        public bool IsBuyEntendDay
        {
            get
            {
                return isBuyEntendDay;
            }
            set
            {
                if(isBuyEntendDay != value)
                {
                    isBuyEntendDay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 购买时间
        [JsonProperty]
        long buyExtendTime;
        [JsonIgnore]
        public long BuyExtendTime
        {
            get
            {
                return buyExtendTime;
            }
            set
            {
                if(buyExtendTime != value)
                {
                    buyExtendTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否发送结束BI
        [JsonProperty]
        bool isSendOverBi;
        [JsonIgnore]
        public bool IsSendOverBi
        {
            get
            {
                return isSendOverBi;
            }
            set
            {
                if(isSendOverBi != value)
                {
                    isSendOverBi = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否为继承
        [JsonProperty]
        bool isExtend;
        [JsonIgnore]
        public bool IsExtend
        {
            get
            {
                return isExtend;
            }
            set
            {
                if(isExtend != value)
                {
                    isExtend = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}