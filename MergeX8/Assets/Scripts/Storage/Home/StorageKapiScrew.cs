/************************************************
 * Storage class : StorageKapiScrew
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageKapiScrew : StorageBase
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
        
        // 生命值
        [JsonProperty]
        int life;
        [JsonIgnore]
        public int Life
        {
            get
            {
                return life;
            }
            set
            {
                if(life != value)
                {
                    life = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 生命值更新时间
        [JsonProperty]
        long lifeUpdateTime;
        [JsonIgnore]
        public long LifeUpdateTime
        {
            get
            {
                return lifeUpdateTime;
            }
            set
            {
                if(lifeUpdateTime != value)
                {
                    lifeUpdateTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 复活道具数量
        [JsonProperty]
        int rebornCount;
        [JsonIgnore]
        public int RebornCount
        {
            get
            {
                return rebornCount;
            }
            set
            {
                if(rebornCount != value)
                {
                    rebornCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 大等级
        [JsonProperty]
        int bigLevel;
        [JsonIgnore]
        public int BigLevel
        {
            get
            {
                return bigLevel;
            }
            set
            {
                if(bigLevel != value)
                {
                    bigLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 小等级
        [JsonProperty]
        int smallLevel;
        [JsonIgnore]
        public int SmallLevel
        {
            get
            {
                return smallLevel;
            }
            set
            {
                if(smallLevel != value)
                {
                    smallLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 正在玩的小等级
        [JsonProperty]
        int playingSmallLevel;
        [JsonIgnore]
        public int PlayingSmallLevel
        {
            get
            {
                return playingSmallLevel;
            }
            set
            {
                if(playingSmallLevel != value)
                {
                    playingSmallLevel = value;
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
        
        // 对手头像ID
        [JsonProperty]
        int enemyHeadIconId;
        [JsonIgnore]
        public int EnemyHeadIconId
        {
            get
            {
                return enemyHeadIconId;
            }
            set
            {
                if(enemyHeadIconId != value)
                {
                    enemyHeadIconId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 对手名字
        [JsonProperty]
        string enemyName = "";
        [JsonIgnore]
        public string EnemyName
        {
            get
            {
                return enemyName;
            }
            set
            {
                if(enemyName != value)
                {
                    enemyName = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否需要匹配新的对手
        [JsonProperty]
        bool changeEnemy;
        [JsonIgnore]
        public bool ChangeEnemy
        {
            get
            {
                return changeEnemy;
            }
            set
            {
                if(changeEnemy != value)
                {
                    changeEnemy = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}