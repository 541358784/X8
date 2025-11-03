/************************************************
 * ShopExtraReward Config Manager class : ShopExtraRewardConfigManager
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

namespace DragonPlus.Config.ShopExtraReward
{
    public partial class ShopExtraRewardConfigManager : Manager<ShopExtraRewardConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<ShopExtraRewardConfig> ShopExtraRewardConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(ShopExtraRewardConfig)] = "ShopExtraRewardConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("shopextrarewardconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "ShopExtraRewardConfig": cfg = ShopExtraRewardConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/ShopExtraReward/shopextrareward");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/ShopExtraReward/shopextrareward error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            ShopExtraRewardConfigList = JsonConvert.DeserializeObject<List<ShopExtraRewardConfig>>(JsonConvert.SerializeObject(table["shopextrarewardconfig"]));
            
        }
    }
}