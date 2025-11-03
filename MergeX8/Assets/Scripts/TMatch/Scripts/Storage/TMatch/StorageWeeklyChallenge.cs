/************************************************
 * Storage class : StorageWeeklyChallenge
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageWeeklyChallenge : DragonU3DSDK.Storage.StorageBase
    {
        
        // 当前周的ID
        [JsonProperty]
        int curWeekId;
        [JsonIgnore]
        public int CurWeekId
        {
            get
            {
                return curWeekId;
            }
            set
            {
                if(curWeekId != value)
                {
                    curWeekId = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前等级
        [JsonProperty]
        int curLevel;
        [JsonIgnore]
        public int CurLevel
        {
            get
            {
                return curLevel;
            }
            set
            {
                if(curLevel != value)
                {
                    curLevel = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前收集的物品数量
        [JsonProperty]
        int curCollectItemNum;
        [JsonIgnore]
        public int CurCollectItemNum
        {
            get
            {
                return curCollectItemNum;
            }
            set
            {
                if(curCollectItemNum != value)
                {
                    curCollectItemNum = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前周领奖次数
        [JsonProperty]
        int curClaimCnt;
        [JsonIgnore]
        public int CurClaimCnt
        {
            get
            {
                return curClaimCnt;
            }
            set
            {
                if(curClaimCnt != value)
                {
                    curClaimCnt = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否已经主动弹出
        [JsonProperty]
        bool popup;
        [JsonIgnore]
        public bool Popup
        {
            get
            {
                return popup;
            }
            set
            {
                if(popup != value)
                {
                    popup = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}