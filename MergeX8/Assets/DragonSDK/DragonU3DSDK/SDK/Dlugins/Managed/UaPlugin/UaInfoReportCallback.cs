using System;
using System.Collections.Generic;
using Dlugin;
using DragonPlus.Ad.UA;
using DragonU3DSDK;
using Newtonsoft.Json;

internal class UaInfoReportCallback
{
    public static void Setup()
    {
        UaSdkCallbacks.Interstitial.OnAdLoadedEvent    -= OnInterstitialLoadedEvent;
        UaSdkCallbacks.Interstitial.OnAdDisplayedEvent -= OnInterstitialDisplayedEvent;
        UaSdkCallbacks.Interstitial.OnAdClickedEvent   -= OnInterstitialClickedEvent;

        UaSdkCallbacks.Rewarded.OnAdLoadedEvent    -= OnRewardedAdLoadedEvent;
        UaSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnRewardedAdDisplayedEvent;
        UaSdkCallbacks.Rewarded.OnAdClickedEvent   -= OnRewardedAdClickedEvent;

        UaSdkCallbacks.Interstitial.OnAdLoadedEvent    += OnInterstitialLoadedEvent;
        UaSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        UaSdkCallbacks.Interstitial.OnAdClickedEvent   += OnInterstitialClickedEvent;

        UaSdkCallbacks.Rewarded.OnAdLoadedEvent    += OnRewardedAdLoadedEvent;
        UaSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        UaSdkCallbacks.Rewarded.OnAdClickedEvent   += OnRewardedAdClickedEvent;
    }

    private static void OnInterstitialLoadedEvent(string adUnitId, UaAdInfo adInfo)
    {
        OnLoaded("Interstitial", adInfo);
    }

    private static void OnInterstitialDisplayedEvent(string adUnitId, UaAdInfo adInfo)
    {
        OnImpression("Interstitial", adInfo);
    }

    private static void OnInterstitialClickedEvent(string adUnitId, UaAdInfo adInfo)
    {
        OnClicked("Interstitial", adInfo);
    }

    private static void OnRewardedAdLoadedEvent(string adUnitId, UaAdInfo adInfo)
    {
        OnLoaded("Rewarded", adInfo);
    }

    private static void OnRewardedAdDisplayedEvent(string adUnitId, UaAdInfo adInfo)
    {
        OnImpression("Rewarded", adInfo);
    }

    private static void OnRewardedAdClickedEvent(string adUnitId, UaAdInfo adInfo)
    {
        OnClicked("Rewarded", adInfo);
    }


    private static void OnLoaded(string adType, UaAdInfo adInfo)
    {
        DebugUtil.Log($"[UaInfoReport] Received loaded event: {adInfo}");

        EnqueueMainThreadTask(() =>
        {
            try
            {
                var parameters = Convert(adType, adInfo);
                var adjust     = SDK.GetInstance().adjustPlugin;
                adjust?.TrackEvent("ua_loaded", 0, parameters);

                DebugUtil.Log($"[UaInfoReport] Report loaded event `{parameters}` ok.");
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"[UaInfoReport] Report loaded event failed: {exception.Message}");
            }
        });
    }

    private static void OnImpression(string adType, UaAdInfo adInfo)
    {
        DebugUtil.Log($"[UaInfoReport] Received impression event: {adInfo}");

        EnqueueMainThreadTask(() =>
        {
            try
            {
                var parameters = Convert(adType, adInfo);
                var adjust     = SDK.GetInstance().adjustPlugin;
                adjust?.TrackEvent("ua_impression", 0, parameters);

                DebugUtil.Log($"[UaInfoReport] Report impression event `{parameters}` ok.");
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"[UaInfoReport] Report impression event failed: {exception.Message}");
            }
        });
    }

    private static void OnClicked(string adType, UaAdInfo adInfo)
    {
        DebugUtil.Log($"[UaInfoReport] Received clicked event: {adInfo}");

        EnqueueMainThreadTask(() =>
        {
            try
            {
                var parameters = Convert(adType, adInfo);
                var adjust     = SDK.GetInstance().adjustPlugin;
                adjust?.TrackEvent("ua_clicked", 0, parameters);

                DebugUtil.Log($"[UaInfoReport] Report clicked event `{parameters} ok.");
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"[UaInfoReport] Report clicked event failed: {exception.Message}");
            }
        });
    }

    private static void EnqueueMainThreadTask(Action callback)
    {
        DelayActionManager.Instance.DebounceInMainThread(Guid.NewGuid().ToString(), 1, callback);
    }

    private static string Convert(string adType, UaAdInfo adInfo)
    {
        var result = new Dictionary<string, object>
        {
            { "name", "UA" },
            { "ad_unit_type", adType },
            { "ad_unit_id", adInfo.AdUnitIdentifier },
            { "ecpm", adInfo.Revenue },
            { "ecpm_precision", "exact" },
            { "creative_id", adInfo.CreativeIdentifier },
            { "network_name", adInfo.NetworkName },
            { "network_placement", adInfo.NetworkPlacement },
            { "dsp_id", string.Empty },
            { "dsp_name", string.Empty },
            { "click_url", adInfo.AppUrl },
            { "instance_id", adInfo.InstanceId },
        };

        try
        {
            result.Add("extra_data", !string.IsNullOrEmpty(adInfo.ExtraData)
                ? JsonConvert.DeserializeObject(adInfo.ExtraData)
                : new Dictionary<string, object>());
        }
        catch (Exception exception)
        {
            DebugUtil.LogError(exception.Message);
        }

        return JsonConvert.SerializeObject(result);
    }
}