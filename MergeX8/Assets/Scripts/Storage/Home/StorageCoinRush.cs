/************************************************
 * Storage class : StorageCoinRush
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCoinRush : StorageBase
    {
        
        // 是否预开启过
        [JsonProperty]
        bool isPreOpen;
        [JsonIgnore]
        public bool IsPreOpen
        {
            get
            {
                return isPreOpen;
            }
            set
            {
                if(isPreOpen != value)
                {
                    isPreOpen = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 预开启时间
        [JsonProperty]
        long preOpenTime;
        [JsonIgnore]
        public long PreOpenTime
        {
            get
            {
                return preOpenTime;
            }
            set
            {
                if(preOpenTime != value)
                {
                    preOpenTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
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
        
        // 目标收集状态
        [JsonProperty]
        StorageDictionary<int,int> targetCollectState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> TargetCollectState
        {
            get
            {
                return targetCollectState;
            }
        }
        // ---------------------------------//
        
        // 目标消耗状态
        [JsonProperty]
        StorageDictionary<int,int> targetConsumeState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> TargetConsumeState
        {
            get
            {
                return targetConsumeState;
            }
        }
        // ---------------------------------//
        
        // 是否领取最终奖励
        [JsonProperty]
        bool isCollectFinalReward;
        [JsonIgnore]
        public bool IsCollectFinalReward
        {
            get
            {
                return isCollectFinalReward;
            }
            set
            {
                if(isCollectFinalReward != value)
                {
                    isCollectFinalReward = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已经领奖的等级
        [JsonProperty]
        StorageList<int> alreadyCollectLevels = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> AlreadyCollectLevels
        {
            get
            {
                return alreadyCollectLevels;
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
        
        // 特殊目标状态
        [JsonProperty]
        StorageDictionary<int,int> specialTargetState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> SpecialTargetState
        {
            get
            {
                return specialTargetState;
            }
        }
        // ---------------------------------//
        
        // 皮肤名
        [JsonProperty]
        string skinName = "";
        [JsonIgnore]
        public string SkinName
        {
            get
            {
                return skinName;
            }
            set
            {
                if(skinName != value)
                {
                    skinName = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
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
        
        // 活动资源的文件名
        [JsonProperty]
        StorageList<string> activityResList = new StorageList<string>();
        [JsonIgnore]
        public StorageList<string> ActivityResList
        {
            get
            {
                return activityResList;
            }
        }
        // ---------------------------------//
        
        // 分层组
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
        
    }
}