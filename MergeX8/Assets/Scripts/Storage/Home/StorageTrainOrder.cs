/************************************************
 * Storage class : StorageTrainOrder
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTrainOrder : StorageBase
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
        
        // 分组ID
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
        
        // 进度
        [JsonProperty]
        int progress;
        [JsonIgnore]
        public int Progress
        {
            get
            {
                return progress;
            }
            set
            {
                if(progress != value)
                {
                    progress = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前任务组下标
        [JsonProperty]
        int curOrderGroupIndex;
        [JsonIgnore]
        public int CurOrderGroupIndex
        {
            get
            {
                return curOrderGroupIndex;
            }
            set
            {
                if(curOrderGroupIndex != value)
                {
                    curOrderGroupIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前关卡下标
        [JsonProperty]
        int curLevelIndex;
        [JsonIgnore]
        public int CurLevelIndex
        {
            get
            {
                return curLevelIndex;
            }
            set
            {
                if(curLevelIndex != value)
                {
                    curLevelIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已完成关卡列表
        [JsonProperty]
        StorageList<int> completeLevelList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CompleteLevelList
        {
            get
            {
                return completeLevelList;
            }
        }
        // ---------------------------------//
        
        // 当前任务组所需物品状态
        [JsonProperty]
        StorageList<StorageTrainOrderItemState> curOrderItemState = new StorageList<StorageTrainOrderItemState>();
        [JsonIgnore]
        public StorageList<StorageTrainOrderItemState> CurOrderItemState
        {
            get
            {
                return curOrderItemState;
            }
        }
        // ---------------------------------//
        
        // 当前任务组任务状态0未完成1完成
        [JsonProperty]
        StorageDictionary<int,int> curOrderState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> CurOrderState
        {
            get
            {
                return curOrderState;
            }
        }
        // ---------------------------------//
        
        // 是否初始化了关卡的建筑
        [JsonProperty]
        bool isInitLevelBuild;
        [JsonIgnore]
        public bool IsInitLevelBuild
        {
            get
            {
                return isInitLevelBuild;
            }
            set
            {
                if(isInitLevelBuild != value)
                {
                    isInitLevelBuild = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前任务组0未完成1完成
        [JsonProperty]
        StorageDictionary<int,int> curOrderGroupState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> CurOrderGroupState
        {
            get
            {
                return curOrderGroupState;
            }
        }
        // ---------------------------------//
        
        // 是否完成
        [JsonProperty]
        bool isDone;
        [JsonIgnore]
        public bool IsDone
        {
            get
            {
                return isDone;
            }
            set
            {
                if(isDone != value)
                {
                    isDone = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 延迟刷新订单数据
        [JsonProperty]
        bool needDelayRefreshOrder;
        [JsonIgnore]
        public bool NeedDelayRefreshOrder
        {
            get
            {
                return needDelayRefreshOrder;
            }
            set
            {
                if(needDelayRefreshOrder != value)
                {
                    needDelayRefreshOrder = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否弹了开始弹窗
        [JsonProperty]
        bool isPopupStart;
        [JsonIgnore]
        public bool IsPopupStart
        {
            get
            {
                return isPopupStart;
            }
            set
            {
                if(isPopupStart != value)
                {
                    isPopupStart = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}