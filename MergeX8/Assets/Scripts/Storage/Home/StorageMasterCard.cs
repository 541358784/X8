/************************************************
 * Storage class : StorageMasterCard
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMasterCard : StorageBase
    {
        
        // MASTERCARDID
        [JsonProperty]
        int masterCardId;
        [JsonIgnore]
        public int MasterCardId
        {
            get
            {
                return masterCardId;
            }
            set
            {
                if(masterCardId != value)
                {
                    masterCardId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 购买结束时间
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
        
        // 增加背包格子数量
        [JsonProperty]
        int addBagNum;
        [JsonIgnore]
        public int AddBagNum
        {
            get
            {
                return addBagNum;
            }
            set
            {
                if(addBagNum != value)
                {
                    addBagNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 剩余未领奖励数量
        [JsonProperty]
        int leftRewardCount;
        [JsonIgnore]
        public int LeftRewardCount
        {
            get
            {
                return leftRewardCount;
            }
            set
            {
                if(leftRewardCount != value)
                {
                    leftRewardCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 领取奖励时间
        [JsonProperty]
        long getRewardTime;
        [JsonIgnore]
        public long GetRewardTime
        {
            get
            {
                return getRewardTime;
            }
            set
            {
                if(getRewardTime != value)
                {
                    getRewardTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 剩余钻石翻倍券数量
        [JsonProperty]
        int leftPayDoubleCount;
        [JsonIgnore]
        public int LeftPayDoubleCount
        {
            get
            {
                return leftPayDoubleCount;
            }
            set
            {
                if(leftPayDoubleCount != value)
                {
                    leftPayDoubleCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否充值7天
        [JsonProperty]
        bool isRecharge7;
        [JsonIgnore]
        public bool IsRecharge7
        {
            get
            {
                return isRecharge7;
            }
            set
            {
                if(isRecharge7 != value)
                {
                    isRecharge7 = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否充值30天
        [JsonProperty]
        bool isRecharge30;
        [JsonIgnore]
        public bool IsRecharge30
        {
            get
            {
                return isRecharge30;
            }
            set
            {
                if(isRecharge30 != value)
                {
                    isRecharge30 = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 到期是否弹出续费
        [JsonProperty]
        bool isPopupRenewal;
        [JsonIgnore]
        public bool IsPopupRenewal
        {
            get
            {
                return isPopupRenewal;
            }
            set
            {
                if(isPopupRenewal != value)
                {
                    isPopupRenewal = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}