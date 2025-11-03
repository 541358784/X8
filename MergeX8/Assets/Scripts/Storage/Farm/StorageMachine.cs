/************************************************
 * Storage class : StorageMachine
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMachine : StorageBase
    {
        
        // NODEID
        [JsonProperty]
        int id;
        [JsonIgnore]
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if(id != value)
                {
                    id = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前订单
        [JsonProperty]
        int orderId;
        [JsonIgnore]
        public int OrderId
        {
            get
            {
                return orderId;
            }
            set
            {
                if(orderId != value)
                {
                    orderId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 成熟时间
        [JsonProperty]
        long ripeningTime;
        [JsonIgnore]
        public long RipeningTime
        {
            get
            {
                return ripeningTime;
            }
            set
            {
                if(ripeningTime != value)
                {
                    ripeningTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 开始时间
        [JsonProperty]
        long startTime;
        [JsonIgnore]
        public long StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                if(startTime != value)
                {
                    startTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // CD总时间
        [JsonProperty]
        long cdTime;
        [JsonIgnore]
        public long CdTime
        {
            get
            {
                return cdTime;
            }
            set
            {
                if(cdTime != value)
                {
                    cdTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}