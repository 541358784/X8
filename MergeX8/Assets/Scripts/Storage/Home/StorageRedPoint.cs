/************************************************
 * Storage class : StorageRedPoint
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageRedPoint : StorageBase
    {
        
        // 红点系统
        [JsonProperty]
        StorageDictionary<int,bool> redPointData = new StorageDictionary<int,bool>();
        [JsonIgnore]
        public StorageDictionary<int,bool> RedPointData
        {
            get
            {
                return redPointData;
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        StorageDictionary<string,bool> redPointChildData = new StorageDictionary<string,bool>();
        [JsonIgnore]
        public StorageDictionary<string,bool> RedPointChildData
        {
            get
            {
                return redPointChildData;
            }
        }
        // ---------------------------------//
        
    }
}