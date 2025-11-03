/************************************************
 * Storage class : StorageASMR
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageASMR : StorageBase
    {
        
        // 开启ASMR的时间戳(MS)
        [JsonProperty]
        ulong startTime;
        [JsonIgnore]
        public ulong StartTime
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
        
        // 是否开启过引导
        [JsonProperty]
        bool isPopGuide;
        [JsonIgnore]
        public bool IsPopGuide
        {
            get
            {
                return isPopGuide;
            }
            set
            {
                if(isPopGuide != value)
                {
                    isPopGuide = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 关卡信息
        [JsonProperty]
        StorageList<StorageASMRLevel> levelInfos = new StorageList<StorageASMRLevel>();
        [JsonIgnore]
        public StorageList<StorageASMRLevel> LevelInfos
        {
            get
            {
                return levelInfos;
            }
        }
        // ---------------------------------//
        
        // 当天掉落头像数量
        [JsonProperty]
        uint todayRewardCount;
        [JsonIgnore]
        public uint TodayRewardCount
        {
            get
            {
                return todayRewardCount;
            }
            set
            {
                if(todayRewardCount != value)
                {
                    todayRewardCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 获得头像次数
        [JsonProperty]
        int rewardAvatarTimes;
        [JsonIgnore]
        public int RewardAvatarTimes
        {
            get
            {
                return rewardAvatarTimes;
            }
            set
            {
                if(rewardAvatarTimes != value)
                {
                    rewardAvatarTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 掉落权重INDEX
        [JsonProperty]
        int dropWeigthIndex;
        [JsonIgnore]
        public int DropWeigthIndex
        {
            get
            {
                return dropWeigthIndex;
            }
            set
            {
                if(dropWeigthIndex != value)
                {
                    dropWeigthIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当天免费游玩次数记录
        [JsonProperty]
        int freePlayCount;
        [JsonIgnore]
        public int FreePlayCount
        {
            get
            {
                return freePlayCount;
            }
            set
            {
                if(freePlayCount != value)
                {
                    freePlayCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}