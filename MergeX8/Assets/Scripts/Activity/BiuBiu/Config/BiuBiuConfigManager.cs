/************************************************
 * BiuBiu Config Manager class : BiuBiuConfigManager
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

namespace DragonPlus.Config.BiuBiu
{
    public partial class BiuBiuConfigManager : Manager<BiuBiuConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<BiuBiuGlobalConfig> BiuBiuGlobalConfigList;
        public List<BiuBiuFateConfig> BiuBiuFateConfigList;
        public List<BiuBiuUIConfig> BiuBiuUIConfigList;
        public List<BiuBiuPackageConfig> BiuBiuPackageConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(BiuBiuGlobalConfig)] = "BiuBiuGlobalConfig",
            [typeof(BiuBiuFateConfig)] = "BiuBiuFateConfig",
            [typeof(BiuBiuUIConfig)] = "BiuBiuUIConfig",
            [typeof(BiuBiuPackageConfig)] = "BiuBiuPackageConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("biubiuglobalconfig")) return false;
            if (!table.ContainsKey("biubiufateconfig")) return false;
            if (!table.ContainsKey("biubiuuiconfig")) return false;
            if (!table.ContainsKey("biubiupackageconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "BiuBiuGlobalConfig": cfg = BiuBiuGlobalConfigList as List<T>; break;
                case "BiuBiuFateConfig": cfg = BiuBiuFateConfigList as List<T>; break;
                case "BiuBiuUIConfig": cfg = BiuBiuUIConfigList as List<T>; break;
                case "BiuBiuPackageConfig": cfg = BiuBiuPackageConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/BiuBiu/biubiu");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/BiuBiu/biubiu error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            BiuBiuGlobalConfigList = JsonConvert.DeserializeObject<List<BiuBiuGlobalConfig>>(JsonConvert.SerializeObject(table["biubiuglobalconfig"]));
            BiuBiuFateConfigList = JsonConvert.DeserializeObject<List<BiuBiuFateConfig>>(JsonConvert.SerializeObject(table["biubiufateconfig"]));
            BiuBiuUIConfigList = JsonConvert.DeserializeObject<List<BiuBiuUIConfig>>(JsonConvert.SerializeObject(table["biubiuuiconfig"]));
            BiuBiuPackageConfigList = JsonConvert.DeserializeObject<List<BiuBiuPackageConfig>>(JsonConvert.SerializeObject(table["biubiupackageconfig"]));
            
        }
    }
}