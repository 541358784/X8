/************************************************
 * CardCollectionReopen Config Manager class : CardCollectionReopenConfigManager
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

namespace DragonPlus.Config.CardCollectionReopen
{
    public partial class CardCollectionReopenConfigManager : Manager<CardCollectionReopenConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<CardCollectionReopenThemeConfig> CardCollectionReopenThemeConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(CardCollectionReopenThemeConfig)] = "CardCollectionReopenThemeConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("cardcollectionreopenthemeconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "CardCollectionReopenThemeConfig": cfg = CardCollectionReopenThemeConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/CardCollectionReopen/cardcollectionreopen");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/CardCollectionReopen/cardcollectionreopen error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            CardCollectionReopenThemeConfigList = JsonConvert.DeserializeObject<List<CardCollectionReopenThemeConfig>>(JsonConvert.SerializeObject(table["cardcollectionreopenthemeconfig"]));
            
        }
    }
}