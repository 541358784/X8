
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.JumpGrid
{
    public partial class JumpGridConfigManager : TableSingleton<JumpGridConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableJumpGridConfig> TableJumpGridConfigList;
        public List<TableJumpGridReward> TableJumpGridRewardList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableJumpGridConfig)] = "TableJumpGridConfig",
            [typeof(TableJumpGridReward)] = "TableJumpGridReward",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("jumpgridconfig")) return false;
            if (!table.ContainsKey("jumpgridreward")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableJumpGridConfig": cfg = TableJumpGridConfigList as List<T>; break;
                case "TableJumpGridReward": cfg = TableJumpGridRewardList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/JumpGrid/JumpGridConfig", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/JumpGrid/JumpGridConfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableJumpGridConfigList = JsonConvert.DeserializeObject<List<TableJumpGridConfig>>(JsonConvert.SerializeObject(table["jumpgridconfig"]));
            TableJumpGridRewardList = JsonConvert.DeserializeObject<List<TableJumpGridReward>>(JsonConvert.SerializeObject(table["jumpgridreward"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}