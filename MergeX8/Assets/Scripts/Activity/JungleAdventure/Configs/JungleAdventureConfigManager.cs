
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.JungleAdventure
{
    public partial class JungleAdventureConfigManager : TableSingleton<JungleAdventureConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableJungleAdventureSetting> TableJungleAdventureSettingList;
        public List<TableJungleAdventureConfig> TableJungleAdventureConfigList;
        public List<TableJungleAdventureRewardConfig> TableJungleAdventureRewardConfigList;
        public List<TableJungleAdventureRankRewardConfig> TableJungleAdventureRankRewardConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableJungleAdventureSetting)] = "TableJungleAdventureSetting",
            [typeof(TableJungleAdventureConfig)] = "TableJungleAdventureConfig",
            [typeof(TableJungleAdventureRewardConfig)] = "TableJungleAdventureRewardConfig",
            [typeof(TableJungleAdventureRankRewardConfig)] = "TableJungleAdventureRankRewardConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("jungleadventuresetting")) return false;
            if (!table.ContainsKey("jungleadventureconfig")) return false;
            if (!table.ContainsKey("jungleadventurerewardconfig")) return false;
            if (!table.ContainsKey("jungleadventurerankrewardconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableJungleAdventureSetting": cfg = TableJungleAdventureSettingList as List<T>; break;
                case "TableJungleAdventureConfig": cfg = TableJungleAdventureConfigList as List<T>; break;
                case "TableJungleAdventureRewardConfig": cfg = TableJungleAdventureRewardConfigList as List<T>; break;
                case "TableJungleAdventureRankRewardConfig": cfg = TableJungleAdventureRankRewardConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/JungleAdventure/JungleAdventureConfig", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/JungleAdventure/JungleAdventureConfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableJungleAdventureSettingList = JsonConvert.DeserializeObject<List<TableJungleAdventureSetting>>(JsonConvert.SerializeObject(table["jungleadventuresetting"]));
            TableJungleAdventureConfigList = JsonConvert.DeserializeObject<List<TableJungleAdventureConfig>>(JsonConvert.SerializeObject(table["jungleadventureconfig"]));
            TableJungleAdventureRewardConfigList = JsonConvert.DeserializeObject<List<TableJungleAdventureRewardConfig>>(JsonConvert.SerializeObject(table["jungleadventurerewardconfig"]));
            TableJungleAdventureRankRewardConfigList = JsonConvert.DeserializeObject<List<TableJungleAdventureRankRewardConfig>>(JsonConvert.SerializeObject(table["jungleadventurerankrewardconfig"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}