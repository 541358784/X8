using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin.PluginStructs;
using DragonU3DSDK;
using System;
using System.Globalization;

namespace Dlugin
{
    public class SDK
    {
        private static SDK s_Instance = null;
        public static SDK GetInstance()
        {
            if (s_Instance == null)
                s_Instance = new SDK();
            return s_Instance;
        }

        private bool m_Initialized;
        private SDK()
        {
            m_Initialized = false;
        }

        #region init
        public void Initialize(bool isLowPowerDevice = false)
        {
            try
            {
                if (!m_Initialized)
                {
                    m_Initialized = true;
                    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en", false);
#if UNITY_IOS && !UNITY_EDITOR && !GOOGLE_UMP_ENABLE
                    iOSATTManager.Instance.AutoTracking();
#endif
                    LoadConfig();
                    InitializeAllServices(isLowPowerDevice);
                }
            }
            catch (System.Exception ex)
            {
                DebugUtil.LogError("exceptions :" + ex.StackTrace);
            }
        }

        void LoadConfig()
        {
            PluginsInfoManager instance = PluginsInfoManager.Instance;
        }

        void InitializeAllServices(bool isLowPowerDevice = false)
        {
#if UNITY_IOS && !UNITY_EDITOR
            try
            {
                //!!!before initializing the mediation SDK!!!
                if (DragonNativeBridge.iOSSAvailableFourteen() && !PlayerPrefs.HasKey("FacebookAdvertiserTracking"))
                {
                    DragonNativeBridge.SetAdvertiserTrackingEnabled(true);
                    PlayerPrefs.SetInt("FacebookAdvertiserTracking", 1);
                
                    DebugUtil.Log("[Audience Network]SetAdvertiserTrackingEnabled Ture");
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e.ToString());
            }
#endif            
            
            DeepLinkPlugin.Instance.Init();


            if (PluginsInfoManager.Instance.UsePlugin(Constants.FireBase))
            {
                try
                {
                    firebasePlugin = new FirebasePlugin();
                    firebasePlugin.Initialize();
                    firebase = firebasePlugin;
                }
                catch (Exception e)
                {
                    DebugUtil.LogError("Firebase初始化失败 原因：" + e.StackTrace);
                }
            }

            //Login_Dispatcher______________________________
            loginDispatcher = new UserLoginDispatcher();
            //Login______________________________________
            if (PluginsInfoManager.Instance.UsePlugin(Constants.FaceBook))
            {
                try
                {
                    FacebookSDKPlugin fbPlugin = new FacebookSDKPlugin();
                    fbPlugin.Initialize();
                    loginService = fbPlugin;
                }
                catch (Exception e)
                {
                    DebugUtil.LogError("Facebook初始化失败 原因：" + e.StackTrace);
                }
            }

            //Apple Login______________________________________
            AppleLoginDispatcher = new UserLoginDispatcher();
            if (GetiOSAddSignInWithAppleState())
            {
                try
                {
                    AppleSignInPlugin applePlugin = new AppleSignInPlugin();
                    applePlugin.Initialize();
                    AppleLoginService = applePlugin;
                }
                catch (Exception e)
                {
                    DebugUtil.LogError("AppleSignInPlugin初始化失败 原因：" + e.StackTrace);
                }
            }

            if (PluginsInfoManager.Instance.UsePlugin(Constants.Adjust))
            {
                try
                {
                    adjustPlugin = new AdjustPlugin();
                    adjustPlugin.Initialize();
                    dataProvider = adjustPlugin;
                }
                catch (Exception e)
                {
                    DebugUtil.LogError("Adjust初始化失败 原因：" + e.StackTrace);
                }
            }

            //Ads_Dispatcher________________________________
            m_AdsDispather = new AdsEventDispatcher();
            //Ads________________________________________

            try
            {
                m_AdsManager = AdsManager.Instance;
                m_AdsManager.Initialize(isLowPowerDevice);
            }
            catch (Exception e)
            {
                DebugUtil.LogError("广告系统初始化失败 原因：" + e.StackTrace);
            }

            //BI______________________________________________

            //Log__________________________________________
            logService = new GroupLogProvider();

            //IAP
            try
            {
                iapManager = new IAPManager();
            }
            catch (Exception e)
            {
                DebugUtil.LogError("IAP初始化失败 原因：" + e.StackTrace);
            }
            
#if UNITY_ANDROID            
            //Android Performance Tuner
            if (PluginsInfoManager.Instance.UsePlugin(Constants.APT))
            {
                try
                {
                    AptPlugin = new APTPlugin();
                    AptPlugin.Initialize();
                }
                catch (Exception e)
                {
                    DebugUtil.LogError("Android Performance Tuner初始化失败 原因：" + e.StackTrace);
                }
            }
#endif
            
#if GOOGLE_UMP_ENABLE
            UmpManager.Instance.Init();
#endif
            

#if DEBUG || DEVELOPMENT_BUILD

            if (ConfigurationController.Instance.version == VersionStatus.DEBUG)
                Application.logMessageReceived += LogMessageReceived;
#endif
            BestHTTP.HTTPManager.UserAgent = DeviceHelper.GetUserAgent();
// #if !DEVELOPMENT_BUILD
//             DragonNativeBridge.RequestAndLoadInterstitialAd();
// #endif
        }

