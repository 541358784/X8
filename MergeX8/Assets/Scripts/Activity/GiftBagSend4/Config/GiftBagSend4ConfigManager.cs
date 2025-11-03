/************************************************
 * GiftBagSend4 Config Manager class : GiftBagSend4ConfigManager
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

namespace DragonPlus.Config.GiftBagSend4
{
    public partial class GiftBagSend4ConfigManager : Manager<GiftBagSend4ConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<GiftBagSend4List> GiftBagSend4ListList;
        public List<GiftBagSend4Resource> GiftBagSend4ResourceList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(GiftBagSend4List)] = "GiftBagSend4List",
            [typeof(GiftBagSend4Resource)] = "GiftBagSend4Resource",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("giftbagsend4list")) return false;
            if (!table.ContainsKey("giftbagsend4resource")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "GiftBagSend4List": cfg = GiftBagSend4ListList as List<T>; break;
                case "GiftBagSend4Resource": cfg = GiftBagSend4ResourceList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/GiftBagSend4/giftbagsend4");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/GiftBagSend4/giftbagsend4 error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            GiftBagSend4ListList = JsonConvert.DeserializeObject<List<GiftBagSend4List>>(JsonConvert.SerializeObject(table["giftbagsend4list"]));
            GiftBagSend4ResourceList = JsonConvert.DeserializeObject<List<GiftBagSend4Resource>>(JsonConvert.SerializeObject(table["giftbagsend4resource"]));
            
        }
    }
}