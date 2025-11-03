/************************************************
 * Storage class : StorageMiniGameVersion
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMiniGameVersion : StorageBase
    {
         
        // 章节存档
        [JsonProperty]
        StorageDictionary<int,StorageChapter> chapters = new StorageDictionary<int,StorageChapter>();
        [JsonIgnore]
        public StorageDictionary<int,StorageChapter> Chapters
        {
            get
            {
                return chapters;
            }
        }
        // ---------------------------------//
        
        // 玩家上一次进入后所看到的最大解锁关卡ID
        [JsonProperty]
        int lastEnterUnlockLevelId;
        [JsonIgnore]
        public int LastEnterUnlockLevelId
        {
            get
            {
                return lastEnterUnlockLevelId;
            }
            set
            {
                if(lastEnterUnlockLevelId != value)
                {
                    lastEnterUnlockLevelId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // MINIGAMEAB分组
        [JsonProperty]
        string abGroup = "";
        [JsonIgnore]
        public string AbGroup
        {
            get
            {
                return abGroup;
            }
            set
            {
                if(abGroup != value)
                {
                    abGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前正在进行的MINIGAME LEVELID
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
        
        // 当前正在进行的CURRENTCHAPER
        [JsonProperty]
        int currentChapter;
        [JsonIgnore]
        public int CurrentChapter
        {
            get
            {
                return currentChapter;
            }
            set
            {
                if(currentChapter != value)
                {
                    currentChapter = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}