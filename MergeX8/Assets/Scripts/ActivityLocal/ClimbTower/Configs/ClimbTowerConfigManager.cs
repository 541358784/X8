
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.ClimbTower
{
    public partial class ClimbTowerConfigManager : TableSingleton<ClimbTowerConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableClimbTowerSetting> TableClimbTowerSettingList;
        public List<TableClimbTowerReward> TableClimbTowerRewardList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableClimbTowerSetting)] = "TableClimbTowerSetting",
            [typeof(TableClimbTowerReward)] = "TableClimbTowerReward",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("climbtowersetting")) return false;
            if (!table.ContainsKey("climbtowerreward")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableClimbTowerSetting": cfg = TableClimbTowerSettingList as List<T>; break;
                case "TableClimbTowerReward": cfg = TableClimbTowerRewardList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/climbtower/climbtowerconfig", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/climbtower/climbtowerconfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableClimbTowerSettingList = JsonConvert.DeserializeObject<List<TableClimbTowerSetting>>(JsonConvert.SerializeObject(table["climbtowersetting"]));
            TableClimbTowerRewardList = JsonConvert.DeserializeObject<List<TableClimbTowerReward>>(JsonConvert.SerializeObject(table["climbtowerreward"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}