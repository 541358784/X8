/************************************************
 * GarageCleanup Config Manager class : GarageCleanupConfigManager
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

namespace DragonPlus.Config.GarageCleanup
{
    public partial class GarageCleanupConfigManager : Manager<GarageCleanupConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<GarageCleanupLevelGroupConfig> GarageCleanupLevelGroupConfigList;
        public List<GarageCleanupConfig> GarageCleanupConfigList;
        public List<GarageCleanupBoard> GarageCleanupBoardList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(GarageCleanupLevelGroupConfig)] = "GarageCleanupLevelGroupConfig",
            [typeof(GarageCleanupConfig)] = "GarageCleanupConfig",
            [typeof(GarageCleanupBoard)] = "GarageCleanupBoard",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("garagecleanuplevelgroupconfig")) return false;
            if (!table.ContainsKey("garagecleanupconfig")) return false;
            if (!table.ContainsKey("garagecleanupboard")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "GarageCleanupLevelGroupConfig": cfg = GarageCleanupLevelGroupConfigList as List<T>; break;
                case "GarageCleanupConfig": cfg = GarageCleanupConfigList as List<T>; break;
                case "GarageCleanupBoard": cfg = GarageCleanupBoardList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/GarageCleanup/garagecleanup");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/GarageCleanup/garagecleanup error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            GarageCleanupLevelGroupConfigList = JsonConvert.DeserializeObject<List<GarageCleanupLevelGroupConfig>>(JsonConvert.SerializeObject(table["garagecleanuplevelgroupconfig"]));
            GarageCleanupConfigList = JsonConvert.DeserializeObject<List<GarageCleanupConfig>>(JsonConvert.SerializeObject(table["garagecleanupconfig"]));
            GarageCleanupBoardList = JsonConvert.DeserializeObject<List<GarageCleanupBoard>>(JsonConvert.SerializeObject(table["garagecleanupboard"]));
            
        }
    }
}