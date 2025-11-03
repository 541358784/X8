/************************************************
 * FishEatFish Config Manager class : FishEatFishConfigManager
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

namespace DragonPlus.Config.FishEatFish
{
    public partial class FishEatFishConfigManager : Manager<FishEatFishConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<FishEatFishLevel> FishEatFishLevelList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(FishEatFishLevel)] = "FishEatFishLevel",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("fisheatfishlevel")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "FishEatFishLevel": cfg = FishEatFishLevelList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/FishEatFish/fish_eat_fish");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/FishEatFish/fish_eat_fish error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            FishEatFishLevelList = JsonConvert.DeserializeObject<List<FishEatFishLevel>>(JsonConvert.SerializeObject(table["fisheatfishlevel"]));
            
        }
    }
}