
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.Ditch
{
    public partial class DitchConfigManager : TableSingleton<DitchConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableDitchLevel> TableDitchLevelList;
        public List<TableDitchMerge> TableDitchMergeList;
        public List<TableDitchBoard> TableDitchBoardList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableDitchLevel)] = "TableDitchLevel",
            [typeof(TableDitchMerge)] = "TableDitchMerge",
            [typeof(TableDitchBoard)] = "TableDitchBoard",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("ditchlevel")) return false;
            if (!table.ContainsKey("ditchmerge")) return false;
            if (!table.ContainsKey("ditchboard")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableDitchLevel": cfg = TableDitchLevelList as List<T>; break;
                case "TableDitchMerge": cfg = TableDitchMergeList as List<T>; break;
                case "TableDitchBoard": cfg = TableDitchBoardList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/ditch/ditchconfig", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/ditch/ditchconfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableDitchLevelList = JsonConvert.DeserializeObject<List<TableDitchLevel>>(JsonConvert.SerializeObject(table["ditchlevel"]));
            TableDitchMergeList = JsonConvert.DeserializeObject<List<TableDitchMerge>>(JsonConvert.SerializeObject(table["ditchmerge"]));
            TableDitchBoardList = JsonConvert.DeserializeObject<List<TableDitchBoard>>(JsonConvert.SerializeObject(table["ditchboard"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}