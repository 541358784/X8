/************************************************
 * DogPlay Config Manager class : DogPlayConfigManager
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

namespace DragonPlus.Config.DogPlay
{
    public partial class DogPlayConfigManager : Manager<DogPlayConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<DogPlayOrderConfig> DogPlayOrderConfigList;
        public List<DogPlayPayTypeConfig> DogPlayPayTypeConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(DogPlayOrderConfig)] = "DogPlayOrderConfig",
            [typeof(DogPlayPayTypeConfig)] = "DogPlayPayTypeConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("dogplayorderconfig")) return false;
            if (!table.ContainsKey("dogplaypaytypeconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "DogPlayOrderConfig": cfg = DogPlayOrderConfigList as List<T>; break;
                case "DogPlayPayTypeConfig": cfg = DogPlayPayTypeConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/DogPlay/dogplay");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/DogPlay/dogplay error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            DogPlayOrderConfigList = JsonConvert.DeserializeObject<List<DogPlayOrderConfig>>(JsonConvert.SerializeObject(table["dogplayorderconfig"]));
            DogPlayPayTypeConfigList = JsonConvert.DeserializeObject<List<DogPlayPayTypeConfig>>(JsonConvert.SerializeObject(table["dogplaypaytypeconfig"]));
            
        }
    }
}