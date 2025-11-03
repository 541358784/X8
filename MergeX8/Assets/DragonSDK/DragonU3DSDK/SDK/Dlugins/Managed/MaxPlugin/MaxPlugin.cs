using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using AmazonAds;
using Dlugin;
using Dlugin.PluginStructs;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using UnityEngine;

// ReSharper disable AccessToStaticMemberViaDerivedType
public class MaxPlugin : IAdsProvider
{
    #region Properties

    private const string RewardedVideoRevenueKey = "ADS_REWARDED_VIDEO_REVENUE";
    private const string InterstitialRevenueKey  = "ADS_INTERSTITIAL_REVENUE";

    private bool _userIdSet;

    private double _rewardedVideoRevenue;
    private double _interstitialRevenue;
    private double _bannerRevenue;
    private double _mrecRevenue;

    private bool   _receiveReward;
    private string _rewardedAdUnitId;
    private string _interstitialAdUnitId;
    private string _bannerAdUnitId;
    private string _mrecAdUnitId;

    private MAXConfigInfo _configInfo;

    private string        _apsAppId;
    private string        _apsInterstitialSlotId;
    private string        _apsRewardedSlotId;
    private string        _apsBannerSlotId;
    private APSConfigInfo _apsConfigInfo;

    private bool _apsIsFirstInterstitialRequest;
    private bool _apsIsFirstRewardedRequest;
    private bool _apsUseBanner;

    private bool _bannerAdReady;
    private bool _mrecAdReady;

    private readonly Dictionary<string, string> _adInstanceIds = new();

    #endregion

    #region Public Apis

    public string GetAdInstanceId(string adUnitIdentifier)
    {
        return _adInstanceIds.GetValueOrDefault(adUnitIdentifier, string.Empty);
    }

    public void NewAdInstanceId(string adUnitIdentifier)
    {
        _adInstanceIds[adUnitIdentifier] = Guid.NewGuid().ToString();
    }

    public override bool DisposeAd(AdsUnitDefine unit)
    {
        switch (unit.m_Type)
        {
            case AD_Type.Interstitial:
                DisposeInterstitial();
                break;

            case AD_Type.RewardVideo:
                DisposeRewardedAd();
                break;
        }

        return true;
    }

    public override void DisposePlugin(string pluginName)
    {
    }

    public override void Initialize()
    {
        DebugUtil.Log("MaxPlugin : Try to initialize MAX SDK");

        // 初始化事件监听需在正式初始化前
        MaxSdkCallbacks.OnSdkInitializedEvent += _ =>
        {
            DebugUtil.Log("MaxPlugin : MAX SDK init successfully");

#if (DEVELOPMENT_BUILD || DEBUG) && !UNITY_STANDALONE && !MAX_DEBUGGER_DISABLE
            MaxSdk.ShowMediationDebugger();
            MaxSdk.SetVerboseLogging(true);
#else
            MaxSdk.SetVerboseLogging(false);
#endif

            InitializeRewardedAds();
            InitializeInterstitialAds();
            InitializeBannerAds();
            InitializeMrecAds();

            TimerManager.Instance.AddDelegate(Update);

            MaxInfoReportCallback.Setup(this);
        };

        _configInfo = PluginsInfoManager.Instance.GetPluginConfig<MAXConfigInfo>(Constants.MAX);
        if (_configInfo == null)
        {
            DebugUtil.LogError("MaxPlugin : MAX config is null. please check SDKEditor");
            return;
        }

        m_PluginDefine.m_PluginParam = _configInfo.SDKKey;

        m_PluginDefine.m_PluginName    = Constants.MAX;
        m_PluginDefine.m_PluginVersion = "1.0";

        DebugUtil.Log("MaxPlugin : Setup ad units.");

        SetupAdUnitIds();

#if !UNITY_STANDALONE

#if !UNITY_EDITOR
        InitializeAps();
#endif

#if DEVELOPMENT_BUILD || DEBUG
        //MaxSdk.SetTestDeviceAdvertisingIdentifiers(new string[] { SystemInfomation.ADID });
#endif

        DebugUtil.Log("MaxPlugin : Initialize MAX SDK");

        MaxSdk.SetSdkKey(_configInfo.SDKKey);
        MaxSdk.InitializeSdk();

#endif
    }

