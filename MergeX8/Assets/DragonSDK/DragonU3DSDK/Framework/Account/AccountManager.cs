using System;
using System.Linq;
using UnityEngine;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.ABTest;

namespace DragonU3DSDK.Account
{
    public enum LoginStatus
    {
        LOGOUT,
        LOGIN_LOCKING,
        LOGIN,
        TOKEN_EXPIRED,
    }

    public class AccountManager : Manager<AccountManager>
    {
        string secret = null;
        StorageCommon storageCommon;

        bool loginCalled = false;
        float autoLoginTimer = 0;
        const float AUTO_LOGIN_INTERVAL = 30.0f;
        public LoginStatus loginStatus = LoginStatus.LOGOUT;
        SLogin loginResult;
        float refreshActiveTimer = 0;

        string token = null;
        string refreshToken = null;
        ulong tokenExpire = ulong.MaxValue;

        public bool HasLogin
        {
            get
            {
                return loginStatus == LoginStatus.LOGIN;
            }
        }
        public SLogin LoginResult
        {
            get
            {
                return loginResult;
            }
        }

        public string Token
        {
            get
            {
                return token;
            }
        }

        public string RefreshToken
        {
            get
            {
                return refreshToken;
            }
        }

        public bool Inited
        {
            get;
            private set;
        }

        public string LastErrorMessage;

        public void Init()
        {
            if (!Inited)
            {
                InitImmediately();
            }
        }

        protected override void InitImmediately()
        {
            secret = ConfigurationController.Instance.APIServerSecret;
            storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();

            if (PlayerPrefs.HasKey("token"))
            {
                token = PlayerPrefs.GetString("token");
            }
            if (PlayerPrefs.HasKey("refreshToken"))
            {
                refreshToken = PlayerPrefs.GetString("refreshToken");
            }
            if (PlayerPrefs.HasKey("tokenExpire"))
            {
                tokenExpire = ulong.Parse(PlayerPrefs.GetString("tokenExpire"));
            }

            if (!string.IsNullOrEmpty(token) && tokenExpire > DeviceHelper.CurrentTimeMillis())
            {
#if !DISABLE_STORAGE_LOG
                DebugUtil.Log("token = {0}, loginStatus = alive", token);
#endif
                loginStatus = LoginStatus.LOGIN;
            }
            else if (!string.IsNullOrEmpty(refreshToken))
            {
#if !DISABLE_STORAGE_LOG
                DebugUtil.Log("refreshToken = {0}, loginStatus = Refreshing", refreshToken);
#endif
                loginStatus = LoginStatus.TOKEN_EXPIRED;
            }
            else
            {
                loginStatus = LoginStatus.LOGOUT;
            }
            Inited = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"> 登录是否成功回调 </param>
        /// <param name="cancelCallback"> 取消Apple或者fb后的回调, 可用于取消转菊花、弹文案等操作 </param>
        public void Login(Action<bool> callback, Action cancelCallback = null)
        {
            loginCalled = true;
            Action<bool> callbackWrapper = (result) =>
            {
                if (callback != null)
                {
                    callback.Invoke(result);
                }
                //获取ABTest数据
                ABTestManager.Instance.Init();
            };

            DebugUtil.Log("account login loginStatus : " + loginStatus.ToString());

            if (HasLogin)
            {
                DebugUtil.Log("account login state = 1");

                if (PlayerPrefs.GetInt("FBAccessTokenReauth") == 1)
                {
                    DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.ReauthFacebookAccessTokenPop);
                    EventManager.Instance.Trigger<SDKEvents.ReauthFacebookAccessTokenPopEvent>().Trigger();

                    DebugUtil.Log("account login state = 2");

                    ReauthFacebookToken((token, facebookMode) =>
                    {
                        DebugUtil.Log("account login state = 3");

                        if (!string.IsNullOrEmpty(token))
                        {
                            DebugUtil.Log("account login state = 4");

                            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.ReauthFacebookAccessTokenSuccess);
                            EventManager.Instance.Trigger<SDKEvents.ReauthFacebookAccessTokenSuccessEvent>().Trigger();

                            PlayerPrefs.SetInt("FBAccessTokenReauth", 0);
                            LoginWithFacebook(token, facebookMode, callbackWrapper);
                        }
                        else
                        {
                            DebugUtil.Log("account login state = 5");
                            DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.ReauthFacebookDataAccessFailure);
                            EventManager.Instance.Trigger<SDKEvents.ReauthFacebookAccessTokenFailureEvent>().Trigger();

                            callbackWrapper.Invoke(false);
                        }
                    });
                }
                else
                {
                    callbackWrapper.Invoke(true);
                }
                // 更新分层
                DragonPlus.ConfigHub.ConfigHubManager.Instance.OnLoginSuccess();
                return;
            }

