/************************************************
 * ClimbTree Config Manager class : ClimbTreeConfigManager
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

namespace DragonPlus.Config.ClimbTree
{
    public partial class ClimbTreeConfigManager : Manager<ClimbTreeConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<ClimbTreeConfig> ClimbTreeConfigList;
        public List<ClimbTreeTaskRewardConfig> ClimbTreeTaskRewardConfigList;
        public List<ClimbTreeGlobalConfig> ClimbTreeGlobalConfigList;
        public List<ClimbTreeLeaderBoardRewardConfig> ClimbTreeLeaderBoardRewardConfigList;
        public List<ClimbTreeProductConfig> ClimbTreeProductConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(ClimbTreeConfig)] = "ClimbTreeConfig",
            [typeof(ClimbTreeTaskRewardConfig)] = "ClimbTreeTaskRewardConfig",
            [typeof(ClimbTreeGlobalConfig)] = "ClimbTreeGlobalConfig",
            [typeof(ClimbTreeLeaderBoardRewardConfig)] = "ClimbTreeLeaderBoardRewardConfig",
            [typeof(ClimbTreeProductConfig)] = "ClimbTreeProductConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("climbtreeconfig")) return false;
            if (!table.ContainsKey("climbtreetaskrewardconfig")) return false;
            if (!table.ContainsKey("climbtreeglobalconfig")) return false;
            if (!table.ContainsKey("climbtreeleaderboardrewardconfig")) return false;
            if (!table.ContainsKey("climbtreeproductconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "ClimbTreeConfig": cfg = ClimbTreeConfigList as List<T>; break;
                case "ClimbTreeTaskRewardConfig": cfg = ClimbTreeTaskRewardConfigList as List<T>; break;
                case "ClimbTreeGlobalConfig": cfg = ClimbTreeGlobalConfigList as List<T>; break;
                case "ClimbTreeLeaderBoardRewardConfig": cfg = ClimbTreeLeaderBoardRewardConfigList as List<T>; break;
                case "ClimbTreeProductConfig": cfg = ClimbTreeProductConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/ClimbTree/climbtree");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/ClimbTree/climbtree error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            ClimbTreeConfigList = JsonConvert.DeserializeObject<List<ClimbTreeConfig>>(JsonConvert.SerializeObject(table["climbtreeconfig"]));
            ClimbTreeTaskRewardConfigList = JsonConvert.DeserializeObject<List<ClimbTreeTaskRewardConfig>>(JsonConvert.SerializeObject(table["climbtreetaskrewardconfig"]));
            ClimbTreeGlobalConfigList = JsonConvert.DeserializeObject<List<ClimbTreeGlobalConfig>>(JsonConvert.SerializeObject(table["climbtreeglobalconfig"]));
            ClimbTreeLeaderBoardRewardConfigList = JsonConvert.DeserializeObject<List<ClimbTreeLeaderBoardRewardConfig>>(JsonConvert.SerializeObject(table["climbtreeleaderboardrewardconfig"]));
            ClimbTreeProductConfigList = JsonConvert.DeserializeObject<List<ClimbTreeProductConfig>>(JsonConvert.SerializeObject(table["climbtreeproductconfig"]));
            
        }
    }
}