    public override bool IsAdsReady(AdsUnitDefine unit)
    {
        if (unit == null) return false;

#if !UNITY_STANDALONE
        return unit.m_Type switch
        {
            AD_Type.Interstitial => MaxSdk.IsInterstitialReady(_interstitialAdUnitId),
            AD_Type.RewardVideo  => MaxSdk.IsRewardedAdReady(_rewardedAdUnitId),
            AD_Type.Banner       => _bannerAdReady,
            AD_Type.Mrec         => _mrecAdReady,
            _                    => false
        };
#endif
    }

    public override void SetTestDeviceAdvertisingIdentifiers(string adid)
    {
        if (string.IsNullOrEmpty(adid))
        {
            return;
        }

        MaxSdk.SetTestDeviceAdvertisingIdentifiers(new[] { adid });
    }

    public override bool PlayAds(AdsUnitDefine unit, string placement = null)
    {
#if !UNITY_STANDALONE
        _receiveReward = false;

        switch (unit.m_Type)
        {
            case AD_Type.Interstitial:
            {
                if (!MaxSdk.IsInterstitialReady(_interstitialAdUnitId))
                {
                    DebugUtil.Log("MaxPlugin : ads inter is not ready when playing, maybe expired. reload it.");

                    LoadInterstitial();
                    return false;
                }

                MaxSdk.ShowInterstitial(_interstitialAdUnitId, placement);
                return true;
            }
            case AD_Type.RewardVideo:
            {
                if (!MaxSdk.IsRewardedAdReady(_rewardedAdUnitId))
                {
                    DebugUtil.Log("MaxPlugin : ads rewarded is not ready when playing, maybe expired. reload it.");

                    LoadRewardedAd();
                    return false;
                }

                MaxSdk.ShowRewardedAd(_rewardedAdUnitId, placement);
                return true;
            }
        }

#endif

        return false;
    }

    public override List<AdsUnitDefine> PreloadAds()
    {
        return null;
    }

    public override void SpendCurrency()
    {
    }


    public override void ShowBanner()
    {
        MaxSdk.ShowBanner(_bannerAdUnitId);
    }

    public override void HideBanner()
    {
        MaxSdk.HideBanner(_bannerAdUnitId);
    }


    public override void ShowMRec()
    {
        MaxSdk.ShowMRec(_mrecAdUnitId);
    }

