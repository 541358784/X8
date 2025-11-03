/************************************************
 * CoinCompetition Config Manager class : CoinCompetitionConfigManager
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

namespace DragonPlus.Config.CoinCompetition
{
    public partial class CoinCompetitionConfigManager : Manager<CoinCompetitionConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<CoinCompetitionConfig> CoinCompetitionConfigList;
        public List<CoinCompetitionReward> CoinCompetitionRewardList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(CoinCompetitionConfig)] = "CoinCompetitionConfig",
            [typeof(CoinCompetitionReward)] = "CoinCompetitionReward",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("coincompetitionconfig")) return false;
            if (!table.ContainsKey("coincompetitionreward")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "CoinCompetitionConfig": cfg = CoinCompetitionConfigList as List<T>; break;
                case "CoinCompetitionReward": cfg = CoinCompetitionRewardList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/CoinCompetition/coincompetition");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/CoinCompetition/coincompetition error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            CoinCompetitionConfigList = JsonConvert.DeserializeObject<List<CoinCompetitionConfig>>(JsonConvert.SerializeObject(table["coincompetitionconfig"]));
            CoinCompetitionRewardList = JsonConvert.DeserializeObject<List<CoinCompetitionReward>>(JsonConvert.SerializeObject(table["coincompetitionreward"]));
            
        }
    }
}