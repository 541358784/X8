/************************************************
 * Storage class : StorageFarmOrder
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageFarmOrder : StorageBase
    {
        
        // 订单唯一ID 自增
        [JsonProperty]
        int onlyId;
        [JsonIgnore]
        public int OnlyId
        {
            get
            {
                return onlyId;
            }
            set
            {
                if(onlyId != value)
                {
                    onlyId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 固定任务INDEX
        [JsonProperty]
        int fixIndex;
        [JsonIgnore]
        public int FixIndex
        {
            get
            {
                return fixIndex;
            }
            set
            {
                if(fixIndex != value)
                {
                    fixIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成订单的总个数
        [JsonProperty]
        int finishOrderCount;
        [JsonIgnore]
        public int FinishOrderCount
        {
            get
            {
                return finishOrderCount;
            }
            set
            {
                if(finishOrderCount != value)
                {
                    finishOrderCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成的订单ID
        [JsonProperty]
        StorageDictionary<string,string> finishOrderIds = new StorageDictionary<string,string>();
        [JsonIgnore]
        public StorageDictionary<string,string> FinishOrderIds
        {
            get
            {
                return finishOrderIds;
            }
        }
        // ---------------------------------//
        
        // 当前订单
        [JsonProperty]
        StorageList<StorageFarmOrderItem> orders = new StorageList<StorageFarmOrderItem>();
        [JsonIgnore]
        public StorageList<StorageFarmOrderItem> Orders
        {
            get
            {
                return orders;
            }
        }
        // ---------------------------------//
        
    }
}