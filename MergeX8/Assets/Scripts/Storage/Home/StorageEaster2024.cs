/************************************************
 * Storage class : StorageEaster2024
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageEaster2024 : StorageBase
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
        
        // 剩余的球
        [JsonProperty]
        int ballCount;
        [JsonIgnore]
        public int BallCount
        {
            get
            {
                return ballCount;
            }
            set
            {
                if(ballCount != value)
                {
                    ballCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 未使用的多球卡
        [JsonProperty]
        StorageList<int> extraBallList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> ExtraBallList
        {
            get
            {
                return extraBallList;
            }
        }
        // ---------------------------------//
        
        // 未使用的乘倍卡
        [JsonProperty]
        StorageList<int> multiBallList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> MultiBallList
        {
            get
            {
                return multiBallList;
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
        
        // 小游戏点数
        [JsonProperty]
        int luckyPointCount;
        [JsonIgnore]
        public int LuckyPointCount
        {
            get
            {
                return luckyPointCount;
            }
            set
            {
                if(luckyPointCount != value)
                {
                    luckyPointCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
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
        StorageEaster2024LeaderBoard leaderBoardStorage = new StorageEaster2024LeaderBoard();
        [JsonIgnore]
        public StorageEaster2024LeaderBoard LeaderBoardStorage
        {
            get
            {
                return leaderBoardStorage;
            }
        }
        // ---------------------------------//
        
        // 是否已购买所有内容
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
        
    }
}