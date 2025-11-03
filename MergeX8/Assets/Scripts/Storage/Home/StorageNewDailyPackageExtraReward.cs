/************************************************
 * Storage class : StorageNewDailyPackageExtraReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageNewDailyPackageExtraReward : StorageBase
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
        
        // 购买次数
        [JsonProperty]
        int buyTimes;
        [JsonIgnore]
        public int BuyTimes
        {
            get
            {
                return buyTimes;
            }
            set
            {
                if(buyTimes != value)
                {
                    buyTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 皮肤
        [JsonProperty]
        string skinName = "";
        [JsonIgnore]
        public string SkinName
        {
            get
            {
                return skinName;
            }
            set
            {
                if(skinName != value)
                {
                    skinName = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}