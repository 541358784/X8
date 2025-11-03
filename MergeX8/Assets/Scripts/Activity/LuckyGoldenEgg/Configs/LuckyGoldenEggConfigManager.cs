
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.LuckyGoldenEgg
{
    public partial class LuckyGoldenEggConfigManager : TableSingleton<LuckyGoldenEggConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableLuckyGoldenEggActivityConfig> TableLuckyGoldenEggActivityConfigList;
        public List<TableLuckyGoldenEggSetting> TableLuckyGoldenEggSettingList;
        public List<TableLuckyGoldenEggLevelConfig> TableLuckyGoldenEggLevelConfigList;
        public List<TableLuckyGoldenEggStoreConfig> TableLuckyGoldenEggStoreConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableLuckyGoldenEggActivityConfig)] = "TableLuckyGoldenEggActivityConfig",
            [typeof(TableLuckyGoldenEggSetting)] = "TableLuckyGoldenEggSetting",
            [typeof(TableLuckyGoldenEggLevelConfig)] = "TableLuckyGoldenEggLevelConfig",
            [typeof(TableLuckyGoldenEggStoreConfig)] = "TableLuckyGoldenEggStoreConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("luckygoldeneggactivityconfig")) return false;
            if (!table.ContainsKey("luckygoldeneggsetting")) return false;
            if (!table.ContainsKey("luckygoldenegglevelconfig")) return false;
            if (!table.ContainsKey("luckygoldeneggstoreconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableLuckyGoldenEggActivityConfig": cfg = TableLuckyGoldenEggActivityConfigList as List<T>; break;
                case "TableLuckyGoldenEggSetting": cfg = TableLuckyGoldenEggSettingList as List<T>; break;
                case "TableLuckyGoldenEggLevelConfig": cfg = TableLuckyGoldenEggLevelConfigList as List<T>; break;
                case "TableLuckyGoldenEggStoreConfig": cfg = TableLuckyGoldenEggStoreConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/LuckyGoldenEgg/LuckyGoldenEggConfig", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/LuckyGoldenEgg/LuckyGoldenEggConfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableLuckyGoldenEggActivityConfigList = JsonConvert.DeserializeObject<List<TableLuckyGoldenEggActivityConfig>>(JsonConvert.SerializeObject(table["luckygoldeneggactivityconfig"]));
            TableLuckyGoldenEggSettingList = JsonConvert.DeserializeObject<List<TableLuckyGoldenEggSetting>>(JsonConvert.SerializeObject(table["luckygoldeneggsetting"]));
            TableLuckyGoldenEggLevelConfigList = JsonConvert.DeserializeObject<List<TableLuckyGoldenEggLevelConfig>>(JsonConvert.SerializeObject(table["luckygoldenegglevelconfig"]));
            TableLuckyGoldenEggStoreConfigList = JsonConvert.DeserializeObject<List<TableLuckyGoldenEggStoreConfig>>(JsonConvert.SerializeObject(table["luckygoldeneggstoreconfig"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}