/************************************************
 * Storage class : StoragePayRebate
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StoragePayRebate : StorageBase
    {
        
        // 是否领取
        [JsonProperty]
        bool isCliam;
        [JsonIgnore]
        public bool IsCliam
        {
            get
            {
                return isCliam;
            }
            set
            {
                if(isCliam != value)
                {
                    isCliam = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否自动弹出
        [JsonProperty]
        bool isAutoPop;
        [JsonIgnore]
        public bool IsAutoPop
        {
            get
            {
                return isAutoPop;
            }
            set
            {
                if(isAutoPop != value)
                {
                    isAutoPop = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动期间是否充值
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
        
        // 
        [JsonProperty]
        ulong endTime;
        [JsonIgnore]
        public ulong EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                if(endTime != value)
                {
                    endTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}