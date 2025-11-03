/************************************************
 * Storage class : StorageJungleAdventure
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageJungleAdventure : StorageBase
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
        
        // 是否加入
        [JsonProperty]
        bool isJoin;
        [JsonIgnore]
        public bool IsJoin
        {
            get
            {
                return isJoin;
            }
            set
            {
                if(isJoin != value)
                {
                    isJoin = value;
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
        
        // 活动预热开始时间
        [JsonProperty]
        long preStartActivityTime;
        [JsonIgnore]
        public long PreStartActivityTime
        {
            get
            {
                return preStartActivityTime;
            }
            set
            {
                if(preStartActivityTime != value)
                {
                    preStartActivityTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动预热结束时间
        [JsonProperty]
        long preActivityEndTime;
        [JsonIgnore]
        public long PreActivityEndTime
        {
            get
            {
                return preActivityEndTime;
            }
            set
            {
                if(preActivityEndTime != value)
                {
                    preActivityEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 参加活动开始时间
        [JsonProperty]
        long joinStartTime;
        [JsonIgnore]
        public long JoinStartTime
        {
            get
            {
                return joinStartTime;
            }
            set
            {
                if(joinStartTime != value)
                {
                    joinStartTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 参加活动结束时间
        [JsonProperty]
        long joinEndTime;
        [JsonIgnore]
        public long JoinEndTime
        {
            get
            {
                return joinEndTime;
            }
            set
            {
                if(joinEndTime != value)
                {
                    joinEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 总分数
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
        
        // 动画记录阶段分数
        [JsonProperty]
        int animScore;
        [JsonIgnore]
        public int AnimScore
        {
            get
            {
                return animScore;
            }
            set
            {
                if(animScore != value)
                {
                    animScore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前阶段分数
        [JsonProperty]
        int currentScore;
        [JsonIgnore]
        public int CurrentScore
        {
            get
            {
                return currentScore;
            }
            set
            {
                if(currentScore != value)
                {
                    currentScore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前阶段
        [JsonProperty]
        int stage;
        [JsonIgnore]
        public int Stage
        {
            get
            {
                return stage;
            }
            set
            {
                if(stage != value)
                {
                    stage = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 领奖状态
        [JsonProperty]
        StorageDictionary<string,int> getRewardState = new StorageDictionary<string,int>();
        [JsonIgnore]
        public StorageDictionary<string,int> GetRewardState
        {
            get
            {
                return getRewardState;
            }
        }
        // ---------------------------------//
        
        // 活动数据
        [JsonProperty]
        string activityJson = "";
        [JsonIgnore]
        public string ActivityJson
        {
            get
            {
                return activityJson;
            }
            set
            {
                if(activityJson != value)
                {
                    activityJson = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动状态
        [JsonProperty]
        int activityStatus;
        [JsonIgnore]
        public int ActivityStatus
        {
            get
            {
                return activityStatus;
            }
            set
            {
                if(activityStatus != value)
                {
                    activityStatus = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}