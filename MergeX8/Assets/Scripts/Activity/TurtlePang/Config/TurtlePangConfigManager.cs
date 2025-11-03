/************************************************
 * TurtlePang Config Manager class : TurtlePangConfigManager
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

namespace DragonPlus.Config.TurtlePang
{
    public partial class TurtlePangConfigManager : Manager<TurtlePangConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<TurtlePangGlobalConfig> TurtlePangGlobalConfigList;
        public List<TurtlePangStoreLevelConfig> TurtlePangStoreLevelConfigList;
        public List<TurtlePangStoreItemConfig> TurtlePangStoreItemConfigList;
        public List<TurtlePangTaskRewardConfig> TurtlePangTaskRewardConfigList;
        public List<TurtlePangGiftBagConfig> TurtlePangGiftBagConfigList;
        public List<TurtlePangItemConfig> TurtlePangItemConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TurtlePangGlobalConfig)] = "TurtlePangGlobalConfig",
            [typeof(TurtlePangStoreLevelConfig)] = "TurtlePangStoreLevelConfig",
            [typeof(TurtlePangStoreItemConfig)] = "TurtlePangStoreItemConfig",
            [typeof(TurtlePangTaskRewardConfig)] = "TurtlePangTaskRewardConfig",
            [typeof(TurtlePangGiftBagConfig)] = "TurtlePangGiftBagConfig",
            [typeof(TurtlePangItemConfig)] = "TurtlePangItemConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("turtlepangglobalconfig")) return false;
            if (!table.ContainsKey("turtlepangstorelevelconfig")) return false;
            if (!table.ContainsKey("turtlepangstoreitemconfig")) return false;
            if (!table.ContainsKey("turtlepangtaskrewardconfig")) return false;
            if (!table.ContainsKey("turtlepanggiftbagconfig")) return false;
            if (!table.ContainsKey("turtlepangitemconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TurtlePangGlobalConfig": cfg = TurtlePangGlobalConfigList as List<T>; break;
                case "TurtlePangStoreLevelConfig": cfg = TurtlePangStoreLevelConfigList as List<T>; break;
                case "TurtlePangStoreItemConfig": cfg = TurtlePangStoreItemConfigList as List<T>; break;
                case "TurtlePangTaskRewardConfig": cfg = TurtlePangTaskRewardConfigList as List<T>; break;
                case "TurtlePangGiftBagConfig": cfg = TurtlePangGiftBagConfigList as List<T>; break;
                case "TurtlePangItemConfig": cfg = TurtlePangItemConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/TurtlePang/turtlepang");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/TurtlePang/turtlepang error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TurtlePangGlobalConfigList = JsonConvert.DeserializeObject<List<TurtlePangGlobalConfig>>(JsonConvert.SerializeObject(table["turtlepangglobalconfig"]));
            TurtlePangStoreLevelConfigList = JsonConvert.DeserializeObject<List<TurtlePangStoreLevelConfig>>(JsonConvert.SerializeObject(table["turtlepangstorelevelconfig"]));
            TurtlePangStoreItemConfigList = JsonConvert.DeserializeObject<List<TurtlePangStoreItemConfig>>(JsonConvert.SerializeObject(table["turtlepangstoreitemconfig"]));
            TurtlePangTaskRewardConfigList = JsonConvert.DeserializeObject<List<TurtlePangTaskRewardConfig>>(JsonConvert.SerializeObject(table["turtlepangtaskrewardconfig"]));
            TurtlePangGiftBagConfigList = JsonConvert.DeserializeObject<List<TurtlePangGiftBagConfig>>(JsonConvert.SerializeObject(table["turtlepanggiftbagconfig"]));
            TurtlePangItemConfigList = JsonConvert.DeserializeObject<List<TurtlePangItemConfig>>(JsonConvert.SerializeObject(table["turtlepangitemconfig"]));
            
        }
    }
}