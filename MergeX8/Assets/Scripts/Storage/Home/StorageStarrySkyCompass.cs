/************************************************
 * Storage class : StorageStarrySkyCompass
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageStarrySkyCompass : StorageBase
    {
        
        // 活动ID
        [JsonProperty]
        string activiryId = "";
        [JsonIgnore]
        public string ActiviryId
        {
            get
            {
                return activiryId;
            }
            set
            {
                if(activiryId != value)
                {
                    activiryId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 预热时间戳(如果活动开启则进行更新，否则用本地存的)
        [JsonProperty]
        long preheatTime;
        [JsonIgnore]
        public long PreheatTime
        {
            get
            {
                return preheatTime;
            }
            set
            {
                if(preheatTime != value)
                {
                    preheatTime = value;
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
        
        // 分数
        [JsonProperty]
        int score;
        [JsonIgnore]
        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                if(score != value)
                {
                    score = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 火箭数量
        [JsonProperty]
        int rocketCount;
        [JsonIgnore]
        public int RocketCount
        {
            get
            {
                return rocketCount;
            }
            set
            {
                if(rocketCount != value)
                {
                    rocketCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 特殊玩法触发累计值
        [JsonProperty]
        int happyValue;
        [JsonIgnore]
        public int HappyValue
        {
            get
            {
                return happyValue;
            }
            set
            {
                if(happyValue != value)
                {
                    happyValue = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 特殊玩法结束时间
        [JsonProperty]
        long happyEndTime;
        [JsonIgnore]
        public long HappyEndTime
        {
            get
            {
                return happyEndTime;
            }
            set
            {
                if(happyEndTime != value)
                {
                    happyEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当次特殊玩法转动次数
        [JsonProperty]
        int happySpinCount;
        [JsonIgnore]
        public int HappySpinCount
        {
            get
            {
                return happySpinCount;
            }
            set
            {
                if(happySpinCount != value)
                {
                    happySpinCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 特殊玩法结果记录
        [JsonProperty]
        StorageList<int> happySpinHistory = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> HappySpinHistory
        {
            get
            {
                return happySpinHistory;
            }
        }
        // ---------------------------------//
        
        // 购买记录
        [JsonProperty]
        StorageDictionary<int,int> buyState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> BuyState
        {
            get
            {
                return buyState;
            }
        }
        // ---------------------------------//
        
    }
}