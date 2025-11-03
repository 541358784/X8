using System;
using System.Collections.Generic;
using DragonU3DSDK;
using UnityEngine;

namespace DragonPlus.Ad.UA
{
    public class UaSdkCallbacks
    {
        #region Properties

        private static readonly Dictionary<string, string> EmptyDictionary = new();

        private static Action<UASdkConfiguration> _onSdkInitializedEvent;

        private static Action<string, UaAdInfo>              _onRewardedAdLoadedEvent;
        private static Action<string, UaErrorInfo>           _onRewardedAdLoadFailedEvent;
        private static Action<string, UaAdInfo>              _onRewardedAdDisplayedEvent;
        private static Action<string, UaErrorInfo, UaAdInfo> _onRewardedAdFailedToDisplayEvent;
        private static Action<string, UaAdInfo>              _onRewardedAdClickedEvent;
        private static Action<string, UaAdInfo>              _onRewardedAdRevenuePaidEvent;
        private static Action<string, UaReward, UaAdInfo>    _onRewardedAdReceivedRewardEvent;
        private static Action<string, UaAdInfo>              _onRewardedAdHiddenEvent;

        private static Action<string, UaAdInfo>              _onInterstitialAdLoadedEvent;
        private static Action<string, UaErrorInfo>           _onInterstitialAdLoadFailedEvent;
        private static Action<string, UaAdInfo>              _onInterstitialAdDisplayedEvent;
        private static Action<string, UaErrorInfo, UaAdInfo> _onInterstitialAdFailedToDisplayEvent;
        private static Action<string, UaAdInfo>              _onInterstitialAdClickedEvent;
        private static Action<string, UaAdInfo>              _onInterstitialAdRevenuePaidEvent;
        private static Action<string, UaAdInfo>              _onInterstitialAdHiddenEvent;

        private static Action<string, UaAdInfo>    _onBannerAdLoadedEvent;
        private static Action<string, UaErrorInfo> _onBannerAdLoadFailedEvent;
        private static Action<string, UaAdInfo>    _onBannerAdClickedEvent;
        private static Action<string, UaAdInfo>    _onBannerAdRevenuePaidEvent;

        #endregion

        #region Public Apis

        public static event Action<UASdkConfiguration> OnSdkInitializedEvent
        {
            add
            {
                LogSubscribedToEvent("OnSdkInitializedEvent");
                _onSdkInitializedEvent += value;
            }
            remove
            {
                LogUnsubscribedToEvent("OnSdkInitializedEvent");
                _onSdkInitializedEvent -= value;
            }
        }

        public class Rewarded
        {
            public static event Action<string, UaAdInfo> OnAdLoadedEvent
            {
                add
                {
                    LogSubscribedToEvent("OnRewardedAdLoadedEvent");
                    _onRewardedAdLoadedEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnRewardedAdLoadedEvent");
                    _onRewardedAdLoadedEvent -= value;
                }
            }

            public static event Action<string, UaErrorInfo> OnAdLoadFailedEvent
            {
                add
                {
                    LogSubscribedToEvent("OnRewardedAdLoadFailedEvent");
                    _onRewardedAdLoadFailedEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnRewardedAdLoadFailedEvent");
                    _onRewardedAdLoadFailedEvent -= value;
                }
            }

            /**
             * Fired when a rewarded ad is displayed (may not be received by Unity until the rewarded ad closes).
             */
            public static event Action<string, UaAdInfo> OnAdDisplayedEvent
            {
                add
                {
                    LogSubscribedToEvent("OnRewardedAdDisplayedEvent");
                    _onRewardedAdDisplayedEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnRewardedAdDisplayedEvent");
                    _onRewardedAdDisplayedEvent -= value;
                }
            }

            public static event Action<string, UaErrorInfo, UaAdInfo> OnAdDisplayFailedEvent
            {
                add
                {
                    LogSubscribedToEvent("OnRewardedAdDisplayFailedEvent");
                    _onRewardedAdFailedToDisplayEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnRewardedAdDisplayFailedEvent");
                    _onRewardedAdFailedToDisplayEvent -= value;
                }
            }

            public static event Action<string, UaAdInfo> OnAdClickedEvent
            {
                add
                {
                    LogSubscribedToEvent("OnRewardedAdClickedEvent");
                    _onRewardedAdClickedEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnRewardedAdClickedEvent");
                    _onRewardedAdClickedEvent -= value;
                }
            }

            /// <summary>
            /// Fired when a rewarded ad impression was validated and revenue will be paid.
            /// Executed on a background thread to avoid any delays in execution.
            /// </summary>
            public static event Action<string, UaAdInfo> OnAdRevenuePaidEvent
            {
                add
                {
                    LogSubscribedToEvent("OnRewardedAdRevenuePaidEvent");
                    _onRewardedAdRevenuePaidEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnRewardedAdRevenuePaidEvent");
                    _onRewardedAdRevenuePaidEvent -= value;
                }
            }

