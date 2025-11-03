/************************************************
 * Storage class : StorageLuckBallRecord
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageLuckBallRecord : StorageBase
    {
        
        // 一日内播放次数
        [JsonProperty]
        int playCount;
        [JsonIgnore]
        public int PlayCount
        {
            get
            {
                return playCount;
            }
            set
            {
                if(playCount != value)
                {
                    playCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次播放时间
        [JsonProperty]
        long lastPlayTime;
        [JsonIgnore]
        public long LastPlayTime
        {
            get
            {
                return lastPlayTime;
            }
            set
            {
                if(lastPlayTime != value)
                {
                    lastPlayTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}