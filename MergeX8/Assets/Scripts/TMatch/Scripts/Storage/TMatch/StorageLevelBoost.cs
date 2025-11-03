/************************************************
 * Storage class : StorageLevelBoost
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageLevelBoost : DragonU3DSDK.Storage.StorageBase
    {
        
        // 是否选择的闪电
        [JsonProperty]
        bool useLighting;
        [JsonIgnore]
        public bool UseLighting
        {
            get
            {
                return useLighting;
            }
            set
            {
                if(useLighting != value)
                {
                    useLighting = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否选择了时钟
        [JsonProperty]
        bool useClock;
        [JsonIgnore]
        public bool UseClock
        {
            get
            {
                return useClock;
            }
            set
            {
                if(useClock != value)
                {
                    useClock = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}