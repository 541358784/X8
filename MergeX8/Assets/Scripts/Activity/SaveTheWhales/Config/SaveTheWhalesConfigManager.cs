/************************************************
 * SaveTheWhales Config Manager class : SaveTheWhalesConfigManager
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

namespace DragonPlus.Config.SaveTheWhales
{
    public partial class SaveTheWhalesConfigManager : Manager<SaveTheWhalesConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<SaveTheWhalesActivityConfig> SaveTheWhalesActivityConfigList;
        public List<SaveTheWhalesTaskConfig> SaveTheWhalesTaskConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(SaveTheWhalesActivityConfig)] = "SaveTheWhalesActivityConfig",
            [typeof(SaveTheWhalesTaskConfig)] = "SaveTheWhalesTaskConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("savethewhalesactivityconfig")) return false;
            if (!table.ContainsKey("savethewhalestaskconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "SaveTheWhalesActivityConfig": cfg = SaveTheWhalesActivityConfigList as List<T>; break;
                case "SaveTheWhalesTaskConfig": cfg = SaveTheWhalesTaskConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/SaveTheWhales/config");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/SaveTheWhales/config error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            SaveTheWhalesActivityConfigList = JsonConvert.DeserializeObject<List<SaveTheWhalesActivityConfig>>(JsonConvert.SerializeObject(table["savethewhalesactivityconfig"]));
            SaveTheWhalesTaskConfigList = JsonConvert.DeserializeObject<List<SaveTheWhalesTaskConfig>>(JsonConvert.SerializeObject(table["savethewhalestaskconfig"]));
            
        }
    }
}