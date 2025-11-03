/************************************************
 * TotalRecharge Config Manager class : TotalRechargeConfigManager
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

namespace DragonPlus.Config.TotalRecharge
{
    public partial class TotalRechargeConfigManager : Manager<TotalRechargeConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<TotalRechargeReward> TotalRechargeRewardList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TotalRechargeReward)] = "TotalRechargeReward",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("totalrechargereward")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TotalRechargeReward": cfg = TotalRechargeRewardList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/TotalRecharge/config");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/TotalRecharge/config error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TotalRechargeRewardList = JsonConvert.DeserializeObject<List<TotalRechargeReward>>(JsonConvert.SerializeObject(table["totalrechargereward"]));
            
        }
    }
}