// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * Ad ConfigHub Manager class : AdConfigManager
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DragonU3DSDK.Asset;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Util;

namespace DragonPlus.ConfigHub.Ad
{
    public class AdConfigManager : ConfigManagerBase
    {   
        private static AdConfigManager _instance;
        public static AdConfigManager Instance => _instance ?? (_instance = new AdConfigManager());
        public override string Guid => "config_ad";
        public override int VersionMinIOS => 21;
        public override int VersionMinAndroid => 21;
        protected override List<string> SubModules => new List<string> { 
            "Common",
            "OrderRules",
            "RVAd",
            "Bonus",
            "InterstitialAd",
            "Global",
        };
        private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(Common)] = "Common",
            [typeof(OrderRules)] = "OrderRules",
            [typeof(RVAd)] = "RVAd",
            [typeof(Bonus)] = "Bonus",
            [typeof(InterstitialAd)] = "InterstitialAd",
            [typeof(Global)] = "Global",
        };
        private List<Common> CommonList;
        private List<OrderRules> OrderRulesList;
        private List<RVAd> RVAdList;
        private List<Bonus> BonusList;
        private List<InterstitialAd> InterstitialAdList;
        private List<Global> GlobalList;
        
        public override List<T> GetConfig<T>(CacheOperate cacheOp = CacheOperate.None, long cacheDuration = -1)
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "Common": cfg = CommonList as List<T>; break;
                case "OrderRules": cfg = OrderRulesList as List<T>; break;
                case "RVAd": cfg = RVAdList as List<T>; break;
                case "Bonus": cfg = BonusList as List<T>; break;
                case "InterstitialAd": cfg = InterstitialAdList as List<T>; break;
                case "Global": cfg = GlobalList as List<T>; break;
                default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
            }
            processCache(cacheOp, cacheDuration);
            return cfg;
        }

        protected override bool CheckTable(Hashtable table)
        {   
            if (!table.ContainsKey("common")) return false;
            if (!table.ContainsKey("orderrules")) return false;
            if (!table.ContainsKey("rvad")) return false;
            if (!table.ContainsKey("bonus")) return false;
            if (!table.ContainsKey("interstitialad")) return false;
            if (!table.ContainsKey("global")) return false;
            return true;
        }

        private Hashtable loadFromLocal()
        {
            var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/UserGroup/ad");
            if (string.IsNullOrEmpty(ta.text))
            {
                ConfigHubUtil.L("Load Configs/UserGroup/ad error!");
                return null;
            }
            return JsonConvert.DeserializeObject<Hashtable>(ta.text);
        }

        public override void InitConfig(MetaData metaData, string jsonData = null)
        {
           
            MetaData = metaData ?? (GetMetaDataCached() ?? GetMetaDataDefault());
            
            ConfigHubUtil.L($"InitConfig:{getModuleString()}");
        }

        private List<Rules> RulesList;
        protected override bool HasGroup(int groupId)
        {
            if (RulesList == null || RulesList.Count == 0)
            {
                var table = loadFromLocal();
                RulesList = JsonConvert.DeserializeObject<List<Rules>>(JsonConvert.SerializeObject(table["rules"]));
            }
            return RulesList.Exists(r => r.GroupId == groupId);
        }

        public void InitLocal()
        {
            InitLocalConfig();
            InitServerConfig();
        }

        public void InitServerConfig()
        {
            CGetConfig cGetConfig = new CGetConfig
            {
                Route = "UserGroup_Ad_" + AssetConfigController.Instance.RootVersion
            };
            
            bool useServerConfig = false;

            APIManager.Instance.Send(cGetConfig, (SGetConfig sGetConfig) =>
            {
                Hashtable table = null;
                try
                {
                    if (string.IsNullOrEmpty(sGetConfig.Config.Json) == false)
                    {
                        table = JsonConvert.DeserializeObject<Hashtable>(sGetConfig.Config.Json);
                    }

                    useServerConfig = table != null && CheckTable(table);
                }
                catch (Exception ex)
                {
                    useServerConfig = false;
                    DebugUtil.LogError($"{ex.ToString()}");
                }

                if (!useServerConfig)
                    return;

                InitConfig2(table);
                var encryptData = RijndaelManager.Instance.EncryptStringToBytes(sGetConfig.Config.Json);
                PlayerPrefs.SetString(_configCacheKey, System.Convert.ToBase64String(encryptData));
            }, (errno, msg, resp) => {  });
        }

        private string _configCacheKey = "AdConfigMangerConfigKey"+AssetConfigController.Instance.RootVersion;

        public void InitLocalConfig()
        {
            
            if (!PlayerPrefs.HasKey(_configCacheKey))
            {
                Debug.LogError("AdConfigManger  没有缓存 走本地配置");
                InitConfig2(loadFromLocal());
            }
            else
            {
                var encryptData = System.Convert.FromBase64String(PlayerPrefs.GetString(_configCacheKey));
                var cacheString = RijndaelManager.Instance.DecryptStringFromBytes(encryptData);
                Hashtable table  = JsonConvert.DeserializeObject<Hashtable>(cacheString);
                if ( table != null && CheckTable(table))
                {
                    Debug.LogError("AdConfigManger  走服务器缓存配置");
                    InitConfig2(table);
                }
                else
                {
                    Debug.LogError("AdConfigManger  缓存数据有误走本地配置");
                    InitConfig2(loadFromLocal());
                }
            }
        }


        public void InitConfig2(Hashtable table)
        {
            foreach (var subModule in SubModules)
            {
                try
                {
                    switch (subModule)
                    { 
                        case "Common":
                            if(table["common"] != null)
                                CommonList = JsonConvert.DeserializeObject<List<Common>>(JsonConvert.SerializeObject(table["common"])); 
                            break;
                        case "OrderRules":
                            if(table["orderrules"] != null)
                                OrderRulesList = JsonConvert.DeserializeObject<List<OrderRules>>(JsonConvert.SerializeObject(table["orderrules"])); 
                            break;
                        case "RVAd":
                            if(table["rvad"] != null)
                                RVAdList = JsonConvert.DeserializeObject<List<RVAd>>(JsonConvert.SerializeObject(table["rvad"])); 
                            break;
                        case "Bonus":
                            if(table["bonus"] != null)
                                BonusList = JsonConvert.DeserializeObject<List<Bonus>>(JsonConvert.SerializeObject(table["bonus"])); 
                            break;
                        case "InterstitialAd":
                            if(table["interstitialad"] != null)
                                InterstitialAdList = JsonConvert.DeserializeObject<List<InterstitialAd>>(JsonConvert.SerializeObject(table["interstitialad"])); 
                            break;
                        case "Global":
                            if(table["global"] != null)
                                GlobalList = JsonConvert.DeserializeObject<List<Global>>(JsonConvert.SerializeObject(table["global"])); 
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
                    }
                }
                catch(Exception ex)
                {
                    DebugUtil.LogError($"ex {ex.ToString()}");
                }
            }
        }

    }
}