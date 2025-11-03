/************************************************
 * GiftBagSendOne Config Manager class : GiftBagSendOneConfigManager
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

namespace DragonPlus.Config.GiftBagSendOne
{
    public partial class GiftBagSendOneConfigManager : Manager<GiftBagSendOneConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<GiftBagSendOneList> GiftBagSendOneListList;
        public List<GiftBagSendOneResource> GiftBagSendOneResourceList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(GiftBagSendOneList)] = "GiftBagSendOneList",
            [typeof(GiftBagSendOneResource)] = "GiftBagSendOneResource",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("giftbagsendonelist")) return false;
            if (!table.ContainsKey("giftbagsendoneresource")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "GiftBagSendOneList": cfg = GiftBagSendOneListList as List<T>; break;
                case "GiftBagSendOneResource": cfg = GiftBagSendOneResourceList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/GiftBagSendOne/giftbagsendone");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/GiftBagSendOne/giftbagsendone error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            GiftBagSendOneListList = JsonConvert.DeserializeObject<List<GiftBagSendOneList>>(JsonConvert.SerializeObject(table["giftbagsendonelist"]));
            GiftBagSendOneResourceList = JsonConvert.DeserializeObject<List<GiftBagSendOneResource>>(JsonConvert.SerializeObject(table["giftbagsendoneresource"]));
            
        }
    }
}