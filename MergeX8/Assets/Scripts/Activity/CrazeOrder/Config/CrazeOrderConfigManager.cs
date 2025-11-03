/************************************************
 * CrazeOrder Config Manager class : CrazeOrderConfigManager
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

namespace DragonPlus.Config.CrazeOrder
{
    public partial class CrazeOrderConfigManager : Manager<CrazeOrderConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<CrazeOrderSetting> CrazeOrderSettingList;
        public List<CrazeOrderConfig> CrazeOrderConfigList;
        public List<CrazeStageConfig> CrazeStageConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(CrazeOrderSetting)] = "CrazeOrderSetting",
            [typeof(CrazeOrderConfig)] = "CrazeOrderConfig",
            [typeof(CrazeStageConfig)] = "CrazeStageConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("crazeordersetting")) return false;
            if (!table.ContainsKey("crazeorderconfig")) return false;
            if (!table.ContainsKey("crazestageconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "CrazeOrderSetting": cfg = CrazeOrderSettingList as List<T>; break;
                case "CrazeOrderConfig": cfg = CrazeOrderConfigList as List<T>; break;
                case "CrazeStageConfig": cfg = CrazeStageConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/CrazeOrder/config");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/CrazeOrder/config error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            CrazeOrderSettingList = JsonConvert.DeserializeObject<List<CrazeOrderSetting>>(JsonConvert.SerializeObject(table["crazeordersetting"]));
            CrazeOrderConfigList = JsonConvert.DeserializeObject<List<CrazeOrderConfig>>(JsonConvert.SerializeObject(table["crazeorderconfig"]));
            CrazeStageConfigList = JsonConvert.DeserializeObject<List<CrazeStageConfig>>(JsonConvert.SerializeObject(table["crazestageconfig"]));
            
        }
    }
}