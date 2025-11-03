/************************************************
 * Storage class : StorageBattlePassRewardConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageBattlePassRewardConfig : StorageBase
    {
        
        // 表示阶段
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
        
        // 普通是否领取
        [JsonProperty]
        bool isNormalGet;
        [JsonIgnore]
        public bool IsNormalGet
        {
            get
            {
                return isNormalGet;
            }
            set
            {
                if(isNormalGet != value)
                {
                    isNormalGet = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 充值是否领取
        [JsonProperty]
        bool isPurchaseGet;
        [JsonIgnore]
        public bool IsPurchaseGet
        {
            get
            {
                return isPurchaseGet;
            }
            set
            {
                if(isPurchaseGet != value)
                {
                    isPurchaseGet = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 领取的ID
        [JsonProperty]
        StorageList<int> getRewardIndex = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> GetRewardIndex
        {
            get
            {
                return getRewardIndex;
            }
        }
        // ---------------------------------//
        
        // 门票商品ID列表参考REWARDS表

        [JsonProperty]
        StorageList<int> purchaseRewardIds = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> PurchaseRewardIds
        {
            get
            {
                return purchaseRewardIds;
            }
        }
        // ---------------------------------//
        
        // 门票商品对应的数量
        [JsonProperty]
        StorageList<int> purchaseRewardCounts = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> PurchaseRewardCounts
        {
            get
            {
                return purchaseRewardCounts;
            }
        }
        // ---------------------------------//
        
        // 普通商品ID列表参考REWARDS表标绿 顾客小屋配置ID
        [JsonProperty]
        StorageList<int> normalRewardIds = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> NormalRewardIds
        {
            get
            {
                return normalRewardIds;
            }
        }
        // ---------------------------------//
        
        // 普通商品对应的数量
        [JsonProperty]
        StorageList<int> normalRewardCounts = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> NormalRewardCounts
        {
            get
            {
                return normalRewardCounts;
            }
        }
        // ---------------------------------//
        
        // 解锁当前级别需求积分
        [JsonProperty]
        int unlockScore;
        [JsonIgnore]
        public int UnlockScore
        {
            get
            {
                return unlockScore;
            }
            set
            {
                if(unlockScore != value)
                {
                    unlockScore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}