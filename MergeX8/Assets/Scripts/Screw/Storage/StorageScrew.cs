/************************************************
 * Storage class : StorageScrew
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageScrew : StorageBase
    {
        
        // 当前关卡ID
        [JsonProperty]
        int mainLevelIndex;
        [JsonIgnore]
        public int MainLevelIndex
        {
            get
            {
                return mainLevelIndex;
            }
            set
            {
                if(mainLevelIndex != value)
                {
                    mainLevelIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
         
        // 玩家资源
        [JsonProperty]
        StorageDictionary<string,StorageUserRes> userRes = new StorageDictionary<string,StorageUserRes>();
        [JsonIgnore]
        public StorageDictionary<string,StorageUserRes> UserRes
        {
            get
            {
                return userRes;
            }
        }
        // ---------------------------------//
        
        // 一些KEY VALUE 记录 
        [JsonProperty]
        StorageDictionary<string,string> recored = new StorageDictionary<string,string>();
        [JsonIgnore]
        public StorageDictionary<string,string> Recored
        {
            get
            {
                return recored;
            }
        }
        // ---------------------------------//
        
        // 一些时间处理
        [JsonProperty]
        StorageDictionary<int,long> buff = new StorageDictionary<int,long>();
        [JsonIgnore]
        public StorageDictionary<int,long> Buff
        {
            get
            {
                return buff;
            }
        }
        // ---------------------------------//
        
        // 体力结构
        [JsonProperty]
        StorageEnergy energy = new StorageEnergy();
        [JsonIgnore]
        public StorageEnergy Energy
        {
            get
            {
                return energy;
            }
        }
        // ---------------------------------//
        
        // 道具弹窗
        [JsonProperty]
        bool isBreakBodyPopGuide;
        [JsonIgnore]
        public bool IsBreakBodyPopGuide
        {
            get
            {
                return isBreakBodyPopGuide;
            }
            set
            {
                if(isBreakBodyPopGuide != value)
                {
                    isBreakBodyPopGuide = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 道具弹窗
        [JsonProperty]
        bool isTwoTaskPopGuide;
        [JsonIgnore]
        public bool IsTwoTaskPopGuide
        {
            get
            {
                return isTwoTaskPopGuide;
            }
            set
            {
                if(isTwoTaskPopGuide != value)
                {
                    isTwoTaskPopGuide = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 道具弹窗
        [JsonProperty]
        bool isExtraSlotPopGuide;
        [JsonIgnore]
        public bool IsExtraSlotPopGuide
        {
            get
            {
                return isExtraSlotPopGuide;
            }
            set
            {
                if(isExtraSlotPopGuide != value)
                {
                    isExtraSlotPopGuide = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 每日礼包
        [JsonProperty]
        StorageScrewDailyPackage dailyPackage = new StorageScrewDailyPackage();
        [JsonIgnore]
        public StorageScrewDailyPackage DailyPackage
        {
            get
            {
                return dailyPackage;
            }
        }
        // ---------------------------------//
        
    }
}