/************************************************
 * Storage class : StorageFarm
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageFarm : StorageBase
    {
        
        // 是否进入
        [JsonProperty]
        bool isEnter;
        [JsonIgnore]
        public bool IsEnter
        {
            get
            {
                return isEnter;
            }
            set
            {
                if(isEnter != value)
                {
                    isEnter = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前等级
        [JsonProperty]
        int level;
        [JsonIgnore]
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if(level != value)
                {
                    level = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前经验
        [JsonProperty]
        int exp;
        [JsonIgnore]
        public int Exp
        {
            get
            {
                return exp;
            }
            set
            {
                if(exp != value)
                {
                    exp = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 仓库格子数量
        [JsonProperty]
        int warehouseNum;
        [JsonIgnore]
        public int WarehouseNum
        {
            get
            {
                return warehouseNum;
            }
            set
            {
                if(warehouseNum != value)
                {
                    warehouseNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前购买仓库的ID
        [JsonProperty]
        int warehouseId;
        [JsonIgnore]
        public int WarehouseId
        {
            get
            {
                return warehouseId;
            }
            set
            {
                if(warehouseId != value)
                {
                    warehouseId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 产出物数量
        [JsonProperty]
        StorageDictionary<int,int> productItems = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> ProductItems
        {
            get
            {
                return productItems;
            }
        }
        // ---------------------------------//
         
        // 机器数据
        [JsonProperty]
        StorageDictionary<int,StorageMachine> machine = new StorageDictionary<int,StorageMachine>();
        [JsonIgnore]
        public StorageDictionary<int,StorageMachine> Machine
        {
            get
            {
                return machine;
            }
        }
        // ---------------------------------//
         
        // 动物数据
        [JsonProperty]
        StorageDictionary<int,StorageAnimal> animal = new StorageDictionary<int,StorageAnimal>();
        [JsonIgnore]
        public StorageDictionary<int,StorageAnimal> Animal
        {
            get
            {
                return animal;
            }
        }
        // ---------------------------------//
         
        // 树数据
        [JsonProperty]
        StorageDictionary<int,StorageTree> tree = new StorageDictionary<int,StorageTree>();
        [JsonIgnore]
        public StorageDictionary<int,StorageTree> Tree
        {
            get
            {
                return tree;
            }
        }
        // ---------------------------------//
         
        // 地块数据
        [JsonProperty]
        StorageDictionary<int,StorageGround> ground = new StorageDictionary<int,StorageGround>();
        [JsonIgnore]
        public StorageDictionary<int,StorageGround> Ground
        {
            get
            {
                return ground;
            }
        }
        // ---------------------------------//
        
        // 任务数据
        [JsonProperty]
        StorageFarmOrder order = new StorageFarmOrder();
        [JsonIgnore]
        public StorageFarmOrder Order
        {
            get
            {
                return order;
            }
        }
        // ---------------------------------//
        
    }
}