/************************************************
 * Storage class : StorageClimbTree
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageClimbTree : StorageBase
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
        
        // 上次获取的分数
        [JsonProperty]
        int lastScore;
        [JsonIgnore]
        public int LastScore
        {
            get
            {
                return lastScore;
            }
            set
            {
                if(lastScore != value)
                {
                    lastScore = value;
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
        
        // 是否显示过活动开始弹窗
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
        
        // 未领取的等级
        [JsonProperty]
        StorageList<int> unCollectLevels = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> UnCollectLevels
        {
            get
            {
                return unCollectLevels;
            }
        }
        // ---------------------------------//
        
        // 通关后的排行榜存档
        [JsonProperty]
        StorageClimbTreeLeaderBoard leaderBoardStorage = new StorageClimbTreeLeaderBoard();
        [JsonIgnore]
        public StorageClimbTreeLeaderBoard LeaderBoardStorage
        {
            get
            {
                return leaderBoardStorage;
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
        
        // 开始时间戳(如果活动开启则进行更新，否则用本地存的)
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
        
        // 结束时间戳(同STARTTIME)
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
        
    }
}