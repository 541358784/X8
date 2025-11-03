/************************************************
 * Storage class : StorageTaskAssistPack
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTaskAssistPack : StorageBase
    {
        
        // 礼包内容
        [JsonProperty]
        StorageList<StorageTaskAssistPackItem> taskAssistPacks = new StorageList<StorageTaskAssistPackItem>();
        [JsonIgnore]
        public StorageList<StorageTaskAssistPackItem> TaskAssistPacks
        {
            get
            {
                return taskAssistPacks;
            }
        }
        // ---------------------------------//
        
        // 已完成展示的任务
        [JsonProperty]
        StorageList<int> finishTaskList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> FinishTaskList
        {
            get
            {
                return finishTaskList;
            }
        }
        // ---------------------------------//
        
    }
}