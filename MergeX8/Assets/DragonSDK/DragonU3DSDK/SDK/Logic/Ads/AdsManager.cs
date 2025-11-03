using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin.PluginStructs;
using System;
using System.Linq;
using System.Threading;
using DragonU3DSDK;
using System.Text;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;

namespace Dlugin
{
    public class AdsManager
    {
        // AdsManager 单例化
        private static          AdsManager _instance;
        private static readonly object     SysLock = new();

        public static AdsManager Instance
        {
            get
            {
                if (_instance != null) return _instance;

                lock (SysLock)
                {
                    _instance ??= new AdsManager();
                }

                return _instance;
            }
        }

        #region Properties

        private readonly ReaderWriterLockSlim callbackLock        = new();
        private readonly Queue<Action>        callbackActionQueue = new();

        private Action              m_PlayInterstitialCallback;
        private Action<bool>        m_PlayRewardCallback;
        private List<AdsUnitDefine> m_InterstitialReady = new();
        private List<AdsUnitDefine> m_RewardReady       = new();

        private Dictionary<string, IAdsProvider> m_PluginsMap        = new();
        private Dictionary<IAdsProvider, int>    m_ProviderWeightMap = new();
        private HashSet<string>                  m_DisabledPlugins   = new();

        private Dictionary<AD_Type, Dictionary<string, int>> m_AdNetworkDisposedCount    = new();
        private int                                          m_AdNetworkDisposedMaxCount = 5;
        private double                                       m_AdNoOpEcpmFloor           = 0.0;
        private long                                         m_ReloadDelayWhenDisposed   = 10 * 1000; // Milliseconds
        private bool                                         m_AdsDisposeEnabled         = true;
        private bool                                         m_UaSplitterEnabled         = false;
        private double                                       m_UaSplitterRate            = 0.0;
        private double                                       m_UaSplitterEcpmFloor       = 0.0;

        private float m_ServerSizeControlConfigurationRefreshInterval = 300.0f; // Seconds

        private float m_LeftTimeToRefreshServerSideControlConfiguration;
        private float m_LeftTimeToRefreshAdsSplitterConfiguration;

        private IAdsProvider m_BannerShowingProvider;
        private bool         m_BannerShowing;
        private bool         m_ShowBannerWhenOtherAdsClosed;

        private IAdsProvider m_MrecShowingProvider;
        private bool         m_MrecShowing;
        private bool         m_ShowMrecWhenOtherAdsClosed;

        private readonly Dictionary<string, Dictionary<AD_Type, int>> m_AdsPlayedCounts = new();

        private double m_dummyRevenue        = 0.0;
        private int    m_dummyAdsPlayedCount = 0;

#if DEBUG || DEVELOPMENT_BUILD
        private AdsEventFileLogger m_AdsEventFileLogger;
#endif

        #endregion

        #region Public

        private AdsManager()
        {
            TimerManager.Instance.AddDelegate(Update);
        }

        public void Initialize(bool isLowPowerDevice = false)
        {
#if DEBUG || DEVELOPMENT_BUILD
            m_AdsEventFileLogger = new AdsEventFileLogger();
            m_AdsEventFileLogger.Startup();
#endif

            //添加全局配置初始化

            InitializeAllPlugins(isLowPowerDevice);
            InitListeners();
            LoadAllAds();

            // Call first refresh
            RefreshAdsSplitterConfigurations(-2);
            RefreshAdsServerSideControlConfigurations(-2);

#if !DISABLE_ADS_LOG
            DebugUtil.Log("Simon Ad AdsManager.Initialize!!!");
#endif
        }

        private void Update(float delta)
        {
            RefreshAdsSplitterConfigurations(delta);
            RefreshAdsServerSideControlConfigurations(delta);

            if (!callbackLock.IsWriteLockHeld && !callbackLock.TryEnterWriteLock(200)) return;

            try
            {
                while (callbackActionQueue.Count > 0)
                {
                    var callback = callbackActionQueue.Dequeue();
                    callback?.Invoke();
                }
            }
            finally
            {
                try
                {
                    callbackLock.ExitWriteLock();
                }
                catch (SynchronizationLockException e)
                {
                    DebugUtil.Log("SynchronizationLockException : " + e.Message);
                }
            }
        }

        public void SetTestDeviceAdvertisingIdentifiers(string adid)
        {
            if (string.IsNullOrEmpty(adid))
            {
                return;
            }

            foreach (var kv in m_PluginsMap)
            {
                kv.Value?.SetTestDeviceAdvertisingIdentifiers(adid);
            }
        }

        public void HandleLoad(AD_Type type)
        {
            foreach (var entry in m_PluginsMap)
            {
                entry.Value?.HandleLoad(type);
            }
        }

        public void SetDummyRevenue(double dummyRevenue)
        {
            m_dummyRevenue = dummyRevenue;
        }

        public void SetDummyAdsPlayedCount(int dummyAdsPlayedCount)
        {
            m_dummyAdsPlayedCount = dummyAdsPlayedCount;
        }

        public bool IsInterstitialReady()
        {
#if UNITY_EDITOR
            return true;
#endif
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }

            if (m_InterstitialReady == null || m_InterstitialReady.Count == 0)
            {
                return false;
            }

            return m_InterstitialReady.Any(unit =>
            {
                if (!m_UaSplitterEnabled && unit.m_PluginName == Constants.UA)
                {
                    return false;
                }

                return unit.m_EcpmFloor >= m_AdNoOpEcpmFloor;
            });
        }

        public int InterstitialBufferCount()
        {
            return m_InterstitialReady.Count;
        }

