/************************************************
 * GiftBagLink Config Manager class : GiftBagLinkConfigManager
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

namespace DragonPlus.Config.GiftBagLink
{
    public partial class GiftBagLinkConfigManager : Manager<GiftBagLinkConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<GiftBagLinkList> GiftBagLinkListList;
        public List<GiftBagLinkResource> GiftBagLinkResourceList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(GiftBagLinkList)] = "GiftBagLinkList",
            [typeof(GiftBagLinkResource)] = "GiftBagLinkResource",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("giftbaglinklist")) return false;
            if (!table.ContainsKey("giftbaglinkresource")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "GiftBagLinkList": cfg = GiftBagLinkListList as List<T>; break;
                case "GiftBagLinkResource": cfg = GiftBagLinkResourceList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/GiftBagLink/giftbaglink");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/GiftBagLink/giftbaglink error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            GiftBagLinkListList = JsonConvert.DeserializeObject<List<GiftBagLinkList>>(JsonConvert.SerializeObject(table["giftbaglinklist"]));
            GiftBagLinkResourceList = JsonConvert.DeserializeObject<List<GiftBagLinkResource>>(JsonConvert.SerializeObject(table["giftbaglinkresource"]));
            
        }
    }
}