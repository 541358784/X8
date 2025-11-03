/************************************************
 * Storage class : StorageCustomTask
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCustomTask : StorageBase
    {
        
        // 每日任务
        [JsonProperty]
        StorageCustomTaskData dailyTaskData = new StorageCustomTaskData();
        [JsonIgnore]
        public StorageCustomTaskData DailyTaskData
        {
            get
            {
                return dailyTaskData;
            }
        }
        // ---------------------------------//
        
        // 每周任务
        [JsonProperty]
        StorageCustomTaskData weekTaskData = new StorageCustomTaskData();
        [JsonIgnore]
        public StorageCustomTaskData WeekTaskData
        {
            get
            {
                return weekTaskData;
            }
        }
        // ---------------------------------//
        
    }
}