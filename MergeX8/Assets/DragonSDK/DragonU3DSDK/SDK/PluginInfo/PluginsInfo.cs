using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.IO;
using DragonU3DSDK.Asset;
using System.Text;
using DragonU3DSDK;

namespace Dlugin
{
    [Serializable]
    public abstract class PluginsInfoBase
    {
        public string pluginName;
    }

    [Serializable]
    public abstract class AdsPluginInfoBase : PluginsInfoBase
    {
        public int adsWeight;
        public int interstitialWeight;
        public KVPair[] iOSInterstitialPlacementsWithEcpmFloor;
        public KVPair[] AndroidInterstitialPlacementsWithEcpmFloor;
        public KVPair[] iOSRewardedPlacementsWithEcpmFloor;
        public KVPair[] AndroidRewardedPlacementsWithEcpmFloor;
        public KVPair[] iOSOfferWallPlacements;
        public KVPair[] AndroidOfferWallPlacements;
        public KVPair[] iOSBannerPlacementsWithEcpmFloor;
        public KVPair[] AndroidBannerPlacementsWithEcpmFloor;
        public KVPair[] iOSMRECPlacementsWithEcpmFloor;
        public KVPair[] AndroidMRECPlacementsWithEcpmFloor;
    }



    #region PluginInfo Data Structure``
    [Serializable]
    public class AdColonyConfigInfo : AdsPluginInfoBase
    {
        public AdColonyConfigInfo()
        {
            pluginName = Constants.AdColony;
        }
        public string iOSAppID;
        public string iOSZoneID;
        public string AndroidAppID;
        public string AndroidZoneID;
    }

    [Serializable]
    public class AdmobConfigInfo : AdsPluginInfoBase
    {
        public AdmobConfigInfo()
        {
            pluginName = Constants.Admob;
        }
        public string iOSAppID;
        public string AndroidAppID;

    }

    [Serializable]
    public class ChartboostConfigInfo : AdsPluginInfoBase
    {
        public ChartboostConfigInfo()
        {
            pluginName = Constants.Chartboost;
        }
        public string iOSAppID;
        public string iOSAppSecret;
        public string AndroidAppID;
        public string AndroidAppSecret;
        public bool supportRewardVideo;
        public bool supportInterstitial;
    }

    [Serializable]
    public class UnityAdsConfigInfo : AdsPluginInfoBase
    {
        public UnityAdsConfigInfo()
        {
            pluginName = Constants.UnityAds;
        }
        public string iOSAppID;
        public string AndroidAppID;
    }

    [Serializable]
    public class IronSourceConfigInfo : AdsPluginInfoBase
    {
        public IronSourceConfigInfo()
        {
            pluginName = Constants.IronSource;
        }
        public string iOSAppKey;
        public string AndroidAppKey;

        //max-ironsource与ironsource-self之间互斥
        public bool ExclusivehMaxiOS;
        public bool ExclusivehMaxAndroid;
    }

    [Serializable]
    public class AudienceConfigInfo : AdsPluginInfoBase
    {
        public AudienceConfigInfo()
        {
            pluginName = Constants.Audience;
        }
    }

    [Serializable]
    public class AppLovinConfigInfo : AdsPluginInfoBase
    {
        public AppLovinConfigInfo()
        {
            pluginName = Constants.AppLovin;
        }
        public string SDKKey;
    }

    [Serializable]
    public class MAXConfigInfo : AdsPluginInfoBase
    {
        public MAXConfigInfo()
        {
            pluginName = Constants.MAX;
        }
        public string SDKKey;
    }

    [Serializable]
    public class TapjoyConfigInfo : AdsPluginInfoBase
    {
        public TapjoyConfigInfo()
        {
            pluginName = Constants.Tapjoy;
        }

        public string iOSSDKKey; 
        public string AndroidSDKKey;
    }

    [Serializable]
    public class AdjustConfigInfo : PluginsInfoBase
    {
        public AdjustConfigInfo()
        {
            pluginName = Constants.Adjust;
        }
        public string appToken;
        public KVPair[] events;
        public KVPair[] trackers;
        
