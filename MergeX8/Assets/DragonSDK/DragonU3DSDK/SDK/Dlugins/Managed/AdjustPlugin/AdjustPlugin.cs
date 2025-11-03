using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dlugin.PluginStructs;
using com.adjust.sdk;
using System;
using DragonU3DSDK;

namespace Dlugin

{
    public class AdjustPlugin : IDataStatProvider
    {
        #region DataProvider
        string inviteCode = null;
        AdjustAttribution attributionData = null;


        public string GetTrackerToken(string trackerName)
        {
            string eventToken = m_ConfigInfo.GetValue(trackerName);
            if (!string.IsNullOrEmpty(eventToken))
            {
                return eventToken;
            }

            var config = DragonU3DSDK.Network.BI.BIManager.Instance.GetThirdPartyTrackingConfig(trackerName);
            if (config != null && !string.IsNullOrEmpty(config.adjustEventToken))
            {
                return config.adjustEventToken;

            }

            return null;
        }

        public override void TrackEvent(string eventName, long secondsInUTC, string info)
        {
            string eventToken = GetTrackerToken(eventName);
            if (string.IsNullOrEmpty(eventToken))
            {
                DebugUtil.LogWarning("config not contain the event : " + eventName);
                return;
            }

            AdjustEvent adjustEvent = new AdjustEvent(eventToken);
            try
            {
                JObject jObj = JObject.Parse(info);
                foreach (var p in jObj)
                {
                    adjustEvent.addPartnerParameter(p.Key, p.Value.ToString());
                }

                if (eventName == "purchase")
                {
                    var price = double.Parse(jObj["price"].ToString());
                    var currency = jObj["currency"].ToString();
                    var transactionId = jObj["transactionId"].ToString();
#if DEBUG
                    adjustEvent.setRevenue(price, currency);
                    adjustEvent.setTransactionId(transactionId);
#else
                    var isTest = bool.Parse(jObj["isTest"].ToString());
                    if (!isTest)
                    {
                        adjustEvent.setRevenue(price, currency);
                        adjustEvent.setTransactionId(transactionId);
                    }
#endif
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogWarning("adjust send event invalid json");
                DebugUtil.Log("Error is " + e.ToString());
                return;
            }
            Adjust.trackEvent(adjustEvent);
        }

        public AdjustAttribution GetAdjustAttribution()
        {
            return attributionData;
        }

        public void TrackAdRevenue(double revenue, string network = null, string adUnit = null, string placement = null)
        {
            if (revenue < 0)
                return;

            // initialise with AppLovin MAX source
            AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
            // set revenue and currency
            adjustAdRevenue.setRevenue(revenue, "USD");
            // optional parameters
            adjustAdRevenue.setAdImpressionsCount(1);

            if (!string.IsNullOrEmpty(network))
                adjustAdRevenue.setAdRevenueNetwork(network);
            if (!string.IsNullOrEmpty(adUnit))
                adjustAdRevenue.setAdRevenueUnit(adUnit);
            if (!string.IsNullOrEmpty(placement))
                adjustAdRevenue.setAdRevenuePlacement(placement);

            // track ad revenue
            Adjust.trackAdRevenue(adjustAdRevenue);
        }

        public void TrackEventWithToken(string eventToken)
        {
            AdjustEvent adjustEvent = new AdjustEvent(eventToken);
            Adjust.trackEvent(adjustEvent);
        }

        public void SetPartnerParameter(string key, string value)
        {
            Adjust.addSessionPartnerParameter(key, value);
        }

        public void SetSessionParameter(string key, string value)
        {
            Adjust.addSessionCallbackParameter(key, value);
        }

        public override string GetTrackeeID()
        {
            return Adjust.getAdid();
        }

        public string GetIdfa()
        {
            return Adjust.getIdfa();
        }

        public string GetInviteCode()
        {
            return inviteCode;
        }
        public void SetPushToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                Adjust.setDeviceToken(token);
            }

        }
        #endregion

