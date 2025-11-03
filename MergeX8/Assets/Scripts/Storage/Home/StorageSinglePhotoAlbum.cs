/************************************************
 * Storage class : StorageSinglePhotoAlbum
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageSinglePhotoAlbum : StorageBase
    {
        
        // 相册收集状态
        [JsonProperty]
        StorageList<int> collectState = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CollectState
        {
            get
            {
                return collectState;
            }
        }
        // ---------------------------------//
        
    }
}