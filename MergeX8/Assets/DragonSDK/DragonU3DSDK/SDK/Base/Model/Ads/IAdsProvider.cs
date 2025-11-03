using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin.PluginStructs;

namespace Dlugin
{
    public enum AD_Type { Unknown, Interstitial, RewardVideo, CrossPromotion, Banner, OfferWall, Mrec, Count }
    public enum AD_PlayingProgress { Idle, Playing, Rewarded }
    public enum AD_Event_Type { Impression, Click }

    namespace PluginStructs
    {
        [System.Serializable]
        public class AdsUnitDefine
        {
            public AdsUnitDefine()
            {
                m_DestorySelf = true;
            }
            public string GetKey()
            {
                if (string.IsNullOrEmpty(_key))
                {
                    _key = string.Format("{0}@{1}", m_PluginParam, m_PluginName);
                }
                return _key;
            }


            private string  _key;
            public  string  m_PluginParam;
            public  string  m_PluginName;
            public  string  m_Placement;
            public  string  m_NetworkName;
            public  AD_Type m_Type;
            public  bool    m_DestorySelf;
            //该广告位 ecpm floor
            public double m_EcpmFloor = 0;
            //该渠道权重
            public int m_Weight = 0;

            public bool m_Need_Reward;

            public Action<bool> show_pre_callback;
            public Action<bool> show_post_callback;

            public int m_wait_request_millisecond;

            public string m_AdUnitIdentifier;
            public string m_NetworkPlacement;
            public string m_CreativeIdentifier;
            public string m_RevenuePrecision;
            public string m_DspName;

            public string m_InstanceId;
        }

        [System.Serializable]
        public class RETAdsUnitArray
        {
            public AdsUnitDefine[] units;
        }

        [System.Serializable]
        public class PreloadAdsParam
        {
            public string m_Placement;
            public AD_Type m_Type;
        }
    }
    /// <summary>
    /// 各广告渠道须继承此类
    /// </summary>
    public abstract class IAdsProvider : IServiceProvider
    {
        //奖励视频权重
        public int m_RewardWeight;
        //插屏权重
        public int m_InterstitialWeight;

        public int m_MaxRetryTimes;
        public float m_RetryCacheWaitSeconds;
        protected float m_StartPlayAdsTime;
        protected AD_PlayingProgress m_VideoPlayProgress;

        public abstract void Initialize();
        public abstract bool PlayAds(AdsUnitDefine unit, string placement = null);
        public abstract bool DisposeAd(AdsUnitDefine unit);
        public abstract bool IsAdsReady(AdsUnitDefine unit);
        public abstract List<AdsUnitDefine> PreloadAds();

        public abstract void SpendCurrency();

        public abstract void SetMuted(bool muted);
        public abstract bool IsMuted();
        
        public abstract void ShowBanner();
        public abstract void HideBanner();

        public abstract void ShowMRec();
        public abstract void HideMRec();


        public abstract void UpdateBannerPosition(float x, float y);

        public abstract void UpdateMRECPosition(float x, float y);

        public abstract void SetBannerWidth(float width);


        public abstract void SetTestDeviceAdvertisingIdentifiers(string adid);

        public abstract void HandleLoad(AD_Type type);

        public abstract double GetLoadedAdRevenue(AD_Type type);

        public override string ToString()
        {
            return string.Format("[AdProvider.Config myDefine:{0}]", m_PluginDefine.ToString());
        }
    }
}
