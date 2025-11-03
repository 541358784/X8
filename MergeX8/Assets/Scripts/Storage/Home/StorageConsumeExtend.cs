/************************************************
 * Storage class : StorageConsumeExtend
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageConsumeExtend : StorageBase
    {
         
        // 消耗记录
        [JsonProperty]
        StorageDictionary<string,StorageConsumeExtendRecord> consumeRecords = new StorageDictionary<string,StorageConsumeExtendRecord>();
        [JsonIgnore]
        public StorageDictionary<string,StorageConsumeExtendRecord> ConsumeRecords
        {
            get
            {
                return consumeRecords;
            }
        }
        // ---------------------------------//
        
    }
}