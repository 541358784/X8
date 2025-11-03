/************************************************
 * Storage class : StorageMiniGames
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMiniGames : StorageBase
    {
        
        // EXIN 小游戏
        [JsonProperty]
        StorageFilthy filthy = new StorageFilthy();
        [JsonIgnore]
        public StorageFilthy Filthy
        {
            get
            {
                return filthy;
            }
        }
        // ---------------------------------//
        
    }
}