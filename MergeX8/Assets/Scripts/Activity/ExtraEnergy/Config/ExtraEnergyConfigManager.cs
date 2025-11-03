/************************************************
 * ExtraEnergy Config Manager class : ExtraEnergyConfigManager
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

namespace DragonPlus.Config.ExtraEnergy
{
    public partial class ExtraEnergyConfigManager : Manager<ExtraEnergyConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<ExtraEnergyActivityConfig> ExtraEnergyActivityConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(ExtraEnergyActivityConfig)] = "ExtraEnergyActivityConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("extraenergyactivityconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "ExtraEnergyActivityConfig": cfg = ExtraEnergyActivityConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/ExtraEnergy/config");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/ExtraEnergy/config error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            ExtraEnergyActivityConfigList = JsonConvert.DeserializeObject<List<ExtraEnergyActivityConfig>>(JsonConvert.SerializeObject(table["extraenergyactivityconfig"]));
            
        }
    }
}