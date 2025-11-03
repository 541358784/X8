/************************************************
 * Storage class : StorageEaster
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageEaster : StorageBase
    {
        
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
        
        // 结束时间
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