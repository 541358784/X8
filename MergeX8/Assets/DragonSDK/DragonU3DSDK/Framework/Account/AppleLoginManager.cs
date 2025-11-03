using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using AppleAuth;
using Dlugin;
using Dlugin.PluginStructs;
using DragonU3DSDK;
using UnityEngine;

namespace DragonU3DSDK.Account
{
    public class AppleLoginManager : Manager<AppleLoginManager>
    {
        Action<string, UserInfo, SDKError> m_callback;

        private ReaderWriterLockSlim actionQueueLock = new ReaderWriterLockSlim();
        private Queue<Action> callActionQueue = new Queue<Action>();

        public AppleLoginManager()
        {
            SDK.GetInstance().AppleLoginDispatcher.onLoginOver += onLoginOver;
            SDK.GetInstance().AppleLoginDispatcher.onLogoutOver += onLogOutOver;
        }

        /// <summary>
        /// 是否可以显示苹果登录按钮
        /// </summary>
        public bool CanShowAppleLogin()
        {
            return !AccountManager.Instance.HasBindApple() && AppleAuthManager.IsCurrentPlatformSupported;
        }

        /// <summary>
        /// 是否支持苹果登录
        /// </summary>
        public bool IsCurrentPlatformSupported()
        {
            return AppleAuthManager.IsCurrentPlatformSupported;
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

        void onLoginOver(string pluginId, UserInfo userInfo, SDKError error)
        {
            if (error.err == 0)
            {
                DebugUtil.Log("点击apple登录返回pluginId：  " + pluginId);
                DebugUtil.Log("点击apple登录返回userInfo：  " + userInfo.userId);
                DebugUtil.Log("点击apple登录返回userInfo.userName：  " + userInfo.userName);
                DebugUtil.Log("点击apple登录返回userInfo.userToken：  " + userInfo.userToken);
                DebugUtil.Log("点击apple登录返回error： 错误码为：  " + error.err);
            }
            else
            {
                DebugUtil.Log("apple登录失败{0}", error.errmsg);
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
                DebugUtil.Log("解绑apple成功");
            }
            else
            {
                DebugUtil.Log("解绑apple失败{0}", error.errmsg);
            }

            if (m_callback != null)
            {
                m_callback(pluginId, userInfo, error);
                m_callback = null;
            }
        }

        /// <summary>
        /// If apple is logged in.
        /// 判断是否已登陆 apple
        /// </summary>
        /// <returns><c>true</c>, if logged in was ised, <c>false</c> otherwise.</returns>
        public bool IsLoggedIn()
        {
            return Dlugin.SDK.GetInstance().AppleLoginService.CheckLoggedIn();
        }

        /// <summary>
        /// Gets the user info.
        /// 获取 apple 用户信息，如未登陆录触发登录逻辑
        /// </summary>
        /// <returns>The user info.</returns>
        public UserInfo GetUserInfo()
        {
            if (!IsLoggedIn())
            {
                Login(null);
            }
            return Dlugin.SDK.GetInstance().AppleLoginService.GetUserInfo();
        }

        /// <summary>
        /// Login with the specified callback.
        /// 调起apple登陆，会异步回调 callback。如已登录会直接返回UserInfo
        /// </summary>
        /// <param name="callback">Callback.</param>
        public UserInfo Login(Action<string, UserInfo, SDKError> callback)
        {
            DebugUtil.Log("此处有log: IsLoggedIn()  " + IsLoggedIn().ToString());

            if (IsLoggedIn())
            {
                return GetUserInfo();
            }

            if (callback != null)
            {
                m_callback = callback;
            }

            Dlugin.SDK.GetInstance().AppleLoginService.Login(null);
            return null;
        }

        public void LogOut(Action<string, UserInfo, SDKError> callback)
        {
            if (callback != null)
            {
                m_callback = callback;
            }

            Dlugin.SDK.GetInstance().AppleLoginService.Logout();
        }
    }
}
