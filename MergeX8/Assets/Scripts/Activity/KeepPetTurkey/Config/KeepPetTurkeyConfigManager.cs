/************************************************
 * KeepPetTurkey Config Manager class : KeepPetTurkeyConfigManager
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

namespace DragonPlus.Config.KeepPetTurkey
{
    public partial class KeepPetTurkeyConfigManager : Manager<KeepPetTurkeyConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<KeepPetTurkeyStoreItemConfig> KeepPetTurkeyStoreItemConfigList;
        public List<KeepPetTurkeyStoreLevelConfig> KeepPetTurkeyStoreLevelConfigList;
        public List<KeepPetTurkeyTaskScoreConfig> KeepPetTurkeyTaskScoreConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(KeepPetTurkeyStoreItemConfig)] = "KeepPetTurkeyStoreItemConfig",
            [typeof(KeepPetTurkeyStoreLevelConfig)] = "KeepPetTurkeyStoreLevelConfig",
            [typeof(KeepPetTurkeyTaskScoreConfig)] = "KeepPetTurkeyTaskScoreConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("keeppetturkeystoreitemconfig")) return false;
            if (!table.ContainsKey("keeppetturkeystorelevelconfig")) return false;
            if (!table.ContainsKey("keeppetturkeytaskscoreconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "KeepPetTurkeyStoreItemConfig": cfg = KeepPetTurkeyStoreItemConfigList as List<T>; break;
                case "KeepPetTurkeyStoreLevelConfig": cfg = KeepPetTurkeyStoreLevelConfigList as List<T>; break;
                case "KeepPetTurkeyTaskScoreConfig": cfg = KeepPetTurkeyTaskScoreConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/KeepPetTurkey/keeppetturkey");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/KeepPetTurkey/keeppetturkey error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            KeepPetTurkeyStoreItemConfigList = JsonConvert.DeserializeObject<List<KeepPetTurkeyStoreItemConfig>>(JsonConvert.SerializeObject(table["keeppetturkeystoreitemconfig"]));
            KeepPetTurkeyStoreLevelConfigList = JsonConvert.DeserializeObject<List<KeepPetTurkeyStoreLevelConfig>>(JsonConvert.SerializeObject(table["keeppetturkeystorelevelconfig"]));
            KeepPetTurkeyTaskScoreConfigList = JsonConvert.DeserializeObject<List<KeepPetTurkeyTaskScoreConfig>>(JsonConvert.SerializeObject(table["keeppetturkeytaskscoreconfig"]));
            
        }
    }
}