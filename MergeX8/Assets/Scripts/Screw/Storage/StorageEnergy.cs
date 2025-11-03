/************************************************
 * Storage class : StorageEnergy
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageEnergy : StorageBase
    {
        
        // 当前体力上限
        [JsonProperty]
        int maxEnergy;
        [JsonIgnore]
        public int MaxEnergy
        {
            get
            {
                return maxEnergy;
            }
            set
            {
                if(maxEnergy != value)
                {
                    maxEnergy = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 体力上限结束时间
        [JsonProperty]
        long maxEnergyEndTime;
        [JsonIgnore]
        public long MaxEnergyEndTime
        {
            get
            {
                return maxEnergyEndTime;
            }
            set
            {
                if(maxEnergyEndTime != value)
                {
                    maxEnergyEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最后增加体力时间
        [JsonProperty]
        long lastAddEnergyTime;
        [JsonIgnore]
        public long LastAddEnergyTime
        {
            get
            {
                return lastAddEnergyTime;
            }
            set
            {
                if(lastAddEnergyTime != value)
                {
                    lastAddEnergyTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}