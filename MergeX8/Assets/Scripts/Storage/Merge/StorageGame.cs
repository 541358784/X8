/************************************************
 * Storage class : StorageGame
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageGame : StorageBase
    {
         
        // 
        [JsonProperty]
        StorageDictionary<int,StorageMergeBoard> mergeBoards = new StorageDictionary<int,StorageMergeBoard>();
        [JsonIgnore]
        public StorageDictionary<int,StorageMergeBoard> MergeBoards
        {
            get
            {
                return mergeBoards;
            }
        }
        // ---------------------------------//
         
        // 
        [JsonProperty]
        StorageDictionary<int,StorageMergeDailyTask> mergeDailyTask = new StorageDictionary<int,StorageMergeDailyTask>();
        [JsonIgnore]
        public StorageDictionary<int,StorageMergeDailyTask> MergeDailyTask
        {
            get
            {
                return mergeDailyTask;
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        StorageMergeUnlockItem mergeUnlockItem = new StorageMergeUnlockItem();
        [JsonIgnore]
        public StorageMergeUnlockItem MergeUnlockItem
        {
            get
            {
                return mergeUnlockItem;
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        StorageTaskGroup taskGroups = new StorageTaskGroup();
        [JsonIgnore]
        public StorageTaskGroup TaskGroups
        {
            get
            {
                return taskGroups;
            }
        }
        // ---------------------------------//
        
        // 每日任务生成时间
        [JsonProperty]
        long dailyTaskRefreshTime;
        [JsonIgnore]
        public long DailyTaskRefreshTime
        {
            get
            {
                return dailyTaskRefreshTime;
            }
            set
            {
                if(dailyTaskRefreshTime != value)
                {
                    dailyTaskRefreshTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 今日每日任务是否已经完成
        [JsonProperty]
        bool dailyTaskIsAllFinish;
        [JsonIgnore]
        public bool DailyTaskIsAllFinish
        {
            get
            {
                return dailyTaskIsAllFinish;
            }
            set
            {
                if(dailyTaskIsAllFinish != value)
                {
                    dailyTaskIsAllFinish = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 神秘礼物完成几个广告展示  广告用
        [JsonProperty]
        int mysteryGiftShowLimitCount;
        [JsonIgnore]
        public int MysteryGiftShowLimitCount
        {
            get
            {
                return mysteryGiftShowLimitCount;
            }
            set
            {
                if(mysteryGiftShowLimitCount != value)
                {
                    mysteryGiftShowLimitCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 神秘礼物完成任务次数 广告用
        [JsonProperty]
        int mysteryGiftCompTaskCount;
        [JsonIgnore]
        public int MysteryGiftCompTaskCount
        {
            get
            {
                return mysteryGiftCompTaskCount;
            }
            set
            {
                if(mysteryGiftCompTaskCount != value)
                {
                    mysteryGiftCompTaskCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 神秘礼物显示次数
        [JsonProperty]
        int mysteryGiftShowCount;
        [JsonIgnore]
        public int MysteryGiftShowCount
        {
            get
            {
                return mysteryGiftShowCount;
            }
            set
            {
                if(mysteryGiftShowCount != value)
                {
                    mysteryGiftShowCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 缓存SHOPID
        [JsonProperty]
        int mySteryGiftShopID;
        [JsonIgnore]
        public int MySteryGiftShopID
        {
            get
            {
                return mySteryGiftShopID;
            }
            set
            {
                if(mySteryGiftShopID != value)
                {
                    mySteryGiftShopID = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 支付显示次数
        [JsonProperty]
        int mySteryPayShowCount;
        [JsonIgnore]
        public int MySteryPayShowCount
        {
            get
            {
                return mySteryPayShowCount;
            }
            set
            {
                if(mySteryPayShowCount != value)
                {
                    mySteryPayShowCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 支付次数
        [JsonProperty]
        int mySteryPayCount;
        [JsonIgnore]
        public int MySteryPayCount
        {
            get
            {
                return mySteryPayCount;
            }
            set
            {
                if(mySteryPayCount != value)
                {
                    mySteryPayCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最后显示时间 
        [JsonProperty]
        long mySteryLastShowTime;
        [JsonIgnore]
        public long MySteryLastShowTime
        {
            get
            {
                return mySteryLastShowTime;
            }
            set
            {
                if(mySteryLastShowTime != value)
                {
                    mySteryLastShowTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 建筑使用计数  广告用
        [JsonProperty]
        int buildUseCount;
        [JsonIgnore]
        public int BuildUseCount
        {
            get
            {
                return buildUseCount;
            }
            set
            {
                if(buildUseCount != value)
                {
                    buildUseCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // HAPPYGO
        [JsonProperty]
        StorageHappyGo happyGo = new StorageHappyGo();
        [JsonIgnore]
        public StorageHappyGo HappyGo
        {
            get
            {
                return happyGo;
            }
        }
        // ---------------------------------//
        
    }
}