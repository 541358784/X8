/************************************************
 * Kapibala Config Manager class : KapibalaConfigManager
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

namespace DragonPlus.Config.Kapibala
{
    public partial class KapibalaConfigManager : Manager<KapibalaConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<KapibalaGlobalConfig> KapibalaGlobalConfigList;
        public List<KapibalaLevelConfig> KapibalaLevelConfigList;
        public List<KapibalaGiftBagConfig> KapibalaGiftBagConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(KapibalaGlobalConfig)] = "KapibalaGlobalConfig",
            [typeof(KapibalaLevelConfig)] = "KapibalaLevelConfig",
            [typeof(KapibalaGiftBagConfig)] = "KapibalaGiftBagConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("kapibalaglobalconfig")) return false;
            if (!table.ContainsKey("kapibalalevelconfig")) return false;
            if (!table.ContainsKey("kapibalagiftbagconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "KapibalaGlobalConfig": cfg = KapibalaGlobalConfigList as List<T>; break;
                case "KapibalaLevelConfig": cfg = KapibalaLevelConfigList as List<T>; break;
                case "KapibalaGiftBagConfig": cfg = KapibalaGiftBagConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/Kapibala/kapibala");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/Kapibala/kapibala error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            KapibalaGlobalConfigList = JsonConvert.DeserializeObject<List<KapibalaGlobalConfig>>(JsonConvert.SerializeObject(table["kapibalaglobalconfig"]));
            KapibalaLevelConfigList = JsonConvert.DeserializeObject<List<KapibalaLevelConfig>>(JsonConvert.SerializeObject(table["kapibalalevelconfig"]));
            KapibalaGiftBagConfigList = JsonConvert.DeserializeObject<List<KapibalaGiftBagConfig>>(JsonConvert.SerializeObject(table["kapibalagiftbagconfig"]));
            
        }
    }
}