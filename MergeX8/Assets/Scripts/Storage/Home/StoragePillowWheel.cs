/************************************************
 * Storage class : StoragePillowWheel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StoragePillowWheel : StorageBase
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
        
        // 预热结束时间戳
        [JsonProperty]
        long preheatCompleteTime;
        [JsonIgnore]
        public long PreheatCompleteTime
        {
            get
            {
                return preheatCompleteTime;
            }
            set
            {
                if(preheatCompleteTime != value)
                {
                    preheatCompleteTime = value;
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
        
        // 是否弹出过开始弹窗
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
        
        // 是否弹出清算弹窗
        [JsonProperty]
        bool isEnd;
        [JsonIgnore]
        public bool IsEnd
        {
            get
            {
                return isEnd;
            }
            set
            {
                if(isEnd != value)
                {
                    isEnd = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 分层组
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
        
        // 道具数量
        [JsonProperty]
        int itemCount;
        [JsonIgnore]
        public int ItemCount
        {
            get
            {
                return itemCount;
            }
            set
            {
                if(itemCount != value)
                {
                    itemCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 收集状态
        [JsonProperty]
        StorageList<int> collectState = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CollectState
        {
            get
            {
                return collectState;
            }
        }
        // ---------------------------------//
        
        // 特殊奖励收集状态
        [JsonProperty]
        StorageDictionary<int,int> specialCollectState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> SpecialCollectState
        {
            get
            {
                return specialCollectState;
            }
        }
        // ---------------------------------//
        
        // 总道具数量
        [JsonProperty]
        int totalItemCount;
        [JsonIgnore]
        public int TotalItemCount
        {
            get
            {
                return totalItemCount;
            }
            set
            {
                if(totalItemCount != value)
                {
                    totalItemCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}