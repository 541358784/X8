/************************************************
 * DiamondReward Config Manager class : DiamondRewardConfigManager
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

namespace DragonPlus.Config.DiamondReward
{
    public partial class DiamondRewardConfigManager : Manager<DiamondRewardConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<DiamondSettingConfig> DiamondSettingConfigList;
        public List<DiamondResultConfig> DiamondResultConfigList;
        public List<DiamondPoolConfig> DiamondPoolConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(DiamondSettingConfig)] = "DiamondSettingConfig",
            [typeof(DiamondResultConfig)] = "DiamondResultConfig",
            [typeof(DiamondPoolConfig)] = "DiamondPoolConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("diamondsettingconfig")) return false;
            if (!table.ContainsKey("diamondresultconfig")) return false;
            if (!table.ContainsKey("diamondpoolconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "DiamondSettingConfig": cfg = DiamondSettingConfigList as List<T>; break;
                case "DiamondResultConfig": cfg = DiamondResultConfigList as List<T>; break;
                case "DiamondPoolConfig": cfg = DiamondPoolConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/DiamondReward/config");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/DiamondReward/config error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            DiamondSettingConfigList = JsonConvert.DeserializeObject<List<DiamondSettingConfig>>(JsonConvert.SerializeObject(table["diamondsettingconfig"]));
            DiamondResultConfigList = JsonConvert.DeserializeObject<List<DiamondResultConfig>>(JsonConvert.SerializeObject(table["diamondresultconfig"]));
            DiamondPoolConfigList = JsonConvert.DeserializeObject<List<DiamondPoolConfig>>(JsonConvert.SerializeObject(table["diamondpoolconfig"]));
            
        }
    }
}