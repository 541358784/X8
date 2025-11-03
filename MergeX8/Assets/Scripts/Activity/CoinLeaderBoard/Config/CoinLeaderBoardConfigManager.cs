/************************************************
 * CoinLeaderBoard Config Manager class : CoinLeaderBoardConfigManager
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

namespace DragonPlus.Config.CoinLeaderBoard
{
    public partial class CoinLeaderBoardConfigManager : Manager<CoinLeaderBoardConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<CoinLeaderBoardRewardConfig> CoinLeaderBoardRewardConfigList;
        public List<CoinLeaderBoardPlayerCountConfig> CoinLeaderBoardPlayerCountConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(CoinLeaderBoardRewardConfig)] = "CoinLeaderBoardRewardConfig",
            [typeof(CoinLeaderBoardPlayerCountConfig)] = "CoinLeaderBoardPlayerCountConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("coinleaderboardrewardconfig")) return false;
            if (!table.ContainsKey("coinleaderboardplayercountconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "CoinLeaderBoardRewardConfig": cfg = CoinLeaderBoardRewardConfigList as List<T>; break;
                case "CoinLeaderBoardPlayerCountConfig": cfg = CoinLeaderBoardPlayerCountConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/CoinLeaderBoard/coinleaderboard");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/CoinLeaderBoard/coinleaderboard error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            CoinLeaderBoardRewardConfigList = JsonConvert.DeserializeObject<List<CoinLeaderBoardRewardConfig>>(JsonConvert.SerializeObject(table["coinleaderboardrewardconfig"]));
            CoinLeaderBoardPlayerCountConfigList = JsonConvert.DeserializeObject<List<CoinLeaderBoardPlayerCountConfig>>(JsonConvert.SerializeObject(table["coinleaderboardplayercountconfig"]));
            
        }
    }
}