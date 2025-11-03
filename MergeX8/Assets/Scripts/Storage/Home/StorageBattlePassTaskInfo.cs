/************************************************
 * Storage class : StorageBattlePassTaskInfo
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageBattlePassTaskInfo : StorageBase
    {
        
        // 任务ID
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
        
        // 总数量
        [JsonProperty]
        int totalNum;
        [JsonIgnore]
        public int TotalNum
        {
            get
            {
                return totalNum;
            }
            set
            {
                if(totalNum != value)
                {
                    totalNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否完成
        [JsonProperty]
        bool isComplete;
        [JsonIgnore]
        public bool IsComplete
        {
            get
            {
                return isComplete;
            }
            set
            {
                if(isComplete != value)
                {
                    isComplete = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成个数
        [JsonProperty]
        int completeNum;
        [JsonIgnore]
        public int CompleteNum
        {
            get
            {
                return completeNum;
            }
            set
            {
                if(completeNum != value)
                {
                    completeNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否展示过
        [JsonProperty]
        bool isShow;
        [JsonIgnore]
        public bool IsShow
        {
            get
            {
                return isShow;
            }
            set
            {
                if(isShow != value)
                {
                    isShow = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}