/************************************************
 * Storage class : StorageSaveTheWhales
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageSaveTheWhales : StorageBase
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
        
        // 分组
        [JsonProperty]
        int groupId;
        [JsonIgnore]
        public int GroupId
        {
            get
            {
                return groupId;
            }
            set
            {
                if(groupId != value)
                {
                    groupId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 任务开始时间
        [JsonProperty]
        long joinTime;
        [JsonIgnore]
        public long JoinTime
        {
            get
            {
                return joinTime;
            }
            set
            {
                if(joinTime != value)
                {
                    joinTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 任务结束时间
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
        
        // 收集数量
        [JsonProperty]
        int collectCount;
        [JsonIgnore]
        public int CollectCount
        {
            get
            {
                return collectCount;
            }
            set
            {
                if(collectCount != value)
                {
                    collectCount = value;
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
        
        // 消耗体力时间 
        [JsonProperty]
        long consumeEnergyTime;
        [JsonIgnore]
        public long ConsumeEnergyTime
        {
            get
            {
                return consumeEnergyTime;
            }
            set
            {
                if(consumeEnergyTime != value)
                {
                    consumeEnergyTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 消耗数量
        [JsonProperty]
        int consumeEnergy;
        [JsonIgnore]
        public int ConsumeEnergy
        {
            get
            {
                return consumeEnergy;
            }
            set
            {
                if(consumeEnergy != value)
                {
                    consumeEnergy = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 本次是否完成
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
        
        // 任务刷新天
        [JsonProperty]
        long joinDay;
        [JsonIgnore]
        public long JoinDay
        {
            get
            {
                return joinDay;
            }
            set
            {
                if(joinDay != value)
                {
                    joinDay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 失败次数
        [JsonProperty]
        int failCount;
        [JsonIgnore]
        public int FailCount
        {
            get
            {
                return failCount;
            }
            set
            {
                if(failCount != value)
                {
                    failCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 成功次数
        [JsonProperty]
        int successCount;
        [JsonIgnore]
        public int SuccessCount
        {
            get
            {
                return successCount;
            }
            set
            {
                if(successCount != value)
                {
                    successCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}