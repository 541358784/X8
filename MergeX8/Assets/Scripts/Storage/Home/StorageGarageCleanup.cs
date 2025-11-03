/************************************************
 * Storage class : StorageGarageCleanup
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageGarageCleanup : StorageBase
    {
        
        // 当前阶段
        [JsonProperty]
        int level;
        [JsonIgnore]
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if(level != value)
                {
                    level = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 棋盘
        [JsonProperty]
        StorageList<StorageGarageCleanupBoard> boards = new StorageList<StorageGarageCleanupBoard>();
        [JsonIgnore]
        public StorageList<StorageGarageCleanupBoard> Boards
        {
            get
            {
                return boards;
            }
        }
        // ---------------------------------//
        
        // 是否显示开始页面
        [JsonProperty]
        bool isShowStart;
        [JsonIgnore]
        public bool IsShowStart
        {
            get
            {
                return isShowStart;
            }
            set
            {
                if(isShowStart != value)
                {
                    isShowStart = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否活动结束
        [JsonProperty]
        bool isActivityEnd;
        [JsonIgnore]
        public bool IsActivityEnd
        {
            get
            {
                return isActivityEnd;
            }
            set
            {
                if(isActivityEnd != value)
                {
                    isActivityEnd = value;
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
        
        // 礼包购买次数
        [JsonProperty]
        int packBuyCount;
        [JsonIgnore]
        public int PackBuyCount
        {
            get
            {
                return packBuyCount;
            }
            set
            {
                if(packBuyCount != value)
                {
                    packBuyCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 等级组
        [JsonProperty]
        int levelGroup;
        [JsonIgnore]
        public int LevelGroup
        {
            get
            {
                return levelGroup;
            }
            set
            {
                if(levelGroup != value)
                {
                    levelGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}