            public static event Action<string, UaReward, UaAdInfo> OnAdReceivedRewardEvent
            {
                add
                {
                    LogSubscribedToEvent("OnRewardedAdReceivedRewardEvent");
                    _onRewardedAdReceivedRewardEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnRewardedAdReceivedRewardEvent");
                    _onRewardedAdReceivedRewardEvent -= value;
                }
            }

            public static event Action<string, UaAdInfo> OnAdHiddenEvent
            {
                add
                {
                    LogSubscribedToEvent("OnRewardedAdHiddenEvent");
                    _onRewardedAdHiddenEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnRewardedAdHiddenEvent");
                    _onRewardedAdHiddenEvent -= value;
                }
            }
        }

        public class Interstitial
        {
            public static event Action<string, UaAdInfo> OnAdLoadedEvent
            {
                add
                {
                    LogSubscribedToEvent("OnInterstitialAdLoadedEvent");
                    _onInterstitialAdLoadedEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnInterstitialAdLoadedEvent");
                    _onInterstitialAdLoadedEvent -= value;
                }
            }

            public static event Action<string, UaErrorInfo> OnAdLoadFailedEvent
            {
                add
                {
                    LogSubscribedToEvent("OnInterstitialAdLoadFailedEvent");
                    _onInterstitialAdLoadFailedEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnInterstitialAdLoadFailedEvent");
                    _onInterstitialAdLoadFailedEvent -= value;
                }
            }

            /**
             * Fired when an interstitial ad is displayed (may not be received by Unity until the interstitial ad closes).
             */
            public static event Action<string, UaAdInfo> OnAdDisplayedEvent
            {
                add
                {
                    LogSubscribedToEvent("OnInterstitialAdDisplayedEvent");
                    _onInterstitialAdDisplayedEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnInterstitialAdDisplayedEvent");
                    _onInterstitialAdDisplayedEvent -= value;
                }
            }

            public static event Action<string, UaErrorInfo, UaAdInfo> OnAdDisplayFailedEvent
            {
                add
                {
                    LogSubscribedToEvent("OnInterstitialAdDisplayFailedEvent");
                    _onInterstitialAdFailedToDisplayEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnInterstitialAdDisplayFailedEvent");
                    _onInterstitialAdFailedToDisplayEvent -= value;
                }
            }

            public static event Action<string, UaAdInfo> OnAdClickedEvent
            {
                add
                {
                    LogSubscribedToEvent("OnInterstitialAdClickedEvent");
                    _onInterstitialAdClickedEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnInterstitialAdClickedEvent");
                    _onInterstitialAdClickedEvent -= value;
                }
            }

            /// <summary>
            /// Fired when an interstitial ad impression was validated and revenue will be paid.
            /// Executed on a background thread to avoid any delays in execution.
            /// </summary>
            public static event Action<string, UaAdInfo> OnAdRevenuePaidEvent
            {
                add
                {
                    LogSubscribedToEvent("OnInterstitialAdRevenuePaidEvent");
                    _onInterstitialAdRevenuePaidEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnInterstitialAdRevenuePaidEvent");
                    _onInterstitialAdRevenuePaidEvent -= value;
                }
            }

            public static event Action<string, UaAdInfo> OnAdHiddenEvent
            {
                add
                {
                    LogSubscribedToEvent("OnInterstitialAdHiddenEvent");
                    _onInterstitialAdHiddenEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnInterstitialAdHiddenEvent");
                    _onInterstitialAdHiddenEvent -= value;
                }
            }
        }

        public class Banner
        {
            public static event Action<string, UaAdInfo> OnAdLoadedEvent
            {
                add
                {
                    LogSubscribedToEvent("OnBannerAdLoadedEvent");
                    _onBannerAdLoadedEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnBannerAdLoadedEvent");
                    _onBannerAdLoadedEvent -= value;
                }
            }

            public static event Action<string, UaErrorInfo> OnAdLoadFailedEvent
            {
                add
                {
                    LogSubscribedToEvent("OnBannerAdLoadFailedEvent");
                    _onBannerAdLoadFailedEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnBannerAdLoadFailedEvent");
                    _onBannerAdLoadFailedEvent -= value;
                }
            }

            public static event Action<string, UaAdInfo> OnAdClickedEvent
            {
                add
                {
                    LogSubscribedToEvent("OnBannerAdClickedEvent");
                    _onBannerAdClickedEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnBannerAdClickedEvent");
                    _onBannerAdClickedEvent -= value;
                }
            }

