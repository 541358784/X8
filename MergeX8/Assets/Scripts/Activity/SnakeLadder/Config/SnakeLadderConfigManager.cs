/************************************************
 * SnakeLadder Config Manager class : SnakeLadderConfigManager
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

namespace DragonPlus.Config.SnakeLadder
{
    public partial class SnakeLadderConfigManager : Manager<SnakeLadderConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<SnakeLadderGlobalConfig> SnakeLadderGlobalConfigList;
        public List<SnakeLadderLevelConfig> SnakeLadderLevelConfigList;
        public List<SnakeLadderCardConfig> SnakeLadderCardConfigList;
        public List<SnakeLadderBlockConfig> SnakeLadderBlockConfigList;
        public List<SnakeLadderStoreLevelConfig> SnakeLadderStoreLevelConfigList;
        public List<SnakeLadderStoreItemConfig> SnakeLadderStoreItemConfigList;
        public List<SnakeLadderTaskRewardConfig> SnakeLadderTaskRewardConfigList;
        public List<SnakeLadderLeaderBoardRewardConfig> SnakeLadderLeaderBoardRewardConfigList;
        public List<SnakeLadderBuyTurntableConfig> SnakeLadderBuyTurntableConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(SnakeLadderGlobalConfig)] = "SnakeLadderGlobalConfig",
            [typeof(SnakeLadderLevelConfig)] = "SnakeLadderLevelConfig",
            [typeof(SnakeLadderCardConfig)] = "SnakeLadderCardConfig",
            [typeof(SnakeLadderBlockConfig)] = "SnakeLadderBlockConfig",
            [typeof(SnakeLadderStoreLevelConfig)] = "SnakeLadderStoreLevelConfig",
            [typeof(SnakeLadderStoreItemConfig)] = "SnakeLadderStoreItemConfig",
            [typeof(SnakeLadderTaskRewardConfig)] = "SnakeLadderTaskRewardConfig",
            [typeof(SnakeLadderLeaderBoardRewardConfig)] = "SnakeLadderLeaderBoardRewardConfig",
            [typeof(SnakeLadderBuyTurntableConfig)] = "SnakeLadderBuyTurntableConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("snakeladderglobalconfig")) return false;
            if (!table.ContainsKey("snakeladderlevelconfig")) return false;
            if (!table.ContainsKey("snakeladdercardconfig")) return false;
            if (!table.ContainsKey("snakeladderblockconfig")) return false;
            if (!table.ContainsKey("snakeladderstorelevelconfig")) return false;
            if (!table.ContainsKey("snakeladderstoreitemconfig")) return false;
            if (!table.ContainsKey("snakeladdertaskrewardconfig")) return false;
            if (!table.ContainsKey("snakeladderleaderboardrewardconfig")) return false;
            if (!table.ContainsKey("snakeladderbuyturntableconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "SnakeLadderGlobalConfig": cfg = SnakeLadderGlobalConfigList as List<T>; break;
                case "SnakeLadderLevelConfig": cfg = SnakeLadderLevelConfigList as List<T>; break;
                case "SnakeLadderCardConfig": cfg = SnakeLadderCardConfigList as List<T>; break;
                case "SnakeLadderBlockConfig": cfg = SnakeLadderBlockConfigList as List<T>; break;
                case "SnakeLadderStoreLevelConfig": cfg = SnakeLadderStoreLevelConfigList as List<T>; break;
                case "SnakeLadderStoreItemConfig": cfg = SnakeLadderStoreItemConfigList as List<T>; break;
                case "SnakeLadderTaskRewardConfig": cfg = SnakeLadderTaskRewardConfigList as List<T>; break;
                case "SnakeLadderLeaderBoardRewardConfig": cfg = SnakeLadderLeaderBoardRewardConfigList as List<T>; break;
                case "SnakeLadderBuyTurntableConfig": cfg = SnakeLadderBuyTurntableConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/SnakeLadder/snakeladder");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/SnakeLadder/snakeladder error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            SnakeLadderGlobalConfigList = JsonConvert.DeserializeObject<List<SnakeLadderGlobalConfig>>(JsonConvert.SerializeObject(table["snakeladderglobalconfig"]));
            SnakeLadderLevelConfigList = JsonConvert.DeserializeObject<List<SnakeLadderLevelConfig>>(JsonConvert.SerializeObject(table["snakeladderlevelconfig"]));
            SnakeLadderCardConfigList = JsonConvert.DeserializeObject<List<SnakeLadderCardConfig>>(JsonConvert.SerializeObject(table["snakeladdercardconfig"]));
            SnakeLadderBlockConfigList = JsonConvert.DeserializeObject<List<SnakeLadderBlockConfig>>(JsonConvert.SerializeObject(table["snakeladderblockconfig"]));
            SnakeLadderStoreLevelConfigList = JsonConvert.DeserializeObject<List<SnakeLadderStoreLevelConfig>>(JsonConvert.SerializeObject(table["snakeladderstorelevelconfig"]));
            SnakeLadderStoreItemConfigList = JsonConvert.DeserializeObject<List<SnakeLadderStoreItemConfig>>(JsonConvert.SerializeObject(table["snakeladderstoreitemconfig"]));
            SnakeLadderTaskRewardConfigList = JsonConvert.DeserializeObject<List<SnakeLadderTaskRewardConfig>>(JsonConvert.SerializeObject(table["snakeladdertaskrewardconfig"]));
            SnakeLadderLeaderBoardRewardConfigList = JsonConvert.DeserializeObject<List<SnakeLadderLeaderBoardRewardConfig>>(JsonConvert.SerializeObject(table["snakeladderleaderboardrewardconfig"]));
            SnakeLadderBuyTurntableConfigList = JsonConvert.DeserializeObject<List<SnakeLadderBuyTurntableConfig>>(JsonConvert.SerializeObject(table["snakeladderbuyturntableconfig"]));
            
        }
    }
}