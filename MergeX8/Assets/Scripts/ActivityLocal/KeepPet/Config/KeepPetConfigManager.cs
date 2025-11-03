/************************************************
 * KeepPet Config Manager class : KeepPetConfigManager
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

namespace DragonPlus.Config.KeepPet
{
    public partial class KeepPetConfigManager : Manager<KeepPetConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<KeepPetGlobalConfig> KeepPetGlobalConfigList;
        public List<KeepPetLevelConfig> KeepPetLevelConfigList;
        public List<KeepPetBuildingItemConfig> KeepPetBuildingItemConfigList;
        public List<KeepPetBuildingHangPointConfig> KeepPetBuildingHangPointConfigList;
        public List<KeepPetSearchTaskConfig> KeepPetSearchTaskConfigList;
        public List<KeepPetSearchTaskRewardPoolConfig> KeepPetSearchTaskRewardPoolConfigList;
        public List<KeepPetDailyTaskConfig> KeepPetDailyTaskConfigList;
        public List<KeepPetOrderConfig> KeepPetOrderConfigList;
        public List<KeepPetClueConfig> KeepPetClueConfigList;
        public List<KeepPetStoreConfig> KeepPetStoreConfigList;
        public List<KeepPetThreeOneStoreConfig> KeepPetThreeOneStoreConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(KeepPetGlobalConfig)] = "KeepPetGlobalConfig",
            [typeof(KeepPetLevelConfig)] = "KeepPetLevelConfig",
            [typeof(KeepPetBuildingItemConfig)] = "KeepPetBuildingItemConfig",
            [typeof(KeepPetBuildingHangPointConfig)] = "KeepPetBuildingHangPointConfig",
            [typeof(KeepPetSearchTaskConfig)] = "KeepPetSearchTaskConfig",
            [typeof(KeepPetSearchTaskRewardPoolConfig)] = "KeepPetSearchTaskRewardPoolConfig",
            [typeof(KeepPetDailyTaskConfig)] = "KeepPetDailyTaskConfig",
            [typeof(KeepPetOrderConfig)] = "KeepPetOrderConfig",
            [typeof(KeepPetClueConfig)] = "KeepPetClueConfig",
            [typeof(KeepPetStoreConfig)] = "KeepPetStoreConfig",
            [typeof(KeepPetThreeOneStoreConfig)] = "KeepPetThreeOneStoreConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("keeppetglobalconfig")) return false;
            if (!table.ContainsKey("keeppetlevelconfig")) return false;
            if (!table.ContainsKey("keeppetbuildingitemconfig")) return false;
            if (!table.ContainsKey("keeppetbuildinghangpointconfig")) return false;
            if (!table.ContainsKey("keeppetsearchtaskconfig")) return false;
            if (!table.ContainsKey("keeppetsearchtaskrewardpoolconfig")) return false;
            if (!table.ContainsKey("keeppetdailytaskconfig")) return false;
            if (!table.ContainsKey("keeppetorderconfig")) return false;
            if (!table.ContainsKey("keeppetclueconfig")) return false;
            if (!table.ContainsKey("keeppetstoreconfig")) return false;
            if (!table.ContainsKey("keeppetthreeonestoreconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "KeepPetGlobalConfig": cfg = KeepPetGlobalConfigList as List<T>; break;
                case "KeepPetLevelConfig": cfg = KeepPetLevelConfigList as List<T>; break;
                case "KeepPetBuildingItemConfig": cfg = KeepPetBuildingItemConfigList as List<T>; break;
                case "KeepPetBuildingHangPointConfig": cfg = KeepPetBuildingHangPointConfigList as List<T>; break;
                case "KeepPetSearchTaskConfig": cfg = KeepPetSearchTaskConfigList as List<T>; break;
                case "KeepPetSearchTaskRewardPoolConfig": cfg = KeepPetSearchTaskRewardPoolConfigList as List<T>; break;
                case "KeepPetDailyTaskConfig": cfg = KeepPetDailyTaskConfigList as List<T>; break;
                case "KeepPetOrderConfig": cfg = KeepPetOrderConfigList as List<T>; break;
                case "KeepPetClueConfig": cfg = KeepPetClueConfigList as List<T>; break;
                case "KeepPetStoreConfig": cfg = KeepPetStoreConfigList as List<T>; break;
                case "KeepPetThreeOneStoreConfig": cfg = KeepPetThreeOneStoreConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/KeepPet/keeppet");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/KeepPet/keeppet error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            KeepPetGlobalConfigList = JsonConvert.DeserializeObject<List<KeepPetGlobalConfig>>(JsonConvert.SerializeObject(table["keeppetglobalconfig"]));
            KeepPetLevelConfigList = JsonConvert.DeserializeObject<List<KeepPetLevelConfig>>(JsonConvert.SerializeObject(table["keeppetlevelconfig"]));
            KeepPetBuildingItemConfigList = JsonConvert.DeserializeObject<List<KeepPetBuildingItemConfig>>(JsonConvert.SerializeObject(table["keeppetbuildingitemconfig"]));
            KeepPetBuildingHangPointConfigList = JsonConvert.DeserializeObject<List<KeepPetBuildingHangPointConfig>>(JsonConvert.SerializeObject(table["keeppetbuildinghangpointconfig"]));
            KeepPetSearchTaskConfigList = JsonConvert.DeserializeObject<List<KeepPetSearchTaskConfig>>(JsonConvert.SerializeObject(table["keeppetsearchtaskconfig"]));
            KeepPetSearchTaskRewardPoolConfigList = JsonConvert.DeserializeObject<List<KeepPetSearchTaskRewardPoolConfig>>(JsonConvert.SerializeObject(table["keeppetsearchtaskrewardpoolconfig"]));
            KeepPetDailyTaskConfigList = JsonConvert.DeserializeObject<List<KeepPetDailyTaskConfig>>(JsonConvert.SerializeObject(table["keeppetdailytaskconfig"]));
            KeepPetOrderConfigList = JsonConvert.DeserializeObject<List<KeepPetOrderConfig>>(JsonConvert.SerializeObject(table["keeppetorderconfig"]));
            KeepPetClueConfigList = JsonConvert.DeserializeObject<List<KeepPetClueConfig>>(JsonConvert.SerializeObject(table["keeppetclueconfig"]));
            KeepPetStoreConfigList = JsonConvert.DeserializeObject<List<KeepPetStoreConfig>>(JsonConvert.SerializeObject(table["keeppetstoreconfig"]));
            KeepPetThreeOneStoreConfigList = JsonConvert.DeserializeObject<List<KeepPetThreeOneStoreConfig>>(JsonConvert.SerializeObject(table["keeppetthreeonestoreconfig"]));
            
        }
    }
}