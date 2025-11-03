/************************************************
 * Storage class : StorageCollectStone
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCollectStone : StorageBase
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
        
        // 付费分层组
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
        
        // 当前石头个数
        [JsonProperty]
        int stoneNum;
        [JsonIgnore]
        public int StoneNum
        {
            get
            {
                return stoneNum;
            }
            set
            {
                if(stoneNum != value)
                {
                    stoneNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否是循环关卡
        [JsonProperty]
        bool isLoop;
        [JsonIgnore]
        public bool IsLoop
        {
            get
            {
                return isLoop;
            }
            set
            {
                if(isLoop != value)
                {
                    isLoop = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 关卡ID
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
        
        // 当前固定索引
        [JsonProperty]
        int fixIndex;
        [JsonIgnore]
        public int FixIndex
        {
            get
            {
                return fixIndex;
            }
            set
            {
                if(fixIndex != value)
                {
                    fixIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 循环索引
        [JsonProperty]
        int loopIndex;
        [JsonIgnore]
        public int LoopIndex
        {
            get
            {
                return loopIndex;
            }
            set
            {
                if(loopIndex != value)
                {
                    loopIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 获取状态
        [JsonProperty]
        StorageList<int> state = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> State
        {
            get
            {
                return state;
            }
        }
        // ---------------------------------//
        
    }
}