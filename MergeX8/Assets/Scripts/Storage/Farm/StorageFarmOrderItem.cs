/************************************************
 * Storage class : StorageFarmOrderItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageFarmOrderItem : StorageBase
    {
        
        // 订单ID
        [JsonProperty]
        string id = "";
        [JsonIgnore]
        public string Id
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
        
        // 头像ID
        [JsonProperty]
        int headIndex;
        [JsonIgnore]
        public int HeadIndex
        {
            get
            {
                return headIndex;
            }
            set
            {
                if(headIndex != value)
                {
                    headIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 订单 原始ID
        [JsonProperty]
        string orgId = "";
        [JsonIgnore]
        public string OrgId
        {
            get
            {
                return orgId;
            }
            set
            {
                if(orgId != value)
                {
                    orgId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 订单位置
        [JsonProperty]
        int slot;
        [JsonIgnore]
        public int Slot
        {
            get
            {
                return slot;
            }
            set
            {
                if(slot != value)
                {
                    slot = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 需要物品ID
        [JsonProperty]
        StorageList<int> needItemIds = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> NeedItemIds
        {
            get
            {
                return needItemIds;
            }
        }
        // ---------------------------------//
        
        // 需要物品个数
        [JsonProperty]
        StorageList<int> needItemNums = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> NeedItemNums
        {
            get
            {
                return needItemNums;
            }
        }
        // ---------------------------------//
        
        // 奖励类型
        [JsonProperty]
        StorageList<int> rewardIds = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> RewardIds
        {
            get
            {
                return rewardIds;
            }
        }
        // ---------------------------------//
        
        // 奖励个数
        [JsonProperty]
        StorageList<int> rewardNums = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> RewardNums
        {
            get
            {
                return rewardNums;
            }
        }
        // ---------------------------------//
        
    }
}