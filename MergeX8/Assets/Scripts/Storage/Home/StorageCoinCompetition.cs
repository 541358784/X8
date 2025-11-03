/************************************************
 * Storage class : StorageCoinCompetition
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCoinCompetition : StorageBase
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
        
        // 支付组
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
        
        // 动画阶段
        [JsonProperty]
        int animIndex;
        [JsonIgnore]
        public int AnimIndex
        {
            get
            {
                return animIndex;
            }
            set
            {
                if(animIndex != value)
                {
                    animIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前处于那个阶段
        [JsonProperty]
        int curIndex;
        [JsonIgnore]
        public int CurIndex
        {
            get
            {
                return curIndex;
            }
            set
            {
                if(curIndex != value)
                {
                    curIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 获取的分数
        [JsonProperty]
        int totalScore;
        [JsonIgnore]
        public int TotalScore
        {
            get
            {
                return totalScore;
            }
            set
            {
                if(totalScore != value)
                {
                    totalScore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否显示活动开始界面
        [JsonProperty]
        bool isShowStartView;
        [JsonIgnore]
        public bool IsShowStartView
        {
            get
            {
                return isShowStartView;
            }
            set
            {
                if(isShowStartView != value)
                {
                    isShowStartView = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否显示活动结束界面
        [JsonProperty]
        bool isShowEndView;
        [JsonIgnore]
        public bool IsShowEndView
        {
            get
            {
                return isShowEndView;
            }
            set
            {
                if(isShowEndView != value)
                {
                    isShowEndView = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否手动结束
        [JsonProperty]
        bool isManualActivity;
        [JsonIgnore]
        public bool IsManualActivity
        {
            get
            {
                return isManualActivity;
            }
            set
            {
                if(isManualActivity != value)
                {
                    isManualActivity = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 奖励领取情况
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
        
        // 是否显示额外奖励动画 
        [JsonProperty]
        bool isPlayEntendAppear;
        [JsonIgnore]
        public bool IsPlayEntendAppear
        {
            get
            {
                return isPlayEntendAppear;
            }
            set
            {
                if(isPlayEntendAppear != value)
                {
                    isPlayEntendAppear = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 预热红点状态 
        [JsonProperty]
        bool isShowPreheat;
        [JsonIgnore]
        public bool IsShowPreheat
        {
            get
            {
                return isShowPreheat;
            }
            set
            {
                if(isShowPreheat != value)
                {
                    isShowPreheat = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
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
        
        // 计算未领取奖励的阶段
        [JsonProperty]
        StorageDictionary<int,bool> collectRewardsLevelList = new StorageDictionary<int,bool>();
        [JsonIgnore]
        public StorageDictionary<int,bool> CollectRewardsLevelList
        {
            get
            {
                return collectRewardsLevelList;
            }
        }
        // ---------------------------------//
        
        // 活动状态
        [JsonProperty]
        int activityStatus;
        [JsonIgnore]
        public int ActivityStatus
        {
            get
            {
                return activityStatus;
            }
            set
            {
                if(activityStatus != value)
                {
                    activityStatus = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}