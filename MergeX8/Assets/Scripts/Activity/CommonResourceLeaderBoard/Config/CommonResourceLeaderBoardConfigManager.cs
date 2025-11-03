/************************************************
 * CommonResourceLeaderBoard Config Manager class : CommonResourceLeaderBoardConfigManager
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

namespace DragonPlus.Config.CommonResourceLeaderBoard
{
    public partial class CommonResourceLeaderBoardConfigManager : Manager<CommonResourceLeaderBoardConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<CommonResourceLeaderBoardGlobalConfig> CommonResourceLeaderBoardGlobalConfigList;
        public List<CommonResourceLeaderBoardRewardConfig> CommonResourceLeaderBoardRewardConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(CommonResourceLeaderBoardGlobalConfig)] = "CommonResourceLeaderBoardGlobalConfig",
            [typeof(CommonResourceLeaderBoardRewardConfig)] = "CommonResourceLeaderBoardRewardConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("commonresourceleaderboardglobalconfig")) return false;
            if (!table.ContainsKey("commonresourceleaderboardrewardconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "CommonResourceLeaderBoardGlobalConfig": cfg = CommonResourceLeaderBoardGlobalConfigList as List<T>; break;
                case "CommonResourceLeaderBoardRewardConfig": cfg = CommonResourceLeaderBoardRewardConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/CommonResourceLeaderBoard/commonresourceleaderboard");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/CommonResourceLeaderBoard/commonresourceleaderboard error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            CommonResourceLeaderBoardGlobalConfigList = JsonConvert.DeserializeObject<List<CommonResourceLeaderBoardGlobalConfig>>(JsonConvert.SerializeObject(table["commonresourceleaderboardglobalconfig"]));
            CommonResourceLeaderBoardRewardConfigList = JsonConvert.DeserializeObject<List<CommonResourceLeaderBoardRewardConfig>>(JsonConvert.SerializeObject(table["commonresourceleaderboardrewardconfig"]));
            
        }
    }
}