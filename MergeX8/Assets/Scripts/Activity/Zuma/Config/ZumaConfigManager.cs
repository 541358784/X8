/************************************************
 * Zuma Config Manager class : ZumaConfigManager
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

namespace DragonPlus.Config.Zuma
{
    public partial class ZumaConfigManager : Manager<ZumaConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<ZumaGlobalConfig> ZumaGlobalConfigList;
        public List<ZumaLevelConfig> ZumaLevelConfigList;
        public List<ZumaTaskRewardConfig> ZumaTaskRewardConfigList;
        public List<ZumaGiftBagConfig> ZumaGiftBagConfigList;
        public List<ZumaLeaderBoardRewardConfig> ZumaLeaderBoardRewardConfigList;
        public List<ZumaStoreItemConfig> ZumaStoreItemConfigList;
        public List<ZumaStoreLevelConfig> ZumaStoreLevelConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(ZumaGlobalConfig)] = "ZumaGlobalConfig",
            [typeof(ZumaLevelConfig)] = "ZumaLevelConfig",
            [typeof(ZumaTaskRewardConfig)] = "ZumaTaskRewardConfig",
            [typeof(ZumaGiftBagConfig)] = "ZumaGiftBagConfig",
            [typeof(ZumaLeaderBoardRewardConfig)] = "ZumaLeaderBoardRewardConfig",
            [typeof(ZumaStoreItemConfig)] = "ZumaStoreItemConfig",
            [typeof(ZumaStoreLevelConfig)] = "ZumaStoreLevelConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("zumaglobalconfig")) return false;
            if (!table.ContainsKey("zumalevelconfig")) return false;
            if (!table.ContainsKey("zumataskrewardconfig")) return false;
            if (!table.ContainsKey("zumagiftbagconfig")) return false;
            if (!table.ContainsKey("zumaleaderboardrewardconfig")) return false;
            if (!table.ContainsKey("zumastoreitemconfig")) return false;
            if (!table.ContainsKey("zumastorelevelconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "ZumaGlobalConfig": cfg = ZumaGlobalConfigList as List<T>; break;
                case "ZumaLevelConfig": cfg = ZumaLevelConfigList as List<T>; break;
                case "ZumaTaskRewardConfig": cfg = ZumaTaskRewardConfigList as List<T>; break;
                case "ZumaGiftBagConfig": cfg = ZumaGiftBagConfigList as List<T>; break;
                case "ZumaLeaderBoardRewardConfig": cfg = ZumaLeaderBoardRewardConfigList as List<T>; break;
                case "ZumaStoreItemConfig": cfg = ZumaStoreItemConfigList as List<T>; break;
                case "ZumaStoreLevelConfig": cfg = ZumaStoreLevelConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/Zuma/zuma");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/Zuma/zuma error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            ZumaGlobalConfigList = JsonConvert.DeserializeObject<List<ZumaGlobalConfig>>(JsonConvert.SerializeObject(table["zumaglobalconfig"]));
            ZumaLevelConfigList = JsonConvert.DeserializeObject<List<ZumaLevelConfig>>(JsonConvert.SerializeObject(table["zumalevelconfig"]));
            ZumaTaskRewardConfigList = JsonConvert.DeserializeObject<List<ZumaTaskRewardConfig>>(JsonConvert.SerializeObject(table["zumataskrewardconfig"]));
            ZumaGiftBagConfigList = JsonConvert.DeserializeObject<List<ZumaGiftBagConfig>>(JsonConvert.SerializeObject(table["zumagiftbagconfig"]));
            ZumaLeaderBoardRewardConfigList = JsonConvert.DeserializeObject<List<ZumaLeaderBoardRewardConfig>>(JsonConvert.SerializeObject(table["zumaleaderboardrewardconfig"]));
            ZumaStoreItemConfigList = JsonConvert.DeserializeObject<List<ZumaStoreItemConfig>>(JsonConvert.SerializeObject(table["zumastoreitemconfig"]));
            ZumaStoreLevelConfigList = JsonConvert.DeserializeObject<List<ZumaStoreLevelConfig>>(JsonConvert.SerializeObject(table["zumastorelevelconfig"]));
            
        }
    }
}