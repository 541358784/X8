/************************************************
 * ThreeGift Config Manager class : ThreeGiftConfigManager
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

namespace DragonPlus.Config.MultipleGift
{
    public class MultipleGiftConfigManager : Manager<MultipleGiftConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<ThreeGiftLevelConfig> ThreeGiftLevelConfigList;
        public List<ThreeGiftConfig> ThreeGiftConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(ThreeGiftLevelConfig)] = "ThreeGiftLevelConfig",
            [typeof(ThreeGiftConfig)] = "ThreeGiftConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("threegiftlevelconfig")) return false;
            if (!table.ContainsKey("threegiftconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "ThreeGiftLevelConfig": cfg = ThreeGiftLevelConfigList as List<T>; break;
                case "ThreeGiftConfig": cfg = ThreeGiftConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("/threegift");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load /threegift error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            ThreeGiftLevelConfigList = JsonConvert.DeserializeObject<List<ThreeGiftLevelConfig>>(JsonConvert.SerializeObject(table["threegiftlevelconfig"]));
            ThreeGiftConfigList = JsonConvert.DeserializeObject<List<ThreeGiftConfig>>(JsonConvert.SerializeObject(table["threegiftconfig"]));
            
        }
    }
}