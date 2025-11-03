/************************************************
 * Storage class : StorageMixMaster
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMixMaster : StorageBase
    {
        
        // 活动ID
        [JsonProperty]
        string activiryId = "";
        [JsonIgnore]
        public string ActiviryId
        {
            get
            {
                return activiryId;
            }
            set
            {
                if(activiryId != value)
                {
                    activiryId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 预热时间戳(如果活动开启则进行更新，否则用本地存的)
        [JsonProperty]
        long preheatTime;
        [JsonIgnore]
        public long PreheatTime
        {
            get
            {
                return preheatTime;
            }
            set
            {
                if(preheatTime != value)
                {
                    preheatTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 开始时间戳(如果活动开启则进行更新，否则用本地存的)
        [JsonProperty]
        long startTime;
        [JsonIgnore]
        public long StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                if(startTime != value)
                {
                    startTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 结束时间戳(同STARTTIME)
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
        
        // 材料背包
        [JsonProperty]
        StorageDictionary<int,int> bag = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> Bag
        {
            get
            {
                return bag;
            }
        }
        // ---------------------------------//
         
        // 调制桌面
        [JsonProperty]
        StorageDictionary<int,StorageResData> desktop = new StorageDictionary<int,StorageResData>();
        [JsonIgnore]
        public StorageDictionary<int,StorageResData> Desktop
        {
            get
            {
                return desktop;
            }
        }
        // ---------------------------------//
        
        // 调制配方记录
        [JsonProperty]
        StorageDictionary<int,int> history = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> History
        {
            get
            {
                return history;
            }
        }
        // ---------------------------------//
        
        // 购买礼包次数
        [JsonProperty]
        int buyTimes;
        [JsonIgnore]
        public int BuyTimes
        {
            get
            {
                return buyTimes;
            }
            set
            {
                if(buyTimes != value)
                {
                    buyTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 配方版本
        [JsonProperty]
        int formulaVersion;
        [JsonIgnore]
        public int FormulaVersion
        {
            get
            {
                return formulaVersion;
            }
            set
            {
                if(formulaVersion != value)
                {
                    formulaVersion = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 礼包存档
        [JsonProperty]
        StorageOptionalGift giftBag = new StorageOptionalGift();
        [JsonIgnore]
        public StorageOptionalGift GiftBag
        {
            get
            {
                return giftBag;
            }
        }
        // ---------------------------------//
        
        // 自动弹礼包日期
        [JsonProperty]
        int giftBagPopupDayId;
        [JsonIgnore]
        public int GiftBagPopupDayId
        {
            get
            {
                return giftBagPopupDayId;
            }
            set
            {
                if(giftBagPopupDayId != value)
                {
                    giftBagPopupDayId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 首次调制次数
        [JsonProperty]
        int firstMixCount;
        [JsonIgnore]
        public int FirstMixCount
        {
            get
            {
                return firstMixCount;
            }
            set
            {
                if(firstMixCount != value)
                {
                    firstMixCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已经领奖的混合任务等级
        [JsonProperty]
        StorageList<int> alreadyCollectLevels = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> AlreadyCollectLevels
        {
            get
            {
                return alreadyCollectLevels;
            }
        }
        // ---------------------------------//
        
        // 可领奖的等级
        [JsonProperty]
        StorageList<int> canCollectLevels = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CanCollectLevels
        {
            get
            {
                return canCollectLevels;
            }
        }
        // ---------------------------------//
        
        // 任务产出材料池状态
        [JsonProperty]
        StorageDictionary<int,int> orderOutPutPool = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> OrderOutPutPool
        {
            get
            {
                return orderOutPutPool;
            }
        }
        // ---------------------------------//
        
        // 任务已挂载产出材料
        [JsonProperty]
        StorageDictionary<int,int> orderOutPutState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> OrderOutPutState
        {
            get
            {
                return orderOutPutState;
            }
        }
        // ---------------------------------//
        
        // 任务已挂载产出材料数量
        [JsonProperty]
        StorageDictionary<int,int> orderOutPutCountState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> OrderOutPutCountState
        {
            get
            {
                return orderOutPutCountState;
            }
        }
        // ---------------------------------//
        
    }
}