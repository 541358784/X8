/************************************************
 * Storage class : StorageDecorationGuide
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDecorationGuide : DragonU3DSDK.Storage.StorageBase
    {
         
        // 引导
        [JsonProperty]
        StorageDictionary<int,StorageDecoGuide> guideData = new StorageDictionary<int,StorageDecoGuide>();
        [JsonIgnore]
        public StorageDictionary<int,StorageDecoGuide> GuideData
        {
            get
            {
                return guideData;
            }
        }
        // ---------------------------------//
        
        // 任务分数
        [JsonProperty]
        StorageDictionary<string,int> taskGoal = new StorageDictionary<string,int>();
        [JsonIgnore]
        public StorageDictionary<string,int> TaskGoal
        {
            get
            {
                return taskGoal;
            }
        }
        // ---------------------------------//
        
        // 是否修复删档BUG
        [JsonProperty]
        bool isDataClearFixed;
        [JsonIgnore]
        public bool IsDataClearFixed
        {
            get
            {
                return isDataClearFixed;
            }
            set
            {
                if(isDataClearFixed != value)
                {
                    isDataClearFixed = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}