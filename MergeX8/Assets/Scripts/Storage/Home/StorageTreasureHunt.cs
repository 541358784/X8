/************************************************
 * Storage class : StorageTreasureHunt
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTreasureHunt : StorageBase
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
        
        // 分组
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
        
        // 是否开始 
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
        
        // 体力消耗
        [JsonProperty]
        int energyCost;
        [JsonIgnore]
        public int EnergyCost
        {
            get
            {
                return energyCost;
            }
            set
            {
                if(energyCost != value)
                {
                    energyCost = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 锤子
        [JsonProperty]
        int hammer;
        [JsonIgnore]
        public int Hammer
        {
            get
            {
                return hammer;
            }
            set
            {
                if(hammer != value)
                {
                    hammer = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 关卡
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
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 商店购买进度
        [JsonProperty]
        int storeLevel;
        [JsonIgnore]
        public int StoreLevel
        {
            get
            {
                return storeLevel;
            }
            set
            {
                if(storeLevel != value)
                {
                    storeLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前敲的次数
        [JsonProperty]
        int breakCount;
        [JsonIgnore]
        public int BreakCount
        {
            get
            {
                return breakCount;
            }
            set
            {
                if(breakCount != value)
                {
                    breakCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已敲位置
        [JsonProperty]
        StorageList<int> breakItems = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> BreakItems
        {
            get
            {
                return breakItems;
            }
        }
        // ---------------------------------//
        
        // 随机奖励获得记录
        [JsonProperty]
        StorageList<int> randomReward = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> RandomReward
        {
            get
            {
                return randomReward;
            }
        }
        // ---------------------------------//
        
        // 
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
        
        // 记录任务得锤子数量
        [JsonProperty]
        int taskHummer;
        [JsonIgnore]
        public int TaskHummer
        {
            get
            {
                return taskHummer;
            }
            set
            {
                if(taskHummer != value)
                {
                    taskHummer = value;
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