
using UnityEngine;
using Dlugin.PluginStructs;
using Firebase.Analytics;
using DragonU3DSDK.Asset;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DragonU3DSDK;
namespace Dlugin

{
    public class FirebasePlugin : IDataStatProvider
    {
        static string TAICHI_AC_25_AD_REVENUE_KEY = "taichi_ac_25_ad_revenue_per_day";
        static string TAICHI_AC_25_DATE_KEY = "taichi_ac_25_ad_revenue_date";
        static List<string> TAICHI_AC_25_AD_REVENUE_EVENTS = new List<string>
        {
            "AdLTV_OneDay_Top10Percent",
            "AdLTV_OneDay_Top20Percent",
            "AdLTV_OneDay_Top30Percent",
            "AdLTV_OneDay_Top40Percent",
            "AdLTV_OneDay_Top50Percent",
        };

        static string TAICHI_AC_30_AD_REVENUE_KEY = "taichi_ac_30_ad_revenue";
        static string TAICHI_AC_30_AD_REVENUE_EVENT = "Total_Ads_Revenue_001";
        static float TAICHI_AC_30_AD_REVENUE_THRESHOLD = 0.01f;

        #region DataProvider
        public override void TrackEvent(string eventId, long secondsInUTC, string info)
        {
            if (FirebaseState.Instance.Initialized)
            {
                FirebaseAnalytics.LogEvent(eventId, "param", info);
            }
        }

        public void TrackEvent(string eventName, Dictionary<string, object> param = null)
        {
            if (!FirebaseState.Instance.Initialized)
            {
                return;
            }

            if (param != null && param.Count > 0)
            {
                var parameters = new Parameter[param.Count];
                int i = 0;
                foreach (var kv in param)
                {
                    switch (kv.Value)
                    {
                        case string s:
                            parameters[i++] = new Parameter(kv.Key, s);
                            break;
                        case long l:
                            parameters[i++] = new Parameter(kv.Key, l);
                            break;
                        case double d:
                            parameters[i++] = new Parameter(kv.Key, d);
                            break;
                    }

                }
                FirebaseAnalytics.LogEvent(eventName, parameters);
            }
            else
            {
                FirebaseAnalytics.LogEvent(eventName);
            }
        }

        public void TrackAdRevenue(double revenue, string network = null, string adUnit = null, string placement = null, string adFormat = null)
        {
            trackTaichiAC30AdRevenue(revenue);
            trackTaichiAC25AdRevenue(revenue);
            trackTaichiAC40AdRevenue(revenue, network, adUnit, placement, adFormat);
        }

        void trackTaichiAC40AdRevenue(double revenue, string network, string adUnit, string placement, string adFormat)
        {
            if (FirebaseState.Instance.Initialized)
            {
                var impressionParameters = new[] {
                    new Parameter("ad_platform", "AppLovin"),
                    new Parameter("ad_source", network),
                    new Parameter("ad_unit_name", adUnit),
                    new Parameter("ad_format", adFormat),
                    new Parameter("value", revenue),
                    new Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
                };
                FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
            }
        }

        void trackTaichiAC30AdRevenue(double revenue)
        {
            if (revenue < 0)
                return;

            double lastAdRevenue = PlayerPrefs.GetFloat(TAICHI_AC_30_AD_REVENUE_KEY, 0);
            lastAdRevenue += revenue;
            DebugUtil.Log("taichi 3.0 current value: " + lastAdRevenue.ToString());

            if (lastAdRevenue >= TAICHI_AC_30_AD_REVENUE_THRESHOLD && FirebaseState.Instance.Initialized)
            {
                // sync to firebase
                Parameter[] AdParameters = {
                        new Parameter(FirebaseAnalytics.ParameterCurrency, "USD"),
                        new Parameter(FirebaseAnalytics.ParameterValue, lastAdRevenue),
                };

                FirebaseAnalytics.LogEvent(TAICHI_AC_30_AD_REVENUE_EVENT, AdParameters);
                DebugUtil.Log("taichi 3.0 event fired: " + TAICHI_AC_30_AD_REVENUE_EVENT);

                // sync to adjust
                var adjust = Dlugin.SDK.GetInstance().adjustPlugin;
                if (adjust != null)
                {
                    JObject obj = new JObject();
                    obj["fb_content_type"] = "ad_revenue";
                    obj["fb_content_id"] = "taichi_3.0";
                    obj["_valueToSum"] = lastAdRevenue;
                    obj["fb_currency"] = "USD";
                    adjust.TrackEvent(TAICHI_AC_30_AD_REVENUE_EVENT, 0, obj.ToString());
                }

                // clear
                lastAdRevenue = 0;
            }

            PlayerPrefs.SetFloat(TAICHI_AC_30_AD_REVENUE_KEY, (float)lastAdRevenue);
        }

