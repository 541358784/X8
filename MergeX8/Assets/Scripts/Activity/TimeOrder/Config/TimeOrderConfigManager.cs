
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.TimeOrder
{
    public partial class TimeOrderConfigManager : TableSingleton<TimeOrderConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableTimeOrderSetting> TableTimeOrderSettingList;
        public List<TableTimeOrderConfig> TableTimeOrderConfigList;
        public List<TableTimeOrderGift> TableTimeOrderGiftList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableTimeOrderSetting)] = "TableTimeOrderSetting",
            [typeof(TableTimeOrderConfig)] = "TableTimeOrderConfig",
            [typeof(TableTimeOrderGift)] = "TableTimeOrderGift",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("timeordersetting")) return false;
            if (!table.ContainsKey("timeorderconfig")) return false;
            if (!table.ContainsKey("timeordergift")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableTimeOrderSetting": cfg = TableTimeOrderSettingList as List<T>; break;
                case "TableTimeOrderConfig": cfg = TableTimeOrderConfigList as List<T>; break;
                case "TableTimeOrderGift": cfg = TableTimeOrderGiftList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/TimeOrder/config", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/TimeOrder/config error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableTimeOrderSettingList = JsonConvert.DeserializeObject<List<TableTimeOrderSetting>>(JsonConvert.SerializeObject(table["timeordersetting"]));
            TableTimeOrderConfigList = JsonConvert.DeserializeObject<List<TableTimeOrderConfig>>(JsonConvert.SerializeObject(table["timeorderconfig"]));
            TableTimeOrderGiftList = JsonConvert.DeserializeObject<List<TableTimeOrderGift>>(JsonConvert.SerializeObject(table["timeordergift"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}