        public override void Initialize()
        {
            m_ConfigInfo = PluginsInfoManager.Instance.GetPluginConfig<AdjustConfigInfo>(Constants.Adjust);
            m_MyDefine = new PluginDefine();
            m_MyDefine.m_PluginParam = m_ConfigInfo.GetAppToken();
            m_MyDefine.m_PluginName = Constants.Adjust;
            m_MyDefine.m_PluginVersion = "1.0";

            m_Agent = GameObject.Instantiate(Resources.Load("Adjust")) as GameObject;
            m_Agent.name = "Adjust";
#if DEBUG
            AdjustConfig adjustConfig = new AdjustConfig(m_ConfigInfo.GetAppToken(), AdjustEnvironment.Sandbox);
#else
            AdjustConfig adjustConfig = new AdjustConfig(m_ConfigInfo.GetAppToken(), AdjustEnvironment.Production);
#endif
            adjustConfig.setLogLevel(AdjustLogLevel.Verbose);
            AdjustLogLevel logLevel = AdjustLogLevel.Info;
            adjustConfig.setLogLevel(logLevel);
            adjustConfig.setSendInBackground(false);
            adjustConfig.setEventBufferingEnabled(true);
            adjustConfig.setLaunchDeferredDeeplink(true);
            adjustConfig.setLogDelegate(msg => DebugUtil.Log(msg));
            adjustConfig.setEventSuccessDelegate(EventSuccessCallback);
            adjustConfig.setEventFailureDelegate(EventFailureCallback);
            adjustConfig.setSessionSuccessDelegate(SessionSuccessCallback);
            adjustConfig.setSessionFailureDelegate(SessionFailureCallback);
            adjustConfig.setDeferredDeeplinkDelegate(DeferredDeeplinkCallback);
            adjustConfig.setAttributionChangedDelegate(AttributionChangedCallback);
            adjustConfig.setDelayStart(10);

            Adjust.start(adjustConfig);

            DebugUtil.Log("AdjustPlugin.Initialize ----> the adjust appId is {0}", m_MyDefine.m_PluginParam);
        }

        public override string GetTrackerUrl(string name)
        {
            return "https://app.adjust.com/" + GetTrackerToken(name);
        }

        public override void DisposePlugin(string pluginId)
        {
            m_MyDefine = null;
        }

        #region callbacks
        public void EventSuccessCallback(AdjustEventSuccess eventSuccessData)
        {
            DebugUtil.Log("Event tracked successfully!");

            if (eventSuccessData.Message != null)
            {
                DebugUtil.Log("Message: {0}", eventSuccessData.Message);
            }
            if (eventSuccessData.Timestamp != null)
            {
                DebugUtil.Log("Timestamp: {0}", eventSuccessData.Timestamp);
            }
            if (eventSuccessData.Adid != null)
            {
                DebugUtil.Log("Adid: {0}", eventSuccessData.Adid);
            }
            if (eventSuccessData.EventToken != null)
            {
                DebugUtil.Log("EventToken: {0}", eventSuccessData.EventToken);
            }
            if (eventSuccessData.CallbackId != null)
            {
                DebugUtil.Log("CallbackId: {0}", eventSuccessData.CallbackId);
            }
            if (eventSuccessData.JsonResponse != null)
            {
                DebugUtil.Log("JsonResponse: {0}", eventSuccessData.GetJsonResponse());
            }
        }

        public void EventFailureCallback(AdjustEventFailure eventFailureData)
        {
            DebugUtil.Log("Event tracking failed!");

            if (eventFailureData.Message != null)
            {
                DebugUtil.Log("Message: {0}", eventFailureData.Message);
            }
            if (eventFailureData.Timestamp != null)
            {
                DebugUtil.Log("Timestamp: {0}", eventFailureData.Timestamp);
            }
            if (eventFailureData.Adid != null)
            {
                DebugUtil.Log("Adid: {0}", eventFailureData.Adid);
            }
            if (eventFailureData.EventToken != null)
            {
                DebugUtil.Log("EventToken: {0}", eventFailureData.EventToken);
            }
            if (eventFailureData.CallbackId != null)
            {
                DebugUtil.Log("CallbackId: {0}", eventFailureData.CallbackId);
            }
            if (eventFailureData.JsonResponse != null)
            {
                DebugUtil.Log("JsonResponse: {0}", eventFailureData.GetJsonResponse());
            }

            DebugUtil.Log("WillRetry: {0}", eventFailureData.WillRetry.ToString());
        }

