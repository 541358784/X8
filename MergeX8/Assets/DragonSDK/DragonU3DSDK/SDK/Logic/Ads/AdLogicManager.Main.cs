using System;
using System.Collections;
using System.Collections.Generic;
using Dlugin;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Network.BI;
using DragonU3DSDK.Storage;
using Facebook.Unity;

namespace DragonPlus
{
    public partial class AdLogicManager : Manager<AdLogicManager>
    {
        private void Awake()
        {
            fails[(int)AD_Type.RewardVideo] = new Dictionary<BiEventCommon.Types.CommonMonetizationAdEventFailedReason, FailDelegate>();
            fails[(int)AD_Type.Interstitial] = new Dictionary<BiEventCommon.Types.CommonMonetizationAdEventFailedReason, FailDelegate>();
            fails[(int)AD_Type.Banner] = new Dictionary<BiEventCommon.Types.CommonMonetizationAdEventFailedReason, FailDelegate>();
            fails[(int)AD_Type.Mrec] = new Dictionary<BiEventCommon.Types.CommonMonetizationAdEventFailedReason, FailDelegate>();
            fails[(int)AD_Type.OfferWall] = new Dictionary<BiEventCommon.Types.CommonMonetizationAdEventFailedReason, FailDelegate>();

            foreach (BiEventCommon.Types.CommonMonetizationAdEventFailedReason p in Enum.GetValues(typeof(BiEventCommon.Types.CommonMonetizationAdEventFailedReason)))
            {
                fails[(int)AD_Type.RewardVideo][p] = id => { return false; };
                fails[(int)AD_Type.Interstitial][p] = id => { return false; };
                fails[(int)AD_Type.Banner][p] = id => { return false; };
                fails[(int)AD_Type.Mrec][p] = id => { return false; };
                fails[(int)AD_Type.OfferWall][p] = id => { return false; };
            }

            StartCoroutine(CheckMonitorPlacement());
            StartCoroutine(CheckInterstitialLoad());
        }
        
        public delegate bool FailDelegate(string placementId);
        private Dictionary<BiEventCommon.Types.CommonMonetizationAdEventFailedReason, FailDelegate>[] fails = 
            new Dictionary<BiEventCommon.Types.CommonMonetizationAdEventFailedReason, FailDelegate>[(int)AD_Type.Count];
        
        private bool m_specialOrder;
        public bool specialOrder
        {
            get
            {
                if (m_specialOrder)
                {
                    m_specialOrder = false;
                    return true;
                }

                return m_specialOrder;
            }
            set { m_specialOrder = value; }
        }
        
        private Action<bool, string> rewardedVideoCallback;
        private Action<bool, string> IntersititaiCallback;
        private Action<bool> offerWallCallback;

        /// <summary>
        /// FailDelegate返回ture：代表匹配这个reason，即广告位不能展示
        /// </summary>
        /// <param name="type"></param>
        /// <param name="reason"></param>
        /// <param name="fail"></param>
        public void RegisterFailDelegate(AD_Type type, BiEventCommon.Types.CommonMonetizationAdEventFailedReason reason, FailDelegate fail)
        {
            if (null == fails[(int) type])
            {
                DebugUtil.LogError("还没支持这种类型");
                return;
            }
            fails[(int) type][reason] = fail;
        }
        
        public bool ShouldShowRV(string placementId, bool withBI = true)
        {
            foreach (var p in fails[(int)AD_Type.RewardVideo])
            {
                if (p.Value == null)
                {
                    DebugUtil.LogError($"激励 缺少{p.Key}类型委托！！！");
                    continue;
                }
                
                if (p.Value.Invoke(placementId))
                {
                    if (withBI)
                    {
                        int delayTime = 0;
                        switch (p.Key)
                        {
                            case BiEventCommon.Types.CommonMonetizationAdEventFailedReason
                                .CommonMonetizationEventReasonAdNewuserlevel:
                            case BiEventCommon.Types.CommonMonetizationAdEventFailedReason
                                .CommonMonetizationEventReasonAdOverdisplay:
                            case BiEventCommon.Types.CommonMonetizationAdEventFailedReason
                                .CommonMonetizationEventReasonAdSeperateCooldown:
                            {
                                delayTime = 5100;
                            }
                                break;
                            case BiEventCommon.Types.CommonMonetizationAdEventFailedReason
                                .CommonMonetizationEventReasonAdNofill:
                            {
                                delayTime = 5200;
                            }
                            break;
                        }
                        DelayActionManager.Instance.DebounceInMainThread(placementId, delayTime, () =>
                        {
                            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                                BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventRewardVideoShouldNotDisplay, placementId, p.Key);
                        });
                    }
                    
                    #if !DISABLE_ADS_LOG
                    DebugUtil.LogWarning($"不能曝光激励视频{placementId}广告位，原因是：{p.Key}");
                    #endif
                    
                    return false;
                }
            }
            
