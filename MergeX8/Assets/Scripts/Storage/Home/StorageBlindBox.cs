/************************************************
 * Storage class : StorageBlindBox
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageBlindBox : StorageBase
    {
        
        // 主题ID
        [JsonProperty]
        int themeId;
        [JsonIgnore]
        public int ThemeId
        {
            get
            {
                return themeId;
            }
            set
            {
                if(themeId != value)
                {
                    themeId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 领奖状态
        [JsonProperty]
        StorageList<int> collectGroups = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CollectGroups
        {
            get
            {
                return collectGroups;
            }
        }
        // ---------------------------------//
        
        // 已收集状态
        [JsonProperty]
        StorageDictionary<int,int> collectItems = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> CollectItems
        {
            get
            {
                return collectItems;
            }
        }
        // ---------------------------------//
        
        // 盲盒数
        [JsonProperty]
        int blindBoxCount;
        [JsonIgnore]
        public int BlindBoxCount
        {
            get
            {
                return blindBoxCount;
            }
            set
            {
                if(blindBoxCount != value)
                {
                    blindBoxCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 总开启盲盒数
        [JsonProperty]
        int totalCollectTimes;
        [JsonIgnore]
        public int TotalCollectTimes
        {
            get
            {
                return totalCollectTimes;
            }
            set
            {
                if(totalCollectTimes != value)
                {
                    totalCollectTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前概率组开启盲盒数
        [JsonProperty]
        int curCollectTimes;
        [JsonIgnore]
        public int CurCollectTimes
        {
            get
            {
                return curCollectTimes;
            }
            set
            {
                if(curCollectTimes != value)
                {
                    curCollectTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}