    public override void HideMRec()
    {
        MaxSdk.HideMRec(_mrecAdUnitId);
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

    public override double GetLoadedAdRevenue(AD_Type type)
    {
        return type switch
        {
            AD_Type.Interstitial => _interstitialRevenue,
            AD_Type.RewardVideo  => _rewardedVideoRevenue,
            AD_Type.Banner       => _bannerRevenue,
            AD_Type.Mrec         => _mrecRevenue,
            _                    => 0.0
        };
    }

    public override void SetMuted(bool muted)
    {
        MaxSdk.SetMuted(muted);
    }

    public override bool IsMuted()
    {
        return MaxSdk.IsMuted();
    }

    public override void UpdateBannerPosition(float x, float y)
    {
        MaxSdk.UpdateBannerPosition(_bannerAdUnitId, x, y);
    }

    public override void UpdateMRECPosition(float x, float y)
    {
        MaxSdk.UpdateMRecPosition(_mrecAdUnitId, x, y);
    }

    public override void SetBannerWidth(float width)
    {
        MaxSdk.SetBannerWidth(_bannerAdUnitId, width);
    }

    #endregion

    #region Private Apis

    private void Update(float delta)
    {
        if (_userIdSet || !StorageManager.Instance.Inited) return;

        var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
        var playerId      = storageCommon.PlayerId;
        if (playerId <= 0) return;

        MaxSdk.SetUserId(playerId.ToString());
        _userIdSet = true;
    }

    #region Interstitial Ad

    private void InitializeInterstitialAds()
    {
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent        += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent    += OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent        += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent     += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent       += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent   += OnAdRevenuePaidEvent;
    }

    private void LoadInterstitial()
    {
#if !UNITY_STANDALONE
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : plan to load inter adUnit {0}.", _interstitialAdUnitId);
#endif
        DelayActionManager.Instance.DebounceInMainThread(_interstitialAdUnitId, 1, () =>
        {
#if !MAX_DEBUGGER_DISABLE
            DebugUtil.Log("MaxPlugin : start to load inter adUnit {0}.", _interstitialAdUnitId);
#endif
            NewAdInstanceId(_interstitialAdUnitId);
            
            if (_apsIsFirstInterstitialRequest)
            {
                _apsIsFirstInterstitialRequest = false;

                var interstitialAdRequest = new APSInterstitialAdRequest(_apsInterstitialSlotId);
                interstitialAdRequest.onSuccess += (adResponse) =>
                {
                    DebugUtil.Log("MaxPlugin : APS APSInterstitialAdRequest success.");

                    MaxSdk.SetInterstitialLocalExtraParameter(_interstitialAdUnitId, "amazon_ad_response",
                        adResponse.GetResponse());
                    MaxSdk.LoadInterstitial(_interstitialAdUnitId);
                };
                interstitialAdRequest.onFailedWithError += (adError) =>
                {
                    DebugUtil.Log($"MaxPlugin : APS APSInterstitialAdRequest error : {adError.GetAdError()}.");

                    MaxSdk.SetInterstitialLocalExtraParameter(_interstitialAdUnitId, "amazon_ad_error",
                        adError.GetAdError());
                    MaxSdk.LoadInterstitial(_interstitialAdUnitId);
                };

                interstitialAdRequest.LoadAd();
            }
            else
            {
                MaxSdk.LoadInterstitial(_interstitialAdUnitId);
            }
        });
#endif
    }

    private void DisposeInterstitial()
    {
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : dispose inter adUnit {0}.", _interstitialAdUnitId);
#endif

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        _MaxDisposeInterstitialAd(_interstitialAdUnitId);
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static void _MaxDisposeInterstitialAd(string adUnitId)
    {
        try
        {
            var bridge = new AndroidJavaClass("com.dragonplus.MaxInfoBridge");
            bridge.CallStatic("disposeInterstitialAd", adUnitId);
        }
        catch (Exception exception)
        {
            DebugUtil.LogError($"[MaxInfoReport] disposeInterstitialAd failed: {exception.Message}");
        } 
    }
#endif

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern IntPtr _MaxDisposeInterstitialAd(string adUnitId);
#endif

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : adUnit {0} is loaded.", adUnitId);
#endif

        var define = ConvertFromInterstitialAd(adInfo);
        SDK.GetInstance().m_AdsDispather.ProcessAdsLoaded(define, null);

        try
        {
            if (adInfo != null)
            {
                _interstitialRevenue = adInfo.Revenue;

                PlayerPrefs.SetString(InterstitialRevenueKey, adInfo.Revenue.ToString(CultureInfo.InvariantCulture));
            }
        }
        catch (Exception exception)
        {
            DebugUtil.LogError(exception.Message);
        }
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.LogWarning("MaxPlugin : adUnit {0} fail to load.", adUnitId);
#endif

        var define = ConvertFromInterstitialAd(null);
        SDK.GetInstance().m_AdsDispather.ProcessAdsLoaded(define,
            new SDKError
            {
                err          = Constants.kErrorUnknown,
                channelErrno = 0,
                errmsg       = $"max interstitial load fail: {errorInfo.Message}"
            });

        LoadInterstitial();
    }

    private void OnInterstitialFailedToDisplayEvent(
        string               adUnitId,
        MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo    adInfo)
    {
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : adUnit {0} failed to display.", adUnitId);
#endif

        LoadInterstitial();
    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : adUnit {0} is closed.", adUnitId);
#endif

        LoadInterstitial();

        var define = ConvertFromInterstitialAd(adInfo);
        SDK.GetInstance().m_AdsDispather.ProcessAdsPlayFinished(define, null);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : adUnit {0} displayed.", adUnitId);
#endif

        SDK.GetInstance().loginService.LogAdEvent(AD_Type.Interstitial, AD_Event_Type.Impression);
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : adUnit {0} clicked.", adUnitId);
#endif

        SDK.GetInstance().loginService.LogAdEvent(AD_Type.Interstitial, AD_Event_Type.Click);
    }

    #endregion

    #region RewardedVideo Ad

    private void InitializeRewardedAds()
    {
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent         += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent     += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent  += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent      += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent        += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent         += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent    += OnAdRevenuePaidEvent;

        // Load the first RewardedAd
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
#if !UNITY_STANDALONE
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : plan to load rewarded adUnit {0}.", _rewardedAdUnitId);
#endif
        DelayActionManager.Instance.DebounceInMainThread(_rewardedAdUnitId, 1, () =>
        {
#if !MAX_DEBUGGER_DISABLE
            DebugUtil.Log("MaxPlugin : plan to load rewarded adUnit {0}.", _rewardedAdUnitId);
#endif
            NewAdInstanceId(_rewardedAdUnitId);
            
            if (_apsIsFirstRewardedRequest)
            {
                _apsIsFirstRewardedRequest = false;

                var portrait = Screen.width < Screen.height;

                DebugUtil.Log($"MaxPlugin : APS portrait : {portrait}");

                var rewardedVideoAd = new APSVideoAdRequest(
                    portrait ? 320 : 480,
                    portrait ? 480 : 320,
                    _apsRewardedSlotId);

                rewardedVideoAd.onSuccess += (adResponse) =>
                {
                    DebugUtil.Log("MaxPlugin : APS APSVideoAdRequest success.");

                    MaxSdk.SetRewardedAdLocalExtraParameter(_rewardedAdUnitId, "amazon_ad_response",
                        adResponse.GetResponse());
                    MaxSdk.LoadRewardedAd(_rewardedAdUnitId);
                };

                rewardedVideoAd.onFailedWithError += (adError) =>
                {
                    DebugUtil.Log($"MaxPlugin : APS APSVideoAdRequest error : {adError.GetAdError()}.");

                    MaxSdk.SetRewardedAdLocalExtraParameter(_rewardedAdUnitId, "amazon_ad_error", adError.GetAdError());
                    MaxSdk.LoadRewardedAd(_rewardedAdUnitId);
                };

                rewardedVideoAd.LoadAd();
            }
            else
            {
                MaxSdk.LoadRewardedAd(_rewardedAdUnitId);
            }
        });
#endif
    }

    private void DisposeRewardedAd()
    {
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : dispose rewarded adUnit {0}.", _rewardedAdUnitId);
#endif

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        _MaxDisposeRewardedVideoAd(_rewardedAdUnitId);
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static void _MaxDisposeRewardedVideoAd(string adUnitId)
    {
        try
        {
            var bridge = new AndroidJavaClass("com.dragonplus.MaxInfoBridge");
            bridge.CallStatic("disposeRewardedVideoAd", adUnitId);
        }
        catch (Exception exception)
        {
            DebugUtil.LogError($"[MaxInfoReport] disposeRewardedVideoAd failed: {exception.Message}");
        } 
    }
#endif

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern IntPtr _MaxDisposeRewardedVideoAd(string adUnitId);
#endif

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : adUnit {0} is loaded.", adUnitId);
#endif

        var define = ConvertFromRewardedAd(adInfo);
        SDK.GetInstance().m_AdsDispather.ProcessAdsLoaded(define, null);

        try
        {
            if (adInfo != null)
            {
                _rewardedVideoRevenue = adInfo.Revenue;

                PlayerPrefs.SetString(RewardedVideoRevenueKey, adInfo.Revenue.ToString(CultureInfo.InvariantCulture));
            }
        }
        catch (Exception exception)
        {
            DebugUtil.LogError(exception.Message);
        }
    }

    private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : adUnit {0} failed to load.", adUnitId);
#endif

        var define = ConvertFromRewardedAd(null);
        SDK.GetInstance().m_AdsDispather.ProcessAdsLoaded(
            define,
            new SDKError
            {
                err          = Constants.kErrorUnknown,
                channelErrno = 0,
                errmsg       = $"max rewarded load fail: {errorInfo.Message}"
            });

        LoadRewardedAd();
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo                                adInfo)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : adUnit {0} failed to display.", adUnitId);
#endif
        LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : adUnit {0} displayed.", adUnitId);
#endif
        SDK.GetInstance().loginService.LogAdEvent(AD_Type.RewardVideo, AD_Event_Type.Impression);
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : adUnit {0} clicked.", adUnitId);
#endif
        SDK.GetInstance().loginService.LogAdEvent(AD_Type.RewardVideo, AD_Event_Type.Click);
    }

    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : adUnit {0} dismissed.", adUnitId);
