/************************************************
 * Storage class : StorageKeepPet
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageKeepPet : StorageBase
    {
        
        // 是否显示过开始弹窗
        [JsonProperty]
        bool isStart;
        [JsonIgnore]
        public bool IsStart
        {
            get
            {
                return isStart;
            }
            set
            {
                if(isStart != value)
                {
                    isStart = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 分组
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
        
        // 当前宠物状态
        [JsonProperty]
        int curPetState;
        [JsonIgnore]
        public int CurPetState
        {
            get
            {
                return curPetState;
            }
            set
            {
                if(curPetState != value)
                {
                    curPetState = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 体力值
        [JsonProperty]
        int power;
        [JsonIgnore]
        public int Power
        {
            get
            {
                return power;
            }
            set
            {
                if(power != value)
                {
                    power = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 体力道具数量
        [JsonProperty]
        int powerPropCount;
        [JsonIgnore]
        public int PowerPropCount
        {
            get
            {
                return powerPropCount;
            }
            set
            {
                if(powerPropCount != value)
                {
                    powerPropCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 飞盘数量
        [JsonProperty]
        int frisbeeCount;
        [JsonIgnore]
        public int FrisbeeCount
        {
            get
            {
                return frisbeeCount;
            }
            set
            {
                if(frisbeeCount != value)
                {
                    frisbeeCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 狗头数量
        [JsonProperty]
        int dogHeadCount;
        [JsonIgnore]
        public int DogHeadCount
        {
            get
            {
                return dogHeadCount;
            }
            set
            {
                if(dogHeadCount != value)
                {
                    dogHeadCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 经验值
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
        
        // 等级奖励领取状态
        [JsonProperty]
        StorageDictionary<int,bool> levelRewardCollectState = new StorageDictionary<int,bool>();
        [JsonIgnore]
        public StorageDictionary<int,bool> LevelRewardCollectState
        {
            get
            {
                return levelRewardCollectState;
            }
        }
        // ---------------------------------//
        
        // 装饰获取状态
        [JsonProperty]
        StorageDictionary<int,bool> buildingCollectState = new StorageDictionary<int,bool>();
        [JsonIgnore]
        public StorageDictionary<int,bool> BuildingCollectState
        {
            get
            {
                return buildingCollectState;
            }
        }
        // ---------------------------------//
        
        // 装饰使用状态
        [JsonProperty]
        StorageDictionary<int,int> buildingActiveState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> BuildingActiveState
        {
            get
            {
                return buildingActiveState;
            }
        }
        // ---------------------------------//
        
        // 饥饿刷新日期
        [JsonProperty]
        int hungryDayId;
        [JsonIgnore]
        public int HungryDayId
        {
            get
            {
                return hungryDayId;
            }
            set
            {
                if(hungryDayId != value)
                {
                    hungryDayId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 病药数量
        [JsonProperty]
        int medicineCount;
        [JsonIgnore]
        public int MedicineCount
        {
            get
            {
                return medicineCount;
            }
            set
            {
                if(medicineCount != value)
                {
                    medicineCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 被治愈
        [JsonProperty]
        bool cure;
        [JsonIgnore]
        public bool Cure
        {
            get
            {
                return cure;
            }
            set
            {
                if(cure != value)
                {
                    cure = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已领取巡逻任务奖励
        [JsonProperty]
        bool collectSearchTaskReward;
        [JsonIgnore]
        public bool CollectSearchTaskReward
        {
            get
            {
                return collectSearchTaskReward;
            }
            set
            {
                if(collectSearchTaskReward != value)
                {
                    collectSearchTaskReward = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 牛排商店刷新时间
        [JsonProperty]
        long storeRefreshTime;
        [JsonIgnore]
        public long StoreRefreshTime
        {
            get
            {
                return storeRefreshTime;
            }
            set
            {
                if(storeRefreshTime != value)
                {
                    storeRefreshTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 三合一商店是否购买
        [JsonProperty]
        bool threeOneStoreBuyState;
        [JsonIgnore]
        public bool ThreeOneStoreBuyState
        {
            get
            {
                return threeOneStoreBuyState;
            }
            set
            {
                if(threeOneStoreBuyState != value)
                {
                    threeOneStoreBuyState = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 牛排商店购买次数
        [JsonProperty]
        int storeBuyTimes;
        [JsonIgnore]
        public int StoreBuyTimes
        {
            get
            {
                return storeBuyTimes;
            }
            set
            {
                if(storeBuyTimes != value)
                {
                    storeBuyTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否获得宝图
        [JsonProperty]
        bool getTreasureMap;
        [JsonIgnore]
        public bool GetTreasureMap
        {
            get
            {
                return getTreasureMap;
            }
            set
            {
                if(getTreasureMap != value)
                {
                    getTreasureMap = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 巡逻任务奖励
        [JsonProperty]
        StorageList<StorageResData> searchTaskRewardList = new StorageList<StorageResData>();
        [JsonIgnore]
        public StorageList<StorageResData> SearchTaskRewardList
        {
            get
            {
                return searchTaskRewardList;
            }
        }
        // ---------------------------------//
        
        // 巡逻任务奖励额外选择数量
        [JsonProperty]
        int searchTaskExtraSelectRewardCount;
        [JsonIgnore]
        public int SearchTaskExtraSelectRewardCount
        {
            get
            {
                return searchTaskExtraSelectRewardCount;
            }
            set
            {
                if(searchTaskExtraSelectRewardCount != value)
                {
                    searchTaskExtraSelectRewardCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 巡逻任务ID
        [JsonProperty]
        int searchTaskId;
        [JsonIgnore]
        public int SearchTaskId
        {
            get
            {
                return searchTaskId;
            }
            set
            {
                if(searchTaskId != value)
                {
                    searchTaskId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 巡逻任务开始时间
        [JsonProperty]
        long searchStartTime;
        [JsonIgnore]
        public long SearchStartTime
        {
            get
            {
                return searchStartTime;
            }
            set
            {
                if(searchStartTime != value)
                {
                    searchStartTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 巡逻任务结束时间
        [JsonProperty]
        long searchEndTime;
        [JsonIgnore]
        public long SearchEndTime
        {
            get
            {
                return searchEndTime;
            }
            set
            {
                if(searchEndTime != value)
                {
                    searchEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 巡逻任务道具数量
        [JsonProperty]
        int searchPropCount;
        [JsonIgnore]
        public int SearchPropCount
        {
            get
            {
                return searchPropCount;
            }
            set
            {
                if(searchPropCount != value)
                {
                    searchPropCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
         
        // 每日任务存档
        [JsonProperty]
        StorageDictionary<long,StorageKeepPetDailyTask> dailyTaskDictionary = new StorageDictionary<long,StorageKeepPetDailyTask>();
        [JsonIgnore]
        public StorageDictionary<long,StorageKeepPetDailyTask> DailyTaskDictionary
        {
            get
            {
                return dailyTaskDictionary;
            }
        }
        // ---------------------------------//
        
        // 每日任务刷新时间
        [JsonProperty]
        long dailyTaskRefreshTime;
        [JsonIgnore]
        public long DailyTaskRefreshTime
        {
            get
            {
                return dailyTaskRefreshTime;
            }
            set
            {
                if(dailyTaskRefreshTime != value)
                {
                    dailyTaskRefreshTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 线索
        [JsonProperty]
        StorageDictionary<int,int> clueDictionary = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> ClueDictionary
        {
            get
            {
                return clueDictionary;
            }
        }
        // ---------------------------------//
        
        // 上次唤醒狗子的时间
        [JsonProperty]
        long lastWakeUpTime;
        [JsonIgnore]
        public long LastWakeUpTime
        {
            get
            {
                return lastWakeUpTime;
            }
            set
            {
                if(lastWakeUpTime != value)
                {
                    lastWakeUpTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 皮肤名
        [JsonProperty]
        string skinName = "";
        [JsonIgnore]
        public string SkinName
        {
            get
            {
                return skinName;
            }
            set
            {
                if(skinName != value)
                {
                    skinName = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}