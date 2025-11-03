using System;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DragonU3DSDK;

namespace Dlugin
{
    public sealed class AdConfig
    {

        private static AdConfig instance = null;
        private static readonly object syslock = new object();

        public static AdConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        if (instance == null)
                        {
                            instance = new AdConfig();
                        }
                    }
                }
                return instance;
            }
        }

        private AdConfig()
        {
            ReloadConfigs();
        }


        public const string AD_GLOBAL_KEY = "ad/global";
        public const string AD_INTERSIT_GLOBAL = "ad/globalinterstitial";
        public const string AD_OFFERWALL_GLOBAL = "ad/globalofferwall";

        public string AdGlobalKey { get { return AD_GLOBAL_KEY; } }
        public string InterstitialKey { get { return AD_INTERSIT_GLOBAL; } }
        public string OfferwallKey { get { return AD_OFFERWALL_GLOBAL; } }

        Dictionary<string, int> AdWeight = new Dictionary<string, int>();
        Dictionary<string, int> InterstitialWeight = new Dictionary<string, int>();
        Dictionary<string, int> OfferwallWeight = new Dictionary<string, int>();

        Dictionary<string, float> AdUnitPlacementEcpmDic = new Dictionary<string, float>();



        /// <summary>
        /// 由广告位置ID,
        /// </summary>
        /// <param name="placement"></param>
        /// <returns></returns>
        public float GetEcpmFloorByAdPlacementID(string placement)
        {
            if (string.IsNullOrEmpty(placement))
            {
                return 0.0f;
            }

            if (!AdUnitPlacementEcpmDic.ContainsKey(placement))
            {
                return 0.0f;
            }

            return AdUnitPlacementEcpmDic[placement];

        }


        /// <summary>
        /// Gets the ad weight.
        /// GetAdWeight返回奖励视频权重
        /// </summary>
        /// <returns>The ad weight.</returns>
        public Dictionary<string, int> GetAdWeight()
        {
            return AdWeight;
        }
        public int GetAdWeightByPluginName(string name)
        {
            if (AdWeight == null || string.IsNullOrEmpty(name))
            {
                return 0;
            }
            if (!AdWeight.ContainsKey(name))
            {
                return 0;
            }

            return AdWeight[name];
        }

        public Dictionary<string, int> GetInterstitialWeight()
        {
            return InterstitialWeight;
        }

        public int GetInterstitialWeightByPluginName(string name)
        {
            if (InterstitialWeight == null || string.IsNullOrEmpty(name))
            {
                return 0;
            }
            if (!InterstitialWeight.ContainsKey(name))
            {
                return 0;
            }

            return InterstitialWeight[name];
        }

        public Dictionary<string, int> GetOfferwallWeight()
        {
            return OfferwallWeight;
        }

        public void UpdateAdConfig(Dictionary<string, int> newConfig)
        {
            if (newConfig == null)
            {
                return;
            }
            var list = newConfig.Select((KeyValuePair<string, int> element) => element.Value > 0);
            if (list.Count <= 0)
            {
                return;
            }

            AdWeight.Clear();
            foreach (var p in list)
            {
                AdWeight.Add(p.Key, p.Value);
            }
            SyncAdConfig();
        }

        public void UpdateInterstitialConfig(Dictionary<string, int> newConfig)
        {
            if (newConfig == null)
            {
                return;
            }
            var list = newConfig.Select((KeyValuePair<string, int> element) => element.Value > 0);
            if (list.Count <= 0)
            {
                return;
            }

            InterstitialWeight.Clear();
            foreach (var p in list)
            {
                InterstitialWeight.Add(p.Key, p.Value);
            }
            SyncAdConfig();
        }

        public void UpdateOfferwallConfig(Dictionary<string, int> newConfig)
        {
            if (newConfig == null)
            {
                return;
            }
            var list = newConfig.Select((KeyValuePair<string, int> element) => element.Value > 0);
            if (list.Count <= 0)
            {
                return;
            }

            OfferwallWeight.Clear();
            foreach (var p in list)
            {
                OfferwallWeight.Add(p.Key, p.Value);
            }
            SyncAdConfig();
        }
        
        public void ReloadConfigs()
        {
            bool needReinitAd = false;
            bool needReinitInter = false;
            bool needReinitOfferwall = false;
            //如果能拿到远程配置数据
            var adGlobalConfigJson = DragonU3DSDK.Utils.ReadFromLocal(AD_GLOBAL_KEY);
            if (!string.IsNullOrEmpty(adGlobalConfigJson))
            {
                try
                {
                    Dictionary<string, int> config = JsonConvert.DeserializeObject<Dictionary<string, int>>(adGlobalConfigJson);

                    if (config.Count > 0)
                    {
                        var list = config.Select((KeyValuePair<string, int> element) => element.Value > 0);
                        if (list.Count > 0)
                        {
                            AdWeight.Clear();
                            foreach (var p in list)
                            {
                                AdWeight.Add(p.Key, p.Value);
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    DebugUtil.LogError(e.ToString());
                    //发生异常，取配置表中配置当广告配置
                    needReinitAd = true;
                }
            }
            else
            {
                needReinitAd = true;
            }

            if (needReinitAd)
            {
                AdWeight.Clear();


                if (PluginsInfoManager.Instance.UsePlugin(Constants.Admob))
                {
                    AdmobConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<AdmobConfigInfo>(Constants.Admob);
                    if (config.adsWeight > 0)
                    {
                        AdWeight.Add(Constants.Admob, config.adsWeight);
                        ProcessEcpmInfo(config);
                    }
                }

                if (PluginsInfoManager.Instance.UsePlugin(Constants.Chartboost))
                {
                    ChartboostConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<ChartboostConfigInfo>(Constants.Chartboost);
                    if (config.adsWeight > 0)
                    {
                        AdWeight.Add(Constants.Chartboost, config.adsWeight);
                        ProcessEcpmInfo(config);
                    }
                }

                if (PluginsInfoManager.Instance.UsePlugin(Constants.IronSource))
                {
                    IronSourceConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<IronSourceConfigInfo>(Constants.IronSource);
                    if (config.adsWeight > 0)
                    {
                        AdWeight.Add(Constants.IronSource, config.adsWeight);
                        ProcessEcpmInfo(config);
                    }
                }

                if (PluginsInfoManager.Instance.UsePlugin(Constants.UnityAds))
                {
                    UnityAdsConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<UnityAdsConfigInfo>(Constants.UnityAds);
                    if (config.adsWeight > 0)
                    {
                        AdWeight.Add(Constants.UnityAds, config.adsWeight);
                        ProcessEcpmInfo(config);
                    }
                }

                if (PluginsInfoManager.Instance.UsePlugin(Constants.AdColony))
                {
                    AdColonyConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<AdColonyConfigInfo>(Constants.AdColony);
                    if (config.adsWeight > 0)
                    {
                        AdWeight.Add(Constants.AdColony, config.adsWeight);
                        ProcessEcpmInfo(config);
                    }
                }

                if (PluginsInfoManager.Instance.UsePlugin(Constants.Audience))
                {
                    AudienceConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<AudienceConfigInfo>(Constants.Audience);
                    if (config.adsWeight > 0)
                    {
                        AdWeight.Add(Constants.Audience, config.adsWeight);
                        ProcessEcpmInfo(config);
                    }
                }

                if (PluginsInfoManager.Instance.UsePlugin(Constants.AppLovin))
                {
                    AppLovinConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<AppLovinConfigInfo>(Constants.AppLovin);
                    if (config.adsWeight > 0)
                    {
                        AdWeight.Add(Constants.AppLovin, config.adsWeight);
                        ProcessEcpmInfo(config);
                    }
                }
            }

            //如果能拿到远程配置数据
            var adIntersitConfigJson = DragonU3DSDK.Utils.ReadFromLocal(AD_INTERSIT_GLOBAL);
            if (!string.IsNullOrEmpty(adIntersitConfigJson))
            {
                try
                {
                    Dictionary<string, int> config = JsonConvert.DeserializeObject<Dictionary<string, int>>(adIntersitConfigJson);

                    if (config.Count > 0)
                    {
                        var list = config.Select((KeyValuePair<string, int> element) => element.Value > 0);
                        if (list.Count > 0)
                        {
                            InterstitialWeight.Clear();
                            foreach (var p in list)
                            {
                                InterstitialWeight.Add(p.Key, p.Value);
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    DebugUtil.LogError(e.ToString());
                    //发生异常，取配置表中配置当广告配置
                    needReinitInter = true;
                }
            }
            else
            {
                needReinitInter = true;
            }

            if (needReinitInter)
            {
                InterstitialWeight.Clear();

                if (PluginsInfoManager.Instance.UsePlugin(Constants.Admob))
                {
                    AdmobConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<AdmobConfigInfo>(Constants.Admob);
                    if (config.interstitialWeight > 0)
                    {
                        InterstitialWeight.Add(Constants.Admob, config.interstitialWeight);
                        ProcessEcpmInfo(config);
                    }
                }

                if (PluginsInfoManager.Instance.UsePlugin(Constants.Chartboost))
                {
                    ChartboostConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<ChartboostConfigInfo>(Constants.Chartboost);
                    if (config.interstitialWeight > 0)
                    {
                        InterstitialWeight.Add(Constants.Chartboost, config.interstitialWeight);
                        ProcessEcpmInfo(config);
                    }
                }

                if (PluginsInfoManager.Instance.UsePlugin(Constants.IronSource))
                {
                    IronSourceConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<IronSourceConfigInfo>(Constants.IronSource);
                    if (config.interstitialWeight > 0)
                    {
                        InterstitialWeight.Add(Constants.IronSource, config.interstitialWeight);
                        ProcessEcpmInfo(config);
                    }
                }

                if (PluginsInfoManager.Instance.UsePlugin(Constants.UnityAds))
                {
                    UnityAdsConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<UnityAdsConfigInfo>(Constants.UnityAds);
                    if (config.interstitialWeight > 0)
                    {
                        InterstitialWeight.Add(Constants.UnityAds, config.interstitialWeight);
                        ProcessEcpmInfo(config);
                    }
                }

                if (PluginsInfoManager.Instance.UsePlugin(Constants.AdColony))
                {
                    AdColonyConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<AdColonyConfigInfo>(Constants.AdColony);
                    if (config.interstitialWeight > 0)
                    {
                        InterstitialWeight.Add(Constants.AdColony, config.interstitialWeight);
                        ProcessEcpmInfo(config);
                    }
                }

                if (PluginsInfoManager.Instance.UsePlugin(Constants.Audience))
                {
                    AudienceConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<AudienceConfigInfo>(Constants.Audience);
                    if (config.interstitialWeight > 0)
                    {
                        InterstitialWeight.Add(Constants.Audience, config.interstitialWeight);
                        ProcessEcpmInfo(config);
                    }
                }

                if (PluginsInfoManager.Instance.UsePlugin(Constants.AppLovin))
                {
                    AppLovinConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<AppLovinConfigInfo>(Constants.AppLovin);
                    if (config.interstitialWeight > 0)
                    {
                        InterstitialWeight.Add(Constants.AppLovin, config.interstitialWeight);
                        ProcessEcpmInfo(config);
                    }
                }
            }
            
            //如果能拿到远程配置数据
            var offerwallGlobalConfigJson = DragonU3DSDK.Utils.ReadFromLocal(AD_OFFERWALL_GLOBAL);
            if (!string.IsNullOrEmpty(offerwallGlobalConfigJson))
            {
                try
                {
                    Dictionary<string, int> config = JsonConvert.DeserializeObject<Dictionary<string, int>>(offerwallGlobalConfigJson);

                    if (config.Count > 0)
                    {
                        var list = config.Select((KeyValuePair<string, int> element) => element.Value > 0);
                        if (list.Count > 0)
                        {
                            OfferwallWeight.Clear();
                            foreach (var p in list)
                            {
                                OfferwallWeight.Add(p.Key, p.Value);
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    DebugUtil.LogError(e.ToString());
                    //发生异常，取配置表中配置当广告配置
                    needReinitOfferwall = true;
                }
            }
            else
            {
                needReinitOfferwall = true;
            }

            if (needReinitOfferwall)
            {
                OfferwallWeight.Clear();

                if (PluginsInfoManager.Instance.UsePlugin(Constants.Tapjoy))
                {
                    TapjoyConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<TapjoyConfigInfo>(Constants.Tapjoy);
                    if (config.adsWeight > 0)
                    {
                        OfferwallWeight.Add(Constants.Tapjoy, config.adsWeight);
                    }
                }
                
                if (PluginsInfoManager.Instance.UsePlugin(Constants.IronSource))
                {
                    IronSourceConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<IronSourceConfigInfo>(Constants.IronSource);
                    if (config.adsWeight > 0)
                    {
                        OfferwallWeight.Add(Constants.IronSource, config.adsWeight);
                    }
                }
            }

            SyncAdConfig();
        }

        private void SyncAdConfig()
        {
            if (AdWeight == null && InterstitialWeight == null && OfferwallWeight == null)
            {
                return;
            }

            if (AdWeight != null)
            {
                string jsonContent = JsonConvert.SerializeObject(AdWeight);

                if (!string.IsNullOrEmpty(jsonContent))
                {
                    DragonU3DSDK.Utils.SaveToLocal(AD_GLOBAL_KEY, jsonContent);
                }
            }

            if (InterstitialWeight != null)
            {
                string jsonContent = JsonConvert.SerializeObject(InterstitialWeight);

                if (!string.IsNullOrEmpty(jsonContent))
                {
                    DragonU3DSDK.Utils.SaveToLocal(AD_INTERSIT_GLOBAL, jsonContent);
                }
            }

            if (OfferwallWeight != null)
            {
                string jsonContent = JsonConvert.SerializeObject(OfferwallWeight);

                if (!string.IsNullOrEmpty(jsonContent))
                {
                    DragonU3DSDK.Utils.SaveToLocal(AD_OFFERWALL_GLOBAL, jsonContent);
                }
            }
        }

        private void ProcessEcpmInfo(AdsPluginInfoBase config)
        {
            if (config == null)
            {
                return;
            }

            KVPair[] placements = null;
#if UNITY_IPHONE || UNITY_IOS
            placements = config.iOSInterstitialPlacementsWithEcpmFloor;
#elif UNITY_ANDROID
            placements = config.AndroidInterstitialPlacementsWithEcpmFloor;
#endif

            if (placements == null || placements.Length == 0)
            {
                return;
            }

            foreach (KVPair kv in placements)
            {
                try
                {
                    AdUnitPlacementEcpmDic[kv.key] = float.Parse(kv.value);
                }
                catch (Exception e)
                {
                    DebugUtil.LogError("placement {0} ecpm value format error. value is {1}. error is {2}", kv.key, kv.value, e.StackTrace);
                }
            }

        }

    }
}
