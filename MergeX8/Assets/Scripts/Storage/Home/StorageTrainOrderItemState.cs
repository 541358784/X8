/************************************************
 * Storage class : StorageTrainOrderItemState
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTrainOrderItemState : StorageBase
    {
        
        // 物品ID
        [JsonProperty]
        int itemId;
        [JsonIgnore]
        public int ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                if(itemId != value)
                {
                    itemId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 任务重下标
        [JsonProperty]
        int index;
        [JsonIgnore]
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                if(index != value)
                {
                    index = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 状态
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
        
        // 任务ID
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
        
        // 额外奖励结束时间
        [JsonProperty]
        long extraEndTime;
        [JsonIgnore]
        public long ExtraEndTime
        {
            get
            {
                return extraEndTime;
            }
            set
            {
                if(extraEndTime != value)
                {
                    extraEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}