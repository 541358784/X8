/************************************************
 * Storage class : StorageStimulate
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageStimulate : StorageBase
    {
        
        // 完成情况
        [JsonProperty]
        StorageDictionary<int,bool> finishInfo = new StorageDictionary<int,bool>();
        [JsonIgnore]
        public StorageDictionary<int,bool> FinishInfo
        {
            get
            {
                return finishInfo;
            }
        }
        // ---------------------------------//
         
        // 当前节点
        [JsonProperty]
        StorageDictionary<int,StorageStimulateLevel> levels = new StorageDictionary<int,StorageStimulateLevel>();
        [JsonIgnore]
        public StorageDictionary<int,StorageStimulateLevel> Levels
        {
            get
            {
                return levels;
            }
        }
        // ---------------------------------//
        
    }
}