using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Dlugin.PluginStructs;
using System;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using DragonU3DSDK;
using System.Text;

namespace Dlugin
{
    public class AppleSignInPlugin : IUserLogin
    {
        private const string AppleUserIdKey = "AppleUserId";

        public void Initialize()
        {
            m_MyDefine = new PluginDefine();
            m_MyDefine.m_PluginName = Constants.Apple;
            m_MyDefine.m_PluginVersion = "1.0";

            // If the current platform is supported
            if (AppleAuthManager.IsCurrentPlatformSupported)
            {
                // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
                var deserializer = new PayloadDeserializer();
                // Creates an Apple Authentication manager with the deserializer
                this._appleAuthManager = new AppleAuthManager(deserializer);

                // If at any point we receive a credentials revoked notification, we delete the stored User ID, and go back to login
                this._appleAuthManager.SetCredentialsRevokedCallback(result =>
                {
                    //PlayerPrefs.DeleteKey(AppleUserIdKey);
                    //SDK.GetInstance().AppleLoginDispatcher.ProcessLoginOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorAppleCredentialsRevoked, 0, result));
                });

                if (PlayerPrefs.HasKey(AppleUserIdKey))
                {
                    var storedAppleUserId = PlayerPrefs.GetString(AppleUserIdKey);
                    this.CheckCredentialStatusForUserId(storedAppleUserId);
                }
                TimerManager.Instance.AddDelegate(OnUpdate);
            }
        }

        private void OnUpdate(float delta)
        {
            if (this._appleAuthManager != null)
            {
                this._appleAuthManager.Update();
            }
        }

        private void CheckCredentialStatusForUserId(string appleUserId)
        {
            // If there is an apple ID available, we should check the credential state
            this._appleAuthManager.GetCredentialState(
                appleUserId,
                state =>
                {
                    switch (state)
                    {
                    // If it's authorized, login with that user id
                    case CredentialState.Authorized:
                            return;

                    // If it was revoked, or not found, we need a new sign in with apple attempt
                    // Discard previous apple user id
                    case CredentialState.Revoked:
                        case CredentialState.NotFound:
                        PlayerPrefs.DeleteKey(AppleUserIdKey);
                            return;
                    }
                },
                error =>
                {
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                }
            );
        }

        private void OnAppleLoginFailure(IAppleError error)
        {
            var authorizationErrorCode = error.GetAuthorizationErrorCode();
            DebugUtil.Log("AppleSignInPlugin----- errCode " + authorizationErrorCode.ToString() + " " + error.ToString());

            var errCode = error.GetAuthorizationErrorCode();
            var sdkErrorCode = Constants.kErrorUnknown;
            if (errCode == AuthorizationErrorCode.Canceled)
            {
                sdkErrorCode = Constants.kErrorUserCanceled;
            }
            
            SDK.GetInstance().AppleLoginDispatcher.ProcessLoginOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(sdkErrorCode, 0, error.ToString()));
        }


        private void OnAppleLoginSuccuss(ICredential credential)
        {
            // If a sign in with apple succeeds, we should have obtained the credential with the user id, name, and email, save it
            var appleIdCredential = credential as IAppleIDCredential;
            m_UserInfo = new UserInfo();
            if (credential != null)
            {
                m_UserInfo.userToken = credential.User;
                m_UserInfo.userId = credential.User;
                m_UserInfo.status = Constants.kLoginStatusUserLogin;
            }
            if (appleIdCredential != null)
            {
                PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                if (appleIdCredential.FullName != null)
                {
                    m_UserInfo.userName = appleIdCredential.FullName.ToLocalizedString();
                }
                if (appleIdCredential.Email != null)
                {
                    m_UserInfo.email = appleIdCredential.Email;
                }
                var token = "";
                var authorizationCode = "";
                if (appleIdCredential.IdentityToken != null)
                {
                    token = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);
                }
                if (appleIdCredential.AuthorizationCode != null)
                {
                    authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode, 0, appleIdCredential.AuthorizationCode.Length);
                }
                m_UserInfo.appleIdentityToken = token;
                m_UserInfo.appleAuthorizationCode = authorizationCode;

            }
            DebugUtil.Log($"AppleSignInPlugin----- token = {m_UserInfo.appleIdentityToken}");
            DebugUtil.Log($"AppleSignInPlugin----- code = {m_UserInfo.appleAuthorizationCode}");
            DebugUtil.Log($"AppleSignInPlugin----- user id = {m_UserInfo.userId}");
            SDK.GetInstance().AppleLoginDispatcher.ProcessLoginOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorSuccess, 0, ""));
        }

        private void AttemptQuickLogin()
        {
            var quickLoginArgs = new AppleAuthQuickLoginArgs();
            // Quick login should succeed if the credential was authorized before and not revoked
            this._appleAuthManager.QuickLogin(
                quickLoginArgs,
                credential =>
                {
                    OnAppleLoginSuccuss(credential);
                },
                error =>
                {
                    // If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
                    if (error.GetAuthorizationErrorCode() != AuthorizationErrorCode.Canceled)
                    {
                        AttemptNormalLogin();
                    }
                    else
                    {
                        OnAppleLoginFailure(error);
                    }
                });
        }

        private void AttemptNormalLogin()
        {
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
            this._appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    OnAppleLoginSuccuss(credential);
                },
                error =>
                {
                    OnAppleLoginFailure(error);
                });
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

        public override void Login(int[] allPermission)
        {
            AttemptNormalLogin();
            //AttemptQuickLogin();
        }

        public override bool CheckLoggedIn()
        {
            return m_UserInfo != null;
        }

        public override void Logout()
        {
            m_UserInfo = null;
            SDK.GetInstance().AppleLoginDispatcher.ProcessLogoutOver(m_MyDefine.m_PluginName, m_UserInfo, new SDKError(Constants.kErrorSuccess, 0, ""));
        }

        public override void DisposePlugin(string pluginName)
        {
            m_MyDefine = null;
        }

        public override void ReauthToken(int[] allPermission)
        {
            throw new NotImplementedException();
        }

        public override void RefreshUserInfo()
        {
            throw new NotImplementedException();
        }

        public override void ChangePermission(int[] allPermission)
        {
            throw new NotImplementedException();
        }

        public override void LogAdEvent(AD_Type ad_Type, AD_Event_Type event_Type)
        {
            throw new NotImplementedException();
        }

        public override void LogPurchaseEvent(decimal price, string currency, string productID)
        {
            throw new NotImplementedException();
        }

        private IAppleAuthManager _appleAuthManager;
        private float m_LastTimeScale = 0;
        private UserInfo m_UserInfo = null;
        private PluginDefine m_MyDefine = new PluginDefine();
    }

}
