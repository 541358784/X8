using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Network.API.Protocol;
using System;
using ActivityLocal.TipReward.Module;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using Newtonsoft.Json;
//using BiEventMakeOver = DragonU3DSDK.Network.API.Protocol.BiEventMakeoverMaster;

namespace DragonPlus
{
    public partial class AdLogicManager
    {
        private StorageHome storageHome = null;
        public StorageHome StorageHome
        {
            get
            {
                if(storageHome == null)
                    storageHome = StorageManager.Instance.GetStorage<StorageHome>();
            
                return storageHome;
            }
        }
        
        public void PlayRV(string pl, System.Action<bool, string> cb)
        {
            this.PlayRewardVideo(pl, cb);
        }

        public void PlayInterstital(string pl, System.Action<bool, string> cb)
        {
            this.PlayInterstitalAd(pl, cb);
        }

        private bool PlayRewardVideo(string placementId, System.Action<bool, string> cb)
        {
            DebugUtil.Log($"尝试展示激励视频 ：{placementId}");

            AdSubSystem.Instance.isPlayRv = true;

            StorageHome.SendBiIndex++;
            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventRewardVideoDisplay,
                placementId,
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNone, UserGroupManager.Instance.GetPlatformUserGroupString() + "_" + StorageHome.SendBiIndex
            );
#if UNITY_ANDROID
            DragonU3DSDK.Network.BI.BIManager.Instance.LocalBackupImmediately();
#endif

            Action<bool> sdkADCallback = (b) =>
            {
                if (b)
                {
                    DebugUtil.Log("激励视频播放成功");
                    var storageHome = DragonU3DSDK.Storage.StorageManager.Instance
                        .GetStorage<DragonU3DSDK.Storage.StorageHome>();
                    var storageGame = DragonU3DSDK.Storage.StorageManager.Instance
                        .GetStorage<DragonU3DSDK.Storage.StorageGame>();
                    DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                        BiEventCommon.Types.CommonMonetizationAdEventType
                            .CommonMonetizationEventRewardVideoDisplaySuccess,
                        placementId,
                        BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNone,
                        UserGroupManager.Instance.GetPlatformUserGroupString(),
                        storageHome?.CurRoomId.ToString(),
                        storageGame?.TaskGroups.CompleteOrderNum.ToString(),
                        AdConfigHandle.Instance.GetCommonID().ToString()
                    );

                    AddRewardWatchCount();
                }
                else
                {
                    DebugUtil.Log("激励视频播放取消");
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

#if UNITY_EDITOR
            sdkADCallback(true);
#else
            Dlugin.SDK.GetInstance().m_AdsManager.PlayReward(sdkADCallback, placementId);
#endif
            return true;
        }

        private bool PlayInterstitalAd(string placementId, System.Action<bool, string> cb)
        {
            DebugUtil.Log($"尝试展示插屏广告 ：{placementId}");

            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventInterstitialAdDisplay,
                placementId,
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNone, UserGroupManager.Instance.GetPlatformUserGroupString()
            );
#if UNITY_ANDROID
            DragonU3DSDK.Network.BI.BIManager.Instance.LocalBackupImmediately();
#endif

            Action sdkADCallback = () =>
            {
                DebugUtil.Log("激励视频播放成功");
                var storageHome = DragonU3DSDK.Storage.StorageManager.Instance
                    .GetStorage<DragonU3DSDK.Storage.StorageHome>();
                var storageGame = DragonU3DSDK.Storage.StorageManager.Instance
                    .GetStorage<DragonU3DSDK.Storage.StorageGame>();
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                    BiEventCommon.Types.CommonMonetizationAdEventType
                        .CommonMonetizationEventInterstitialAdDisplaySuccess,
                    placementId,
                    BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNone,
                    UserGroupManager.Instance.GetPlatformUserGroupString(),
                    storageHome?.CurRoomId.ToString(),
                    storageGame?.TaskGroups.CompleteOrderNum.ToString(),
                    AdConfigHandle.Instance.GetCommonID().ToString()
                );

                this.IntersititaiCallback?.Invoke(true, "");
                this.IntersititaiCallback = null;
            };
            if (cb != null)
                this.IntersititaiCallback = cb;

