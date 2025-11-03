/************************************************
 * Storage class : StorageDogPlayExtraReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDogPlayExtraReward : StorageBase
    {
        
        // 活动ID
        [JsonProperty]
        string activityId = "";
        [JsonIgnore]
        public string ActivityId
        {
            get
            {
                return activityId;
            }
            set
            {
                if(activityId != value)
                {
                    activityId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否弹出过开始弹窗
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
        
        // 暂存奖励
        [JsonProperty]
        StorageList<StorageResData> rewards = new StorageList<StorageResData>();
        [JsonIgnore]
        public StorageList<StorageResData> Rewards
        {
            get
            {
                return rewards;
            }
        }
        // ---------------------------------//
        
        // 狗任务ID
        [JsonProperty]
        int dogPlayOrderId;
        [JsonIgnore]
        public int DogPlayOrderId
        {
            get
            {
                return dogPlayOrderId;
            }
            set
            {
                if(dogPlayOrderId != value)
                {
                    dogPlayOrderId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}