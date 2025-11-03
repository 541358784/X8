/************************************************
 * KapiTile Config Manager class : KapiTileConfigManager
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

namespace DragonPlus.Config.KapiTile
{
    public partial class KapiTileConfigManager : Manager<KapiTileConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<KapiTileGlobalConfig> KapiTileGlobalConfigList;
        public List<KapiTileLevelConfig> KapiTileLevelConfigList;
        public List<KapiTileGiftBagConfig> KapiTileGiftBagConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(KapiTileGlobalConfig)] = "KapiTileGlobalConfig",
            [typeof(KapiTileLevelConfig)] = "KapiTileLevelConfig",
            [typeof(KapiTileGiftBagConfig)] = "KapiTileGiftBagConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("kapitileglobalconfig")) return false;
            if (!table.ContainsKey("kapitilelevelconfig")) return false;
            if (!table.ContainsKey("kapitilegiftbagconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "KapiTileGlobalConfig": cfg = KapiTileGlobalConfigList as List<T>; break;
                case "KapiTileLevelConfig": cfg = KapiTileLevelConfigList as List<T>; break;
                case "KapiTileGiftBagConfig": cfg = KapiTileGiftBagConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/KapiTile/kapitile");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/KapiTile/kapitile error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            KapiTileGlobalConfigList = JsonConvert.DeserializeObject<List<KapiTileGlobalConfig>>(JsonConvert.SerializeObject(table["kapitileglobalconfig"]));
            KapiTileLevelConfigList = JsonConvert.DeserializeObject<List<KapiTileLevelConfig>>(JsonConvert.SerializeObject(table["kapitilelevelconfig"]));
            KapiTileGiftBagConfigList = JsonConvert.DeserializeObject<List<KapiTileGiftBagConfig>>(JsonConvert.SerializeObject(table["kapitilegiftbagconfig"]));
            
        }
    }
}