/************************************************
 * Storage class : StorageDogHope
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDogHope : StorageBase
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
        
        // 属性名
        [JsonProperty]
        StorageDogHopeLeaderBoard leaderBoardStorage = new StorageDogHopeLeaderBoard();
        [JsonIgnore]
        public StorageDogHopeLeaderBoard LeaderBoardStorage
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