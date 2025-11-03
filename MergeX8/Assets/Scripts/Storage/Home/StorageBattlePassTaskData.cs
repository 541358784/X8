/************************************************
 * Storage class : StorageBattlePassTaskData
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageBattlePassTaskData : StorageBase
    {
        
        // 任务
        [JsonProperty]
        StorageList<StorageBattlePassTaskInfo> taskInfos = new StorageList<StorageBattlePassTaskInfo>();
        [JsonIgnore]
        public StorageList<StorageBattlePassTaskInfo> TaskInfos
        {
            get
            {
                return taskInfos;
            }
        }
        // ---------------------------------//
        
    }
}