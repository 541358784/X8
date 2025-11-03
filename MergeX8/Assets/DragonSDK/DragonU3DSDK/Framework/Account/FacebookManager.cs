using System;
using System.Collections;
using DragonU3DSDK;
using Dlugin;
using Dlugin.PluginStructs;
using UnityEngine;
using Facebook.Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Network.BI;
using DragonU3DSDK.Storage;

namespace DragonU3DSDK.Account
{
    public class FacebookManager : Manager<FacebookManager>
    {
        [System.Serializable]
        private class FacebookRawDataModel
        {
            public List<FacebookFriendModel> Data;
        }

        [System.Serializable]
        private class FacebookFriendModel
        {
            public string Id;
            public string Name;
            public FacebookDataModel Picture;
        }

        [System.Serializable]
        private class FacebookDataModel
        {
            public FacebookFriendImageModel Data;
        }

        [System.Serializable]
        private class FacebookFriendImageModel
        {
            public int Height;
            public int Width;
            public bool Is_sihouette;
            public string Url;
        }

        [System.Serializable]
        public class FacebookFriendInfoModel
        {
            public string Id { set; get; }
            public string Name { set; get; }
            public string AvatarUrl { set; get; }
            public int AvatarHeight { set; get; }
            public int AvatarWidth { set; get; }
            public bool Is_sihouette { set; get; }
        }



        Action<string, UserInfo, SDKError> m_callback;

        private ReaderWriterLockSlim actionQueueLock = new ReaderWriterLockSlim();
        private Queue<Action> callActionQueue = new Queue<Action>();

        public FacebookManager()
        {
            SDK.GetInstance().loginDispatcher.onLoginOver += onLoginOver;
            SDK.GetInstance().loginDispatcher.onLogoutOver += onLogOutOver;
            SDK.GetInstance().loginDispatcher.onReauthAccessTokenOver += onReauthTokenOver;
        }

