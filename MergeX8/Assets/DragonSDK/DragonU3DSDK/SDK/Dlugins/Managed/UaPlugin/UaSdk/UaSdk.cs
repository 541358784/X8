using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using File = System.IO.File;

namespace DragonPlus.Ad.UA
{
    public static class UaSdk
    {
        #region Properties

        private const long VideoOutputDirectorySizeLimitation = 1024 * 1024 * 200; // 200MB

        private const string VideoPlayerTemplatePath = "UAVideoPlayerTemplates";
        private const string VideoJsPath             = "video.min.js";
        private const string VideoJsCssPath          = "video-js.min.css";

        private static readonly string OutputPath = Path.Combine(Application.persistentDataPath, "ua");

        private static readonly string VideoPlayerTemplateOutputPath =
            Path.Combine(OutputPath, "video_player.html");

        private static readonly string VideoJsOutputPath =
            Path.Combine(OutputPath, VideoJsPath);

        private static readonly string VideoJsCssOutputPath =
            Path.Combine(OutputPath, VideoJsCssPath);


        private static readonly string VideoOutputPath = Path.Combine(OutputPath, "videos");

        private static bool   _isInitialized;
        private static string _adid     = string.Empty;
        private static string _gpsAdid  = string.Empty;
        private static string _fireAdid = string.Empty;
        private static string _idfa     = string.Empty;
        private static bool   _muted    = false;

        private static readonly List<UaAdvertisement> RewardVideoAds  = new();
        private static readonly List<UaAdvertisement> InterstitialAds = new();
        private static readonly List<UaAdvertisement> BannerAds       = new();

        private static bool _rewardVideoAdsRequesting  = false;
        private static bool _interstitialAdsRequesting = false;
        private static bool _bannerAdsRequesting       = false;

        private static readonly ConcurrentQueue<UaAdvertisementDownloadTask> DownloadTasks = new();
        private static          UaAdvertisementDownloadTask                  _currentDownloadTask;

        private static readonly HashSet<string> DownloadingAds = new();

        private const float RetryDelayWhenNoAds = 600.0f;

        #endregion

        #region Public Apis

        public static void InitializeSdk()
        {
#if DEVELOPMENT_BUILD
            UniWebViewLogger.Instance.LogLevel = UniWebViewLogger.Level.Verbose;
            UniWebView.SetForwardWebConsoleToNativeOutput(true);
            UniWebView.SetWebContentsDebuggingEnabled(true);
#endif
            var tasks = new List<Task> { FetchAdjustParameters() };

            CheckAndCleanAdsDirectory();
            SetupTemplates();

            UaSdkUtility.AddLooseUpdateListener(OnTickUpdate, 1f);

            Task.WhenAll(tasks.ToArray()).ContinueWith(_ =>
            {
                UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnSdkInitializedEvent);
                _isInitialized = true;
            });
        }

        public static bool IsInitialized()
        {
            return _isInitialized;
        }

        public static void SetTestDeviceAdvertisingIdentifiers(string[] advertisingIdentifiers)
        {
        }

        public static void SetMuted(bool muted)
        {
            _muted = muted;
        }

        public static bool IsMuted()
        {
            return _muted;
        }

        public static void SetUserId(string userId)
        {
        }


        public static string GetCountryCode()
        {
            try
            {
                return RegionInfo.CurrentRegion.TwoLetterISORegionName;
            }
#pragma warning disable 0168
            catch (Exception ignored)
#pragma warning restore 0168
            {
                // Ignored
            }

            return "US";
        }

        #region Rewarded Video

