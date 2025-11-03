using System;
using System.Collections.Generic;
using Dlugin;
using DragonU3DSDK;
using Newtonsoft.Json;
using UnityEngine;

internal class MaxInfoReportCallback : MonoBehaviour
{
    #region Properties

    private const string CallbackObjectName = "[MaxInfoReport]";

    private static readonly Queue<Action> Actions = new();
    private static          MaxPlugin     _plugin;

    #endregion

    #region Public Apis

    public static void Setup(MaxPlugin maxPlugin)
    {
        _plugin = maxPlugin;

        DelayActionManager.Instance.DebounceInMainThread("MaxInfoReportSetup", 1, () =>
        {
            var gameObject = GameObject.Find(CallbackObjectName);
            if (gameObject)
            {
                var callbacks = gameObject.GetComponent<MaxInfoReportCallback>();
                if (!callbacks)
                {
                    gameObject.AddComponent<MaxInfoReportCallback>();
                }
            }
            else
            {
                gameObject = new GameObject(CallbackObjectName, typeof(MaxInfoReportCallback));
            }

            DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID && !UNITY_EDITOR
            var maxConfig = PluginsInfoManager.Instance.GetPluginConfig<MAXConfigInfo>(Constants.MAX);

            try
            {
                var rewardedAdUnitId = maxConfig.AndroidRewardedPlacementsWithEcpmFloor[0].key;
                var interstitialAdUnitId = maxConfig.AndroidInterstitialPlacementsWithEcpmFloor[0].key;

                var bannerAdUnitId = string.Empty;
                if (maxConfig.AndroidBannerPlacementsWithEcpmFloor is { Length: > 0 })
                {
                    bannerAdUnitId = maxConfig.AndroidBannerPlacementsWithEcpmFloor[0].key;
                }

                var bridge = new AndroidJavaClass("com.dragonplus.MaxInfoBridge");
                bridge.CallStatic(
                    "initialize",
                    CallbackObjectName,
                    rewardedAdUnitId,
                    interstitialAdUnitId,
                    bannerAdUnitId);
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"[MaxInfoReport] Initialize failed: {exception.Message}");
            }
#endif

#if UNITY_IOS && !UNITY_EDITOR
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialAdLoaded;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialAdDisplayed;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialAdClicked;

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayed;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClicked;
#endif
        });
    }

    public void OnLoaded(string message)
    {
        DebugUtil.Log($"[MaxInfoReport] Received loaded event: {message}");

        EnqueueMainThreadTask(() =>
        {
            try
            {
                var modifiedMessage = FillInstanceId(message);
                var adjust = SDK.GetInstance().adjustPlugin;
                adjust?.TrackEvent("ua_loaded", 0, modifiedMessage);

                DebugUtil.Log($"[MaxInfoReport] Report loaded event `{modifiedMessage}` ok.");
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"[MaxInfoReport] Report loaded event failed: {exception.Message}");
            }
        });
    }

    public void OnImpression(string message)
    {
        DebugUtil.Log($"[MaxInfoReport] Received impression event: {message}");

        EnqueueMainThreadTask(() =>
        {
            try
            {
                var modifiedMessage = FillInstanceId(message);
                var adjust = SDK.GetInstance().adjustPlugin;
                adjust?.TrackEvent("ua_impression", 0, modifiedMessage);

                DebugUtil.Log($"[MaxInfoReport] Report impression event `{modifiedMessage}` ok.");
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"[MaxInfoReport] Report impression event failed: {exception.Message}");
            }
        });
    }

    public void OnClicked(string message)
    {
        DebugUtil.Log($"[MaxInfoReport] Received clicked event: {message}");

        EnqueueMainThreadTask(() =>
        {
            try
            {
                var modifiedMessage = FillInstanceId(message);
                var adjust = SDK.GetInstance().adjustPlugin;
                adjust?.TrackEvent("ua_clicked", 0, modifiedMessage);

                DebugUtil.Log($"[MaxInfoReport] Report clicked event `{modifiedMessage} ok.");
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"[MaxInfoReport] Report clicked event failed: {exception.Message}");
            }
        });
    }

    #endregion

    #region Private Apis

    private static void EnqueueMainThreadTask(Action action)
    {
        Actions.Enqueue(action);
    }

    private void Update()
    {
        while (Actions.TryDequeue(out var action))
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"[MaxInfoReport] MainThreadTask failed: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    private static void OnInterstitialAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        OnLoaded("Interstitial", adInfo);
    }

    private static void OnInterstitialAdDisplayed(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        OnImpression("Interstitial", adInfo);
    }

    private static void OnInterstitialAdClicked(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        OnClicked("Interstitial", adInfo);
    }

    private static void OnRewardedAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        OnLoaded("Rewarded", adInfo);
    }

    private static void OnRewardedAdDisplayed(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        OnImpression("Rewarded", adInfo);
    }

    private static void OnRewardedAdClicked(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        OnClicked("Rewarded", adInfo);
    }

    private static void OnLoaded(string adType, MaxSdkBase.AdInfo adInfo)
    {
        DebugUtil.Log($"[MaxInfoReport] Received loaded event: {adInfo}");

        EnqueueMainThreadTask(() =>
        {
            try
            {
                var parameters = Convert(adType, adInfo);

                var adjust = SDK.GetInstance().adjustPlugin;
                adjust?.TrackEvent("ua_loaded", 0, parameters);

                DebugUtil.Log($"[MaxInfoReport] Report loaded event `{parameters}` ok.");
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"[MaxInfoReport] Report loaded event failed: {exception.Message}");
            }
        });
    }

    private static void OnImpression(string adType, MaxSdkBase.AdInfo adInfo)
    {
        DebugUtil.Log($"[MaxInfoReport] Received impression event: {adInfo}");

        EnqueueMainThreadTask(() =>
        {
            try
            {
                var parameters = Convert(adType, adInfo);

                var adjust = SDK.GetInstance().adjustPlugin;
                adjust?.TrackEvent("ua_impression", 0, parameters);

                DebugUtil.Log($"[MaxInfoReport] Report impression event `{parameters}` ok.");
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"[MaxInfoReport] Report impression event failed: {exception.Message}");
            }
        });
    }

    private static void OnClicked(string adType, MaxSdkBase.AdInfo adInfo)
    {
        DebugUtil.Log($"[MaxInfoReport] Received clicked event: {adInfo}");

        EnqueueMainThreadTask(() =>
        {
            try
            {
                var parameters = Convert(adType, adInfo);

                var adjust = SDK.GetInstance().adjustPlugin;
                adjust?.TrackEvent("ua_clicked", 0, parameters);

                DebugUtil.Log($"[MaxInfoReport] Report clicked event `{parameters} ok.");
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"[MaxInfoReport] Report clicked event failed: {exception.Message}");
            }
        });
    }

    private static string Convert(string adType, MaxSdkBase.AdInfo adInfo)
    {
        var result = new Dictionary<string, object>
        {
            { "name", "MAX" },
            { "ad_unit_type", adType },
            { "ad_unit_id", adInfo.AdUnitIdentifier },
            { "ecpm", adInfo.Revenue },
            { "ecpm_precision", "exact" },
            { "creative_id", adInfo.CreativeIdentifier },
            { "network_name", adInfo.NetworkName },
            { "network_placement", adInfo.NetworkPlacement },
            { "dsp_id", string.Empty },
            { "dsp_name", string.Empty },
            { "click_url", string.Empty },
            { "extra_data", new Dictionary<string, object>() },
            { "instance_id", _plugin.GetAdInstanceId(adInfo.AdUnitIdentifier) },
        };
        return JsonConvert.SerializeObject(result);
    }

    private static string FillInstanceId(string message)
    {
        var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
        
        if (result.TryGetValue("ad_unit_id", out var value))
        {
            if (value is string adUnitIdentifier)
            {
                result["instance_id"] = _plugin.GetAdInstanceId(adUnitIdentifier);
            }
        }

        return JsonConvert.SerializeObject(result);
    }

    #endregion
}