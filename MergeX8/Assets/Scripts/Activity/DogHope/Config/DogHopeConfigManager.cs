/************************************************
 * DogHope Config Manager class : DogHopeConfigManager
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

namespace DragonPlus.Config.DogHope
{
    public partial class DogHopeConfigManager : Manager<DogHopeConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<DogHopeReward> DogHopeRewardList;
        public List<DogHopeGlobalConfig> DogHopeGlobalConfigList;
        public List<DogHopeLeaderBoardRewardConfig> DogHopeLeaderBoardRewardConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(DogHopeReward)] = "DogHopeReward",
            [typeof(DogHopeGlobalConfig)] = "DogHopeGlobalConfig",
            [typeof(DogHopeLeaderBoardRewardConfig)] = "DogHopeLeaderBoardRewardConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("doghopereward")) return false;
            if (!table.ContainsKey("doghopeglobalconfig")) return false;
            if (!table.ContainsKey("doghopeleaderboardrewardconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "DogHopeReward": cfg = DogHopeRewardList as List<T>; break;
                case "DogHopeGlobalConfig": cfg = DogHopeGlobalConfigList as List<T>; break;
                case "DogHopeLeaderBoardRewardConfig": cfg = DogHopeLeaderBoardRewardConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/DogHope/doghope");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/DogHope/doghope error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            DogHopeRewardList = JsonConvert.DeserializeObject<List<DogHopeReward>>(JsonConvert.SerializeObject(table["doghopereward"]));
            DogHopeGlobalConfigList = JsonConvert.DeserializeObject<List<DogHopeGlobalConfig>>(JsonConvert.SerializeObject(table["doghopeglobalconfig"]));
            DogHopeLeaderBoardRewardConfigList = JsonConvert.DeserializeObject<List<DogHopeLeaderBoardRewardConfig>>(JsonConvert.SerializeObject(table["doghopeleaderboardrewardconfig"]));
            
        }
    }
}