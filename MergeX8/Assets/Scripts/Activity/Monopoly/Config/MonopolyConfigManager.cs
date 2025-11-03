/************************************************
 * Monopoly Config Manager class : MonopolyConfigManager
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

namespace DragonPlus.Config.Monopoly
{
    public partial class MonopolyConfigManager : Manager<MonopolyConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<MonopolyGlobalConfig> MonopolyGlobalConfigList;
        public List<MonopolyBlockConfig> MonopolyBlockConfigList;
        public List<MonopolyRewardBoxConfig> MonopolyRewardBoxConfigList;
        public List<MonopolyCardConfig> MonopolyCardConfigList;
        public List<MonopolyDiceConfig> MonopolyDiceConfigList;
        public List<MonopolyMiniGameConfig> MonopolyMiniGameConfigList;
        public List<MonopolyStoreLevelConfig> MonopolyStoreLevelConfigList;
        public List<MonopolyStoreItemConfig> MonopolyStoreItemConfigList;
        public List<MonopolyTaskRewardConfig> MonopolyTaskRewardConfigList;
        public List<MonopolyLeaderBoardRewardConfig> MonopolyLeaderBoardRewardConfigList;
        public List<MonopolyBuyDiceConfig> MonopolyBuyDiceConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(MonopolyGlobalConfig)] = "MonopolyGlobalConfig",
            [typeof(MonopolyBlockConfig)] = "MonopolyBlockConfig",
            [typeof(MonopolyRewardBoxConfig)] = "MonopolyRewardBoxConfig",
            [typeof(MonopolyCardConfig)] = "MonopolyCardConfig",
            [typeof(MonopolyDiceConfig)] = "MonopolyDiceConfig",
            [typeof(MonopolyMiniGameConfig)] = "MonopolyMiniGameConfig",
            [typeof(MonopolyStoreLevelConfig)] = "MonopolyStoreLevelConfig",
            [typeof(MonopolyStoreItemConfig)] = "MonopolyStoreItemConfig",
            [typeof(MonopolyTaskRewardConfig)] = "MonopolyTaskRewardConfig",
            [typeof(MonopolyLeaderBoardRewardConfig)] = "MonopolyLeaderBoardRewardConfig",
            [typeof(MonopolyBuyDiceConfig)] = "MonopolyBuyDiceConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("monopolyglobalconfig")) return false;
            if (!table.ContainsKey("monopolyblockconfig")) return false;
            if (!table.ContainsKey("monopolyrewardboxconfig")) return false;
            if (!table.ContainsKey("monopolycardconfig")) return false;
            if (!table.ContainsKey("monopolydiceconfig")) return false;
            if (!table.ContainsKey("monopolyminigameconfig")) return false;
            if (!table.ContainsKey("monopolystorelevelconfig")) return false;
            if (!table.ContainsKey("monopolystoreitemconfig")) return false;
            if (!table.ContainsKey("monopolytaskrewardconfig")) return false;
            if (!table.ContainsKey("monopolyleaderboardrewardconfig")) return false;
            if (!table.ContainsKey("monopolybuydiceconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "MonopolyGlobalConfig": cfg = MonopolyGlobalConfigList as List<T>; break;
                case "MonopolyBlockConfig": cfg = MonopolyBlockConfigList as List<T>; break;
                case "MonopolyRewardBoxConfig": cfg = MonopolyRewardBoxConfigList as List<T>; break;
                case "MonopolyCardConfig": cfg = MonopolyCardConfigList as List<T>; break;
                case "MonopolyDiceConfig": cfg = MonopolyDiceConfigList as List<T>; break;
                case "MonopolyMiniGameConfig": cfg = MonopolyMiniGameConfigList as List<T>; break;
                case "MonopolyStoreLevelConfig": cfg = MonopolyStoreLevelConfigList as List<T>; break;
                case "MonopolyStoreItemConfig": cfg = MonopolyStoreItemConfigList as List<T>; break;
                case "MonopolyTaskRewardConfig": cfg = MonopolyTaskRewardConfigList as List<T>; break;
                case "MonopolyLeaderBoardRewardConfig": cfg = MonopolyLeaderBoardRewardConfigList as List<T>; break;
                case "MonopolyBuyDiceConfig": cfg = MonopolyBuyDiceConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/Monopoly/monopoly");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/Monopoly/monopoly error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            MonopolyGlobalConfigList = JsonConvert.DeserializeObject<List<MonopolyGlobalConfig>>(JsonConvert.SerializeObject(table["monopolyglobalconfig"]));
            MonopolyBlockConfigList = JsonConvert.DeserializeObject<List<MonopolyBlockConfig>>(JsonConvert.SerializeObject(table["monopolyblockconfig"]));
            MonopolyRewardBoxConfigList = JsonConvert.DeserializeObject<List<MonopolyRewardBoxConfig>>(JsonConvert.SerializeObject(table["monopolyrewardboxconfig"]));
            MonopolyCardConfigList = JsonConvert.DeserializeObject<List<MonopolyCardConfig>>(JsonConvert.SerializeObject(table["monopolycardconfig"]));
            MonopolyDiceConfigList = JsonConvert.DeserializeObject<List<MonopolyDiceConfig>>(JsonConvert.SerializeObject(table["monopolydiceconfig"]));
            MonopolyMiniGameConfigList = JsonConvert.DeserializeObject<List<MonopolyMiniGameConfig>>(JsonConvert.SerializeObject(table["monopolyminigameconfig"]));
            MonopolyStoreLevelConfigList = JsonConvert.DeserializeObject<List<MonopolyStoreLevelConfig>>(JsonConvert.SerializeObject(table["monopolystorelevelconfig"]));
            MonopolyStoreItemConfigList = JsonConvert.DeserializeObject<List<MonopolyStoreItemConfig>>(JsonConvert.SerializeObject(table["monopolystoreitemconfig"]));
            MonopolyTaskRewardConfigList = JsonConvert.DeserializeObject<List<MonopolyTaskRewardConfig>>(JsonConvert.SerializeObject(table["monopolytaskrewardconfig"]));
            MonopolyLeaderBoardRewardConfigList = JsonConvert.DeserializeObject<List<MonopolyLeaderBoardRewardConfig>>(JsonConvert.SerializeObject(table["monopolyleaderboardrewardconfig"]));
            MonopolyBuyDiceConfigList = JsonConvert.DeserializeObject<List<MonopolyBuyDiceConfig>>(JsonConvert.SerializeObject(table["monopolybuydiceconfig"]));
            
        }
    }
}