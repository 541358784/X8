/************************************************
 * Filthy Config Manager class : FilthyConfigManager
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

namespace DragonPlus.Config.Filthy
{
    public partial class FilthyConfigManager : TableSingleton<FilthyConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<FilthyASetting> FilthyASettingList;
        public List<FilthyBSetting> FilthyBSettingList;
        public List<FilthyAProcedure> FilthyAProcedureList;
        public List<FilthyBProcedure> FilthyBProcedureList;
        public List<FilthyNodes> FilthyNodesList;
        public List<FilthySpine> FilthySpineList;
        public List<FilthyMerge> FilthyMergeList;
        public List<FilthyBoard> FilthyBoardList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(FilthyASetting)] = "FilthyASetting",
            [typeof(FilthyBSetting)] = "FilthyBSetting",
            [typeof(FilthyAProcedure)] = "FilthyAProcedure",
            [typeof(FilthyBProcedure)] = "FilthyBProcedure",
            [typeof(FilthyNodes)] = "FilthyNodes",
            [typeof(FilthySpine)] = "FilthySpine",
            [typeof(FilthyMerge)] = "FilthyMerge",
            [typeof(FilthyBoard)] = "FilthyBoard",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("filthyasetting")) return false;
            if (!table.ContainsKey("filthybsetting")) return false;
            if (!table.ContainsKey("filthyaprocedure")) return false;
            if (!table.ContainsKey("filthybprocedure")) return false;
            if (!table.ContainsKey("filthynodes")) return false;
            if (!table.ContainsKey("filthyspine")) return false;
            if (!table.ContainsKey("filthymerge")) return false;
            if (!table.ContainsKey("filthyboard")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "FilthyASetting": cfg = FilthyASettingList as List<T>; break;
                case "FilthyBSetting": cfg = FilthyBSettingList as List<T>; break;
                case "FilthyAProcedure": cfg = FilthyAProcedureList as List<T>; break;
                case "FilthyBProcedure": cfg = FilthyBProcedureList as List<T>; break;
                case "FilthyNodes": cfg = FilthyNodesList as List<T>; break;
                case "FilthySpine": cfg = FilthySpineList as List<T>; break;
                case "FilthyMerge": cfg = FilthyMergeList as List<T>; break;
                case "FilthyBoard": cfg = FilthyBoardList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/filthy/filthyconfig");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/filthy/filthyconfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            FilthyASettingList = JsonConvert.DeserializeObject<List<FilthyASetting>>(JsonConvert.SerializeObject(table["filthyasetting"]));
            FilthyBSettingList = JsonConvert.DeserializeObject<List<FilthyBSetting>>(JsonConvert.SerializeObject(table["filthybsetting"]));
            FilthyAProcedureList = JsonConvert.DeserializeObject<List<FilthyAProcedure>>(JsonConvert.SerializeObject(table["filthyaprocedure"]));
            FilthyBProcedureList = JsonConvert.DeserializeObject<List<FilthyBProcedure>>(JsonConvert.SerializeObject(table["filthybprocedure"]));
            FilthyNodesList = JsonConvert.DeserializeObject<List<FilthyNodes>>(JsonConvert.SerializeObject(table["filthynodes"]));
            FilthySpineList = JsonConvert.DeserializeObject<List<FilthySpine>>(JsonConvert.SerializeObject(table["filthyspine"]));
            FilthyMergeList = JsonConvert.DeserializeObject<List<FilthyMerge>>(JsonConvert.SerializeObject(table["filthymerge"]));
            FilthyBoardList = JsonConvert.DeserializeObject<List<FilthyBoard>>(JsonConvert.SerializeObject(table["filthyboard"]));
            

            Trim();
        }
    }
}