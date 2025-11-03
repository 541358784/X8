/************************************************
 * Storage class : StorageCardCollectionCardBook
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCardCollectionCardBook : StorageBase
    {
        
        // 卡册ID
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
        
        // 卡册完成时间
        [JsonProperty]
        ulong completedTime;
        [JsonIgnore]
        public ulong CompletedTime
        {
            get
            {
                return completedTime;
            }
            set
            {
                if(completedTime != value)
                {
                    completedTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}