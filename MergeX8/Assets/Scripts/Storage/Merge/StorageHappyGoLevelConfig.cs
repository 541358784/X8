/************************************************
 * Storage class : StorageHappyGoLevelConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageHappyGoLevelConfig : StorageBase
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
        
        // 等级
        [JsonProperty]
        int lv;
        [JsonIgnore]
        public int Lv
        {
            get
            {
                return lv;
            }
            set
            {
                if(lv != value)
                {
                    lv = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 升级需要经验
        [JsonProperty]
        int xp;
        [JsonIgnore]
        public int Xp
        {
            get
            {
                return xp;
            }
            set
            {
                if(xp != value)
                {
                    xp = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 奖励物品
        [JsonProperty]
        StorageList<int> reward = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> Reward
        {
            get
            {
                return reward;
            }
        }
        // ---------------------------------//
        
        // 物品数量
        [JsonProperty]
        StorageList<int> amount = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> Amount
        {
            get
            {
                return amount;
            }
        }
        // ---------------------------------//
        
        // 是否补发
        [JsonProperty]
        bool reissue;
        [JsonIgnore]
        public bool Reissue
        {
            get
            {
                return reissue;
            }
            set
            {
                if(reissue != value)
                {
                    reissue = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}