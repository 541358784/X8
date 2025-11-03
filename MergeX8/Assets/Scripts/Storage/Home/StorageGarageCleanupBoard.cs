/************************************************
 * Storage class : StorageGarageCleanupBoard
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageGarageCleanupBoard : StorageBase
    {
        
        // 棋盘元素
        [JsonProperty]
        StorageList<StorageGarageCleanupBoardItem> items = new StorageList<StorageGarageCleanupBoardItem>();
        [JsonIgnore]
        public StorageList<StorageGarageCleanupBoardItem> Items
        {
            get
            {
                return items;
            }
        }
        // ---------------------------------//
        
        // 是否展开
        [JsonProperty]
        bool isReveal;
        [JsonIgnore]
        public bool IsReveal
        {
            get
            {
                return isReveal;
            }
            set
            {
                if(isReveal != value)
                {
                    isReveal = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否开始（可能有钻石消耗）
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
        
    }
}