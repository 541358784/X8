/************************************************
 * Storage class : StorageHGBundleItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageHGBundleItem : StorageBase
    {
        
        // 对应ID
        [JsonProperty]
        int packId;
        [JsonIgnore]
        public int PackId
        {
            get
            {
                return packId;
            }
            set
            {
                if(packId != value)
                {
                    packId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // SHOPID
        [JsonProperty]
        int shopId;
        [JsonIgnore]
        public int ShopId
        {
            get
            {
                return shopId;
            }
            set
            {
                if(shopId != value)
                {
                    shopId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 结束时间
        [JsonProperty]
        long endTime;
        [JsonIgnore]
        public long EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                if(endTime != value)
                {
                    endTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 剩余购买次数
        [JsonProperty]
        int leftBuyCount;
        [JsonIgnore]
        public int LeftBuyCount
        {
            get
            {
                return leftBuyCount;
            }
            set
            {
                if(leftBuyCount != value)
                {
                    leftBuyCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}