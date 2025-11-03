/************************************************
 * Storage class : StorageNewIceBreakGiftBag
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageNewIceBreakGiftBag : StorageBase
    {
        
        // 礼包ID
        [JsonProperty]
        int giftBagId;
        [JsonIgnore]
        public int GiftBagId
        {
            get
            {
                return giftBagId;
            }
            set
            {
                if(giftBagId != value)
                {
                    giftBagId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 礼包结束时间
        [JsonProperty]
        long endTime;
        [JsonIgnore]
        public long EndTime
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
        
        // 是否结束
        [JsonProperty]
        bool isFinish;
        [JsonIgnore]
        public bool IsFinish
        {
            get
            {
                return isFinish;
            }
            set
            {
                if(isFinish != value)
                {
                    isFinish = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否支付
        [JsonProperty]
        bool isPay;
        [JsonIgnore]
        public bool IsPay
        {
            get
            {
                return isPay;
            }
            set
            {
                if(isPay != value)
                {
                    isPay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}