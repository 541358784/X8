/************************************************
 * Storage class : StorageDays
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage.Decoration
{
    [System.Serializable]
    public class StorageDays : StorageBase
    {
        
        // 是否修正DAYS 数据
        [JsonProperty]
        bool isFixDays;
        [JsonIgnore]
        public bool IsFixDays
        {
            get
            {
                return isFixDays;
            }
            set
            {
                if(isFixDays != value)
                {
                    isFixDays = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否初始化DAYS 数据
        [JsonProperty]
        bool initDays;
        [JsonIgnore]
        public bool InitDays
        {
            get
            {
                return initDays;
            }
            set
            {
                if(initDays != value)
                {
                    initDays = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前天数
        [JsonProperty]
        int dayNum;
        [JsonIgnore]
        public int DayNum
        {
            get
            {
                return dayNum;
            }
            set
            {
                if(dayNum != value)
                {
                    dayNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前进度
        [JsonProperty]
        int dayStep;
        [JsonIgnore]
        public int DayStep
        {
            get
            {
                return dayStep;
            }
            set
            {
                if(dayStep != value)
                {
                    dayStep = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 总完成的挂点数量
        [JsonProperty]
        int totalNodeNum;
        [JsonIgnore]
        public int TotalNodeNum
        {
            get
            {
                return totalNodeNum;
            }
            set
            {
                if(totalNodeNum != value)
                {
                    totalNodeNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 领取奖励状态
        [JsonProperty]
        StorageDictionary<string,bool> getRewardState = new StorageDictionary<string,bool>();
        [JsonIgnore]
        public StorageDictionary<string,bool> GetRewardState
        {
            get
            {
                return getRewardState;
            }
        }
        // ---------------------------------//
        
    }
}