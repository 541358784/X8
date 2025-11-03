/************************************************
 * Storage class : StorageDailyRank
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDailyRank : StorageBase
    {
        
        // 是否完成初始化
        [JsonProperty]
        bool isInit;
        [JsonIgnore]
        public bool IsInit
        {
            get
            {
                return isInit;
            }
            set
            {
                if(isInit != value)
                {
                    isInit = value;
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
        
        // 激活活动时间
        [JsonProperty]
        ulong activeEndTime;
        [JsonIgnore]
        public ulong ActiveEndTime
        {
            get
            {
                return activeEndTime;
            }
            set
            {
                if(activeEndTime != value)
                {
                    activeEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 刷新时间
        [JsonProperty]
        ulong updateTime;
        [JsonIgnore]
        public ulong UpdateTime
        {
            get
            {
                return updateTime;
            }
            set
            {
                if(updateTime != value)
                {
                    updateTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 机器人
        [JsonProperty]
        StorageList<StorageDailyRankRobot> robots = new StorageList<StorageDailyRankRobot>();
        [JsonIgnore]
        public StorageList<StorageDailyRankRobot> Robots
        {
            get
            {
                return robots;
            }
        }
        // ---------------------------------//
        
        // 当前获得的分数
        [JsonProperty]
        int curScore;
        [JsonIgnore]
        public int CurScore
        {
            get
            {
                return curScore;
            }
            set
            {
                if(curScore != value)
                {
                    curScore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 奖励
        [JsonProperty]
        StorageList<StorageDailyRankRewardGroup> rewards = new StorageList<StorageDailyRankRewardGroup>();
        [JsonIgnore]
        public StorageList<StorageDailyRankRewardGroup> Rewards
        {
            get
            {
                return rewards;
            }
        }
        // ---------------------------------//
        
    }
}