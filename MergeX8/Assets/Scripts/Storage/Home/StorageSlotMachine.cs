/************************************************
 * Storage class : StorageSlotMachine
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageSlotMachine : StorageBase
    {
        
        // 有未完成的牌面
        [JsonProperty]
        bool hasUnCollectResult;
        [JsonIgnore]
        public bool HasUnCollectResult
        {
            get
            {
                return hasUnCollectResult;
            }
            set
            {
                if(hasUnCollectResult != value)
                {
                    hasUnCollectResult = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 重转次数
        [JsonProperty]
        int reSpinTimes;
        [JsonIgnore]
        public int ReSpinTimes
        {
            get
            {
                return reSpinTimes;
            }
            set
            {
                if(reSpinTimes != value)
                {
                    reSpinTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 结果配置和轮带对应关系
        [JsonProperty]
        StorageList<int> resultConfigList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> ResultConfigList
        {
            get
            {
                return resultConfigList;
            }
        }
        // ---------------------------------//
        
        // 轮带对应的图标
        [JsonProperty]
        StorageList<int> elementIndexList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> ElementIndexList
        {
            get
            {
                return elementIndexList;
            }
        }
        // ---------------------------------//
        
        // SPIN次数
        [JsonProperty]
        int spinCount;
        [JsonIgnore]
        public int SpinCount
        {
            get
            {
                return spinCount;
            }
            set
            {
                if(spinCount != value)
                {
                    spinCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否弹出开始弹窗
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
        
        // 初始结果
        [JsonProperty]
        StorageList<int> initElementIndexList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> InitElementIndexList
        {
            get
            {
                return initElementIndexList;
            }
        }
        // ---------------------------------//
        
    }
}