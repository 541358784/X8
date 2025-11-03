/************************************************
 * SeaRacing Config Manager class : SeaRacingConfigManager
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

namespace DragonPlus.Config.SeaRacing
{
    public partial class SeaRacingConfigManager : Manager<SeaRacingConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<SeaRacingPreheatConfig> SeaRacingPreheatConfigList;
        public List<SeaRacingRoundConfig> SeaRacingRoundConfigList;
        public List<SeaRacingRewardConfig> SeaRacingRewardConfigList;
        public List<SeaRacingRobotRandomConfig> SeaRacingRobotRandomConfigList;
        public List<SeaRacingRobotConfig> SeaRacingRobotConfigList;
        public List<SeaRacingRobotNameConfig> SeaRacingRobotNameConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(SeaRacingPreheatConfig)] = "SeaRacingPreheatConfig",
            [typeof(SeaRacingRoundConfig)] = "SeaRacingRoundConfig",
            [typeof(SeaRacingRewardConfig)] = "SeaRacingRewardConfig",
            [typeof(SeaRacingRobotRandomConfig)] = "SeaRacingRobotRandomConfig",
            [typeof(SeaRacingRobotConfig)] = "SeaRacingRobotConfig",
            [typeof(SeaRacingRobotNameConfig)] = "SeaRacingRobotNameConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("searacingpreheatconfig")) return false;
            if (!table.ContainsKey("searacingroundconfig")) return false;
            if (!table.ContainsKey("searacingrewardconfig")) return false;
            if (!table.ContainsKey("searacingrobotrandomconfig")) return false;
            if (!table.ContainsKey("searacingrobotconfig")) return false;
            if (!table.ContainsKey("searacingrobotnameconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "SeaRacingPreheatConfig": cfg = SeaRacingPreheatConfigList as List<T>; break;
                case "SeaRacingRoundConfig": cfg = SeaRacingRoundConfigList as List<T>; break;
                case "SeaRacingRewardConfig": cfg = SeaRacingRewardConfigList as List<T>; break;
                case "SeaRacingRobotRandomConfig": cfg = SeaRacingRobotRandomConfigList as List<T>; break;
                case "SeaRacingRobotConfig": cfg = SeaRacingRobotConfigList as List<T>; break;
                case "SeaRacingRobotNameConfig": cfg = SeaRacingRobotNameConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/SeaRacing/searacing");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/SeaRacing/searacing error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            SeaRacingPreheatConfigList = JsonConvert.DeserializeObject<List<SeaRacingPreheatConfig>>(JsonConvert.SerializeObject(table["searacingpreheatconfig"]));
            SeaRacingRoundConfigList = JsonConvert.DeserializeObject<List<SeaRacingRoundConfig>>(JsonConvert.SerializeObject(table["searacingroundconfig"]));
            SeaRacingRewardConfigList = JsonConvert.DeserializeObject<List<SeaRacingRewardConfig>>(JsonConvert.SerializeObject(table["searacingrewardconfig"]));
            SeaRacingRobotRandomConfigList = JsonConvert.DeserializeObject<List<SeaRacingRobotRandomConfig>>(JsonConvert.SerializeObject(table["searacingrobotrandomconfig"]));
            SeaRacingRobotConfigList = JsonConvert.DeserializeObject<List<SeaRacingRobotConfig>>(JsonConvert.SerializeObject(table["searacingrobotconfig"]));
            SeaRacingRobotNameConfigList = JsonConvert.DeserializeObject<List<SeaRacingRobotNameConfig>>(JsonConvert.SerializeObject(table["searacingrobotnameconfig"]));
            
        }
    }
}