            return true;
        }
        public bool TryShowRewardedVideoInternal(string placementId, Action<bool, string> cb)
        {
            DebugUtil.Log($"尝试展示激励视频 ：{placementId}");

            if (!ShouldShowRV(placementId, false))
            {
                return false;
            }

            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventRewardVideoDisplay,
                placementId,
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNone
            );
#if UNITY_ANDROID
            DragonU3DSDK.Network.BI.BIManager.Instance.LocalBackupImmediately();
#endif

            Action<bool> sdkADCallback = (b) =>
            {
                if (b)
                {
                    DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                        BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventRewardVideoDisplaySuccess,
                        placementId,
                        BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNone
                    );

                    AddRewardWatchCount();
                }
                else
                {
                    DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                        BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventRewardVideoUserCanceled,
                        placementId,
                        BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNone
                    );
                }
                
                rewardedVideoCallback?.Invoke(b, "");
                rewardedVideoCallback = null;
            };

            if (cb != null)
            {
                rewardedVideoCallback = cb;
            }
            
            specialOrder = true;

            Dlugin.SDK.GetInstance().m_AdsManager.PlayReward(sdkADCallback, placementId);
            
            return true;
        }

        public bool ShouldShowInterstitial(string placementId, bool withBI = true)
        {
            //神秘力量是现在唯一需要考虑顺序关系的，如果后续还有其余reason有顺序需求，则拓展支持自定义顺序。
            BiEventCommon.Types.CommonMonetizationAdEventFailedReason topReason =
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdMysteryPower;
            FailDelegate topFailDelegate = fails[(int) AD_Type.Interstitial][topReason];

            if (null != topFailDelegate)
            {
                if (topFailDelegate.Invoke(placementId))
                {
                    if (withBI)
                    {
                        DelayActionManager.Instance.DebounceInMainThread(placementId, 1000, () =>
                        {
                            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                                BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventInterstitialAdShouldNotDisplay, placementId, topReason);
                        });
                    }
                    
                    DebugUtil.LogWarning($"不能曝光插屏{placementId}广告位，原因是：{topReason}");
                    
                    return false;
                }
            }
            
            foreach (var p in fails[(int)AD_Type.Interstitial])
            {
                if (p.Key == topReason)
                {
                    continue;
                }
                
                if (p.Value == null)
                {
                    DebugUtil.LogError($"插屏 缺少{p.Key}类型委托！！！");
                    continue;
                }

                if (p.Value.Invoke(placementId))
                {
                    if (withBI)
                    {
                        int delayTime = 0;
                        switch (p.Key)
                        {
                            case BiEventCommon.Types.CommonMonetizationAdEventFailedReason
                                .CommonMonetizationEventReasonAdMysteryPower:
                            case BiEventCommon.Types.CommonMonetizationAdEventFailedReason
                                .CommonMonetizationEventReasonAdPaid:
                            case BiEventCommon.Types.CommonMonetizationAdEventFailedReason
                                .CommonMonetizationEventReasonAdNewuserlevel:
                            case BiEventCommon.Types.CommonMonetizationAdEventFailedReason
                                .CommonMonetizationEventReasonAdSeperateCooldown:
                            case BiEventCommon.Types.CommonMonetizationAdEventFailedReason
                                .CommonMonetizationEventReasonAdNofill:
                            {
                                delayTime = 1000;
                            }
                            break;
                        }
                        DelayActionManager.Instance.DebounceInMainThread(placementId, delayTime, () =>
                        {
                            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                                BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventInterstitialAdShouldNotDisplay, placementId, p.Key);
                        });
                    }
                    
                    DebugUtil.LogWarning($"不能曝光插屏{placementId}广告位，原因是：{p.Key}");
                    
                    return false;
                }
            }
            
            return true;
        }
        public bool TryShowInterstitialInternal(string placementId, Action<bool, string> cb)
        {
            DebugUtil.Log($"尝试展示插屏广告 ：{placementId}");

            if (!ShouldShowInterstitial(placementId))
            {
                return false;
            }
            
            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventInterstitialAdDisplay,
                placementId,
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNone
            );
