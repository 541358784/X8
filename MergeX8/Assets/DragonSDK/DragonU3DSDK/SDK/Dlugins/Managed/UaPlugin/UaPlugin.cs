using System.Collections.Generic;
using Dlugin.PluginStructs;
using DragonPlus.Ad.UA;
using DragonU3DSDK;

namespace Dlugin
{
    public class UaPlugin : IAdsProvider
    {
        #region Properties

        private const string InterstitialUnitId  = "test-interstitial-unit-id";
        private const string RewardedVideoUnitId = "test-rewarded-video-unit-id";

        private bool _needLoadInterstitial;
        private int  _leftTickToLoadInterstitial;
        private int  _interstitialRetryCount;

        private bool _needLoadRewardAd;
        private int  _leftTickToLoadRewardedVideo;
        private int  _rewardedViewRetryCount;

        private double _loadedInterstitialAdRevenue;
        private double _loadedRewardedVideoAdRevenue;

        private bool _receiveReward;

        private string _interstitialAdExtraData;
        private string _rewardedAdExtraData;

        private static readonly int[] RetryCounts =
        {
            1, 2, 4, 8, 16, 32
        };

        #endregion

        public override void DisposePlugin(string pluginName)
        {
        }

        public override void Initialize()
        {
            UaSdkCallbacks.OnSdkInitializedEvent += OnSdkInitializedEvent;

            UaSdk.InitializeSdk();

            UaSdkUtility.AddLooseUpdateListener(OnTickUpdate, 1f);

            UaSdkCallbacks.Interstitial.OnAdLoadedEvent        += OnInterstitialLoadedEvent;
            UaSdkCallbacks.Interstitial.OnAdLoadFailedEvent    += OnInterstitialFailedEvent;
            UaSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialFailedToDisplayEvent;
            UaSdkCallbacks.Interstitial.OnAdHiddenEvent        += OnInterstitialDismissedEvent;
            UaSdkCallbacks.Interstitial.OnAdDisplayedEvent     += OnInterstitialDisplayedEvent;
            UaSdkCallbacks.Interstitial.OnAdClickedEvent       += OnInterstitialClickedEvent;

            UaSdkCallbacks.Rewarded.OnAdLoadedEvent         += OnRewardedAdLoadedEvent;
            UaSdkCallbacks.Rewarded.OnAdLoadFailedEvent     += OnRewardedAdLoadFailedEvent;
            UaSdkCallbacks.Rewarded.OnAdDisplayedEvent      += OnRewardedAdDisplayedEvent;
            UaSdkCallbacks.Rewarded.OnAdClickedEvent        += OnRewardedAdClickedEvent;
            UaSdkCallbacks.Rewarded.OnAdHiddenEvent         += OnRewardedAdHiddenEvent;
            UaSdkCallbacks.Rewarded.OnAdDisplayFailedEvent  += OnRewardedAdFailedToDisplayEvent;
            UaSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            m_PluginDefine.m_PluginParam = string.Empty;

            m_PluginDefine.m_PluginName    = Constants.UA;
            m_PluginDefine.m_PluginVersion = "1.0";
        }

        public override bool PlayAds(AdsUnitDefine unit, string placement = null)
        {
#if !UNITY_STANDALONE
            _receiveReward = false;

            switch (unit.m_Type)
            {
                case AD_Type.Interstitial when UaSdk.IsInterstitialReady(InterstitialUnitId):
                    UaSdk.ShowInterstitial(InterstitialUnitId, placement, unit.m_InstanceId);
                    return true;
                case AD_Type.Interstitial:
                    return false;
                case AD_Type.RewardVideo when UaSdk.IsRewardedAdReady(InterstitialUnitId):
                    UaSdk.ShowRewardedAd(InterstitialUnitId, placement, unit.m_InstanceId);
                    return true;
                default:
                    return false;
            }
#else
            return false;
#endif
        }

        public override bool DisposeAd(AdsUnitDefine unit)
        {
            return false;
        }

        public override bool IsAdsReady(AdsUnitDefine unit)
        {
            if (unit == null) return false;

#if !UNITY_STANDALONE
            return unit.m_Type switch
            {
                AD_Type.Interstitial => UaSdk.IsInterstitialReady(InterstitialUnitId, unit.m_InstanceId),
                AD_Type.RewardVideo  => UaSdk.IsRewardedAdReady(RewardedVideoUnitId, unit.m_InstanceId),
                _                    => false
            };

#else
            return false;
#endif
        }

        public override List<AdsUnitDefine> PreloadAds()
        {
            return null;
        }

        public override void SpendCurrency()
        {
        }

        public override void SetMuted(bool muted)
        {
            UaSdk.SetMuted(muted);
        }

        public override bool IsMuted()
        {
            return UaSdk.IsMuted();
        }

