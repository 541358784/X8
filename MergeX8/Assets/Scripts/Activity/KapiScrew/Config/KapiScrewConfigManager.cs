/************************************************
 * KapiScrew Config Manager class : KapiScrewConfigManager
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

namespace DragonPlus.Config.KapiScrew
{
    public partial class KapiScrewConfigManager : Manager<KapiScrewConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<KapiScrewGlobalConfig> KapiScrewGlobalConfigList;
        public List<KapiScrewLevelConfig> KapiScrewLevelConfigList;
        public List<KapiScrewGiftBagConfig> KapiScrewGiftBagConfigList;
        public List<KapiScrewNameConfig> KapiScrewNameConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(KapiScrewGlobalConfig)] = "KapiScrewGlobalConfig",
            [typeof(KapiScrewLevelConfig)] = "KapiScrewLevelConfig",
            [typeof(KapiScrewGiftBagConfig)] = "KapiScrewGiftBagConfig",
            [typeof(KapiScrewNameConfig)] = "KapiScrewNameConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("kapiscrewglobalconfig")) return false;
            if (!table.ContainsKey("kapiscrewlevelconfig")) return false;
            if (!table.ContainsKey("kapiscrewgiftbagconfig")) return false;
            if (!table.ContainsKey("kapiscrewnameconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "KapiScrewGlobalConfig": cfg = KapiScrewGlobalConfigList as List<T>; break;
                case "KapiScrewLevelConfig": cfg = KapiScrewLevelConfigList as List<T>; break;
                case "KapiScrewGiftBagConfig": cfg = KapiScrewGiftBagConfigList as List<T>; break;
                case "KapiScrewNameConfig": cfg = KapiScrewNameConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/KapiScrew/kapiscrew");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/KapiScrew/kapiscrew error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            KapiScrewGlobalConfigList = JsonConvert.DeserializeObject<List<KapiScrewGlobalConfig>>(JsonConvert.SerializeObject(table["kapiscrewglobalconfig"]));
            KapiScrewLevelConfigList = JsonConvert.DeserializeObject<List<KapiScrewLevelConfig>>(JsonConvert.SerializeObject(table["kapiscrewlevelconfig"]));
            KapiScrewGiftBagConfigList = JsonConvert.DeserializeObject<List<KapiScrewGiftBagConfig>>(JsonConvert.SerializeObject(table["kapiscrewgiftbagconfig"]));
            KapiScrewNameConfigList = JsonConvert.DeserializeObject<List<KapiScrewNameConfig>>(JsonConvert.SerializeObject(table["kapiscrewnameconfig"]));
            
        }
    }
}