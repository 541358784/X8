/************************************************
 * AdLocal Config Manager class : AdLocalConfigManager
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

namespace DragonPlus.Config.AdLocal
{
    public partial class AdLocalConfigManager : Manager<AdLocalConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<CampaignInto> CampaignIntoList;
        public List<UserTypeInto> UserTypeIntoList;
        public List<PayLevelConfig> PayLevelConfigList;
        public List<UserTypeConfig> UserTypeConfigList;
        public List<NewUserIntoPolling> NewUserIntoPollingList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(CampaignInto)] = "CampaignInto",
            [typeof(UserTypeInto)] = "UserTypeInto",
            [typeof(PayLevelConfig)] = "PayLevelConfig",
            [typeof(UserTypeConfig)] = "UserTypeConfig",
            [typeof(NewUserIntoPolling)] = "NewUserIntoPolling",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("campaigninto")) return false;
            if (!table.ContainsKey("usertypeinto")) return false;
            if (!table.ContainsKey("paylevelconfig")) return false;
            if (!table.ContainsKey("usertypeconfig")) return false;
            if (!table.ContainsKey("newuserintopolling")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "CampaignInto": cfg = CampaignIntoList as List<T>; break;
                case "UserTypeInto": cfg = UserTypeIntoList as List<T>; break;
                case "PayLevelConfig": cfg = PayLevelConfigList as List<T>; break;
                case "UserTypeConfig": cfg = UserTypeConfigList as List<T>; break;
                case "NewUserIntoPolling": cfg = NewUserIntoPollingList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/UserGroup/adlocal");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/UserGroup/adlocal error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            CampaignIntoList = JsonConvert.DeserializeObject<List<CampaignInto>>(JsonConvert.SerializeObject(table["campaigninto"]));
            UserTypeIntoList = JsonConvert.DeserializeObject<List<UserTypeInto>>(JsonConvert.SerializeObject(table["usertypeinto"]));
            PayLevelConfigList = JsonConvert.DeserializeObject<List<PayLevelConfig>>(JsonConvert.SerializeObject(table["paylevelconfig"]));
            UserTypeConfigList = JsonConvert.DeserializeObject<List<UserTypeConfig>>(JsonConvert.SerializeObject(table["usertypeconfig"]));
            NewUserIntoPollingList = JsonConvert.DeserializeObject<List<NewUserIntoPolling>>(JsonConvert.SerializeObject(table["newuserintopolling"]));
            
        }
    }
}