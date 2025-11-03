/************************************************
 * Storage class : StorageTaskGroup
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTaskGroup : StorageBase
    {
        
        // 唯一ID
        [JsonProperty]
        int onlyId;
        [JsonIgnore]
        public int OnlyId
        {
            get
            {
                return onlyId;
            }
            set
            {
                if(onlyId != value)
                {
                    onlyId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 固定任务索引
        [JsonProperty]
        int orderFixIndex;
        [JsonIgnore]
        public int OrderFixIndex
        {
            get
            {
                return orderFixIndex;
            }
            set
            {
                if(orderFixIndex != value)
                {
                    orderFixIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 适配ORDER
        [JsonProperty]
        bool adaptOrder;
        [JsonIgnore]
        public bool AdaptOrder
        {
            get
            {
                return adaptOrder;
            }
            set
            {
                if(adaptOrder != value)
                {
                    adaptOrder = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 适配ORDER 个数
        [JsonProperty]
        bool adaptOrderNum;
        [JsonIgnore]
        public bool AdaptOrderNum
        {
            get
            {
                return adaptOrderNum;
            }
            set
            {
                if(adaptOrderNum != value)
                {
                    adaptOrderNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成任务总数
        [JsonProperty]
        int completeOrderNum;
        [JsonIgnore]
        public int CompleteOrderNum
        {
            get
            {
                return completeOrderNum;
            }
            set
            {
                if(completeOrderNum != value)
                {
                    completeOrderNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // ORDER TIME
        [JsonProperty]
        StorageDictionary<string,long> orderRefreshTime = new StorageDictionary<string,long>();
        [JsonIgnore]
        public StorageDictionary<string,long> OrderRefreshTime
        {
            get
            {
                return orderRefreshTime;
            }
        }
        // ---------------------------------//
         
        // LASTFINISHORDER
        [JsonProperty]
        StorageDictionary<int,StorageTaskItem> lastFinishOrder = new StorageDictionary<int,StorageTaskItem>();
        [JsonIgnore]
        public StorageDictionary<int,StorageTaskItem> LastFinishOrder
        {
            get
            {
                return lastFinishOrder;
            }
        }
        // ---------------------------------//
        
        // 每日完成的任务数量
        [JsonProperty]
        int dailyCompleteNum;
        [JsonIgnore]
        public int DailyCompleteNum
        {
            get
            {
                return dailyCompleteNum;
            }
            set
            {
                if(dailyCompleteNum != value)
                {
                    dailyCompleteNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成普通的任务总数
        [JsonProperty]
        int completeNormalNum;
        [JsonIgnore]
        public int CompleteNormalNum
        {
            get
            {
                return completeNormalNum;
            }
            set
            {
                if(completeNormalNum != value)
                {
                    completeNormalNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 困难任务缓存队列
        [JsonProperty]
        StorageList<int> hardTaskCacheQueue = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> HardTaskCacheQueue
        {
            get
            {
                return hardTaskCacheQueue;
            }
        }
        // ---------------------------------//
        
        // 是否初始化任务了
        [JsonProperty]
        bool isInitTask;
        [JsonIgnore]
        public bool IsInitTask
        {
            get
            {
                return isInitTask;
            }
            set
            {
                if(isInitTask != value)
                {
                    isInitTask = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否补全过任务
        [JsonProperty]
        bool isAlignTask;
        [JsonIgnore]
        public bool IsAlignTask
        {
            get
            {
                return isAlignTask;
            }
            set
            {
                if(isAlignTask != value)
                {
                    isAlignTask = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成的任务
        [JsonProperty]
        StorageDictionary<int,int> completedTaskIds = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> CompletedTaskIds
        {
            get
            {
                return completedTaskIds;
            }
        }
        // ---------------------------------//
        
        // 进行中的任务
        [JsonProperty]
        StorageList<StorageTaskItem> curTasks = new StorageList<StorageTaskItem>();
        [JsonIgnore]
        public StorageList<StorageTaskItem> CurTasks
        {
            get
            {
                return curTasks;
            }
        }
        // ---------------------------------//
        
        // 当前时间
        [JsonProperty]
        long currentDayTime;
        [JsonIgnore]
        public long CurrentDayTime
        {
            get
            {
                return currentDayTime;
            }
            set
            {
                if(currentDayTime != value)
                {
                    currentDayTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 每日完成任务个数
        [JsonProperty]
        int completeTaskNum;
        [JsonIgnore]
        public int CompleteTaskNum
        {
            get
            {
                return completeTaskNum;
            }
            set
            {
                if(completeTaskNum != value)
                {
                    completeTaskNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 刷新简单任务时间
        [JsonProperty]
        long refreshEasyTime;
        [JsonIgnore]
        public long RefreshEasyTime
        {
            get
            {
                return refreshEasyTime;
            }
            set
            {
                if(refreshEasyTime != value)
                {
                    refreshEasyTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否完成了所有任务
        [JsonProperty]
        bool isCompleteAllTask;
        [JsonIgnore]
        public bool IsCompleteAllTask
        {
            get
            {
                return isCompleteAllTask;
            }
            set
            {
                if(isCompleteAllTask != value)
                {
                    isCompleteAllTask = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前玩游戏的总时间
        [JsonProperty]
        long playGameTime;
        [JsonIgnore]
        public long PlayGameTime
        {
            get
            {
                return playGameTime;
            }
            set
            {
                if(playGameTime != value)
                {
                    playGameTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 普通动态任务索引
        [JsonProperty]
        int dynamicNormalIndex;
        [JsonIgnore]
        public int DynamicNormalIndex
        {
            get
            {
                return dynamicNormalIndex;
            }
            set
            {
                if(dynamicNormalIndex != value)
                {
                    dynamicNormalIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 普通动态任务ID
        [JsonProperty]
        int dynamicNormalId;
        [JsonIgnore]
        public int DynamicNormalId
        {
            get
            {
                return dynamicNormalId;
            }
            set
            {
                if(dynamicNormalId != value)
                {
                    dynamicNormalId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 特殊动态任务索引
        [JsonProperty]
        int dynamicSpecialIndex;
        [JsonIgnore]
        public int DynamicSpecialIndex
        {
            get
            {
                return dynamicSpecialIndex;
            }
            set
            {
                if(dynamicSpecialIndex != value)
                {
                    dynamicSpecialIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 特殊动态任务ID
        [JsonProperty]
        int dynamicSpecialId;
        [JsonIgnore]
        public int DynamicSpecialId
        {
            get
            {
                return dynamicSpecialId;
            }
            set
            {
                if(dynamicSpecialId != value)
                {
                    dynamicSpecialId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 衰减时间
        [JsonProperty]
        long attenuationTime;
        [JsonIgnore]
        public long AttenuationTime
        {
            get
            {
                return attenuationTime;
            }
            set
            {
                if(attenuationTime != value)
                {
                    attenuationTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 衰减次数
        [JsonProperty]
        int attenuationNum;
        [JsonIgnore]
        public int AttenuationNum
        {
            get
            {
                return attenuationNum;
            }
            set
            {
                if(attenuationNum != value)
                {
                    attenuationNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否处于衰减中
        [JsonProperty]
        bool isAttenuation;
        [JsonIgnore]
        public bool IsAttenuation
        {
            get
            {
                return isAttenuation;
            }
            set
            {
                if(isAttenuation != value)
                {
                    isAttenuation = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 补充时间
        [JsonProperty]
        long replenishTime;
        [JsonIgnore]
        public long ReplenishTime
        {
            get
            {
                return replenishTime;
            }
            set
            {
                if(replenishTime != value)
                {
                    replenishTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 补充任务时间
        [JsonProperty]
        long alignTime;
        [JsonIgnore]
        public long AlignTime
        {
            get
            {
                return alignTime;
            }
            set
            {
                if(alignTime != value)
                {
                    alignTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 产出序列索引
        [JsonProperty]
        StorageDictionary<int,int> mergeLineUnlockIndex = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> MergeLineUnlockIndex
        {
            get
            {
                return mergeLineUnlockIndex;
            }
        }
        // ---------------------------------//
        
        // 当前的正在预产出队列的物品
        [JsonProperty]
        StorageDictionary<int,int> mergeLineUnlockId = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> MergeLineUnlockId
        {
            get
            {
                return mergeLineUnlockId;
            }
        }
        // ---------------------------------//
        
        // 爽单记录的时间
        [JsonProperty]
        long readilyRecordsTime;
        [JsonIgnore]
        public long ReadilyRecordsTime
        {
            get
            {
                return readilyRecordsTime;
            }
            set
            {
                if(readilyRecordsTime != value)
                {
                    readilyRecordsTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前刷新爽单时间
        [JsonProperty]
        long readilyTime;
        [JsonIgnore]
        public long ReadilyTime
        {
            get
            {
                return readilyTime;
            }
            set
            {
                if(readilyTime != value)
                {
                    readilyTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 爽单当前完成任务个数
        [JsonProperty]
        int readilyCompleteNum;
        [JsonIgnore]
        public int ReadilyCompleteNum
        {
            get
            {
                return readilyCompleteNum;
            }
            set
            {
                if(readilyCompleteNum != value)
                {
                    readilyCompleteNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 贮藏爽单任务个数
        [JsonProperty]
        int readilyStorageNum;
        [JsonIgnore]
        public int ReadilyStorageNum
        {
            get
            {
                return readilyStorageNum;
            }
            set
            {
                if(readilyStorageNum != value)
                {
                    readilyStorageNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 爽单刷新个数
        [JsonProperty]
        int readilyRefreshNum;
        [JsonIgnore]
        public int ReadilyRefreshNum
        {
            get
            {
                return readilyRefreshNum;
            }
            set
            {
                if(readilyRefreshNum != value)
                {
                    readilyRefreshNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}