            if (!string.IsNullOrEmpty(refreshToken))
            {
                Relogin(refreshToken, callback);
            }
            else if (HasBindFacebook())
            {
                GetFacebookToken((token, facebookMode) =>
                {
                    if (string.IsNullOrEmpty(token))
                    {
                        callbackWrapper.Invoke(false);
                        return;
                    }
                    LoginWithFacebook(token, facebookMode, callbackWrapper);
                },
                () =>
                {
                    DebugUtil.Log("facebook user canceled");
                    cancelCallback?.Invoke();
                });
            }
            else if (HasBindApple())
            {
                GetAppleToken((token, code) =>
                {
                    if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(code))
                    {
                        callbackWrapper.Invoke(false);
                        return;
                    }
                    LoginWithApple(token, code, callbackWrapper);
                },
                () =>
                {
                    DebugUtil.Log("apple user canceled");
                    cancelCallback?.Invoke();
                });
            }
            else
            {
                LoginWithDeviceId(DeviceHelper.GetDeviceId(), callbackWrapper);
            }
        }

        public void Relogin(string refreshToken, Action<bool> callback)
        {
            var cLogin = new CLogin
            {
                RefreshToken = refreshToken,
            };
            Login(cLogin, callback);
        }

        void Login(CLogin cLogin, Action<bool> callback)
        {
            if (loginStatus == LoginStatus.LOGIN_LOCKING)
            {
                DebugUtil.Log("LOGIN_LOCKING");
                callback?.Invoke(false);
                return;
            }
            loginStatus = LoginStatus.LOGIN_LOCKING;

            if (!string.IsNullOrEmpty(storageCommon.AdjustId))
            {
                cLogin.AdjustId = storageCommon.AdjustId;
            }
            if (!string.IsNullOrEmpty(storageCommon.InviteCode))
            {
                cLogin.InviteCode = storageCommon.InviteCode;
            }
            if (!string.IsNullOrEmpty(storageCommon.Idfa))
            {
                cLogin.Idfa = storageCommon.Idfa;
            }
            if (!string.IsNullOrEmpty(storageCommon.Idfv))
            {
                cLogin.Idfv = storageCommon.Idfv;
            }
            if (!string.IsNullOrEmpty(storageCommon.Gaid))
            {
                cLogin.GpsAdid = storageCommon.Gaid;
            }
            if (!string.IsNullOrEmpty(storageCommon.FirebaseInstanceId))
            {
                cLogin.FirebaseInstanceId = storageCommon.FirebaseInstanceId;
            }
            if (!string.IsNullOrEmpty(ChangeableConfig.Instance.GetGroups()))
            {
                cLogin.GroupId = ChangeableConfig.Instance.GetGroups();
            }

            cLogin.DeviceId = DeviceHelper.GetDeviceId();
            cLogin.Platform = DeviceHelper.GetPlatform();

            Network.BI.BIManager.Instance.SendCommonGameEvent(BiEventCommon.Types.CommonGameEventType.LoginStart);
            APIManager.Instance.Send(cLogin, (SLogin sLogin) =>
            {
                Network.BI.BIManager.Instance.SendCommonGameEvent(BiEventCommon.Types.CommonGameEventType.LoginSuccess);
                OnLoginSuccess(sLogin);
                StorageManager.Instance.GetOrCreateProfile(callback);
            }, (errno, errmsg, resp) =>
            {
                Network.BI.BIManager.Instance.SendCommonGameEvent(BiEventCommon.Types.CommonGameEventType.LoginFailure, errno.ToString(), errmsg);
                loginStatus = LoginStatus.LOGOUT;
                DebugUtil.LogError("login failed errno = {0} errmsg = {1}", errno, errmsg);
                callback?.Invoke(false);
            });
        }

        void LoginWithDeviceId(string deviceId, Action<bool> callback)
        {
            var cLogin = new CLogin
            {
                DeviceId = deviceId,
                Checksum = Sha1.HashEx(deviceId + secret),
            };
#if UNITY_ANDROID
            cLogin.OldDeviceId = DragonNativeBridge.getDeivceId();
#endif
            Login(cLogin, callback);
        }

        void LoginWithEmailAndPassword(string email, string password, Action<bool> callback)
        {
            var cLogin = new CLogin
            {
                Email = email,
                Password = password,
            };
            Login(cLogin, callback);
        }
        
        void LoginWithEmailAndVerifyCode(string email, string verifyCode, Action<bool> callBack)
        {
            var cLogin = new CLogin
            {
                Email = email,
                Verify = verifyCode,
            };
            Login(cLogin, callBack);
        }

        void LoginWithFacebook(string facebookToken, int facebookMode, Action<bool> callback)
        {
            var cLogin = new CLogin
            {
                FacebookToken = facebookToken,
                IsFacebookLimitedMode = facebookMode == 1

            };
            Login(cLogin, callback);
        }

        void LoginWithApple(string identityToken, string authorizationCode, Action<bool> callback)
        {
            var cLogin = new CLogin
            {
                AppleIdentityToken = identityToken,
                AppleAuthorizationCode = authorizationCode,
            };
            Login(cLogin, callback);
        }

        void UpdateStorageAfterLogin(SLogin sLogin)
        {
            if (storageCommon.PlayerId != sLogin.PlayerId)
            {
                storageCommon.Extensions.Clear();
            }
            storageCommon.PlayerId = sLogin.PlayerId;

            if (!string.IsNullOrEmpty(sLogin.FacebookId))
            {
                storageCommon.FacebookId = sLogin.FacebookId;
            }

            if (!string.IsNullOrEmpty(sLogin.FacebookName))
            {
                storageCommon.FacebookName = sLogin.FacebookName;
            }

            if (!string.IsNullOrEmpty(sLogin.FacebookEmail))
            {
                storageCommon.FacebookEmail = sLogin.FacebookEmail;
            }
            
            if (!string.IsNullOrEmpty(sLogin.Email))
            {
                storageCommon.Email = sLogin.Email;
            }

            if (storageCommon.InstalledAt == 0)
            {
                storageCommon.InstalledAt = sLogin.Timestamp;
            }

            if (string.IsNullOrEmpty(storageCommon.Adid))
            {
                storageCommon.Adid = SystemInfomation.GetADID();
            }

            if (string.IsNullOrEmpty(storageCommon.Country))
            {
                storageCommon.Country = sLogin.Region.CountryCode;
            }

            if (string.IsNullOrEmpty(storageCommon.Region))
            {
                storageCommon.Region = sLogin.Region.RegionCode;
            }

            if (string.IsNullOrEmpty(storageCommon.TimeZone))
            {
                storageCommon.TimeZone = sLogin.Region.TimeZone;
            }

            if (string.IsNullOrEmpty(storageCommon.AppleAccountId))
            {
                storageCommon.AppleAccountId = sLogin.AppleAccountId;
            }
            // abtests
            //string[] oldAbtestKeys = storageCommon.Abtests.Keys.ToArray();
            //string[] newAbtestKeys = sLogin.Abtests.Keys.ToArray();

            //bool diffrent = false;
            //if (oldAbtestKeys.Length != newAbtestKeys.Length)
            //{
            //    diffrent = true;
            //}
            //else if (oldAbtestKeys.Length > 0)
            //{
            //    Array.Sort(oldAbtestKeys, StringComparer.InvariantCulture);
            //    Array.Sort(newAbtestKeys, StringComparer.InvariantCulture);
            //    for (int i = 0; i < oldAbtestKeys.Length; i++)
            //    {
            //        if (oldAbtestKeys[i] != newAbtestKeys[i])
            //        {
            //            diffrent = true;
            //            break;
            //        }
            //    }
            //}

            //if (diffrent)
            //{
            //    storageCommon.Abtests.Clear();
            //    foreach (var abtest in sLogin.Abtests)
            //    {
            //        storageCommon.Abtests.Add(abtest.Key, abtest.Value);
            //    };
            //    StorageManager.Instance.LocalVersion++;
            //}
        }

        public bool HasBindEmail()
        {
            if (storageCommon == null)
                return false;
            return !string.IsNullOrEmpty(storageCommon.Email);
        }

        public bool HasBindFacebook()
        {
            if (storageCommon == null)
                return false;
            return !string.IsNullOrEmpty(storageCommon.FacebookId);
        }

        public string GetFacebookId()
        {
            return storageCommon.FacebookId;
        }

        void BindFacebookWithServer(string facebookToken, int facebookMode, Action<bool> callback = null)
        {
            var cBindFacebook = new CBindFacebook
            {
                FacebookToken = facebookToken,
                IsFacebookLimitedMode = facebookMode == 1
            };
            APIManager.Instance.Send(cBindFacebook, (SBindFacebook sBindFadebook) =>
            {
                DebugUtil.Log("[facebook] 9");
                UpdateStorageAfterBindFacebook(sBindFadebook);
                StorageManager.Instance.GetOrCreateProfile(callback);
            },
            (errno, errmsg, resp) =>
            {
                DebugUtil.LogError("bind facebook failure errno {0} errmsg {1}", errno, errmsg);
                if (errno == ErrorCode.FacebookAlreadyBindedError)
                {
                    DebugUtil.Log("[facebook] 10");
                    Clear();
                    LoginWithFacebook(facebookToken, facebookMode, (result) =>
                    {
                        // 选档会导致刚刚获得的绑定关系消失，所以需要重新更新绑定关系
                        if (LoginResult != null)
                        {
                            UpdateStorageAfterLogin(LoginResult);
                        }
                        
                        callback?.Invoke(result);
                    });
                }
                else
                {
                    LastErrorMessage = errmsg;
                    DebugUtil.Log("[facebook] 11");
                    if (callback != null)
                    {
                        callback.Invoke(false);
                    }
                }
            });
        }

        void BindAppleWithServer(string appleToken, string code, Action<bool> callback = null)
        {
            var cBindApple = new CBindApple
            {
                AppleIdentityToken = appleToken,
                AppleAuthorizationCode = code
            };
            APIManager.Instance.Send(cBindApple, (SBindApple sBindApple) =>
            {
                DebugUtil.Log("[apple] 9");
                UpdateStorageAfterBindApple(sBindApple);
                StorageManager.Instance.GetOrCreateProfile(callback);
            },
            (errno, errmsg, resp) =>
            {
                DebugUtil.LogError("bind apple failure errno {0} errmsg {1}", errno, errmsg);
                if (errno == ErrorCode.AppleIdAlreadyBindedError)
                {
                    DebugUtil.Log("[apple] 10");
                    Clear();
                    LoginWithApple(appleToken, code, (result) =>
                    {
                        // 选档会导致刚刚获得的绑定关系消失，所以需要重新更新绑定关系
                        if (LoginResult != null)
                        {
                            UpdateStorageAfterLogin(LoginResult);
                        }

                        callback?.Invoke(result);
                    });
                }
                else
                {
                    LastErrorMessage = errmsg;
                    DebugUtil.Log("[apple] 11");
                    if (callback != null)
                    {
                        callback.Invoke(false);
                    }
                }
            });
        }

        public void EmailAccountC2SSendAddress(string emailAddress, Action<bool, int> callBack = null)
        {
            DebugUtil.Log("[EmailAccount] 1");
            CSendEmailVerify message = new CSendEmailVerify()
            {
                Email = emailAddress
            };
            APIManager.Instance.Send(message, (SSendEmailVerify resp) =>
                {
                    DebugUtil.Log($" [EmailAccount] 2");
                    callBack?.Invoke(true, 0);
                },
                (errorCode, errorMsg, resp) =>
                {
                    DebugUtil.LogError($" [EmailAccount] send address failure, errorCode: {errorCode}, errorMsg: {errorMsg}");
                    DebugUtil.Log($" [EmailAccount] 3");
                    callBack?.Invoke(false, (int)errorCode);
                });
        }

        public void EmailAccountBindEmailVerify(string emailAddress, string verifyCode, Action<bool, int> callBack = null)
        {
            DebugUtil.Log("[EmailAccount] 4");
            CBindEmailVerify message = new CBindEmailVerify()
            {
                Email = emailAddress,
                Verify = verifyCode
            };
            APIManager.Instance.Send(message, (SBindEmailVerify resp) =>
                {
                    DebugUtil.Log($" [EmailAccount] 5 SBindEmailVerify resp: {resp}");
                    UpdateStorageAfterBindEmailVerify(resp);
                    StorageManager.Instance.GetOrCreateProfile((result) =>
                    {
                        DebugUtil.Log($" [EmailAccount] 6 GetOrCreateProfile result: {result}");
                        callBack?.Invoke(result, 0);
                    });
                },
                (errorCode, errorMsg, resp) =>
                {
                    DebugUtil.Log($" [EmailAccount] 7 verify code failure, errorCode: {errorCode}， errorMsg: {errorMsg}");
                    // if (errorCode == ErrorCode.EmailAlreadyBindedError)
                    // {
                    //     DebugUtil.Log($" [EmailAccount] 9");
                    //     Clear();
                    //     LoginWithEmailAndVerifyCode(emailAddress, verifyCode, (result) =>
                    //     {
                    //         DebugUtil.Log($" [EmailAccount] 10");
                    //         if (LoginResult != null)
                    //         {
                    //             DebugUtil.Log($" [EmailAccount] 11");
                    //             // 选档会导致刚刚获得的绑定关系消失，所以需要重新更新绑定关系, 这里添加邮箱的绑定关系
                    //             UpdateStorageAfterLogin(LoginResult);
                    //         }
                    //         DebugUtil.Log($" [EmailAccount] 12 result: {result}");
                    //         // CLogin重新登录成功 --> CGetProfile存档获取成功 --> result:true
                    //         callBack?.Invoke(result, (int)errorCode);
                    //     });
                    // }
                    // else
                    // {
                    //     DebugUtil.Log($" [EmailAccount] 8");
                    //     callBack?.Invoke(false, (int)errorCode);
                    // }
                    
                    callBack?.Invoke(false, (int)errorCode);
                });
        }

        void ReauthFacebookToken(Action<string, int> callback)
        {
            FacebookManager.Instance.ReauthFacebookToken((pluginId, userInfo, error) =>
            {
                DebugUtil.Log("ReauthFacebookToken state 1");

                DebugUtil.Log("ReauthFacebookToken plugInId {0} userInfo {1} error {2}", pluginId, userInfo, error);
                if (error != null && error.err != 0)
                {
                    DebugUtil.Log("ReauthFacebookToken state 2");
                    LastErrorMessage = error.errmsg;
                    callback.Invoke(null, 0);
                }
                else
                {
                    DebugUtil.Log("ReauthFacebookToken state 3 facebookMode {0}", userInfo.facebookMode);
                    callback.Invoke(userInfo.userToken, userInfo.facebookMode);
                }
            });
        }

        void GetFacebookToken(Action<string, int> callback, Action cancelCallback = null)
        {
            if (FacebookManager.Instance.IsLoggedIn())
            {
                var userInfo = FacebookManager.Instance.GetUserInfo();
                if (userInfo == null)
                {
                    DebugUtil.Log("[facebook] 6");
                    LastErrorMessage = "IsLoggedInAndEmptyFacebookToken";
                    callback.Invoke(null, 0);
                }
                else
                {
                    DebugUtil.Log("[facebook] 7 facebookMode {0}", userInfo.facebookMode);
                    callback.Invoke(userInfo.userToken, userInfo.facebookMode);
                }
                return;
            }

            FacebookManager.Instance.Login((pluginId, userInfo, error) =>
            {
                DebugUtil.Log("Facebook Login plugInId {0} userInfo {1} error {2}", pluginId, userInfo, error);
                if (error != null && error.err == Dlugin.Constants.kErrorUserCanceled && cancelCallback != null)
                {
                    cancelCallback.Invoke();
                    return;
                }

                if (error != null && error.err != 0)
                {
                    // error
                    DebugUtil.Log("[facebook] 7");
                    LastErrorMessage = error.errmsg;
                    callback.Invoke(null, 0);
                }
                else
                {
                    DebugUtil.Log("[facebook] 8 facebookMode {0}", userInfo.facebookMode);
                    callback.Invoke(userInfo.userToken, userInfo.facebookMode);
                }
            });
        }

        /// <summary>
        /// 回调参数：token, code
        /// </summary>
        void GetAppleToken(Action<string, string> callback, Action cancelCallback = null)
        {
            if (AppleLoginManager.Instance.IsLoggedIn())
            {
                var userInfo = AppleLoginManager.Instance.GetUserInfo();
                if (userInfo == null)
                {
                    DebugUtil.Log("[apple] 6");
                    LastErrorMessage = "IsLoggedInAndEmptyAppleToken";
                    callback.Invoke(null, null);
                }
                else
                {
                    DebugUtil.Log("[apple] 7");
                    callback.Invoke(userInfo.appleIdentityToken, userInfo.appleAuthorizationCode);
                }
                return;
            }

            AppleLoginManager.Instance.Login((pluginId, userInfo, error) =>
            {
                if (error != null && error.err == Dlugin.Constants.kErrorUserCanceled && cancelCallback != null)
                {
                    cancelCallback.Invoke();
                    return;
                }

                if (error != null && error.err != 0)
                {
                    // error
                    DebugUtil.Log("[apple] 8");
                    LastErrorMessage = error.errmsg;
                    callback.Invoke(null, null);
                }
                else
                {
                    DebugUtil.Log("[apple] 9");
                    callback.Invoke(userInfo.appleIdentityToken, userInfo.appleAuthorizationCode);
                }
            });
        }

        public bool HasBindApple()
        {
            if (!AppleLoginManager.Instance.IsCurrentPlatformSupported())
            {
                return false;
            }
            if (storageCommon == null)
            {
                return false;
            }
            return !string.IsNullOrEmpty(storageCommon.AppleAccountId);
        }

        public void BindApple(Action<bool> callback = null, Action cancelCallback = null)
        {
            if (HasBindApple())
            {
                callback?.Invoke(true);
                return;
            }

            GetAppleToken((token, code) =>
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(code))
                {
                    DebugUtil.Log("[apple] 4");
                    callback?.Invoke(false);
                    return;
                }
                DebugUtil.Log("[apple] 5");
                DebugUtil.LogError($"token = {token}, code = {code}");
                if (HasLogin)
                {
                    BindAppleWithServer(token, code, callback);
                }
                else
                {
                    LoginWithApple(token, code, callback);
                }
            },
            cancelCallback);
        }

        public void BindFacebook(Action<bool> callback = null, Action cancelCallback = null)
        {
            DebugUtil.Log("[facebook] 1");
            if (HasBindFacebook())
            {
                DebugUtil.Log("[facebook] 2");
                if (callback != null)
                {
                    callback.Invoke(true);
                }
                return;
            }

            DebugUtil.Log("[facebook] 3");
            GetFacebookToken((token, facebookMode) =>
            {
                if (string.IsNullOrEmpty(token))
                {
                    DebugUtil.Log("[facebook] 4");
                    if (callback != null)
                    {
                        callback.Invoke(false);
                    }
                    return;
                }
                DebugUtil.Log("[facebook] 5 facebookMode {0}", facebookMode);
                if (HasLogin)
                {
                    BindFacebookWithServer(token, facebookMode, callback);
                }
                else
                {
                    LoginWithFacebook(token, facebookMode, callback);
                }
            },
            cancelCallback);
        }

        void UpdateStorageAfterBindFacebook(SBindFacebook sBindFacebook)
        {
            if (string.IsNullOrEmpty(storageCommon.FacebookId) && !string.IsNullOrEmpty(sBindFacebook.FacebookId))
            {
                storageCommon.FacebookId = sBindFacebook.FacebookId;
            }
            if (string.IsNullOrEmpty(storageCommon.FacebookName) && !string.IsNullOrEmpty(sBindFacebook.FacebookName))
            {
                storageCommon.FacebookName = sBindFacebook.FacebookName;
            }

            if (string.IsNullOrEmpty(storageCommon.FacebookEmail) && !string.IsNullOrEmpty(sBindFacebook.FacebookEmail))
            {
                storageCommon.FacebookEmail = sBindFacebook.FacebookEmail;
            }
        }

        void UpdateStorageAfterBindApple(SBindApple sBindApple)
        {
            if (string.IsNullOrEmpty(storageCommon.AppleAccountId) && !string.IsNullOrEmpty(sBindApple.AppleAccountId))
            {
                storageCommon.AppleAccountId = sBindApple.AppleAccountId;
            }
        }
        
        void UpdateStorageAfterBindEmailVerify(SBindEmailVerify sBindEmailVerify)
        {
            if (string.IsNullOrEmpty(storageCommon.Email) && !string.IsNullOrEmpty(sBindEmailVerify.Email))
            {
                storageCommon.Email = sBindEmailVerify.Email;
            }
        }
        
        public void LogoutWithFacebook(Action<bool> callback = null)
        {
            var cUnbindFacebook = new CUnbindFacebook
            {
                DeviceId = DeviceHelper.GetDeviceId()
            };
            APIManager.Instance.Send(cUnbindFacebook, (SUnbindFacebook resp) =>
            {
                DebugUtil.Log("UnbindFacebook resp =" + resp);
                FacebookManager.Instance.LogOut((pluginId, userInfo, error) =>
                {
                    DebugUtil.Log("UnbindFacebook error =" + error);
                    if (error != null && error.err != 0)
                    {
                        if (callback != null)
                        {
                            callback.Invoke(false);
                        }
                    }
                    else
                    {
                        //StorageManager.Instance.ClearAll();
                        //Clear();
                        storageCommon.FacebookId = "";
                        if (callback != null)
                        {
                            callback.Invoke(true);
                        }
                    }
                });
            }, (errno, errmsg, resp) =>
            {
                DebugUtil.Log("UnbindFacebook errno = " + errno + "errmsg=" + errmsg + "resp =" + resp);
                if (callback != null)
                {
                    callback.Invoke(false);
                }
            });
        }


        public void LogoutWithApple(Action<bool> callback = null)
        {
            if (!AppleLoginManager.Instance.IsCurrentPlatformSupported())
            {
                callback?.Invoke(false);
                return;
            }
            var cUnbindApple = new CUnbindApple
            {
                DeviceId = DeviceHelper.GetDeviceId()
            };
            APIManager.Instance.Send(cUnbindApple, (SUnbindApple resp) =>
            {
                DebugUtil.Log("CUnbindApple resp =" + resp);
                AppleLoginManager.Instance.LogOut((pluginId, userInfo, error) =>
                {
                    DebugUtil.Log("CUnbindApple error =" + error);
                    if (error != null && error.err != 0)
                    {
                        if (callback != null)
                        {
                            callback.Invoke(false);
                        }
                    }
                    else
                    {
                        storageCommon.AppleAccountId = "";
                        //StorageManager.Instance.ClearAll();
                        //Clear();
                        if (callback != null)
                        {
                            callback.Invoke(true);
                        }
                    }
                });
            }, (errno, errmsg, resp) =>
            {
                DebugUtil.Log("UnbindFacebook errno = " + errno + "errmsg=" + errmsg + "resp =" + resp);
                if (callback != null)
                {
                    callback.Invoke(false);
                }
            });
        }
        
        public void LogoutWithEmailAccount(Action<bool> callBack = null)
        {
            CUnbindEmailVerify message = new CUnbindEmailVerify()
            {
                DeviceId = DeviceHelper.GetDeviceId()
            };
            APIManager.Instance.Send(message, (SUnbindEmailVerify resp) =>
                {
                    DebugUtil.Log(" [EmailAccount] Unbind success, resp =" + resp);
                    storageCommon.Email = "";
                    callBack?.Invoke(true);
                },
                (errorCode, errorMsg, resp) =>
                {
                    DebugUtil.Log($" [EmailAccount] Unbind failure, errorCode: {errorCode}, errorMsg: {errorMsg}, resp: {resp}");
                    callBack?.Invoke(false);
                });
        }

        void OnLoginSuccess(SLogin sLogin)
        {
            token = sLogin.Token;
            refreshToken = sLogin.RefreshToken;
            tokenExpire = sLogin.Expire + DeviceHelper.CurrentTimeMillis();
            PlayerPrefs.SetString("token", token);
            PlayerPrefs.SetString("refreshToken", refreshToken);
            PlayerPrefs.SetString("tokenExpire", tokenExpire.ToString());

            loginStatus = LoginStatus.LOGIN;
            loginResult = sLogin;

            UpdateStorageAfterLogin(sLogin);

            EventManager.Instance.Trigger<SDKEvents.LoginEvent>().Trigger();

            //登录成功后立即进行一次心跳
            APIManager.Instance.HeartBeat();
            
            // 断掉长连接
            //APIManager.Instance.CloseWebsockets();

            // 更新分组
            ChangeableConfig.Instance.SetGroupId(sLogin.GroupId);
            
            // 更新分层
            DragonPlus.ConfigHub.ConfigHubManager.Instance.OnLoginSuccess();
        }

        public void OnTokenExpire()
        {
            token = null;
            tokenExpire = ulong.MaxValue;
            PlayerPrefs.DeleteKey("token");
            PlayerPrefs.DeleteKey("tokenExpire");

            //secret = null;
            if (loginStatus == LoginStatus.LOGIN)
            {
                loginStatus = LoginStatus.TOKEN_EXPIRED;
            }
        }

        public void OnRefreshTokenExpire()
        {
            DebugUtil.Log("[异地登录] OnRefreshTokenExpire");
            refreshToken = null;
            PlayerPrefs.DeleteKey("refreshToken");

            loginStatus = LoginStatus.LOGOUT;
            EventManager.Instance.Trigger<SDKEvents.AccountLoginOtherDeviceEvent>().Data().Trigger();
            Network.BI.BIManager.Instance.SendCommonGameEvent(BiEventCommon.Types.CommonGameEventType.AccountLoginOtherDevice);
        }

        void AutoLogin()
        {
            if (!loginCalled || loginStatus != LoginStatus.LOGOUT)
            {
                return;
            }

            autoLoginTimer += UnityEngine.Time.deltaTime;
            if (autoLoginTimer > AUTO_LOGIN_INTERVAL && APIManager.Instance.HasNetwork)
            {
                autoLoginTimer = 0;
                
                Login((result) =>
                {
                    DebugUtil.Log("AutoLogin result = {0}", result);
                });
            }
        }

        public void Clear()
        {
            token = null;
            refreshToken = null;
            loginStatus = LoginStatus.LOGOUT;

            PlayerPrefs.DeleteKey("token");
            PlayerPrefs.DeleteKey("tokenExpire");
            PlayerPrefs.DeleteKey("refreshToken");
        }

        private void Update()
        {
            AutoLogin();

            if (loginStatus == LoginStatus.TOKEN_EXPIRED && refreshToken != null)
            {
                Relogin(refreshToken, (success) =>
                {
                    DebugUtil.Log("relogin result = {0}", success);
                });
            }

            refreshActiveTimer += Time.deltaTime;
            if (refreshActiveTimer > 20)
            {
                refreshActiveTimer = 0;
                RefreshActiveDays();
            }
        }

        public void RefreshActiveDays()
        {
            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
            if (storageCommon != null)
            {
                var activeData = storageCommon.ActiveData;
                ulong currentSeconds = DeviceHelper.CurrentTimeMillis() / 1000;
                uint currentDay = (uint)(currentSeconds / 3600 / 24);
                uint lastActiveDay = (uint)(activeData.LastActiveTime / 3600 / 24);
                if (currentDay > lastActiveDay)
                {
                    activeData.TotalActiveDays++;
                    activeData.LastActiveTime = currentSeconds;
                    if (currentDay == lastActiveDay + 1)
                    {
                        activeData.ContinuousActiveDays++;
                    }
                    else
                    {
                        activeData.ContinuousActiveDays = 1;
                    }
                }
            }
        }
    }
}
