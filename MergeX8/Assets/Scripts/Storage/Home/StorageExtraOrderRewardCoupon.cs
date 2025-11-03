/************************************************
 * Storage class : StorageExtraOrderRewardCoupon
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageExtraOrderRewardCoupon : StorageBase
    {
        
        // 免费券队列
        [JsonProperty]
        StorageList<StorageExtraOrderRewardCouponItem> freeCouponList = new StorageList<StorageExtraOrderRewardCouponItem>();
        [JsonIgnore]
        public StorageList<StorageExtraOrderRewardCouponItem> FreeCouponList
        {
            get
            {
                return freeCouponList;
            }
        }
        // ---------------------------------//
        
        // 付费券队列
        [JsonProperty]
        StorageList<StorageExtraOrderRewardCouponItem> payCouponList = new StorageList<StorageExtraOrderRewardCouponItem>();
        [JsonIgnore]
        public StorageList<StorageExtraOrderRewardCouponItem> PayCouponList
        {
            get
            {
                return payCouponList;
            }
        }
        // ---------------------------------//
        
        // 已生效券列表
        [JsonProperty]
        StorageList<StorageExtraOrderRewardCouponItem> curCouponList = new StorageList<StorageExtraOrderRewardCouponItem>();
        [JsonIgnore]
        public StorageList<StorageExtraOrderRewardCouponItem> CurCouponList
        {
            get
            {
                return curCouponList;
            }
        }
        // ---------------------------------//
        
    }
}