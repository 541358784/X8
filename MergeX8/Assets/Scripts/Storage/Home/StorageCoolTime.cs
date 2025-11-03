/************************************************
 * Storage class : StorageCoolTime
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCoolTime : StorageBase
    {
        
        // 不是同一天
        [JsonProperty]
        StorageDictionary<string,long> otherDay = new StorageDictionary<string,long>();
        [JsonIgnore]
        public StorageDictionary<string,long> OtherDay
        {
            get
            {
                return otherDay;
            }
        }
        // ---------------------------------//
        
        // 记录过去时间
        [JsonProperty]
        StorageDictionary<string,long> lossTime = new StorageDictionary<string,long>();
        [JsonIgnore]
        public StorageDictionary<string,long> LossTime
        {
            get
            {
                return lossTime;
            }
        }
        // ---------------------------------//
        
        // 间隔多少时间 S
        [JsonProperty]
        StorageDictionary<string,long> intervalTime = new StorageDictionary<string,long>();
        [JsonIgnore]
        public StorageDictionary<string,long> IntervalTime
        {
            get
            {
                return intervalTime;
            }
        }
        // ---------------------------------//
        
    }
}