        public string appTokenAmazon;
        public KVPair[] eventsAmazon;
        public KVPair[] trackersAmazon;

        public string GetAppToken()
        {
#if SUB_CHANNEL_AMAZON
            return appTokenAmazon;
#endif
            return appToken;
        }
        
        public string GetValue(string key)
        {
            KVPair[] _events = events;
#if SUB_CHANNEL_AMAZON
            _events = eventsAmazon;
#endif
            for (int i = 0; i < _events.Length; i++)
            {
                if (string.Equals(_events[i].key, key))
                {
                    return _events[i].value;
                }
            }

            return "";
        }

        public string GetTrackerToken(string name)
        {
            KVPair[] _trackers = trackers;
#if SUB_CHANNEL_AMAZON
            _trackers = trackersAmazon;
#endif
            for (int i = 0; i < _trackers.Length; i++)
            {
                if (string.Equals(_trackers[i].key, name))
                {
                    return _trackers[i].value;
                }
            }

            return "";
        }
    }
    [Serializable]
    public class KVPair
    {
        public string key;
        public string value;
    }

    [Serializable]
    public class OneSignalConfigInfo : PluginsInfoBase
    {
        public OneSignalConfigInfo()
        {
            pluginName = Constants.OneSignal;
        }
        public string appID;
    }

    [Serializable]
    public class FacebookConfigInfo : PluginsInfoBase
    {
        public FacebookConfigInfo()
        {
            pluginName = Constants.FaceBook;
        }

        public string AppID;
        public string ClientToken;

    }

    [Serializable]
    public class FirebaseConfigInfo : PluginsInfoBase
    {
        public FirebaseConfigInfo()
        {
            pluginName = Constants.FireBase;
        }
    }
    
    [Serializable]
    public class APTPluginInfo : PluginsInfoBase
    {
        public APTPluginInfo()
        {
            pluginName = Constants.APT;
        }
        public string APIKey;
    }
    
    [Serializable]
    public class APSConfigInfo : PluginsInfoBase
    {
        public APSConfigInfo()
        {
            pluginName = Constants.APS;
        }

        public string iOSAppId;
        public string iOSInterstitialSlotId;
        public string iOSRewardedSlotId;
        public string iOSBannerSlotId;
        
        public string GoogleAppId;
        public string GoogleInterstitialSlotId;
        public string GoogleRewardedSlotId;
        public string GoogleBannerSlotId;
        
        public string AmazonAppId;
        public string AmazonInterstitialSlotId;
        public string AmazonRewardedSlotId;
        public string AmazonBannerSlotId;

        public string GetAppId()
        {
#if UNITY_IOS
            return iOSAppId;
#elif UNITY_ANDROID
#if SUB_CHANNEL_AMAZON
            return AmazonAppId;
#else
            return GoogleAppId;
#endif
#endif
        }

        public string GetInterstitialSlotId()
        {
#if UNITY_IOS
            return iOSInterstitialSlotId;
#elif UNITY_ANDROID
#if SUB_CHANNEL_AMAZON
            return AmazonInterstitialSlotId;
#else
            return GoogleInterstitialSlotId;
#endif
#endif
        }

        public string GetRewardedSlotId()
        {
#if UNITY_IOS
            return iOSRewardedSlotId;
#elif UNITY_ANDROID
#if SUB_CHANNEL_AMAZON
            return AmazonRewardedSlotId;
#else
            return GoogleRewardedSlotId;
#endif 
#endif
        }

        public string GetBannerSlotId()
        {
#if UNITY_IOS
            return iOSBannerSlotId;
#elif UNITY_ANDROID
#if SUB_CHANNEL_AMAZON
            return AmazonBannerSlotId;
#else
            return GoogleBannerSlotId;
#endif 
#endif
        }
    }

    [Serializable]
    public class PluginConfigInfo
    {
        public Dictionary<string, PluginsInfoBase> m_Map = new Dictionary<string, PluginsInfoBase>();

    }
    #endregion



