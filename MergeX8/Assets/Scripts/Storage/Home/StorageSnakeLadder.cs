/************************************************
 * Storage class : StorageSnakeLadder
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageSnakeLadder : StorageBase
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
        
        // 剩余的转盘数
        [JsonProperty]
        int turntableCount;
        [JsonIgnore]
        public int TurntableCount
        {
            get
            {
                return turntableCount;
            }
            set
            {
                if(turntableCount != value)
                {
                    turntableCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前分数倍数
        [JsonProperty]
        int scoreMultiValue;
        [JsonIgnore]
        public int ScoreMultiValue
        {
            get
            {
                return scoreMultiValue;
            }
            set
            {
                if(scoreMultiValue != value)
                {
                    scoreMultiValue = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前步数倍数
        [JsonProperty]
        int stepMultiValue;
        [JsonIgnore]
        public int StepMultiValue
        {
            get
            {
                return stepMultiValue;
            }
            set
            {
                if(stepMultiValue != value)
                {
                    stepMultiValue = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
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
        
        // 防御卡数量
        [JsonProperty]
        int defenseCardCount;
        [JsonIgnore]
        public int DefenseCardCount
        {
            get
            {
                return defenseCardCount;
            }
            set
            {
                if(defenseCardCount != value)
                {
                    defenseCardCount = value;
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
        
        // 通关后的排行榜存档
        [JsonProperty]
        StorageSnakeLadderLeaderBoard leaderBoardStorage = new StorageSnakeLadderLeaderBoard();
        [JsonIgnore]
        public StorageSnakeLadderLeaderBoard LeaderBoardStorage
        {
            get
            {
                return leaderBoardStorage;
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
        StorageList<int> turntableRandomPool = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> TurntableRandomPool
        {
            get
            {
                return turntableRandomPool;
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
        
        // 当前关卡ID
        [JsonProperty]
        int curLevelId;
        [JsonIgnore]
        public int CurLevelId
        {
            get
            {
                return curLevelId;
            }
            set
            {
                if(curLevelId != value)
                {
                    curLevelId = value;
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
        
        // 转盘购买状态
        [JsonProperty]
        StorageDictionary<int,int> turntableBuyState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> TurntableBuyState
        {
            get
            {
                return turntableBuyState;
            }
        }
        // ---------------------------------//
        
    }
}