        public int RewardBufferCount()
        {
            return m_RewardReady.Count;
        }

        public void EnablePlugin(string pluginName)
        {
            m_DisabledPlugins.Remove(pluginName);

            if (m_PluginsMap.TryGetValue(pluginName, out var provider))
            {
                provider.HandleLoad(AD_Type.Interstitial);
                provider.HandleLoad(AD_Type.RewardVideo);
            }
        }

        public void DisablePlugin(string pluginName)
        {
            m_DisabledPlugins.Add(pluginName);
        }

        public bool IsRewardReady()
        {
#if UNITY_EDITOR
            return true;
#endif
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }

            if (m_RewardReady == null || m_RewardReady.Count == 0)
            {
                return false;
            }

            return m_RewardReady.Any(unit =>
            {
                if (!m_UaSplitterEnabled && unit.m_PluginName == Constants.UA)
                {
                    return false;
                }

                return unit.m_EcpmFloor >= m_AdNoOpEcpmFloor;
            });
        }

        public void PlayInterstitial(Action callback, string placement = null)
        {
#if UNITY_EDITOR
            EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.ConfirmWindowEvent>().Data(
                new DragonU3DSDK.SDKEvents.ConfirmWindowEvent.UIData()
                {
                    DescString     = $"AD Interstitial : {placement} play finish.",
                    OKCallback     = callback,
                    HasCloseButton = false
                }).Trigger();
            return;
#endif

            var maxRevenue       = 0.0;
            var levelPlayRevenue = 0.0;
            var uaRevenue        = 0.0;
            FindRevenues(m_InterstitialReady, out maxRevenue, out levelPlayRevenue, out uaRevenue);

            var uaBidding = UnityEngine.Random.Range(0.0f, 1.0f) < m_UaSplitterRate;

            var extraData = string.Empty;
            if (m_PluginsMap.TryGetValue(Constants.UA, out var plugin))
            {
                if (plugin is UaPlugin uaPlugin)
                {
                    extraData = uaPlugin.GetAdExtraData(AD_Type.Interstitial);
                }
            }
            
            AdsUnitDefine ad;
            var allAdInstanceIds = m_InterstitialReady.Select(adUnit => adUnit.m_InstanceId).Distinct().ToList();
            do
            {
                DebugUtil.Log("Simon Ad Choosing ad Interstitial ad COUNT is " + m_InterstitialReady.Count);

                
                ad = AdDispatcher.PickAdsUnitDefine(
                    m_InterstitialReady,
                    m_AdNoOpEcpmFloor,
                    uaBidding,
                    m_UaSplitterEnabled);


                if (ad == null)
                {
                    AdsEventReporter.ReportBidding(
                        "None",
                        string.Empty,
                        "Interstitial",
                        maxRevenue,
                        levelPlayRevenue,
                        uaRevenue,
                        string.Empty,
                        m_UaSplitterEcpmFloor,
                        m_AdNoOpEcpmFloor,
                        uaBidding,
                        m_UaSplitterRate,
                        extraData,
                        string.Empty,
                        allAdInstanceIds);

#if DEBUG || DEVELOPMENT_BUILD
                    m_AdsEventFileLogger.LogAdsNoOp(
                        maxRevenue,
                        levelPlayRevenue,
                        m_UaSplitterEcpmFloor,
                        m_UaSplitterRate);
#endif
                    return;
                }

                if (m_DisabledPlugins.Contains(ad.m_PluginName))
                {
                    continue;
                }

                break;
            } while (true);

            DebugUtil.Log("Simon Ad Simon in Interstitial :plugin:{0}, param : {1}, placement: {2}, type: {3} ",
                ad.m_PluginName,
                ad.m_PluginParam,
                ad.m_Placement,
                ad.m_Type);

            var provider = m_PluginsMap[ad.m_PluginName];
            m_PlayInterstitialCallback = () =>
            {
                callback?.Invoke();

                if (m_ShowBannerWhenOtherAdsClosed)
                {
                    m_ShowBannerWhenOtherAdsClosed = false;

                    ShowBanner();
                }

                if (m_ShowMrecWhenOtherAdsClosed)
                {
                    m_ShowMrecWhenOtherAdsClosed = false;

                    ShowMrec();
                }
            };

            if (provider == null)
            {
                return;
            }

            var result = provider.PlayAds(ad, placement);
            SDK.GetInstance().loginService.LogAdEvent(ad.m_Type, AD_Event_Type.Impression);

#if DEBUG || DEVELOPMENT_BUILD
            m_AdsEventFileLogger.LogAdsTryPlay(
                ad,
                maxRevenue,
                levelPlayRevenue,
                uaRevenue,
                m_UaSplitterEcpmFloor,
                m_UaSplitterRate,
                m_AdNoOpEcpmFloor,
                uaBidding,
                result);
#endif

            AdsEventReporter.ReportBidding(
                ad.m_PluginName,
                ad.m_AdUnitIdentifier,
                "Interstitial",
                maxRevenue,
                levelPlayRevenue,
                uaRevenue,
                ad.m_CreativeIdentifier,
                m_UaSplitterEcpmFloor,
                m_AdNoOpEcpmFloor,
                uaBidding,
                m_UaSplitterRate,
                extraData,
                ad.m_InstanceId,
                allAdInstanceIds);

            if (result && m_BannerShowing)
            {
                HideBanner();

                m_ShowBannerWhenOtherAdsClosed = true;
            }

            if (result && m_MrecShowing)
            {
                HideMrec();

                m_ShowMrecWhenOtherAdsClosed = true;
            }
        }

        private static readonly AdsUnitDefine BannerUnitDefine = new() { m_Type = AD_Type.Banner };

        public void ShowBanner()
        {
#if DEBUG || DEVELOPMENT_BUILD
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("AdsManager : ShowBanner -> ");
            stringBuilder.AppendLine("\tBanner Revenues:");
#endif

            var          maxRevenue       = 0.0;
            IAdsProvider selectedProvider = null;
            foreach (var entry in m_PluginsMap)
            {
                var provider = entry.Value;

#if DEBUG || DEVELOPMENT_BUILD

                stringBuilder.AppendLine(provider.IsAdsReady(BannerUnitDefine)
                    ? $"\t{provider.m_PluginDefine.m_PluginName} is ready"
                    : $"\t{provider.m_PluginDefine.m_PluginName} is not ready");

#endif
                if (!provider.IsAdsReady(BannerUnitDefine)) continue;

                var revenue = provider.GetLoadedAdRevenue(AD_Type.Banner);

#if DEBUG || DEVELOPMENT_BUILD

                stringBuilder.AppendLine($"\t{provider.m_PluginDefine.m_PluginName} -> {revenue}");

#endif

                if (!(revenue > maxRevenue)) continue;

                maxRevenue       = revenue;
                selectedProvider = provider;
            }

            selectedProvider?.ShowBanner();

            m_BannerShowing         = true;
            m_BannerShowingProvider = selectedProvider;

#if DEBUG || DEVELOPMENT_BUILD

            if (selectedProvider != null)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"Selected {selectedProvider.m_PluginDefine.m_PluginName} -> {maxRevenue}");
            }

            DebugUtil.Log(stringBuilder.ToString());

