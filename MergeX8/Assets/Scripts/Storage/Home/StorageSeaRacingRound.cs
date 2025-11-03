/************************************************
 * Storage class : StorageSeaRacingRound
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageSeaRacingRound : StorageBase
    {
        
        // 轮次配置ID
        [JsonProperty]
        int roundConfigId;
        [JsonIgnore]
        public int RoundConfigId
        {
            get
            {
                return roundConfigId;
            }
            set
            {
                if(roundConfigId != value)
                {
                    roundConfigId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 这一轮的开始时间
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
        
        // 玩家分数
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
        
        // 分数更新的时间戳
        [JsonProperty]
        ulong scoreUpdateTime;
        [JsonIgnore]
        public ulong ScoreUpdateTime
        {
            get
            {
                return scoreUpdateTime;
            }
            set
            {
                if(scoreUpdateTime != value)
                {
                    scoreUpdateTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 轮次最大分数
        [JsonProperty]
        int maxScore;
        [JsonIgnore]
        public int MaxScore
        {
            get
            {
                return maxScore;
            }
            set
            {
                if(maxScore != value)
                {
                    maxScore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 未领取的奖励
        [JsonProperty]
        StorageDictionary<int,int> unCollectRewards = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> UnCollectRewards
        {
            get
            {
                return unCollectRewards;
            }
        }
        // ---------------------------------//
        
        // 机器人列表
        [JsonProperty]
        StorageList<StorageSeaRacingRobot> robotList = new StorageList<StorageSeaRacingRobot>();
        [JsonIgnore]
        public StorageList<StorageSeaRacingRobot> RobotList
        {
            get
            {
                return robotList;
            }
        }
        // ---------------------------------//
        
        // 是否已结算
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
        
        // 是否已开始
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
        
    }
}