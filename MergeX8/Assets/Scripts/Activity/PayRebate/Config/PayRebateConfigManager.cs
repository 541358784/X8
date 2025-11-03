/************************************************
 * PayRebate Config Manager class : PayRebateConfigManager
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

namespace DragonPlus.Config.PayRebate
{
    public partial class PayRebateConfigManager : Manager<PayRebateConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<PayRebateConfig> PayRebateConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(PayRebateConfig)] = "PayRebateConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("payrebateconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "PayRebateConfig": cfg = PayRebateConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/PayRebate/payrebate");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/PayRebate/payrebate error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            PayRebateConfigList = JsonConvert.DeserializeObject<List<PayRebateConfig>>(JsonConvert.SerializeObject(table["payrebateconfig"]));
            
        }
    }
}