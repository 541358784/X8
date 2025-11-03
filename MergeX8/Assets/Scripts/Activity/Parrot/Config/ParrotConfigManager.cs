/************************************************
 * Parrot Config Manager class : ParrotConfigManager
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

namespace DragonPlus.Config.Parrot
{
    public partial class ParrotConfigManager : Manager<ParrotConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<ParrotRewardConfig> ParrotRewardConfigList;
        public List<ParrotTaskRewardConfig> ParrotTaskRewardConfigList;
        public List<ParrotGlobalConfig> ParrotGlobalConfigList;
        public List<ParrotLeaderBoardRewardConfig> ParrotLeaderBoardRewardConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(ParrotRewardConfig)] = "ParrotRewardConfig",
            [typeof(ParrotTaskRewardConfig)] = "ParrotTaskRewardConfig",
            [typeof(ParrotGlobalConfig)] = "ParrotGlobalConfig",
            [typeof(ParrotLeaderBoardRewardConfig)] = "ParrotLeaderBoardRewardConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("parrotrewardconfig")) return false;
            if (!table.ContainsKey("parrottaskrewardconfig")) return false;
            if (!table.ContainsKey("parrotglobalconfig")) return false;
            if (!table.ContainsKey("parrotleaderboardrewardconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "ParrotRewardConfig": cfg = ParrotRewardConfigList as List<T>; break;
                case "ParrotTaskRewardConfig": cfg = ParrotTaskRewardConfigList as List<T>; break;
                case "ParrotGlobalConfig": cfg = ParrotGlobalConfigList as List<T>; break;
                case "ParrotLeaderBoardRewardConfig": cfg = ParrotLeaderBoardRewardConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/Parrot/parrot");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/Parrot/parrot error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            ParrotRewardConfigList = JsonConvert.DeserializeObject<List<ParrotRewardConfig>>(JsonConvert.SerializeObject(table["parrotrewardconfig"]));
            ParrotTaskRewardConfigList = JsonConvert.DeserializeObject<List<ParrotTaskRewardConfig>>(JsonConvert.SerializeObject(table["parrottaskrewardconfig"]));
            ParrotGlobalConfigList = JsonConvert.DeserializeObject<List<ParrotGlobalConfig>>(JsonConvert.SerializeObject(table["parrotglobalconfig"]));
            ParrotLeaderBoardRewardConfigList = JsonConvert.DeserializeObject<List<ParrotLeaderBoardRewardConfig>>(JsonConvert.SerializeObject(table["parrotleaderboardrewardconfig"]));
            
        }
    }
}