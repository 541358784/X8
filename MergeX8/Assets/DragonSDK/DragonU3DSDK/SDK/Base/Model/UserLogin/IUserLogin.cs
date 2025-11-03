using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Dlugin.PluginStructs;

namespace Dlugin
{
    public abstract class IUserLogin : IServiceProvider
    {
        public abstract UserInfo GetUserInfo();
        public abstract int GetUserLoginStatus();           //see login status in constant
        public abstract void Login(int[] allPermission);
        public abstract void ReauthToken(int[] allPermission);
        public abstract bool CheckLoggedIn();
        public abstract void RefreshUserInfo();
        public abstract void ChangePermission(int[] allPermission);
        public abstract void Logout();
        public abstract void LogAdEvent(AD_Type ad_Type, AD_Event_Type event_Type);
        public abstract void LogPurchaseEvent(decimal price, string currency, string productID);
    }

}