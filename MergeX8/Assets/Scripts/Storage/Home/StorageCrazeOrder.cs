/************************************************
 * Storage class : StorageCrazeOrder
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCrazeOrder : StorageBase
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
        
        // 是否加入
        [JsonProperty]
        bool isJoin;
        [JsonIgnore]
        public bool IsJoin
        {
            get
            {
                return isJoin;
            }
            set
            {
                if(isJoin != value)
                {
                    isJoin = value;
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
        
        // 参加活动开始时间
        [JsonProperty]
        long joinStartTime;
        [JsonIgnore]
        public long JoinStartTime
        {
            get
            {
                return joinStartTime;
            }
            set
            {
                if(joinStartTime != value)
                {
                    joinStartTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 参加活动结束时间
        [JsonProperty]
        long joinEndTime;
        [JsonIgnore]
        public long JoinEndTime
        {
            get
            {
                return joinEndTime;
            }
            set
            {
                if(joinEndTime != value)
                {
                    joinEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 动画阶段
        [JsonProperty]
        int animStage;
        [JsonIgnore]
        public int AnimStage
        {
            get
            {
                return animStage;
            }
            set
            {
                if(animStage != value)
                {
                    animStage = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前阶段
        [JsonProperty]
        int stage;
        [JsonIgnore]
        public int Stage
        {
            get
            {
                return stage;
            }
            set
            {
                if(stage != value)
                {
                    stage = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成个数
        [JsonProperty]
        int completeNum;
        [JsonIgnore]
        public int CompleteNum
        {
            get
            {
                return completeNum;
            }
            set
            {
                if(completeNum != value)
                {
                    completeNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 组ID
        [JsonProperty]
        int groupId;
        [JsonIgnore]
        public int GroupId
        {
            get
            {
                return groupId;
            }
            set
            {
                if(groupId != value)
                {
                    groupId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
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