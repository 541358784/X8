/************************************************
 * Storage class : StoragePigBankId
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StoragePigBankId : StorageBase
    {
        
        // 小猪ID
        [JsonProperty]
        StorageList<int> ids = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> Ids
        {
            get
            {
                return ids;
            }
        }
        // ---------------------------------//
        
    }
}