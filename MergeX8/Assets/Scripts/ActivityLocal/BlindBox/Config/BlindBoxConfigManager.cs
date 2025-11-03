/************************************************
 * BlindBox Config Manager class : BlindBoxConfigManager
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

namespace DragonPlus.Config.BlindBox
{
    public partial class BlindBoxConfigManager : Manager<BlindBoxConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<BlindBoxGlobalConfig> BlindBoxGlobalConfigList;
        public List<BlindBoxThemeConfig> BlindBoxThemeConfigList;
        public List<BlindBoxRecycleShopConfig> BlindBoxRecycleShopConfigList;
        public List<BlindBoxItemConfig> BlindBoxItemConfigList;
        public List<BlindBoxGroupConfig> BlindBoxGroupConfigList;
        public List<BlindBoxBoxConfig> BlindBoxBoxConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(BlindBoxGlobalConfig)] = "BlindBoxGlobalConfig",
            [typeof(BlindBoxThemeConfig)] = "BlindBoxThemeConfig",
            [typeof(BlindBoxRecycleShopConfig)] = "BlindBoxRecycleShopConfig",
            [typeof(BlindBoxItemConfig)] = "BlindBoxItemConfig",
            [typeof(BlindBoxGroupConfig)] = "BlindBoxGroupConfig",
            [typeof(BlindBoxBoxConfig)] = "BlindBoxBoxConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("blindboxglobalconfig")) return false;
            if (!table.ContainsKey("blindboxthemeconfig")) return false;
            if (!table.ContainsKey("blindboxrecycleshopconfig")) return false;
            if (!table.ContainsKey("blindboxitemconfig")) return false;
            if (!table.ContainsKey("blindboxgroupconfig")) return false;
            if (!table.ContainsKey("blindboxboxconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "BlindBoxGlobalConfig": cfg = BlindBoxGlobalConfigList as List<T>; break;
                case "BlindBoxThemeConfig": cfg = BlindBoxThemeConfigList as List<T>; break;
                case "BlindBoxRecycleShopConfig": cfg = BlindBoxRecycleShopConfigList as List<T>; break;
                case "BlindBoxItemConfig": cfg = BlindBoxItemConfigList as List<T>; break;
                case "BlindBoxGroupConfig": cfg = BlindBoxGroupConfigList as List<T>; break;
                case "BlindBoxBoxConfig": cfg = BlindBoxBoxConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/BlindBox/blindbox");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/BlindBox/blindbox error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            BlindBoxGlobalConfigList = JsonConvert.DeserializeObject<List<BlindBoxGlobalConfig>>(JsonConvert.SerializeObject(table["blindboxglobalconfig"]));
            BlindBoxThemeConfigList = JsonConvert.DeserializeObject<List<BlindBoxThemeConfig>>(JsonConvert.SerializeObject(table["blindboxthemeconfig"]));
            BlindBoxRecycleShopConfigList = JsonConvert.DeserializeObject<List<BlindBoxRecycleShopConfig>>(JsonConvert.SerializeObject(table["blindboxrecycleshopconfig"]));
            BlindBoxItemConfigList = JsonConvert.DeserializeObject<List<BlindBoxItemConfig>>(JsonConvert.SerializeObject(table["blindboxitemconfig"]));
            BlindBoxGroupConfigList = JsonConvert.DeserializeObject<List<BlindBoxGroupConfig>>(JsonConvert.SerializeObject(table["blindboxgroupconfig"]));
            BlindBoxBoxConfigList = JsonConvert.DeserializeObject<List<BlindBoxBoxConfig>>(JsonConvert.SerializeObject(table["blindboxboxconfig"]));
            
        }
    }
}