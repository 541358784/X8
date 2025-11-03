/************************************************
 * NewDailyPackageExtraReward Config Manager class : NewDailyPackageExtraRewardConfigManager
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

namespace DragonPlus.Config.NewDailyPackageExtraReward
{
    public partial class NewDailyPackageExtraRewardConfigManager : Manager<NewDailyPackageExtraRewardConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<NewDailyPackageExtraRewardGlobalConfig> NewDailyPackageExtraRewardGlobalConfigList;
        public List<NewDailyPackageExtraRewardConfig> NewDailyPackageExtraRewardConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(NewDailyPackageExtraRewardGlobalConfig)] = "NewDailyPackageExtraRewardGlobalConfig",
            [typeof(NewDailyPackageExtraRewardConfig)] = "NewDailyPackageExtraRewardConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("newdailypackageextrarewardglobalconfig")) return false;
            if (!table.ContainsKey("newdailypackageextrarewardconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "NewDailyPackageExtraRewardGlobalConfig": cfg = NewDailyPackageExtraRewardGlobalConfigList as List<T>; break;
                case "NewDailyPackageExtraRewardConfig": cfg = NewDailyPackageExtraRewardConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/NewDailyPackageExtraReward/newdailypackageextrareward");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/NewDailyPackageExtraReward/newdailypackageextrareward error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            NewDailyPackageExtraRewardGlobalConfigList = JsonConvert.DeserializeObject<List<NewDailyPackageExtraRewardGlobalConfig>>(JsonConvert.SerializeObject(table["newdailypackageextrarewardglobalconfig"]));
            NewDailyPackageExtraRewardConfigList = JsonConvert.DeserializeObject<List<NewDailyPackageExtraRewardConfig>>(JsonConvert.SerializeObject(table["newdailypackageextrarewardconfig"]));
            
        }
    }
}