/************************************************
 * Storage class : StorageMergeItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMergeItem : StorageBase
    {
        
        // 
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
        
        // -1-未开启，0-锁定，1-解锁 2 --气泡 3 --激活类型
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
        
        // 0-蛛网，1-解锁
        [JsonProperty]
        int unlockState;
        [JsonIgnore]
        public int UnlockState
        {
            get
            {
                return unlockState;
            }
            set
            {
                if(unlockState != value)
                {
                    unlockState = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上一次产出时间
        [JsonProperty]
        ulong productTime;
        [JsonIgnore]
        public ulong ProductTime
        {
            get
            {
                return productTime;
            }
            set
            {
                if(productTime != value)
                {
                    productTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上一次时间产出时间
        [JsonProperty]
        ulong timProductTime;
        [JsonIgnore]
        public ulong TimProductTime
        {
            get
            {
                return timProductTime;
            }
            set
            {
                if(timProductTime != value)
                {
                    timProductTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 产出了多少次
        [JsonProperty]
        int productCount;
        [JsonIgnore]
        public int ProductCount
        {
            get
            {
                return productCount;
            }
            set
            {
                if(productCount != value)
                {
                    productCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 时间产出了多少次
        [JsonProperty]
        int timeProductCount;
        [JsonIgnore]
        public int TimeProductCount
        {
            get
            {
                return timeProductCount;
            }
            set
            {
                if(timeProductCount != value)
                {
                    timeProductCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 生成时间
        [JsonProperty]
        ulong openTime;
        [JsonIgnore]
        public ulong OpenTime
        {
            get
            {
                return openTime;
            }
            set
            {
                if(openTime != value)
                {
                    openTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 时间产出建筑产出的暂未放入棋盘的物品
        [JsonProperty]
        StorageList<int> productItems = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> ProductItems
        {
            get
            {
                return productItems;
            }
        }
        // ---------------------------------//
        
        // 当前建筑存贮的产出量
        [JsonProperty]
        int storeMax;
        [JsonIgnore]
        public int StoreMax
        {
            get
            {
                return storeMax;
            }
            set
            {
                if(storeMax != value)
                {
                    storeMax = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 时间产出存贮
        [JsonProperty]
        int timeStoreMax;
        [JsonIgnore]
        public int TimeStoreMax
        {
            get
            {
                return timeStoreMax;
            }
            set
            {
                if(timeStoreMax != value)
                {
                    timeStoreMax = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当存贮产出量不满的时间
        [JsonProperty]
        ulong inCdTime;
        [JsonIgnore]
        public ulong InCdTime
        {
            get
            {
                return inCdTime;
            }
            set
            {
                if(inCdTime != value)
                {
                    inCdTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 气泡解锁类型 0 钻石 1 RV
        [JsonProperty]
        int bubbleType;
        [JsonIgnore]
        public int BubbleType
        {
            get
            {
                return bubbleType;
            }
            set
            {
                if(bubbleType != value)
                {
                    bubbleType = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 激活时间
        [JsonProperty]
        ulong activeTime;
        [JsonIgnore]
        public ulong ActiveTime
        {
            get
            {
                return activeTime;
            }
            set
            {
                if(activeTime != value)
                {
                    activeTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否处于暂停
        [JsonProperty]
        bool isPause;
        [JsonIgnore]
        public bool IsPause
        {
            get
            {
                return isPause;
            }
            set
            {
                if(isPause != value)
                {
                    isPause = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 暂停时的CD(秒)
        [JsonProperty]
        int pauseCDTime;
        [JsonIgnore]
        public int PauseCDTime
        {
            get
            {
                return pauseCDTime;
            }
            set
            {
                if(pauseCDTime != value)
                {
                    pauseCDTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 保底机制KEY:ID VALE 累计没产生该ID的次数
        [JsonProperty]
        StorageDictionary<int,int> dropIntervalDic = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> DropIntervalDic
        {
            get
            {
                return dropIntervalDic;
            }
        }
        // ---------------------------------//
        
        // 观看的RV次数
        [JsonProperty]
        int playRvNum;
        [JsonIgnore]
        public int PlayRvNum
        {
            get
            {
                return playRvNum;
            }
            set
            {
                if(playRvNum != value)
                {
                    playRvNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 一共产出多少轮
        [JsonProperty]
        int productWheel;
        [JsonIgnore]
        public int ProductWheel
        {
            get
            {
                return productWheel;
            }
            set
            {
                if(productWheel != value)
                {
                    productWheel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 堆叠数量
        [JsonProperty]
        int stackNum;
        [JsonIgnore]
        public int StackNum
        {
            get
            {
                return stackNum;
            }
            set
            {
                if(stackNum != value)
                {
                    stackNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 道具参数 
        [JsonProperty]
        int boosterFactor;
        [JsonIgnore]
        public int BoosterFactor
        {
            get
            {
                return boosterFactor;
            }
            set
            {
                if(boosterFactor != value)
                {
                    boosterFactor = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 大小CD刷新时间
        [JsonProperty]
        ulong bsRefreshTime;
        [JsonIgnore]
        public ulong BsRefreshTime
        {
            get
            {
                return bsRefreshTime;
            }
            set
            {
                if(bsRefreshTime != value)
                {
                    bsRefreshTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 大小CD位置 
        [JsonProperty]
        int bsIndex;
        [JsonIgnore]
        public int BsIndex
        {
            get
            {
                return bsIndex;
            }
            set
            {
                if(bsIndex != value)
                {
                    bsIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 吃建筑存档
        [JsonProperty]
        StorageDictionary<int,int> eatBuildingDic = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> EatBuildingDic
        {
            get
            {
                return eatBuildingDic;
            }
        }
        // ---------------------------------//
        
    }
}