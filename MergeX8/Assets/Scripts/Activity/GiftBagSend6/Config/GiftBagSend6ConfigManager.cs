/************************************************
 * GiftBagSend6 Config Manager class : GiftBagSend6ConfigManager
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

namespace DragonPlus.Config.GiftBagSend6
{
    public partial class GiftBagSend6ConfigManager : Manager<GiftBagSend6ConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<GiftBagSend6List> GiftBagSend6ListList;
        public List<GiftBagSend6Resource> GiftBagSend6ResourceList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(GiftBagSend6List)] = "GiftBagSend6List",
            [typeof(GiftBagSend6Resource)] = "GiftBagSend6Resource",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("giftbagsend6list")) return false;
            if (!table.ContainsKey("giftbagsend6resource")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "GiftBagSend6List": cfg = GiftBagSend6ListList as List<T>; break;
                case "GiftBagSend6Resource": cfg = GiftBagSend6ResourceList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/GiftBagSend6/giftbagsend6");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/GiftBagSend6/giftbagsend6 error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            GiftBagSend6ListList = JsonConvert.DeserializeObject<List<GiftBagSend6List>>(JsonConvert.SerializeObject(table["giftbagsend6list"]));
            GiftBagSend6ResourceList = JsonConvert.DeserializeObject<List<GiftBagSend6Resource>>(JsonConvert.SerializeObject(table["giftbagsend6resource"]));
            
        }
    }
}