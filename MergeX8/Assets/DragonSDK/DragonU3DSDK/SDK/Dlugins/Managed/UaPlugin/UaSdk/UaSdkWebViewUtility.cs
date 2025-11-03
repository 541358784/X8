using DragonU3DSDK;
using UnityEngine;

namespace DragonPlus.Ad.UA
{
    internal static class UaSdkWebViewUtility
    {
        #region Properties

        private static UniWebView CurrentWebView { get; set; }
        private static bool       CallbackAdded  { get; set; }

        #endregion

        #region Public Apis

        public static void ShowWebView(string path, IUaSdkWebViewEventCallback callback)
        {
            if (!CallbackAdded)
            {
                UaSdkUtility.AddApplicationPauseCallback(pauseStatus =>
                {
                    var value = pauseStatus ? "true" : "false";
                    Call($"OnApplicationPause({value});");
                });
                
                CallbackAdded = true;
            }

            UniWebView.SetAllowInlinePlay(true);
            UniWebView.SetAllowAutoPlay(true);
            UniWebView.SetJavaScriptEnabled(true);
            UniWebView.SetForwardWebConsoleToNativeOutput(true);

            if (CurrentWebView)
            {
                Object.DestroyImmediate(CurrentWebView.gameObject);

                CurrentWebView = null;
            }

            var systemWidth  = Display.main.systemWidth;
            var systemHeight = Display.main.systemHeight;

            var webViewObject = new GameObject("[WebViewObject]");

            CurrentWebView = webViewObject.AddComponent<UniWebView>();
            CurrentWebView.SetContentInsetAdjustmentBehavior(UniWebViewContentInsetAdjustmentBehavior.Never);
            CurrentWebView.SetDragInteractionEnabled(false);
            CurrentWebView.SetWindowUserResizeEnabled(false);
            CurrentWebView.Frame = new Rect(0, 0, systemWidth, systemHeight);
            CurrentWebView.OnMessageReceived += (_, message) =>
            {
                DebugUtil.Log($"[UASdk] Message from WebView: {message.Path}, {message.Args}");
                callback?.OnEvent(message.Path, message.Args);
            };
            CurrentWebView.OnShouldClose += _ =>
            {
                CurrentWebView = null;
                return true;
            };
            CurrentWebView.OnOrientationChanged += (_, _) =>
            {
                CurrentWebView.Frame = new Rect(0, 0, Screen.width, Screen.height);
            };
            CurrentWebView.RegisterShouldHandleRequest((request) =>
            {
                var url = request.Url;
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                    {
                        if (url.StartsWith("market://details"))
                        {
                            UaSdkUtility.EnqueueMainThreadTask(() =>
                            {
                                if (!JumpToMarket(url))
                                {
                                    Application.OpenURL(url);
                                }
                            });

                            return false;
                        }

                        if (url.StartsWith("https://play.google.com/store/apps/details"))
                        {
                            UaSdkUtility.EnqueueMainThreadTask(() =>
                            {
                                if (!JumpToGooglePlay(url))
                                {
                                    Application.OpenURL(url);
                                }
                            });

                            return false;
                        }

                        if (url.StartsWith("http://www.amazon.com/") ||
                            url.StartsWith("https://www.amazon.com/"))
                        {
                            UaSdkUtility.EnqueueMainThreadTask(() => { Application.OpenURL(url); });

                            return false;
                        }

                        break;
                    }

                    case RuntimePlatform.IPhonePlayer:
                    {
                        if (url.StartsWith("https://apps.apple.com/"))
                        {
                            UaSdkUtility.EnqueueMainThreadTask(() =>
                            {
                                DragonNativeBridge.OpenAppStoreRate(url.Replace(
                                    "https://apps.apple.com/",
                                    "itms-apps://itunes.apple.com/"));
                            });

                            return false;
                        }

                        break;
                    }
                }

                return true;
            });
            CurrentWebView.OnPageFinished += (_, _, _) => { callback?.OnPageLoaded(); };
            CurrentWebView.SetBackButtonEnabled(false);
            CurrentWebView.SetZoomEnabled(false);
            CurrentWebView.BackgroundColor = Color.black;
            CurrentWebView.Alpha           = 1;

            var url = UniWebViewHelper.PersistentDataURLForPath(path);
            CurrentWebView.Load(url);
            CurrentWebView.Show();

            DebugUtil.Log($"[UASdk] WebView size: {Screen.width}x{Screen.height}");
        }

        public static void Call(string statement)
        {
            if (!CurrentWebView)
            {
                return;
            }

            CurrentWebView.EvaluateJavaScript(statement);
        }

        public static void HideWebView()
        {
            if (!CurrentWebView) return;

            CurrentWebView.Hide();
            Object.DestroyImmediate(CurrentWebView.gameObject);
            CurrentWebView = null;
        }

        #endregion

        #region Private Apis

        private static bool JumpToMarket(string url)
        {
            try
            {
                var intentObject = new AndroidJavaObject("android.content.Intent");
                intentObject.Call<AndroidJavaObject>("setAction", "android.intent.action.VIEW");

                var uriClass  = new AndroidJavaClass("android.net.Uri");
                var uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", url);

                intentObject.Call<AndroidJavaObject>("setData",    uriObject);
                intentObject.Call<AndroidJavaObject>("setPackage", "com.android.vending");

                var unityPlayer     = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("startActivity", intentObject);

                return true;
            }
            catch (System.Exception exception)
            {
                DebugUtil.LogError($"JumpToGooglePlay failed: {exception.Message}\n{exception.StackTrace}");

                return false;
            }
        }


        private static bool JumpToGooglePlay(string url)
        {
            const string baseUrl   = "https://play.google.com/store/apps/details";
            const string marketUrl = "market://details";

            if (!url.Contains(baseUrl))
            {
                DebugUtil.Log($"[UaSdk] Url is not Google Play: {url}");
                return false;
            }

            return JumpToMarket(url.Replace(baseUrl, marketUrl));
        }

        #endregion
    }
}