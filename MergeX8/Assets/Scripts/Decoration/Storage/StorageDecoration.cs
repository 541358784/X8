/************************************************
 * Storage class : StorageDecoration
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage.Decoration
{
    [System.Serializable]
    public class StorageDecoration : StorageBase
    {
         
        // 大地图数据
        [JsonProperty]
        StorageDictionary<int,StorageWorld> worldMap = new StorageDictionary<int,StorageWorld>();
        [JsonIgnore]
        public StorageDictionary<int,StorageWorld> WorldMap
        {
            get
            {
                return worldMap;
            }
        }
        // ---------------------------------//
        
        // 天数进度
        [JsonProperty]
        StorageDays days = new StorageDays();
        [JsonIgnore]
        public StorageDays Days
        {
            get
            {
                return days;
            }
        }
        // ---------------------------------//
        
        // 装修控制元素
        [JsonProperty]
        StorageDictionary<int,string> interactElements = new StorageDictionary<int,string>();
        [JsonIgnore]
        public StorageDictionary<int,string> InteractElements
        {
            get
            {
                return interactElements;
            }
        }
        // ---------------------------------//
        
        // 扩展区域
        [JsonProperty]
        StorageDictionary<int,int> extendArea = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> ExtendArea
        {
            get
            {
                return extendArea;
            }
        }
        // ---------------------------------//
        
    }
}