            public static event Action<string, UaAdInfo> OnAdRevenuePaidEvent
            {
                add
                {
                    LogSubscribedToEvent("OnBannerAdRevenuePaidEvent");
                    _onBannerAdRevenuePaidEvent += value;
                }
                remove
                {
                    LogUnsubscribedToEvent("OnBannerAdRevenuePaidEvent");
                    _onBannerAdRevenuePaidEvent -= value;
                }
            }
        }

        public static void ForwardEvent(string eventName)
        {
            ForwardEvent(eventName, EmptyDictionary);
        }

        public static void ForwardEvent(string eventName, Dictionary<string, string> arguments)
        {
            switch (eventName)
            {
                case UaSdkCallbackEvents.OnInitialCallbackEvent:
                    DebugUtil.Log("Initial background callback.");
                    break;
                case UaSdkCallbackEvents.OnSdkInitializedEvent:
                {
                    var sdkConfiguration = new UASdkConfiguration();
                    InvokeEvent(_onSdkInitializedEvent, sdkConfiguration, eventName);
                    break;
                }
                // Ad Events
                default:
                {
                    var adInfo           = new UaAdInfo(arguments);
                    var adUnitIdentifier = adInfo.AdUnitIdentifier;
                    switch (eventName)
                    {
                        case UaSdkCallbackEvents.OnBannerAdLoadedEvent:
                            InvokeEvent(_onBannerAdLoadedEvent, adUnitIdentifier, adInfo, eventName);
                            break;
                        case UaSdkCallbackEvents.OnBannerAdLoadFailedEvent:
                        {
                            var errorInfo = new UaErrorInfo();
                            InvokeEvent(_onBannerAdLoadFailedEvent, adUnitIdentifier, errorInfo, eventName);
                            break;
                        }
                        case UaSdkCallbackEvents.OnBannerAdClickedEvent:
                            InvokeEvent(_onBannerAdClickedEvent, adUnitIdentifier, adInfo, eventName);
                            break;
                        case UaSdkCallbackEvents.OnBannerAdRevenuePaidEvent:
                            InvokeEvent(_onBannerAdRevenuePaidEvent, adUnitIdentifier, adInfo, eventName);
                            break;
                        case UaSdkCallbackEvents.OnInterstitialLoadedEvent:
                            InvokeEvent(_onInterstitialAdLoadedEvent, adUnitIdentifier, adInfo, eventName);
                            break;
                        case UaSdkCallbackEvents.OnInterstitialLoadFailedEvent:
                        {
                            var errorInfo = new UaErrorInfo();
                            InvokeEvent(_onInterstitialAdLoadFailedEvent, adUnitIdentifier, errorInfo, eventName);
                            break;
                        }
                        case UaSdkCallbackEvents.OnInterstitialHiddenEvent:
                            InvokeEvent(_onInterstitialAdHiddenEvent, adUnitIdentifier, adInfo, eventName);
                            break;
                        case UaSdkCallbackEvents.OnInterstitialDisplayedEvent:
                            InvokeEvent(_onInterstitialAdDisplayedEvent, adUnitIdentifier, adInfo, eventName);
                            break;
                        case UaSdkCallbackEvents.OnInterstitialAdFailedToDisplayEvent:
                        {
                            var errorInfo = new UaErrorInfo();
                            InvokeEvent(_onInterstitialAdFailedToDisplayEvent, adUnitIdentifier, errorInfo, adInfo,
                                eventName);
                            break;
                        }
                        case UaSdkCallbackEvents.OnInterstitialClickedEvent:
                            InvokeEvent(_onInterstitialAdClickedEvent, adUnitIdentifier, adInfo, eventName);
                            break;
                        case UaSdkCallbackEvents.OnInterstitialAdRevenuePaidEvent:
                            InvokeEvent(_onInterstitialAdRevenuePaidEvent, adUnitIdentifier, adInfo, eventName);
                            break;
                        case UaSdkCallbackEvents.OnRewardedAdLoadedEvent:
                            InvokeEvent(_onRewardedAdLoadedEvent, adUnitIdentifier, adInfo, eventName);
                            break;
                        case UaSdkCallbackEvents.OnRewardedAdLoadFailedEvent:
                        {
                            var errorInfo = new UaErrorInfo();
                            InvokeEvent(_onRewardedAdLoadFailedEvent, adUnitIdentifier, errorInfo, eventName);
                            break;
                        }
                        case UaSdkCallbackEvents.OnRewardedAdDisplayedEvent:
                            InvokeEvent(_onRewardedAdDisplayedEvent, adUnitIdentifier, adInfo, eventName);
                            break;
                        case UaSdkCallbackEvents.OnRewardedAdHiddenEvent:
                            InvokeEvent(_onRewardedAdHiddenEvent, adUnitIdentifier, adInfo, eventName);
                            break;
                        case UaSdkCallbackEvents.OnRewardedAdClickedEvent:
                            InvokeEvent(_onRewardedAdClickedEvent, adUnitIdentifier, adInfo, eventName);
                            break;
                        case UaSdkCallbackEvents.OnRewardedAdRevenuePaidEvent:
                            InvokeEvent(_onRewardedAdRevenuePaidEvent, adUnitIdentifier, adInfo, eventName);
                            break;
                        case UaSdkCallbackEvents.OnRewardedAdFailedToDisplayEvent:
                        {
                            var errorInfo = new UaErrorInfo();
                            InvokeEvent(_onRewardedAdFailedToDisplayEvent, adUnitIdentifier, errorInfo, adInfo,
                                eventName);
                            break;
                        }
                        case UaSdkCallbackEvents.OnRewardedAdReceivedRewardEvent:
                        {
                            var reward = new UaReward();

                            InvokeEvent(_onRewardedAdReceivedRewardEvent, adUnitIdentifier, reward, adInfo, eventName);
                            break;
                        }
                        default:
                            DebugUtil.LogWarning("Unknown UA Ads event fired: " + eventName);
                            break;
                    }

                    break;
                }
            }
        }

