
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.Team
{
    public partial class TeamConfigManager : TableSingleton<TeamConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableTeamGlobalConfig> TableTeamGlobalConfigList;
        public List<TableTeamLevelConfig> TableTeamLevelConfigList;
        public List<TableTeamShopConfig> TableTeamShopConfigList;
        public List<TableTeamIconConfig> TableTeamIconConfigList;
        public List<TableTeamIconFrameConfig> TableTeamIconFrameConfigList;
        public List<TableTeamOrder> TableTeamOrderList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableTeamGlobalConfig)] = "TableTeamGlobalConfig",
            [typeof(TableTeamLevelConfig)] = "TableTeamLevelConfig",
            [typeof(TableTeamShopConfig)] = "TableTeamShopConfig",
            [typeof(TableTeamIconConfig)] = "TableTeamIconConfig",
            [typeof(TableTeamIconFrameConfig)] = "TableTeamIconFrameConfig",
            [typeof(TableTeamOrder)] = "TableTeamOrder",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("teamglobalconfig")) return false;
            if (!table.ContainsKey("teamlevelconfig")) return false;
            if (!table.ContainsKey("teamshopconfig")) return false;
            if (!table.ContainsKey("teamiconconfig")) return false;
            if (!table.ContainsKey("teamiconframeconfig")) return false;
            if (!table.ContainsKey("teamorder")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableTeamGlobalConfig": cfg = TableTeamGlobalConfigList as List<T>; break;
                case "TableTeamLevelConfig": cfg = TableTeamLevelConfigList as List<T>; break;
                case "TableTeamShopConfig": cfg = TableTeamShopConfigList as List<T>; break;
                case "TableTeamIconConfig": cfg = TableTeamIconConfigList as List<T>; break;
                case "TableTeamIconFrameConfig": cfg = TableTeamIconFrameConfigList as List<T>; break;
                case "TableTeamOrder": cfg = TableTeamOrderList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/team/teamconfig", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/team/teamconfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableTeamGlobalConfigList = JsonConvert.DeserializeObject<List<TableTeamGlobalConfig>>(JsonConvert.SerializeObject(table["teamglobalconfig"]));
            TableTeamLevelConfigList = JsonConvert.DeserializeObject<List<TableTeamLevelConfig>>(JsonConvert.SerializeObject(table["teamlevelconfig"]));
            TableTeamShopConfigList = JsonConvert.DeserializeObject<List<TableTeamShopConfig>>(JsonConvert.SerializeObject(table["teamshopconfig"]));
            TableTeamIconConfigList = JsonConvert.DeserializeObject<List<TableTeamIconConfig>>(JsonConvert.SerializeObject(table["teamiconconfig"]));
            TableTeamIconFrameConfigList = JsonConvert.DeserializeObject<List<TableTeamIconFrameConfig>>(JsonConvert.SerializeObject(table["teamiconframeconfig"]));
            TableTeamOrderList = JsonConvert.DeserializeObject<List<TableTeamOrder>>(JsonConvert.SerializeObject(table["teamorder"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}