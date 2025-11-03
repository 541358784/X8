/************************************************
 * Storage class : StorageIceBreakingPack
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageIceBreakingPack : DragonU3DSDK.Storage.StorageBase
    {
        
        // 是否购买过
        [JsonProperty]
        bool isBuy;
        [JsonIgnore]
        public bool IsBuy
        {
            get
            {
                return isBuy;
            }
            set
            {
                if(isBuy != value)
                {
                    isBuy = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 礼包链序号
        [JsonProperty]
        int chainIndex;
        [JsonIgnore]
        public int ChainIndex
        {
            get
            {
                return chainIndex;
            }
            set
            {
                if(chainIndex != value)
                {
                    chainIndex = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 礼包链ID
        [JsonProperty]
        int chainId;
        [JsonIgnore]
        public int ChainId
        {
            get
            {
                return chainId;
            }
            set
            {
                if(chainId != value)
                {
                    chainId = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 礼包链开始时间
        [JsonProperty]
        long chainStartTime;
        [JsonIgnore]
        public long ChainStartTime
        {
            get
            {
                return chainStartTime;
            }
            set
            {
                if(chainStartTime != value)
                {
                    chainStartTime = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 礼包链结束时间
        [JsonProperty]
        long chainEndTime;
        [JsonIgnore]
        public long ChainEndTime
        {
            get
            {
                return chainEndTime;
            }
            set
            {
                if(chainEndTime != value)
                {
                    chainEndTime = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 连胜次数记录
        [JsonProperty]
        int winStreakTimes;
        [JsonIgnore]
        public int WinStreakTimes
        {
            get
            {
                return winStreakTimes;
            }
            set
            {
                if(winStreakTimes != value)
                {
                    winStreakTimes = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次弹出时间
        [JsonProperty]
        long lastShowTime;
        [JsonIgnore]
        public long LastShowTime
        {
            get
            {
                return lastShowTime;
            }
            set
            {
                if(lastShowTime != value)
                {
                    lastShowTime = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 礼包当天弹出次数记录
        [JsonProperty]
        StorageDictionary<int,int> packShowTimes = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> PackShowTimes
        {
            get
            {
                return packShowTimes;
            }
        }
        // ---------------------------------//
        
        // 礼包购买次数记录
        [JsonProperty]
        StorageDictionary<int,int> packBuyTimes = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> PackBuyTimes
        {
            get
            {
                return packBuyTimes;
            }
        }
        // ---------------------------------//
        
        // 当天在线时长记录
        [JsonProperty]
        int onlineTime;
        [JsonIgnore]
        public int OnlineTime
        {
            get
            {
                return onlineTime;
            }
            set
            {
                if(onlineTime != value)
                {
                    onlineTime = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最后一次记录在线时长的时间点
        [JsonProperty]
        long lastOnlineTime;
        [JsonIgnore]
        public long LastOnlineTime
        {
            get
            {
                return lastOnlineTime;
            }
            set
            {
                if(lastOnlineTime != value)
                {
                    lastOnlineTime = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 弹出次数记录
        [JsonProperty]
        int popTotalTimes;
        [JsonIgnore]
        public int PopTotalTimes
        {
            get
            {
                return popTotalTimes;
            }
            set
            {
                if(popTotalTimes != value)
                {
                    popTotalTimes = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}