/************************************************
 * EasterGift Config Manager class : EasterGiftConfigManager
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

namespace DragonPlus.Config.EasterGift
{
    public partial class EasterGiftConfigManager : Manager<EasterGiftConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<EasterBundle> EasterBundleList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(EasterBundle)] = "EasterBundle",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("easterbundle")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "EasterBundle": cfg = EasterBundleList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/EasterGift/eastergift");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/EasterGift/eastergift error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            EasterBundleList = JsonConvert.DeserializeObject<List<EasterBundle>>(JsonConvert.SerializeObject(table["easterbundle"]));
            
        }
    }
}