/************************************************
 * Storage class : StorageNode
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage.Decoration
{
    [System.Serializable]
    public class StorageNode : StorageBase
    {
        
        // NODEID
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
        
        // 挂点状态
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
        
        // 当前使用的ITEMID
        [JsonProperty]
        int currentItemId;
        [JsonIgnore]
        public int CurrentItemId
        {
            get
            {
                return currentItemId;
            }
            set
            {
                if(currentItemId != value)
                {
                    currentItemId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
         
        // 
        [JsonProperty]
        StorageDictionary<int,StorageItem> itemsData = new StorageDictionary<int,StorageItem>();
        [JsonIgnore]
        public StorageDictionary<int,StorageItem> ItemsData
        {
            get
            {
                return itemsData;
            }
        }
        // ---------------------------------//
        
    }
}