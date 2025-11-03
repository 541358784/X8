/************************************************
 * Storage class : StorageRecoverCoin
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageRecoverCoin : StorageBase
    {
         
        // 按周存每周的数据
        [JsonProperty]
        StorageDictionary<int,StorageRecoverCoinWeek> storageByWeek = new StorageDictionary<int,StorageRecoverCoinWeek>();
        [JsonIgnore]
        public StorageDictionary<int,StorageRecoverCoinWeek> StorageByWeek
        {
            get
            {
                return storageByWeek;
            }
        }
        // ---------------------------------//
        
        // 弹出完成所有任务弹窗的触发NODEID
        [JsonProperty]
        StorageList<int> lastFinishNodeList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> LastFinishNodeList
        {
            get
            {
                return lastFinishNodeList;
            }
        }
        // ---------------------------------//
         
        // 时间配置
        [JsonProperty]
        StorageDictionary<int,StorageRecoverCoinTimeConfig> weekTimeConfig = new StorageDictionary<int,StorageRecoverCoinTimeConfig>();
        [JsonIgnore]
        public StorageDictionary<int,StorageRecoverCoinTimeConfig> WeekTimeConfig
        {
            get
            {
                return weekTimeConfig;
            }
        }
        // ---------------------------------//
        
        // 最后一个装修挂点的ID
        [JsonProperty]
        int lastDecoNodeId;
        [JsonIgnore]
        public int LastDecoNodeId
        {
            get
            {
                return lastDecoNodeId;
            }
            set
            {
                if(lastDecoNodeId != value)
                {
                    lastDecoNodeId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}