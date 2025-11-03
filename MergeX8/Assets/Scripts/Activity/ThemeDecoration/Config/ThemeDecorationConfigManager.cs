/************************************************
 * ThemeDecoration Config Manager class : ThemeDecorationConfigManager
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

namespace DragonPlus.Config.ThemeDecoration
{
    public partial class ThemeDecorationConfigManager : Manager<ThemeDecorationConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<ThemeDecorationGlobalConfig> ThemeDecorationGlobalConfigList;
        public List<ThemeDecorationStoreLevelConfig> ThemeDecorationStoreLevelConfigList;
        public List<ThemeDecorationStoreItemConfig> ThemeDecorationStoreItemConfigList;
        public List<ThemeDecorationTaskRewardConfig> ThemeDecorationTaskRewardConfigList;
        public List<ThemeDecorationLeaderBoardRewardConfig> ThemeDecorationLeaderBoardRewardConfigList;
        public List<ThmeDecorationLeaderBoardScheduleConfig> ThmeDecorationLeaderBoardScheduleConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(ThemeDecorationGlobalConfig)] = "ThemeDecorationGlobalConfig",
            [typeof(ThemeDecorationStoreLevelConfig)] = "ThemeDecorationStoreLevelConfig",
            [typeof(ThemeDecorationStoreItemConfig)] = "ThemeDecorationStoreItemConfig",
            [typeof(ThemeDecorationTaskRewardConfig)] = "ThemeDecorationTaskRewardConfig",
            [typeof(ThemeDecorationLeaderBoardRewardConfig)] = "ThemeDecorationLeaderBoardRewardConfig",
            [typeof(ThmeDecorationLeaderBoardScheduleConfig)] = "ThmeDecorationLeaderBoardScheduleConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("themedecorationglobalconfig")) return false;
            if (!table.ContainsKey("themedecorationstorelevelconfig")) return false;
            if (!table.ContainsKey("themedecorationstoreitemconfig")) return false;
            if (!table.ContainsKey("themedecorationtaskrewardconfig")) return false;
            if (!table.ContainsKey("themedecorationleaderboardrewardconfig")) return false;
            if (!table.ContainsKey("thmedecorationleaderboardscheduleconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "ThemeDecorationGlobalConfig": cfg = ThemeDecorationGlobalConfigList as List<T>; break;
                case "ThemeDecorationStoreLevelConfig": cfg = ThemeDecorationStoreLevelConfigList as List<T>; break;
                case "ThemeDecorationStoreItemConfig": cfg = ThemeDecorationStoreItemConfigList as List<T>; break;
                case "ThemeDecorationTaskRewardConfig": cfg = ThemeDecorationTaskRewardConfigList as List<T>; break;
                case "ThemeDecorationLeaderBoardRewardConfig": cfg = ThemeDecorationLeaderBoardRewardConfigList as List<T>; break;
                case "ThmeDecorationLeaderBoardScheduleConfig": cfg = ThmeDecorationLeaderBoardScheduleConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/ThemeDecoration/themedecoration");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/ThemeDecoration/themedecoration error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            ThemeDecorationGlobalConfigList = JsonConvert.DeserializeObject<List<ThemeDecorationGlobalConfig>>(JsonConvert.SerializeObject(table["themedecorationglobalconfig"]));
            ThemeDecorationStoreLevelConfigList = JsonConvert.DeserializeObject<List<ThemeDecorationStoreLevelConfig>>(JsonConvert.SerializeObject(table["themedecorationstorelevelconfig"]));
            ThemeDecorationStoreItemConfigList = JsonConvert.DeserializeObject<List<ThemeDecorationStoreItemConfig>>(JsonConvert.SerializeObject(table["themedecorationstoreitemconfig"]));
            ThemeDecorationTaskRewardConfigList = JsonConvert.DeserializeObject<List<ThemeDecorationTaskRewardConfig>>(JsonConvert.SerializeObject(table["themedecorationtaskrewardconfig"]));
            ThemeDecorationLeaderBoardRewardConfigList = JsonConvert.DeserializeObject<List<ThemeDecorationLeaderBoardRewardConfig>>(JsonConvert.SerializeObject(table["themedecorationleaderboardrewardconfig"]));
            ThmeDecorationLeaderBoardScheduleConfigList = JsonConvert.DeserializeObject<List<ThmeDecorationLeaderBoardScheduleConfig>>(JsonConvert.SerializeObject(table["thmedecorationleaderboardscheduleconfig"]));
            
        }
    }
}