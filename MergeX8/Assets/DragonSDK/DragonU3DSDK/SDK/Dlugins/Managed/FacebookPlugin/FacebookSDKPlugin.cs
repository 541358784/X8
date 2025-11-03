using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Dlugin.PluginStructs;
using System;
using DragonU3DSDK;
using Newtonsoft.Json;

namespace Dlugin
{
    public class FacebookSDKPlugin : IUserLogin
    {
        public void Initialize()
        {
            m_MyDefine = new PluginDefine();
            m_MyDefine.m_PluginName = Constants.FaceBook;
            m_MyDefine.m_PluginVersion = "1.0";

            if (!FB.IsInitialized)
            {
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                FB.ActivateApp();
            }

            DebugUtil.Log("FacebookSDKPlugin.Initialize ----> the fb appId is {0}", FB.AppId);
        }
        public override UserInfo GetUserInfo()
        {
            return m_UserInfo;
        }
        public override int GetUserLoginStatus()           //see login status in constant
        {
            if (m_UserInfo != null)
                return m_UserInfo.status;
            else
                return Constants.kLoginStatusUnknown;
        }
        public  bool IsFacebookLimitedLoginMode()
        {
#if UNITY_IOS
            DebugUtil.Log("facebook iosOSVersion: " + DeviceHelper.GetOSVersion());
            if (DeviceHelper.s_mainOSVersion >= 17)
            {
                DebugUtil.Log("facebook iOS version is greater than 17.0, iosOSVersion={0}", DeviceHelper.s_mainOSVersion);
                return true;
            }
            else
            {
                DebugUtil.Log("facebook iOS version is less than 17.0,  iosOSVersion={0}", DeviceHelper.s_mainOSVersion);
                return false;
            }
#else
            return false;
#endif
        }

        public override void Login(int[] allPermission)
        {
            DebugUtil.Log("FacebookSDKPlugin.Login");
            List<string> perms = decodePermission(allPermission);
            if (IsFacebookLimitedLoginMode())
            {
                if (_IsTokenExists())
                {
                    DebugUtil.Log("FacebookSDKPlugin.Login LIMITED ----> there is a existing token");
                    LimitedAuthCallback(null);
                    return;
                }
                FB.Mobile.LoginWithTrackingPreference(LoginTracking.LIMITED, decodePermission(allPermission), "dragonplus123", (result) => LimitedAuthCallback(result));
                return;
            }
            if (_IsTokenExists())
            {
                DebugUtil.Log("FacebookSDKPlugin.Login ----> there is a existing token");
                //have a recentlly token, and token is not expire
                bool allIn = true;
                IEnumerable<string> permissions = AccessToken.CurrentAccessToken.Permissions;
                foreach (var p in perms)
                {
                    if (permissions.IndexOfEx(p) < 0)
                    {
                        //there is one permission that is not satisfy
                        DebugUtil.Log("FacebookSDKPlugin.Login ----> permission {0} requested is not exists", p);
                        allIn = false;
                    }
                }
                if (!allIn)
                {
                    DebugUtil.Log("FacebookSDKPlugin.Login ----> because permission not satisfy, we still will login.");
                }
                else
                {
                    AuthCallback(null);
                    return;
                }
            }
            if (perms.Contains("publish_actions"))
            {
                FB.LogInWithPublishPermissions(decodePermission(allPermission), (result) => AuthCallback(result));
            }
            else
            {
                FB.LogInWithReadPermissions(decodePermission(allPermission), (result) => AuthCallback(result));
            }
        }

        public override void ReauthToken(int[] allPermission)
        {
            DebugUtil.Log("facebooksdk ReauthToken 1");
            List<string> perms = decodePermission(allPermission);
            if (IsFacebookLimitedLoginMode())
            {
                FB.Mobile.LoginWithTrackingPreference(LoginTracking.LIMITED, decodePermission(allPermission), "dragonplus123", (result) => LimitedReAuthCallback(result));
            }
            else
            {
                if (perms.Contains("publish_actions"))
                {
                    FB.LogInWithPublishPermissions(decodePermission(allPermission), (result) => ReAuthCallback(result));
                }
                else
                {
                    FB.LogInWithReadPermissions(decodePermission(allPermission), (result) => ReAuthCallback(result));
                }
            }
        }

