/************************************************
 * Storage class : StorageDailyRankReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDailyRankReward : StorageBase
    {
        
        // 奖励类型
        [JsonProperty]
        int type;
        [JsonIgnore]
        public int Type
        {
            get
            {
                return type;
            }
            set
            {
                if(type != value)
                {
                    type = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 奖励数量
        [JsonProperty]
        int num;
        [JsonIgnore]
        public int Num
        {
            get
            {
                return num;
            }
            set
            {
                if(num != value)
                {
                    num = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}