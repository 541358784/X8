/************************************************
 * Storage class : StorageFilthy
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageFilthy : StorageBase
    {
        
        // 当前关卡
        [JsonProperty]
        int currentLevel;
        [JsonIgnore]
        public int CurrentLevel
        {
            get
            {
                return currentLevel;
            }
            set
            {
                if(currentLevel != value)
                {
                    currentLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成情况
        [JsonProperty]
        StorageDictionary<int,bool> finishInfo = new StorageDictionary<int,bool>();
        [JsonIgnore]
        public StorageDictionary<int,bool> FinishInfo
        {
            get
            {
                return finishInfo;
            }
        }
        // ---------------------------------//
         
        // 当前节点
        [JsonProperty]
        StorageDictionary<int,StorageFilthyLevel> levels = new StorageDictionary<int,StorageFilthyLevel>();
        [JsonIgnore]
        public StorageDictionary<int,StorageFilthyLevel> Levels
        {
            get
            {
                return levels;
            }
        }
        // ---------------------------------//
        
    }
}