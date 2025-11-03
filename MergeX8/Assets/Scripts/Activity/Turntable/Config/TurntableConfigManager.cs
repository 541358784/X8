/************************************************
 * Turntable Config Manager class : TurntableConfigManager
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

namespace DragonPlus.Config.Turntable
{
    public partial class TurntableConfigManager : Manager<TurntableConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<TurntableSetingConfig> TurntableSetingConfigList;
        public List<TurntableResultConfig> TurntableResultConfigList;
        public List<TurntablePoolConfig> TurntablePoolConfigList;
        public List<TurntableOrderConfig> TurntableOrderConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TurntableSetingConfig)] = "TurntableSetingConfig",
            [typeof(TurntableResultConfig)] = "TurntableResultConfig",
            [typeof(TurntablePoolConfig)] = "TurntablePoolConfig",
            [typeof(TurntableOrderConfig)] = "TurntableOrderConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("turntablesetingconfig")) return false;
            if (!table.ContainsKey("turntableresultconfig")) return false;
            if (!table.ContainsKey("turntablepoolconfig")) return false;
            if (!table.ContainsKey("turntableorderconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TurntableSetingConfig": cfg = TurntableSetingConfigList as List<T>; break;
                case "TurntableResultConfig": cfg = TurntableResultConfigList as List<T>; break;
                case "TurntablePoolConfig": cfg = TurntablePoolConfigList as List<T>; break;
                case "TurntableOrderConfig": cfg = TurntableOrderConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/Turntable/config");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/Turntable/config error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TurntableSetingConfigList = JsonConvert.DeserializeObject<List<TurntableSetingConfig>>(JsonConvert.SerializeObject(table["turntablesetingconfig"]));
            TurntableResultConfigList = JsonConvert.DeserializeObject<List<TurntableResultConfig>>(JsonConvert.SerializeObject(table["turntableresultconfig"]));
            TurntablePoolConfigList = JsonConvert.DeserializeObject<List<TurntablePoolConfig>>(JsonConvert.SerializeObject(table["turntablepoolconfig"]));
            TurntableOrderConfigList = JsonConvert.DeserializeObject<List<TurntableOrderConfig>>(JsonConvert.SerializeObject(table["turntableorderconfig"]));
            
        }
    }
}