        public override void ShowBanner()
        {
        }

        public override void HideBanner()
        {
        }

        public override void ShowMRec()
        {
        }

        public override void HideMRec()
        {
        }

        public override void UpdateBannerPosition(float x, float y)
        {
        }

        public override void UpdateMRECPosition(float x, float y)
        {
        }

        public override void SetBannerWidth(float width)
        {
        }

        public override void SetTestDeviceAdvertisingIdentifiers(string adid)
        {
            UaSdk.SetTestDeviceAdvertisingIdentifiers(new[] { adid });
        }

        public override void HandleLoad(AD_Type type)
        {
            switch (type)
            {
                case AD_Type.Interstitial:
                    LoadInterstitial();
                    break;
                case AD_Type.RewardVideo:
                    LoadRewardedAd();
                    break;
            }
        }

        public string GetAdExtraData(AD_Type type)
        {
            switch (type)
            {
                case AD_Type.Interstitial:
                    return _interstitialAdExtraData;

                case AD_Type.RewardVideo:
                    return _rewardedAdExtraData;
            }

            return string.Empty;
        }

        public override double GetLoadedAdRevenue(AD_Type type)
        {
            return type switch
            {
                AD_Type.Interstitial => _loadedInterstitialAdRevenue,
                AD_Type.RewardVideo  => _loadedRewardedVideoAdRevenue,
                _                    => 0.0
            };
        }

        #region Private Apis

        private static void LoadInterstitial()
        {
            UaSdkUtility.EnqueueMainThreadTask(() => UaSdk.LoadInterstitial(InterstitialUnitId));
        }

        private static void LoadRewardedAd()
        {
            UaSdkUtility.EnqueueMainThreadTask(() => { UaSdk.LoadRewardedAd(RewardedVideoUnitId); });
        }

        private void OnTickUpdate()
        {
            if (_needLoadInterstitial)
            {
                if (_leftTickToLoadInterstitial-- < 0)
                {
                    LoadInterstitial();
                    _needLoadInterstitial = false;
                }
            }

            if (_needLoadRewardAd)
            {
                if (_leftTickToLoadRewardedVideo-- < 0)
                {
                    LoadRewardedAd();
                    _needLoadRewardAd = false;
                }
            }
        }

        private void OnSdkInitializedEvent(UASdkConfiguration configuration)
        {
            DebugUtil.Log("UAPlugin : UaSdk initialized");

            LoadRewardedAd();

            UaInfoReportCallback.Setup();
        }

        private void OnInterstitialLoadedEvent(string adUnitId, UaAdInfo adInfo)
        {
            // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
            DebugUtil.Log("UAPlugin : adUnit {0} is loaded.", adUnitId);

            _interstitialRetryCount      = 0;
            _needLoadInterstitial        = false;
            _loadedInterstitialAdRevenue = adInfo.Revenue;

            var define = ConvertFromInterstitialAd(adInfo);
            SDK.GetInstance().m_AdsDispather.ProcessAdsLoaded(define, null);

            _interstitialAdExtraData = adInfo.ExtraData;
        }

        private void OnInterstitialFailedEvent(string adUnitId, UaErrorInfo errorInfo)
        {
            // Interstitial ad failed to load. We recommend re-trying in 3 seconds.
            DebugUtil.LogWarning("UAPlugin : adUnit {0} fail to load.", adUnitId);

            _leftTickToLoadInterstitial = _interstitialRetryCount >= RetryCounts.Length
                ? RetryCounts[^1]
                : RetryCounts[_interstitialRetryCount];

            ++_interstitialRetryCount;

            _needLoadInterstitial = true;

            var define = ConvertFromInterstitialAd(null);
            SDK.GetInstance().m_AdsDispather.ProcessAdsLoaded(define,
                new SDKError()
                    { err = Constants.kErrorUnknown, channelErrno = 0, errmsg = "ua interstitial load fail" });
        }

        private void OnInterstitialFailedToDisplayEvent(string adUnitId, UaErrorInfo errorInfo,
            UaAdInfo                                           adInfo)
        {
            // Interstitial ad failed to display. We recommend loading the next ad
            DebugUtil.Log("UAPlugin : adUnit {0} failed to display.", adUnitId);

            LoadInterstitial();
        }

        private void OnInterstitialDismissedEvent(string adUnitId, UaAdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad
            DebugUtil.Log("UAPlugin : adUnit {0} is closed.", adUnitId);

            LoadInterstitial();

            var define = ConvertFromInterstitialAd(adInfo);
            SDK.GetInstance().m_AdsDispather.ProcessAdsPlayFinished(define, null);
        }

