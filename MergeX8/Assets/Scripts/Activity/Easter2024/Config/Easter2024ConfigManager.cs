/************************************************
 * Easter2024 Config Manager class : Easter2024ConfigManager
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

namespace DragonPlus.Config.Easter2024
{
    public partial class Easter2024ConfigManager : Manager<Easter2024ConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<Easter2024GlobalConfig> Easter2024GlobalConfigList;
        public List<Easter2024LevelConfig> Easter2024LevelConfigList;
        public List<Easter2024CardConfig> Easter2024CardConfigList;
        public List<Easter2024StoreLevelConfig> Easter2024StoreLevelConfigList;
        public List<Easter2024StoreItemConfig> Easter2024StoreItemConfigList;
        public List<Easter2024MiniGameConfig> Easter2024MiniGameConfigList;
        public List<Easter2024TaskRewardConfig> Easter2024TaskRewardConfigList;
        public List<Easter2024LeaderBoardRewardConfig> Easter2024LeaderBoardRewardConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(Easter2024GlobalConfig)] = "Easter2024GlobalConfig",
            [typeof(Easter2024LevelConfig)] = "Easter2024LevelConfig",
            [typeof(Easter2024CardConfig)] = "Easter2024CardConfig",
            [typeof(Easter2024StoreLevelConfig)] = "Easter2024StoreLevelConfig",
            [typeof(Easter2024StoreItemConfig)] = "Easter2024StoreItemConfig",
            [typeof(Easter2024MiniGameConfig)] = "Easter2024MiniGameConfig",
            [typeof(Easter2024TaskRewardConfig)] = "Easter2024TaskRewardConfig",
            [typeof(Easter2024LeaderBoardRewardConfig)] = "Easter2024LeaderBoardRewardConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("easter2024globalconfig")) return false;
            if (!table.ContainsKey("easter2024levelconfig")) return false;
            if (!table.ContainsKey("easter2024cardconfig")) return false;
            if (!table.ContainsKey("easter2024storelevelconfig")) return false;
            if (!table.ContainsKey("easter2024storeitemconfig")) return false;
            if (!table.ContainsKey("easter2024minigameconfig")) return false;
            if (!table.ContainsKey("easter2024taskrewardconfig")) return false;
            if (!table.ContainsKey("easter2024leaderboardrewardconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "Easter2024GlobalConfig": cfg = Easter2024GlobalConfigList as List<T>; break;
                case "Easter2024LevelConfig": cfg = Easter2024LevelConfigList as List<T>; break;
                case "Easter2024CardConfig": cfg = Easter2024CardConfigList as List<T>; break;
                case "Easter2024StoreLevelConfig": cfg = Easter2024StoreLevelConfigList as List<T>; break;
                case "Easter2024StoreItemConfig": cfg = Easter2024StoreItemConfigList as List<T>; break;
                case "Easter2024MiniGameConfig": cfg = Easter2024MiniGameConfigList as List<T>; break;
                case "Easter2024TaskRewardConfig": cfg = Easter2024TaskRewardConfigList as List<T>; break;
                case "Easter2024LeaderBoardRewardConfig": cfg = Easter2024LeaderBoardRewardConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/Easter2024/easter2024");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/Easter2024/easter2024 error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            Easter2024GlobalConfigList = JsonConvert.DeserializeObject<List<Easter2024GlobalConfig>>(JsonConvert.SerializeObject(table["easter2024globalconfig"]));
            Easter2024LevelConfigList = JsonConvert.DeserializeObject<List<Easter2024LevelConfig>>(JsonConvert.SerializeObject(table["easter2024levelconfig"]));
            Easter2024CardConfigList = JsonConvert.DeserializeObject<List<Easter2024CardConfig>>(JsonConvert.SerializeObject(table["easter2024cardconfig"]));
            Easter2024StoreLevelConfigList = JsonConvert.DeserializeObject<List<Easter2024StoreLevelConfig>>(JsonConvert.SerializeObject(table["easter2024storelevelconfig"]));
            Easter2024StoreItemConfigList = JsonConvert.DeserializeObject<List<Easter2024StoreItemConfig>>(JsonConvert.SerializeObject(table["easter2024storeitemconfig"]));
            Easter2024MiniGameConfigList = JsonConvert.DeserializeObject<List<Easter2024MiniGameConfig>>(JsonConvert.SerializeObject(table["easter2024minigameconfig"]));
            Easter2024TaskRewardConfigList = JsonConvert.DeserializeObject<List<Easter2024TaskRewardConfig>>(JsonConvert.SerializeObject(table["easter2024taskrewardconfig"]));
            Easter2024LeaderBoardRewardConfigList = JsonConvert.DeserializeObject<List<Easter2024LeaderBoardRewardConfig>>(JsonConvert.SerializeObject(table["easter2024leaderboardrewardconfig"]));
            
        }
    }
}