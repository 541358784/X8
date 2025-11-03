/************************************************
 * Storage class : StorageGardenTreasure
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageGardenTreasure : StorageBase
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
        
        // 预热结束时间
        [JsonProperty]
        long preheatEndTime;
        [JsonIgnore]
        public long PreheatEndTime
        {
            get
            {
                return preheatEndTime;
            }
            set
            {
                if(preheatEndTime != value)
                {
                    preheatEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 展示的关卡ID
        [JsonProperty]
        int showLevelId;
        [JsonIgnore]
        public int ShowLevelId
        {
            get
            {
                return showLevelId;
            }
            set
            {
                if(showLevelId != value)
                {
                    showLevelId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前棋盘ID
        [JsonProperty]
        int boardId;
        [JsonIgnore]
        public int BoardId
        {
            get
            {
                return boardId;
            }
            set
            {
                if(boardId != value)
                {
                    boardId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 普通关卡ID
        [JsonProperty]
        int normalLevelId;
        [JsonIgnore]
        public int NormalLevelId
        {
            get
            {
                return normalLevelId;
            }
            set
            {
                if(normalLevelId != value)
                {
                    normalLevelId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 随机关卡ID
        [JsonProperty]
        int randomLevelId;
        [JsonIgnore]
        public int RandomLevelId
        {
            get
            {
                return randomLevelId;
            }
            set
            {
                if(randomLevelId != value)
                {
                    randomLevelId = value;
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
        
        // 打开的格子
        [JsonProperty]
        StorageList<int> openGrids = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> OpenGrids
        {
            get
            {
                return openGrids;
            }
        }
        // ---------------------------------//
        
        // 获得的形状
        [JsonProperty]
        StorageList<int> getShapes = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> GetShapes
        {
            get
            {
                return getShapes;
            }
        }
        // ---------------------------------//
        
        // 是否随机关卡
        [JsonProperty]
        bool isRandomLevel;
        [JsonIgnore]
        public bool IsRandomLevel
        {
            get
            {
                return isRandomLevel;
            }
            set
            {
                if(isRandomLevel != value)
                {
                    isRandomLevel = value;
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
        
        // 进入关卡次数
        [JsonProperty]
        StorageDictionary<int,int> recordEnterLevelCount = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> RecordEnterLevelCount
        {
            get
            {
                return recordEnterLevelCount;
            }
        }
        // ---------------------------------//
        
        // 记录消耗
        [JsonProperty]
        StorageDictionary<int,int> recordConsume = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> RecordConsume
        {
            get
            {
                return recordConsume;
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