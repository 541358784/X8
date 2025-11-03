/************************************************
 * Storage class : StorageTreasureMap
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTreasureMap : StorageBase
    {
        
        // 
        [JsonProperty]
        string activityId = "";
        [JsonIgnore]
        public string ActivityId
        {
            get
            {
                return activityId;
            }
            set
            {
                if(activityId != value)
                {
                    activityId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        int mapId;
        [JsonIgnore]
        public int MapId
        {
            get
            {
                return mapId;
            }
            set
            {
                if(mapId != value)
                {
                    mapId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        bool isFinish;
        [JsonIgnore]
        public bool IsFinish
        {
            get
            {
                return isFinish;
            }
            set
            {
                if(isFinish != value)
                {
                    isFinish = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        StorageList<int> collectedChip = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CollectedChip
        {
            get
            {
                return collectedChip;
            }
        }
        // ---------------------------------//
        
        // 新获取未播动画的
        [JsonProperty]
        int newChip;
        [JsonIgnore]
        public int NewChip
        {
            get
            {
                return newChip;
            }
            set
            {
                if(newChip != value)
                {
                    newChip = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        int finishTaskCount;
        [JsonIgnore]
        public int FinishTaskCount
        {
            get
            {
                return finishTaskCount;
            }
            set
            {
                if(finishTaskCount != value)
                {
                    finishTaskCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}