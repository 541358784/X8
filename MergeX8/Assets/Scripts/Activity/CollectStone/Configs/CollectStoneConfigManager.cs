
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.CollectStone
{
    public partial class CollectStoneConfigManager : TableSingleton<CollectStoneConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableCollectSetting> TableCollectSettingList;
        public List<TableCollectReward> TableCollectRewardList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableCollectSetting)] = "TableCollectSetting",
            [typeof(TableCollectReward)] = "TableCollectReward",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("collectsetting")) return false;
            if (!table.ContainsKey("collectreward")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableCollectSetting": cfg = TableCollectSettingList as List<T>; break;
                case "TableCollectReward": cfg = TableCollectRewardList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/CollectStone/CollectStoneConfig", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/CollectStone/CollectStoneConfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableCollectSettingList = JsonConvert.DeserializeObject<List<TableCollectSetting>>(JsonConvert.SerializeObject(table["collectsetting"]));
            TableCollectRewardList = JsonConvert.DeserializeObject<List<TableCollectReward>>(JsonConvert.SerializeObject(table["collectreward"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}