        #endregion

        #region Private Apis

        private static void LogSubscribedToEvent(string eventName)
        {
            DebugUtil.Log($"[UASdk] Listener has been added to callback: {eventName}");
        }

        private static void LogUnsubscribedToEvent(string eventName)
        {
            DebugUtil.Log($"[UASdk] Listener has been removed from callback: {eventName}");
        }

        private static void InvokeEvent<T>(Action<T> evt, T param, string eventName)
        {
            DebugUtil.Log("Invoking event: " + eventName + ". Param: " + param);

            UaSdkUtility.EnqueueMainThreadTask(() =>
            {
                try
                {
                    evt(param);
                }
                catch (Exception exception)
                {
                    DebugUtil.LogError($"Caught exception in publisher event: {eventName}, exception: {exception}");
                    Debug.LogException(exception);
                }
            });
        }


        private static void InvokeEvent<T1, T2>(Action<T1, T2> evt, T1 param1, T2 param2, string eventName)
        {
            DebugUtil.Log("Invoking event: " + eventName + ". Params: " + param1 + ", " + param2);

            UaSdkUtility.EnqueueMainThreadTask(() =>
            {
                try
                {
                    evt(param1, param2);
                }
                catch (Exception exception)
                {
                    DebugUtil.LogError($"Caught exception in publisher event: {eventName}, exception: {exception}");
                    Debug.LogException(exception);
                }
            });
        }

        private static void InvokeEvent<T1, T2, T3>(
            Action<T1, T2, T3> evt,
            T1                 param1,
            T2                 param2,
            T3                 param3,
            string             eventName)
        {
            DebugUtil.Log("Invoking event: " + eventName + ". Params: " + param1 + ", " + param2 + ", " + param3);

            UaSdkUtility.EnqueueMainThreadTask(() =>
            {
                try
                {
                    evt(param1, param2, param3);
                }
                catch (Exception exception)
                {
                    DebugUtil.LogError($"Caught exception in publisher event: {eventName}, exception: {exception}");
                    Debug.LogException(exception);
                }
            });
        }

#if UNITY_EDITOR && UNITY_2019_2_OR_NEWER
        /// <summary>
        /// Resets static event handlers so they still get reset even if Domain Reloading is disabled
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnDomainReload()
        {
            _onSdkInitializedEvent = null;

            _onInterstitialAdLoadedEvent          = null;
            _onInterstitialAdLoadFailedEvent      = null;
            _onInterstitialAdDisplayedEvent       = null;
            _onInterstitialAdFailedToDisplayEvent = null;
            _onInterstitialAdClickedEvent         = null;
            _onInterstitialAdRevenuePaidEvent     = null;
            _onInterstitialAdHiddenEvent          = null;

            _onRewardedAdLoadedEvent          = null;
            _onRewardedAdLoadFailedEvent      = null;
            _onRewardedAdDisplayedEvent       = null;
            _onRewardedAdFailedToDisplayEvent = null;
            _onRewardedAdClickedEvent         = null;
            _onRewardedAdRevenuePaidEvent     = null;
            _onRewardedAdReceivedRewardEvent  = null;
            _onRewardedAdHiddenEvent          = null;

            _onBannerAdLoadedEvent      = null;
            _onBannerAdLoadFailedEvent  = null;
            _onBannerAdClickedEvent     = null;
            _onBannerAdRevenuePaidEvent = null;
        }
#endif

        #endregion
    }
}