/************************************************
 * Storage class : StorageAdPlayRecord
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageAdPlayRecord : StorageBase
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
        
        // 当前播放次数
        [JsonProperty]
        int currentPlayCount;
        [JsonIgnore]
        public int CurrentPlayCount
        {
            get
            {
                return currentPlayCount;
            }
            set
            {
                if(currentPlayCount != value)
                {
                    currentPlayCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 成功获得奖励次数
        [JsonProperty]
        int successGetRewardCount;
        [JsonIgnore]
        public int SuccessGetRewardCount
        {
            get
            {
                return successGetRewardCount;
            }
            set
            {
                if(successGetRewardCount != value)
                {
                    successGetRewardCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 广告类型1RV2主动插屏3被动插屏
        [JsonProperty]
        int adType;
        [JsonIgnore]
        public int AdType
        {
            get
            {
                return adType;
            }
            set
            {
                if(adType != value)
                {
                    adType = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}