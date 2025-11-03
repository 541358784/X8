/************************************************
 * Storage class : StorageEndlessEnergyGiftBag
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageEndlessEnergyGiftBag : StorageBase
    {
        
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
        
        // 进度
        [JsonProperty]
        int level;
        [JsonIgnore]
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if(level != value)
                {
                    level = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 记录日期
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
        
        // 当天是否购买体力
        [JsonProperty]
        bool buyEnergy;
        [JsonIgnore]
        public bool BuyEnergy
        {
            get
            {
                return buyEnergy;
            }
            set
            {
                if(buyEnergy != value)
                {
                    buyEnergy = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 连续未购买体力活跃天数
        [JsonProperty]
        int dayCount;
        [JsonIgnore]
        public int DayCount
        {
            get
            {
                return dayCount;
            }
            set
            {
                if(dayCount != value)
                {
                    dayCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}