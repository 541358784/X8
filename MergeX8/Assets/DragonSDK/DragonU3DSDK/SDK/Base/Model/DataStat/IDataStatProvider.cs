using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dlugin
{
    namespace PluginStructs
    {
        [System.Serializable]
        public class DataStatPurchase : PaymentInfo
        {
            public string userId;
            public int revenue;

            public DataStatPurchase CopyPaymentInfo(PaymentInfo pi)
            {
                DataStatPurchase ret = this;
                ret.count = pi.count;
                ret.desc = pi.desc;
                ret.payload = pi.payload;
                ret.paymentId = pi.paymentId;
                ret.productId = pi.productId;
                ret.productType = pi.productType;
                ret.purchaseInfo = pi.purchaseInfo;
                ret.purchaseReceipt = pi.purchaseReceipt;
                ret.purchaseSignature = pi.purchaseSignature;
                return ret;
            }
        }
        [System.Serializable]
        public class DataStatLogin
        {
            public string userId;
        }
    }

    [Obsolete("IDataStatProvider is deprecated, please use Dlugin.adjustPlugin or Dlugin.firebasePlugin instead.")]
    public abstract class IDataStatProvider : IServiceProvider
    {
        public abstract void Initialize();
        public abstract void TrackEvent(string eventId, long secondsInUTC, string info);
        public abstract string GetTrackeeID();
        public abstract string GetTrackerUrl(string name);
    }
}
