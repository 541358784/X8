/************************************************
 * Storage class : StorageCardCollectionCardPackage
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCardCollectionCardPackage : StorageBase
    {
        
        // 卡包ID
        [JsonProperty]
        int id;
        [JsonIgnore]
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if(id != value)
                {
                    id = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 卡包获取的时间
        [JsonProperty]
        ulong getTime;
        [JsonIgnore]
        public ulong GetTime
        {
            get
            {
                return getTime;
            }
            set
            {
                if(getTime != value)
                {
                    getTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 获取方式
        [JsonProperty]
        string source = "";
        [JsonIgnore]
        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                if(source != value)
                {
                    source = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}