        public void Update()
        {
            if (actionQueueLock.IsWriteLockHeld || actionQueueLock.TryEnterWriteLock(200))
            {
                try
                {
                    while (callActionQueue.Count > 0)
                    {
                        Action _callback = callActionQueue.Dequeue();
                        if (_callback != null)
                        {
                            _callback();
                        }

                    }
                }
                finally
                {
                    try
                    {
                        actionQueueLock.ExitWriteLock();
                    }
                    catch (SynchronizationLockException e)
                    {
                        DragonU3DSDK.DebugUtil.Log("SynchronizationLockException : " + e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the user info.
        /// 获取 facebook 用户信息，如未登陆录触发登录逻辑
        /// </summary>
        /// <returns>The user info.</returns>
        public UserInfo GetUserInfo()
        {
            if (!IsLoggedIn())
            {
                Login(null);
            }
            //if (!DragonNativeBridge.IsFacebookDataAccessValid())
            //{
            //    DebugUtil.LogWarning("Facebook: 需要刷新Data Access!");
            //    DragonNativeBridge.ReauthorizeFacebookDataAccess();
            //}
            return Dlugin.SDK.GetInstance().loginService.GetUserInfo();
        }

        public bool SendRequest(List<string> facebookIds)
        {

            //FB.AppRequest("Come play this great game!", OGActionType.TURN,  null , facebookIds, null, null, delegate (IAppRequestResult result)
            //{

            //    DebugUtil.Log((result.RawResult);
            //});

            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("action_type", "SEND");
            p.Add("message", "Come play");

            //FB.

            FB.API(String.Format("{0}/apprequests", facebookIds[0]), HttpMethod.POST, (result) =>
            {

                DebugUtil.Log(result.RawResult);

            }, p);



            return false;
        }

        public bool AppRequest(string message)
        {

            FB.AppRequest(
                message, null, null, null, null, null, null,
                delegate (IAppRequestResult result)
                {

                    DebugUtil.Log(result.RawResult);
                    //return true;
                }
                );

            return true;
        }

        void HandleFacebookDelegate1(IAppRequestResult result)
        {
        }


        void HandleFacebookDelegate(IAppRequestResult result)
        {
        }


        /// <summary>
        /// Login with the specified callback.
        /// 调起facebook登陆，会异步回调 callback。如已登录会直接返回UserInfo
        /// </summary>
        /// <param name="callback">Callback.</param>
        public UserInfo Login(Action<string, UserInfo, SDKError> callback)
        {
            DebugUtil.Log("此处有log: IsLoggedIn()  " + IsLoggedIn().ToString());

            if (IsLoggedIn())
            {
                //if (!DragonNativeBridge.IsFacebookDataAccessValid())
                //{
                //    DebugUtil.LogWarning("Facebook: 需要刷新Data Access!");
                //    DragonNativeBridge.ReauthorizeFacebookDataAccess();
                //}
                return GetUserInfo();
            }

            if (callback != null)
            {
                m_callback = callback;
            }

            List<int> permisssions = new List<int>();
            if (ConfigurationController.Instance.kUserPermissionLoginToken)
            {
                permisssions.Add(Dlugin.Constants.kUserPermissionLoginToken);
            }
            if (ConfigurationController.Instance.kUserPermissionBasicInfo)
            {
                permisssions.Add(Dlugin.Constants.kUserPermissionBasicInfo);
            }
            if (ConfigurationController.Instance.kUserPermissionPublish)
            {
                permisssions.Add(Dlugin.Constants.kUserPermissionPublish);
            }
            if (ConfigurationController.Instance.kUserPermissionFriendList)
            {
                permisssions.Add(Dlugin.Constants.kUserPermissionFriendList);
            }
            Dlugin.SDK.GetInstance().loginService.Login(permisssions.ToArray());
            return null;
        }

        public void LogOut(Action<string, UserInfo, SDKError> callback)
        {
            if (callback != null)
            {
                m_callback = callback;
            }

            Dlugin.SDK.GetInstance().loginService.Logout();
        }

        public void ReauthFacebookToken(Action<string, UserInfo, SDKError> callback)
        {
            DebugUtil.Log("ReauthFacebookToken 此处有log: IsLoggedIn()  " + IsLoggedIn().ToString());

            if (callback != null)
            {
                m_callback = callback;
            }

            List<int> permisssions = new List<int>();
            if (ConfigurationController.Instance.kUserPermissionLoginToken)
            {
                permisssions.Add(Dlugin.Constants.kUserPermissionLoginToken);
            }
            if (ConfigurationController.Instance.kUserPermissionBasicInfo)
            {
                permisssions.Add(Dlugin.Constants.kUserPermissionBasicInfo);
            }
            if (ConfigurationController.Instance.kUserPermissionPublish)
            {
                permisssions.Add(Dlugin.Constants.kUserPermissionPublish);
            }
            if (ConfigurationController.Instance.kUserPermissionFriendList)
            {
                permisssions.Add(Dlugin.Constants.kUserPermissionFriendList);
            }
            Dlugin.SDK.GetInstance().loginService.ReauthToken(permisssions.ToArray());
        }

        /// <summary>
        /// If facebook is logged in.
        /// 判断是否已登陆 facebook
        /// </summary>
        /// <returns><c>true</c>, if logged in was ised, <c>false</c> otherwise.</returns>
        public bool IsLoggedIn()
        {
            return Dlugin.SDK.GetInstance().loginService.CheckLoggedIn();
        }

        void onLoginOver(string pluginId, UserInfo userInfo, SDKError error)
        {
            if (error.err == 0)
            {
                DebugUtil.Log("点击faceBook登录返回pluginId：  " + pluginId);
                DebugUtil.Log("点击faceBook登录返回userInfo：  " + userInfo.userId);
                DebugUtil.Log("点击faceBook登录返回userInfo.userName：  " + userInfo.userName);
                DebugUtil.Log("点击faceBook登录返回userInfo.userToken：  " + userInfo.userToken);
                DebugUtil.Log("点击faceBook登录返回error： 错误码为：  " + error.err);
            }
            else
            {
                DebugUtil.Log("facebook登录失败{0}", error.errmsg);
            }

            if (m_callback != null)
            {
                m_callback(pluginId, userInfo, error);
                m_callback = null;
            }
        }

        void onLogOutOver(string pluginId, UserInfo userInfo, SDKError error)
        {
            if (error.err == 0)
            {
                DebugUtil.Log("解绑facebook成功");
            }
            else
            {
                DebugUtil.Log("解绑facebook失败{0}", error.errmsg);
            }

            if (m_callback != null)
            {
                m_callback(pluginId, userInfo, error);
                m_callback = null;
            }
        }

        void onReauthTokenOver(string pluginId, UserInfo userInfo, SDKError error)
        {
            if (error.err == 0)
            {
                DebugUtil.Log("重新绑定facebook成功");
            }
            else
            {
                DebugUtil.Log("重新绑定facebook失败{0}", error.errmsg);
            }

            if (m_callback != null)
            {
                m_callback(pluginId, userInfo, error);
                m_callback = null;
            }
        }

        /// <summary>
        /// Gets the FB Friend in game.
        /// </summary>
        public void GetFBFriendInGame(Action<List<FacebookFriendInfoModel>> action)
        {
            DebugUtil.Log("GetFBFriendInGame");
            if (!AccountManager.Instance.HasBindFacebook())
            {
                if (action != null)
                {
                    action(new List<FacebookFriendInfoModel>());
                }
                return;
            }

            RequestFBData((ret) =>
            {
                if (100 == ret || 1 == ret)
                {
                    if (1 == ret)
                    {
                        EventManager.Instance.Trigger<SDKEvents.ReauthFacebookDataAccessSuccessEvent>().Trigger();
                        DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.ReauthFacebookDataAccessSuccess);
                    }

                    FB.API("me/friends?fields=id,name,picture", HttpMethod.GET, (result) =>
                    {
                        DebugUtil.Log(result.RawResult);
                        DebugUtil.Log(result.Error);
                        DebugUtil.Log(result.ResultList);
                        DebugUtil.Log(result.ResultDictionary);

                        if (result.Error != null)
                        {
                            // error happened
                            DebugUtil.Log("Error fetching friends : " + result.Error.ToString());
                            if (actionQueueLock.IsWriteLockHeld || actionQueueLock.TryEnterWriteLock(200))
                            {
                                try
                                {
                                    callActionQueue.Enqueue(() =>
                                    {
                                        if (action != null)
                                        {
                                            action(new List<FacebookFriendInfoModel>());
                                        }
                                    });
                                }
                                finally
                                {
                                    try
                                    {
                                        actionQueueLock.ExitWriteLock();
                                    }
                                    catch (SynchronizationLockException e)
                                    {
                                        DragonU3DSDK.DebugUtil.Log("SynchronizationLockException : " + e.Message);
                                    }
                                }
                            }

                            CheckGraphAPIError(result);

                            return;
                        }

                        FacebookRawDataModel o = JsonConvert.DeserializeObject<FacebookRawDataModel>(result.RawResult);

                        if (o.Data == null)
                        {
                            if (actionQueueLock.IsWriteLockHeld || actionQueueLock.TryEnterWriteLock(200))
                            {
                                try
                                {
                                    callActionQueue.Enqueue(() =>
                                    {
                                        if (action != null)
                                        {
                                            action(new List<FacebookFriendInfoModel>());
                                        }
                                    });
                                }
                                finally
                                {
                                    try
                                    {
                                        actionQueueLock.ExitWriteLock();
                                    }
                                    catch (SynchronizationLockException e)
                                    {
                                        DragonU3DSDK.DebugUtil.Log("SynchronizationLockException : " + e.Message);
                                    }
                                }
                            }
                            return;
                        }

                        List<FacebookFriendInfoModel> friends = new List<FacebookFriendInfoModel>();

                        foreach (FacebookFriendModel model in o.Data)
                        {
                            FacebookFriendInfoModel friend = new FacebookFriendInfoModel
                            {
                                Id = model.Id,
                                Name = model.Name,
                                AvatarUrl = model.Picture.Data.Url,
                                AvatarWidth = model.Picture.Data.Width,
                                AvatarHeight = model.Picture.Data.Height,
                                Is_sihouette = model.Picture.Data.Is_sihouette
                            };
                            friends.Add(friend);
                        }

                        DebugUtil.Log("Friends Count = " + friends.Count);
                        if (actionQueueLock.IsWriteLockHeld || actionQueueLock.TryEnterWriteLock(200))
                        {
                            try
                            {
                                callActionQueue.Enqueue(() =>
                                {
                                    if (action != null)
                                    {
                                        action(friends);
                                    }
                                });
                            }
                            finally
                            {
                                try
                                {
                                    actionQueueLock.ExitWriteLock();
                                }
                                catch (SynchronizationLockException e)
                                {
                                    DragonU3DSDK.DebugUtil.Log("SynchronizationLockException : " + e.Message);
                                }
                            }
                        }
                    });
                }
                else
                {
                    EventManager.Instance.Trigger<SDKEvents.ReauthFacebookDataAccessFailureEvent>().Trigger();
                    DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.ReauthFacebookDataAccessFailure);

                    if (actionQueueLock.IsWriteLockHeld || actionQueueLock.TryEnterWriteLock(200))
                    {
                        try
                        {
                            callActionQueue.Enqueue(() =>
                            {
                                if (action != null)
                                {
                                    action(new List<FacebookFriendInfoModel>());
                                }
                            });
                        }
                        finally
                        {
                            try
                            {
                                actionQueueLock.ExitWriteLock();
                            }
                            catch (SynchronizationLockException e)
                            {
                                DragonU3DSDK.DebugUtil.Log("SynchronizationLockException : " + e.Message);
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Gets the FB Friend invitable.
        /// </summary>
        public void GetFBFriendInvitable(Action<List<FacebookFriendInfoModel>> action)
        {
            if (!AccountManager.Instance.HasBindFacebook())
            {
                return;
            }

            RequestFBData((ret) =>
            {
                if (100 == ret || 1 == ret)
                {
                    if (1 == ret)
                    {
                        EventManager.Instance.Trigger<SDKEvents.ReauthFacebookDataAccessSuccessEvent>().Trigger();
                        DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.ReauthFacebookDataAccessSuccess);
                    }

                    FB.API("/me/invitable_friends?fields=id,name,picture", HttpMethod.GET, (result) =>
                    {
                        if (result.Error != null)
                        {
                            // error happened
                            DebugUtil.Log("Error fetching friends : " + result.Error.ToString());
                            if (action != null)
                            {
                                action(new List<FacebookFriendInfoModel>());
                            }

                            CheckGraphAPIError(result);

                            return;
                        }

                        FacebookRawDataModel o = JsonConvert.DeserializeObject<FacebookRawDataModel>(result.RawResult);

                        if (o.Data == null)
                        {
                            if (action != null)
                            {
                                action(new List<FacebookFriendInfoModel>());
                            }
                            return;
                        }

                        List<FacebookFriendInfoModel> friends = new List<FacebookFriendInfoModel>();

                        foreach (FacebookFriendModel model in o.Data)
                        {
                            FacebookFriendInfoModel friend = new FacebookFriendInfoModel
                            {
                                Id = model.Id,
                                Name = model.Name,
                                AvatarUrl = model.Picture.Data.Url,
                                AvatarWidth = model.Picture.Data.Width,
                                AvatarHeight = model.Picture.Data.Height,
                                Is_sihouette = model.Picture.Data.Is_sihouette
                            };
                            friends.Add(friend);
                        }

                        DebugUtil.Log("Friends Count = " + friends.Count);
                        if (action != null)
                        {
                            action(friends);
                        }
                    });
                }
                else
                {
                    EventManager.Instance.Trigger<SDKEvents.ReauthFacebookDataAccessFailureEvent>().Trigger();
                    DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.ReauthFacebookDataAccessFailure);

                    if (action != null)
                    {
                        action(new List<FacebookFriendInfoModel>());
                    }
                }
            });
        }

        public bool MessangerShare(String linkUrl, String imageUrl, String pageId, String title, String subTitle, String buttonText)
        {
            return DragonU3DSDK.DragonNativeBridge.FBMessagerShare(linkUrl, imageUrl, pageId, title, subTitle, buttonText);
        }

        /// <summary>
        /// Share link to User's Facebook Feed.
        /// </summary>
        /// <returns><c>true</c>, if share was fed, <c>false</c> otherwise.</returns>
        /// <param name="link">Link.</param>
        /// <param name="linkName">Link name.</param>
        /// <param name="linkCaption">Link caption.</param>
        /// <param name="linkDescription">Link description.</param>
        /// <param name="picture">Picture.</param>
        /// <param name="mediaSource">Media source.</param>
        public bool FeedShare(Uri link, string linkName = "", string linkCaption = "", string linkDescription = "", Uri picture = null, string mediaSource = "")
        {
            //FB.FeedShare(link: link, linkName: linkName, linkCaption: linkCaption, linkDescription: linkDescription, picture: picture, mediaSource: mediaSource, callback: null);

            FB.ShareLink(link, linkName, linkDescription, picture, null);

            return true;
        }

        public void OnFacebookAuthExpire()
        {
            // TODO
        }

        public void RequestFBData(Action<int> action)
        {
            if (!DragonNativeBridge.isFaceBookAccessTokenActive())//access token 无效
            {
                DebugUtil.Log(" AccessToken is inactive, need ReauthFacebookAccessToken");

                EventManager.Instance.Trigger<SDKEvents.ReauthFacebookAccessTokenComfirmEvent>().Trigger();
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.ReauthFacebookAccessTokenComfirm);
            }
            else if (DragonNativeBridge.isFacebookDataAccessExpired())//data access 过期
            {
                DebugUtil.Log(" DataAccess is Expired, need ReauthorizeFacebookDataAccess");

                EventManager.Instance.Trigger<SDKEvents.ReauthFacebookDataAccessPopEvent>().Trigger();
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.ReauthFacebookDataAccessPop);

                DragonNativeBridge.ReauthorizeFacebookDataAccess(action);
            }
            else
            {
                action(100);
            }
        }

        public bool ReauthFacebookAccessTokenCareCode = true;//调控不同版本参数，临时变量，观察一段时间后会删掉 -- 2020.8.25 qibo.li
        public void CheckGraphAPIError(IGraphResult result)
        {
            {
                var errorEvent = new BiEventCommon.Types.ErrorEvent
                {
                    Errno = "FacebookGraphAPIError",
                    Errmsg = string.Format("error : {0} result : {1}", result.Error, result.RawResult)
                };
                BIManager.Instance.SendErrorEvent(errorEvent);
            }
            JObject j_error = new JObject();
            try
            {
                j_error = JObject.Parse(result.RawResult);
            }
            catch (JsonReaderException e)
            {
                DebugUtil.Log("FB.API error json format = " + e.ToString());
            }
            finally
            {
                JToken error_token;
                if (j_error.TryGetValue("error", out error_token))
                {
                    string type = error_token["type"].ToString().ToLower();
                    string code = error_token["code"].ToString();
                    string error_subcode = string.Empty;
                    if (null != error_token["error_subcode"])
                        error_subcode = error_token["error_subcode"].ToString();
                    if (type.Equals("oauthexception"))
                    {
                        if (!ReauthFacebookAccessTokenCareCode || (code.Equals("190") && error_subcode.Equals("460")))
                        {
                            EventManager.Instance.Trigger<SDKEvents.ReauthFacebookAccessTokenComfirmEvent>().Trigger();
                        }
                    }
                }
            }
        }
    }
}
