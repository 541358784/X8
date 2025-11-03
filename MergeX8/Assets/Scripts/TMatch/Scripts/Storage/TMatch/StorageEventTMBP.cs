/************************************************
 * Storage class : StorageEventTMBP
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageEventTMBP : DragonU3DSDK.Storage.StorageBase
    {
        
        // 经验（禁止直接加经验 请调用MODEL里面的ADDEXP方法）
        [JsonProperty]
        int exp;
        [JsonIgnore]
        public int Exp
        {
            get
            {
                return exp;
            }
            set
            {
                if(exp != value)
                {
                    exp = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 等级
        [JsonProperty]
        int level;
        [JsonIgnore]
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if(level != value)
                {
                    level = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已领取
        [JsonProperty]
        StorageList<int> claimed = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> Claimed
        {
            get
            {
                return claimed;
            }
        }
        // ---------------------------------//
        
        // 免费已领取
        [JsonProperty]
        StorageList<int> claimedFree = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> ClaimedFree
        {
            get
            {
                return claimedFree;
            }
        }
        // ---------------------------------//
        
        // 上次弹出购买弹出的时间
        [JsonProperty]
        ulong lastPopBuyTime;
        [JsonIgnore]
        public ulong LastPopBuyTime
        {
            get
            {
                return lastPopBuyTime;
            }
            set
            {
                if(lastPopBuyTime != value)
                {
                    lastPopBuyTime = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已查看战令等级
        [JsonProperty]
        int levelViewed;
        [JsonIgnore]
        public int LevelViewed
        {
            get
            {
                return levelViewed;
            }
            set
            {
                if(levelViewed != value)
                {
                    levelViewed = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 战令状态
        [JsonProperty]
        int status;
        [JsonIgnore]
        public int Status
        {
            get
            {
                return status;
            }
            set
            {
                if(status != value)
                {
                    status = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已查看战令状态
        [JsonProperty]
        int statusView;
        [JsonIgnore]
        public int StatusView
        {
            get
            {
                return statusView;
            }
            set
            {
                if(statusView != value)
                {
                    statusView = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否参加过游戏
        [JsonProperty]
        bool isJoinGame;
        [JsonIgnore]
        public bool IsJoinGame
        {
            get
            {
                return isJoinGame;
            }
            set
            {
                if(isJoinGame != value)
                {
                    isJoinGame = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
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
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
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
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}