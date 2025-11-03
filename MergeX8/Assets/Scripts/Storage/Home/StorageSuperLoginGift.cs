/************************************************
 * Storage class : StorageSuperLoginGift
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageSuperLoginGift : StorageBase
    {
        
        // 上次领奖时间
        [JsonProperty]
        ulong lastGetRewardTime;
        [JsonIgnore]
        public ulong LastGetRewardTime
        {
            get
            {
                return lastGetRewardTime;
            }
            set
            {
                if(lastGetRewardTime != value)
                {
                    lastGetRewardTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 领奖记录
        [JsonProperty]
        StorageList<string> getRewardHistory = new StorageList<string>();
        [JsonIgnore]
        public StorageList<string> GetRewardHistory
        {
            get
            {
                return getRewardHistory;
            }
        }
        // ---------------------------------//
        
        // 当前选中的礼包IDS
        [JsonProperty]
        StorageList<int> currentSeletBundleIds = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CurrentSeletBundleIds
        {
            get
            {
                return currentSeletBundleIds;
            }
        }
        // ---------------------------------//
        
        // 当前摇中倍率，0表示没有
        [JsonProperty]
        int currentSpinTimes;
        [JsonIgnore]
        public int CurrentSpinTimes
        {
            get
            {
                return currentSpinTimes;
            }
            set
            {
                if(currentSpinTimes != value)
                {
                    currentSpinTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 0表示没有
        [JsonProperty]
        int selectGroup;
        [JsonIgnore]
        public int SelectGroup
        {
            get
            {
                return selectGroup;
            }
            set
            {
                if(selectGroup != value)
                {
                    selectGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动是否结束
        [JsonProperty]
        bool isFinished;
        [JsonIgnore]
        public bool IsFinished
        {
            get
            {
                return isFinished;
            }
            set
            {
                if(isFinished != value)
                {
                    isFinished = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}