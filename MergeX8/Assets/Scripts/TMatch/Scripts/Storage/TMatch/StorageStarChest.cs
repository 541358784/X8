/************************************************
 * Storage class : StorageStarChest
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageStarChest : DragonU3DSDK.Storage.StorageBase
    {
        
        // 当前总共获得的星星数量
        [JsonProperty]
        int totalStars;
        [JsonIgnore]
        public int TotalStars
        {
            get
            {
                return totalStars;
            }
            set
            {
                if(totalStars != value)
                {
                    totalStars = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前奖励的下标
        [JsonProperty]
        int curIndex;
        [JsonIgnore]
        public int CurIndex
        {
            get
            {
                return curIndex;
            }
            set
            {
                if(curIndex != value)
                {
                    curIndex = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最后一次展示动画的星星数量
        [JsonProperty]
        int lastShowedStars;
        [JsonIgnore]
        public int LastShowedStars
        {
            get
            {
                return lastShowedStars;
            }
            set
            {
                if(lastShowedStars != value)
                {
                    lastShowedStars = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}