        public AccessToken GetAccessToken()
        {
            return AccessToken.CurrentAccessToken;
        }

        public override bool CheckLoggedIn()
        {
            DebugUtil.Log("FB IsloggedIn is " + FB.IsLoggedIn);
            DebugUtil.Log("FB m_UserInfo is " + m_UserInfo);
            return FB.IsLoggedIn && m_UserInfo != null;
        }

        public override void RefreshUserInfo()
        {
            DebugUtil.Log("FacebookSDKPlugin.Refreshing UserInfo");
            FB.API("/me?fields=name,id,email,picture", HttpMethod.GET, (res) =>
            {
                if (res.Cancelled)
                {
                    SDK.GetInstance().loginDispatcher.ProcessRefreshUserInfoOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorUserCanceled, 0, "canceled"));
                }
                else if (!string.IsNullOrEmpty(res.Error))
                {
                    SDK.GetInstance().loginDispatcher.ProcessRefreshUserInfoOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorChannelError, -1, res.Error));
                }
                else
                {
                    if (m_UserInfo == null)
                    {
                        SDK.GetInstance().loginDispatcher.ProcessRefreshUserInfoOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorNotLogin, 0, "should login first"));
                    }
                    else
                    {

                        Dictionary<string, object> result = res.ResultDictionary as Dictionary<string, object>;
                        m_UserInfo.userName = result.GetDefault("name", "") as string;
                        m_UserInfo.email = result.GetDefault("email", "") as string;
                        DebugUtil.Log("FacebookSDKPlugin.Refreshing UserInfo {0}  {1}", m_UserInfo.userName, m_UserInfo.email);
                        SDK.GetInstance().loginDispatcher.ProcessRefreshUserInfoOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorSuccess, 0, ""));
                    }
                }
            });
        }
        public override void ChangePermission(int[] allPermission)
        {
            DebugUtil.Log("FacebookSDKPlugin.ChangePermission");
            List<string> perms = decodePermission(allPermission);
            if (perms.Contains("publish_actions"))
            {
                FB.LogInWithPublishPermissions(decodePermission(allPermission), (result) => PermissionCallback(result));
            }
            else
            {
                FB.LogInWithReadPermissions(decodePermission(allPermission), (result) => PermissionCallback(result));
            }
        }
        public override void Logout()
        {
            FB.LogOut();
            m_UserInfo = null;
            SDK.GetInstance().loginDispatcher.ProcessLogoutOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorSuccess, 0, ""));
        }

        public override void DisposePlugin(string pluginName)
        {
            m_MyDefine = null;
        }

        public string GetAppId()
        {
            return FB.AppId;
        }

        public override void LogPurchaseEvent(decimal price, string currency, string productID)
        {
            var iapParameters = new Dictionary<string, object>();
            iapParameters["product_id"] = productID;
            FB.LogPurchase(
              price,
              currency,
              iapParameters
            );

            if (FB.IsInitialized)
                FB.LogPurchase(
                        price,
                        currency,
                        iapParameters
                );
        }


        public override void LogAdEvent(AD_Type ad_Type, AD_Event_Type event_Type)
        {

            string adType = "";
            switch (ad_Type)
            {
                case AD_Type.Interstitial:
                    adType = "Interstitial";
                    break;
                case AD_Type.RewardVideo:
                    adType = "Rewarded";
                    break;
                default:
                    DebugUtil.LogWarning("Ad Type Not Supported");
                    return;
            }

            string adEventName = "";
            switch (event_Type)
            {
                case AD_Event_Type.Click:
                    adEventName = "AdClick";
                    break;
                case AD_Event_Type.Impression:
                    adEventName = "AdImpression";
                    break;
                default:
                    DebugUtil.LogWarning("Ad Event Type Not Supported");
                    return;
            }

            Dictionary<string, object> param = new Dictionary<string, object>();
            param["ad_type"] = adType;
            if (FB.IsInitialized)
                FB.LogAppEvent(adEventName, 1, param);
        }


        private List<string> decodePermission(int[] permissions)
        {
            List<string> ret = new List<string>();

            if (permissions.CheckContains(Constants.kUserPermissionBasicInfo))
            {
                ret.Add("public_profile");
                ret.Add("email");
            }
            if (permissions.CheckContains(Constants.kUserPermissionPublish))
            {
                ret.Add("publish_actions");
            }
            if (permissions.CheckContains(Constants.kUserPermissionFriendList))
            {
                ret.Add("user_friends");
            }
            return ret;
        }
        private int[] encodePermission(List<string> permissions)
        {
            List<int> ret = new List<int>();
            ret.Add(Constants.kUserLoginService);
            if (permissions.Contains("public_profile"))
            {
                ret.Add(Constants.kUserPermissionBasicInfo);
            }
            if (permissions.Contains("publish"))
            {
                ret.Add(Constants.kUserPermissionPublish);
            }
            if (permissions.Contains("user_friends"))
            {
                ret.Add(Constants.kUserPermissionFriendList);
            }
            return ret.ToArray();
        }

        private bool _IsTokenExists()
        {  
            if (IsFacebookLimitedLoginMode())
            {
                return _IsLimitedTokenExists();
            }
            else
                return AccessToken.CurrentAccessToken != null && System.DateTime.Compare(AccessToken.CurrentAccessToken.ExpirationTime, System.DateTime.Now) > 0;
        }

        public bool _IsLimitedTokenExists()
        {
            var token = FB.Mobile.CurrentAuthenticationToken();
            if (token == null)
                return false;
            var nonce = token.Nonce;
            if (nonce != "dragonplus123")
            {
                DebugUtil.Log("facebook token nonce check error!nonce={0}", nonce);
                return false;
            }
            var parts = token.TokenString.Split('.');
            if (parts.Length != 3)
            {
                DebugUtil.Log("facebook Token is not valid JWT");
                return false;
            }
            var payload = parts[1]; // PAYLOAD部分在第2个部分
            try
            {
                var payloadJson = System.Text.Encoding.UTF8.GetString(Base64UrlDecode(payload));
                var table = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(payloadJson);
                if (table != null && table.ContainsKey("exp")) 
                {
                    var expiration = Utils.ParseTime(long.Parse(table["exp"].ToString()));
                    bool isE = System.DateTime.Compare(expiration, System.DateTime.UtcNow) > 0;
                    DebugUtil.Log("facebook limited token, Expiration: {0}, UtcNow: {1}, isE={2}", expiration, System.DateTime.UtcNow, isE);
                    return isE;
                }
                return false;
            }
            catch (JsonReaderException e)
            {
                DebugUtil.Log("facebook token error format = " + e.ToString());
                return false;
            }
        }

        public static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break;  // One pad char
                default: throw new Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }

        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                SDK.FormatWarning("FacebookSDKPlugin ----> Failed to Initialize Facebook SDK");
            }
        }

        private void AuthCallback(ILoginResult result)
        {
            if (_IsTokenExists())
            {
                SDK.FormatWarning("进来了1");
                m_UserInfo = new UserInfo();
                SDK.FormatWarning("进来了2");
                m_UserInfo.userToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
                SDK.FormatWarning("进来了3");
                m_UserInfo.userId = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
                SDK.FormatWarning("进来了4");
                m_UserInfo.userName = "";
                SDK.FormatWarning("进来了5");
                m_UserInfo.status = Constants.kLoginStatusUserLogin;
                SDK.FormatWarning("进来了6");
                m_UserInfo.userPermissions = encodePermission(Facebook.Unity.AccessToken.CurrentAccessToken.Permissions.ToListEx());
                // Print current access token's granted permissions
                SDK.FormatWarning("进来了7");
                m_UserInfo.facebookMode = 0;
                SDK.FormatWarning("进来了8");
                foreach (string perm in Facebook.Unity.AccessToken.CurrentAccessToken.Permissions)
                {
                    DebugUtil.Log(perm);
                }
                SDK.GetInstance().loginDispatcher.ProcessLoginOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorSuccess, 0, ""));
            }
            else if (result == null)
            {
                SDK.FormatWarning("FacebookSDKPlugin.AuthCallback ----> result is null, but token is illigal, this shouldn't happen");
                SDK.GetInstance().loginDispatcher.ProcessLoginOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorUnknown, 0, "result is null, but token is illigal, this shouldn't happen"));
            }
            else if (result.Cancelled)
            {
                DebugUtil.Log("FacebookSDKPlugin.AuthCallback ----> login canceled");
                SDK.GetInstance().loginDispatcher.ProcessLoginOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorUserCanceled, 0, "user canceled"));
            }
            else
            {
                DebugUtil.Log("FacebookSDKPlugin.AuthCallback ----> login error " + result.Error);
                SDK.GetInstance().loginDispatcher.ProcessLoginOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorChannelError, -1, result.Error));
            }
        }

        
        private void LimitedAuthCallback(ILoginResult result)
        {
            if (_IsTokenExists())
            {
                SDK.FormatWarning("Limited 进来了1");
                m_UserInfo = new UserInfo();
                SDK.FormatWarning("Limited 进来了2");
                m_UserInfo.userToken = FB.Mobile.CurrentAuthenticationToken().TokenString;
                SDK.FormatWarning("Limited 进来了3");
                m_UserInfo.userId = FB.Mobile.CurrentProfile().UserID;
                SDK.FormatWarning("Limited 进来了4");
                m_UserInfo.userName = "";
                SDK.FormatWarning("Limited 进来了5");
                m_UserInfo.status = Constants.kLoginStatusUserLogin;
                SDK.FormatWarning("Limited 进来了6");
                m_UserInfo.userPermissions = null;
                 // Print current access token's granted permissions
                SDK.FormatWarning("Limited 进来了7");
                m_UserInfo.facebookMode = 1;
                SDK.FormatWarning("Limited 进来了8");
                // foreach (string perm in Facebook.Unity.AccessToken.CurrentAccessToken.Permissions)
                // {
                //     DebugUtil.Log(perm);
                // }
                DebugUtil.Log($"Limited fb Nonce={FB.Mobile.CurrentAuthenticationToken().Nonce}");
                SDK.GetInstance().loginDispatcher.ProcessLoginOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorSuccess, 0, ""));
            }
            else if (result == null)
            {
                SDK.FormatWarning("FacebookSDKPlugin.LimitedAuthCallback ----> result is null, but token is illigal, this shouldn't happen");
                SDK.GetInstance().loginDispatcher.ProcessLoginOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorUnknown, 0, "result is null, but token is illigal, this shouldn't happen"));
            }
            else if (result.Cancelled)
            {
                DebugUtil.Log("FacebookSDKPlugin.LimitedAuthCallback ----> login canceled");
                SDK.GetInstance().loginDispatcher.ProcessLoginOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorUserCanceled, 0, "user canceled"));
            }
            else
            {
                DebugUtil.Log("FacebookSDKPlugin.LimitedAuthCallback ----> login error " + result.Error);
                SDK.GetInstance().loginDispatcher.ProcessLoginOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorChannelError, -1, result.Error));
            }
        }

 
        private void ReAuthCallback(ILoginResult result)
        {
            if (result == null)
            {
                SDK.FormatWarning("FacebookSDKPlugin.ReAuthCallback ----> result is null, but token is illigal, this shouldn't happen");
                SDK.GetInstance().loginDispatcher.ProcessReauthAccessTokenOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorUnknown, 0, "ReAuthCallback result is null, but token is illigal, this shouldn't happen"));
            }
            else if (result.Cancelled)
            {
                DebugUtil.Log("FacebookSDKPlugin.ReAuthCallback ----> login canceled");
                SDK.GetInstance().loginDispatcher.ProcessReauthAccessTokenOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorUserCanceled, 0, "ReAuthCallback user canceled"));
            }
            else if (_IsTokenExists())
            {
                SDK.FormatWarning("re 进来了1");
                m_UserInfo = new UserInfo();
                SDK.FormatWarning("re 进来了2");
                m_UserInfo.userToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
                SDK.FormatWarning("re 进来了3");
                m_UserInfo.userId = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
                SDK.FormatWarning("re 进来了4");
                m_UserInfo.userName = "";
                SDK.FormatWarning("re 进来了5");
                m_UserInfo.status = Constants.kLoginStatusUserLogin;
                SDK.FormatWarning("re 进来了6");
                m_UserInfo.userPermissions = encodePermission(Facebook.Unity.AccessToken.CurrentAccessToken.Permissions.ToListEx());
                // Print current access token's granted permissions
                SDK.FormatWarning("re 进来了7");
                m_UserInfo.facebookMode = 0;
                SDK.FormatWarning("re 进来了8");
                foreach (string perm in Facebook.Unity.AccessToken.CurrentAccessToken.Permissions)
                {
                    DebugUtil.Log(perm);
                }
                SDK.GetInstance().loginDispatcher.ProcessReauthAccessTokenOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorSuccess, 0, ""));
            }
            else
            {
                DebugUtil.Log("FacebookSDKPlugin.ReAuthCallback ----> login error " + result.Error);
                SDK.GetInstance().loginDispatcher.ProcessReauthAccessTokenOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorChannelError, -1, result.Error));
            }
        }
        private void LimitedReAuthCallback(ILoginResult result)
        {
            if (result == null)
            {
                SDK.FormatWarning("FacebookSDKPlugin.LimitedReAuthCallback ----> result is null, but token is illigal, this shouldn't happen");
                SDK.GetInstance().loginDispatcher.ProcessReauthAccessTokenOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorUnknown, 0, "ReAuthCallback result is null, but token is illigal, this shouldn't happen"));
            }
            else if (result.Cancelled)
            {
                DebugUtil.Log("FacebookSDKPlugin.LimitedReAuthCallback ----> login canceled");
                SDK.GetInstance().loginDispatcher.ProcessReauthAccessTokenOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorUserCanceled, 0, "ReAuthCallback user canceled"));
            }
            else if (_IsTokenExists())
            {
                SDK.FormatWarning("Limited re 进来了1");
                m_UserInfo = new UserInfo();
                SDK.FormatWarning("Limited re 进来了2");
                m_UserInfo.userToken = FB.Mobile.CurrentAuthenticationToken().TokenString;
                SDK.FormatWarning("Limited re 进来了3");
                m_UserInfo.userId = FB.Mobile.CurrentProfile().UserID;
                SDK.FormatWarning("Limited re 进来了4");
                m_UserInfo.userName = "";
                SDK.FormatWarning("Limited re 进来了5");
                m_UserInfo.status = Constants.kLoginStatusUserLogin;
                SDK.FormatWarning("Limited re 进来了6");
                m_UserInfo.userPermissions = null;
                // Print current access token's granted permissions
                SDK.FormatWarning("Limited re 进来了7");
                m_UserInfo.facebookMode = 1;
                SDK.FormatWarning("Limited re 进来了8");
                DebugUtil.Log($"Limited re fb Nonce={FB.Mobile.CurrentAuthenticationToken().Nonce}");
                // foreach (string perm in Facebook.Unity.AccessToken.CurrentAccessToken.Permissions)
                // {
                //     DebugUtil.Log(perm);
                // }
                SDK.GetInstance().loginDispatcher.ProcessReauthAccessTokenOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorSuccess, 0, ""));
            }
            else
            {
                DebugUtil.Log("FacebookSDKPlugin.LimitedReAuthCallback ----> login error " + result.Error);
                SDK.GetInstance().loginDispatcher.ProcessReauthAccessTokenOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorChannelError, -1, result.Error));
            }
        }

        private void PermissionCallback(ILoginResult result)
        {
            if (result.Cancelled)
            {
                DebugUtil.Log("FacebookSDKPlugin.PermissionCallback ----> permission canceled");
                SDK.GetInstance().loginDispatcher.ProcessChangePermissionOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorUserCanceled, 0, "user canceled"));
            }
            else if (!string.IsNullOrEmpty(result.Error))
            {
                DebugUtil.Log("FacebookSDKPlugin.PermissionCallback ----> permisssion error " + result.Error);
                SDK.GetInstance().loginDispatcher.ProcessChangePermissionOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorChannelError, -1, result.Error));
            }
            else
            {
                m_UserInfo.userToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
                m_UserInfo.userId = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
                m_UserInfo.status = Constants.kLoginStatusUserLogin;
                m_UserInfo.userPermissions = encodePermission(Facebook.Unity.AccessToken.CurrentAccessToken.Permissions.ToListEx());
                // Print current access token's granted permissions
                foreach (string perm in Facebook.Unity.AccessToken.CurrentAccessToken.Permissions)
                {
                    DebugUtil.Log(perm);
                }
                SDK.GetInstance().loginDispatcher.ProcessChangePermissionOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorSuccess, 0, ""));
            }
        }



        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                m_LastTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = m_LastTimeScale;
            }
        }

        private float m_LastTimeScale = 0;
        private UserInfo m_UserInfo = null;
        private PluginDefine m_MyDefine = new PluginDefine();
    }

}