/************************************************
 * Storage class : StorageScrewDailyPackage
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageScrewDailyPackage : StorageBase
    {
        
        // 刷新日期
        [JsonProperty]
        int dayId;
        [JsonIgnore]
        public int DayId
        {
            get
            {
                return dayId;
            }
            set
            {
                if(dayId != value)
                {
                    dayId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 购买状态
        [JsonProperty]
        StorageDictionary<int,int> buyState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> BuyState
        {
            get
            {
                return buyState;
            }
        }
        // ---------------------------------//
        
    }
}