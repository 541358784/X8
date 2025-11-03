/************************************************
 * GiftBagSendTwo Config Manager class : GiftBagSendTwoConfigManager
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

namespace DragonPlus.Config.GiftBagSendTwo
{
    public partial class GiftBagSendTwoConfigManager : Manager<GiftBagSendTwoConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<GiftBagSendTwoList> GiftBagSendTwoListList;
        public List<GiftBagSendTwoResource> GiftBagSendTwoResourceList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(GiftBagSendTwoList)] = "GiftBagSendTwoList",
            [typeof(GiftBagSendTwoResource)] = "GiftBagSendTwoResource",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("giftbagsendtwolist")) return false;
            if (!table.ContainsKey("giftbagsendtworesource")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "GiftBagSendTwoList": cfg = GiftBagSendTwoListList as List<T>; break;
                case "GiftBagSendTwoResource": cfg = GiftBagSendTwoResourceList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/GiftBagSendTwo/giftbagsendtwo");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/GiftBagSendTwo/giftbagsendtwo error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            GiftBagSendTwoListList = JsonConvert.DeserializeObject<List<GiftBagSendTwoList>>(JsonConvert.SerializeObject(table["giftbagsendtwolist"]));
            GiftBagSendTwoResourceList = JsonConvert.DeserializeObject<List<GiftBagSendTwoResource>>(JsonConvert.SerializeObject(table["giftbagsendtworesource"]));
            
        }
    }
}