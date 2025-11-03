/************************************************
 * StarrySkyCompass Config Manager class : StarrySkyCompassConfigManager
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

namespace DragonPlus.Config.StarrySkyCompass
{
    public partial class StarrySkyCompassConfigManager : Manager<StarrySkyCompassConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<StarrySkyCompassGlobalConfig> StarrySkyCompassGlobalConfigList;
        public List<StarrySkyCompassTurntableConfig> StarrySkyCompassTurntableConfigList;
        public List<StarrySkyCompassResultConfig> StarrySkyCompassResultConfigList;
        public List<StarrySkyCompassTaskRewardConfig> StarrySkyCompassTaskRewardConfigList;
        public List<StarrySkyCompassShopConfig> StarrySkyCompassShopConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(StarrySkyCompassGlobalConfig)] = "StarrySkyCompassGlobalConfig",
            [typeof(StarrySkyCompassTurntableConfig)] = "StarrySkyCompassTurntableConfig",
            [typeof(StarrySkyCompassResultConfig)] = "StarrySkyCompassResultConfig",
            [typeof(StarrySkyCompassTaskRewardConfig)] = "StarrySkyCompassTaskRewardConfig",
            [typeof(StarrySkyCompassShopConfig)] = "StarrySkyCompassShopConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("starryskycompassglobalconfig")) return false;
            if (!table.ContainsKey("starryskycompassturntableconfig")) return false;
            if (!table.ContainsKey("starryskycompassresultconfig")) return false;
            if (!table.ContainsKey("starryskycompasstaskrewardconfig")) return false;
            if (!table.ContainsKey("starryskycompassshopconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "StarrySkyCompassGlobalConfig": cfg = StarrySkyCompassGlobalConfigList as List<T>; break;
                case "StarrySkyCompassTurntableConfig": cfg = StarrySkyCompassTurntableConfigList as List<T>; break;
                case "StarrySkyCompassResultConfig": cfg = StarrySkyCompassResultConfigList as List<T>; break;
                case "StarrySkyCompassTaskRewardConfig": cfg = StarrySkyCompassTaskRewardConfigList as List<T>; break;
                case "StarrySkyCompassShopConfig": cfg = StarrySkyCompassShopConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/StarrySkyCompass/starryskycompass");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/StarrySkyCompass/starryskycompass error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            StarrySkyCompassGlobalConfigList = JsonConvert.DeserializeObject<List<StarrySkyCompassGlobalConfig>>(JsonConvert.SerializeObject(table["starryskycompassglobalconfig"]));
            StarrySkyCompassTurntableConfigList = JsonConvert.DeserializeObject<List<StarrySkyCompassTurntableConfig>>(JsonConvert.SerializeObject(table["starryskycompassturntableconfig"]));
            StarrySkyCompassResultConfigList = JsonConvert.DeserializeObject<List<StarrySkyCompassResultConfig>>(JsonConvert.SerializeObject(table["starryskycompassresultconfig"]));
            StarrySkyCompassTaskRewardConfigList = JsonConvert.DeserializeObject<List<StarrySkyCompassTaskRewardConfig>>(JsonConvert.SerializeObject(table["starryskycompasstaskrewardconfig"]));
            StarrySkyCompassShopConfigList = JsonConvert.DeserializeObject<List<StarrySkyCompassShopConfig>>(JsonConvert.SerializeObject(table["starryskycompassshopconfig"]));
            
        }
    }
}