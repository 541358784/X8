/************************************************
 * Storage class : StorageFilthyNode
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageFilthyNode : StorageBase
    {
        
        // ID
        [JsonProperty]
        int id;
        [JsonIgnore]
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if(id != value)
                {
                    id = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 状态
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