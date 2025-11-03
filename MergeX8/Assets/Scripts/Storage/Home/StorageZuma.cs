/************************************************
 * Storage class : StorageZuma
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageZuma : StorageBase
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
        
        // 剩余的球数
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
        
        // 炸弹球数量
        [JsonProperty]
        int bombCount;
        [JsonIgnore]
        public int BombCount
        {
            get
            {
                return bombCount;
            }
            set
            {
                if(bombCount != value)
                {
                    bombCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 万能球数量
        [JsonProperty]
        int wildCount;
        [JsonIgnore]
        public int WildCount
        {
            get
            {
                return wildCount;
            }
            set
            {
                if(wildCount != value)
                {
                    wildCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前球颜色(配置颜色)
        [JsonProperty]
        int curBallColor;
        [JsonIgnore]
        public int CurBallColor
        {
            get
            {
                return curBallColor;
            }
            set
            {
                if(curBallColor != value)
                {
                    curBallColor = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 下个球颜色(配置颜色)
        [JsonProperty]
        int nextBallColor;
        [JsonIgnore]
        public int NextBallColor
        {
            get
            {
                return nextBallColor;
            }
            set
            {
                if(nextBallColor != value)
                {
                    nextBallColor = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 关卡ID
        [JsonProperty]
        int levelId;
        [JsonIgnore]
        public int LevelId
        {
            get
            {
                return levelId;
            }
            set
            {
                if(levelId != value)
                {
                    levelId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 颜色转换表(配置为KEY,对应颜色为VALUE)
        [JsonProperty]
        StorageDictionary<int,int> colorTransformTable = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> ColorTransformTable
        {
            get
            {
                return colorTransformTable;
            }
        }
        // ---------------------------------//
        
        // 关卡内积累分数
        [JsonProperty]
        int levelScore;
        [JsonIgnore]
        public int LevelScore
        {
            get
            {
                return levelScore;
            }
            set
            {
                if(levelScore != value)
                {
                    levelScore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}