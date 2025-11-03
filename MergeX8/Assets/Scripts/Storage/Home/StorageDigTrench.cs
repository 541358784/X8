/************************************************
 * Storage class : StorageDigTrench
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDigTrench : StorageBase
    {
        
        // 完成情况
        [JsonProperty]
        StorageDictionary<int,bool> finishInfo = new StorageDictionary<int,bool>();
        [JsonIgnore]
        public StorageDictionary<int,bool> FinishInfo
        {
            get
            {
                return finishInfo;
            }
        }
        // ---------------------------------//
        
        // 当前进度
        [JsonProperty]
        StorageDictionary<int,int> bodyPartStep = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> BodyPartStep
        {
            get
            {
                return bodyPartStep;
            }
        }
        // ---------------------------------//
        
    }
}