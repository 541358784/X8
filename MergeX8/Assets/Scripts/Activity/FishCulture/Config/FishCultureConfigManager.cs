/************************************************
 * FishCulture Config Manager class : FishCultureConfigManager
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

namespace DragonPlus.Config.FishCulture
{
    public partial class FishCultureConfigManager : Manager<FishCultureConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<FishCultureRewardConfig> FishCultureRewardConfigList;
        public List<FishCultureTaskRewardConfig> FishCultureTaskRewardConfigList;
        public List<FishCultureGlobalConfig> FishCultureGlobalConfigList;
        public List<FishCultureLeaderBoardRewardConfig> FishCultureLeaderBoardRewardConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(FishCultureRewardConfig)] = "FishCultureRewardConfig",
            [typeof(FishCultureTaskRewardConfig)] = "FishCultureTaskRewardConfig",
            [typeof(FishCultureGlobalConfig)] = "FishCultureGlobalConfig",
            [typeof(FishCultureLeaderBoardRewardConfig)] = "FishCultureLeaderBoardRewardConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("fishculturerewardconfig")) return false;
            if (!table.ContainsKey("fishculturetaskrewardconfig")) return false;
            if (!table.ContainsKey("fishcultureglobalconfig")) return false;
            if (!table.ContainsKey("fishcultureleaderboardrewardconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "FishCultureRewardConfig": cfg = FishCultureRewardConfigList as List<T>; break;
                case "FishCultureTaskRewardConfig": cfg = FishCultureTaskRewardConfigList as List<T>; break;
                case "FishCultureGlobalConfig": cfg = FishCultureGlobalConfigList as List<T>; break;
                case "FishCultureLeaderBoardRewardConfig": cfg = FishCultureLeaderBoardRewardConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/FishCulture/fishculture");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/FishCulture/fishculture error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            FishCultureRewardConfigList = JsonConvert.DeserializeObject<List<FishCultureRewardConfig>>(JsonConvert.SerializeObject(table["fishculturerewardconfig"]));
            FishCultureTaskRewardConfigList = JsonConvert.DeserializeObject<List<FishCultureTaskRewardConfig>>(JsonConvert.SerializeObject(table["fishculturetaskrewardconfig"]));
            FishCultureGlobalConfigList = JsonConvert.DeserializeObject<List<FishCultureGlobalConfig>>(JsonConvert.SerializeObject(table["fishcultureglobalconfig"]));
            FishCultureLeaderBoardRewardConfigList = JsonConvert.DeserializeObject<List<FishCultureLeaderBoardRewardConfig>>(JsonConvert.SerializeObject(table["fishcultureleaderboardrewardconfig"]));
            
        }
    }
}