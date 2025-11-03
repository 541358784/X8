/************************************************
 * Storage class : StorageGiftBagSend4
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageGiftBagSend4 : StorageBase
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
        
        // 付费分层组
        [JsonProperty]
        int payLevelGroup;
        [JsonIgnore]
        public int PayLevelGroup
        {
            get
            {
                return payLevelGroup;
            }
            set
            {
                if(payLevelGroup != value)
                {
                    payLevelGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否购买
        [JsonProperty]
        bool buyState;
        [JsonIgnore]
        public bool BuyState
        {
            get
            {
                return buyState;
            }
            set
            {
                if(buyState != value)
                {
                    buyState = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 领取状态
        [JsonProperty]
        StorageList<int> collectState = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CollectState
        {
            get
            {
                return collectState;
            }
        }
        // ---------------------------------//
        
        // 观看RV次数
        [JsonProperty]
        int rvTimes;
        [JsonIgnore]
        public int RvTimes
        {
            get
            {
                return rvTimes;
            }
            set
            {
                if(rvTimes != value)
                {
                    rvTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}