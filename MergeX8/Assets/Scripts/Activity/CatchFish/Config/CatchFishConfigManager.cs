/************************************************
 * CatchFish Config Manager class : CatchFishConfigManager
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

namespace DragonPlus.Config.CatchFish
{
    public partial class CatchFishConfigManager : Manager<CatchFishConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<CatchFishGlobalConfig> CatchFishGlobalConfigList;
        public List<CatchFishTaskRewardConfig> CatchFishTaskRewardConfigList;
        public List<CatchFishShopConfig> CatchFishShopConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(CatchFishGlobalConfig)] = "CatchFishGlobalConfig",
            [typeof(CatchFishTaskRewardConfig)] = "CatchFishTaskRewardConfig",
            [typeof(CatchFishShopConfig)] = "CatchFishShopConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("catchfishglobalconfig")) return false;
            if (!table.ContainsKey("catchfishtaskrewardconfig")) return false;
            if (!table.ContainsKey("catchfishshopconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "CatchFishGlobalConfig": cfg = CatchFishGlobalConfigList as List<T>; break;
                case "CatchFishTaskRewardConfig": cfg = CatchFishTaskRewardConfigList as List<T>; break;
                case "CatchFishShopConfig": cfg = CatchFishShopConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/CatchFish/catchfish");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/CatchFish/catchfish error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            CatchFishGlobalConfigList = JsonConvert.DeserializeObject<List<CatchFishGlobalConfig>>(JsonConvert.SerializeObject(table["catchfishglobalconfig"]));
            CatchFishTaskRewardConfigList = JsonConvert.DeserializeObject<List<CatchFishTaskRewardConfig>>(JsonConvert.SerializeObject(table["catchfishtaskrewardconfig"]));
            CatchFishShopConfigList = JsonConvert.DeserializeObject<List<CatchFishShopConfig>>(JsonConvert.SerializeObject(table["catchfishshopconfig"]));
            
        }
    }
}