        bool GetiOSAddSignInWithAppleState()
        {
#if ((UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX) && !UNITY_EDITOR)
           return ConfigurationController.Instance.iOSAddSignInWithApple;
#endif
            return false;
        }

        public void DisposePlugin(string pluginId)
        {
            loginService.DisposePlugin(pluginId);
            m_AdsManager.Clear();
            logService.DisposePlugin(pluginId);
            FormatDebug("Dlugin.DisposePlugin ----> {0} is disposed!", pluginId);
        }

#endregion

#region about_user_login
        public UserLoginDispatcher loginDispatcher { get; private set; }
        public IUserLogin loginService { get; private set; }
        public UserLoginDispatcher AppleLoginDispatcher { get; private set; }
        public IUserLogin AppleLoginService { get; private set; }
#endregion
#region about_ads
        public AdsEventDispatcher m_AdsDispather { get; private set; }
        public AdsManager m_AdsManager { get; private set; }
#endregion
#region about_data_statistic
#endregion
#region about_log
        public GroupLogProvider logService { get; private set; }
#endregion

        public IAPManager iapManager { get; private set; }

        public AdjustPlugin adjustPlugin { get; private set; }
        [Obsolete("Dlugin.dataProvider is deprecated, please use Dlugin.adjustPlugin instead.")]
        public IDataStatProvider dataProvider { get; private set; }

        public FirebasePlugin firebasePlugin { get; private set; }
        [Obsolete("Dlugin.firebase is deprecated, please use Dlugin.firebasePlugin instead.")]
        public IDataStatProvider firebase { get; private set; }
        
        public APTPlugin AptPlugin { get; private set; }

        //public OneSignalPlugin oneSignalPlugin { get; private set; }

        //public IPushProvider pushManager { get; private set; }

        public void Clear()
        {
            //TODO
        }


        internal static void FormatDebug(string format, params object[] objs)
        {
            DebugUtil.Log("*************************************************\n" +
                             "*************************************************\n" +
                             "      [Dlugin log]      " + format + "\n" +
                             "*************************************************\n" +
                             "*************************************************\n", objs);
        }

        internal static void FormatWarning(string format, params object[] objs)
        {
            DebugUtil.LogWarning("*************************************************\n" +
                                    "*************************************************\n" +
                                    "      [Dlugin warning]      " + format + "\n" +
                                    "*************************************************\n" +
                                    "*************************************************\n", objs);
        }

        internal static void FormatError(string format, params object[] objs)
        {
            DebugUtil.LogError("*************************************************\n" +
                                   "*************************************************\n" +
                                   "      [Dlugin error]      " + format + "n" +
                                   "*************************************************\n" +
                                   "*************************************************\n", objs);
        }

        private void LogMessageReceived(string message, string stackTrace, LogType type)
        {
            //if (type == LogType.Exception || type == LogType.Error || type == LogType.Assert)
            if (type == LogType.Exception)
            {
                var errorEvent = new DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.ErrorEvent
                {
                    Errmsg = message,
                    LogType = type.ToString(),
                };
                var stacks = stackTrace.Split('\n');
                foreach (var stack in stacks)
                {
                    errorEvent.Stacks.Add(stack);
                }
                DragonU3DSDK.Network.BI.BIManager.Instance.SendErrorEvent(errorEvent);
            }
        }
    }
}