#endif
        }

        public void HideBanner()
        {
            foreach (var entry in m_PluginsMap)
            {
                entry.Value.HideBanner();
            }

            m_BannerShowing                = false;
            m_BannerShowingProvider        = null;
            m_ShowBannerWhenOtherAdsClosed = false;
        }

        private static readonly AdsUnitDefine MrecUnitDefine = new() { m_Type = AD_Type.Mrec };

        public void ShowMrec()
        {
#if DEBUG || DEVELOPMENT_BUILD
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("AdsManager : ShowMrec -> ");
            stringBuilder.AppendLine("\tMrec Revenues:");
#endif

            var          maxRevenue       = 0.0;
            IAdsProvider selectedProvider = null;
            foreach (var entry in m_PluginsMap)
            {
                var provider = entry.Value;

#if DEBUG || DEVELOPMENT_BUILD

                stringBuilder.AppendLine(provider.IsAdsReady(BannerUnitDefine)
                    ? $"\t{provider.m_PluginDefine.m_PluginName} is ready"
                    : $"\t{provider.m_PluginDefine.m_PluginName} is not ready");

#endif

                if (!provider.IsAdsReady(MrecUnitDefine)) continue;

                var revenue = provider.GetLoadedAdRevenue(AD_Type.Mrec);

#if DEBUG || DEVELOPMENT_BUILD

                stringBuilder.AppendLine($"\t{provider.m_PluginDefine.m_PluginName} -> {revenue}");

#endif

                if (!(revenue > maxRevenue)) continue;

                maxRevenue       = revenue;
                selectedProvider = provider;
            }

            selectedProvider?.ShowMRec();

            m_MrecShowing         = true;
            m_MrecShowingProvider = selectedProvider;

#if DEBUG || DEVELOPMENT_BUILD

            if (selectedProvider != null)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"Selected {selectedProvider.m_PluginDefine.m_PluginName} -> {maxRevenue}");
            }

            DebugUtil.Log(stringBuilder.ToString());

