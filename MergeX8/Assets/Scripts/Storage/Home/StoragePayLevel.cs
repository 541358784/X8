/************************************************
 * Storage class : StoragePayLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StoragePayLevel : StorageBase
    {
        
        // 日期
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
        
        // 消费档次
        [JsonProperty]
        int payLevel;
        [JsonIgnore]
        public int PayLevel
        {
            get
            {
                return payLevel;
            }
            set
            {
                if(payLevel != value)
                {
                    payLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 连续付费天数
        [JsonProperty]
        int continuePayDays;
        [JsonIgnore]
        public int ContinuePayDays
        {
            get
            {
                return continuePayDays;
            }
            set
            {
                if(continuePayDays != value)
                {
                    continuePayDays = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 连续未付费天数
        [JsonProperty]
        int continueUnPayDays;
        [JsonIgnore]
        public int ContinueUnPayDays
        {
            get
            {
                return continueUnPayDays;
            }
            set
            {
                if(continueUnPayDays != value)
                {
                    continueUnPayDays = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当日付费金额
        [JsonProperty]
        float curDayPayValue;
        [JsonIgnore]
        public float CurDayPayValue
        {
            get
            {
                return curDayPayValue;
            }
            set
            {
                if(curDayPayValue != value)
                {
                    curDayPayValue = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否付费过
        [JsonProperty]
        bool hasPay;
        [JsonIgnore]
        public bool HasPay
        {
            get
            {
                return hasPay;
            }
            set
            {
                if(hasPay != value)
                {
                    hasPay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最小付费等级
        [JsonProperty]
        int minLevel;
        [JsonIgnore]
        public int MinLevel
        {
            get
            {
                return minLevel;
            }
            set
            {
                if(minLevel != value)
                {
                    minLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 每日付费记录
        [JsonProperty]
        StorageDictionary<int,float> dayPayDic = new StorageDictionary<int,float>();
        [JsonIgnore]
        public StorageDictionary<int,float> DayPayDic
        {
            get
            {
                return dayPayDic;
            }
        }
        // ---------------------------------//
        
        // 新分层生效时间
        [JsonProperty]
        int newPayLevelStartDay;
        [JsonIgnore]
        public int NewPayLevelStartDay
        {
            get
            {
                return newPayLevelStartDay;
            }
            set
            {
                if(newPayLevelStartDay != value)
                {
                    newPayLevelStartDay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 降档CD
        [JsonProperty]
        int downLevelEnableDay;
        [JsonIgnore]
        public int DownLevelEnableDay
        {
            get
            {
                return downLevelEnableDay;
            }
            set
            {
                if(downLevelEnableDay != value)
                {
                    downLevelEnableDay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 登录日付费记录
        [JsonProperty]
        StorageList<float> dayPayList = new StorageList<float>();
        [JsonIgnore]
        public StorageList<float> DayPayList
        {
            get
            {
                return dayPayList;
            }
        }
        // ---------------------------------//
        
        // 无过滤每日付费记录
        [JsonProperty]
        StorageDictionary<int,float> allDayPayDic = new StorageDictionary<int,float>();
        [JsonIgnore]
        public StorageDictionary<int,float> AllDayPayDic
        {
            get
            {
                return allDayPayDic;
            }
        }
        // ---------------------------------//
        
        // 2日内付费天平均值
        [JsonProperty]
        float avgValue2;
        [JsonIgnore]
        public float AvgValue2
        {
            get
            {
                return avgValue2;
            }
            set
            {
                if(avgValue2 != value)
                {
                    avgValue2 = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 7日内付费天平均值
        [JsonProperty]
        float avgValue7;
        [JsonIgnore]
        public float AvgValue7
        {
            get
            {
                return avgValue7;
            }
            set
            {
                if(avgValue7 != value)
                {
                    avgValue7 = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 30日内付费天平均值
        [JsonProperty]
        float avgValue30;
        [JsonIgnore]
        public float AvgValue30
        {
            get
            {
                return avgValue30;
            }
            set
            {
                if(avgValue30 != value)
                {
                    avgValue30 = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}