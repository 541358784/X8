/************************************************
 * Easter Config Manager class : EasterConfigManager
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

namespace DragonPlus.Config.Easter
{
    public partial class EasterConfigManager : Manager<EasterConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<EasterConfig> EasterConfigList;
        public List<EasterReward> EasterRewardList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(EasterConfig)] = "EasterConfig",
            [typeof(EasterReward)] = "EasterReward",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("easterconfig")) return false;
            if (!table.ContainsKey("easterreward")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "EasterConfig": cfg = EasterConfigList as List<T>; break;
                case "EasterReward": cfg = EasterRewardList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/Easter/easter");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/Easter/easter error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            EasterConfigList = JsonConvert.DeserializeObject<List<EasterConfig>>(JsonConvert.SerializeObject(table["easterconfig"]));
            EasterRewardList = JsonConvert.DeserializeObject<List<EasterReward>>(JsonConvert.SerializeObject(table["easterreward"]));
            
        }
    }
}