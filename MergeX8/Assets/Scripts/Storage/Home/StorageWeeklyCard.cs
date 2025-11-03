/************************************************
 * Storage class : StorageWeeklyCard
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageWeeklyCard : StorageBase
    {
        
        // 购买状态
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
        
        // 购买时间
        [JsonProperty]
        long buyTime;
        [JsonIgnore]
        public long BuyTime
        {
            get
            {
                return buyTime;
            }
            set
            {
                if(buyTime != value)
                {
                    buyTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次领取时间
        [JsonProperty]
        long lastClaimTime;
        [JsonIgnore]
        public long LastClaimTime
        {
            get
            {
                return lastClaimTime;
            }
            set
            {
                if(lastClaimTime != value)
                {
                    lastClaimTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 领取次数
        [JsonProperty]
        int totalClaimCount;
        [JsonIgnore]
        public int TotalClaimCount
        {
            get
            {
                return totalClaimCount;
            }
            set
            {
                if(totalClaimCount != value)
                {
                    totalClaimCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 全部完成时间
        [JsonProperty]
        long finishTime;
        [JsonIgnore]
        public long FinishTime
        {
            get
            {
                return finishTime;
            }
            set
            {
                if(finishTime != value)
                {
                    finishTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        bool isNewBuy;
        [JsonIgnore]
        public bool IsNewBuy
        {
            get
            {
                return isNewBuy;
            }
            set
            {
                if(isNewBuy != value)
                {
                    isNewBuy = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}