        void trackTaichiAC25AdRevenue(double revenue)
        {
            if (revenue < 0)
                return;
            double lastAdRevenue = PlayerPrefs.GetFloat(TAICHI_AC_25_AD_REVENUE_KEY, 0);
            string currentDate = System.DateTime.Now.ToString("yyyyMMdd");
            string lastAdRevenueDay = PlayerPrefs.GetString(TAICHI_AC_25_DATE_KEY, "00000000");
            PlayerPrefs.SetString(TAICHI_AC_25_DATE_KEY, currentDate);
            if (lastAdRevenueDay != currentDate)
            {
                lastAdRevenue = 0;
            }

            var taichiConfig = ChangeableConfig.Instance.GetTaichiAC25Config();
            if (taichiConfig != null && FirebaseState.Instance.Initialized)
            {
                double before = lastAdRevenue;
                double after = lastAdRevenue + revenue;

                for (int i = 0; i < taichiConfig.thresholds.Count && i < TAICHI_AC_25_AD_REVENUE_EVENTS.Count; i++)
                {
                    if (before < taichiConfig.thresholds[i] && after >= taichiConfig.thresholds[i])
                    {
                        // sync to firebase
                        Parameter[] AdParameters = {
                            new Parameter(FirebaseAnalytics.ParameterCurrency, "USD"),
                            new Parameter(FirebaseAnalytics.ParameterValue, taichiConfig.thresholds[i]),
                        };
                        // taichi2.5不记录广告收入
                        // taichi2.5又开启广告收入
                        FirebaseAnalytics.LogEvent(TAICHI_AC_25_AD_REVENUE_EVENTS[i], AdParameters);
                        //FirebaseAnalytics.LogEvent(TAICHI_AC_25_AD_REVENUE_EVENTS[i]);
                        DebugUtil.Log("taichi 2.5 event fired: " + TAICHI_AC_25_AD_REVENUE_EVENTS[i]);

                        // sync to adjust
                        var adjust = Dlugin.SDK.GetInstance().adjustPlugin;
                        if (adjust != null)
                        {
                            JObject obj = new JObject();
                            obj["fb_content_type"] = "ad_revenue";
                            obj["fb_content_id"] = "taichi_2.5";
                            obj["_valueToSum"] = taichiConfig.thresholds[i];
                            obj["fb_currency"] = "USD";
                            adjust.TrackEvent(TAICHI_AC_25_AD_REVENUE_EVENTS[i], 0, obj.ToString());
                        }
                    }
                }
            }

            lastAdRevenue += revenue;
            DebugUtil.Log("taichi 2.5 current value: " + lastAdRevenue.ToString());
            PlayerPrefs.SetFloat(TAICHI_AC_25_AD_REVENUE_KEY, (float)lastAdRevenue);
        }

        public override string GetTrackeeID()
        {
            return "firebase";
        }

        #endregion

        public override void Initialize()
        {
            FirebaseState.Instance.Initialized = false;
            GameObject obj = new GameObject();
            GameObject.DontDestroyOnLoad(obj);
            obj.name = "CrashlyticsInitializer";
            obj.AddComponent<CrashlyticsInit>();

        }

        public override void DisposePlugin(string pluginId)
        {
            m_MyDefine = null;
        }

        public override string GetTrackerUrl(string name)
        {
            return "";
        }


        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                m_LastTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = m_LastTimeScale;
            }
        }

        private float m_LastTimeScale = 0;
        private GameObject m_Agent;
        private PluginDefine m_MyDefine = new PluginDefine();
    }
}
