/************************************************
 * Storage class : StorageMiniGame
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMiniGame : StorageBase
    {
        
        // 已领奖的GROUPLIST
        [JsonProperty]
        StorageList<int> finishGroupList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> FinishGroupList
        {
            get
            {
                return finishGroupList;
            }
        }
        // ---------------------------------//
        
        // 是否初始化
        [JsonProperty]
        bool isInit;
        [JsonIgnore]
        public bool IsInit
        {
            get
            {
                return isInit;
            }
            set
            {
                if(isInit != value)
                {
                    isInit = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 默认选择
        [JsonProperty]
        int defaultType;
        [JsonIgnore]
        public int DefaultType
        {
            get
            {
                return defaultType;
            }
            set
            {
                if(defaultType != value)
                {
                    defaultType = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}