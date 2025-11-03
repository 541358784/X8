/************************************************
 * Storage class : StoragePigBank
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StoragePigBank : StorageBase
    {
         
        // 小猪IDS
        [JsonProperty]
        StorageDictionary<string,StoragePigBankId> pigBankIds = new StorageDictionary<string,StoragePigBankId>();
        [JsonIgnore]
        public StorageDictionary<string,StoragePigBankId> PigBankIds
        {
            get
            {
                return pigBankIds;
            }
        }
        // ---------------------------------//
        
        // 当前小猪的索引
        [JsonProperty]
        StorageDictionary<string,int> indexs = new StorageDictionary<string,int>();
        [JsonIgnore]
        public StorageDictionary<string,int> Indexs
        {
            get
            {
                return indexs;
            }
        }
        // ---------------------------------//
        
        // 当前小猪值
        [JsonProperty]
        StorageDictionary<string,int> pigBankValue = new StorageDictionary<string,int>();
        [JsonIgnore]
        public StorageDictionary<string,int> PigBankValue
        {
            get
            {
                return pigBankValue;
            }
        }
        // ---------------------------------//
        
        // 小猪是否自动弹出
        [JsonProperty]
        StorageDictionary<string,bool> pigBankAutoPop = new StorageDictionary<string,bool>();
        [JsonIgnore]
        public StorageDictionary<string,bool> PigBankAutoPop
        {
            get
            {
                return pigBankAutoPop;
            }
        }
        // ---------------------------------//
         
        // 购买过的小猪ID
        [JsonProperty]
        StorageDictionary<string,StoragePigBankId> buyPigBankIds = new StorageDictionary<string,StoragePigBankId>();
        [JsonIgnore]
        public StorageDictionary<string,StoragePigBankId> BuyPigBankIds
        {
            get
            {
                return buyPigBankIds;
            }
        }
        // ---------------------------------//
        
        // 忽略的ID
        [JsonProperty]
        StorageList<string> ignoreIds = new StorageList<string>();
        [JsonIgnore]
        public StorageList<string> IgnoreIds
        {
            get
            {
                return ignoreIds;
            }
        }
        // ---------------------------------//
        
        // 开始
        [JsonProperty]
        ulong localStartTime;
        [JsonIgnore]
        public ulong LocalStartTime
        {
            get
            {
                return localStartTime;
            }
            set
            {
                if(localStartTime != value)
                {
                    localStartTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 结束
        [JsonProperty]
        ulong localEndTime;
        [JsonIgnore]
        public ulong LocalEndTime
        {
            get
            {
                return localEndTime;
            }
            set
            {
                if(localEndTime != value)
                {
                    localEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}