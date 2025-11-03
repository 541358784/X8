/************************************************
 * Storage class : StorageSeaRacing
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageSeaRacing : StorageBase
    {
        
        // 触发活动时的ACTIVITYID
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
         
        // 每一轮的存档
        [JsonProperty]
        StorageDictionary<int,StorageSeaRacingRound> seaRacingRoundList = new StorageDictionary<int,StorageSeaRacingRound>();
        [JsonIgnore]
        public StorageDictionary<int,StorageSeaRacingRound> SeaRacingRoundList
        {
            get
            {
                return seaRacingRoundList;
            }
        }
        // ---------------------------------//
        
        // 当前轮次ID
        [JsonProperty]
        int currencyRoundIndex;
        [JsonIgnore]
        public int CurrencyRoundIndex
        {
            get
            {
                return currencyRoundIndex;
            }
            set
            {
                if(currencyRoundIndex != value)
                {
                    currencyRoundIndex = value;
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
        
        // 是否完成结算
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
        
        // 是否已完成所有轮次
        [JsonProperty]
        bool isCompletedAll;
        [JsonIgnore]
        public bool IsCompletedAll
        {
            get
            {
                return isCompletedAll;
            }
            set
            {
                if(isCompletedAll != value)
                {
                    isCompletedAll = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}