        public void SessionSuccessCallback(AdjustSessionSuccess sessionSuccessData)
        {
            DebugUtil.Log("Session tracked successfully!");

            if (sessionSuccessData.Message != null)
            {
                DebugUtil.Log("Message: {0}", sessionSuccessData.Message);
            }
            if (sessionSuccessData.Timestamp != null)
            {
                DebugUtil.Log("Timestamp: {0}", sessionSuccessData.Timestamp);
            }
            if (sessionSuccessData.Adid != null)
            {
                DebugUtil.Log("Adid: {0}", sessionSuccessData.Adid);
            }
            if (sessionSuccessData.JsonResponse != null)
            {
                DebugUtil.Log("JsonResponse: {0}", sessionSuccessData.GetJsonResponse());
            }
        }

        public void SessionFailureCallback(AdjustSessionFailure sessionFailureData)
        {
            DebugUtil.Log("Session tracking failed!");

            if (sessionFailureData.Message != null)
            {
                DebugUtil.Log("Message: {0}", sessionFailureData.Message);
            }
            if (sessionFailureData.Timestamp != null)
            {
                DebugUtil.Log("Timestamp: {0}", sessionFailureData.Timestamp);
            }
            if (sessionFailureData.Adid != null)
            {
                DebugUtil.Log("Adid: {0}", sessionFailureData.Adid);
            }
            if (sessionFailureData.JsonResponse != null)
            {
                DebugUtil.Log("JsonResponse: {0}", sessionFailureData.GetJsonResponse());
            }

            DebugUtil.Log("WillRetry: {0}", sessionFailureData.WillRetry.ToString());
        }

        private void DeferredDeeplinkCallback(string deeplinkURL)
        {
            DebugUtil.Log("Deferred deeplink reported!");

            if (deeplinkURL != null)
            {
                DebugUtil.Log("Deeplink URL: {0}", deeplinkURL);
            }
            else
            {
                DebugUtil.Log("Deeplink URL is null!");
            }
        }

        public void AttributionChangedCallback(AdjustAttribution attributionData)
        {
            DebugUtil.Log("Attribution changed!");
            this.attributionData = attributionData;

            if (attributionData.trackerName != null)
            {
                DebugUtil.Log("Tracker name: {0}", attributionData.trackerName);
            }
            if (attributionData.trackerToken != null)
            {
                DebugUtil.Log("Tracker token: {0}", attributionData.trackerToken);
            }
            if (attributionData.network != null)
            {
                DebugUtil.Log("Network: {0}", attributionData.network);
            }
            if (attributionData.campaign != null)
            {
                DebugUtil.Log("Campaign: {0}", attributionData.campaign);
            }
            if (attributionData.adgroup != null)
            {
                DebugUtil.Log("Adgroup: {0}", attributionData.adgroup);
            }
            if (attributionData.creative != null)
            {
                DebugUtil.Log("Creative: {0}", attributionData.creative);
            }
            if (attributionData.clickLabel != null)
            {
                DebugUtil.Log("Click label: {0}", attributionData.clickLabel);
            }
            if (attributionData.adid != null)
            {
                DebugUtil.Log("ADID: {0}", attributionData.adid);
            }

            if (attributionData.trackerName == "Invite" && !string.IsNullOrEmpty(attributionData.clickLabel))
            {
                inviteCode = attributionData.clickLabel;
            }

            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.AdjustIdUpdated, attributionData.adid);
        }
        #endregion


        private void InitCallback()
        {

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
        private AdjustConfigInfo m_ConfigInfo;
    }
}