#endif
        }

        public void HideMrec()
        {
            foreach (var entry in m_PluginsMap)
            {
                entry.Value.HideMRec();
            }

            m_MrecShowing                = false;
            m_MrecShowingProvider        = null;
            m_ShowMrecWhenOtherAdsClosed = false;
        }

        public void UpdateBannerPosition(float x, float y)
        {
            foreach (var entry in m_PluginsMap)
            {
                entry.Value.UpdateBannerPosition(x, y);
            }
        }

        public void UpdateMRECPosition(float x, float y)
        {
            foreach (var entry in m_PluginsMap)
            {
                entry.Value.UpdateMRECPosition(x, y);
            }
        }

        public void SetBannerWidth(float width)
        {
            foreach (var entry in m_PluginsMap)
            {
                entry.Value.SetBannerWidth(width);
            }
        }

        public string InterstitialStatus()
        {
            var sb = new StringBuilder();

            foreach (var ad in m_InterstitialReady)
            {
                sb.AppendFormat("{0} | {1} | {2}\n", ad.m_PluginName, ad.m_EcpmFloor, ad.m_Weight);
            }

            return sb.ToString();
        }


        public string RewardVideoStatus()
        {
            var sb = new StringBuilder();

            foreach (var ad in m_RewardReady)
            {
                sb.AppendFormat("{0} | {1} | {2}\n", ad.m_PluginName, ad.m_EcpmFloor, ad.m_Weight);
            }

            return sb.ToString();
        }

        public void PlayReward(Action<bool> callback, string placement = null)
        {
#if UNITY_EDITOR
            EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.ConfirmWindowEvent>().Data(
                new DragonU3DSDK.SDKEvents.ConfirmWindowEvent.UIData()
                {
                    DescString =
                        $"AD Reward : {placement} play finish.\nConfirm that the representative has a prize, if canceled, there is no.",
                    OKCallback      = () => { callback(true); },
                    HasCancelButton = true,
                    CancelCallback  = () => { callback(false); },
                    HasCloseButton  = false
                }).Trigger();
            return;
#endif

            if (!IsRewardReady()) return;

            var maxRevenue       = 0.0;
            var levelPlayRevenue = 0.0;
            var uaRevenue        = 0.0;
            FindRevenues(m_RewardReady, out maxRevenue, out levelPlayRevenue, out uaRevenue);

            var uaBidding = UnityEngine.Random.Range(0.0f, 1.0f) < m_UaSplitterRate;

            var extraData = string.Empty;
            if (m_PluginsMap.TryGetValue(Constants.UA, out var plugin))
            {
                if (plugin is UaPlugin uaPlugin)
                {
                    extraData = uaPlugin.GetAdExtraData(AD_Type.RewardVideo);
                }
            }
            
            AdsUnitDefine ad;
            var allAdInstanceIds = m_RewardReady.Select(adUnit => adUnit.m_InstanceId).Distinct().ToList();

            do
            {
                DebugUtil.Log("Simon Ad Choosing ad Ready ad COUNT is " + m_RewardReady.Count);

                ad = AdDispatcher.PickAdsUnitDefine(m_RewardReady, m_AdNoOpEcpmFloor, uaBidding, m_UaSplitterEnabled);

                if (ad == null)
                {
                    AdsEventReporter.ReportBidding(
                        "None",
                        string.Empty,
                        "Rewarded",
                        maxRevenue,
                        levelPlayRevenue,
                        uaRevenue,
                        string.Empty,
                        m_UaSplitterEcpmFloor,
                        m_AdNoOpEcpmFloor,
                        uaBidding,
                        m_UaSplitterRate,
                        extraData,
                        string.Empty,
                        allAdInstanceIds);

#if DEBUG || DEVELOPMENT_BUILD
                    m_AdsEventFileLogger.LogAdsNoOp(
                        maxRevenue,
                        levelPlayRevenue,
                        m_UaSplitterEcpmFloor,
                        m_UaSplitterRate);
#endif
                    return;
                }

                if (m_DisabledPlugins.Contains(ad.m_PluginName))
                {
                    continue;
                }

                break;
            } while (true);

            m_PlayRewardCallback = (rewarded) =>
            {
                callback?.Invoke(rewarded);

                if (m_ShowBannerWhenOtherAdsClosed)
                {
                    ShowBanner();
                }

                if (m_ShowMrecWhenOtherAdsClosed)
                {
                    ShowMrec();
                }
            };

            DebugUtil.Log(
                "Simon Ad Simon in PlayReward :plugin:{0}, param : {1}, placement: {2}, type: {3}, ecpm: {4} ",
                ad.m_PluginName, ad.m_PluginParam, ad.m_Placement, ad.m_Type, ad.m_EcpmFloor);

            var provider = m_PluginsMap[ad.m_PluginName];

            if (provider == null)
            {
                return;
            }

            var result = provider.PlayAds(ad, placement);
            SDK.GetInstance().loginService.LogAdEvent(ad.m_Type, AD_Event_Type.Impression);

#if DEBUG || DEVELOPMENT_BUILD
            m_AdsEventFileLogger.LogAdsTryPlay(
                ad,
                maxRevenue,
                levelPlayRevenue,
                uaRevenue,
                m_UaSplitterEcpmFloor,
                m_UaSplitterRate,
                m_AdNoOpEcpmFloor,
                uaBidding,
                result);
#endif

            AdsEventReporter.ReportBidding(
                ad.m_PluginName,
                ad.m_AdUnitIdentifier,
                "Rewarded",
                maxRevenue,
                levelPlayRevenue,
                uaRevenue,
                ad.m_CreativeIdentifier,
                m_UaSplitterEcpmFloor,
                m_AdNoOpEcpmFloor,
                uaBidding,
                m_UaSplitterRate,
                extraData,
                ad.m_InstanceId,
                allAdInstanceIds);

            if (result && m_MrecShowing)
            {
                HideMrec();

                m_ShowMrecWhenOtherAdsClosed = true;
            }

            if (result && m_BannerShowing)
            {
                HideBanner();

                m_ShowBannerWhenOtherAdsClosed = true;
            }
        }

        /// <summary>
        /// 显示积分墙
        /// </summary>
        public bool ShowOfferwall(
            string       name,
            int          wait_request_millisecond,
            Action<bool> show_pre_callback,
            Action<bool> show_post_callback)
        {
            if (IsOfferwallReady(name))
            {
                if (!m_PluginsMap.TryGetValue(name, out var provider)) return true;

                var unit = new AdsUnitDefine
                {
                    m_Type                     = AD_Type.OfferWall,
                    show_pre_callback          = show_pre_callback,
                    show_post_callback         = show_post_callback,
                    m_wait_request_millisecond = wait_request_millisecond
                };
                provider.PlayAds(unit);

                return true;
            }

            show_pre_callback?.Invoke(false);

            return false;
        }

        /// <summary>
        /// 积分墙是否准备好
        /// </summary>
        /// <returns></returns>
        public bool IsOfferwallReady(string name)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }

            return m_PluginsMap.TryGetValue(name, out var provider) && provider.IsAdsReady(null);
        }

        public void SpendCurrency()
        {
            foreach (var entry in m_PluginsMap)
            {
                entry.Value.SpendCurrency();
            }
        }

        public void SetMuted(bool muted)
        {
            foreach (var entry in m_PluginsMap)
            {
                entry.Value.SetMuted(muted);
            }
        }

        public bool IsMuted()
        {
            return m_PluginsMap.Values.All(provider => provider.IsMuted());
        }

        public void Clear()
        {
            SDK.GetInstance().m_AdsDispather.onAdsLoadFinish          -= OnAdsLoadFinish;
            SDK.GetInstance().m_AdsDispather.onAdsPlayFinish          -= OnAdsPlayFinish;
            SDK.GetInstance().m_AdsDispather.onAdsAvailabilityChanged -= OnAdsAvailabilityChanged;
            SDK.GetInstance().m_AdsDispather.onAdsWatched             -= OnAdsWatched;
            SDK.GetInstance().m_AdsDispather.onAdsClick               -= OnAdsClick;
            SDK.GetInstance().m_AdsDispather.onAdsImpression          -= OnAdsImpression;

#if DEBUG || DEVELOPMENT_BUILD
            m_AdsEventFileLogger?.Shutdown();
            m_AdsEventFileLogger = null;
#endif
        }

        #endregion

        private void InitializeAllPlugins(bool isLowPowerDevice = false)
        {
            var configs          = AdConfig.Instance.GetAdWeight();
            var interConfigs     = AdConfig.Instance.GetInterstitialWeight();
            var offerwallConfigs = AdConfig.Instance.GetOfferwallWeight();

            var mergedConfigs = new Dictionary<string, int>();

            foreach (var entry in configs)
            {
                if (entry.Value <= 0)
                {
                    continue;
                }

                if (!mergedConfigs.ContainsKey(entry.Key))
                {
                    mergedConfigs.Add(entry.Key, entry.Value);
                }
            }

            foreach (var entry in interConfigs)
            {
                if (entry.Value <= 0)
                {
                    continue;
                }

                if (!mergedConfigs.ContainsKey(entry.Key))
                {
                    mergedConfigs.Add(entry.Key, entry.Value);
                }
            }

            foreach (var entry in offerwallConfigs)
            {
                if (entry.Value <= 0)
                {
                    continue;
                }

                if (!mergedConfigs.ContainsKey(entry.Key))
                {
                    mergedConfigs.Add(entry.Key, entry.Value);
                }
            }


            foreach (var kv in mergedConfigs)
            {
                if (isLowPowerDevice)
                {
                    if (kv.Key.Equals(Constants.UnityAds) ||
                        kv.Key.Equals(Constants.AppLovin) ||
                        kv.Key.Equals(Constants.AdColony) ||
                        kv.Key.Equals(Constants.Chartboost))
                    {
                        DebugUtil.Log("AdManager 低端机型 不初始化{0}平台", kv.Key);
                        continue;
                    }
                }

#if !DISABLE_ADS_LOG
                DebugUtil.Log("Plugin {0} weight {1}", kv.Key, kv.Value);
#endif

                IAdsProvider plugin = null;
                switch (kv.Key)
                {
                    case Constants.Admob:
                        //plugin = new AdmobPlugin();
                        break;
                    case Constants.Audience:
#if !UNITY_EDITOR
                        //plugin = new AudiencePlugin();
#endif
                        break;
                    case Constants.IronSource:
                        /*
                        IronSourceConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<IronSourceConfigInfo>(Constants.IronSource);
                        if (null != config)
                        {
    #if UNITY_ANDROID
                            if(config.ExclusivehMaxAndroid) plugin = new IronSourcePlugin();
    #elif UNITY_IPHONE
                            if(config.ExclusivehMaxiOS) plugin = new IronSourcePlugin();
    #endif
                            }
                        */
                        break;
                    case Constants.UnityAds:
                        //plugin = new UnityAdsPlugin();
                        break;
                    case Constants.AppLovin:
#if !UNITY_EDITOR
                         //plugin = new AppLovinPlugin();
#endif
                        break;
                    case Constants.Tapjoy:
#if USE_TAPJOY_SDK
                        plugin = new TapjoyPlugin();
#endif
                        break;
                }

                if (plugin != null)
                {
                    m_PluginsMap.Add(kv.Key, plugin);
                    m_ProviderWeightMap.Add(plugin, kv.Value);
                }
            }

            IAdsProvider uaProvider        = new UaPlugin();
            IAdsProvider maxProvider       = new MaxPlugin();

            m_PluginsMap.Add(Constants.UA,        uaProvider);
            m_PluginsMap.Add(Constants.MAX,       maxProvider);

            m_ProviderWeightMap.Add(uaProvider,        100);
            m_ProviderWeightMap.Add(maxProvider,       100);

            foreach (var kv in m_PluginsMap)
            {
                try
                {
                    kv.Value.Initialize();
                }
                catch (Exception e)
                {
                    DebugUtil.LogWarning("[广告]存在广告初始化异常，异常为{0}. \n堆栈: {1}", e.ToString(), e.StackTrace);
                    //继续执行其他平台初始化
                }
            }
        }

        private void InitListeners()
        {
            SDK.GetInstance().m_AdsDispather.onAdsLoadFinish          += OnAdsLoadFinish;
            SDK.GetInstance().m_AdsDispather.onAdsPlayFinish          += OnAdsPlayFinish;
            SDK.GetInstance().m_AdsDispather.onAdsAvailabilityChanged += OnAdsAvailabilityChanged;
            SDK.GetInstance().m_AdsDispather.onAdsWatched             += OnAdsWatched;
            SDK.GetInstance().m_AdsDispather.onAdsClick               += OnAdsClick;
            SDK.GetInstance().m_AdsDispather.onAdsImpression          += OnAdsImpression;
        }

        private void LoadAllAds()
        {
            m_RewardReady.Clear();
            m_InterstitialReady.Clear();

            foreach (var kv in m_PluginsMap)
            {
                kv.Value.PreloadAds();
            }
        }


        private void OnAdsLoadFinish(AdsUnitDefine unit, SDKError error)
        {
            if (error != null && error.err != Constants.kErrorSuccess && error.err != Constants.kErrorDelayed)
            {
#if DEBUG || DEVELOPMENT_BUILD
                m_AdsEventFileLogger.LogAdsLoadFailed(unit);
#endif

                EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.AdsLoadFailEvent>();
                return;
            }

            if (unit != null)
            {
#if DEBUG || DEVELOPMENT_BUILD
                m_AdsEventFileLogger.LogAdsLoaded(unit);
#endif

                QueryAdsDispose(unit, (dispose, adsUnit) =>
                {
                    if (dispose && DisposeAds(adsUnit))
                    {
#if DEBUG || DEVELOPMENT_BUILD
                        m_AdsEventFileLogger.LogAdsDisposed(unit);
#endif

                        DelayActionManager.Instance.DebounceInMainThread(
                            $"{adsUnit.m_Type}-{adsUnit.m_PluginName}",
                            (int)m_ReloadDelayWhenDisposed,
                            () =>
                            {
                                if (m_DisabledPlugins.Contains(adsUnit.m_PluginName))
                                {
                                    return;
                                }

                                if (m_PluginsMap.TryGetValue(adsUnit.m_PluginName, out var provider))
                                {
                                    provider.HandleLoad(adsUnit.m_Type);
                                }
                            });
                        return;
                    }

                    ClearDisposeCount(adsUnit.m_Type, adsUnit.m_NetworkName);

                    switch (adsUnit.m_Type)
                    {
                        case AD_Type.Interstitial:
                            m_InterstitialReady.Add(adsUnit);
                            AdDispatcher.SortAdUnits(m_InterstitialReady);
                            break;
                        case AD_Type.RewardVideo:
                            m_RewardReady.Add(adsUnit);
                            AdDispatcher.SortAdUnits(m_RewardReady);
                            break;
                    }
                });
            }
            else
            {
                SDK.FormatDebug("the coming unit is nulllllllllllll");
            }
        }

        private bool DisposeAds(AdsUnitDefine unit)
        {
            var networkName = unit.m_NetworkName;
            var adType      = unit.m_Type;

            var count = AddDisposeCount(adType, networkName);
            if (count >= m_AdNetworkDisposedMaxCount)
            {
                return false;
            }

            return m_PluginsMap.TryGetValue(unit.m_PluginName, out var provider) && provider.DisposeAd(unit);
        }

        private int AddDisposeCount(AD_Type adType, string networkName)
        {
            if (!m_AdNetworkDisposedCount.TryGetValue(adType, out var counts))
            {
                counts = new Dictionary<string, int>();
                m_AdNetworkDisposedCount.Add(adType, counts);
            }

            if (!counts.TryGetValue(networkName, out var count))
            {
                counts.Add(networkName, 1);
                return 1;
            }

            ++count;
            counts[networkName] = count;
            return count;
        }

        private void ClearDisposeCount(AD_Type adType, string networkName)
        {
            if (!m_AdNetworkDisposedCount.TryGetValue(adType, out var counts))
            {
                return;
            }

            counts[networkName] = 0;
        }

        private static string FormatAdsUnitDefineInfo(AdsUnitDefine unit)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Plugin: {unit.m_PluginName}");
            stringBuilder.AppendLine($"AdFormat: {unit.m_Type}");
            stringBuilder.AppendLine($"AdUnitIdentifier: {unit.m_AdUnitIdentifier}");
            stringBuilder.AppendLine($"NetworkName: {unit.m_NetworkName}");
            stringBuilder.AppendLine($"NetworkPlacement: {unit.m_NetworkPlacement}");
            stringBuilder.AppendLine($"CreativeIndentifier: {unit.m_CreativeIdentifier}");
            stringBuilder.AppendLine($"Revenue: {unit.m_EcpmFloor}");
            stringBuilder.AppendLine($"RevenuePrecision: {unit.m_RevenuePrecision}");
            stringBuilder.AppendLine($"DspName: {unit.m_DspName}");

            return stringBuilder.ToString();
        }

        private void QueryAdsDispose(AdsUnitDefine unit, Action<bool, AdsUnitDefine> callback)
        {
            if (!m_AdsDisposeEnabled)
            {
                callback?.Invoke(false, unit);
                return;
            }

            PluginNameType pluginName;
            switch (unit.m_PluginName)
            {
                case Constants.MAX:
                    pluginName = PluginNameType.Max;
                    break;

                case Constants.UA:
                    callback?.Invoke(false, unit);
                    return;

                default:
                    DebugUtil.LogError($"AdsManager : Query bidding failed: Unsupported plugin {unit.m_PluginName}");

                    callback?.Invoke(false, unit);
                    return;
            }

            AdFormatType adFormat;
            string       adFormatName;
            switch (unit.m_Type)
            {
                case AD_Type.Interstitial:
                    adFormat     = AdFormatType.InterstitialAd;
                    adFormatName = "Interstitial";
                    break;

                case AD_Type.RewardVideo:
                    adFormat     = AdFormatType.RewardedVideoAd;
                    adFormatName = "Rewarded";
                    break;

                default:
                    DebugUtil.LogError($"AdsManager : Query bidding failed: Unsupported ad format: {unit.m_Type}");

                    callback?.Invoke(false, unit);
                    return;
            }

            try
            {
                var revenue = m_dummyRevenue > 0 ? m_dummyRevenue : unit.m_EcpmFloor;
                var playedCount = m_dummyAdsPlayedCount > 0
                    ? m_dummyAdsPlayedCount
                    : GetAdsPlayedCount(unit.m_PluginName, unit.m_Type);

                DebugUtil.Log(
                    $"AdsManager : Query bidding with revenue: {revenue}, playedCount: {playedCount}, info:{FormatAdsUnitDefineInfo(unit)}");

#if DEBUG || DEVELOPMENT_BUILD
                m_AdsEventFileLogger.LogAdsQueryBidding(unit, revenue, playedCount);
#endif

                // request server for dispose
                var request = new CGetAdvertisementBidding()
                {
                    PluginName         = pluginName,
                    AdFormat           = adFormat,
                    AdUnitIdentifier   = unit.m_AdUnitIdentifier,
                    NetworkName        = unit.m_NetworkName,
                    NetworkPlacement   = unit.m_NetworkPlacement,
                    Placement          = unit.m_Placement,
                    CreativeIdentifier = unit.m_CreativeIdentifier,
                    Revenue            = revenue,
                    RevenuePrecision   = unit.m_RevenuePrecision,
                    DspName            = unit.m_DspName,
                    SessionImpressions = playedCount,
                };

                APIManager.Instance.Send<CGetAdvertisementBidding, SGetAdvertisementBidding>(
                    request,
                    response =>
                    {
                        DebugUtil.Log($"AdsManager : Query bidding succeed: result is {response.IsDiscard}");

#if DEBUG || DEVELOPMENT_BUILD
                        m_AdsEventFileLogger.LogAdsQueryBiddingResult(unit, response.IsDiscard, response.DiscardRate,
                            response.UserEcpmFloor);
#endif

                        AdsEventReporter.ReportAdsQueryDisposing(
                            unit.m_PluginName,
                            unit.m_AdUnitIdentifier,
                            adFormatName,
                            unit.m_EcpmFloor,
                            unit.m_RevenuePrecision,
                            unit.m_CreativeIdentifier,
                            unit.m_NetworkName,
                            response.UserEcpmFloor,
                            response.DiscardRate,
                            response.IsDiscard,
                            m_UaSplitterEcpmFloor,
                            response.ExtraData,
                            unit.m_InstanceId);

                        callback?.Invoke(response.IsDiscard, unit);
                    },
                    (errorCode, errorMessage, _) =>
                    {
                        DebugUtil.Log($"AdsManager : Query bidding failed: {errorCode} - {errorMessage}");

                        callback?.Invoke(false, unit);
                    });
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"AdsManager : request bidding failed: {exception.Message}");

                callback?.Invoke(false, unit);
            }
        }

        private void OnAdsWatched(AdsUnitDefine define)
        {
            DebugUtil.Log($"AdsManager.OnAdsWatched for placement {define.m_Placement}");

            switch (define.m_Type)
            {
                case AD_Type.Interstitial:
                {
                    if (define.m_DestorySelf)
                    {
                        m_InterstitialReady.Remove(define);
                    }

                    break;
                }
                case AD_Type.RewardVideo:
                {
                    if (m_PlayRewardCallback != null)
                    {
                        if (callbackLock.IsWriteLockHeld || callbackLock.TryEnterWriteLock(200))
                        {
                            try
                            {
                                callbackActionQueue.Enqueue(() =>
                                {
                                    m_PlayRewardCallback?.Invoke(define.m_Need_Reward);
                                    m_PlayRewardCallback = null;
                                });
                            }
                            finally
                            {
                                try
                                {
                                    callbackLock.ExitWriteLock();
                                }
                                catch (SynchronizationLockException e)
                                {
                                    DebugUtil.Log("SynchronizationLockException : " + e.Message);
                                }
                            }
                        }
                    }

                    if (define.m_DestorySelf)
                    {
                        m_RewardReady.Remove(define);
                    }

                    break;
                }
            }
        }


        private void OnAdsClick(AdsUnitDefine define)
        {
            DebugUtil.Log($"AdsManager.OnAdsClick for placement {define.m_Placement}");

            SDK.GetInstance().loginService.LogAdEvent(define.m_Type, AD_Event_Type.Click);

#if DEBUG || DEVELOPMENT_BUILD
            m_AdsEventFileLogger.LogAdsClicked(define);
#endif
        }


        private void OnAdsImpression(AdsUnitDefine define)
        {
            DebugUtil.Log($"AdsManager.OnAdsImpression for placement {define.m_Placement}");

            SDK.GetInstance().loginService.LogAdEvent(define.m_Type, AD_Event_Type.Impression);
        }

        private void OnAdsAvailabilityChanged(AdsUnitDefine unit, bool newState)
        {
            DebugUtil.Log(
                "AdsManager.OnAdsAvailabilityChanged ----> ads availability changed for id:{0}, new state is {1}",
                unit.m_PluginParam,
                newState);
        }

        private void OnAdsPlayFinish(AdsUnitDefine unit, SDKError error)
        {
            DebugUtil.Log("AdsManager.OnAdsPlayFinish ----> ads play finished for id:{0}, error is {1}",
                unit.m_PluginParam, error == null ? "null" : error.ToString());

            AddAdsPlayedCount(unit.m_PluginName, unit.m_Type);

#if DEBUG || DEVELOPMENT_BUILD
            m_AdsEventFileLogger.LogAdsDisplayed(unit);
#endif

            if (!unit.m_DestorySelf) return;

            switch (unit.m_Type)
            {
                case AD_Type.Interstitial:
                {
                    if (m_PlayInterstitialCallback != null)
                    {
                        if (callbackLock.IsWriteLockHeld || callbackLock.TryEnterWriteLock(200))
                        {
                            try
                            {
                                callbackActionQueue.Enqueue(() =>
                                {
                                    m_PlayInterstitialCallback?.Invoke();
                                    m_PlayInterstitialCallback = null;
                                });
                            }
                            finally
                            {
                                try
                                {
                                    callbackLock.ExitWriteLock();
                                }
                                catch (SynchronizationLockException e)
                                {
                                    DebugUtil.Log("SynchronizationLockException : " + e.Message);
                                }
                            }
                        }
                    }

                    if (m_InterstitialReady.Contains(unit))
                        m_InterstitialReady.Remove(unit);

                    break;
                }
                case AD_Type.RewardVideo:
                {
                    if (m_RewardReady.Contains(unit))
                        m_RewardReady.Remove(unit);

                    if (callbackLock.IsWriteLockHeld || callbackLock.TryEnterWriteLock(200))
                    {
                        try
                        {
                            callbackActionQueue.Enqueue(() => { });
                        }
                        finally
                        {
                            try
                            {
                                callbackLock.ExitWriteLock();
                            }
                            catch (SynchronizationLockException e)
                            {
                                DebugUtil.Log("SynchronizationLockException : " + e.Message);
                            }
                        }
                    }

                    break;
                }
            }
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void RefreshAdsSplitterConfigurations(float delta)
        {
            m_LeftTimeToRefreshAdsSplitterConfiguration -= delta;
            if (m_LeftTimeToRefreshAdsSplitterConfiguration > 0.0f)
            {
                return;
            }

            m_LeftTimeToRefreshAdsSplitterConfiguration = m_ServerSizeControlConfigurationRefreshInterval;

            // Query Splitter configurations
            APIManager.Instance.Send<CGetAdvertisementSplitter, SGetAdvertisementSplitter>(
                new CGetAdvertisementSplitter(),
                response =>
                {
                    m_UaSplitterEnabled   = response.Rate > 0;
                    m_UaSplitterRate      = response.Rate;
                    m_UaSplitterEcpmFloor = response.EcpmFloor;

                    DebugUtil.Log(
                        $"AdsManager : Fetch Splitter settings succeed: UaSplitterEnabled => {m_UaSplitterEnabled}, UaSplitterRate => {m_UaSplitterRate}, UaSplitterEcpmFloor => {m_UaSplitterEcpmFloor}");

#if DEBUG || DEVELOPMENT_BUILD
                    m_AdsEventFileLogger.LogSplitterSettings(
                        m_UaSplitterEnabled,
                        m_UaSplitterRate,
                        m_UaSplitterEcpmFloor);
#endif
                },
                (errorCode, errorMessage, _) =>
                {
                    DebugUtil.Log($"AdsManager : Fetch Bidding settings failed: {errorCode} - {errorMessage}");
                });
        }

        private void RefreshAdsServerSideControlConfigurations(float delta)
        {
            m_LeftTimeToRefreshServerSideControlConfiguration -= delta;
            if (m_LeftTimeToRefreshServerSideControlConfiguration > 0.0f)
            {
                return;
            }

            m_LeftTimeToRefreshServerSideControlConfiguration = m_ServerSizeControlConfigurationRefreshInterval;

            // Query ServerSide Control Configurations
            APIManager.Instance.Send<CGetAdvertisementBiddingSetting, SGetAdvertisementBiddingSetting>(
                new CGetAdvertisementBiddingSetting(),
                response =>
                {
                    m_AdNoOpEcpmFloor         = response.NoOpEcpmFloor;
                    m_ReloadDelayWhenDisposed = response.ReloadDelay;
                    m_AdsDisposeEnabled       = response.IsOpen;

                    DebugUtil.Log(
                        $"AdsManager : Fetch Bidding settings succeed: NoOpEcpmFloor => {m_AdNoOpEcpmFloor}, ReloadDelay => {m_ReloadDelayWhenDisposed}, AdsDisposeEnabled => {m_AdsDisposeEnabled}");

#if DEBUG || DEVELOPMENT_BUILD
                    m_AdsEventFileLogger.LogBiddingSettings(
                        m_AdsDisposeEnabled,
                        m_AdNoOpEcpmFloor,
                        m_ReloadDelayWhenDisposed);
#endif
                },
                (errorCode, errorMessage, _) =>
                {
                    DebugUtil.Log($"AdsManager : Fetch Bidding settings failed: {errorCode} - {errorMessage}");
                });
        }

        private int GetAdsPlayedCount(string pluginName, AD_Type adType)
        {
            return !m_AdsPlayedCounts.TryGetValue(pluginName, out var countsByType)
                ? 0
                : countsByType.GetValueOrDefault(adType, 0);
        }

        private void AddAdsPlayedCount(string pluginName, AD_Type adType)
        {
            if (!m_AdsPlayedCounts.TryGetValue(pluginName, out var countsByType))
            {
                countsByType = new Dictionary<AD_Type, int> { { adType, 1 } };
                m_AdsPlayedCounts.Add(pluginName, countsByType);
                return;
            }

            if (!countsByType.TryGetValue(adType, out var count))
            {
                countsByType[adType] = 1;
            }
            else
            {
                countsByType[adType] = count + 1;
            }
        }

        private static void FindRevenues(
            List<AdsUnitDefine> sources,
            out double          maxRevenue,
            out double          levelPlayRevenue,
            out double          uaRevenue)
        {
            maxRevenue       = 0;
            levelPlayRevenue = 0;
            uaRevenue        = 0;

            foreach (var source in sources)
            {
                switch (source.m_PluginName)
                {
                    case Constants.MAX:
                        maxRevenue = source.m_EcpmFloor;
                        break;

                    case Constants.UA:
                        if (source.m_EcpmFloor > uaRevenue)
                        {
                            uaRevenue = source.m_EcpmFloor;
                        }
                        break;
                }
            }
        }
    }
}