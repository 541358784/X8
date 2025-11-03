/************************************************
 * Storage class : StorageThreeGift
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageThreeGift : StorageBase
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
        
        // 是否已购买
        [JsonProperty]
        bool isPurchase;
        [JsonIgnore]
        public bool IsPurchase
        {
            get
            {
                return isPurchase;
            }
            set
            {
                if(isPurchase != value)
                {
                    isPurchase = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当期分组ID
        [JsonProperty]
        int groupId;
        [JsonIgnore]
        public int GroupId
        {
            get
            {
                return groupId;
            }
            set
            {
                if(groupId != value)
                {
                    groupId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}