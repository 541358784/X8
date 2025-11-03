/************************************************
 * GiftBagBuyBetter Config Manager class : GiftBagBuyBetterConfigManager
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

namespace DragonPlus.Config.GiftBagBuyBetter
{
    public partial class GiftBagBuyBetterConfigManager : Manager<GiftBagBuyBetterConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<GiftBagBuyBetterList> GiftBagBuyBetterListList;
        public List<GiftBagBuyBetterResource> GiftBagBuyBetterResourceList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(GiftBagBuyBetterList)] = "GiftBagBuyBetterList",
            [typeof(GiftBagBuyBetterResource)] = "GiftBagBuyBetterResource",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("giftbagbuybetterlist")) return false;
            if (!table.ContainsKey("giftbagbuybetterresource")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "GiftBagBuyBetterList": cfg = GiftBagBuyBetterListList as List<T>; break;
                case "GiftBagBuyBetterResource": cfg = GiftBagBuyBetterResourceList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/GiftBagBuyBetter/giftbagbuybetter");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/GiftBagBuyBetter/giftbagbuybetter error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            GiftBagBuyBetterListList = JsonConvert.DeserializeObject<List<GiftBagBuyBetterList>>(JsonConvert.SerializeObject(table["giftbagbuybetterlist"]));
            GiftBagBuyBetterResourceList = JsonConvert.DeserializeObject<List<GiftBagBuyBetterResource>>(JsonConvert.SerializeObject(table["giftbagbuybetterresource"]));
            
        }
    }
}