            specialOrder = true;

#if UNITY_EDITOR
            sdkADCallback();
#else
            Dlugin.SDK.GetInstance().m_AdsManager.PlayInterstitial(sdkADCallback, placementId);
#endif
            return true;
        }
    }


    public class AdSubSystem : GlobalSystem<AdSubSystem>, IOnApplicationPause
    {
        //0 none, 1 rv, 2 inter
        public int PlayingAdState { get; private set; }
        StorageAdData m_Storage;
        bool m_Init = false;

        private long m_LastINAdTime = 0;

        public bool isPlayRv = false;
        
        public System.TimeSpan TimeAfterInstall
        {
            get
            {
                System.DateTime install;
                if (StorageManager.Instance.GetStorage<StorageCommon>().InstalledAt > 0)
                    install = CommonUtils.ConvertFromUnixTimestamp((ulong)StorageManager.Instance.GetStorage<StorageCommon>().InstalledAt);
                else
                    install = CommonUtils.ConvertFromUnixTimestamp((ulong)StorageManager.Instance.GetStorage<StorageHome>().LocalFirstRunTimeStamp);
                System.DateTime cur = System.DateTime.UtcNow;
                return cur - install;
            }
        }

        public System.DateTime InstallTime
        {
            get
            {
                System.DateTime install;
                if (StorageManager.Instance.GetStorage<StorageCommon>().InstalledAt > 0)
                    install = CommonUtils.ConvertFromUnixTimestamp((ulong)StorageManager.Instance.GetStorage<StorageCommon>().InstalledAt);
                else
                    install = CommonUtils.ConvertFromUnixTimestamp((ulong)StorageManager.Instance.GetStorage<StorageHome>().LocalFirstRunTimeStamp);

                return install;
            }
        }

        public System.TimeSpan TimeAfterFirstLogin
        {
            get
            {
                return System.DateTime.Now - CommonUtils.ConvertFromUnixTimestamp((ulong)m_Storage.LoginTime);
            }
        }

        private bool m_SkipPauseInt = false;

        public bool CheckSkipPauseInt()
        {
            if (m_SkipPauseInt)
            {
                m_SkipPauseInt = false;
                return true;
            }
            return false;
        }

        public void SkipAPauseInt()
        {
            m_SkipPauseInt = true;
        }
       
        public void Init()
        {
            if (m_Init)
                return;

            m_Storage = StorageManager.Instance.GetStorage<StorageHome>().AdData;

            PlayingAdState = 0;

            m_Init = true;
            RefreshLoginTime();
            RegAdManager();

            AdRewardedVideoPlacementMonitor.BindUICamera(UIRoot.Instance.mUICamera);
        }

        
        /// <summary>
        /// 登录重置部分插屏判断数据
        /// </summary>
        public void LoginRefreshData()
        {
            AdConfigHandle.Instance.GetInterstitialAds().ForEach((ad =>
            {
                if (!m_Storage.InterstitalPlayRecords.ContainsKey(ad.PlaceId))
                {
                    StorageAdPlayRecord pr = new StorageAdPlayRecord();
                    pr.AdType = ad.IsActiveInterAds==1 ? 2 : 3;
                    m_Storage.InterstitalPlayRecords.Add(ad.PlaceId, pr); 
                }

                m_Storage.InterstitalPlayRecords[ad.PlaceId].LastPlayTime = CommonUtils.GetCurTime();
            }));
            m_Storage.PassiveInterCompleteOrderNum = 0;
            m_Storage.ActiveInterPlayRvNum = 0;
            m_Storage.PlayActiveInterTodayTime = CommonUtils.GetCurTime();
            m_Storage.PlayPassiveInterTodayTime = CommonUtils.GetCurTime();
            m_Storage.PlayRvTodayTime = 0;
            
            AdLocalConfigHandle.Instance.StartPassInGamePlayTime(false);
        }

        void RefreshLoginTime()
        {
            if (m_Storage.LoginTime == 0)
            {
                m_Storage.LoginTime = CommonUtils.GetCurTime();
            }
            else
            {
                var last_time = CommonUtils.ConvertFromUnixTimestamp((ulong)m_Storage.LoginTime);
                if (System.DateTime.Now.Year > last_time.Year || System.DateTime.Now.DayOfYear > last_time.DayOfYear)
                {
                    m_Storage.LoginTime = CommonUtils.GetCurTime();
                }
            }
        }
        
        void RegAdManager()
        {
            //rv
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.RewardVideo, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdSeperateCooldown, this.IsFailedByRVCDOK);
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.RewardVideo, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdOverdisplay, this.IsFailedByRVWithinLimit);
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.RewardVideo, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNofill, this.IsFailedRVByAdNotFilled);
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.RewardVideo, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNewuserlevel, this.IsFailedByRvReached);
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.RewardVideo, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdConfigDisable, this.IsFailedByRvConfiged);

            
            //interstital
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNewuserlevel, this.IsFailedByInterstitialLevelReached);
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdCommonCooldown, this.IsFailedByInterstitalShareCDOK);
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdConfigDisable, this.IsFailedByInterstitalConfiged);
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdDailyStartCooldown, this.IsFailedByInterstitalLoginTimeOK);
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNofill, this.IsFailedInterstitalByAdNoFilled);
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdPaid, this.IsFailedInterstitalByNoAdsPaid);
        }

        public bool CanPlayRV(string pl)
        {
            // if (LevelGroupSystem.Instance.GetCurLevelCountIdx() < GlobalConfigManager.Instance.GetNumValue("reward_video_unlock"))
            //     return false;
            
            return AdLogicManager.Instance.ShouldShowRV(pl);
        }

        public bool CanPlayRVFilterFill(string pl)
        {
            if (IsFailedByRVCDOK(pl))
                return false;

            if (IsFailedByRVWithinLimit(pl))
                return false;
            
            if (IsFailedByRvReached(pl))
                return false;
            
            if (IsFailedByRvConfiged(pl))
                return false;

            return true;
        }
        public bool CanPlayInterstitial(string pl)
        {
#if UNITY_EDITOR
            return true;
#endif
            if (StoreModel.Instance.IsPaying)
            {
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                    BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventInterstitialAdShouldNotDisplay, pl, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdMysteryPower);
                return false;
            }

            if (IsReachInterstitialOpenLimit(pl))
            {
                return false;
            }
            
            if (IsReachInterstitialDailyLimit(pl))
            {
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                                BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventInterstitialAdShouldNotDisplay, pl, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdOverdisplay, string.Format("{0}_{1}", UserGroupManager.Instance.UserGroup, (CommercialUserType)UserGroupManager.Instance.SubUserGroup));
                return false;
            }

            if (IsBuyNoAds())
            {
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                    BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventInterstitialAdShouldNotDisplay, pl, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdPaid, "RV_NO_INT");
                return false;
            }

            if (IsInNoAdsTime())
            {
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                                BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventInterstitialAdShouldNotDisplay, pl, BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdDailyStartCooldown, "RV_NO_INT");
                return false;
            }

            bool ret = AdLogicManager.Instance.ShouldShowInterstitial(pl);
            Debug.LogError($"Can PlayInterstitial {ret}");
            return ret;
        }

        public bool IsRVPlacementEnable(string placementId)
        {
            RVAd rvAd = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, placementId);
            if (rvAd == null)
            {
                return false;
            }
            return rvAd.LimitPerDay > 0;
        }


        /// <summary>
        /// 开启状态限制
        /// </summary>
        /// <param name="pl"></param>
        /// <returns></returns>
        bool IsReachInterstitialOpenLimit(string pl)
        {
            var commonData =
                AdConfigHandle.Instance.GetCommon(UserGroupManager.Instance.SubUserGroup);
            if (commonData == null)
                return true;
            
            InterstitialAd interstitialAd = AdConfigHandle.Instance.GetInterstitialAd(UserGroupManager.Instance.SubUserGroup, pl);
            if (interstitialAd == null)
                return true;

            bool isLimit = interstitialAd.IsActiveInterAds == 1 ?commonData.ActiveInterOpen == 0:commonData.PassiveInterOpen == 0;
            if (isLimit)
             Debug.LogError(
                    $"插屏开启限制 IsReachInterstitialOpenLimit 是否是主动插屏: {(interstitialAd.IsActiveInterAds == 1).ToString()}");
            
            return isLimit;
            
        }
        
        
        
        /// <summary>
        /// 插屏达到当天不放次数限制
        /// </summary>
        /// <param name="pl"></param>
        /// <returns></returns>
        bool IsReachInterstitialDailyLimit(string pl)
        {
            var commonData =
                AdConfigHandle.Instance.GetCommon(UserGroupManager.Instance.SubUserGroup);
            if (commonData == null)
                return true;
            
            //int limit = commonData.IntTotalPerDay;
            System.DateTime dt = CommonUtils.ConvertFromUnixTimestamp((ulong)m_Storage.InterstitalDayTimestamp);
            System.DateTime next_day = dt + new TimeSpan(24, 0, 0);
            System.DateTime refresh_time = new DateTime(next_day.Year, next_day.Month, next_day.Day, 3, 0, 0);
            if (System.DateTime.Now >= refresh_time)
            {
                m_Storage.InterstitalDayTimestamp = CommonUtils.GetCurTime();
                m_Storage.PlayedInterstitalToday = 0;
                m_Storage.PlayActiveInterTodayNum = 0;
                m_Storage.PlayPassiveInterTodayNum = 0;
            }

            
            // if (m_Storage.PlayedInterstitalToday >= limit)
            //     return true; 
            
            InterstitialAd interstitialAd = AdConfigHandle.Instance.GetInterstitialAd(UserGroupManager.Instance.SubUserGroup, pl);
            if (interstitialAd == null)
                return true;
            
            
            //主动插屏才有当天总限制次数
            if (interstitialAd.IsActiveInterAds == 1 &&
                m_Storage.PlayActiveInterTodayNum >= commonData.ActiveInterTotalPerDay)
            {
                Debug.LogError(
                    $"主动插屏达到当天总数限制 ");
                return true; 

            }
            
            
            if (!m_Storage.InterstitalPlayRecords.ContainsKey(pl))
            {
                StorageAdPlayRecord pr = new StorageAdPlayRecord();
                pr.AdType = interstitialAd.IsActiveInterAds==1 ? 2 : 3;
                pr.LastPlayTime = 0;
                pr.PlayCount = 0;
                m_Storage.InterstitalPlayRecords.Add(pl, pr); 
            }

            System.DateTime l = CommonUtils.ConvertFromUnixTimestamp((ulong)m_Storage.InterstitalPlayRecords[pl].LastPlayTime);
            if (interstitialAd.LimitPerDay > 0 && (System.DateTime.Now.Year > l.Year || System.DateTime.Now.DayOfYear > l.DayOfYear))
            {
                m_Storage.InterstitalPlayRecords[pl].PlayCount = 0;
                return false;
            }


            bool isLimit = m_Storage.InterstitalPlayRecords[pl].PlayCount >= interstitialAd.LimitPerDay;
            if (isLimit)
            {
                Debug.LogError(
                    $"插屏开启限制 {pl} 达到每天播放次数上限 {interstitialAd.LimitPerDay}");
            }
            
            return isLimit;
        }

        void PauseMusic(bool p)
        {
            if (p)
            {
                AudioManager.Instance.MusicClose = true;
            }
            else
            {
                AudioManager.Instance.MusicClose = SettingManager.Instance.MusicClose;
            }
        }

        public void PlayRV(string pl, System.Action<bool> cb, bool outer_reward = false)
        {
            TipRewardModule.Instance.ResetCoolTime();
            
            PauseMusic(true);
            PlayingAdState = 1;
            SkipAPauseInt();
            GamePlayTimeTracker.Instance.StopAllTracking();
            AdLogicManager.Instance.PlayRV(pl, (s, p) =>
            {
                PlayingAdState = 0;
                PauseMusic(false);
                GamePlayTimeTracker.Instance.StartAllTracking();

                this.OnRVRet(s, pl, cb, outer_reward);
            });
        }
        void RecordRvPlayedCount()
        {
            StorageCommon storage = StorageManager.Instance.GetStorage<StorageCommon>();
            GameBIManager.Instance.SendRvThirdBI( storage.AdRewardWatchedCount);
        }

        public void AdRewardRecord(string pl)
        {
        
            StorageAdPlayRecord pr = null;
            if (!m_Storage.AdPlayRecords.TryGetValue(pl, out pr))
            {
                pr = new StorageAdPlayRecord();
                m_Storage.AdPlayRecords.Add(pl, pr);
            }
            pr.AdType = GetAdType(pl);
            pr.CurrentPlayCount = 0;
            pr.SuccessGetRewardCount++;

        }

        public string GetRvText(string pl)
        {
            int currentPlayCount = GetCurrentPlayCount(pl);
            int currentNeedPlayCount = GetNeedPlayCount(pl);
            if (currentNeedPlayCount == 1)
            {
                return LocalizationManager.Instance.GetLocalizedString("");
            }
            else
            {
                return currentPlayCount + "/" + currentNeedPlayCount;
            }
        }
        public int GetCurrentPlayCount(string pl)
        {
            StorageAdPlayRecord pr = null;
            if (!m_Storage.AdPlayRecords.TryGetValue(pl, out pr))
            {
                pr = new StorageAdPlayRecord();
                m_Storage.AdPlayRecords.Add(pl, pr);
            }
            pr.AdType = GetAdType(pl);
            return pr.CurrentPlayCount;
        }     
        public int GetSuccessGetRewardCount(string pl)
        {
            StorageAdPlayRecord pr = null;
            if (!m_Storage.AdPlayRecords.TryGetValue(pl, out pr))
            {
                pr = new StorageAdPlayRecord();
                m_Storage.AdPlayRecords.Add(pl, pr);
            }

            pr.AdType = GetAdType(pl);
            return pr.SuccessGetRewardCount;
        }
    
        public int GetNeedPlayCount(string pl)
        {
            var rv = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, pl);
            if (rv != null)
            {
                return rv.WatchTimes;
            }
            
            return 1;
        }

        public void ActiveInToRvGet(string pl, System.Action cb=null, bool outer_reward=true)
        {
            ADPlayRecord(pl);
            if (outer_reward)
            {
                cb?.Invoke();
                return;
            }

            if (GetCurrentPlayCount(pl) < GetNeedPlayCount(pl))
            {
                cb?.Invoke();
                return;
            }

            AdRewardRecord(pl);
            var rv = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, pl);
            if (rv != null && rv.Bonus > 0)
            {
                var bs = AdConfigHandle.Instance.GetBonus(rv.Bonus,true);
                if (bs != null)
                {
                    CommonRewardManager.Instance.PopCommonReward(bs, CurrencyGroupManager.Instance.GetCurrencyUseController(), true, new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Adv,
                        data2 = pl
                    }, ()=>
                    {
                        cb?.Invoke();
                    });
                }
                else
                {
                    cb?.Invoke();
                }
            }
            else
            {
                cb?.Invoke();
            }
        }
        
        void OnRVRet(bool suc, string pl, System.Action<bool> cb, bool outer_reward)
        {
            if (suc)
            {
                //记录Rv播放时间
                m_Storage.PlayRvTodayTime = CommonUtils.GetCurTime();
                AdLocalConfigHandle.Instance.PausePassInGamePlayTime(true);
                TryRefreshInAdRvNum();
                ADPlayRecord(pl);
                RecordRvPlayedCount();
                AdLocalConfigHandle.Instance.RefreshAdPlayNum(true);
                if (outer_reward)
                {
                    cb?.Invoke(true);
                    return;
                }

                if (GetCurrentPlayCount(pl) < GetNeedPlayCount(pl))
                {
                    cb?.Invoke(true);
                    return;
                }

                AdRewardRecord(pl);
                var rv = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, pl);
                if (rv != null && rv.Bonus > 0)
                {
                    var bs = AdConfigHandle.Instance.GetBonus(rv.Bonus,true);
                    if (bs != null)
                    {
                        CommonRewardManager.Instance.PopCommonReward(bs, CurrencyGroupManager.Instance.GetCurrencyUseController(), true, new GameBIManager.ItemChangeReasonArgs()
                        {
                            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Adv,
                            data2 = pl
                        }, ()=>
                        {
                            cb?.Invoke(true);
                        });
                    }
                    else
                    {
                        cb?.Invoke(true);
                    }
                }
                else
                {
                    cb?.Invoke(true);
                }
            }
            else
            {
                cb?.Invoke(false);
            }

        }

        public void ADPlayRecord(string pl)
        {
            StorageAdPlayRecord pr = null;
            if (!m_Storage.AdPlayRecords.TryGetValue(pl, out pr))
            {
                pr = new StorageAdPlayRecord();
                m_Storage.AdPlayRecords.Add(pl, pr);
            }

            pr.AdType = GetAdType(pl);
            pr.LastPlayTime = CommonUtils.GetCurTime();
            pr.PlayCount++;
            pr.CurrentPlayCount ++;
        }

        public void ADClearRecord(string pl)
        {
            if(m_Storage.AdPlayRecords.ContainsKey(pl))
                return;
            m_Storage.AdPlayRecords.Remove(pl);
        }
        
        public void PlayInterstital(string pl, System.Action<bool> cb)
        {
            PauseMusic(true);
            PlayingAdState = 2;
            GamePlayTimeTracker.Instance.StopAllTracking();
            AdLogicManager.Instance.PlayInterstital(pl, (s, p) =>
            {
                PlayingAdState = 0;
                PauseMusic(false);
                GamePlayTimeTracker.Instance.StartAllTracking();
                OnInterstitalRet(s, pl, cb);
            });
        }

        void OnInterstitalRet(bool suc, string pl, System.Action<bool> cb)
        {
            if (suc)
            {
                m_LastINAdTime = CommonUtils.GetCurTime();
                m_Storage.PlayedInterstitalToday += 1;
                InterstitialAd interstitialAd = AdConfigHandle.Instance.GetInterstitialAd(UserGroupManager.Instance.SubUserGroup, pl);
                //记录插屏次数时间
                if (interstitialAd != null)
                {
                    //主动插屏
                    if ( interstitialAd.IsActiveInterAds == 1)
                    {
                        m_Storage.PlayActiveInterTodayNum++;
                        m_Storage.PlayActiveInterTodayTime = m_LastINAdTime;
                        m_Storage.ActiveInterPlayRvNum = 0;
                    }
                    else
                    {
                        m_Storage.PlayPassiveInterTodayNum++;
                        m_Storage.PlayPassiveInterTodayTime = m_LastINAdTime;
                        m_Storage.PassiveInterCompleteOrderNum = 0;
                        AdLocalConfigHandle.Instance.StartPassInGamePlayTime();

                    }
                }
                
                StorageAdPlayRecord pr = null;
                if (!m_Storage.InterstitalPlayRecords.TryGetValue(pl, out pr))
                {
                    pr = new StorageAdPlayRecord();
                    m_Storage.InterstitalPlayRecords.Add(pl, pr);
                }

                pr.AdType = interstitialAd == null ? 0 : interstitialAd.IsActiveInterAds == 1 ? 2 : 3;
                pr.LastPlayTime = m_LastINAdTime;
                pr.PlayCount++;
                AdLocalConfigHandle.Instance.RefreshAdPlayNum(false);

            }

            cb?.Invoke(suc);
        }

        bool IsFailedRVByAdNotFilled(string pl)
        {
#if !UNITY_EDITOR
            if (!Dlugin.SDK.GetInstance().m_AdsManager.IsRewardReady())
                return true;
#endif
            return false;
        }

        bool IsFailedByRvReached(string pl)
        {
        
            Common common = AdConfigHandle.Instance.GetCommon(UserGroupManager.Instance.SubUserGroup);
            if (common == null)
            {
                Debug.LogError($"IsFailedByInterstitialLevelReached(): Not found {pl} config !!!");
                return false;
            }
            return ExperenceModel.Instance.GetLevel()< common.RvUnlock;
        }
        public bool IsFailedByRvConfiged(string pl)
        {
            RVAd rvAd = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, pl);
            if (rvAd == null)
            {
                Debug.LogError($"IsFailedByRvConfiged(): Not found {pl} config !!!");
                return false;
            }
            return rvAd.IfOpen == 0;
        }
        public bool IsRVReady()
        {
#if !UNITY_EDITOR
            if (!Dlugin.SDK.GetInstance().m_AdsManager.IsRewardReady())
                return false;
#endif
            return true;
        }
        
        bool IsFailedByInterstitialLevelReached(string pl)
        {
            Common common = AdConfigHandle.Instance.GetCommon(UserGroupManager.Instance.SubUserGroup);
            if (common == null)
            {
                Debug.LogError($"IsFailedByInterstitialLevelReached(): Not found {pl} config !!!");
                return true;
            }

            int level = ExperenceModel.Instance.GetLevel();
            if (level < common.InterstitialUnlock)
                return true;
            
            InterstitialAd interstitialAd = AdConfigHandle.Instance.GetInterstitialAd(UserGroupManager.Instance.SubUserGroup, pl);
            if (interstitialAd == null)
                return true;

            return interstitialAd.UnlockLevel > 0 && level < interstitialAd.UnlockLevel;
        }

        bool IsFailedInterstitalByAdNoFilled(string pl)
        {
#if !UNITY_EDITOR
            if(!Dlugin.SDK.GetInstance().m_AdsManager.IsInterstitialReady())
                return true;
#endif

            return false;
        }

        bool IsFailedByInterstitalShareCDOK(string pl)
        {
            Common common = AdConfigHandle.Instance.GetCommon(UserGroupManager.Instance.SubUserGroup);
            if (common == null)
            {
                Debug.LogError($"IsFailedByInterstitalShareCDOK(): Not found {pl} config !!!");
                return true;
            }

            //去掉整体时长间隔判断区分主动被动
            // if(Math.Abs(CommonUtils.GetCurTime() - m_LastINAdTime) < (common.AdsMinInterval * 1000))
            //     return true;
            
            InterstitialAd interstitialAd = AdConfigHandle.Instance.GetInterstitialAd(UserGroupManager.Instance.SubUserGroup, pl);
            if (interstitialAd == null)
            {
                Debug.LogError($"IsFailedBy  InterstitialAd Null: Not found {pl} config !!!");
                return true;
            }
            
            //没初始化的不做限制
            if (!m_Storage.InterstitalPlayRecords.ContainsKey(pl))
                return false;


            //主动插屏
            if (interstitialAd.IsActiveInterAds==1)
            {
                int activeInState = 0;
                if (m_Storage.PlayActiveInterTodayNum<=common.ActiveInterShowNum[0])
                    activeInState = 0;
                else if (m_Storage.PlayActiveInterTodayNum > common.ActiveInterShowNum[0] && m_Storage.PlayActiveInterTodayNum <= common.ActiveInterShowNum[1])
                    activeInState = 1;
                else
                    activeInState = 2;

                int timeLimit = common.ActiveInterShowRvCD[activeInState];
                int rvLimit = common.ActiveInterShowTimeCD[activeInState];
                
                bool isFailedTimeLimit = Math.Abs(CommonUtils.GetCurTime() - m_Storage.PlayActiveInterTodayTime) < (timeLimit * 1000);
                bool isFailedRvLimit = m_Storage.ActiveInterPlayRvNum < rvLimit;
                
                
                Debug.LogError($"主动插屏广告播放间隔限制 {pl} {isFailedTimeLimit}  cur: {Math.Abs(CommonUtils.GetCurTime() - m_Storage.InterstitalPlayRecords[pl].LastPlayTime)}  Limit: {timeLimit * 1000}");
                Debug.LogError($"主动插屏广告Rv播放限制 {pl} {isFailedRvLimit}  cur: { m_Storage.ActiveInterPlayRvNum}  Limit: {rvLimit}");
                
                bool isFailed = isFailedTimeLimit && isFailedRvLimit;
                if (isFailed) 
                    return true;
            }
            else
            {
                bool isFailedRvPlayTimeLimit =m_Storage.PlayRvTodayTime > 0 && Math.Abs(CommonUtils.GetCurTime() -m_Storage.PlayRvTodayTime) < (common.RvShowPause * 1000);
                Debug.LogError($"被动动插屏 Rv播放豁免 {pl} {isFailedRvPlayTimeLimit}  cur: { Math.Abs(CommonUtils.GetCurTime() -m_Storage.PlayRvTodayTime)}  Limit: {common.RvShowPause * 1000}" );
                
                if (isFailedRvPlayTimeLimit)
                    return true;
                
                
                int timeLimit = common.PassiveInterShowCD[0];
                int orderLimit = common.PassiveInterShowCD[1];
                
                //被动插屏用净游戏时长判断时间限制
                bool isFailedTimeLimit = AdLocalConfigHandle.Instance.GetPassInGamePlayTime() < timeLimit ;
                bool isFailedOrderLimit = m_Storage.PassiveInterCompleteOrderNum < orderLimit;
                Debug.LogError($"被动动插屏 广告净时长限制 {pl} {isFailedTimeLimit}  cur: {AdLocalConfigHandle.Instance.GetPassInGamePlayTime()}  Limit: {timeLimit}");
                Debug.LogError($"被动动插屏 广告完成任务数量限制 {pl} {isFailedOrderLimit}  cur: {m_Storage.PassiveInterCompleteOrderNum}  Limit: {orderLimit}" );
                
                bool isFailed = isFailedTimeLimit && isFailedOrderLimit;
                if (isFailed)
                    return true;
            }

            return false;
        }

        bool IsFailedInterstitalByNoAdsPaid(string pl)
        {
            return StorageManager.Instance.GetStorage<StorageHome>().ShopData.GotNoAds;
        }
        
        bool IsFailedByInterstitalLoginTimeOK(string pl)
        {
            Common common = AdConfigHandle.Instance.GetCommon(UserGroupManager.Instance.SubUserGroup);
            if (common == null)
            {
                Debug.LogError($"IsFailedByInterstitalLoginTimeOK(): Not found {pl} config !!!");
                return true;
            }
            return TimeAfterFirstLogin.TotalSeconds < common.AdOpenTime;
        }

        bool IsFailedByInterstitalConfiged(string pl)
        {
            InterstitialAd interstitialAd = AdConfigHandle.Instance.GetInterstitialAd(UserGroupManager.Instance.SubUserGroup, pl);
            if (interstitialAd == null)
            {
                Debug.LogError($"IsFailedByInterstitalConfiged(): Not found {pl} config !!!");
                return false;
            }
            return interstitialAd.IfOpen == 0;
        }
       

        bool IsFailedByRVCDOK(string pl)
        {
            var c = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, pl);
            //没有次数限制
            if (c == null)
            {
                return false;
            }
            if (!m_Storage.AdPlayRecords.ContainsKey(pl))
                return false;

            if (Math.Abs(CommonUtils.GetCurTime() - m_Storage.AdPlayRecords[pl].LastPlayTime) < (c.ShowInterval * 1000))
            {
                return true;
            }

            return false;
        }

        public bool IsFailedByRVWithinLimit(string pl)
        {
            var c = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, pl);

            //没有次数限制
            if (c == null)
            {
                return false;
            }

            if (!m_Storage.AdPlayRecords.ContainsKey(pl))
            {
                StorageAdPlayRecord pr = new StorageAdPlayRecord();
                pr.AdType = 1;
                pr.LastPlayTime = 0;
                pr.PlayCount = 0;
                pr.CurrentPlayCount = 0;
                pr.SuccessGetRewardCount = 0;
                m_Storage.AdPlayRecords.Add(pl, pr); 
            }

            System.DateTime l = CommonUtils.ConvertFromUnixTimestamp((ulong)m_Storage.AdPlayRecords[pl].LastPlayTime);
            if (c.LimitPerDay > 0 && (System.DateTime.Now.Year > l.Year || System.DateTime.Now.DayOfYear > l.DayOfYear))
            {
                m_Storage.AdPlayRecords[pl].PlayCount = 0;
                m_Storage.AdPlayRecords[pl].CurrentPlayCount = 0;
                m_Storage.AdPlayRecords[pl].SuccessGetRewardCount = 0;
                return false;
            }

            return m_Storage.AdPlayRecords[pl].PlayCount >= c.LimitPerDay;
        }
        public int GetRvLeftCount(string pl)
        {
            var c = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, pl);

            //没有次数限制
            if (c == null)
            {
                return 0;
            }

            if (!m_Storage.AdPlayRecords.ContainsKey(pl))
            {
                StorageAdPlayRecord pr = new StorageAdPlayRecord();
                pr.AdType = 1;
                pr.LastPlayTime = 0;
                pr.PlayCount = 0;
                pr.CurrentPlayCount = 0;
                pr.SuccessGetRewardCount = 0;
                m_Storage.AdPlayRecords.Add(pl, pr); 
            }

            System.DateTime l = CommonUtils.ConvertFromUnixTimestamp((ulong)m_Storage.AdPlayRecords[pl].LastPlayTime);
            if (c.LimitPerDay > 0 && (System.DateTime.Now.Year > l.Year || System.DateTime.Now.DayOfYear > l.DayOfYear))
            {
                m_Storage.AdPlayRecords[pl].PlayCount = 0;
                m_Storage.AdPlayRecords[pl].CurrentPlayCount = 0;
                m_Storage.AdPlayRecords[pl].SuccessGetRewardCount = 0;
                return c.LimitPerDay ;
            }

            return c.LimitPerDay-m_Storage.AdPlayRecords[pl].PlayCount;
        }


        public int GetAdType(string pl)
        {
            var rvAd = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, pl);
            if (rvAd != null)
                return 1;
            InterstitialAd interstitialAd = AdConfigHandle.Instance.GetInterstitialAd(UserGroupManager.Instance.SubUserGroup, pl);
            if (interstitialAd != null)
            {
                return interstitialAd.IsActiveInterAds == 1 ? 2 : 3;
            }

            return 0;
        }
        

        private void OnApplicationQuit()
        {
            // GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventAppQuit, PlayingAdState.ToString());
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventAppPause, PlayingAdState.ToString());
                return;
            }


            if (isPlayRv)
            {
                isPlayRv = false;
                return;
            }

            if (!SceneFsm.mInstance.ClientInited)
                return;

            if (!SceneFsm.mInstance.ClientLogin)
                return;

            if (!CanPlayInterstitial(ADConstDefine.IN_BackGround))
                return;

            if (SceneFsm.mInstance != null && SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            {
                return;
            }

            if (CheckSkipPauseInt()) {
                return;
            }

            AdSubSystem.Instance.PlayInterstital(ADConstDefine.IN_BackGround, b =>
            {
            
            });
        }


        /// <summary>
        /// 尝试刷新任务完成次数
        /// </summary>
        public void TryRefreshInAdOrderNum()
        {
            m_Storage.PassiveInterCompleteOrderNum++;

        }
        /// <summary>
        /// 尝试刷新Rv播放次数
        /// </summary>
        public void TryRefreshInAdRvNum()
        {
            m_Storage.ActiveInterPlayRvNum++;
        }
        
        #region NoAdTime
        // public void AddNoAdTime(int timeInSecond, BiEventMakeOver.Types.ItemChangeReason r, string data1 = "", string data2 = "", string data3 = "", string data4 = "")
        // {
        //     int timems = timeInSecond * 1000;
        //     if (IsInNoAdsTime())
        //     {
        //         m_Storage.NoADEndTime += timems;
        //     }
        //     else
        //     {
        //         m_Storage.NoADEndTime = CommonUtils.GetTimeStamp() + timems;
        //     }
        //
        //     GameBIManager.Instance.ItemChangeBI((int)ResourceId.RemoveAdWithTime, GetNoAdsLeftTime(), timeInSecond, r, data1, data2, data3, data4);
        // }

        public void DebugSetNoAddTime(int sec)
        {
            if (!Debug.isDebugBuild)
                return;

            m_Storage.NoADEndTime = CommonUtils.GetTimeStamp() + sec * 1000;
        }

        public bool IsBuyNoAds()
        {
            StorageHome storageHome = StorageManager.Instance.GetStorage<StorageHome>();
            return storageHome.ShopData.GotNoAds;
        }
        public bool IsInNoAdsTime()
        {
            return m_Storage.NoADEndTime >= CommonUtils.GetTimeStamp();
        }

        public int GetNoAdsLeftTime()
        {
            if (IsInNoAdsTime())
            {
                return (int)((m_Storage.NoADEndTime - CommonUtils.GetTimeStamp()) / 1000);
            }
            else
            {
                return 0;
            }
        }

        public string GetNoAdsLeftTimeString()
        {
            if (!IsInNoAdsTime())
            {
                return "00:00";
            }

            int left_sec = (int)((m_Storage.NoADEndTime - CommonUtils.GetTimeStamp()) / 1000);

            if (left_sec > 3600)
            {
                int left_min = left_sec / 60;

                int hour = left_min / 60;
                int minute = left_min % 60;

                return string.Format("{0:D2}h{1:D2}m", hour, minute);
            }
            else
            {
                int minute = left_sec / 60;
                int sec = left_sec % 60;

                return string.Format("{0:D2}:{1:D2}", minute, sec);
            }
        }
        #endregion
    }
    
}

