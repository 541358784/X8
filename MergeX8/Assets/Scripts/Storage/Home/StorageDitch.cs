/************************************************
 * Storage class : StorageDitch
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDitch : StorageBase
    {
        
        // 完成的挖沟关卡
        [JsonProperty]
        StorageDictionary<int,int> finishDitchLevel = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> FinishDitchLevel
        {
            get
            {
                return finishDitchLevel;
            }
        }
        // ---------------------------------//
        
    }
}