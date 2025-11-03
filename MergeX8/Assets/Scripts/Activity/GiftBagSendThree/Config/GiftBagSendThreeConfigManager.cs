/************************************************
 * GiftBagSendThree Config Manager class : GiftBagSendThreeConfigManager
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

namespace DragonPlus.Config.GiftBagSendThree
{
    public partial class GiftBagSendThreeConfigManager : Manager<GiftBagSendThreeConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<GiftBagSendThreeList> GiftBagSendThreeListList;
        public List<GiftBagSendThreeResource> GiftBagSendThreeResourceList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(GiftBagSendThreeList)] = "GiftBagSendThreeList",
            [typeof(GiftBagSendThreeResource)] = "GiftBagSendThreeResource",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("giftbagsendthreelist")) return false;
            if (!table.ContainsKey("giftbagsendthreeresource")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "GiftBagSendThreeList": cfg = GiftBagSendThreeListList as List<T>; break;
                case "GiftBagSendThreeResource": cfg = GiftBagSendThreeResourceList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/GiftBagSendThree/giftbagsendthree");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/GiftBagSendThree/giftbagsendthree error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            GiftBagSendThreeListList = JsonConvert.DeserializeObject<List<GiftBagSendThreeList>>(JsonConvert.SerializeObject(table["giftbagsendthreelist"]));
            GiftBagSendThreeResourceList = JsonConvert.DeserializeObject<List<GiftBagSendThreeResource>>(JsonConvert.SerializeObject(table["giftbagsendthreeresource"]));
            
        }
    }
}