/************************************************
 * Storage class : StorageFilthyLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageFilthyLevel : StorageBase
    {
         
        // ITEMS
        [JsonProperty]
        StorageDictionary<int,StorageFilthyNode> nodes = new StorageDictionary<int,StorageFilthyNode>();
        [JsonIgnore]
        public StorageDictionary<int,StorageFilthyNode> Nodes
        {
            get
            {
                return nodes;
            }
        }
        // ---------------------------------//
        
    }
}