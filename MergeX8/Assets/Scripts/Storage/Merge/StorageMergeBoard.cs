/************************************************
 * Storage class : StorageMergeBoard
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMergeBoard : StorageBase
    {
        
        // 
        [JsonProperty]
        int width;
        [JsonIgnore]
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                if(width != value)
                {
                    width = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        int height;
        [JsonIgnore]
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                if(height != value)
                {
                    height = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        StorageList<StorageMergeItem> items = new StorageList<StorageMergeItem>();
        [JsonIgnore]
        public StorageList<StorageMergeItem> Items
        {
            get
            {
                return items;
            }
        }
        // ---------------------------------//
        
        // 背包物品
        [JsonProperty]
        StorageList<StorageMergeItem> bags = new StorageList<StorageMergeItem>();
        [JsonIgnore]
        public StorageList<StorageMergeItem> Bags
        {
            get
            {
                return bags;
            }
        }
        // ---------------------------------//
        
        // 背包解锁数量
        [JsonProperty]
        int bagCapacity;
        [JsonIgnore]
        public int BagCapacity
        {
            get
            {
                return bagCapacity;
            }
            set
            {
                if(bagCapacity != value)
                {
                    bagCapacity = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 建筑背包物品
        [JsonProperty]
        StorageList<StorageMergeItem> buildingBags = new StorageList<StorageMergeItem>();
        [JsonIgnore]
        public StorageList<StorageMergeItem> BuildingBags
        {
            get
            {
                return buildingBags;
            }
        }
        // ---------------------------------//
        
        // 建筑背包解锁数量
        [JsonProperty]
        int buildingBagCapacity;
        [JsonIgnore]
        public int BuildingBagCapacity
        {
            get
            {
                return buildingBagCapacity;
            }
            set
            {
                if(buildingBagCapacity != value)
                {
                    buildingBagCapacity = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 奖励暂存
        [JsonProperty]
        StorageList<StorageMergeItem> rewards = new StorageList<StorageMergeItem>();
        [JsonIgnore]
        public StorageList<StorageMergeItem> Rewards
        {
            get
            {
                return rewards;
            }
        }
        // ---------------------------------//
        
        // 每个ITEM 合成次数
        [JsonProperty]
        StorageDictionary<int,int> mergeCounts = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> MergeCounts
        {
            get
            {
                return mergeCounts;
            }
        }
        // ---------------------------------//
        
        // 合成总次数
        [JsonProperty]
        int mergeCount;
        [JsonIgnore]
        public int MergeCount
        {
            get
            {
                return mergeCount;
            }
            set
            {
                if(mergeCount != value)
                {
                    mergeCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成不能解锁挂点任务的次数
        [JsonProperty]
        int finishTaskCount;
        [JsonIgnore]
        public int FinishTaskCount
        {
            get
            {
                return finishTaskCount;
            }
            set
            {
                if(finishTaskCount != value)
                {
                    finishTaskCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 装修次数
        [JsonProperty]
        int decoCount;
        [JsonIgnore]
        public int DecoCount
        {
            get
            {
                return decoCount;
            }
            set
            {
                if(decoCount != value)
                {
                    decoCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 合成链产出次数
        [JsonProperty]
        StorageDictionary<int,int> lineProducts = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> LineProducts
        {
            get
            {
                return lineProducts;
            }
        }
        // ---------------------------------//
        
        // VIP格子
        [JsonProperty]
        StorageList<StorageMergeItem> vipBags = new StorageList<StorageMergeItem>();
        [JsonIgnore]
        public StorageList<StorageMergeItem> VipBags
        {
            get
            {
                return vipBags;
            }
        }
        // ---------------------------------//
        
        // 今日产生气泡次数
        [JsonProperty]
        int todayBubbleCount;
        [JsonIgnore]
        public int TodayBubbleCount
        {
            get
            {
                return todayBubbleCount;
            }
            set
            {
                if(todayBubbleCount != value)
                {
                    todayBubbleCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最后一次产出气泡的时间
        [JsonProperty]
        ulong lastProductBubbleTime;
        [JsonIgnore]
        public ulong LastProductBubbleTime
        {
            get
            {
                return lastProductBubbleTime;
            }
            set
            {
                if(lastProductBubbleTime != value)
                {
                    lastProductBubbleTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 今日气泡产生信息(ID,COUNT)
        [JsonProperty]
        StorageDictionary<int,int> todayBubbleInfo = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> TodayBubbleInfo
        {
            get
            {
                return todayBubbleInfo;
            }
        }
        // ---------------------------------//
        
        // 无限产出结束时间
        [JsonProperty]
        long unlimtProductEndTime;
        [JsonIgnore]
        public long UnlimtProductEndTime
        {
            get
            {
                return unlimtProductEndTime;
            }
            set
            {
                if(unlimtProductEndTime != value)
                {
                    unlimtProductEndTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}