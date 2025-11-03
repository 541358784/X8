/************************************************
 * TreasureMap Config Manager class : TreasureMapConfigManager
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.TreasureMap
{
    public partial class TreasureMapConfigManager : Manager<TreasureMapConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<TreasureMapActivityConfig> TreasureMapActivityConfigList;
        public List<TreasureMapConfig> TreasureMapConfigList;
        public List<TreasureMapLimitConfig> TreasureMapLimitConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TreasureMapActivityConfig)] = "TreasureMapActivityConfig",
            [typeof(TreasureMapConfig)] = "TreasureMapConfig",
            [typeof(TreasureMapLimitConfig)] = "TreasureMapLimitConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("treasuremapactivityconfig")) return false;
            if (!table.ContainsKey("treasuremapconfig")) return false;
            if (!table.ContainsKey("treasuremaplimitconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TreasureMapActivityConfig": cfg = TreasureMapActivityConfigList as List<T>; break;
                case "TreasureMapConfig": cfg = TreasureMapConfigList as List<T>; break;
                case "TreasureMapLimitConfig": cfg = TreasureMapLimitConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/TreasureMap/config");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/TreasureMap/config error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TreasureMapActivityConfigList = JsonConvert.DeserializeObject<List<TreasureMapActivityConfig>>(JsonConvert.SerializeObject(table["treasuremapactivityconfig"]));
            TreasureMapConfigList = JsonConvert.DeserializeObject<List<TreasureMapConfig>>(JsonConvert.SerializeObject(table["treasuremapconfig"]));
            TreasureMapLimitConfigList = JsonConvert.DeserializeObject<List<TreasureMapLimitConfig>>(JsonConvert.SerializeObject(table["treasuremaplimitconfig"]));
            
        }
    }
}