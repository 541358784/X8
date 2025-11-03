/************************************************
 * Storage class : StorageBalloon
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageBalloon : StorageBase
    {
        
        // 显示次数
        [JsonProperty]
        int showCount;
        [JsonIgnore]
        public int ShowCount
        {
            get
            {
                return showCount;
            }
            set
            {
                if(showCount != value)
                {
                    showCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 缓存SHOPID
        [JsonProperty]
        int shopID;
        [JsonIgnore]
        public int ShopID
        {
            get
            {
                return shopID;
            }
            set
            {
                if(shopID != value)
                {
                    shopID = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 支付显示次数
        [JsonProperty]
        int payShowCount;
        [JsonIgnore]
        public int PayShowCount
        {
            get
            {
                return payShowCount;
            }
            set
            {
                if(payShowCount != value)
                {
                    payShowCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 支付次数
        [JsonProperty]
        int payCount;
        [JsonIgnore]
        public int PayCount
        {
            get
            {
                return payCount;
            }
            set
            {
                if(payCount != value)
                {
                    payCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最后显示时间 
        [JsonProperty]
        long lastShowTime;
        [JsonIgnore]
        public long LastShowTime
        {
            get
            {
                return lastShowTime;
            }
            set
            {
                if(lastShowTime != value)
                {
                    lastShowTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}