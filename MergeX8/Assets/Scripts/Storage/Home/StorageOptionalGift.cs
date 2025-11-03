/************************************************
 * Storage class : StorageOptionalGift
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageOptionalGift : StorageBase
    {
        
        // 活动ID
        [JsonProperty]
        string activityId = "";
        [JsonIgnore]
        public string ActivityId
        {
            get
            {
                return activityId;
            }
            set
            {
                if(activityId != value)
                {
                    activityId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 选择状态
        [JsonProperty]
        StorageDictionary<int,int> selectItem = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> SelectItem
        {
            get
            {
                return selectItem;
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        bool isBuy;
        [JsonIgnore]
        public bool IsBuy
        {
            get
            {
                return isBuy;
            }
            set
            {
                if(isBuy != value)
                {
                    isBuy = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}