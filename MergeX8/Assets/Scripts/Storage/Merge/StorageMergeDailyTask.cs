/************************************************
 * Storage class : StorageMergeDailyTask
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMergeDailyTask : StorageBase
    {
        
        // 当前任务ID
        [JsonProperty]
        int id;
        [JsonIgnore]
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if(id != value)
                {
                    id = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 任务表中的ID
        [JsonProperty]
        int taskID;
        [JsonIgnore]
        public int TaskID
        {
            get
            {
                return taskID;
            }
            set
            {
                if(taskID != value)
                {
                    taskID = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前所需物品
        [JsonProperty]
        int demonds;
        [JsonIgnore]
        public int Demonds
        {
            get
            {
                return demonds;
            }
            set
            {
                if(demonds != value)
                {
                    demonds = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前所需物品数量
        [JsonProperty]
        int demondsCount;
        [JsonIgnore]
        public int DemondsCount
        {
            get
            {
                return demondsCount;
            }
            set
            {
                if(demondsCount != value)
                {
                    demondsCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成任务时间
        [JsonProperty]
        ulong operateTime;
        [JsonIgnore]
        public ulong OperateTime
        {
            get
            {
                return operateTime;
            }
            set
            {
                if(operateTime != value)
                {
                    operateTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前任务进度
        [JsonProperty]
        int progress;
        [JsonIgnore]
        public int Progress
        {
            get
            {
                return progress;
            }
            set
            {
                if(progress != value)
                {
                    progress = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 奖励ID
        [JsonProperty]
        int awards;
        [JsonIgnore]
        public int Awards
        {
            get
            {
                return awards;
            }
            set
            {
                if(awards != value)
                {
                    awards = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 奖励数量
        [JsonProperty]
        int awardsCount;
        [JsonIgnore]
        public int AwardsCount
        {
            get
            {
                return awardsCount;
            }
            set
            {
                if(awardsCount != value)
                {
                    awardsCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否完成每日任务
        [JsonProperty]
        bool isComplete;
        [JsonIgnore]
        public bool IsComplete
        {
            get
            {
                return isComplete;
            }
            set
            {
                if(isComplete != value)
                {
                    isComplete = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}