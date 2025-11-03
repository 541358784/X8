/************************************************
 * DigTrench Config Manager class : DigTrenchConfigManager
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

namespace DragonPlus.Config.DigTrench
{
    public partial class DigTrenchConfigManager : Manager<DigTrenchConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<DigTrenchLevel> DigTrenchLevelList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(DigTrenchLevel)] = "DigTrenchLevel",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("digtrenchlevel")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "DigTrenchLevel": cfg = DigTrenchLevelList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/DigTrench/dig_trench");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/DigTrench/dig_trench error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            DigTrenchLevelList = JsonConvert.DeserializeObject<List<DigTrenchLevel>>(JsonConvert.SerializeObject(table["digtrenchlevel"]));
            
        }
    }
}