/************************************************
 * GiftBagDouble Config Manager class : GiftBagDoubleConfigManager
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

namespace DragonPlus.Config.GiftBagDouble
{
    public partial class GiftBagDoubleConfigManager : Manager<GiftBagDoubleConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<GiftBagDoubleGlobalConfig> GiftBagDoubleGlobalConfigList;
        public List<GiftBagDoubleGroupConfig> GiftBagDoubleGroupConfigList;
        public List<GiftBagDoubleProductConfig> GiftBagDoubleProductConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(GiftBagDoubleGlobalConfig)] = "GiftBagDoubleGlobalConfig",
            [typeof(GiftBagDoubleGroupConfig)] = "GiftBagDoubleGroupConfig",
            [typeof(GiftBagDoubleProductConfig)] = "GiftBagDoubleProductConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("giftbagdoubleglobalconfig")) return false;
            if (!table.ContainsKey("giftbagdoublegroupconfig")) return false;
            if (!table.ContainsKey("giftbagdoubleproductconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "GiftBagDoubleGlobalConfig": cfg = GiftBagDoubleGlobalConfigList as List<T>; break;
                case "GiftBagDoubleGroupConfig": cfg = GiftBagDoubleGroupConfigList as List<T>; break;
                case "GiftBagDoubleProductConfig": cfg = GiftBagDoubleProductConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/GiftBagDouble/giftbagdouble");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/GiftBagDouble/giftbagdouble error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            GiftBagDoubleGlobalConfigList = JsonConvert.DeserializeObject<List<GiftBagDoubleGlobalConfig>>(JsonConvert.SerializeObject(table["giftbagdoubleglobalconfig"]));
            GiftBagDoubleGroupConfigList = JsonConvert.DeserializeObject<List<GiftBagDoubleGroupConfig>>(JsonConvert.SerializeObject(table["giftbagdoublegroupconfig"]));
            GiftBagDoubleProductConfigList = JsonConvert.DeserializeObject<List<GiftBagDoubleProductConfig>>(JsonConvert.SerializeObject(table["giftbagdoubleproductconfig"]));
            
        }
    }
}