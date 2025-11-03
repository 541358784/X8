/************************************************
 * Storage class : StorageRemoveAd
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageRemoveAd : DragonU3DSDK.Storage.StorageBase
    {
        
        // 每日弹窗次数
        [JsonProperty]
        int popupDailyTimes;
        [JsonIgnore]
        public int PopupDailyTimes
        {
            get
            {
                return popupDailyTimes;
            }
            set
            {
                if(popupDailyTimes != value)
                {
                    popupDailyTimes = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 去广告
        [JsonProperty]
        bool removeAd;
        [JsonIgnore]
        public bool RemoveAd
        {
            get
            {
                return removeAd;
            }
            set
            {
                if(removeAd != value)
                {
                    removeAd = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次弹出的时间戳
        [JsonProperty]
        long lastPopupTimestamp;
        [JsonIgnore]
        public long LastPopupTimestamp
        {
            get
            {
                return lastPopupTimestamp;
            }
            set
            {
                if(lastPopupTimestamp != value)
                {
                    lastPopupTimestamp = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}