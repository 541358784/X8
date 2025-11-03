/************************************************
 * Storage class : StorageCardCollectionCardItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCardCollectionCardItem : StorageBase
    {
        
        // 卡牌ID
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
        
        // 已获取的数量
        [JsonProperty]
        int count;
        [JsonIgnore]
        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                if(count != value)
                {
                    count = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 每次卡牌获取的时间
        [JsonProperty]
        StorageList<ulong> getTime = new StorageList<ulong>();
        [JsonIgnore]
        public StorageList<ulong> GetTime
        {
            get
            {
                return getTime;
            }
        }
        // ---------------------------------//
        
        // 玩家是否预览过
        [JsonProperty]
        bool isViewed;
        [JsonIgnore]
        public bool IsViewed
        {
            get
            {
                return isViewed;
            }
            set
            {
                if(isViewed != value)
                {
                    isViewed = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 消耗数量
        [JsonProperty]
        int consumeCount;
        [JsonIgnore]
        public int ConsumeCount
        {
            get
            {
                return consumeCount;
            }
            set
            {
                if(consumeCount != value)
                {
                    consumeCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}