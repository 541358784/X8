/************************************************
 * ChristmaxBlindBox Config Manager class : ChristmaxBlindBoxConfigManager
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

namespace DragonPlus.Config.ChristmaxBlindBox
{
    public partial class ChristmaxBlindBoxConfigManager : Manager<ChristmaxBlindBoxConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<ChristmasBlindBoxGlobalConfig> ChristmasBlindBoxGlobalConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(ChristmasBlindBoxGlobalConfig)] = "ChristmasBlindBoxGlobalConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("christmasblindboxglobalconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "ChristmasBlindBoxGlobalConfig": cfg = ChristmasBlindBoxGlobalConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/ChristmaxBlindBox/christmasblindbox");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/ChristmaxBlindBox/christmasblindbox error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            ChristmasBlindBoxGlobalConfigList = JsonConvert.DeserializeObject<List<ChristmasBlindBoxGlobalConfig>>(JsonConvert.SerializeObject(table["christmasblindboxglobalconfig"]));
            
        }
    }
}