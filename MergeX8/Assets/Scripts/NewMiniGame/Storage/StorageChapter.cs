/************************************************
 * Storage class : StorageChapter
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageChapter : StorageBase
    {
        
        // ID
        [JsonProperty]
        int id;
        [JsonIgnore]
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if(id != value)
                {
                    id = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
         
        // 关卡信息
        [JsonProperty]
        StorageDictionary<int,StorageLevel> levelsDic = new StorageDictionary<int,StorageLevel>();
        [JsonIgnore]
        public StorageDictionary<int,StorageLevel> LevelsDic
        {
            get
            {
                return levelsDic;
            }
        }
        // ---------------------------------//
        
        // 进度奖励
        [JsonProperty]
        bool claimed;
        [JsonIgnore]
        public bool Claimed
        {
            get
            {
                return claimed;
            }
            set
            {
                if(claimed != value)
                {
                    claimed = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 剧情已播放
        [JsonProperty]
        bool storyPlayed;
        [JsonIgnore]
        public bool StoryPlayed
        {
            get
            {
                return storyPlayed;
            }
            set
            {
                if(storyPlayed != value)
                {
                    storyPlayed = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}