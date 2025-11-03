/************************************************
 * Storage class : StorageStage
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage.Decoration
{
    [System.Serializable]
    public class StorageStage : StorageBase
    {
         
        // 该区域内的挂点详情
        [JsonProperty]
        StorageDictionary<int,StorageNode> nodesData = new StorageDictionary<int,StorageNode>();
        [JsonIgnore]
        public StorageDictionary<int,StorageNode> NodesData
        {
            get
            {
                return nodesData;
            }
        }
        // ---------------------------------//
        
        // 当前状态
        [JsonProperty]
        int state;
        [JsonIgnore]
        public int State
        {
            get
            {
                return state;
            }
            set
            {
                if(state != value)
                {
                    state = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}