        public static bool IsRewardedAdReady(string rewardedAdUnitId, string instanceId = null)
        {
            lock (RewardVideoAds)
            {
                foreach (var ads in RewardVideoAds)
                {
                    if (ads.Loaded && (string.IsNullOrEmpty(instanceId) || ads.Id == instanceId))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static void ShowRewardedAd(string rewardedAdUnitId, string placementId, string instanceId)
        {
            UaAdvertisement advertisement = null;
            lock (RewardVideoAds)
            {
                for (var i = RewardVideoAds.Count - 1; i >= 0; i--)
                {
                    if (RewardVideoAds[i].Loaded && (string.IsNullOrEmpty(instanceId) || RewardVideoAds[i].Id == instanceId))
                    {
                        advertisement = RewardVideoAds[i];
                        RewardVideoAds.RemoveAt(i);
                        break;
                    }
                }
            }


            if (advertisement == null)
            {
                DebugUtil.LogError("[UaSdk] Rewarded video ads is not ready yet.");

                LoadRewardedAd(rewardedAdUnitId);
                return;
            }

            var videoName   = $"videos/{advertisement.VideoName}";
            var duration    = 30;
            var mutedString = _muted ? "true" : "false";

            UaSdkWebViewUtility.ShowWebView("ua/video_player.html", new UAAdsRewardVideoCallback
            {
                Advertisement = advertisement,
                Placement     = placementId,
                OnPageLoadedCallback = () =>
                {
                    var encodedImpressionUrl =
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(
                            AppendAdvertisingParameters(
                                advertisement.Advertisement.AdjustViewUrl,
                                advertisement.Id)));
                    var encodedClickUrl =
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(
                            AppendAdvertisingParameters(
                                advertisement.Advertisement.AdjustClickUrl,
                                advertisement.Id)));
                    UaSdkWebViewUtility.Call(
                        $"playVideo('{videoName}', {duration}, {mutedString}, '{encodedImpressionUrl}', '{encodedClickUrl}')");
                }
            });
        }

        public static void LoadRewardedAd(string rewardedAdUnitId)
        {
            lock (RewardVideoAds)
            {
                foreach (var ads in RewardVideoAds)
                {
                    if (ads.Loaded || DownloadingAds.Contains(ads.Id))
                    {
                        return;
                    }
                }
            }

            if (_rewardVideoAdsRequesting)
            {
                return;
            }

            try
            {
                DebugUtil.Log($"[UaSdk] Request more rewarded ads for {rewardedAdUnitId}");

                var request = new CGetAdvertisement()
                {
                    AppStore      = GetCurrentAppStore(),
                    DeliveryRange = "1",
                };

                if (!FillExtraRequestParams(request))
                {
                    UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnRewardedAdLoadFailedEvent,
                        new Dictionary<string, string>());
                    return;
                }

                _rewardVideoAdsRequesting = true;

                APIManager.Instance.Send<CGetAdvertisement, SGetAdvertisement>(request, response =>
                {
                    _rewardVideoAdsRequesting = false;

                    var validCount = 0;

                    foreach (var ads in response.Advertisement)
                    {
                        if (string.IsNullOrEmpty(ads.AdUrl))
                        {
                            DebugUtil.LogWarning($"[UaSdk] Empty advertisement URL for {ads.AdId}");
                            continue;
                        }

                        var uaAds = new UaAdvertisement()
                        {
                            Advertisement    = ads,
                            Loaded           = false,
                            Id               = Guid.NewGuid().ToString(),
                            Type             = UaAdvertisementType.RewardVideo,
                            VideoName        = $"{CalculateMD5Hash(ads.AdUrl)}.mp4",
                            AdUnitIdentifier = rewardedAdUnitId,
                        };

                        if (ads.ExtraData != null)
                        {
                            DebugUtil.Log($"[UaSdk] Ads with extra data {ads.ExtraData.ModelId}");

                            uaAds.ExtraData = JsonConvert.SerializeObject(new Dictionary<string, object>
                            {
                                { "model_id", ads.ExtraData.ModelId },
                                { "predict_model_id", ads.ExtraData.PredictModelId },
                                { "predict_rate", ads.ExtraData.PredictRate },
                                { "predict_weight", ads.ExtraData.PredictWeight },
                                { "predict_ua_origin_price", ads.ExtraData.PredictUaOriginPrice },
                                { "recommend_model_id", ads.ExtraData.RecommendModelId },
                            });
                            
                            DebugUtil.Log($"[UaSdk] Ads with extra data {uaAds.ExtraData}");
                        }
                        else
                        {
                            DebugUtil.Log($"[UaSdk] Ads without extra data");

                            uaAds.ExtraData = "{}";
                        }
                        
                        lock (RewardVideoAds)
                        {
                            RewardVideoAds.Add(uaAds);
                        }

                        Download(uaAds);

                        validCount++;
                    }

                    DebugUtil.Log($"[UaSdk] {validCount} more rewarded ads requested");

                    if (validCount <= 0)
                    {
                        DebugUtil.Log($"[UaSdk] No rewarded ads valid, retry in {RetryDelayWhenNoAds} seconds.");

                        UaSdkUtility.DelayCall(() => { LoadRewardedAd(rewardedAdUnitId); }, RetryDelayWhenNoAds);
                    }
                }, (errorCode, message, _) =>
                {
                    DebugUtil.LogError($"[UaSdk] Request for rewarded ads failed: {errorCode} - {message}");

                    _rewardVideoAdsRequesting = false;
                    UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnRewardedAdLoadFailedEvent,
                        GenerateAdvertisementProperties(rewardedAdUnitId, UaAdvertisementType.RewardVideo));
                });
            }
            catch (Exception exception)
            {
                DebugUtil.LogError(
                    $"[UASdk] Request more rewarded ads failed: {exception.Message}\n{exception.StackTrace}");

                _rewardVideoAdsRequesting = false;

                UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnRewardedAdLoadFailedEvent,
                    GenerateAdvertisementProperties(rewardedAdUnitId, UaAdvertisementType.RewardVideo));
            }
        }

        #endregion

        #region Interstitial

        public static bool IsInterstitialReady(string interstitialAdUnitId, string instanceId = null)
        {
            lock (InterstitialAds)
            {
                foreach (var ads in InterstitialAds)
                {
                    if (ads.Loaded && (string.IsNullOrEmpty(instanceId) || ads.Id == instanceId))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static void ShowInterstitial(string interstitialAdUnitId, string placementId, string instanceId = null)
        {
            UaAdvertisement advertisement = null;
            lock (InterstitialAds)
            {
                for (var i = InterstitialAds.Count - 1; i >= 0; i--)
                {
                    if (InterstitialAds[i].Loaded && (string.IsNullOrEmpty(instanceId) || InterstitialAds[i].Id == instanceId))
                    {
                        advertisement = InterstitialAds[i];
                        InterstitialAds.RemoveAt(i);
                        break;
                    }
                }
            }


            if (advertisement == null)
            {
                DebugUtil.LogError("[UaSdk] Interstitial video ads is not ready yet.");

                LoadInterstitial(interstitialAdUnitId);
                return;
            }

            var videoName   = $"videos/{advertisement.VideoName}";
            var duration    = 5;
            var mutedString = _muted ? "true" : "false";

            UaSdkWebViewUtility.ShowWebView("ua/video_player.html", new UAAdsInterstitialCallback
            {
                Advertisement = advertisement,
                Placement     = placementId,
                OnPageLoadedCallback = () =>
                {
                    var encodedImpressionUrl =
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(
                            AppendAdvertisingParameters(
                                advertisement.Advertisement.AdjustViewUrl,
                                advertisement.Id)));
                    var encodedClickUrl =
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(
                            AppendAdvertisingParameters(
                                advertisement.Advertisement.AdjustClickUrl,
                                advertisement.Id)));
                    UaSdkWebViewUtility.Call(
                        $"playVideo('{videoName}', {duration}, {mutedString}, '{encodedImpressionUrl}', '{encodedClickUrl}')");
                }
            });
        }

        public static void LoadInterstitial(string interstitialAdUnitId)
        {
            lock (InterstitialAds)
            {
                foreach (var ads in InterstitialAds)
                {
                    if (ads.Loaded || DownloadingAds.Contains(ads.Id))
                    {
                        return;
                    }
                }
            }

            if (_interstitialAdsRequesting)
            {
                return;
            }

            try
            {
                DebugUtil.Log($"[UaSdk] Request more interstitial ads for {interstitialAdUnitId}");

                var request = new CGetAdvertisement()
                {
                    AppStore      = GetCurrentAppStore(),
                    DeliveryRange = "2",
                };

                if (!FillExtraRequestParams(request))
                {
                    UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnInterstitialLoadFailedEvent,
                        new Dictionary<string, string>());
                    return;
                }

                _interstitialAdsRequesting = true;

                APIManager.Instance.Send<CGetAdvertisement, SGetAdvertisement>(request, response =>
                {
                    _interstitialAdsRequesting = false;

                    var validCount = 0;

                    foreach (var ads in response.Advertisement)
                    {
                        if (string.IsNullOrEmpty(ads.AdUrl))
                        {
                            DebugUtil.LogWarning($"[UaSdk] Empty advertisement URL for {ads.AdId}");
                            continue;
                        }

                        var uaAds = new UaAdvertisement()
                        {
                            Advertisement    = ads,
                            Loaded           = false,
                            Id               = Guid.NewGuid().ToString(),
                            Type             = UaAdvertisementType.Interstitial,
                            VideoName        = $"{CalculateMD5Hash(ads.AdUrl)}.mp4",
                            AdUnitIdentifier = interstitialAdUnitId,
                        };

                        if (ads.ExtraData != null)
                        {
                            DebugUtil.Log($"[UaSdk] Ads with extra data {ads.ExtraData.ModelId}");

                            uaAds.ExtraData = JsonConvert.SerializeObject(new Dictionary<string, object>
                            {
                                { "model_id", ads.ExtraData.ModelId },
                                { "predict_model_id", ads.ExtraData.PredictModelId },
                                { "predict_rate", ads.ExtraData.PredictRate },
                                { "predict_weight", ads.ExtraData.PredictWeight },
                                { "predict_ua_origin_price", ads.ExtraData.PredictUaOriginPrice },
                                { "recommend_model_id", ads.ExtraData.RecommendModelId },
                            });
                            
                            DebugUtil.Log($"[UaSdk] Ads with extra data {uaAds.ExtraData}");
                        }
                        else
                        {
                            DebugUtil.Log($"[UaSdk] Ads without extra data");

                            uaAds.ExtraData = "{}";
                        }
                        
                        lock (InterstitialAds)
                        {
                            InterstitialAds.Add(uaAds);
                        }

                        Download(uaAds);

                        validCount++;
                    }

                    DebugUtil.Log($"[UaSdk] {validCount} more interstitial ads requested");

                    if (validCount <= 0)
                    {
                        DebugUtil.Log($"[UaSdk] No interstitial ads valid, retry in {RetryDelayWhenNoAds} seconds.");

                        UaSdkUtility.DelayCall(() => LoadInterstitial(interstitialAdUnitId), RetryDelayWhenNoAds);
                    }
                }, (errorCode, message, _) =>
                {
                    DebugUtil.LogError($"[UaSdk] Request for interstitial ads failed: {errorCode} - {message}");

                    _interstitialAdsRequesting = false;
                    UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnInterstitialLoadFailedEvent,
                        GenerateAdvertisementProperties(interstitialAdUnitId, UaAdvertisementType.Interstitial));
                });
            }
            catch (Exception exception)
            {
                DebugUtil.LogError(
                    $"[UASdk] Request more interstitial ads failed: {exception.Message}\n{exception.StackTrace}");

                _interstitialAdsRequesting = false;
                UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnInterstitialLoadFailedEvent,
                    GenerateAdvertisementProperties(interstitialAdUnitId, UaAdvertisementType.Interstitial));
            }
        }

        private static string GetCurrentAppStore()
        {
#if SUB_CHANNEL_AMAZON
            return "3";
#else
            return Application.platform switch
            {
                RuntimePlatform.IPhonePlayer => "1",
                RuntimePlatform.Android      => "2",
                _                            => "0"
            };
#endif
        }

        #endregion

        #region Banner

        public static void ShowBanner(string bannerAdUnitId)
        {
            // TODO
        }

        public static void HideBanner(string bannerAdUnitId)
        {
            // TODO
        }

        public static float GetAdaptiveBannerHeight()
        {
            // TODO
            return 0f;
        }

        public static void UpdateBannerPosition(float x, float y)
        {
            // TODO
        }

        public static void SetBannerWidth(string bannerAdUnitId, float width)
        {
            // TODO
        }

        public static void CreateBanner(string bannerAdUnitId, UaBannerPosition position)
        {
            // TODO
        }

        public static void SetBannerBackgroundColor(string bannerAdUnitId, Color color)
        {
            // TODO
        }

        public static void SetBannerPlacement(string bannerAdUnitId, string placementId)
        {
            // TODO
        }

        #endregion

        #endregion

        #region Private Apis

        private sealed class UAAdsRewardVideoCallback : UAAdsEventCallback
        {
            protected override void OnImpression()
            {
                UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnRewardedAdDisplayedEvent,
                    GenerateAdvertisementProperties(Advertisement, Placement));
            }

            protected override void OnClicked()
            {
                UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnRewardedAdClickedEvent,
                    GenerateAdvertisementProperties(Advertisement, Placement));
            }

            protected override void OnClosed()
            {
                UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnRewardedAdHiddenEvent,
                    GenerateAdvertisementProperties(Advertisement, Placement));
            }

            protected override void OnRewarded()
            {
                UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnRewardedAdReceivedRewardEvent,
                    GenerateAdvertisementProperties(Advertisement, Placement));
            }
        }

        private sealed class UAAdsInterstitialCallback : UAAdsEventCallback
        {
            protected override void OnImpression()
            {
                UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnInterstitialDisplayedEvent,
                    GenerateAdvertisementProperties(Advertisement, Placement));
            }

            protected override void OnClicked()
            {
                UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnInterstitialClickedEvent,
                    GenerateAdvertisementProperties(Advertisement, Placement));
            }

            protected override void OnClosed()
            {
                UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnInterstitialHiddenEvent,
                    GenerateAdvertisementProperties(Advertisement, Placement));
            }

            protected override void OnRewarded()
            {
            }
        }

        private abstract class UAAdsEventCallback : IUaSdkWebViewEventCallback
        {
            #region Properties

            public Action          OnPageLoadedCallback { get; set; }
            public UaAdvertisement Advertisement        { get; set; }
            public string          Placement            { get; set; }

            #endregion endregion

            #region Public Apis

            public void OnEvent(string eventName, Dictionary<string, string> parameters)
            {
                DebugUtil.Log($"[UASdk] WebView event received: {eventName}");

                switch (eventName)
                {
                    case "OnUASdkAdsShown":
                        OnImpression();
                        break;

                    case "OnUASdkAdsClosed":
                        OnClosed();
                        UaSdkWebViewUtility.HideWebView();
                        break;

                    case "OnUASdkAdsClicked":
                        OnClicked();
                        break;

                    case "OnUASdkAdsRewarded":
                        OnRewarded();
                        break;
                }
            }

            public void OnPageLoaded()
            {
                OnPageLoadedCallback?.Invoke();
                OnPageLoadedCallback = null;
            }

            #endregion

            #region Abstract Apis

            protected abstract void OnImpression();
            protected abstract void OnClicked();
            protected abstract void OnClosed();
            protected abstract void OnRewarded();

            #endregion
        }

        private static void CheckAndCleanAdsDirectory()
        {
            try
            {
                var directoryPath = VideoOutputPath;
                if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
                {
                    return;
                }

                var directoryInfo = new DirectoryInfo(directoryPath);
                var directorySize = GetDirectorySize(directoryInfo);
                if (directorySize < VideoOutputDirectorySizeLimitation)
                {
                    return;
                }

                DebugUtil.Log(
                    $"[UaSdk] Video output path size {directorySize / 1024 / 1024}MB exceed limits {VideoOutputDirectorySizeLimitation / 1024 / 1024}MB.");

                CleanOldFilesUntilUnderLimit(directoryInfo, directorySize);
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"Check and clean ads directory failed: {exception.Message}");
            }
        }

        private static long GetDirectorySize(DirectoryInfo directoryInfo)
        {
            // 计算所有文件的大小
            var files = directoryInfo.GetFiles();
            var size  = files.Sum(file => file.Length);

            // 递归计算子目录的大小
            var directories = directoryInfo.GetDirectories();
            size += directories.Sum(GetDirectorySize);

            return size;
        }

        private static void CleanOldFilesUntilUnderLimit(DirectoryInfo directoryInfo, long currentSize)
        {
            const long sizeLimitBytes = VideoOutputDirectorySizeLimitation / 2;

            // 获取所有文件并排序，根据最后访问时间升序排列
            var files = directoryInfo.GetFiles();
            Array.Sort(files, (x, y) => x.LastAccessTime.CompareTo(y.LastAccessTime));

            var removedSize      = 0L;
            var removedFileCount = 0;

            foreach (var file in files)
            {
                if (currentSize <= sizeLimitBytes)
                    break;

                try
                {
                    var fileSize = file.Length;
                    file.Delete();
                    currentSize -= fileSize;

                    removedSize += fileSize;
                    removedFileCount++;

                    DebugUtil.Log($"[UaSdk] Remove old file: {file.Name}");
                }
                catch (Exception exception)
                {
                    DebugUtil.LogError($"[UaSdk] Can't delete file {file.Name}: {exception.Message}");
                }
            }

            DebugUtil.Log($"[UaSdk] Removed {removedFileCount} file(s), freed {removedSize / 1024 / 1024}MB");

            if (currentSize > sizeLimitBytes)
            {
                DebugUtil.Log($"[UaSdk] Size exceed limits after removal: {currentSize / 1024 / 1024}MB");
            }
        }

        private static void SetupTemplates()
        {
            if (!Directory.Exists(OutputPath))
            {
                Directory.CreateDirectory(OutputPath);
            }

            if (!Directory.Exists(VideoOutputPath))
            {
                Directory.CreateDirectory(VideoOutputPath);
            }

            CopyTemplates(VideoPlayerTemplatePath, VideoPlayerTemplateOutputPath);
            CopyTemplates(VideoJsPath,             VideoJsOutputPath);
            CopyTemplates(VideoJsCssPath,          VideoJsCssOutputPath);
        }

        private static void CopyTemplates(string assetPath, string outputPath)
        {
            var asset = Resources.Load<TextAsset>($"ua/{assetPath}");
            if (asset != null)
            {
                File.WriteAllText(outputPath, asset.text);
            }
        }

        private static Task FetchAdjustParameters()
        {
            var taskSource = new TaskCompletionSource<bool>();

#if UNITY_EDITOR
            taskSource.SetResult(true);
            return taskSource.Task;
#else
            _adid = com.adjust.sdk.Adjust.getAdid();

            switch (Application.platform)
            {
                case RuntimePlatform.Android:

#if SUB_CHANNEL_AMAZON
                    _fireAdid = com.adjust.sdk.Adjust.getAmazonAdId();
                    taskSource.SetResult(true);
#else
                    com.adjust.sdk.Adjust.getGoogleAdId(gpsAdid =>
                    {
                        _gpsAdid = gpsAdid;
                    });

                    taskSource.SetResult(true);
#endif
                    break;

                case RuntimePlatform.IPhonePlayer:
                    _idfa = com.adjust.sdk.Adjust.getIdfa();
                    taskSource.SetResult(true);
                    break;

                default:
                    taskSource.SetResult(true);
                    break;
            }

            return taskSource.Task;
#endif
        }

        private static void Download(UaAdvertisement advertisement)
        {
            DownloadTasks.Enqueue(new UaAdvertisementDownloadTask()
            {
                Advertisement = advertisement,
                IsDownloading = false,
                IsCompleted   = false,
                IsFailed      = false,
            });

            DownloadingAds.Add(advertisement.Id);
        }

        private static void DownloadWithRetry(UaAdvertisementDownloadTask task, int retryCount)
        {
            var advertisement = task.Advertisement;
            var outputPath    = Path.Combine(VideoOutputPath, advertisement.VideoName);
            if (File.Exists(outputPath))
            {
                OnVideoLoaded();
                return;
            }

            DebugUtil.Log($"[UaSdk] Start downloading advertisement {advertisement.Advertisement.AdUrl}");

            UaSdkUtility.StartCoroutine(DownloadVideo());

            return;

            IEnumerator DownloadVideo()
            {
                var request = UnityWebRequest.Get(advertisement.Advertisement.AdUrl);

                try
                {
                    yield return request.SendWebRequest();

                    var downloadHandler = request.downloadHandler;

                    if (string.IsNullOrEmpty(request.error) && downloadHandler.data != null)
                    {
                        DebugUtil.Log($"[UaSdk] Advertisement download result: OK");

                        var tempOutputPath = outputPath + ".tmp";
                        File.WriteAllBytes(tempOutputPath, downloadHandler.data);

                        if (File.Exists(outputPath))
                        {
                            File.Delete(outputPath);
                        }

                        File.Move(tempOutputPath, outputPath);

                        OnVideoLoaded();
                    }
                    else
                    {
                        DebugUtil.Log($"[UaSdk] Advertisement download result: {request.error}");

                        var nextRetryCount = retryCount - 1;

                        if (nextRetryCount > 0)
                        {
                            UaSdkUtility.DelayCall(() => DownloadWithRetry(task, nextRetryCount), 5.0f);
                        }
                        else
                        {
                            task.IsCompleted   = true;
                            task.IsDownloading = false;
                            task.IsFailed      = true;
                        }
                    }
                }
                finally
                {
                    request?.Dispose();
                }
            }

            void OnVideoLoaded()
            {
                task.IsCompleted   = true;
                task.IsDownloading = false;
            }
        }

        private static string CalculateMD5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            using var md5        = System.Security.Cryptography.MD5.Create();
            var       inputBytes = Encoding.ASCII.GetBytes(input);
            var       hashBytes  = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string prior to .NET 5
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        private static void OnTickUpdate()
        {
            if (_currentDownloadTask == null)
            {
                DownloadTasks.TryDequeue(out _currentDownloadTask);
            }

            if (_currentDownloadTask == null) return;

            if (_currentDownloadTask.IsCompleted)
            {
                var advertisement = _currentDownloadTask.Advertisement;

                if (_currentDownloadTask.IsFailed)
                {
                    switch (advertisement.Type)
                    {
                        case UaAdvertisementType.Banner:
                        {
                            int bannerCount;

                            lock (BannerAds)
                            {
                                BannerAds.Remove(advertisement);
                                bannerCount = BannerAds.Count;
                            }

                            if (bannerCount <= 0)
                            {
                                UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnBannerAdLoadFailedEvent,
                                    GenerateAdvertisementProperties(advertisement, string.Empty));
                            }

                            break;
                        }

                        case UaAdvertisementType.Interstitial:
                        {
                            int interstitialCount;

                            lock (InterstitialAds)
                            {
                                InterstitialAds.Remove(advertisement);
                                interstitialCount = InterstitialAds.Count;
                            }

                            if (interstitialCount <= 0)
                            {
                                UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnInterstitialLoadFailedEvent,
                                    GenerateAdvertisementProperties(advertisement, string.Empty));
                            }

                            break;
                        }

                        case UaAdvertisementType.RewardVideo:
                        {
                            int rewardVideoCount;

                            lock (RewardVideoAds)
                            {
                                RewardVideoAds.Remove(advertisement);
                                rewardVideoCount = RewardVideoAds.Count;
                            }

                            if (rewardVideoCount <= 0)
                            {
                                UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnInterstitialLoadFailedEvent,
                                    GenerateAdvertisementProperties(advertisement, string.Empty));
                            }

                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    advertisement.Loaded = true;

                    DebugUtil.Log($"[UaSdk] Advertisement {advertisement.Advertisement.AdUrl} loaded");

                    switch (advertisement.Type)
                    {
                        case UaAdvertisementType.RewardVideo:
                        {
                            UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnRewardedAdLoadedEvent,
                                GenerateAdvertisementProperties(advertisement, string.Empty));
                            break;
                        }

                        case UaAdvertisementType.Interstitial:
                        {
                            UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnInterstitialLoadedEvent,
                                GenerateAdvertisementProperties(advertisement, string.Empty));
                            break;
                        }

                        case UaAdvertisementType.Banner:
                        {
                            UaSdkCallbacks.ForwardEvent(UaSdkCallbackEvents.OnBannerAdLoadedEvent,
                                GenerateAdvertisementProperties(advertisement, string.Empty));
                            break;
                        }

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                _currentDownloadTask = null;

                DownloadingAds.Remove(advertisement.Id);
            }
            else if (!_currentDownloadTask.IsDownloading)
            {
                _currentDownloadTask.IsDownloading = true;
                DownloadWithRetry(_currentDownloadTask, 5);
            }
        }

        private static bool FillExtraRequestParams(CGetAdvertisement request)
        {
#if SUB_CHANNEL_AMAZON
            if (string.IsNullOrEmpty(_fireAdid))
            {
                _fireAdid = com.adjust.sdk.Adjust.getAmazonAdId() ?? string.Empty;

                if (string.IsNullOrEmpty(_fireAdid))
                {
                    DebugUtil.Log($"[UaSdk] Amazon adid is null, retry later.");

                    return false;
                }
            }
#else
            if (string.IsNullOrEmpty(_adid))
            {
                _adid = com.adjust.sdk.Adjust.getAdid() ?? string.Empty;

                if (string.IsNullOrEmpty(_adid))
                {
                    DebugUtil.Log($"[UaSdk] Adjust adid is null, retry later.");

                    return false;
                }
            }
#endif

            request.DeviceId    = SystemInfo.deviceUniqueIdentifier;
            request.DeviceModel = UnityWebRequest.EscapeURL(SystemInfo.deviceModel);
            request.DeviceType  = UnityWebRequest.EscapeURL(SystemInfo.deviceType.ToString());
            request.DeviceName  = UnityWebRequest.EscapeURL(SystemInfo.deviceName);
            request.AdId        = _adid    ?? string.Empty;
            request.GaId        = _gpsAdid ?? string.Empty;
            request.IdFa        = _idfa    ?? string.Empty;
            request.ViewWidth   = Screen.width;
            request.ViewHeight  = Screen.height;

            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
            request.PlayerId = (long)storageCommon.PlayerId;
            request.Country  = UnityWebRequest.EscapeURL(storageCommon.Country  ?? string.Empty);
            request.Region   = UnityWebRequest.EscapeURL(storageCommon.Region   ?? string.Empty);
            request.TimeZone = UnityWebRequest.EscapeURL(storageCommon.TimeZone ?? string.Empty);

            return true;
        }

        private static Dictionary<string, string> GenerateAdvertisementProperties(
            string              adUnitIdentifier,
            UaAdvertisementType adType)

        {
            var properties = new Dictionary<string, string>
            {
                { "AdUnitIdentifier", adUnitIdentifier },
                { "NetworkName", "UA" },
                { "Revenue", "0.0" },
                { "RevenuePrecision", "exact" },
            };

            switch (adType)
            {
                case UaAdvertisementType.RewardVideo:
                    properties.Add("AdType", "RewardedVideo");
                    break;

                case UaAdvertisementType.Interstitial:
                    properties.Add("AdType", "Interstitial");
                    break;
            }

            return properties;
        }

        private static Dictionary<string, string> GenerateAdvertisementProperties(
            UaAdvertisement advertisement,
            string          placement)
        {
            var properties = new Dictionary<string, string>
            {
                { "Id", advertisement.Id },
                { "AdUnitIdentifier", advertisement.AdUnitIdentifier },
                { "NetworkName", "UA" },
                { "Revenue", advertisement.Advertisement.AdRevenue.ToString(CultureInfo.InvariantCulture) },
                { "RevenuePrecision", "exact" },
                { "Placement", placement },
                { "CreativeIdentifier", advertisement.Advertisement.AdId },
#if UNITY_IOS
                { "AppUrl", advertisement.Advertisement.AppleStoreUrl },
#endif
#if UNITY_ANDROID
                { "AppUrl", advertisement.Advertisement.GoogleStoreUrl },
#endif
                { "ExtraData", advertisement.ExtraData },
            };

            switch (advertisement.Type)
            {
                case UaAdvertisementType.RewardVideo:
                    properties.Add("AdType", "RewardedVideo");
                    break;

                case UaAdvertisementType.Interstitial:
                    properties.Add("AdType", "Interstitial");
                    break;
            }

            return properties;
        }

        private static string AppendAdvertisingParameters(string url, string advertisementId)
        {
            var parameterList = new List<string>();

            if (!url.Contains("adid=") && !string.IsNullOrEmpty(_adid))
            {
                parameterList.Add($"adid={_adid}");
            }

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    if (!url.Contains("gps_adid=") && !string.IsNullOrEmpty(_gpsAdid))
                    {
                        parameterList.Add($"gps_adid={_gpsAdid}");
                    }

                    if (!url.Contains("fire_adid=") && !string.IsNullOrEmpty(_fireAdid))
                    {
                        parameterList.Add($"fire_adid={_fireAdid}");
                    }

                    break;

                case RuntimePlatform.IPhonePlayer:
                    if (!url.Contains("idfa="))
                    {
                        parameterList.Add($"idfa={_idfa}");
                    }

                    break;
            }

            parameterList.Add($"label={advertisementId}");
            
            if (parameterList.Count <= 0)
            {
                DebugUtil.Log($"[UaSdk] Final url: {url}.");
                return url;
            }

            var parameters = string.Join("&", parameterList);
            var finalUrl = url.Contains("?") ? $"{url}&{parameters}" : $"{url}?{parameters}";
            
            DebugUtil.Log($"[UaSdk] Final url: {finalUrl}.");

            return finalUrl;
        }

        #endregion
    }
}