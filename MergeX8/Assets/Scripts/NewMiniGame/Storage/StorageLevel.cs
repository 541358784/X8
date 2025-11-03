/************************************************
 * Storage class : StorageLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageLevel : StorageBase
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
        
        // 已领取奖励
        [JsonProperty]
        bool claimed;
        [JsonIgnore]
        public bool Claimed
        {
            get
            {
                return claimed;
            }
            set
            {
                if(claimed != value)
                {
                    claimed = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否购买
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