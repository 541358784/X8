/************************************************
 * Storage class : StorageGlodenHatter
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageGlodenHatter : DragonU3DSDK.Storage.StorageBase
    {
        
        // 连赢次数
        [JsonProperty]
        int winningStreakCnt;
        [JsonIgnore]
        public int WinningStreakCnt
        {
            get
            {
                return winningStreakCnt;
            }
            set
            {
                if(winningStreakCnt != value)
                {
                    winningStreakCnt = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 黄金帽引导标记
        [JsonProperty]
        bool glodenHatterGuid;
        [JsonIgnore]
        public bool GlodenHatterGuid
        {
            get
            {
                return glodenHatterGuid;
            }
            set
            {
                if(glodenHatterGuid != value)
                {
                    glodenHatterGuid = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}