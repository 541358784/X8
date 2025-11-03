/************************************************
 * DogPlayExtraReward Config Manager class : DogPlayExtraRewardConfigManager
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

namespace DragonPlus.Config.DogPlayExtraReward
{
    public partial class DogPlayExtraRewardConfigManager : Manager<DogPlayExtraRewardConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<DogPlayExtraRewardRewardConfig> DogPlayExtraRewardRewardConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(DogPlayExtraRewardRewardConfig)] = "DogPlayExtraRewardRewardConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("dogplayextrarewardrewardconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "DogPlayExtraRewardRewardConfig": cfg = DogPlayExtraRewardRewardConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/DogPlayExtraReward/dogplayextrareward");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/DogPlayExtraReward/dogplayextrareward error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            DogPlayExtraRewardRewardConfigList = JsonConvert.DeserializeObject<List<DogPlayExtraRewardRewardConfig>>(JsonConvert.SerializeObject(table["dogplayextrarewardrewardconfig"]));
            
        }
    }
}