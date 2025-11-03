/************************************************
 * Storage class : StorageItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage.Decoration
{
    [System.Serializable]
    public class StorageItem : StorageBase
    {
        
        // 建筑状态
        [JsonProperty]
        int status;
        [JsonIgnore]
        public int Status
        {
            get
            {
                return status;
            }
            set
            {
                if(status != value)
                {
                    status = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}