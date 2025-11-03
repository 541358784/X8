
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.FarmTimeOrder
{
    public partial class FarmTimeOrderConfigManager : TableSingleton<FarmTimeOrderConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableFarmTimeOrderGroup> TableFarmTimeOrderGroupList;
        public List<TableFarmTimeOrderConfig> TableFarmTimeOrderConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableFarmTimeOrderGroup)] = "TableFarmTimeOrderGroup",
            [typeof(TableFarmTimeOrderConfig)] = "TableFarmTimeOrderConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("farmtimeordergroup")) return false;
            if (!table.ContainsKey("farmtimeorderconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableFarmTimeOrderGroup": cfg = TableFarmTimeOrderGroupList as List<T>; break;
                case "TableFarmTimeOrderConfig": cfg = TableFarmTimeOrderConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/FarmTimeOrder/", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/FarmTimeOrder/ error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableFarmTimeOrderGroupList = JsonConvert.DeserializeObject<List<TableFarmTimeOrderGroup>>(JsonConvert.SerializeObject(table["farmtimeordergroup"]));
            TableFarmTimeOrderConfigList = JsonConvert.DeserializeObject<List<TableFarmTimeOrderConfig>>(JsonConvert.SerializeObject(table["farmtimeorderconfig"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}