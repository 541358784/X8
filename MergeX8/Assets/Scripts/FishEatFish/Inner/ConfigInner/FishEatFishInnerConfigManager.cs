/************************************************
 * FishEatFishInner Config Manager class : FishEatFishInnerConfigManager
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

namespace DragonPlus.Config.FishEatFishInner
{
    public partial class FishEatFishInnerConfigManager : Manager<FishEatFishInnerConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<FishEatFishInnerLevel> FishEatFishInnerLevelList;
        public List<FishEatFishInnerBoss> FishEatFishInnerBossList;
        public List<FishEatFishInnerEnemy> FishEatFishInnerEnemyList;
        public List<FishEatFishInnerPlayerSize> FishEatFishInnerPlayerSizeList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(FishEatFishInnerLevel)] = "FishEatFishInnerLevel",
            [typeof(FishEatFishInnerBoss)] = "FishEatFishInnerBoss",
            [typeof(FishEatFishInnerEnemy)] = "FishEatFishInnerEnemy",
            [typeof(FishEatFishInnerPlayerSize)] = "FishEatFishInnerPlayerSize",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("fisheatfishinnerlevel")) return false;
            if (!table.ContainsKey("fisheatfishinnerboss")) return false;
            if (!table.ContainsKey("fisheatfishinnerenemy")) return false;
            if (!table.ContainsKey("fisheatfishinnerplayersize")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "FishEatFishInnerLevel": cfg = FishEatFishInnerLevelList as List<T>; break;
                case "FishEatFishInnerBoss": cfg = FishEatFishInnerBossList as List<T>; break;
                case "FishEatFishInnerEnemy": cfg = FishEatFishInnerEnemyList as List<T>; break;
                case "FishEatFishInnerPlayerSize": cfg = FishEatFishInnerPlayerSizeList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("FishEatFish/Configs/fish_eat_fish_inner");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load FishEatFish/Configs/fish_eat_fish_inner error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            FishEatFishInnerLevelList = JsonConvert.DeserializeObject<List<FishEatFishInnerLevel>>(JsonConvert.SerializeObject(table["fisheatfishinnerlevel"]));
            FishEatFishInnerBossList = JsonConvert.DeserializeObject<List<FishEatFishInnerBoss>>(JsonConvert.SerializeObject(table["fisheatfishinnerboss"]));
            FishEatFishInnerEnemyList = JsonConvert.DeserializeObject<List<FishEatFishInnerEnemy>>(JsonConvert.SerializeObject(table["fisheatfishinnerenemy"]));
            FishEatFishInnerPlayerSizeList = JsonConvert.DeserializeObject<List<FishEatFishInnerPlayerSize>>(JsonConvert.SerializeObject(table["fisheatfishinnerplayersize"]));
            
        }
    }
}