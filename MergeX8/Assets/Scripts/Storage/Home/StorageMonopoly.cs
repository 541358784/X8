/************************************************
 * Storage class : StorageMonopoly
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMonopoly : StorageBase
    {
        
        // 活动ID
        [JsonProperty]
        string activityId = "";
        [JsonIgnore]
        public string ActivityId
        {
            get
            {
                return activityId;
            }
            set
            {
                if(activityId != value)
                {
                    activityId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 剩余的骰子数
        [JsonProperty]
        int diceCount;
        [JsonIgnore]
        public int DiceCount
        {
            get
            {
                return diceCount;
            }
            set
            {
                if(diceCount != value)
                {
                    diceCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 分数倍数次数
        [JsonProperty]
        StorageList<int> scoreMultiList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> ScoreMultiList
        {
            get
            {
                return scoreMultiList;
            }
        }
        // ---------------------------------//
        
        // 步数翻倍次数
        [JsonProperty]
        StorageList<int> stepMultiList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> StepMultiList
        {
            get
            {
                return stepMultiList;
            }
        }
        // ---------------------------------//
        
        // 任意卡数量
        [JsonProperty]
        int wildCardCount;
        [JsonIgnore]
        public int WildCardCount
        {
            get
            {
                return wildCardCount;
            }
            set
            {
                if(wildCardCount != value)
                {
                    wildCardCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 开始时间戳(如果活动开启则进行更新，否则用本地存的)
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
        
        // 预热结束时间戳
        [JsonProperty]
        long preheatCompleteTime;
        [JsonIgnore]
        public long PreheatCompleteTime
        {
            get
            {
                return preheatCompleteTime;
            }
            set
            {
                if(preheatCompleteTime != value)
                {
                    preheatCompleteTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 结束时间戳(同STARTTIME)
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
        
        // 活动资源的文件名
        [JsonProperty]
        StorageList<string> activityResList = new StorageList<string>();
        [JsonIgnore]
        public StorageList<string> ActivityResList
        {
            get
            {
                return activityResList;
            }
        }
        // ---------------------------------//
        
        // 活动资源的下载路径
        [JsonProperty]
        StorageList<string> activityResMd5List = new StorageList<string>();
        [JsonIgnore]
        public StorageList<string> ActivityResMd5List
        {
            get
            {
                return activityResMd5List;
            }
        }
        // ---------------------------------//
        
        // 已购买的STOREITEMID
        [JsonProperty]
        StorageList<int> finishStoreItemList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> FinishStoreItemList
        {
            get
            {
                return finishStoreItemList;
            }
        }
        // ---------------------------------//
        
        // 活动分数
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
        
        // 是否弹出过开始弹窗
        [JsonProperty]
        bool isStart;
        [JsonIgnore]
        public bool IsStart
        {
            get
            {
                return isStart;
            }
            set
            {
                if(isStart != value)
                {
                    isStart = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 积累获得的总分数
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
        
        // 卡牌随机池子
        [JsonProperty]
        StorageList<int> cardRandomPool = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CardRandomPool
        {
            get
            {
                return cardRandomPool;
            }
        }
        // ---------------------------------//
        
        // 骰子随机池子
        [JsonProperty]
        StorageList<int> diceRandomPool = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> DiceRandomPool
        {
            get
            {
                return diceRandomPool;
            }
        }
        // ---------------------------------//
        
        // 已经表演过解锁的商店等级
        [JsonProperty]
        StorageList<int> unLockStoreLevel = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> UnLockStoreLevel
        {
            get
            {
                return unLockStoreLevel;
            }
        }
        // ---------------------------------//
        
        // 通关次数
        [JsonProperty]
        int completeTimes;
        [JsonIgnore]
        public int CompleteTimes
        {
            get
            {
                return completeTimes;
            }
            set
            {
                if(completeTimes != value)
                {
                    completeTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 宝箱完成次数
        [JsonProperty]
        int rewardBoxCompleteTimes;
        [JsonIgnore]
        public int RewardBoxCompleteTimes
        {
            get
            {
                return rewardBoxCompleteTimes;
            }
            set
            {
                if(rewardBoxCompleteTimes != value)
                {
                    rewardBoxCompleteTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 宝箱已收集的数量
        [JsonProperty]
        int rewardBoxCollectNum;
        [JsonIgnore]
        public int RewardBoxCollectNum
        {
            get
            {
                return rewardBoxCollectNum;
            }
            set
            {
                if(rewardBoxCollectNum != value)
                {
                    rewardBoxCollectNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前所在格子
        [JsonProperty]
        int curBlockIndex;
        [JsonIgnore]
        public int CurBlockIndex
        {
            get
            {
                return curBlockIndex;
            }
            set
            {
                if(curBlockIndex != value)
                {
                    curBlockIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 骰子购买状态
        [JsonProperty]
        StorageDictionary<int,int> diceBuyState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> DiceBuyState
        {
            get
            {
                return diceBuyState;
            }
        }
        // ---------------------------------//
        
        // 地块购买状态
        [JsonProperty]
        StorageDictionary<int,int> blockBuyState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> BlockBuyState
        {
            get
            {
                return blockBuyState;
            }
        }
        // ---------------------------------//
        
        // 当前地块是否可购买
        [JsonProperty]
        bool curBlockBuyState;
        [JsonIgnore]
        public bool CurBlockBuyState
        {
            get
            {
                return curBlockBuyState;
            }
            set
            {
                if(curBlockBuyState != value)
                {
                    curBlockBuyState = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 未完成的小游戏ID
        [JsonProperty]
        int unFinishedMiniGameConfigId;
        [JsonIgnore]
        public int UnFinishedMiniGameConfigId
        {
            get
            {
                return unFinishedMiniGameConfigId;
            }
            set
            {
                if(unFinishedMiniGameConfigId != value)
                {
                    unFinishedMiniGameConfigId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 地块购买次数
        [JsonProperty]
        int blockBuyTimes;
        [JsonIgnore]
        public int BlockBuyTimes
        {
            get
            {
                return blockBuyTimes;
            }
            set
            {
                if(blockBuyTimes != value)
                {
                    blockBuyTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 分层组
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
        
    }
}