#if UNITY_ANDROID
            DragonU3DSDK.Network.BI.BIManager.Instance.LocalBackupImmediately();
#endif

            Action sdkADCallback = () =>
            {
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                    BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventInterstitialAdDisplaySuccess,
                    placementId,
                    BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNone
                );
                
                this.IntersititaiCallback?.Invoke(true, "");
                this.IntersititaiCallback = null;
            };
            if (cb != null)
                this.IntersititaiCallback = cb;
            
            specialOrder = true;

            Dlugin.SDK.GetInstance().m_AdsManager.PlayInterstitial(sdkADCallback, placementId);
            
            return true;
        }

        public bool ShouldShowBanner(string placementId, bool withBI = true)
        {
            foreach (var p in fails[(int)AD_Type.Banner])
            {
                if (p.Value == null)
                {
                    DebugUtil.LogError($"Banner 缺少{p.Key}类型委托！！！");
                    continue;
                }
                
                if (p.Value.Invoke(placementId))
                {
                    if (withBI)
                    {
                        // bi没有价值，没有发送
                    }
                    
                    DebugUtil.LogWarning($"不能曝banner{placementId}广告位，原因是：{p.Key}");
                    
                    return false;
                }
            }
            
            return true;
        }
        private bool TryShowBannerInternal(string placementId)
        {
            DebugUtil.Log($"尝试展示Banner ：{placementId}");
            
            if (!ShouldShowBanner(placementId, false))
            {
                return false;
            }
            
            // bi没有价值，没有发送
            
            Dlugin.SDK.GetInstance().m_AdsManager.ShowBanner();
            return true;
        }
        
        public bool ShouldShowMrec(string placementId, bool withBI = true)
        {
            foreach (var p in fails[(int)AD_Type.Mrec])
            {
                if (p.Value == null)
                {
                    DebugUtil.LogError($"Mrec 缺少{p.Key}类型委托！！！");
                    continue;
                }
                
                if (p.Value.Invoke(placementId))
                {
                    if (withBI)
                    {
                        // bi没有价值，没有发送
                    }
                    
                    DebugUtil.LogWarning($"不能曝Mrec{placementId}广告位，原因是：{p.Key}");
                    
                    return false;
                }
            }
            
            return true;
        }
        private bool TryShowMrecrInternal(string placementId)
        {
            DebugUtil.Log($"尝试展示Mrec ：{placementId}");
            
            if (!ShouldShowMrec(placementId, false))
            {
                return false;
            }
            
            // bi没有价值，没有发送
            
            Dlugin.SDK.GetInstance().m_AdsManager.ShowMrec();
            return true;
        }

        public bool ShouldShowOfferWall(string placementId, bool withBI = true)
        {
            foreach (var p in fails[(int)AD_Type.OfferWall])
            {
                if (p.Value == null)
                {
                    DebugUtil.LogError($"OfferWall 缺少{p.Key}类型委托！！！");
                    continue;
                }
                
                if (p.Value.Invoke(placementId))
                {
                    if (withBI)
                    {
                        // bi没有价值，没有发送
                    }
                    
                    DebugUtil.LogWarning($"不能曝OfferWall{placementId}广告位，原因是：{p.Key}");
                    
                    return false;
                }
            }
            
            return true;
        }
        private bool TryShowOfferWallInternal(string placementId, string name, int wait_request_millisecond, Action<bool> show_pre_callback, Action<bool> show_post_callback)
        {
            DebugUtil.Log($"尝试展示OfferWall ：{placementId}");
            
            if (!ShouldShowOfferWall(placementId, false))
            {
                return false;
            }

            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventOfferwallAdDisplay,
                placementId,
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNone
            );
