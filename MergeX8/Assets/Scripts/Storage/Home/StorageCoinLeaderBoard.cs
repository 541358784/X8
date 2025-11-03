/************************************************
 * Storage class : StorageCoinLeaderBoard
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCoinLeaderBoard : StorageBase
    {
         
        // 按活动ID存每个周期的数据
        [JsonProperty]
        StorageDictionary<string,StorageCoinLeaderBoardWeek> storageByWeek = new StorageDictionary<string,StorageCoinLeaderBoardWeek>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCoinLeaderBoardWeek> StorageByWeek
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