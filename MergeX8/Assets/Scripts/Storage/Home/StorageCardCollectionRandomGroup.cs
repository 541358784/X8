/************************************************
 * Storage class : StorageCardCollectionRandomGroup
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCardCollectionRandomGroup : StorageBase
    {
        
        // 剩余池子
        [JsonProperty]
        StorageDictionary<int,int> pool = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> Pool
        {
            get
            {
                return pool;
            }
        }
        // ---------------------------------//
        
        // 升级所需剩余数量
        [JsonProperty]
        int levelUpLeftCount;
        [JsonIgnore]
        public int LevelUpLeftCount
        {
            get
            {
                return levelUpLeftCount;
            }
            set
            {
                if(levelUpLeftCount != value)
                {
                    levelUpLeftCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上一级权重放大倍率
        [JsonProperty]
        float lastLevelWeightScale;
        [JsonIgnore]
        public float LastLevelWeightScale
        {
            get
            {
                return lastLevelWeightScale;
            }
            set
            {
                if(lastLevelWeightScale != value)
                {
                    lastLevelWeightScale = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}