#if UNITY_ANDROID
            DragonU3DSDK.Network.BI.BIManager.Instance.LocalBackupImmediately();
#endif

            Action<bool> cb = b =>
            {
                if (b)
                {
                    DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                        BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventOfferwallAdDisplaySuccess,
                        placementId,
                        BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNone
                    );
                }

                offerWallCallback(b);
            };

            offerWallCallback = show_post_callback;
            
            return Dlugin.SDK.GetInstance().m_AdsManager.ShowOfferwall(name, wait_request_millisecond, show_pre_callback, cb);
        }
        
        public int ValidShouleTotalCnt;
        List<AdPlacementMonitor> monitorRV = new List<AdPlacementMonitor>();
        public void RegisterPlacementMonitor(AdPlacementMonitor placement)
        {
            monitorRV.Add(placement);

            if (monitorRV.Count > 15)
            {
                DebugUtil.LogError("$同时监控的广告位数量超过15个，请检测是否存在滥用！！！");
            }
        }
        public void UnregisterPlacementMonitor(AdPlacementMonitor placement)
        {
            monitorRV.Remove(placement);
        }
        public IEnumerator CheckMonitorPlacement()
        {
            while (true)
            {
                foreach (var p in monitorRV)
                {
                    if (null == p)
                    {
                        continue;
                    }
                    try
                    {
                        p.Check();
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
                
                yield return new WaitForSeconds(0.1f);
            }
        }

        public IEnumerator CheckInterstitialLoad()
        {
            while (true)
            {
                yield return new WaitForSeconds(2.0f);
                
                FailDelegate act = fails[(int) AD_Type.Interstitial][
                    BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdPaid];
                if (null != act)
                {
                    if (!act.Invoke(""))
                    {
                        AdsManager.Instance.HandleLoad(AD_Type.Interstitial);
                        yield break;
                    }
                }
            }
        }

        private void AddRewardWatchCount()
        {
            StorageCommon storage = StorageManager.Instance.GetStorage<StorageCommon>();
            long currentTime = Utils.TotalSeconds();
            if (currentTime > storage.AdCountResetTime)
            {
                storage.AdCountResetTime = DragonU3DSDK.Utils.GetTomorrowTimestamp();
                storage.AdCountStageState = 0;
                storage.AdRewardWatchedCount = 0;
            }

            storage.AdRewardWatchedCount++;
            
            //For RV_DIDREWARD_STAGE to Facebook
            if (storage.AdRewardWatchedCount >= 15 && (storage.AdCountStageState & 0x00000001) == 0)
            {
                OnAddRewardWatchCountEvent("RV_DIDREWARD_STAGE_1");

                storage.AdCountStageState |= 0x00000001;
            }
            if (storage.AdRewardWatchedCount >= 20 && (storage.AdCountStageState & 0x00000010) == 0)
            {
                OnAddRewardWatchCountEvent("RV_DIDREWARD_STAGE_2");

                storage.AdCountStageState |= 0x00000010;
            }
            if (storage.AdRewardWatchedCount >= 30 && (storage.AdCountStageState & 0x00000100) == 0)
            {
                OnAddRewardWatchCountEvent("RV_DIDREWARD_STAGE_3");

                storage.AdCountStageState |= 0x00000100;
            }
            if (storage.AdRewardWatchedCount >= 50 && (storage.AdCountStageState & 0x00001000) == 0)
            {
                OnAddRewardWatchCountEvent("RV_DIDREWARD_STAGE_4");

                storage.AdCountStageState |= 0x00001000;
            }
        }

        public void OnAddRewardWatchCountEvent(string evt)
        {
            if (FB.IsInitialized) FB.LogAppEvent(evt);
            
            BIManager.Instance.onThirdPartyTracking(evt);
        }
    }

    public interface AdPlacementMonitor
    {
        void Check();
    }
}