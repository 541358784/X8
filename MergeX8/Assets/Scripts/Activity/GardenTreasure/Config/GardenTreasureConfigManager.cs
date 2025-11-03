/************************************************
 * GardenTreasure Config Manager class : GardenTreasureConfigManager
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

namespace DragonPlus.Config.GardenTreasure
{
    public partial class GardenTreasureConfigManager : Manager<GardenTreasureConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<GardenTreasureSetingConfig> GardenTreasureSetingConfigList;
        public List<GardenTreasureLevelConfig> GardenTreasureLevelConfigList;
        public List<GardenTreasureBoardConfig> GardenTreasureBoardConfigList;
        public List<GardenTreasureShapeConfig> GardenTreasureShapeConfigList;
        public List<GardenTreasureLeaderBoardRewardConfig> GardenTreasureLeaderBoardRewardConfigList;
        public List<GardenTreasurePackageConfig> GardenTreasurePackageConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(GardenTreasureSetingConfig)] = "GardenTreasureSetingConfig",
            [typeof(GardenTreasureLevelConfig)] = "GardenTreasureLevelConfig",
            [typeof(GardenTreasureBoardConfig)] = "GardenTreasureBoardConfig",
            [typeof(GardenTreasureShapeConfig)] = "GardenTreasureShapeConfig",
            [typeof(GardenTreasureLeaderBoardRewardConfig)] = "GardenTreasureLeaderBoardRewardConfig",
            [typeof(GardenTreasurePackageConfig)] = "GardenTreasurePackageConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("gardentreasuresetingconfig")) return false;
            if (!table.ContainsKey("gardentreasurelevelconfig")) return false;
            if (!table.ContainsKey("gardentreasureboardconfig")) return false;
            if (!table.ContainsKey("gardentreasureshapeconfig")) return false;
            if (!table.ContainsKey("gardentreasureleaderboardrewardconfig")) return false;
            if (!table.ContainsKey("gardentreasurepackageconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "GardenTreasureSetingConfig": cfg = GardenTreasureSetingConfigList as List<T>; break;
                case "GardenTreasureLevelConfig": cfg = GardenTreasureLevelConfigList as List<T>; break;
                case "GardenTreasureBoardConfig": cfg = GardenTreasureBoardConfigList as List<T>; break;
                case "GardenTreasureShapeConfig": cfg = GardenTreasureShapeConfigList as List<T>; break;
                case "GardenTreasureLeaderBoardRewardConfig": cfg = GardenTreasureLeaderBoardRewardConfigList as List<T>; break;
                case "GardenTreasurePackageConfig": cfg = GardenTreasurePackageConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/GardenTreasure/config");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/GardenTreasure/config error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            GardenTreasureSetingConfigList = JsonConvert.DeserializeObject<List<GardenTreasureSetingConfig>>(JsonConvert.SerializeObject(table["gardentreasuresetingconfig"]));
            GardenTreasureLevelConfigList = JsonConvert.DeserializeObject<List<GardenTreasureLevelConfig>>(JsonConvert.SerializeObject(table["gardentreasurelevelconfig"]));
            GardenTreasureBoardConfigList = JsonConvert.DeserializeObject<List<GardenTreasureBoardConfig>>(JsonConvert.SerializeObject(table["gardentreasureboardconfig"]));
            GardenTreasureShapeConfigList = JsonConvert.DeserializeObject<List<GardenTreasureShapeConfig>>(JsonConvert.SerializeObject(table["gardentreasureshapeconfig"]));
            GardenTreasureLeaderBoardRewardConfigList = JsonConvert.DeserializeObject<List<GardenTreasureLeaderBoardRewardConfig>>(JsonConvert.SerializeObject(table["gardentreasureleaderboardrewardconfig"]));
            GardenTreasurePackageConfigList = JsonConvert.DeserializeObject<List<GardenTreasurePackageConfig>>(JsonConvert.SerializeObject(table["gardentreasurepackageconfig"]));
            
        }
    }
}