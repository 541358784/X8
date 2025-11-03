
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.BalloonRacing
{
    public partial class BalloonRacingConfigManager : TableSingleton<BalloonRacingConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableBalloonRacingSetting> TableBalloonRacingSettingList;
        public List<TableBalloonRacingReward> TableBalloonRacingRewardList;
        public List<TableBalloonRacingItem> TableBalloonRacingItemList;
        public List<TableBalloonRacingRobot> TableBalloonRacingRobotList;
        public List<TableBalloonRacingScore> TableBalloonRacingScoreList;
        public List<TableBalloonRacingRobotName> TableBalloonRacingRobotNameList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableBalloonRacingSetting)] = "TableBalloonRacingSetting",
            [typeof(TableBalloonRacingReward)] = "TableBalloonRacingReward",
            [typeof(TableBalloonRacingItem)] = "TableBalloonRacingItem",
            [typeof(TableBalloonRacingRobot)] = "TableBalloonRacingRobot",
            [typeof(TableBalloonRacingScore)] = "TableBalloonRacingScore",
            [typeof(TableBalloonRacingRobotName)] = "TableBalloonRacingRobotName",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("balloonracingsetting")) return false;
            if (!table.ContainsKey("balloonracingreward")) return false;
            if (!table.ContainsKey("balloonracingitem")) return false;
            if (!table.ContainsKey("balloonracingrobot")) return false;
            if (!table.ContainsKey("balloonracingscore")) return false;
            if (!table.ContainsKey("balloonracingrobotname")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableBalloonRacingSetting": cfg = TableBalloonRacingSettingList as List<T>; break;
                case "TableBalloonRacingReward": cfg = TableBalloonRacingRewardList as List<T>; break;
                case "TableBalloonRacingItem": cfg = TableBalloonRacingItemList as List<T>; break;
                case "TableBalloonRacingRobot": cfg = TableBalloonRacingRobotList as List<T>; break;
                case "TableBalloonRacingScore": cfg = TableBalloonRacingScoreList as List<T>; break;
                case "TableBalloonRacingRobotName": cfg = TableBalloonRacingRobotNameList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/BalloonRacing/BalloonRacingConfig", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/BalloonRacing/BalloonRacingConfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableBalloonRacingSettingList = JsonConvert.DeserializeObject<List<TableBalloonRacingSetting>>(JsonConvert.SerializeObject(table["balloonracingsetting"]));
            TableBalloonRacingRewardList = JsonConvert.DeserializeObject<List<TableBalloonRacingReward>>(JsonConvert.SerializeObject(table["balloonracingreward"]));
            TableBalloonRacingItemList = JsonConvert.DeserializeObject<List<TableBalloonRacingItem>>(JsonConvert.SerializeObject(table["balloonracingitem"]));
            TableBalloonRacingRobotList = JsonConvert.DeserializeObject<List<TableBalloonRacingRobot>>(JsonConvert.SerializeObject(table["balloonracingrobot"]));
            TableBalloonRacingScoreList = JsonConvert.DeserializeObject<List<TableBalloonRacingScore>>(JsonConvert.SerializeObject(table["balloonracingscore"]));
            TableBalloonRacingRobotNameList = JsonConvert.DeserializeObject<List<TableBalloonRacingRobotName>>(JsonConvert.SerializeObject(table["balloonracingrobotname"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}