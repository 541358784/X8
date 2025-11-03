/************************************************
 * Storage class : StorageTaskItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTaskItem : StorageBase
    {
        
        // ID
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
        
        // 位置
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
        
        // 物品索引
        [JsonProperty]
        StorageList<int> itemSeatIndex = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> ItemSeatIndex
        {
            get
            {
                return itemSeatIndex;
            }
        }
        // ---------------------------------//
        
        // 原始ID
        [JsonProperty]
        int orgId;
        [JsonIgnore]
        public int OrgId
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
        
        // 预产出ID
        [JsonProperty]
        int preProductId;
        [JsonIgnore]
        public int PreProductId
        {
            get
            {
                return preProductId;
            }
            set
            {
                if(preProductId != value)
                {
                    preProductId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 任务TYPE
        [JsonProperty]
        int type;
        [JsonIgnore]
        public int Type
        {
            get
            {
                return type;
            }
            set
            {
                if(type != value)
                {
                    type = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 需要物品ID
        [JsonProperty]
        StorageList<int> itemIds = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> ItemIds
        {
            get
            {
                return itemIds;
            }
        }
        // ---------------------------------//
        
        // 需要物品个数
        [JsonProperty]
        StorageList<int> itemNums = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> ItemNums
        {
            get
            {
                return itemNums;
            }
        }
        // ---------------------------------//
        
        // 奖励类型
        [JsonProperty]
        StorageList<int> rewardTypes = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> RewardTypes
        {
            get
            {
                return rewardTypes;
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
        
        // 额外奖励类型
        [JsonProperty]
        StorageList<int> extraRewardTypes = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> ExtraRewardTypes
        {
            get
            {
                return extraRewardTypes;
            }
        }
        // ---------------------------------//
        
        // 额外奖励个数
        [JsonProperty]
        StorageList<int> extraRewardNums = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> ExtraRewardNums
        {
            get
            {
                return extraRewardNums;
            }
        }
        // ---------------------------------//
        
        // 是否是困难任务
        [JsonProperty]
        bool isHard;
        [JsonIgnore]
        public bool IsHard
        {
            get
            {
                return isHard;
            }
            set
            {
                if(isHard != value)
                {
                    isHard = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 小狗饼干数量
        [JsonProperty]
        int dogCookiesNum;
        [JsonIgnore]
        public int DogCookiesNum
        {
            get
            {
                return dogCookiesNum;
            }
            set
            {
                if(dogCookiesNum != value)
                {
                    dogCookiesNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        //  是否触发任务资源礼包
        [JsonProperty]
        bool assist;
        [JsonIgnore]
        public bool Assist
        {
            get
            {
                return assist;
            }
            set
            {
                if(assist != value)
                {
                    assist = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}