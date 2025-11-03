/************************************************
 * Storage class : StorageStimulateLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageStimulateLevel : StorageBase
    {
         
        // ITEMS
        [JsonProperty]
        StorageDictionary<int,StorageStimulateNode> nodes = new StorageDictionary<int,StorageStimulateNode>();
        [JsonIgnore]
        public StorageDictionary<int,StorageStimulateNode> Nodes
        {
            get
            {
                return nodes;
            }
        }
        // ---------------------------------//
        
    }
}