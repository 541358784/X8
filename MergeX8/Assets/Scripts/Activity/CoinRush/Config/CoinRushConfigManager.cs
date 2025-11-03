/************************************************
 * CoinRush Config Manager class : CoinRushConfigManager
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

namespace DragonPlus.Config.CoinRush
{
    public partial class CoinRushConfigManager : Manager<CoinRushConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<CoinRushTaskConfig> CoinRushTaskConfigList;
        public List<PreheatConfig> PreheatConfigList;
        public List<LastRewardConfig> LastRewardConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(CoinRushTaskConfig)] = "CoinRushTaskConfig",
            [typeof(PreheatConfig)] = "PreheatConfig",
            [typeof(LastRewardConfig)] = "LastRewardConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("coinrushtaskconfig")) return false;
            if (!table.ContainsKey("preheatconfig")) return false;
            if (!table.ContainsKey("lastrewardconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "CoinRushTaskConfig": cfg = CoinRushTaskConfigList as List<T>; break;
                case "PreheatConfig": cfg = PreheatConfigList as List<T>; break;
                case "LastRewardConfig": cfg = LastRewardConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/CoinRush/coinrush");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/CoinRush/coinrush error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            CoinRushTaskConfigList = JsonConvert.DeserializeObject<List<CoinRushTaskConfig>>(JsonConvert.SerializeObject(table["coinrushtaskconfig"]));
            PreheatConfigList = JsonConvert.DeserializeObject<List<PreheatConfig>>(JsonConvert.SerializeObject(table["preheatconfig"]));
            LastRewardConfigList = JsonConvert.DeserializeObject<List<LastRewardConfig>>(JsonConvert.SerializeObject(table["lastrewardconfig"]));
            
        }
    }
}