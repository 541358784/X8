/************************************************
 * Storage class : StorageLevelChest
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageLevelChest : DragonU3DSDK.Storage.StorageBase
    {
        
        // 当前累计的等级数量
        [JsonProperty]
        int totalLevel;
        [JsonIgnore]
        public int TotalLevel
        {
            get
            {
                return totalLevel;
            }
            set
            {
                if(totalLevel != value)
                {
                    totalLevel = value;
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
        
        // 最后一次展示动画的累计等级
        [JsonProperty]
        int lastShowedLevel;
        [JsonIgnore]
        public int LastShowedLevel
        {
            get
            {
                return lastShowedLevel;
            }
            set
            {
                if(lastShowedLevel != value)
                {
                    lastShowedLevel = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}