/************************************************
 * Storage class : StorageAdConfigLocal
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageAdConfigLocal : StorageBase
    {
        
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
        
        // 初始化次数
        [JsonProperty]
        int initCount;
        [JsonIgnore]
        public int InitCount
        {
            get
            {
                return initCount;
            }
            set
            {
                if(initCount != value)
                {
                    initCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否新用户
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
        
        // 当前分组
        [JsonProperty]
        int curGroup;
        [JsonIgnore]
        public int CurGroup
        {
            get
            {
                return curGroup;
            }
            set
            {
                if(curGroup != value)
                {
                    curGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次登录时间时间
        [JsonProperty]
        long lastLoginTime;
        [JsonIgnore]
        public long LastLoginTime
        {
            get
            {
                return lastLoginTime;
            }
            set
            {
                if(lastLoginTime != value)
                {
                    lastLoginTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前降档次数
        [JsonProperty]
        int curLowTimes;
        [JsonIgnore]
        public int CurLowTimes
        {
            get
            {
                return curLowTimes;
            }
            set
            {
                if(curLowTimes != value)
                {
                    curLowTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 进入当前分组时间
        [JsonProperty]
        long enterCurGroupTime;
        [JsonIgnore]
        public long EnterCurGroupTime
        {
            get
            {
                return enterCurGroupTime;
            }
            set
            {
                if(enterCurGroupTime != value)
                {
                    enterCurGroupTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次付费时间
        [JsonProperty]
        long lastPayTime;
        [JsonIgnore]
        public long LastPayTime
        {
            get
            {
                return lastPayTime;
            }
            set
            {
                if(lastPayTime != value)
                {
                    lastPayTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 换组相关判断数据
        [JsonProperty]
        StorageAdJudgingData judgingData = new StorageAdJudgingData();
        [JsonIgnore]
        public StorageAdJudgingData JudgingData
        {
            get
            {
                return judgingData;
            }
        }
        // ---------------------------------//
        
        // 往前30天内付费金额数据
        [JsonProperty]
        StorageList<float> lastPayData = new StorageList<float>();
        [JsonIgnore]
        public StorageList<float> LastPayData
        {
            get
            {
                return lastPayData;
            }
        }
        // ---------------------------------//
        
        // 当天付费金额数据
        [JsonProperty]
        float curDayPay;
        [JsonIgnore]
        public float CurDayPay
        {
            get
            {
                return curDayPay;
            }
            set
            {
                if(curDayPay != value)
                {
                    curDayPay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当天RV播放次数
        [JsonProperty]
        int curDayRvNum;
        [JsonIgnore]
        public int CurDayRvNum
        {
            get
            {
                return curDayRvNum;
            }
            set
            {
                if(curDayRvNum != value)
                {
                    curDayRvNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当天插屏播放次数
        [JsonProperty]
        int curDayInNum;
        [JsonIgnore]
        public int CurDayInNum
        {
            get
            {
                return curDayInNum;
            }
            set
            {
                if(curDayInNum != value)
                {
                    curDayInNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 往前30个活跃天内播放RV次数记录
        [JsonProperty]
        StorageList<int> lastPlayRvNum = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> LastPlayRvNum
        {
            get
            {
                return lastPlayRvNum;
            }
        }
        // ---------------------------------//
        
        // 往前30个活跃天内播放IN次数记录
        [JsonProperty]
        StorageList<int> lastPlayInNum = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> LastPlayInNum
        {
            get
            {
                return lastPlayInNum;
            }
        }
        // ---------------------------------//
        
    }
}