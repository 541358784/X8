/************************************************
 * Matreshkas Config Manager class : MatreshkasConfigManager
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

namespace DragonPlus.Config.Matreshkas
{
    public partial class MatreshkasConfigManager : Manager<MatreshkasConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<MatreshkasSetting> MatreshkasSettingList;
        public List<MatreshkasConfig> MatreshkasConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(MatreshkasSetting)] = "MatreshkasSetting",
            [typeof(MatreshkasConfig)] = "MatreshkasConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("matreshkassetting")) return false;
            if (!table.ContainsKey("matreshkasconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "MatreshkasSetting": cfg = MatreshkasSettingList as List<T>; break;
                case "MatreshkasConfig": cfg = MatreshkasConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/Matreshkas/config");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/Matreshkas/config error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            MatreshkasSettingList = JsonConvert.DeserializeObject<List<MatreshkasSetting>>(JsonConvert.SerializeObject(table["matreshkassetting"]));
            MatreshkasConfigList = JsonConvert.DeserializeObject<List<MatreshkasConfig>>(JsonConvert.SerializeObject(table["matreshkasconfig"]));
            
        }
    }
}