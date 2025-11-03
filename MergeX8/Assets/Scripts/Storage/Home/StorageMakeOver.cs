/************************************************
 * Storage class : StorageMakeOver
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMakeOver : StorageBase
    {
        
        // 是否完成
        [JsonProperty]
        bool isFinish;
        [JsonIgnore]
        public bool IsFinish
        {
            get
            {
                return isFinish;
            }
            set
            {
                if(isFinish != value)
                {
                    isFinish = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否开始
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
        
        // 对应LEVEL表索引 第几个
        [JsonProperty]
        int levelIndex;
        [JsonIgnore]
        public int LevelIndex
        {
            get
            {
                return levelIndex;
            }
            set
            {
                if(levelIndex != value)
                {
                    levelIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成情况
        [JsonProperty]
        StorageDictionary<int,bool> finishInfo = new StorageDictionary<int,bool>();
        [JsonIgnore]
        public StorageDictionary<int,bool> FinishInfo
        {
            get
            {
                return finishInfo;
            }
        }
        // ---------------------------------//
        
        // 当前进度
        [JsonProperty]
        StorageDictionary<int,int> bodyPartStep = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> BodyPartStep
        {
            get
            {
                return bodyPartStep;
            }
        }
        // ---------------------------------//
        
    }
}