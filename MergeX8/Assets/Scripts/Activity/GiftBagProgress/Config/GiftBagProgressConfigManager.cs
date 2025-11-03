/************************************************
 * GiftBagProgress Config Manager class : GiftBagProgressConfigManager
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

namespace DragonPlus.Config.GiftBagProgress
{
    public partial class GiftBagProgressConfigManager : Manager<GiftBagProgressConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<GiftBagProgressGlobalConfig> GiftBagProgressGlobalConfigList;
        public List<GiftBagProgressTaskConfig> GiftBagProgressTaskConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(GiftBagProgressGlobalConfig)] = "GiftBagProgressGlobalConfig",
            [typeof(GiftBagProgressTaskConfig)] = "GiftBagProgressTaskConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("giftbagprogressglobalconfig")) return false;
            if (!table.ContainsKey("giftbagprogresstaskconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "GiftBagProgressGlobalConfig": cfg = GiftBagProgressGlobalConfigList as List<T>; break;
                case "GiftBagProgressTaskConfig": cfg = GiftBagProgressTaskConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/GiftBagProgress/giftbagprogress");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/GiftBagProgress/giftbagprogress error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            GiftBagProgressGlobalConfigList = JsonConvert.DeserializeObject<List<GiftBagProgressGlobalConfig>>(JsonConvert.SerializeObject(table["giftbagprogressglobalconfig"]));
            GiftBagProgressTaskConfigList = JsonConvert.DeserializeObject<List<GiftBagProgressTaskConfig>>(JsonConvert.SerializeObject(table["giftbagprogresstaskconfig"]));
            
        }
    }
}