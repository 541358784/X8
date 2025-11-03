/************************************************
 * Storage class : StorageDailyRankGroup
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDailyRankGroup : StorageBase
    {
        
        // 默认初始值
        [JsonProperty]
        StorageList<int> defaultValues = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> DefaultValues
        {
            get
            {
                return defaultValues;
            }
        }
        // ---------------------------------//
        
        // 连续输的次数
        [JsonProperty]
        int lostCount;
        [JsonIgnore]
        public int LostCount
        {
            get
            {
                return lostCount;
            }
            set
            {
                if(lostCount != value)
                {
                    lostCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 连赢次数
        [JsonProperty]
        int winCount;
        [JsonIgnore]
        public int WinCount
        {
            get
            {
                return winCount;
            }
            set
            {
                if(winCount != value)
                {
                    winCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前难度ID
        [JsonProperty]
        int difficultyId;
        [JsonIgnore]
        public int DifficultyId
        {
            get
            {
                return difficultyId;
            }
            set
            {
                if(difficultyId != value)
                {
                    difficultyId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 默认初始值
        [JsonProperty]
        int defaultValue;
        [JsonIgnore]
        public int DefaultValue
        {
            get
            {
                return defaultValue;
            }
            set
            {
                if(defaultValue != value)
                {
                    defaultValue = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
         
        // 排行榜数据
        [JsonProperty]
        StorageDictionary<string,StorageDailyRank> dailyRanks = new StorageDictionary<string,StorageDailyRank>();
        [JsonIgnore]
        public StorageDictionary<string,StorageDailyRank> DailyRanks
        {
            get
            {
                return dailyRanks;
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        StorageList<StorageDailyRankBotscore> botScore = new StorageList<StorageDailyRankBotscore>();
        [JsonIgnore]
        public StorageList<StorageDailyRankBotscore> BotScore
        {
            get
            {
                return botScore;
            }
        }
        // ---------------------------------//
        
    }
}