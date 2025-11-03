using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin;

namespace Dlugin.PluginStructs
{
    [System.Serializable]
    public class MessageBase
    {
        public int msg;
        public string pluginId;
        public string context;
        public SDKError error;
        public void Clear()
        {
            msg = 0;
            pluginId = "";
            context = "";
            error = null;
        }
    }

    [System.Serializable]
    public class MSGPayment : MessageBase
    {
        public PaymentInfo paymentInfo;
    }

    [System.Serializable]
    public class MSGPaymentArray : MessageBase
    {
        public PaymentInfo[] paymentInfos;
    }

    [System.Serializable]
    public class MSGProductArray : MessageBase
    {
        public ProductInfo[] productInfos;
    }

    [System.Serializable]
    public class MSGAdsLoaded : MessageBase
    {
        public AdsUnitDefine adsUnit;
    }

    [System.Serializable]
    public class MSGAdsWatched : MessageBase
    {
        public AdsUnitDefine adsUnit;
    }

    [System.Serializable]
    public class MSGAdsPlayFinished : MessageBase
    {
        public AdsUnitDefine adsUnit;
    }

    [System.Serializable]
    public class MSGAdsAvailabilityChanged : MessageBase
    {
        public AdsUnitDefine adsUnit;
        public bool newAvailability;
    }

    [System.Serializable]
    public class MSGNetworkStatusChanged : MessageBase
    {
        public int newStatus;
    }
}