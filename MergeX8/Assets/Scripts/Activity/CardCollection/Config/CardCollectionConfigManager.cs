/************************************************
 * CardCollection Config Manager class : CardCollectionConfigManager
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

namespace DragonPlus.Config.CardCollection
{
    public partial class CardCollectionConfigManager : Manager<CardCollectionConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<CardCollectionThemeConfig> CardCollectionThemeConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(CardCollectionThemeConfig)] = "CardCollectionThemeConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("cardcollectionthemeconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "CardCollectionThemeConfig": cfg = CardCollectionThemeConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/CardCollection/cardcollection");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/CardCollection/cardcollection error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            CardCollectionThemeConfigList = JsonConvert.DeserializeObject<List<CardCollectionThemeConfig>>(JsonConvert.SerializeObject(table["cardcollectionthemeconfig"]));
            
        }
    }
}