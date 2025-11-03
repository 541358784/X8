/************************************************
 * FlowerField Config Manager class : FlowerFieldConfigManager
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

namespace DragonPlus.Config.FlowerField
{
    public partial class FlowerFieldConfigManager : Manager<FlowerFieldConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<FlowerFieldRewardConfig> FlowerFieldRewardConfigList;
        public List<FlowerFieldTaskRewardConfig> FlowerFieldTaskRewardConfigList;
        public List<FlowerFieldGlobalConfig> FlowerFieldGlobalConfigList;
        public List<FlowerFieldLeaderBoardRewardConfig> FlowerFieldLeaderBoardRewardConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(FlowerFieldRewardConfig)] = "FlowerFieldRewardConfig",
            [typeof(FlowerFieldTaskRewardConfig)] = "FlowerFieldTaskRewardConfig",
            [typeof(FlowerFieldGlobalConfig)] = "FlowerFieldGlobalConfig",
            [typeof(FlowerFieldLeaderBoardRewardConfig)] = "FlowerFieldLeaderBoardRewardConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("flowerfieldrewardconfig")) return false;
            if (!table.ContainsKey("flowerfieldtaskrewardconfig")) return false;
            if (!table.ContainsKey("flowerfieldglobalconfig")) return false;
            if (!table.ContainsKey("flowerfieldleaderboardrewardconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "FlowerFieldRewardConfig": cfg = FlowerFieldRewardConfigList as List<T>; break;
                case "FlowerFieldTaskRewardConfig": cfg = FlowerFieldTaskRewardConfigList as List<T>; break;
                case "FlowerFieldGlobalConfig": cfg = FlowerFieldGlobalConfigList as List<T>; break;
                case "FlowerFieldLeaderBoardRewardConfig": cfg = FlowerFieldLeaderBoardRewardConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/FlowerField/flowerfield");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/FlowerField/flowerfield error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            FlowerFieldRewardConfigList = JsonConvert.DeserializeObject<List<FlowerFieldRewardConfig>>(JsonConvert.SerializeObject(table["flowerfieldrewardconfig"]));
            FlowerFieldTaskRewardConfigList = JsonConvert.DeserializeObject<List<FlowerFieldTaskRewardConfig>>(JsonConvert.SerializeObject(table["flowerfieldtaskrewardconfig"]));
            FlowerFieldGlobalConfigList = JsonConvert.DeserializeObject<List<FlowerFieldGlobalConfig>>(JsonConvert.SerializeObject(table["flowerfieldglobalconfig"]));
            FlowerFieldLeaderBoardRewardConfigList = JsonConvert.DeserializeObject<List<FlowerFieldLeaderBoardRewardConfig>>(JsonConvert.SerializeObject(table["flowerfieldleaderboardrewardconfig"]));
            
        }
    }
}