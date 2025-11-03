
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.TipReward
{
    public partial class TipRewardConfigManager : TableSingleton<TipRewardConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableTipRewardSetting> TableTipRewardSettingList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableTipRewardSetting)] = "TableTipRewardSetting",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("tiprewardsetting")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableTipRewardSetting": cfg = TableTipRewardSettingList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/tipreward/tiprewardconfig", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/tipreward/tiprewardconfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableTipRewardSettingList = JsonConvert.DeserializeObject<List<TableTipRewardSetting>>(JsonConvert.SerializeObject(table["tiprewardsetting"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}