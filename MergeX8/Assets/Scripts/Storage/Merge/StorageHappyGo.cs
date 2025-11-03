/************************************************
 * Storage class : StorageHappyGo
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageHappyGo : StorageBase
    {
        
        // 当前活动ID
        [JsonProperty]
        string activtyId = "";
        [JsonIgnore]
        public string ActivtyId
        {
            get
            {
                return activtyId;
            }
            set
            {
                if(activtyId != value)
                {
                    activtyId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        int exp;
        [JsonIgnore]
        public int Exp
        {
            get
            {
                return exp;
            }
            set
            {
                if(exp != value)
                {
                    exp = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 获取开启时间
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
        
        // 
        [JsonProperty]
        bool isShowStartView;
        [JsonIgnore]
        public bool IsShowStartView
        {
            get
            {
                return isShowStartView;
            }
            set
            {
                if(isShowStartView != value)
                {
                    isShowStartView = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否购买延期
        [JsonProperty]
        bool isBuyEntendDay;
        [JsonIgnore]
        public bool IsBuyEntendDay
        {
            get
            {
                return isBuyEntendDay;
            }
            set
            {
                if(isBuyEntendDay != value)
                {
                    isBuyEntendDay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否领取奖励
        [JsonProperty]
        bool isGetReward;
        [JsonIgnore]
        public bool IsGetReward
        {
            get
            {
                return isGetReward;
            }
            set
            {
                if(isGetReward != value)
                {
                    isGetReward = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否玩过游戏
        [JsonProperty]
        bool isPlayGame;
        [JsonIgnore]
        public bool IsPlayGame
        {
            get
            {
                return isPlayGame;
            }
            set
            {
                if(isPlayGame != value)
                {
                    isPlayGame = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 仓鼠任务ID
        [JsonProperty]
        int requestId;
        [JsonIgnore]
        public int RequestId
        {
            get
            {
                return requestId;
            }
            set
            {
                if(requestId != value)
                {
                    requestId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 仓鼠任务索引
        [JsonProperty]
        int requestIndex;
        [JsonIgnore]
        public int RequestIndex
        {
            get
            {
                return requestIndex;
            }
            set
            {
                if(requestIndex != value)
                {
                    requestIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成的任务ID
        [JsonProperty]
        StorageList<int> completeRequestIds = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CompleteRequestIds
        {
            get
            {
                return completeRequestIds;
            }
        }
        // ---------------------------------//
        
        // 完成的任务个数
        [JsonProperty]
        int requestCount;
        [JsonIgnore]
        public int RequestCount
        {
            get
            {
                return requestCount;
            }
            set
            {
                if(requestCount != value)
                {
                    requestCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 体力
        [JsonProperty]
        int hgEnergy;
        [JsonIgnore]
        public int HgEnergy
        {
            get
            {
                return hgEnergy;
            }
            set
            {
                if(hgEnergy != value)
                {
                    hgEnergy = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次加体力时间
        [JsonProperty]
        long hgLastAddEnergyTime;
        [JsonIgnore]
        public long HgLastAddEnergyTime
        {
            get
            {
                return hgLastAddEnergyTime;
            }
            set
            {
                if(hgLastAddEnergyTime != value)
                {
                    hgLastAddEnergyTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        long unlimitEnergyEndUTCTimeInSeconds;
        [JsonIgnore]
        public long UnlimitEnergyEndUTCTimeInSeconds
        {
            get
            {
                return unlimitEnergyEndUTCTimeInSeconds;
            }
            set
            {
                if(unlimitEnergyEndUTCTimeInSeconds != value)
                {
                    unlimitEnergyEndUTCTimeInSeconds = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        StorageHGFlashSale flashSale = new StorageHGFlashSale();
        [JsonIgnore]
        public StorageHGFlashSale FlashSale
        {
            get
            {
                return flashSale;
            }
        }
        // ---------------------------------//
         
        // 
        [JsonProperty]
        StorageDictionary<int,StorageHGBundleItem> bundles = new StorageDictionary<int,StorageHGBundleItem>();
        [JsonIgnore]
        public StorageDictionary<int,StorageHGBundleItem> Bundles
        {
            get
            {
                return bundles;
            }
        }
        // ---------------------------------//
        
        // 奖励领取等级
        [JsonProperty]
        int claimLevel;
        [JsonIgnore]
        public int ClaimLevel
        {
            get
            {
                return claimLevel;
            }
            set
            {
                if(claimLevel != value)
                {
                    claimLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // BUNDLE刷新时间
        [JsonProperty]
        long bundleRefreshTime;
        [JsonIgnore]
        public long BundleRefreshTime
        {
            get
            {
                return bundleRefreshTime;
            }
            set
            {
                if(bundleRefreshTime != value)
                {
                    bundleRefreshTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 礼包弹出索引
        [JsonProperty]
        int packPopIndex;
        [JsonIgnore]
        public int PackPopIndex
        {
            get
            {
                return packPopIndex;
            }
            set
            {
                if(packPopIndex != value)
                {
                    packPopIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 配置
        [JsonProperty]
        StorageList<StorageHappyGoLevelConfig> lvConfig = new StorageList<StorageHappyGoLevelConfig>();
        [JsonIgnore]
        public StorageList<StorageHappyGoLevelConfig> LvConfig
        {
            get
            {
                return lvConfig;
            }
        }
        // ---------------------------------//
        
        // 是否显示过结束
        [JsonProperty]
        bool isShowEnd;
        [JsonIgnore]
        public bool IsShowEnd
        {
            get
            {
                return isShowEnd;
            }
            set
            {
                if(isShowEnd != value)
                {
                    isShowEnd = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否购买建筑
        [JsonProperty]
        bool isBuyBuild;
        [JsonIgnore]
        public bool IsBuyBuild
        {
            get
            {
                return isBuyBuild;
            }
            set
            {
                if(isBuyBuild != value)
                {
                    isBuyBuild = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 延期购买时间
        [JsonProperty]
        int endBuyTime;
        [JsonIgnore]
        public int EndBuyTime
        {
            get
            {
                return endBuyTime;
            }
            set
            {
                if(endBuyTime != value)
                {
                    endBuyTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}