        private void OnInterstitialDisplayedEvent(string adUnitId, UaAdInfo adInfo)
        {
            DebugUtil.Log("UAPlugin : adUnit {0} displayed.", adUnitId);

            SDK.GetInstance().loginService.LogAdEvent(AD_Type.Interstitial, AD_Event_Type.Impression);
        }

        private void OnInterstitialClickedEvent(string adUnitId, UaAdInfo adInfo)
        {
            DebugUtil.Log("UAPlugin : adUnit {0} clicked.", adUnitId);

            SDK.GetInstance().loginService.LogAdEvent(AD_Type.Interstitial, AD_Event_Type.Click);
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, UaAdInfo adInfo)
        {
            DebugUtil.Log("UAPlugin : adUnit {0} is loaded.", adUnitId);

            _rewardedViewRetryCount       = 0;
            _needLoadRewardAd             = false;
            _loadedRewardedVideoAdRevenue = adInfo.Revenue;

            var define = ConvertFromRewardedAd(adInfo);
            SDK.GetInstance().m_AdsDispather.ProcessAdsLoaded(define, null);

            _rewardedAdExtraData = adInfo.ExtraData;
        }

        private void OnRewardedAdLoadFailedEvent(string adUnitId, UaErrorInfo errorInfo)
        {
            DebugUtil.LogWarning("UAPlugin : adUnit {0} fail to load.", adUnitId);

            _leftTickToLoadRewardedVideo = _rewardedViewRetryCount >= RetryCounts.Length
                ? RetryCounts[^1]
                : RetryCounts[_rewardedViewRetryCount];

            ++_rewardedViewRetryCount;

            _needLoadRewardAd = true;

            var define = ConvertFromRewardedAd(null);
            SDK.GetInstance().m_AdsDispather.ProcessAdsLoaded(define,
                new SDKError() { err = Constants.kErrorUnknown, channelErrno = 0, errmsg = "ua rewarded load fail" });
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, UaAdInfo adInfo)
        {
            DebugUtil.Log("UAPlugin : adUnit {0} displayed.", adUnitId);

            SDK.GetInstance().loginService.LogAdEvent(AD_Type.RewardVideo, AD_Event_Type.Impression);
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, UaErrorInfo errorInfo, UaAdInfo adInfo)
        {
            LoadRewardedAd();
        }

        private void OnRewardedAdClickedEvent(string adUnitId, UaAdInfo adInfo)
        {
            DebugUtil.Log("UAPlugin : adUnit {0} clicked.", adUnitId);

            SDK.GetInstance().loginService.LogAdEvent(AD_Type.RewardVideo, AD_Event_Type.Click);
        }

        private void OnRewardedAdHiddenEvent(string adUnitId, UaAdInfo adInfo)
        {
            DebugUtil.Log("UAPlugin : adUnit {0} dismissed.", adUnitId);

            var adsUnit = ConvertFromRewardedAd(adInfo);
            adsUnit.m_Need_Reward = _receiveReward;

            SDK.GetInstance().m_AdsDispather.ProcessAdsPlayFinished(adsUnit, null);
            SDK.GetInstance().m_AdsDispather.ProcessAdsWatched(adsUnit);

            _receiveReward = false;

            // Rewarded ad is hidden. Pre-load the next ad
            LoadRewardedAd();
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, UaReward reward, UaAdInfo adInfo)
        {
            _receiveReward = true;
            DebugUtil.Log("UAPlugin : adUnit {0} received reward.", adUnitId);
        }

        private AdsUnitDefine ConvertFromInterstitialAd(UaAdInfo adInfo)
        {
            return Convert(adInfo, AD_Type.Interstitial);
        }

        private AdsUnitDefine ConvertFromRewardedAd(UaAdInfo adInfo)
        {
            return Convert(adInfo, AD_Type.RewardVideo);
        }

        private AdsUnitDefine Convert(UaAdInfo adInfo, AD_Type adType)
        {
            var define = new AdsUnitDefine
            {
                m_Type               = adType,
                m_PluginParam        = m_PluginDefine.m_PluginParam,
                m_PluginName         = m_PluginDefine.m_PluginName,
                m_NetworkName        = "UA",
                m_Weight             = m_InterstitialWeight,
                m_EcpmFloor          = adInfo?.Revenue            ?? 0.0,
                m_Placement          = adInfo?.Placement          ?? string.Empty,
                m_AdUnitIdentifier   = adInfo?.AdUnitIdentifier   ?? string.Empty,
                m_NetworkPlacement   = adInfo?.Placement          ?? string.Empty,
                m_CreativeIdentifier = adInfo?.CreativeIdentifier ?? string.Empty,
                m_RevenuePrecision   = adInfo?.RevenuePrecision   ?? string.Empty,
                m_DspName            = string.Empty,
                m_InstanceId         = adInfo?.InstanceId ?? string.Empty
            };
            return define;
        }

        #endregion
    }
}