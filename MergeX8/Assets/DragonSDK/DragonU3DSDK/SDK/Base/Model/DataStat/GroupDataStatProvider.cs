using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin.PluginStructs;

namespace Dlugin
{
    public class GroupDataStatProvider : GroupServiceProvider
    {
        #region utility method
        //some common event utility
        public void TrackLogin(DataStatLogin login, long secondsInUTC)
        {
            TrackEvent(Constants.kEventTypeLogin, secondsInUTC, JsonUtility.ToJson(login));
        }
        public void TrackPurchase(DataStatPurchase purchase, long secondsInUTC)
        {
            TrackEvent(Constants.kEventTypePurchase, secondsInUTC, JsonUtility.ToJson(purchase));
        }
        public void TrackLaunch(long secondsInUTC)
        {
            TrackEvent(Constants.kEventTypeLaunch, secondsInUTC, "");
        }
        #endregion

        public void TrackEvent(string eventId, long secondsInUTC, string infomation)
        {
            foreach(IServiceProvider provider in m_AllService)
            {
                IDataStatProvider iData = provider as IDataStatProvider;
                iData.TrackEvent(eventId, secondsInUTC, infomation);
            }
        }

        public string GetTrackeeID()
        {
            foreach (IServiceProvider provider in m_AllService)
            {
                IDataStatProvider iData = provider as IDataStatProvider;
                string id = iData.GetTrackeeID();
                if (!string.IsNullOrEmpty(id))
                {
                    return id;
                }
            }

            return null;
        }
    }
}
