/************************************************
 * Storage class : StorageSeaRacingRobot
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageSeaRacingRobot : StorageBase
    {
        
        // 机器人配置ID
        [JsonProperty]
        int robotConfigId;
        [JsonIgnore]
        public int RobotConfigId
        {
            get
            {
                return robotConfigId;
            }
            set
            {
                if(robotConfigId != value)
                {
                    robotConfigId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 机器人当前分数
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
        
        // 机器人名字
        [JsonProperty]
        string playerName = "";
        [JsonIgnore]
        public string PlayerName
        {
            get
            {
                return playerName;
            }
            set
            {
                if(playerName != value)
                {
                    playerName = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 机器人头像ID
        [JsonProperty]
        int avatarIconId;
        [JsonIgnore]
        public int AvatarIconId
        {
            get
            {
                return avatarIconId;
            }
            set
            {
                if(avatarIconId != value)
                {
                    avatarIconId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 机器人类型1:按照时间增长2:永远比玩家分低
        [JsonProperty]
        int robotType;
        [JsonIgnore]
        public int RobotType
        {
            get
            {
                return robotType;
            }
            set
            {
                if(robotType != value)
                {
                    robotType = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 限制在玩家分数的比例(类型2专属)
        [JsonProperty]
        float scoreLimit;
        [JsonIgnore]
        public float ScoreLimit
        {
            get
            {
                return scoreLimit;
            }
            set
            {
                if(scoreLimit != value)
                {
                    scoreLimit = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 更新分数时间(毫秒)
        [JsonProperty]
        long updateScoreTime;
        [JsonIgnore]
        public long UpdateScoreTime
        {
            get
            {
                return updateScoreTime;
            }
            set
            {
                if(updateScoreTime != value)
                {
                    updateScoreTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 更新分数次数
        [JsonProperty]
        int updateScoreCount;
        [JsonIgnore]
        public int UpdateScoreCount
        {
            get
            {
                return updateScoreCount;
            }
            set
            {
                if(updateScoreCount != value)
                {
                    updateScoreCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 更新分数最大次数
        [JsonProperty]
        int updateScoreMaxCount;
        [JsonIgnore]
        public int UpdateScoreMaxCount
        {
            get
            {
                return updateScoreMaxCount;
            }
            set
            {
                if(updateScoreMaxCount != value)
                {
                    updateScoreMaxCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 下次更新分数间隔(毫秒)
        [JsonProperty]
        long updateScoreInterval;
        [JsonIgnore]
        public long UpdateScoreInterval
        {
            get
            {
                return updateScoreInterval;
            }
            set
            {
                if(updateScoreInterval != value)
                {
                    updateScoreInterval = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 下次更新增加的分数
        [JsonProperty]
        int updateScoreValue;
        [JsonIgnore]
        public int UpdateScoreValue
        {
            get
            {
                return updateScoreValue;
            }
            set
            {
                if(updateScoreValue != value)
                {
                    updateScoreValue = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 随机配置ID
        [JsonProperty]
        int randomConfigId;
        [JsonIgnore]
        public int RandomConfigId
        {
            get
            {
                return randomConfigId;
            }
            set
            {
                if(randomConfigId != value)
                {
                    randomConfigId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}