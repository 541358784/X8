
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.LimitOrderLine
{
    public partial class LimitOrderLineConfigManager : TableSingleton<LimitOrderLineConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableTimeOrderLineSetting> TableTimeOrderLineSettingList;
        public List<TableTimeOrderLineGroup> TableTimeOrderLineGroupList;
        public List<TableTimeOrderLineConfig> TableTimeOrderLineConfigList;
        public List<TableTimeOrderLineGift> TableTimeOrderLineGiftList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableTimeOrderLineSetting)] = "TableTimeOrderLineSetting",
            [typeof(TableTimeOrderLineGroup)] = "TableTimeOrderLineGroup",
            [typeof(TableTimeOrderLineConfig)] = "TableTimeOrderLineConfig",
            [typeof(TableTimeOrderLineGift)] = "TableTimeOrderLineGift",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("timeorderlinesetting")) return false;
            if (!table.ContainsKey("timeorderlinegroup")) return false;
            if (!table.ContainsKey("timeorderlineconfig")) return false;
            if (!table.ContainsKey("timeorderlinegift")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableTimeOrderLineSetting": cfg = TableTimeOrderLineSettingList as List<T>; break;
                case "TableTimeOrderLineGroup": cfg = TableTimeOrderLineGroupList as List<T>; break;
                case "TableTimeOrderLineConfig": cfg = TableTimeOrderLineConfigList as List<T>; break;
                case "TableTimeOrderLineGift": cfg = TableTimeOrderLineGiftList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/LimitOrderLine/config", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/LimitOrderLine/config error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableTimeOrderLineSettingList = JsonConvert.DeserializeObject<List<TableTimeOrderLineSetting>>(JsonConvert.SerializeObject(table["timeorderlinesetting"]));
            TableTimeOrderLineGroupList = JsonConvert.DeserializeObject<List<TableTimeOrderLineGroup>>(JsonConvert.SerializeObject(table["timeorderlinegroup"]));
            TableTimeOrderLineConfigList = JsonConvert.DeserializeObject<List<TableTimeOrderLineConfig>>(JsonConvert.SerializeObject(table["timeorderlineconfig"]));
            TableTimeOrderLineGiftList = JsonConvert.DeserializeObject<List<TableTimeOrderLineGift>>(JsonConvert.SerializeObject(table["timeorderlinegift"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}