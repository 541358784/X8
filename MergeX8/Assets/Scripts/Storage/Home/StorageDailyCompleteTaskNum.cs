/************************************************
 * Storage class : StorageDailyCompleteTaskNum
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDailyCompleteTaskNum : StorageBase
    {
        
        // 当前天数
        [JsonProperty]
        int dayNum;
        [JsonIgnore]
        public int DayNum
        {
            get
            {
                return dayNum;
            }
            set
            {
                if(dayNum != value)
                {
                    dayNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成任务数量
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
        
    }
}