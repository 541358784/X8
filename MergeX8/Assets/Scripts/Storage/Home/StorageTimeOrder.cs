/************************************************
 * Storage class : StorageTimeOrder
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTimeOrder : StorageBase
    {
        
        // 活动ID
        [JsonProperty]
        string activityId = "";
        [JsonIgnore]
        public string ActivityId
        {
            get
            {
                return activityId;
            }
            set
            {
                if(activityId != value)
                {
                    activityId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否加入
        [JsonProperty]
        bool isJoin;
        [JsonIgnore]
        public bool IsJoin
        {
            get
            {
                return isJoin;
            }
            set
            {
                if(isJoin != value)
                {
                    isJoin = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动开始时间
        [JsonProperty]
        long startActivityTime;
        [JsonIgnore]
        public long StartActivityTime
        {
            get
            {
                return startActivityTime;
            }
            set
            {
                if(startActivityTime != value)
                {
                    startActivityTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动结束时间
        [JsonProperty]
        long activityEndTime;
        [JsonIgnore]
        public long ActivityEndTime
        {
            get
            {
                return activityEndTime;
            }
            set
            {
                if(activityEndTime != value)
                {
                    activityEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 参加活动开始时间
        [JsonProperty]
        long joinStartTime;
        [JsonIgnore]
        public long JoinStartTime
        {
            get
            {
                return joinStartTime;
            }
            set
            {
                if(joinStartTime != value)
                {
                    joinStartTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 参加活动结束时间
        [JsonProperty]
        long joinEndTime;
        [JsonIgnore]
        public long JoinEndTime
        {
            get
            {
                return joinEndTime;
            }
            set
            {
                if(joinEndTime != value)
                {
                    joinEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // CONFIG
        [JsonProperty]
        string orderConfig = "";
        [JsonIgnore]
        public string OrderConfig
        {
            get
            {
                return orderConfig;
            }
            set
            {
                if(orderConfig != value)
                {
                    orderConfig = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 礼包ID
        [JsonProperty]
        int giftId;
        [JsonIgnore]
        public int GiftId
        {
            get
            {
                return giftId;
            }
            set
            {
                if(giftId != value)
                {
                    giftId = value;
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
        
        // 是否购买礼包
        [JsonProperty]
        bool isBuyGift;
        [JsonIgnore]
        public bool IsBuyGift
        {
            get
            {
                return isBuyGift;
            }
            set
            {
                if(isBuyGift != value)
                {
                    isBuyGift = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否显示礼包
        [JsonProperty]
        bool isShowGift;
        [JsonIgnore]
        public bool IsShowGift
        {
            get
            {
                return isShowGift;
            }
            set
            {
                if(isShowGift != value)
                {
                    isShowGift = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前完成INDEX
        [JsonProperty]
        StorageList<int> orderGiftIndex = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> OrderGiftIndex
        {
            get
            {
                return orderGiftIndex;
            }
        }
        // ---------------------------------//
        
        // 礼包物品
        [JsonProperty]
        StorageList<int> orderGiftContent = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> OrderGiftContent
        {
            get
            {
                return orderGiftContent;
            }
        }
        // ---------------------------------//
        
        // 分层组
        [JsonProperty]
        int payLevelGroup;
        [JsonIgnore]
        public int PayLevelGroup
        {
            get
            {
                return payLevelGroup;
            }
            set
            {
                if(payLevelGroup != value)
                {
                    payLevelGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}