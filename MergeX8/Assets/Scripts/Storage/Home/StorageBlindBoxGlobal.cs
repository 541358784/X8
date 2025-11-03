/************************************************
 * Storage class : StorageBlindBoxGlobal
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageBlindBoxGlobal : StorageBase
    {
        
        // 回收积分
        [JsonProperty]
        int recycleValue;
        [JsonIgnore]
        public int RecycleValue
        {
            get
            {
                return recycleValue;
            }
            set
            {
                if(recycleValue != value)
                {
                    recycleValue = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}