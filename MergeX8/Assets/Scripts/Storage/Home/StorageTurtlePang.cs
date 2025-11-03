/************************************************
 * Storage class : StorageTurtlePang
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTurtlePang : StorageBase
    {
        
        // 活动ID
        [JsonProperty]
        string activiryId = "";
        [JsonIgnore]
        public string ActiviryId
        {
            get
            {
                return activiryId;
            }
            set
            {
                if(activiryId != value)
                {
                    activiryId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 预热时间戳(如果活动开启则进行更新，否则用本地存的)
        [JsonProperty]
        long preheatTime;
        [JsonIgnore]
        public long PreheatTime
        {
            get
            {
                return preheatTime;
            }
            set
            {
                if(preheatTime != value)
                {
                    preheatTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
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
        
        // 背包
        [JsonProperty]
        StorageDictionary<int,int> bag = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> Bag
        {
            get
            {
                return bag;
            }
        }
        // ---------------------------------//
        
        // 包数
        [JsonProperty]
        int packageCount;
        [JsonIgnore]
        public int PackageCount
        {
            get
            {
                return packageCount;
            }
            set
            {
                if(packageCount != value)
                {
                    packageCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 分数
        [JsonProperty]
        int score;
        [JsonIgnore]
        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                if(score != value)
                {
                    score = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已购买的STOREITEMID
        [JsonProperty]
        StorageList<int> finishStoreItemList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> FinishStoreItemList
        {
            get
            {
                return finishStoreItemList;
            }
        }
        // ---------------------------------//
        
        // 已经表演过解锁的商店等级
        [JsonProperty]
        StorageList<int> unLockStoreLevel = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> UnLockStoreLevel
        {
            get
            {
                return unLockStoreLevel;
            }
        }
        // ---------------------------------//
        
        // 是否在游戏中
        [JsonProperty]
        bool isInGame;
        [JsonIgnore]
        public bool IsInGame
        {
            get
            {
                return isInGame;
            }
            set
            {
                if(isInGame != value)
                {
                    isInGame = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 幸运色
        [JsonProperty]
        int luckyColor;
        [JsonIgnore]
        public int LuckyColor
        {
            get
            {
                return luckyColor;
            }
            set
            {
                if(luckyColor != value)
                {
                    luckyColor = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 棋盘状态
        [JsonProperty]
        StorageDictionary<int,int> boardState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> BoardState
        {
            get
            {
                return boardState;
            }
        }
        // ---------------------------------//
        
        // 基础包剩余数量
        [JsonProperty]
        int basePackageCount;
        [JsonIgnore]
        public int BasePackageCount
        {
            get
            {
                return basePackageCount;
            }
            set
            {
                if(basePackageCount != value)
                {
                    basePackageCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 加赠包剩余数量
        [JsonProperty]
        int extraPackageCount;
        [JsonIgnore]
        public int ExtraPackageCount
        {
            get
            {
                return extraPackageCount;
            }
            set
            {
                if(extraPackageCount != value)
                {
                    extraPackageCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 临时背包
        [JsonProperty]
        StorageDictionary<int,int> bagGame = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> BagGame
        {
            get
            {
                return bagGame;
            }
        }
        // ---------------------------------//
        
    }
}