/************************************************
 * Storage class : StorageMergeUnlockItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMergeUnlockItem : StorageBase
    {
        
        // 已经解锁过的图标
        [JsonProperty]
        StorageList<int> unlockIds = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> UnlockIds
        {
            get
            {
                return unlockIds;
            }
        }
        // ---------------------------------//
        
        // 已经解锁过的合成链
        [JsonProperty]
        StorageList<int> unlockLines = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> UnlockLines
        {
            get
            {
                return unlockLines;
            }
        }
        // ---------------------------------//
        
        // 图鉴已经领取过奖励的物品
        [JsonProperty]
        StorageList<int> galleryAwarded = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> GalleryAwarded
        {
            get
            {
                return galleryAwarded;
            }
        }
        // ---------------------------------//
        
        // 已经获取的物品
        [JsonProperty]
        StorageDictionary<int,int> getIds = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> GetIds
        {
            get
            {
                return getIds;
            }
        }
        // ---------------------------------//
        
    }
}