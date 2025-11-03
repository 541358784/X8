/************************************************
 * Storage class : StorageCardCollectionLevelRandomGroup
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCardCollectionLevelRandomGroup : StorageBase
    {
        
        // 等级
        [JsonProperty]
        int level;
        [JsonIgnore]
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if(level != value)
                {
                    level = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 随机组列表
        [JsonProperty]
        StorageList<StorageCardCollectionRandomGroup> randomGroups = new StorageList<StorageCardCollectionRandomGroup>();
        [JsonIgnore]
        public StorageList<StorageCardCollectionRandomGroup> RandomGroups
        {
            get
            {
                return randomGroups;
            }
        }
        // ---------------------------------//
        
    }
}