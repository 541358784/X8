/************************************************
 * TreasureHunt Config Manager class : TreasureHuntConfigManager
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

namespace DragonPlus.Config.TreasureHunt
{
    public partial class TreasureHuntConfigManager : Manager<TreasureHuntConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<TreasureHuntActivityConfig> TreasureHuntActivityConfigList;
        public List<TreasureHuntLevelConfig> TreasureHuntLevelConfigList;
        public List<TreasureHuntStoreConfig> TreasureHuntStoreConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TreasureHuntActivityConfig)] = "TreasureHuntActivityConfig",
            [typeof(TreasureHuntLevelConfig)] = "TreasureHuntLevelConfig",
            [typeof(TreasureHuntStoreConfig)] = "TreasureHuntStoreConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("treasurehuntactivityconfig")) return false;
            if (!table.ContainsKey("treasurehuntlevelconfig")) return false;
            if (!table.ContainsKey("treasurehuntstoreconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TreasureHuntActivityConfig": cfg = TreasureHuntActivityConfigList as List<T>; break;
                case "TreasureHuntLevelConfig": cfg = TreasureHuntLevelConfigList as List<T>; break;
                case "TreasureHuntStoreConfig": cfg = TreasureHuntStoreConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/TreasureHunt/config");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/TreasureHunt/config error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TreasureHuntActivityConfigList = JsonConvert.DeserializeObject<List<TreasureHuntActivityConfig>>(JsonConvert.SerializeObject(table["treasurehuntactivityconfig"]));
            TreasureHuntLevelConfigList = JsonConvert.DeserializeObject<List<TreasureHuntLevelConfig>>(JsonConvert.SerializeObject(table["treasurehuntlevelconfig"]));
            TreasureHuntStoreConfigList = JsonConvert.DeserializeObject<List<TreasureHuntStoreConfig>>(JsonConvert.SerializeObject(table["treasurehuntstoreconfig"]));
            
        }
    }
}