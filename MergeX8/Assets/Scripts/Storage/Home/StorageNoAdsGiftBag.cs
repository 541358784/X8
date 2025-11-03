/************************************************
 * Storage class : StorageNoAdsGiftBag
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageNoAdsGiftBag : StorageBase
    {
        
        // 显示次数
        [JsonProperty]
        int showTime;
        [JsonIgnore]
        public int ShowTime
        {
            get
            {
                return showTime;
            }
            set
            {
                if(showTime != value)
                {
                    showTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 开始时间戳
        [JsonProperty]
        long startTime;
        [JsonIgnore]
        public long StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                if(startTime != value)
                {
                    startTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 结束时间戳
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
        
        // 是否付费
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
        
        // 是否主动弹出过
        [JsonProperty]
        bool hasShow;
        [JsonIgnore]
        public bool HasShow
        {
            get
            {
                return hasShow;
            }
            set
            {
                if(hasShow != value)
                {
                    hasShow = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}