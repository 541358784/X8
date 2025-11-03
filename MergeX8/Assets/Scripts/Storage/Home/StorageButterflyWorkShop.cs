/************************************************
 * Storage class : StorageButterflyWorkShop
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageButterflyWorkShop : StorageBase
    {
        
        // 是否开启
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
        
        // 领取的奖励
        [JsonProperty]
        StorageDictionary<int,int> collectRewards = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> CollectRewards
        {
            get
            {
                return collectRewards;
            }
        }
        // ---------------------------------//
        
        // 未放置的活动棋子
        [JsonProperty]
        StorageList<int> unSetItems = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> UnSetItems
        {
            get
            {
                return unSetItems;
            }
        }
        // ---------------------------------//
        
        // 活动开始时间
        [JsonProperty]
        long startActivityTime;
        [JsonIgnore]
        public long StartActivityTime
        {
            get
            {
                return startActivityTime;
            }
            set
            {
                if(startActivityTime != value)
                {
                    startActivityTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动结束时间
        [JsonProperty]
        long activityEndTime;
        [JsonIgnore]
        public long ActivityEndTime
        {
            get
            {
                return activityEndTime;
            }
            set
            {
                if(activityEndTime != value)
                {
                    activityEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 今日产出的次数
        [JsonProperty]
        int dayProductCount;
        [JsonIgnore]
        public int DayProductCount
        {
            get
            {
                return dayProductCount;
            }
            set
            {
                if(dayProductCount != value)
                {
                    dayProductCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 今日日期
        [JsonProperty]
        int dayId;
        [JsonIgnore]
        public int DayId
        {
            get
            {
                return dayId;
            }
            set
            {
                if(dayId != value)
                {
                    dayId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最高解锁的活动ICON等级
        [JsonProperty]
        int maxUnlockLevel;
        [JsonIgnore]
        public int MaxUnlockLevel
        {
            get
            {
                return maxUnlockLevel;
            }
            set
            {
                if(maxUnlockLevel != value)
                {
                    maxUnlockLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前阶段
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
        
        // 已领取
        [JsonProperty]
        StorageList<int> claimedItem = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> ClaimedItem
        {
            get
            {
                return claimedItem;
            }
        }
        // ---------------------------------//
        
        // 随机领序列
        [JsonProperty]
        int randomLine;
        [JsonIgnore]
        public int RandomLine
        {
            get
            {
                return randomLine;
            }
            set
            {
                if(randomLine != value)
                {
                    randomLine = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 阶段 
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
        
        // 阶段分数
        [JsonProperty]
        int stageStore;
        [JsonIgnore]
        public int StageStore
        {
            get
            {
                return stageStore;
            }
            set
            {
                if(stageStore != value)
                {
                    stageStore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 分层组ID
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