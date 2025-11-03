/************************************************
 * Storage class : StorageTaskStatisticsDetailed
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTaskStatisticsDetailed : StorageBase
    {
        
        // 任务详细信息
        [JsonProperty]
        StorageDictionary<int,int> detailed = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> Detailed
        {
            get
            {
                return detailed;
            }
        }
        // ---------------------------------//
        
        // 登陆时间
        [JsonProperty]
        long loginTime;
        [JsonIgnore]
        public long LoginTime
        {
            get
            {
                return loginTime;
            }
            set
            {
                if(loginTime != value)
                {
                    loginTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}