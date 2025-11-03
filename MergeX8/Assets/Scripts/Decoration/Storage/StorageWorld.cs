/************************************************
 * Storage class : StorageWorld
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage.Decoration
{
    [System.Serializable]
    public class StorageWorld : StorageBase
    {
        
        // 当前状态
        [JsonProperty]
        int state;
        [JsonIgnore]
        public int State
        {
            get
            {
                return state;
            }
            set
            {
                if(state != value)
                {
                    state = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
         
        // 该世界地图上各区域详情
        [JsonProperty]
        StorageDictionary<int,StorageArea> areasData = new StorageDictionary<int,StorageArea>();
        [JsonIgnore]
        public StorageDictionary<int,StorageArea> AreasData
        {
            get
            {
                return areasData;
            }
        }
        // ---------------------------------//
        
        // 地图上NPC的剧情存档（已弃用）
        [JsonProperty]
        StorageDictionary<string,string> worldNpcStoryDict = new StorageDictionary<string,string>();
        [JsonIgnore]
        public StorageDictionary<string,string> WorldNpcStoryDict
        {
            get
            {
                return worldNpcStoryDict;
            }
        }
        // ---------------------------------//
        
        // 地图上NPC的IDLE剧情
        [JsonProperty]
        StorageList<string> worldNpcStoryList = new StorageList<string>();
        [JsonIgnore]
        public StorageList<string> WorldNpcStoryList
        {
            get
            {
                return worldNpcStoryList;
            }
        }
        // ---------------------------------//
        
    }
}