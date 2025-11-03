/************************************************
 * Storage class : StorageActivityWinStreak
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageActivityWinStreak : DragonU3DSDK.Storage.StorageBase
    {
        
        // 当前轮次
        [JsonProperty]
        int turn;
        [JsonIgnore]
        public int Turn
        {
            get
            {
                return turn;
            }
            set
            {
                if(turn != value)
                {
                    turn = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 挑战次数
        [JsonProperty]
        int challengeTimes;
        [JsonIgnore]
        public int ChallengeTimes
        {
            get
            {
                return challengeTimes;
            }
            set
            {
                if(challengeTimes != value)
                {
                    challengeTimes = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最后一次主动弹出时间
        [JsonProperty]
        string lastPopupTime = "";
        [JsonIgnore]
        public string LastPopupTime
        {
            get
            {
                return lastPopupTime;
            }
            set
            {
                if(lastPopupTime != value)
                {
                    lastPopupTime = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否正在挑战中
        [JsonProperty]
        bool inChallenge;
        [JsonIgnore]
        public bool InChallenge
        {
            get
            {
                return inChallenge;
            }
            set
            {
                if(inChallenge != value)
                {
                    inChallenge = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 开始挑战的时间
        [JsonProperty]
        string startChallengeTime = "";
        [JsonIgnore]
        public string StartChallengeTime
        {
            get
            {
                return startChallengeTime;
            }
            set
            {
                if(startChallengeTime != value)
                {
                    startChallengeTime = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前进行的关卡的难度记录
        [JsonProperty]
        int levelDifficult;
        [JsonIgnore]
        public int LevelDifficult
        {
            get
            {
                return levelDifficult;
            }
            set
            {
                if(levelDifficult != value)
                {
                    levelDifficult = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 关卡内使用道具数记录
        [JsonProperty]
        int useBoost;
        [JsonIgnore]
        public int UseBoost
        {
            get
            {
                return useBoost;
            }
            set
            {
                if(useBoost != value)
                {
                    useBoost = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 关卡内复活次数记录
        [JsonProperty]
        int reviveCount;
        [JsonIgnore]
        public int ReviveCount
        {
            get
            {
                return reviveCount;
            }
            set
            {
                if(reviveCount != value)
                {
                    reviveCount = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 挑战完成时间
        [JsonProperty]
        string endChallengeTime = "";
        [JsonIgnore]
        public string EndChallengeTime
        {
            get
            {
                return endChallengeTime;
            }
            set
            {
                if(endChallengeTime != value)
                {
                    endChallengeTime = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前达到的等级
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
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前正在进行的等级
        [JsonProperty]
        int enterLevel;
        [JsonIgnore]
        public int EnterLevel
        {
            get
            {
                return enterLevel;
            }
            set
            {
                if(enterLevel != value)
                {
                    enterLevel = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前关的挑战结果
        [JsonProperty]
        bool enterLevelResult;
        [JsonIgnore]
        public bool EnterLevelResult
        {
            get
            {
                return enterLevelResult;
            }
            set
            {
                if(enterLevelResult != value)
                {
                    enterLevelResult = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最终挑战结果
        [JsonProperty]
        bool challengeResult;
        [JsonIgnore]
        public bool ChallengeResult
        {
            get
            {
                return challengeResult;
            }
            set
            {
                if(challengeResult != value)
                {
                    challengeResult = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前剩余机器人数量
        [JsonProperty]
        int robotCount;
        [JsonIgnore]
        public int RobotCount
        {
            get
            {
                return robotCount;
            }
            set
            {
                if(robotCount != value)
                {
                    robotCount = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}