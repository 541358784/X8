/************************************************
 * Storage class : StorageBiuBiu
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageBiuBiu : StorageBase
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
        
        // 显示状态
        [JsonProperty]
        StorageList<int> showState = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> ShowState
        {
            get
            {
                return showState;
            }
        }
        // ---------------------------------//
        
        // 命运
        [JsonProperty]
        StorageList<int> fate = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> Fate
        {
            get
            {
                return fate;
            }
        }
        // ---------------------------------//
        
        // 轮回
        [JsonProperty]
        int round;
        [JsonIgnore]
        public int Round
        {
            get
            {
                return round;
            }
            set
            {
                if(round != value)
                {
                    round = value;
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
        
    }
}