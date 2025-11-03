/************************************************
 * SummerWatermelonBread Config Manager class : SummerWatermelonBreadConfigManager
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

namespace DragonPlus.Config.SummerWatermelonBread
{
    public partial class SummerWatermelonBreadConfigManager : Manager<SummerWatermelonBreadConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<SummerWatermelonBreadConfig> SummerWatermelonBreadConfigList;
        public List<SummerWatermelonBreadProductConfig> SummerWatermelonBreadProductConfigList;
        public List<SummerWatermelonBreadProductAttenuationConfig> SummerWatermelonBreadProductAttenuationConfigList;
        public List<SummerWatermelonBreadFirstTimeRewardConfig> SummerWatermelonBreadFirstTimeRewardConfigList;
        public List<SummerWatermelonBreadMergeRewardConfig> SummerWatermelonBreadMergeRewardConfigList;
        public List<SummerWatermelonBreadPackageConfig> SummerWatermelonBreadPackageConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(SummerWatermelonBreadConfig)] = "SummerWatermelonBreadConfig",
            [typeof(SummerWatermelonBreadProductConfig)] = "SummerWatermelonBreadProductConfig",
            [typeof(SummerWatermelonBreadProductAttenuationConfig)] = "SummerWatermelonBreadProductAttenuationConfig",
            [typeof(SummerWatermelonBreadFirstTimeRewardConfig)] = "SummerWatermelonBreadFirstTimeRewardConfig",
            [typeof(SummerWatermelonBreadMergeRewardConfig)] = "SummerWatermelonBreadMergeRewardConfig",
            [typeof(SummerWatermelonBreadPackageConfig)] = "SummerWatermelonBreadPackageConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("summerwatermelonbreadconfig")) return false;
            if (!table.ContainsKey("summerwatermelonbreadproductconfig")) return false;
            if (!table.ContainsKey("summerwatermelonbreadproductattenuationconfig")) return false;
            if (!table.ContainsKey("summerwatermelonbreadfirsttimerewardconfig")) return false;
            if (!table.ContainsKey("summerwatermelonbreadmergerewardconfig")) return false;
            if (!table.ContainsKey("summerwatermelonbreadpackageconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "SummerWatermelonBreadConfig": cfg = SummerWatermelonBreadConfigList as List<T>; break;
                case "SummerWatermelonBreadProductConfig": cfg = SummerWatermelonBreadProductConfigList as List<T>; break;
                case "SummerWatermelonBreadProductAttenuationConfig": cfg = SummerWatermelonBreadProductAttenuationConfigList as List<T>; break;
                case "SummerWatermelonBreadFirstTimeRewardConfig": cfg = SummerWatermelonBreadFirstTimeRewardConfigList as List<T>; break;
                case "SummerWatermelonBreadMergeRewardConfig": cfg = SummerWatermelonBreadMergeRewardConfigList as List<T>; break;
                case "SummerWatermelonBreadPackageConfig": cfg = SummerWatermelonBreadPackageConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/SummerWatermelonBread/summerwatermelonbread");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/SummerWatermelonBread/summerwatermelonbread error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            SummerWatermelonBreadConfigList = JsonConvert.DeserializeObject<List<SummerWatermelonBreadConfig>>(JsonConvert.SerializeObject(table["summerwatermelonbreadconfig"]));
            SummerWatermelonBreadProductConfigList = JsonConvert.DeserializeObject<List<SummerWatermelonBreadProductConfig>>(JsonConvert.SerializeObject(table["summerwatermelonbreadproductconfig"]));
            SummerWatermelonBreadProductAttenuationConfigList = JsonConvert.DeserializeObject<List<SummerWatermelonBreadProductAttenuationConfig>>(JsonConvert.SerializeObject(table["summerwatermelonbreadproductattenuationconfig"]));
            SummerWatermelonBreadFirstTimeRewardConfigList = JsonConvert.DeserializeObject<List<SummerWatermelonBreadFirstTimeRewardConfig>>(JsonConvert.SerializeObject(table["summerwatermelonbreadfirsttimerewardconfig"]));
            SummerWatermelonBreadMergeRewardConfigList = JsonConvert.DeserializeObject<List<SummerWatermelonBreadMergeRewardConfig>>(JsonConvert.SerializeObject(table["summerwatermelonbreadmergerewardconfig"]));
            SummerWatermelonBreadPackageConfigList = JsonConvert.DeserializeObject<List<SummerWatermelonBreadPackageConfig>>(JsonConvert.SerializeObject(table["summerwatermelonbreadpackageconfig"]));
            
        }
    }
}