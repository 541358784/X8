/************************************************
 * ButterflyWorkShop Config Manager class : ButterflyWorkShopConfigManager
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

namespace DragonPlus.Config.ButterflyWorkShop
{
    public partial class ButterflyWorkShopConfigManager : Manager<ButterflyWorkShopConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<ButterflyWorkShopConfig> ButterflyWorkShopConfigList;
        public List<ButterflyWorkShopRewardConfig> ButterflyWorkShopRewardConfigList;
        public List<ButterflyRandomConfig> ButterflyRandomConfigList;
        public List<StageRewardConfig> StageRewardConfigList;
        public List<ButterflyWorkShopProductAttenuationConfig> ButterflyWorkShopProductAttenuationConfigList;
        public List<ButterflyWorkShopPackageConfig> ButterflyWorkShopPackageConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(ButterflyWorkShopConfig)] = "ButterflyWorkShopConfig",
            [typeof(ButterflyWorkShopRewardConfig)] = "ButterflyWorkShopRewardConfig",
            [typeof(ButterflyRandomConfig)] = "ButterflyRandomConfig",
            [typeof(StageRewardConfig)] = "StageRewardConfig",
            [typeof(ButterflyWorkShopProductAttenuationConfig)] = "ButterflyWorkShopProductAttenuationConfig",
            [typeof(ButterflyWorkShopPackageConfig)] = "ButterflyWorkShopPackageConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("butterflyworkshopconfig")) return false;
            if (!table.ContainsKey("butterflyworkshoprewardconfig")) return false;
            if (!table.ContainsKey("butterflyrandomconfig")) return false;
            if (!table.ContainsKey("stagerewardconfig")) return false;
            if (!table.ContainsKey("butterflyworkshopproductattenuationconfig")) return false;
            if (!table.ContainsKey("butterflyworkshoppackageconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "ButterflyWorkShopConfig": cfg = ButterflyWorkShopConfigList as List<T>; break;
                case "ButterflyWorkShopRewardConfig": cfg = ButterflyWorkShopRewardConfigList as List<T>; break;
                case "ButterflyRandomConfig": cfg = ButterflyRandomConfigList as List<T>; break;
                case "StageRewardConfig": cfg = StageRewardConfigList as List<T>; break;
                case "ButterflyWorkShopProductAttenuationConfig": cfg = ButterflyWorkShopProductAttenuationConfigList as List<T>; break;
                case "ButterflyWorkShopPackageConfig": cfg = ButterflyWorkShopPackageConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/ButterflyWorkShop/config");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/ButterflyWorkShop/config error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            ButterflyWorkShopConfigList = JsonConvert.DeserializeObject<List<ButterflyWorkShopConfig>>(JsonConvert.SerializeObject(table["butterflyworkshopconfig"]));
            ButterflyWorkShopRewardConfigList = JsonConvert.DeserializeObject<List<ButterflyWorkShopRewardConfig>>(JsonConvert.SerializeObject(table["butterflyworkshoprewardconfig"]));
            ButterflyRandomConfigList = JsonConvert.DeserializeObject<List<ButterflyRandomConfig>>(JsonConvert.SerializeObject(table["butterflyrandomconfig"]));
            StageRewardConfigList = JsonConvert.DeserializeObject<List<StageRewardConfig>>(JsonConvert.SerializeObject(table["stagerewardconfig"]));
            ButterflyWorkShopProductAttenuationConfigList = JsonConvert.DeserializeObject<List<ButterflyWorkShopProductAttenuationConfig>>(JsonConvert.SerializeObject(table["butterflyworkshopproductattenuationconfig"]));
            ButterflyWorkShopPackageConfigList = JsonConvert.DeserializeObject<List<ButterflyWorkShopPackageConfig>>(JsonConvert.SerializeObject(table["butterflyworkshoppackageconfig"]));
            
        }
    }
}