/************************************************
 * RecoverCoin Config Manager class : RecoverCoinConfigManager
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

namespace DragonPlus.Config.RecoverCoin
{
    public partial class RecoverCoinConfigManager : Manager<RecoverCoinConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<RecoverCoinRewardConfig> RecoverCoinRewardConfigList;
        public List<RecoverCoinTimeConfig> RecoverCoinTimeConfigList;
        public List<RecoverCoinExchangeStarConfig> RecoverCoinExchangeStarConfigList;
        public List<RecoverCoinPlayerCoinCountGroupConfig> RecoverCoinPlayerCoinCountGroupConfigList;
        public List<RecoverCoinRobotCountConfig> RecoverCoinRobotCountConfigList;
        public List<RecoverCoinRobotGrowSpeedConfig> RecoverCoinRobotGrowSpeedConfigList;
        public List<RecoverCoinRobotMinStarUpdateIntervalConfig> RecoverCoinRobotMinStarUpdateIntervalConfigList;
        public List<RecoverCoinRobotNameConfig> RecoverCoinRobotNameConfigList;
        public List<RecoverCoinSkinConfig> RecoverCoinSkinConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(RecoverCoinRewardConfig)] = "RecoverCoinRewardConfig",
            [typeof(RecoverCoinTimeConfig)] = "RecoverCoinTimeConfig",
            [typeof(RecoverCoinExchangeStarConfig)] = "RecoverCoinExchangeStarConfig",
            [typeof(RecoverCoinPlayerCoinCountGroupConfig)] = "RecoverCoinPlayerCoinCountGroupConfig",
            [typeof(RecoverCoinRobotCountConfig)] = "RecoverCoinRobotCountConfig",
            [typeof(RecoverCoinRobotGrowSpeedConfig)] = "RecoverCoinRobotGrowSpeedConfig",
            [typeof(RecoverCoinRobotMinStarUpdateIntervalConfig)] = "RecoverCoinRobotMinStarUpdateIntervalConfig",
            [typeof(RecoverCoinRobotNameConfig)] = "RecoverCoinRobotNameConfig",
            [typeof(RecoverCoinSkinConfig)] = "RecoverCoinSkinConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("recovercoinrewardconfig")) return false;
            if (!table.ContainsKey("recovercointimeconfig")) return false;
            if (!table.ContainsKey("recovercoinexchangestarconfig")) return false;
            if (!table.ContainsKey("recovercoinplayercoincountgroupconfig")) return false;
            if (!table.ContainsKey("recovercoinrobotcountconfig")) return false;
            if (!table.ContainsKey("recovercoinrobotgrowspeedconfig")) return false;
            if (!table.ContainsKey("recovercoinrobotminstarupdateintervalconfig")) return false;
            if (!table.ContainsKey("recovercoinrobotnameconfig")) return false;
            if (!table.ContainsKey("recovercoinskinconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "RecoverCoinRewardConfig": cfg = RecoverCoinRewardConfigList as List<T>; break;
                case "RecoverCoinTimeConfig": cfg = RecoverCoinTimeConfigList as List<T>; break;
                case "RecoverCoinExchangeStarConfig": cfg = RecoverCoinExchangeStarConfigList as List<T>; break;
                case "RecoverCoinPlayerCoinCountGroupConfig": cfg = RecoverCoinPlayerCoinCountGroupConfigList as List<T>; break;
                case "RecoverCoinRobotCountConfig": cfg = RecoverCoinRobotCountConfigList as List<T>; break;
                case "RecoverCoinRobotGrowSpeedConfig": cfg = RecoverCoinRobotGrowSpeedConfigList as List<T>; break;
                case "RecoverCoinRobotMinStarUpdateIntervalConfig": cfg = RecoverCoinRobotMinStarUpdateIntervalConfigList as List<T>; break;
                case "RecoverCoinRobotNameConfig": cfg = RecoverCoinRobotNameConfigList as List<T>; break;
                case "RecoverCoinSkinConfig": cfg = RecoverCoinSkinConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/RecoverCoin/recovercoin");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/RecoverCoin/recovercoin error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            RecoverCoinRewardConfigList = JsonConvert.DeserializeObject<List<RecoverCoinRewardConfig>>(JsonConvert.SerializeObject(table["recovercoinrewardconfig"]));
            RecoverCoinTimeConfigList = JsonConvert.DeserializeObject<List<RecoverCoinTimeConfig>>(JsonConvert.SerializeObject(table["recovercointimeconfig"]));
            RecoverCoinExchangeStarConfigList = JsonConvert.DeserializeObject<List<RecoverCoinExchangeStarConfig>>(JsonConvert.SerializeObject(table["recovercoinexchangestarconfig"]));
            RecoverCoinPlayerCoinCountGroupConfigList = JsonConvert.DeserializeObject<List<RecoverCoinPlayerCoinCountGroupConfig>>(JsonConvert.SerializeObject(table["recovercoinplayercoincountgroupconfig"]));
            RecoverCoinRobotCountConfigList = JsonConvert.DeserializeObject<List<RecoverCoinRobotCountConfig>>(JsonConvert.SerializeObject(table["recovercoinrobotcountconfig"]));
            RecoverCoinRobotGrowSpeedConfigList = JsonConvert.DeserializeObject<List<RecoverCoinRobotGrowSpeedConfig>>(JsonConvert.SerializeObject(table["recovercoinrobotgrowspeedconfig"]));
            RecoverCoinRobotMinStarUpdateIntervalConfigList = JsonConvert.DeserializeObject<List<RecoverCoinRobotMinStarUpdateIntervalConfig>>(JsonConvert.SerializeObject(table["recovercoinrobotminstarupdateintervalconfig"]));
            RecoverCoinRobotNameConfigList = JsonConvert.DeserializeObject<List<RecoverCoinRobotNameConfig>>(JsonConvert.SerializeObject(table["recovercoinrobotnameconfig"]));
            RecoverCoinSkinConfigList = JsonConvert.DeserializeObject<List<RecoverCoinSkinConfig>>(JsonConvert.SerializeObject(table["recovercoinskinconfig"]));
            
        }
    }
}