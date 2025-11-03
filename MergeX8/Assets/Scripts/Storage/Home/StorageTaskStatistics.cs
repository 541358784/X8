/************************************************
 * Storage class : StorageTaskStatistics
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTaskStatistics : StorageBase
    {
         
        // 任务统计信息
        [JsonProperty]
        StorageDictionary<string,StorageTaskStatisticsDetailed> statistics = new StorageDictionary<string,StorageTaskStatisticsDetailed>();
        [JsonIgnore]
        public StorageDictionary<string,StorageTaskStatisticsDetailed> Statistics
        {
            get
            {
                return statistics;
            }
        }
        // ---------------------------------//
        
    }
}