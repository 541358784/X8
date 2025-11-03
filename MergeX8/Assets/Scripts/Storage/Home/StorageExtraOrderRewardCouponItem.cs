/************************************************
 * Storage class : StorageExtraOrderRewardCouponItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageExtraOrderRewardCouponItem : StorageBase
    {
        
        // 券ID
        [JsonProperty]
        int couponId;
        [JsonIgnore]
        public int CouponId
        {
            get
            {
                return couponId;
            }
            set
            {
                if(couponId != value)
                {
                    couponId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 获取的日期
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
        
        // 券开始时间
        [JsonProperty]
        ulong startTime;
        [JsonIgnore]
        public ulong StartTime
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
        
        // 券结束时间
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
        
        // 是否已生效
        [JsonProperty]
        bool isStart;
        [JsonIgnore]
        public bool IsStart
        {
            get
            {
                return isStart;
            }
            set
            {
                if(isStart != value)
                {
                    isStart = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}