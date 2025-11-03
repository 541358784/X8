/************************************************
 * Storage class : StorageThemeDecoration
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageThemeDecoration : StorageBase
    {
        
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
        
        // 预热结束时间戳
        [JsonProperty]
        long preheatCompleteTime;
        [JsonIgnore]
        public long PreheatCompleteTime
        {
            get
            {
                return preheatCompleteTime;
            }
            set
            {
                if(preheatCompleteTime != value)
                {
                    preheatCompleteTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 提前结束时间戳
        [JsonProperty]
        long preEndTime;
        [JsonIgnore]
        public long PreEndTime
        {
            get
            {
                return preEndTime;
            }
            set
            {
                if(preEndTime != value)
                {
                    preEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 提前结束购买时间戳
        [JsonProperty]
        long preEndBuyTime;
        [JsonIgnore]
        public long PreEndBuyTime
        {
            get
            {
                return preEndBuyTime;
            }
            set
            {
                if(preEndBuyTime != value)
                {
                    preEndBuyTime = value;
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
        
        // 活动资源的文件名
        [JsonProperty]
        StorageList<string> activityResList = new StorageList<string>();
        [JsonIgnore]
        public StorageList<string> ActivityResList
        {
            get
            {
                return activityResList;
            }
        }
        // ---------------------------------//
        
        // 已购买的STOREITEMID
        [JsonProperty]
        StorageList<int> finishStoreItemList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> FinishStoreItemList
        {
            get
            {
                return finishStoreItemList;
            }
        }
        // ---------------------------------//
        
        // 活动分数
        [JsonProperty]
        int score;
        [JsonIgnore]
        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                if(score != value)
                {
                    score = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 排行榜存档
        [JsonProperty]
        StorageList<StorageThemeDecorationLeaderBoard> leaderBoardStorageList = new StorageList<StorageThemeDecorationLeaderBoard>();
        [JsonIgnore]
        public StorageList<StorageThemeDecorationLeaderBoard> LeaderBoardStorageList
        {
            get
            {
                return leaderBoardStorageList;
            }
        }
        // ---------------------------------//
        
        // 是否弹出过开始弹窗
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
        
        // 积累获得的总分数
        [JsonProperty]
        int totalScore;
        [JsonIgnore]
        public int TotalScore
        {
            get
            {
                return totalScore;
            }
            set
            {
                if(totalScore != value)
                {
                    totalScore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已经表演过解锁的商店等级
        [JsonProperty]
        StorageList<int> unLockStoreLevel = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> UnLockStoreLevel
        {
            get
            {
                return unLockStoreLevel;
            }
        }
        // ---------------------------------//
        
        // 是否购买额外时间
        [JsonProperty]
        bool isBuyPreEnd;
        [JsonIgnore]
        public bool IsBuyPreEnd
        {
            get
            {
                return isBuyPreEnd;
            }
            set
            {
                if(isBuyPreEnd != value)
                {
                    isBuyPreEnd = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已领取的商店完成奖励等级
        [JsonProperty]
        StorageList<int> collectStoreCompleteLevel = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CollectStoreCompleteLevel
        {
            get
            {
                return collectStoreCompleteLevel;
            }
        }
        // ---------------------------------//
        
        // 未领取的奖励
        [JsonProperty]
        StorageDictionary<int,int> unCollectRewards = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> UnCollectRewards
        {
            get
            {
                return unCollectRewards;
            }
        }
        // ---------------------------------//
        
        // 商店等级记录
        [JsonProperty]
        StorageList<int> collectStoreLevel = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CollectStoreLevel
        {
            get
            {
                return collectStoreLevel;
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