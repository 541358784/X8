/************************************************
 * Storage class : StorageNewNewIceBreakPack
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageNewNewIceBreakPack : StorageBase
    {
        
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
        
        // 已收集的奖励
        [JsonProperty]
        StorageList<int> collectState = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CollectState
        {
            get
            {
                return collectState;
            }
        }
        // ---------------------------------//
        
        // 是否购买
        [JsonProperty]
        bool buyState;
        [JsonIgnore]
        public bool BuyState
        {
            get
            {
                return buyState;
            }
            set
            {
                if(buyState != value)
                {
                    buyState = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否弹出过结束弹窗
        [JsonProperty]
        bool showEndView;
        [JsonIgnore]
        public bool ShowEndView
        {
            get
            {
                return showEndView;
            }
            set
            {
                if(showEndView != value)
                {
                    showEndView = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否是新用户
        [JsonProperty]
        bool isNewUser;
        [JsonIgnore]
        public bool IsNewUser
        {
            get
            {
                return isNewUser;
            }
            set
            {
                if(isNewUser != value)
                {
                    isNewUser = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
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
        
        // 是否初始化
        [JsonProperty]
        bool isInit;
        [JsonIgnore]
        public bool IsInit
        {
            get
            {
                return isInit;
            }
            set
            {
                if(isInit != value)
                {
                    isInit = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}