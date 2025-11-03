/************************************************
 * Storage class : StorageSummerWatermelon
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageSummerWatermelon : StorageBase
    {
        
        // 是否开启
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
        
        // 最高解锁的活动ICON等级
        [JsonProperty]
        int maxUnlockLevel;
        [JsonIgnore]
        public int MaxUnlockLevel
        {
            get
            {
                return maxUnlockLevel;
            }
            set
            {
                if(maxUnlockLevel != value)
                {
                    maxUnlockLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 未放置的奖励棋子(暂时不使用)
        [JsonProperty]
        StorageList<int> unSetRewards = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> UnSetRewards
        {
            get
            {
                return unSetRewards;
            }
        }
        // ---------------------------------//
        
        // 未领取的奖励
        [JsonProperty]
        StorageDictionary<int,int> unCollectRewards = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> UnCollectRewards
        {
            get
            {
                return unCollectRewards;
            }
        }
        // ---------------------------------//
        
        // 未放置的活动棋子
        [JsonProperty]
        StorageList<int> unSetItems = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> UnSetItems
        {
            get
            {
                return unSetItems;
            }
        }
        // ---------------------------------//
        
        // 活动开始时间
        [JsonProperty]
        long startActivityTime;
        [JsonIgnore]
        public long StartActivityTime
        {
            get
            {
                return startActivityTime;
            }
            set
            {
                if(startActivityTime != value)
                {
                    startActivityTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动结束时间
        [JsonProperty]
        long activityEndTime;
        [JsonIgnore]
        public long ActivityEndTime
        {
            get
            {
                return activityEndTime;
            }
            set
            {
                if(activityEndTime != value)
                {
                    activityEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已购买的礼包SHOPID
        [JsonProperty]
        StorageDictionary<int,int> buyPackageShopIdDictionary = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> BuyPackageShopIdDictionary
        {
            get
            {
                return buyPackageShopIdDictionary;
            }
        }
        // ---------------------------------//
        
        // 今日产出的次数
        [JsonProperty]
        int dayProductCount;
        [JsonIgnore]
        public int DayProductCount
        {
            get
            {
                return dayProductCount;
            }
            set
            {
                if(dayProductCount != value)
                {
                    dayProductCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 今日日期
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
        
        // 礼包是否允许购买
        [JsonProperty]
        StorageDictionary<int,bool> packageEnableStateDictionary = new StorageDictionary<int,bool>();
        [JsonIgnore]
        public StorageDictionary<int,bool> PackageEnableStateDictionary
        {
            get
            {
                return packageEnableStateDictionary;
            }
        }
        // ---------------------------------//
        
    }
}