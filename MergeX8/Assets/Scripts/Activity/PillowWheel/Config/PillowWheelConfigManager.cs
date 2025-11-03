/************************************************
 * PillowWheel Config Manager class : PillowWheelConfigManager
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

namespace DragonPlus.Config.PillowWheel
{
    public partial class PillowWheelConfigManager : Manager<PillowWheelConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<PillowWheelGlobalConfig> PillowWheelGlobalConfigList;
        public List<PillowWheelTurntableConfig> PillowWheelTurntableConfigList;
        public List<PillowWheelResultConfig> PillowWheelResultConfigList;
        public List<PillowWheelSpecialRewardConfig> PillowWheelSpecialRewardConfigList;
        public List<PillowWheelTaskRewardConfig> PillowWheelTaskRewardConfigList;
        public List<PillowWheelShopConfig> PillowWheelShopConfigList;
        public List<PillowWheelLeaderBoardRewardConfig> PillowWheelLeaderBoardRewardConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(PillowWheelGlobalConfig)] = "PillowWheelGlobalConfig",
            [typeof(PillowWheelTurntableConfig)] = "PillowWheelTurntableConfig",
            [typeof(PillowWheelResultConfig)] = "PillowWheelResultConfig",
            [typeof(PillowWheelSpecialRewardConfig)] = "PillowWheelSpecialRewardConfig",
            [typeof(PillowWheelTaskRewardConfig)] = "PillowWheelTaskRewardConfig",
            [typeof(PillowWheelShopConfig)] = "PillowWheelShopConfig",
            [typeof(PillowWheelLeaderBoardRewardConfig)] = "PillowWheelLeaderBoardRewardConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("pillowwheelglobalconfig")) return false;
            if (!table.ContainsKey("pillowwheelturntableconfig")) return false;
            if (!table.ContainsKey("pillowwheelresultconfig")) return false;
            if (!table.ContainsKey("pillowwheelspecialrewardconfig")) return false;
            if (!table.ContainsKey("pillowwheeltaskrewardconfig")) return false;
            if (!table.ContainsKey("pillowwheelshopconfig")) return false;
            if (!table.ContainsKey("pillowwheelleaderboardrewardconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "PillowWheelGlobalConfig": cfg = PillowWheelGlobalConfigList as List<T>; break;
                case "PillowWheelTurntableConfig": cfg = PillowWheelTurntableConfigList as List<T>; break;
                case "PillowWheelResultConfig": cfg = PillowWheelResultConfigList as List<T>; break;
                case "PillowWheelSpecialRewardConfig": cfg = PillowWheelSpecialRewardConfigList as List<T>; break;
                case "PillowWheelTaskRewardConfig": cfg = PillowWheelTaskRewardConfigList as List<T>; break;
                case "PillowWheelShopConfig": cfg = PillowWheelShopConfigList as List<T>; break;
                case "PillowWheelLeaderBoardRewardConfig": cfg = PillowWheelLeaderBoardRewardConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/PillowWheel/pillowwheel");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/PillowWheel/pillowwheel error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            PillowWheelGlobalConfigList = JsonConvert.DeserializeObject<List<PillowWheelGlobalConfig>>(JsonConvert.SerializeObject(table["pillowwheelglobalconfig"]));
            PillowWheelTurntableConfigList = JsonConvert.DeserializeObject<List<PillowWheelTurntableConfig>>(JsonConvert.SerializeObject(table["pillowwheelturntableconfig"]));
            PillowWheelResultConfigList = JsonConvert.DeserializeObject<List<PillowWheelResultConfig>>(JsonConvert.SerializeObject(table["pillowwheelresultconfig"]));
            PillowWheelSpecialRewardConfigList = JsonConvert.DeserializeObject<List<PillowWheelSpecialRewardConfig>>(JsonConvert.SerializeObject(table["pillowwheelspecialrewardconfig"]));
            PillowWheelTaskRewardConfigList = JsonConvert.DeserializeObject<List<PillowWheelTaskRewardConfig>>(JsonConvert.SerializeObject(table["pillowwheeltaskrewardconfig"]));
            PillowWheelShopConfigList = JsonConvert.DeserializeObject<List<PillowWheelShopConfig>>(JsonConvert.SerializeObject(table["pillowwheelshopconfig"]));
            PillowWheelLeaderBoardRewardConfigList = JsonConvert.DeserializeObject<List<PillowWheelLeaderBoardRewardConfig>>(JsonConvert.SerializeObject(table["pillowwheelleaderboardrewardconfig"]));
            
        }
    }
}