/************************************************
 * FishEatFishInnerTwo Config Manager class : FishEatFishInnerTwoConfigManager
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

namespace DragonPlus.Config.FishEatFishInnerTwo
{
    public partial class FishEatFishInnerTwoConfigManager : Manager<FishEatFishInnerTwoConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<FishEatFishInnerTwoLevel> FishEatFishInnerTwoLevelList;
        public List<FishEatFishInnerTwoEnemy> FishEatFishInnerTwoEnemyList;
        public List<FishEatFishInnerTwoPlayerSize> FishEatFishInnerTwoPlayerSizeList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(FishEatFishInnerTwoLevel)] = "FishEatFishInnerTwoLevel",
            [typeof(FishEatFishInnerTwoEnemy)] = "FishEatFishInnerTwoEnemy",
            [typeof(FishEatFishInnerTwoPlayerSize)] = "FishEatFishInnerTwoPlayerSize",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("fisheatfishinnertwolevel")) return false;
            if (!table.ContainsKey("fisheatfishinnertwoenemy")) return false;
            if (!table.ContainsKey("fisheatfishinnertwoplayersize")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "FishEatFishInnerTwoLevel": cfg = FishEatFishInnerTwoLevelList as List<T>; break;
                case "FishEatFishInnerTwoEnemy": cfg = FishEatFishInnerTwoEnemyList as List<T>; break;
                case "FishEatFishInnerTwoPlayerSize": cfg = FishEatFishInnerTwoPlayerSizeList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("FishEatFish/TwoConfigs/fish_eat_fish_inner_two");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load FishEatFish/TwoConfigs/fish_eat_fish_inner_two error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            FishEatFishInnerTwoLevelList = JsonConvert.DeserializeObject<List<FishEatFishInnerTwoLevel>>(JsonConvert.SerializeObject(table["fisheatfishinnertwolevel"]));
            FishEatFishInnerTwoEnemyList = JsonConvert.DeserializeObject<List<FishEatFishInnerTwoEnemy>>(JsonConvert.SerializeObject(table["fisheatfishinnertwoenemy"]));
            FishEatFishInnerTwoPlayerSizeList = JsonConvert.DeserializeObject<List<FishEatFishInnerTwoPlayerSize>>(JsonConvert.SerializeObject(table["fisheatfishinnertwoplayersize"]));
            
        }
    }
}