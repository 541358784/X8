
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.RabbitRacing
{
    public partial class RabbitRacingConfigManager : TableSingleton<RabbitRacingConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableRabbitRacingSetting> TableRabbitRacingSettingList;
        public List<TableRabbitRacingReward> TableRabbitRacingRewardList;
        public List<TableRabbitRacingItem> TableRabbitRacingItemList;
        public List<TableRabbitRacingRobot> TableRabbitRacingRobotList;
        public List<TableRabbitRacingScore> TableRabbitRacingScoreList;
        public List<TableRabbitRacingRobotName> TableRabbitRacingRobotNameList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableRabbitRacingSetting)] = "TableRabbitRacingSetting",
            [typeof(TableRabbitRacingReward)] = "TableRabbitRacingReward",
            [typeof(TableRabbitRacingItem)] = "TableRabbitRacingItem",
            [typeof(TableRabbitRacingRobot)] = "TableRabbitRacingRobot",
            [typeof(TableRabbitRacingScore)] = "TableRabbitRacingScore",
            [typeof(TableRabbitRacingRobotName)] = "TableRabbitRacingRobotName",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("rabbitracingsetting")) return false;
            if (!table.ContainsKey("rabbitracingreward")) return false;
            if (!table.ContainsKey("rabbitracingitem")) return false;
            if (!table.ContainsKey("rabbitracingrobot")) return false;
            if (!table.ContainsKey("rabbitracingscore")) return false;
            if (!table.ContainsKey("rabbitracingrobotname")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableRabbitRacingSetting": cfg = TableRabbitRacingSettingList as List<T>; break;
                case "TableRabbitRacingReward": cfg = TableRabbitRacingRewardList as List<T>; break;
                case "TableRabbitRacingItem": cfg = TableRabbitRacingItemList as List<T>; break;
                case "TableRabbitRacingRobot": cfg = TableRabbitRacingRobotList as List<T>; break;
                case "TableRabbitRacingScore": cfg = TableRabbitRacingScoreList as List<T>; break;
                case "TableRabbitRacingRobotName": cfg = TableRabbitRacingRobotNameList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/RabbitRacing/RabbitRacingConfig", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/RabbitRacing/RabbitRacingConfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableRabbitRacingSettingList = JsonConvert.DeserializeObject<List<TableRabbitRacingSetting>>(JsonConvert.SerializeObject(table["rabbitracingsetting"]));
            TableRabbitRacingRewardList = JsonConvert.DeserializeObject<List<TableRabbitRacingReward>>(JsonConvert.SerializeObject(table["rabbitracingreward"]));
            TableRabbitRacingItemList = JsonConvert.DeserializeObject<List<TableRabbitRacingItem>>(JsonConvert.SerializeObject(table["rabbitracingitem"]));
            TableRabbitRacingRobotList = JsonConvert.DeserializeObject<List<TableRabbitRacingRobot>>(JsonConvert.SerializeObject(table["rabbitracingrobot"]));
            TableRabbitRacingScoreList = JsonConvert.DeserializeObject<List<TableRabbitRacingScore>>(JsonConvert.SerializeObject(table["rabbitracingscore"]));
            TableRabbitRacingRobotNameList = JsonConvert.DeserializeObject<List<TableRabbitRacingRobotName>>(JsonConvert.SerializeObject(table["rabbitracingrobotname"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}