    public class PluginsInfoManager
    {
        static PluginsInfoManager instance;
        public static PluginsInfoManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PluginsInfoManager();
                }
                return instance;
            }
        }

        PluginConfigInfo m_ConfigInfo;
        public PluginsInfoManager()
        {
            m_ConfigInfo = LoadPluginConfig();
            if (m_ConfigInfo == null)
            {
                DebugUtil.LogWarning("Load Plugin Config Failed!!");
            }
        }

        public static PluginConfigInfo LoadPluginConfig(bool autoDecrypt = true)
        {
            PluginConfigInfo info = new PluginConfigInfo();

            TextAsset textAsset = Resources.Load<TextAsset>("PluginsConfig");
            if (textAsset != null)
            {
                info = new PluginConfigInfo();
                string config = textAsset.text;
                if (autoDecrypt)
                {
                    config = EncryptDecrypt.Decrypt(config);
                }
                JObject configJson = JObject.Parse(config);

                JsonSerializerSettings setting = new JsonSerializerSettings();
                setting.NullValueHandling = NullValueHandling.Ignore;

                JObject map = configJson["m_Map"] as JObject;

                if (map != null)
                {
                    if (map[Constants.Admob] != null)
                    {
                        AdmobConfigInfo configInfo = JsonConvert.DeserializeObject<AdmobConfigInfo>(map[Constants.Admob].ToString());

                        //if (configInfo.AndroidInterstitialPlacements == null) {
                        //    configInfo.AndroidInterstitialPlacements = new string[0];
                        //}
                        //if (configInfo.AndroidInterstitialPlacements.Length == 0)
                        //{
                        //    if (!string.IsNullOrEmpty(configInfo.AndroidInterstitialID))
                        //    {
                        //        configInfo.AndroidInterstitialPlacements = new string[] { configInfo.AndroidInterstitialID };
                        //    }
                        //}

                        //if (configInfo.iOSInterstitialPlacements == null)
                        //{
                        //    configInfo.iOSInterstitialPlacements = new string[0];
                        //}
                        //if (configInfo.iOSInterstitialPlacements.Length == 0)
                        //{
                        //    if (!string.IsNullOrEmpty(configInfo.iOSIntersititialID))
                        //    {
                        //        configInfo.iOSInterstitialPlacements = new string[] { configInfo.iOSIntersititialID };
                        //    }
                        //}

                        info.m_Map.Add(Constants.Admob, configInfo);
                    }

                    if (map[Constants.Adjust] != null)
                    {
                        AdjustConfigInfo configInfo = JsonConvert.DeserializeObject<AdjustConfigInfo>(map[Constants.Adjust].ToString());
                        info.m_Map.Add(Constants.Adjust, configInfo);
                    }

                    if (map[Constants.AdColony] != null)
                    {
                        AdColonyConfigInfo configInfo = JsonConvert.DeserializeObject<AdColonyConfigInfo>(map[Constants.AdColony].ToString());
                        info.m_Map.Add(Constants.AdColony, configInfo);
                    }

                    if (map[Constants.Audience] != null)
                    {
                        AudienceConfigInfo configInfo = JsonConvert.DeserializeObject<AudienceConfigInfo>(map[Constants.Audience].ToString());
                        info.m_Map.Add(Constants.Audience, configInfo);
                    }

                    if (map[Constants.FireBase] != null)
                    {
                        FirebaseConfigInfo configInfo = JsonConvert.DeserializeObject<FirebaseConfigInfo>(map[Constants.FireBase].ToString());
                        info.m_Map.Add(Constants.FireBase, configInfo);
                    }

                    if (map[Constants.FaceBook] != null)
                    {
                        FacebookConfigInfo configInfo = JsonConvert.DeserializeObject<FacebookConfigInfo>(map[Constants.FaceBook].ToString());
                        info.m_Map.Add(Constants.FaceBook, configInfo);
                    }

                    if (map[Constants.Chartboost] != null)
                    {
                        ChartboostConfigInfo configInfo = JsonConvert.DeserializeObject<ChartboostConfigInfo>(map[Constants.Chartboost].ToString());
                        info.m_Map.Add(Constants.Chartboost, configInfo);
                    }

                    if (map[Constants.IronSource] != null)
                    {
                        IronSourceConfigInfo configInfo = JsonConvert.DeserializeObject<IronSourceConfigInfo>(map[Constants.IronSource].ToString());
                        info.m_Map.Add(Constants.IronSource, configInfo);
                    }

                    if (map[Constants.UnityAds] != null)
                    {
                        UnityAdsConfigInfo configInfo = JsonConvert.DeserializeObject<UnityAdsConfigInfo>(map[Constants.UnityAds].ToString());
                        info.m_Map.Add(Constants.UnityAds, configInfo);
                    }

                    if (map[Constants.OneSignal] != null)
                    {
                        OneSignalConfigInfo configInfo = JsonConvert.DeserializeObject<OneSignalConfigInfo>(map[Constants.OneSignal].ToString());
                        info.m_Map.Add(Constants.OneSignal, configInfo);
                    }

                    if (map[Constants.AppLovin] != null)
                    {
                        AppLovinConfigInfo configInfo = JsonConvert.DeserializeObject<AppLovinConfigInfo>(map[Constants.AppLovin].ToString());
                        info.m_Map.Add(Constants.AppLovin, configInfo);
                    }

                    if (map[Constants.MAX] != null)
                    {
                        MAXConfigInfo configInfo = JsonConvert.DeserializeObject<MAXConfigInfo>(map[Constants.MAX].ToString());
                        info.m_Map.Add(Constants.MAX, configInfo);
                    }

                    if (map[Constants.Tapjoy] != null)
                    {
                        TapjoyConfigInfo configInfo = JsonConvert.DeserializeObject<TapjoyConfigInfo>(map[Constants.Tapjoy].ToString());
                        info.m_Map.Add(Constants.Tapjoy, configInfo);
                    }
                    
                    if (map[Constants.APT] != null)
                    {
                        APTPluginInfo configInfo = JsonConvert.DeserializeObject<APTPluginInfo>(map[Constants.APT].ToString());
                        info.m_Map.Add(Constants.APT, configInfo);
                    }
                    
                    if (map[Constants.APS] != null)
                    {
                        APSConfigInfo configInfo = JsonConvert.DeserializeObject<APSConfigInfo>(map[Constants.APS].ToString());
                        info.m_Map.Add(Constants.APS, configInfo);
                    }

                    DebugUtil.Log("Load PluginConfig.txt from Resources Complete!");
                }
            }
            else
            {
                DebugUtil.LogError("the PluginConfig.txt file is not exist!!");
                return null;
            }

            // adjust配置文件单独加载
            //textAsset = Resources.Load<TextAsset>("adjust");
            //if (textAsset != null)
            //{
            //    AdjustConfigInfo configInfo = JsonConvert.DeserializeObject<AdjustConfigInfo>(textAsset.text);
            //    info.m_Map[Constants.Adjust] = configInfo;
            //}

            return info;
        }

        public bool UsePlugin(string pluginName)
        {
            if (m_ConfigInfo.m_Map.ContainsKey(pluginName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public T GetPluginConfig<T>(string pluginName) where T : PluginsInfoBase
        {
            if (m_ConfigInfo.m_Map.ContainsKey(pluginName))
            {
                return m_ConfigInfo.m_Map[pluginName] as T;
            }
            else
            {
                DebugUtil.LogError("Don't contain the plugin config : " + pluginName);
                return null;
            }
        }

        public static void SaveToResourceJson(PluginConfigInfo info)
        {
            string json = JsonConvert.SerializeObject(info);
            json = EncryptDecrypt.Encrypt(json);
            File.WriteAllText(Application.dataPath + "/Resources/PluginsConfig.txt", json);
        }
    }
}

