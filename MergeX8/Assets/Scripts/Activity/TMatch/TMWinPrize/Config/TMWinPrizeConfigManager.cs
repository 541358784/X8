/************************************************
 * TMWinPrize Config Manager class : TMWinPrizeConfigManager
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

namespace DragonPlus.Config.TMWinPrize
{
    public partial class TMWinPrizeConfigManager : Manager<TMWinPrizeConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<TMWinPrizeRewardConfig> TMWinPrizeRewardConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TMWinPrizeRewardConfig)] = "TMWinPrizeRewardConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("tmwinprizerewardconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TMWinPrizeRewardConfig": cfg = TMWinPrizeRewardConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/TMatch/TMWinPrize/tmwinprize");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/TMatch/TMWinPrize/tmwinprize error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TMWinPrizeRewardConfigList = JsonConvert.DeserializeObject<List<TMWinPrizeRewardConfig>>(JsonConvert.SerializeObject(table["tmwinprizerewardconfig"]));
            
        }
    }
}