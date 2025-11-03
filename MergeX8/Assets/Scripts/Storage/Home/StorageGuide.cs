/************************************************
 * Storage class : StorageGuide
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageGuide : StorageBase
    {
        
        // 已完成的引导
        [JsonProperty]
        StorageDictionary<int,int> guideFinished = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> GuideFinished
        {
            get
            {
                return guideFinished;
            }
        }
        // ---------------------------------//
        
    }
}