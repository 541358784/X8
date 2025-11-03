/************************************************
 * Storage class : StorageDailyBonus
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDailyBonus : StorageBase
    {
        
        // 上次登录的天数
        [JsonProperty]
        int lastLoginDay;
        [JsonIgnore]
        public int LastLoginDay
        {
            get
            {
                return lastLoginDay;
            }
            set
            {
                if(lastLoginDay != value)
                {
                    lastLoginDay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 连续登录天数
        [JsonProperty]
        int consecutiveLoginDays;
        [JsonIgnore]
        public int ConsecutiveLoginDays
        {
            get
            {
                return consecutiveLoginDays;
            }
            set
            {
                if(consecutiveLoginDays != value)
                {
                    consecutiveLoginDays = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次领奖的天数
        [JsonProperty]
        int lastClaimDailyBonusDay;
        [JsonIgnore]
        public int LastClaimDailyBonusDay
        {
            get
            {
                return lastClaimDailyBonusDay;
            }
            set
            {
                if(lastClaimDailyBonusDay != value)
                {
                    lastClaimDailyBonusDay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 总天数
        [JsonProperty]
        int totalClaimDay;
        [JsonIgnore]
        public int TotalClaimDay
        {
            get
            {
                return totalClaimDay;
            }
            set
            {
                if(totalClaimDay != value)
                {
                    totalClaimDay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次领取宝箱天数
        [JsonProperty]
        int lasCliamChestDay;
        [JsonIgnore]
        public int LasCliamChestDay
        {
            get
            {
                return lasCliamChestDay;
            }
            set
            {
                if(lasCliamChestDay != value)
                {
                    lasCliamChestDay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}