#endif
        // Rewarded ad is hidden. Pre-load the next ad
        AdsUnitDefine adsUnit = new AdsUnitDefine();
        adsUnit.m_PluginName  = m_PluginDefine.m_PluginName;
        adsUnit.m_PluginParam = m_PluginDefine.m_PluginParam;
        adsUnit.m_Type        = AD_Type.RewardVideo;
        adsUnit.m_Need_Reward = _receiveReward;

        SDK.GetInstance().m_AdsDispather.ProcessAdsPlayFinished(adsUnit, null);
        SDK.GetInstance().m_AdsDispather.ProcessAdsWatched(adsUnit);

        _receiveReward = false;

        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad was displayed and user should receive the reward
        _receiveReward = true;
#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : adUnit {0} received reward.", adUnitId);
#endif
    }

    #endregion

    #region Banner Ad

    private void InitializeBannerAds()
    {
        // Attach callback
        MaxSdkCallbacks.Banner.OnAdLoadedEvent      += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

        // Load the first banner
        LoadBannerAd();
    }

    private void LoadBannerAd()
    {
#if !UNITY_STANDALONE
        if (string.IsNullOrEmpty(_bannerAdUnitId))
        {
            return;
        }

        DebugUtil.Log("MaxPlugin : plan to load banner adUnit {0}.", _bannerAdUnitId);
        DelayActionManager.Instance.DebounceInMainThread(_bannerAdUnitId, 1, () =>
        {
            DebugUtil.Log("MaxPlugin : plan to load banner adUnit {0}.", _bannerAdUnitId);

            if (_apsUseBanner)
            {
                var apsBanner = new APSBannerAdRequest(320, 50, _apsBannerSlotId);
                apsBanner.onSuccess += (adResponse) =>
                {
                    DebugUtil.Log("MaxPlugin : APS APSBannerAdRequest success.");

                    MaxSdk.SetBannerLocalExtraParameter(
                        _bannerAdUnitId,
                        "amazon_ad_response",
                        adResponse.GetResponse());
                    CreateBanner();
                };
                apsBanner.onFailedWithError += (adError) =>
                {
                    DebugUtil.Log($"MaxPlugin : APS APSBannerAdRequest error : {adError.GetAdError()}.");

                    MaxSdk.SetBannerLocalExtraParameter(
                        _bannerAdUnitId,
                        "amazon_ad_error",
                        adError.GetAdError());
                    CreateBanner();
                };
                apsBanner.LoadAd();
            }
            else
            {
                CreateBanner();
            }
        });
#endif
    }

    private void OnBannerAdLoadedEvent(string arg1, MaxSdkBase.AdInfo adInfo)
    {
        _bannerRevenue = adInfo.Revenue;
        _bannerAdReady = true;
    }

    private void CreateBanner()
    {
        MaxSdk.CreateBanner(_bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

#if DEBUG || DEVELOPMENT_BUILD
        MaxSdk.SetBannerBackgroundColor(_bannerAdUnitId, Color.white);
#else
        MaxSdk.SetBannerBackgroundColor(_bannerAdUnitId, Color.clear);
#endif
        MaxSdk.SetBannerPlacement(_bannerAdUnitId, "MY_BANNER_PLACEMENT");
    }

    #endregion

    #region Mrec Ad

    private void InitializeMrecAds()
    {
        // Attach callback
        // 暂不埋入mrec生命周期
        MaxSdkCallbacks.MRec.OnAdLoadedEvent      += OnMrecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

        // Load the first banner
        LoadMrecAd();
    }

    private void LoadMrecAd()
    {
#if !UNITY_STANDALONE
        if (string.IsNullOrEmpty(_mrecAdUnitId))
        {
            return;
        }

        DebugUtil.Log("MaxPlugin : plan to load mrec adUnit {0}.", _mrecAdUnitId);

        DelayActionManager.Instance.DebounceInMainThread(_mrecAdUnitId, 1, () =>
        {
            DebugUtil.Log("MaxPlugin : plan to load mrec adUnit {0}.", _mrecAdUnitId);

            MaxSdk.CreateMRec(_mrecAdUnitId, MaxSdkBase.AdViewPosition.Centered);
        });
#endif
    }

    private void OnMrecAdLoadedEvent(string arg1, MaxSdkBase.AdInfo adInfo)
    {
        _mrecRevenue = adInfo.Revenue;
        _mrecAdReady = true;
    }

    #endregion

    #region Helpers

    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        DelayActionManager.Instance.DebounceInMainThread(Guid.NewGuid().ToString(), 1,
            () => OnAdRevenuePaidEventHelp(adInfo));
    }

    private static void OnAdRevenuePaidEventHelp(MaxSdkBase.AdInfo adInfo)
    {
        var revenue = adInfo.Revenue;

        // Miscellaneous data
        var networkName      = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        var adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        var placement        = adInfo.Placement; // The placement this ad's postbacks are tied to
        var adFormat         = adInfo.AdFormat;

#if !MAX_DEBUGGER_DISABLE
        DebugUtil.Log("MaxPlugin : adUnit {0} get revenue paid. revenue: {1} , network : {2} , placement: {3}",
            adInfo.AdUnitIdentifier, revenue, networkName, placement);
#endif

        var adjust = SDK.GetInstance().adjustPlugin;
        adjust?.TrackAdRevenue(revenue, networkName, adUnitIdentifier, placement);

        var firebase = SDK.GetInstance().firebasePlugin;
        firebase?.TrackAdRevenue(revenue, networkName, adUnitIdentifier, placement, adFormat);
    }

    private void InitializeAps()
    {
        if (!PluginsInfoManager.Instance.UsePlugin(Constants.APS)) return;

        DebugUtil.Log("MaxPlugin : Initialize aps");

        _apsConfigInfo = PluginsInfoManager.Instance.GetPluginConfig<APSConfigInfo>(Constants.APS);
        if (_apsConfigInfo == null) return;

        _apsAppId              = _apsConfigInfo.GetAppId();
        _apsInterstitialSlotId = _apsConfigInfo.GetInterstitialSlotId();
        _apsRewardedSlotId     = _apsConfigInfo.GetRewardedSlotId();
        _apsBannerSlotId       = _apsConfigInfo.GetBannerSlotId();

        if (!string.IsNullOrEmpty(_apsInterstitialSlotId))
        {
            _apsIsFirstInterstitialRequest = true;
        }

        if (!string.IsNullOrEmpty(_apsRewardedSlotId))
        {
            _apsIsFirstRewardedRequest = true;
        }

        if (!string.IsNullOrEmpty(_apsBannerSlotId))
        {
            _apsUseBanner = true;
        }

        Amazon.Initialize(_apsAppId);
#if (DEVELOPMENT_BUILD || DEBUG) && !UNITY_STANDALONE && !MAX_DEBUGGER_DISABLE
        Amazon.EnableTesting(true);
        Amazon.EnableLogging(true);
        DebugUtil.Log("MAX APS Open TestMode.");
#else
        Amazon.EnableTesting (false);
        Amazon.EnableLogging (false);
#endif
        Amazon.SetAdNetworkInfo(new AdNetworkInfo(DTBAdNetwork.MAX));

#if UNITY_IOS
        Amazon.SetAPSPublisherExtendedIdFeatureEnabled(true);
#endif

        DebugUtil.Log($"MAX APS Initialize TestMode : {Amazon.IsTestMode()}.");

        DebugUtil.Log("MAX Use APS.");
    }

    private void SetupAdUnitIds()
    {
        SetupIosAdUnits();
        SetupAndroidAdUnits();
    }

    [Conditional("UNITY_IOS")]
    private void SetupIosAdUnits()
    {
        DebugUtil.Log("MaxPlugin : Setup ios ad units.");

        _rewardedAdUnitId     = _configInfo.iOSRewardedPlacementsWithEcpmFloor[0].key;
        _interstitialAdUnitId = _configInfo.iOSInterstitialPlacementsWithEcpmFloor[0].key;

        if (_configInfo.iOSBannerPlacementsWithEcpmFloor is { Length: > 0 })
        {
            _bannerAdUnitId = _configInfo.iOSBannerPlacementsWithEcpmFloor[0].key;
        }

        if (_configInfo.iOSMRECPlacementsWithEcpmFloor is { Length: > 0 })
        {
            _mrecAdUnitId = _configInfo.iOSMRECPlacementsWithEcpmFloor[0].key;
        }
    }

    [Conditional("UNITY_ANDROID")]
    private void SetupAndroidAdUnits()
    {
#if !SUB_CHANNEL_AMAZON
        DebugUtil.Log("MaxPlugin : Setup android ad units.");

        _rewardedAdUnitId     = _configInfo.AndroidRewardedPlacementsWithEcpmFloor[0].key;
        _interstitialAdUnitId = _configInfo.AndroidInterstitialPlacementsWithEcpmFloor[0].key;
#endif

        SetupAmazonAdUnits();

        if (_configInfo.AndroidBannerPlacementsWithEcpmFloor is { Length: > 0 })
        {
            _bannerAdUnitId = _configInfo.AndroidBannerPlacementsWithEcpmFloor[0].key;
        }

        if (_configInfo.AndroidMRECPlacementsWithEcpmFloor is { Length: > 0 })
        {
            _mrecAdUnitId = _configInfo.AndroidMRECPlacementsWithEcpmFloor[0].key;
        }
    }

    [Conditional("SUB_CHANNEL_AMAZON")]
    private void SetupAmazonAdUnits()
    {
        DebugUtil.Log("MaxPlugin : Setup amazon ad units.");

        foreach (var p in _configInfo.AndroidRewardedPlacementsWithEcpmFloor)
        {
            if (string.IsNullOrEmpty(p.value) || !p.value.Equals("Amazon")) continue;

            _rewardedAdUnitId = p.key;
            break;
        }

        if (string.IsNullOrEmpty(_rewardedAdUnitId))
        {
            DebugUtil.LogError("MaxPlugin : can not find subChannel Amazon rewardedAdUnitId!");
        }

        foreach (var p in _configInfo.AndroidInterstitialPlacementsWithEcpmFloor)
        {
            if (string.IsNullOrEmpty(p.value) || !p.value.Equals("Amazon")) continue;

            _interstitialAdUnitId = p.key;
            break;
        }

        if (string.IsNullOrEmpty(_interstitialAdUnitId))
        {
            DebugUtil.LogError("MaxPlugin : can not find subChannel Amazon interstitialAdUnitId!");
        }
    }


    private AdsUnitDefine ConvertFromInterstitialAd(MaxSdkBase.AdInfo adInfo)
    {
        return Convert(adInfo, AD_Type.Interstitial);
    }

    private AdsUnitDefine ConvertFromRewardedAd(MaxSdkBase.AdInfo adInfo)
    {
        return Convert(adInfo, AD_Type.RewardVideo);
    }

    private AdsUnitDefine Convert(MaxSdkBase.AdInfo adInfo, AD_Type adType)
    {
        var define = new AdsUnitDefine
        {
            m_Type               = adType,
            m_PluginParam        = m_PluginDefine.m_PluginParam,
            m_PluginName         = m_PluginDefine.m_PluginName,
            m_NetworkName        = adInfo?.NetworkName ?? string.Empty,
            m_Weight             = m_InterstitialWeight,
            m_EcpmFloor          = adInfo?.Revenue            ?? 0.0,
            m_Placement          = adInfo?.Placement          ?? string.Empty,
            m_AdUnitIdentifier   = adInfo?.AdUnitIdentifier   ?? string.Empty,
            m_NetworkPlacement   = adInfo?.NetworkPlacement   ?? string.Empty,
            m_CreativeIdentifier = adInfo?.CreativeIdentifier ?? string.Empty,
            m_RevenuePrecision   = adInfo?.RevenuePrecision   ?? string.Empty,
            m_DspName            = adInfo?.DspName            ?? string.Empty,
            m_InstanceId         = GetAdInstanceId(adInfo?.AdUnitIdentifier ?? string.Empty)
        };
        return define;
    }

    #endregion

    #endregion
}