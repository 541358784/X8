/************************************************
 * Storage class : StorageClimbTower
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageClimbTower : StorageBase
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
        
        // 刷新时间
        [JsonProperty]
        long refreshTime;
        [JsonIgnore]
        public long RefreshTime
        {
            get
            {
                return refreshTime;
            }
            set
            {
                if(refreshTime != value)
                {
                    refreshTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 分层
        [JsonProperty]
        int payLevel;
        [JsonIgnore]
        public int PayLevel
        {
            get
            {
                return payLevel;
            }
            set
            {
                if(payLevel != value)
                {
                    payLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否进入过
        [JsonProperty]
        bool isEnter;
        [JsonIgnore]
        public bool IsEnter
        {
            get
            {
                return isEnter;
            }
            set
            {
                if(isEnter != value)
                {
                    isEnter = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前关卡ID
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
        
        // 游戏状态
        [JsonProperty]
        int state;
        [JsonIgnore]
        public int State
        {
            get
            {
                return state;
            }
            set
            {
                if(state != value)
                {
                    state = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否复活过
        [JsonProperty]
        bool hasReborn;
        [JsonIgnore]
        public bool HasReborn
        {
            get
            {
                return hasReborn;
            }
            set
            {
                if(hasReborn != value)
                {
                    hasReborn = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否付费过
        [JsonProperty]
        bool isPay;
        [JsonIgnore]
        public bool IsPay
        {
            get
            {
                return isPay;
            }
            set
            {
                if(isPay != value)
                {
                    isPay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否付费关卡
        [JsonProperty]
        bool isPayLevel;
        [JsonIgnore]
        public bool IsPayLevel
        {
            get
            {
                return isPayLevel;
            }
            set
            {
                if(isPayLevel != value)
                {
                    isPayLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前奖励
        [JsonProperty]
        StorageList<StorageResData> rewards = new StorageList<StorageResData>();
        [JsonIgnore]
        public StorageList<StorageResData> Rewards
        {
            get
            {
                return rewards;
            }
        }
        // ---------------------------------//
        
        // 剩余免费次数
        [JsonProperty]
        int freeTimes;
        [JsonIgnore]
        public int FreeTimes
        {
            get
            {
                return freeTimes;
            }
            set
            {
                if(freeTimes != value)
                {
                    freeTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 钻石购买次数
        [JsonProperty]
        int payTimes;
        [JsonIgnore]
        public int PayTimes
        {
            get
            {
                return payTimes;
            }
            set
            {
                if(payTimes != value)
                {
                    payTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前生成的关卡缓存
        [JsonProperty]
        int levelIdCache;
        [JsonIgnore]
        public int LevelIdCache
        {
            get
            {
                return levelIdCache;
            }
            set
            {
                if(levelIdCache != value)
                {
                    levelIdCache = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 缓存奖励
        [JsonProperty]
        StorageList<int> rewardIndexCache = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> RewardIndexCache
        {
            get
            {
                return rewardIndexCache;
            }
        }
        // ---------------------------------//
        
        // 缓存奖励
        [JsonProperty]
        StorageList<int> rewardIdCache = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> RewardIdCache
        {
            get
            {
                return rewardIdCache;
            }
        }
        // ---------------------------------//
        
        // 缓存数量
        [JsonProperty]
        StorageList<int> rewardNumCache = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> RewardNumCache
        {
            get
            {
                return rewardNumCache;
            }
        }
        // ---------------------------------//
        
        // 打开状态缓存
        [JsonProperty]
        StorageList<int> openStateCache = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> OpenStateCache
        {
            get
            {
                return openStateCache;
            }
        }
        // ---------------------------------//
        
    }
}