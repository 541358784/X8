using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin.PluginStructs;

namespace Dlugin
{
    public class AdsEventDispatcher 
    {
        public event System.Action<AdsUnitDefine, SDKError> onAdsLoadFinish;  
        public event System.Action<AdsUnitDefine, bool> onAdsAvailabilityChanged;
        public event System.Action<AdsUnitDefine> onAdsImpression;
        public event System.Action<AdsUnitDefine> onAdsWatched;         //called when ads is watched
        public event System.Action<AdsUnitDefine,  SDKError> onAdsPlayFinish;  //called when ads is finished play
        public event System.Action<AdsUnitDefine> onAdsClick;  //called when ads is clicked

        public void ProcessAdsLoaded(AdsUnitDefine unit, SDKError error)
        {
            if (onAdsLoadFinish != null)
            {
                onAdsLoadFinish(unit, error);
            }
        }

        public void ProcessAdsImpression(AdsUnitDefine unit)
        {
            if (onAdsImpression != null)
            {
                onAdsImpression(unit);
            }
        }

        public void ProcessAdsWatched(AdsUnitDefine unit)
        {
            if (onAdsWatched != null)
            {
                onAdsWatched(unit);
            }
        }

        public void ProcessAdsClick(AdsUnitDefine unit)
        {
            if (onAdsClick != null)
            {
                onAdsClick(unit);
            }
        }


        public void ProcessAdsPlayFinished(AdsUnitDefine unit, SDKError error)
        {
            if (onAdsPlayFinish != null)
            {
                onAdsPlayFinish(unit,  error);
            }
        }

        public void ProcessAdsAvailabilityChanged(AdsUnitDefine unit, bool newState)
        {
            if (onAdsAvailabilityChanged != null)
            {
                onAdsAvailabilityChanged(unit, newState);
            }
        }
    }
}
