/************************************************
 * Storage class : StorageCustomTaskData
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCustomTaskData : StorageBase
    {
        
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
        
        // 任务分数
        [JsonProperty]
        int score;
        [JsonIgnore]
        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                if(score != value)
                {
                    score = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 任务领取情况
        [JsonProperty]
        StorageDictionary<int,bool> reward = new StorageDictionary<int,bool>();
        [JsonIgnore]
        public StorageDictionary<int,bool> Reward
        {
            get
            {
                return reward;
            }
        }
        // ---------------------------------//
        
        // 任务箱子领取情况
        [JsonProperty]
        StorageDictionary<int,bool> boxReward = new StorageDictionary<int,bool>();
        [JsonIgnore]
        public StorageDictionary<int,bool> BoxReward
        {
            get
            {
                return boxReward;
            }
        }
        // ---------------------------------//
        
    }
}