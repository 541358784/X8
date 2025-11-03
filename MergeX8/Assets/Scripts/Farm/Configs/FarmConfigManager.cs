
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.Farm
{
    public partial class FarmConfigManager : TableSingleton<FarmConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableFarmSetting> TableFarmSettingList;
        public List<TableFarmSale> TableFarmSaleList;
        public List<TableFarmProp> TableFarmPropList;
        public List<TableFarmSeed> TableFarmSeedList;
        public List<TableFarmTree> TableFarmTreeList;
        public List<TableFarmGround> TableFarmGroundList;
        public List<TableFarmAnimal> TableFarmAnimalList;
        public List<TableFarmMachine> TableFarmMachineList;
        public List<TableFarmMachineOrder> TableFarmMachineOrderList;
        public List<TableFarmWarehouse> TableFarmWarehouseList;
        public List<TableFarmLevel> TableFarmLevelList;
        public List<TableFarmProduct> TableFarmProductList;
        public List<TableFarmOrderFix> TableFarmOrderFixList;
        public List<TableFarmOrderItem> TableFarmOrderItemList;
        public List<TableFarmOrderSlot> TableFarmOrderSlotList;
        public List<TableFarmOrderWeight> TableFarmOrderWeightList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableFarmSetting)] = "TableFarmSetting",
            [typeof(TableFarmSale)] = "TableFarmSale",
            [typeof(TableFarmProp)] = "TableFarmProp",
            [typeof(TableFarmSeed)] = "TableFarmSeed",
            [typeof(TableFarmTree)] = "TableFarmTree",
            [typeof(TableFarmGround)] = "TableFarmGround",
            [typeof(TableFarmAnimal)] = "TableFarmAnimal",
            [typeof(TableFarmMachine)] = "TableFarmMachine",
            [typeof(TableFarmMachineOrder)] = "TableFarmMachineOrder",
            [typeof(TableFarmWarehouse)] = "TableFarmWarehouse",
            [typeof(TableFarmLevel)] = "TableFarmLevel",
            [typeof(TableFarmProduct)] = "TableFarmProduct",
            [typeof(TableFarmOrderFix)] = "TableFarmOrderFix",
            [typeof(TableFarmOrderItem)] = "TableFarmOrderItem",
            [typeof(TableFarmOrderSlot)] = "TableFarmOrderSlot",
            [typeof(TableFarmOrderWeight)] = "TableFarmOrderWeight",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("farmsetting")) return false;
            if (!table.ContainsKey("farmsale")) return false;
            if (!table.ContainsKey("farmprop")) return false;
            if (!table.ContainsKey("farmseed")) return false;
            if (!table.ContainsKey("farmtree")) return false;
            if (!table.ContainsKey("farmground")) return false;
            if (!table.ContainsKey("farmanimal")) return false;
            if (!table.ContainsKey("farmmachine")) return false;
            if (!table.ContainsKey("farmmachineorder")) return false;
            if (!table.ContainsKey("farmwarehouse")) return false;
            if (!table.ContainsKey("farmlevel")) return false;
            if (!table.ContainsKey("farmproduct")) return false;
            if (!table.ContainsKey("farmorderfix")) return false;
            if (!table.ContainsKey("farmorderitem")) return false;
            if (!table.ContainsKey("farmorderslot")) return false;
            if (!table.ContainsKey("farmorderweight")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableFarmSetting": cfg = TableFarmSettingList as List<T>; break;
                case "TableFarmSale": cfg = TableFarmSaleList as List<T>; break;
                case "TableFarmProp": cfg = TableFarmPropList as List<T>; break;
                case "TableFarmSeed": cfg = TableFarmSeedList as List<T>; break;
                case "TableFarmTree": cfg = TableFarmTreeList as List<T>; break;
                case "TableFarmGround": cfg = TableFarmGroundList as List<T>; break;
                case "TableFarmAnimal": cfg = TableFarmAnimalList as List<T>; break;
                case "TableFarmMachine": cfg = TableFarmMachineList as List<T>; break;
                case "TableFarmMachineOrder": cfg = TableFarmMachineOrderList as List<T>; break;
                case "TableFarmWarehouse": cfg = TableFarmWarehouseList as List<T>; break;
                case "TableFarmLevel": cfg = TableFarmLevelList as List<T>; break;
                case "TableFarmProduct": cfg = TableFarmProductList as List<T>; break;
                case "TableFarmOrderFix": cfg = TableFarmOrderFixList as List<T>; break;
                case "TableFarmOrderItem": cfg = TableFarmOrderItemList as List<T>; break;
                case "TableFarmOrderSlot": cfg = TableFarmOrderSlotList as List<T>; break;
                case "TableFarmOrderWeight": cfg = TableFarmOrderWeightList as List<T>; break;
                default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
            }
            return cfg;
        }
        public void InitConfig(String configJson = null)
        {
            ConfigFromRemote = true;
            Hashtable table = null;
            if (!string.IsNullOrEmpty(configJson))
                table = JsonConvert.DeserializeObject<Hashtable>(configJson);

            if (table == null || !CheckTable(table))
            {
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/farm/farmconfig", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/farm/farmconfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableFarmSettingList = JsonConvert.DeserializeObject<List<TableFarmSetting>>(JsonConvert.SerializeObject(table["farmsetting"]));
            TableFarmSaleList = JsonConvert.DeserializeObject<List<TableFarmSale>>(JsonConvert.SerializeObject(table["farmsale"]));
            TableFarmPropList = JsonConvert.DeserializeObject<List<TableFarmProp>>(JsonConvert.SerializeObject(table["farmprop"]));
            TableFarmSeedList = JsonConvert.DeserializeObject<List<TableFarmSeed>>(JsonConvert.SerializeObject(table["farmseed"]));
            TableFarmTreeList = JsonConvert.DeserializeObject<List<TableFarmTree>>(JsonConvert.SerializeObject(table["farmtree"]));
            TableFarmGroundList = JsonConvert.DeserializeObject<List<TableFarmGround>>(JsonConvert.SerializeObject(table["farmground"]));
            TableFarmAnimalList = JsonConvert.DeserializeObject<List<TableFarmAnimal>>(JsonConvert.SerializeObject(table["farmanimal"]));
            TableFarmMachineList = JsonConvert.DeserializeObject<List<TableFarmMachine>>(JsonConvert.SerializeObject(table["farmmachine"]));
            TableFarmMachineOrderList = JsonConvert.DeserializeObject<List<TableFarmMachineOrder>>(JsonConvert.SerializeObject(table["farmmachineorder"]));
            TableFarmWarehouseList = JsonConvert.DeserializeObject<List<TableFarmWarehouse>>(JsonConvert.SerializeObject(table["farmwarehouse"]));
            TableFarmLevelList = JsonConvert.DeserializeObject<List<TableFarmLevel>>(JsonConvert.SerializeObject(table["farmlevel"]));
            TableFarmProductList = JsonConvert.DeserializeObject<List<TableFarmProduct>>(JsonConvert.SerializeObject(table["farmproduct"]));
            TableFarmOrderFixList = JsonConvert.DeserializeObject<List<TableFarmOrderFix>>(JsonConvert.SerializeObject(table["farmorderfix"]));
            TableFarmOrderItemList = JsonConvert.DeserializeObject<List<TableFarmOrderItem>>(JsonConvert.SerializeObject(table["farmorderitem"]));
            TableFarmOrderSlotList = JsonConvert.DeserializeObject<List<TableFarmOrderSlot>>(JsonConvert.SerializeObject(table["farmorderslot"]));
            TableFarmOrderWeightList = JsonConvert.DeserializeObject<List<TableFarmOrderWeight>>(JsonConvert.SerializeObject(table["farmorderweight"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}