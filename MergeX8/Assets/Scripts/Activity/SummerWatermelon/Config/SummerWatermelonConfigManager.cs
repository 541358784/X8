/************************************************
 * SummerWatermelon Config Manager class : SummerWatermelonConfigManager
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

namespace DragonPlus.Config.SummerWatermelon
{
    public partial class SummerWatermelonConfigManager : Manager<SummerWatermelonConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<SummerWatermelonConfig> SummerWatermelonConfigList;
        public List<SummerWatermelonProductConfig> SummerWatermelonProductConfigList;
        public List<SummerWatermelonFirstTimeRewardConfig> SummerWatermelonFirstTimeRewardConfigList;
        public List<SummerWatermelonMergeRewardConfig> SummerWatermelonMergeRewardConfigList;
        public List<SummerWatermelonPackageConfig> SummerWatermelonPackageConfigList;
        public List<SummerWatermelonProductAttenuationConfig> SummerWatermelonProductAttenuationConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(SummerWatermelonConfig)] = "SummerWatermelonConfig",
            [typeof(SummerWatermelonProductConfig)] = "SummerWatermelonProductConfig",
            [typeof(SummerWatermelonFirstTimeRewardConfig)] = "SummerWatermelonFirstTimeRewardConfig",
            [typeof(SummerWatermelonMergeRewardConfig)] = "SummerWatermelonMergeRewardConfig",
            [typeof(SummerWatermelonPackageConfig)] = "SummerWatermelonPackageConfig",
            [typeof(SummerWatermelonProductAttenuationConfig)] = "SummerWatermelonProductAttenuationConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("summerwatermelonconfig")) return false;
            if (!table.ContainsKey("summerwatermelonproductconfig")) return false;
            if (!table.ContainsKey("summerwatermelonfirsttimerewardconfig")) return false;
            if (!table.ContainsKey("summerwatermelonmergerewardconfig")) return false;
            if (!table.ContainsKey("summerwatermelonpackageconfig")) return false;
            if (!table.ContainsKey("summerwatermelonproductattenuationconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "SummerWatermelonConfig": cfg = SummerWatermelonConfigList as List<T>; break;
                case "SummerWatermelonProductConfig": cfg = SummerWatermelonProductConfigList as List<T>; break;
                case "SummerWatermelonFirstTimeRewardConfig": cfg = SummerWatermelonFirstTimeRewardConfigList as List<T>; break;
                case "SummerWatermelonMergeRewardConfig": cfg = SummerWatermelonMergeRewardConfigList as List<T>; break;
                case "SummerWatermelonPackageConfig": cfg = SummerWatermelonPackageConfigList as List<T>; break;
                case "SummerWatermelonProductAttenuationConfig": cfg = SummerWatermelonProductAttenuationConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/SummerWatermelon/summerwatermelon");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/SummerWatermelon/summerwatermelon error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            SummerWatermelonConfigList = JsonConvert.DeserializeObject<List<SummerWatermelonConfig>>(JsonConvert.SerializeObject(table["summerwatermelonconfig"]));
            SummerWatermelonProductConfigList = JsonConvert.DeserializeObject<List<SummerWatermelonProductConfig>>(JsonConvert.SerializeObject(table["summerwatermelonproductconfig"]));
            SummerWatermelonFirstTimeRewardConfigList = JsonConvert.DeserializeObject<List<SummerWatermelonFirstTimeRewardConfig>>(JsonConvert.SerializeObject(table["summerwatermelonfirsttimerewardconfig"]));
            SummerWatermelonMergeRewardConfigList = JsonConvert.DeserializeObject<List<SummerWatermelonMergeRewardConfig>>(JsonConvert.SerializeObject(table["summerwatermelonmergerewardconfig"]));
            SummerWatermelonPackageConfigList = JsonConvert.DeserializeObject<List<SummerWatermelonPackageConfig>>(JsonConvert.SerializeObject(table["summerwatermelonpackageconfig"]));
            SummerWatermelonProductAttenuationConfigList = JsonConvert.DeserializeObject<List<SummerWatermelonProductAttenuationConfig>>(JsonConvert.SerializeObject(table["summerwatermelonproductattenuationconfig"]));
            
        }
    }
}