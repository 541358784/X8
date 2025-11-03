using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin.PluginStructs;

namespace Dlugin
{
    public class GroupAdsProvider : GroupServiceProvider
    {
        public bool PlayAds(AdsUnitDefine param)
        {
            foreach(IServiceProvider provider in m_AllService)
            {
                IAdsProvider ads = provider as IAdsProvider;
                if(ads.PlayAds(param))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsAdsReady(AdsUnitDefine adsUnit)
        {
            foreach (IServiceProvider provider in m_AllService)
            {
                IAdsProvider ads = provider as IAdsProvider;
                if (ads.IsAdsReady(adsUnit))
                {
                    return true;
                }
            }
            return false;
        }

        public void PreloadAds(PreloadAdsParam preParams)
        {
            foreach (IServiceProvider provider in m_AllService)
            {
                IAdsProvider ads = provider as IAdsProvider;
                ads.PreloadAds();
            }
        }
    }
}