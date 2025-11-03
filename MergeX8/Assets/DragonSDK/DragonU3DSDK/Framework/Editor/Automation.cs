using UnityEngine;
using System.IO;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using UnityEditor;
using System.Text;
using UnityEditor.Build.Reporting;
using DragonU3DSDK.Asset;
using Unity;
using Dlugin;
using UnityEditor.Purchasing;
using UnityEngine.Purchasing;

namespace DragonU3DSDK
{
    public partial class Automation : Editor
    {
        public static readonly string commandProtocol = @"Assets/../../ExcelTools/build_protocol.sh";
        public static readonly string commandStorage = @"Assets/../../ExcelTools/build_storage.sh";
        public static readonly string commandConfigs = @"Assets/../../ExcelTools/gbuild.sh";

        [MenuItem("Automation/BuildProtocol", false, 1)]
        public static void BuildProtocol()
        {
            ShellExecutor.ExecuteShell(commandProtocol, "");
            AssetDatabase.Refresh();
        }

        [MenuItem("Automation/ClearAllLocalData", false, 2)]
        public static void ClearAllLocalData()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Automation/BuildStorage", false, 3)]
        public static void BuildStorage()
        {
            ShellExecutor.ExecuteShell(commandStorage, "");
            AssetDatabase.Refresh();
        }

        [MenuItem("Automation/BuildConfigs", false, 4)]
        public static void BuildConfigs()
        {
            ShellExecutor.ExecuteShell(commandConfigs, "");
            AssetDatabase.Refresh();
        }

        [MenuItem("Automation/DeleteAllEmptyDirectories", false, 5)]
        public static void FindAndRemove()
        {
            var root = Application.dataPath;
            string[] dirs = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);
            List<DirectoryInfo> emptyDirs = new List<DirectoryInfo>();
            foreach (var dir in dirs)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                if (IsDirectoryEmpty(di))
                    emptyDirs.Add(di);
            }
            foreach (var emptyDir in emptyDirs)
            {
                if (Directory.Exists(emptyDir.FullName))
                {
                    Directory.Delete(emptyDir.FullName, true);
                    DebugUtil.Log("Recursively delete folder: " + emptyDir.FullName);
                }
            }
            AssetDatabase.Refresh();
        }

        static bool HasNoFile(DirectoryInfo dir)
        {
            bool noFile = true;
            foreach (var file in dir.GetFiles())
            {
                if (file.Name == ".DS_Store")
                    continue;

                if (file.Name.EndsWith(".meta") && Directory.Exists(
                        Path.Combine(dir.FullName, file.Name.Substring(0, file.Name.IndexOf(".meta")))))
                    continue;

                noFile = false;
                break;
            }
            return noFile;
        }

        static bool IsDirectoryEmpty(DirectoryInfo dir)
        {
            if (HasNoFile(dir))
            {
                var subDirs = dir.GetDirectories();
                bool allEmpty = true;
                foreach (var subDir in subDirs)
                {
                    if (!IsDirectoryEmpty(subDir))
                    {
                        allEmpty = false;
                        break;
                    }
                }
                return allEmpty;
            }
            return false;
        }

        [MenuItem("Automation/Tools/ExportScriptableObj/Configuration", false, 2)]
        public static void CreateConfigrationAsset()
        {
            CreateAsset<ConfigurationController>(ConfigurationController.ConfigurationControllerPath);
        }

        [MenuItem("Automation/Tools/ExportScriptableObj/DebugConfig", false, 3)]
        public static void CreateServerConfigAsset()
        {
            CreateAsset<DebugConfigController>(DebugConfigController.DebugConfigControllerPath);
        }
        [MenuItem("Automation/Tools/ExportScriptableObj/AssetConfig", false, 4)]
        private static void CreateController()
        {
            CreateAsset<AssetConfigController>(AssetConfigController.AssetConfigPath);
        }
        //[MenuItem("Automation/Tools/ExportScriptableObj/OutsideAssetConfig", false, 4)]
        //private static void CreateOutsideAssetConfig()
        //{
        //    CreateAsset<OutsideAssetConfigController>(OutsideAssetConfigController.AssetConfigPath);
        //}
        //[MenuItem("AssetBundle/HomeRoom/创建HomeRoomConfig", false, 1)]
        private static void CreateHomeRoomController()
        {
            CreateAsset<HomeRoomConfigController>(HomeRoomConfigController.AssetConfigPath);
        }

        

        //[MenuItem("AssetBundle/CookingGame/创建CookingGameConfig", false, 1)]
        private static void CreateCookingGameController()
        {
            CreateAsset<CookingGameConfigController>(CookingGameConfigController.AssetConfigPath);
        }

        [MenuItem("Automation/Tools/ExportScriptableObj/ResPublicLibraryController", false, 2)]
        public static void CreateAssetCommon()
        {
            CreateAsset<ResPublicLibraryController>(ResPublicLibraryController.AssetConfigPath);
        }
        
        [MenuItem("Automation/Tools/Build/iOSBuild", false, 4)]
        public static void iOSBuild()
        {
            Build(BuildTarget.iOS);
        }

        [MenuItem("Automation/Tools/Build/AndroidBuild", false, 5)]
        public static void AndroidBuild()
        {
            if (CheckHasSubChannelSet()) throw new Exception("子渠道相关配置侵入了主渠道打包环境，请调整打包机流程！");
            Build(BuildTarget.Android);
        }
        
        [MenuItem("Automation/Tools/Build/AndroidBuild_SubChannel_Amazon", false, 6)]
        public static void AndroidBuild_SubChannel_Amazon()
        {
            if (!CheckIsSubChannelSet()) throw new Exception("子渠道相关配置不完整，请调整打包机流程！");
            Build(BuildTarget.Android);
        }

        [MenuItem("Automation/Tools/Build/iOSBuild_Debug", false, 7)]
        public static void iOSBuild_Debug()
        {
            Build(BuildTarget.iOS, true);
        }

        [MenuItem("Automation/Tools/Build/AndroidBuild_Debug", false, 8)]
        public static void AndroidBuild_Debug()
        {
            if (CheckHasSubChannelSet()) throw new Exception("子渠道相关配置侵入了主渠道打包环境，请调整打包机流程！");
            Build(BuildTarget.Android, true);
        }
        
        [MenuItem("Automation/Tools/Build/AndroidBuild_Debug_SubChannel_Amazon", false, 9)]
        public static void AndroidBuild_Debug_SubChannel_Amazon()
        {
            if (!CheckIsSubChannelSet()) throw new Exception("子渠道相关配置不完整，请调整打包机流程！");
            Build(BuildTarget.Android, true);
        }

        [MenuItem("Automation/Tools/Build/MacBuild_Debug", false, 10)]
        public static void MacBuild_Debug()
        {
            Build(BuildTarget.StandaloneOSX, true);
        }

        [MenuItem("Automation/Tools/Build/ForceResolve", false, 9)]
        public static void ForceResovle()
        {
            //Build前Resolve 所有依赖
            GooglePlayServices.PlayServicesResolver.ResolveSync(true);

        }

        class BuildArgs
        {
            public BuildTarget target;
            public bool isDebug;
        }
        [UnityEditor.Callbacks.DidReloadScripts]
        static void CheckBuild()
        {
            var desc = EditorPrefs.GetString("BuildParams", "");
            if (!string.IsNullOrEmpty(desc))
            {
                EditorPrefs.DeleteKey("BuildParams");
                BuildArgs args = JsonUtility.FromJson<BuildArgs>(desc);
                AfterBuild(args.target, args.isDebug);
            }
        }
        
        static void Build(BuildTarget target, bool isDebug = false)
        {
            BuildArgs args = new BuildArgs();
            args.target = target;
            args.isDebug = isDebug;

            PreBuild(target, isDebug);

            bool wait = false;
            var type = System.Reflection.Assembly.GetExecutingAssembly().GetType("CKEditor." + "ClientMacro");
            if (type != null)
            {
                type.InvokeMember("SetPlatform", System.Reflection.BindingFlags.InvokeMethod |
                 System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, null, null);
            }
            
            EditorPrefs.SetBool("BuildIsDebug", isDebug);
            EditorPrefs.DeleteKey("AppLovinQualityServiceState");

            if (EditorApplication.isCompiling)
            {
                var str = JsonUtility.ToJson(args);
                EditorPrefs.SetString("BuildParams", str);
                wait = true;
            }

            if (!wait)
            {
                AfterBuild(target, isDebug);

#if FORCE_RESET_PROJECT
                if (Application.isBatchMode)
                {
                    ResetProject();
                }
#endif
            }
        }

        static void ResetProject()
        {
            Process process = new Process();
            process.StartInfo.FileName = Application.dataPath + "/DragonSDK/reset_project";
            process.StartInfo.Arguments = "";
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
            process.Dispose();
        }

        static void PreBuild(BuildTarget target, bool isDebug = false)
        {
            ////检测max-ironsource
            //{
            //    AssetDatabase.CopyAsset(
            //        "Assets/DragonSDK/DragonU3DSDK/SDK/Dlugins/Managed/MaxPlugin/MaxSdk/Mediation/IronSource/Editor/Dependencies.xml.back",
            //        "Assets/DragonSDK/DragonU3DSDK/SDK/Dlugins/Managed/MaxPlugin/MaxSdk/Mediation/IronSource/Editor/Dependencies.xml");
            //}
            
            ////max-ironsource和ironsource版本一致检测
            //{
            //    TextAsset version_max = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/DragonSDK/DragonU3DSDK/SDK/Dlugins/Managed/MaxPlugin/MaxSdk/Mediation/IronSource/Editor/Dependencies.xml");
            //    TextAsset version_ironsource = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/DragonSDK/DragonU3DSDK/SDK/Dlugins/Managed/IronSourcePlugin/IronSource/Editor/IronSourceSDKDependencies.xml");
            //    if (null != version_max)
            //    {
            //        if (!version_max.text.Contains(
            //                "<androidPackage spec=\"com.applovin.mediation:ironsource-adapter:7.1.5.1.2\">") ||
            //            !version_max.text.Contains(
            //                "<iosPod name=\"AppLovinMediationIronSourceAdapter\" version=\"7.1.5.0.0\" />"))
            //        {
            //            throw new Exception("max-ironsource is not the specified version！！！");
            //        }
            //    }
                
            //    if (null != version_ironsource)
            //    {
            //        if (!version_ironsource.text.Contains(
            //                "<androidPackage spec=\"com.ironsource.sdk:mediationsdk:7.1.5.1\">") ||
            //            !version_ironsource.text.Contains("<iosPod name=\"IronSourceSDK\" version=\"7.1.5.0\">"))
            //        {
            //            throw new Exception("ironsource is not the specified version！！！");
            //        }
            //    }
            //}
            
            ////max-ironsource和ironsource互斥
            //IronSourceConfigInfo config = PluginsInfoManager.Instance.GetPluginConfig<IronSourceConfigInfo>(Constants.IronSource);
            //if (config != null)
            //{
            //    if ((target == BuildTarget.Android && config.ExclusivehMaxAndroid) ||
            //        (target == BuildTarget.iOS && config.ExclusivehMaxiOS))
            //    {
            //        AssetDatabase.DeleteAsset("Assets/DragonSDK/DragonU3DSDK/SDK/Dlugins/Managed/MaxPlugin/MaxSdk/Mediation/IronSource/Editor/Dependencies.xml");
            //    }
            //}
        }

        static void AfterBuild(BuildTarget target, bool isDebug = false)
        {
            SDKEditor.SetFirebase();
            if (target == BuildTarget.Android) SDKEditor.SetAPT(isDebug);
            PluginConfigInfo info = PluginsInfoManager.LoadPluginConfig();

            if (info != null && info.m_Map.ContainsKey(Constants.FaceBook))
            {
                FacebookConfigInfo fbInfo = info.m_Map[Constants.FaceBook] as FacebookConfigInfo;
                SDKEditor.SetFacebook(fbInfo.AppID, fbInfo.ClientToken);
            }

#if !UNITY_2019_1_OR_NEWER
            //所有项目已经弃用 2018.4以下，2019.2以下版本。NDK统一为r16b,以下代码已无用。
            //if (!EditorPrefs.HasKey("AndroidNdkRoot") && string.IsNullOrEmpty(EditorPrefs.GetString("AndroidNdkRoot")))
            //{
            //    EditorPrefs.SetString("AndroidNdkRoot", "/Users/dragonplus/Downloads/android-ndk-r13b");
            //}

            //if (!EditorPrefs.HasKey("JdkPath") || string.IsNullOrEmpty(EditorPrefs.GetString("JdkPath")))
            //{
            //    EditorPrefs.SetString("JdkPath", "/Library/Java/JavaVirtualMachines/jdk1.8.0_181.jdk/Contents/Home/");
            //}

            //if (!EditorPrefs.HasKey("AndroidSdkRoot") || string.IsNullOrEmpty(EditorPrefs.GetString("AndroidSdkRoot")))
            //{
            //    EditorPrefs.SetString("AndroidSdkRoot", "/Users/dragonplus/Library/Android/sdk");
            //}

#endif
            DebugUtil.Log("Android NDK Path is now {0}", EditorPrefs.GetString("AndroidNdkRoot"));

#if UNITY_2018_4_OR_NEWER || UNITY_2019_1_OR_NEWER

            //强制设置NDK版本
            //Unity修改本 key 为 AndroidNdkRootR16b，详见：https://forum.unity.com/threads/android-ndk-path-editorprefs-key-changed.639103/
            if (!EditorPrefs.HasKey("AndroidNdkRoot") || string.IsNullOrEmpty(EditorPrefs.GetString("AndroidNdkRoot")))
                EditorPrefs.SetString("AndroidNdkRoot", "/Users/dragonplus/Downloads/android-ndk-r16b");
            if (!EditorPrefs.HasKey("AndroidNdkRootR16b") || string.IsNullOrEmpty(EditorPrefs.GetString("AndroidNdkRootR16b")))
                EditorPrefs.SetString("AndroidNdkRootR16b", "/Users/dragonplus/Downloads/android-ndk-r16b");
#endif
            
#if UNITY_2019_3_OR_NEWER
            if (!EditorPrefs.HasKey("AndroidNdkRootR19") || string.IsNullOrEmpty(EditorPrefs.GetString("AndroidNdkRootR19")))
                EditorPrefs.SetString("AndroidNdkRootR19", "/Users/dragonplus/Downloads/android-ndk-r19");
#endif
            

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetScenes();

            PlayerSettings.SplashScreen.showUnityLogo = false;

            string platform = "";
            string platformFolder = "";
            if (target == BuildTarget.Android)
            {
                platform = "Android";

                EditorUserBuildSettings.androidCreateSymbolsZip = false;
                if (!isDebug)
                {
                    EditorUserBuildSettings.androidCreateSymbolsZip = true;

                    //PlayerSettings.Android.keystoreName = "/Users/dragonplus/flydragon.keystore";
                    //PlayerSettings.Android.keystorePass = "FlyDragon123";
                    //PlayerSettings.Android.keyaliasName = "flydragon.keystore";
                    //PlayerSettings.Android.keyaliasPass = "FlyDragon123";

#if UNITY_2019_1_OR_NEWER

                    PlayerSettings.Android.useCustomKeystore = true;
#endif

                    if (ConfigurationController.Instance.AndroidKeyStoreUseConfiguration)
                    {
                        PlayerSettings.Android.keystoreName = ConfigurationController.Instance.AndroidKeyStorePath;
                        PlayerSettings.Android.keystorePass = ConfigurationController.Instance.AndroidKeyStorePass;
                        PlayerSettings.Android.keyaliasName = ConfigurationController.Instance.AndroidKeyStoreAlias;
                        PlayerSettings.Android.keyaliasPass = ConfigurationController.Instance.AndroidKeyStoreAliasPass;
                    }
                    else
                    {
#if !UNITY_2019_1_OR_NEWER
                        PlayerSettings.Android.keystoreName = "/Users/dragonplus/smartfunapp.keystore";
                        PlayerSettings.Android.keystorePass = "SmartFun123";
                        PlayerSettings.Android.keyaliasName = "SmartFun.keystore";
                        PlayerSettings.Android.keyaliasPass = "SmartFun123";
#endif
                    }
                }

#if UNITY_2019_1_OR_NEWER
                DebugUtil.Log("useCustomKeystore is " + PlayerSettings.Android.useCustomKeystore);
#endif
                DebugUtil.Log("PlayerSettings.Android.keystoreName is " + PlayerSettings.Android.keystoreName);
                DebugUtil.Log("PlayerSettings.Android.keystorePass is " + PlayerSettings.Android.keystorePass);
                DebugUtil.Log("PlayerSettings.Android.keyaliasName is " + PlayerSettings.Android.keyaliasName);
                DebugUtil.Log("PlayerSettings.Android.keyaliasPass is " + PlayerSettings.Android.keyaliasPass);

#if UNITY_2018_4_OR_NEWER || UNITY_2019_1_OR_NEWER
                if (ConfigurationController.Instance.BuildAppBundle)
                    EditorUserBuildSettings.buildAppBundle = true;
                else
                    EditorUserBuildSettings.buildAppBundle = false;

#endif
            }
            else if (target == BuildTarget.iOS)
            {
                platform = "iOS";
                PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            }
            platformFolder = Path.GetFullPath(Application.dataPath + "/../" + platform + "/build/");
            if (!Directory.Exists(platformFolder))
                Directory.CreateDirectory(platformFolder);

            if (target == BuildTarget.Android)
            {
                string productName = PlayerSettings.productName;

                string subChannelSymbol = GetSubChannelName();
                if (!string.IsNullOrEmpty(subChannelSymbol))
                {
                    if (subChannelSymbol.Equals("SUB_CHANNEL_AMAZON"))
                    {
                        productName += "_Amazon";
                        UnityPurchasingEditor.TargetAndroidStore(AppStore.AmazonAppStore);
#if UNITY_2022_3_OR_NEWER
                        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
                        EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Disabled;
#else
                        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel19;
                        EditorUserBuildSettings.androidCreateSymbolsZip = false;
#endif
                    }
                }
                
#if UNITY_2018_4_OR_NEWER || UNITY_2019_1_OR_NEWER
                if (ConfigurationController.Instance.BuildAppBundle)
                    platformFolder = platformFolder + productName + ".aab";
                else
                    platformFolder = platformFolder + productName + ".apk";
#else
                platformFolder = platformFolder + productName + ".apk";
#endif
            }

            if (target == BuildTarget.StandaloneOSX)
            {
                platformFolder = platformFolder + PlayerSettings.productName + ".app";
            }

            buildPlayerOptions.locationPathName = platformFolder;
            
            buildPlayerOptions.target = target;
            if (isDebug)
            {
                buildPlayerOptions.options |= BuildOptions.Development;
            }
            //else
            //{
            //    buildPlayerOptions.options = BuildOptions.None;
            //}

#if UNITY_ANDROID && ENABLE_ASSET_DELIVERY
            bool flag = BuildWithAssetPack(buildPlayerOptions.locationPathName, buildPlayerOptions.options);
            BuildResult result = flag ? BuildResult.Succeeded : BuildResult.Failed;
#else
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildResult result = report.summary.result;
#endif
            CheckAppLovinMAXQualityService();
            EditorPrefs.DeleteKey("BuildIsDebug");
            DebugUtil.Log(result.ToString());
        }

        public static void CreateAsset<T>(string path) where T : ScriptableObject
        {
            var folder = Path.GetDirectoryName(Application.dataPath + "/Resources/" + path);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            T ac = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(ac, "Assets/Resources/" + path + ".asset");
        }

        static string[] GetScenes()
        {
            List<string> scenes = new List<string>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                scenes.Add(scene.path);
            }

            return scenes.ToArray();
        }
        
        //检测AppLovinQualityService是否集成成功
        static void CheckAppLovinMAXQualityService()
        {
            if (AppLovinSettings.Instance.QualityServiceEnabled)
            {
#if UNITY_ANDROID
                string applicationGradleBuildFilePath =
                    Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/Assets")) +
#if UNITY_2019_3_OR_NEWER
    #if UNITY_2022_3_OR_NEWER
                    "/Library/Bee/Android/Prj/IL2CPP/Gradle/launcher/build.gradle";
    #else
                    "/Temp/gradleOut/launcher/build.gradle";
    #endif
#else
                    "/Temp/gradleOut/build.gradle";
#endif               
                
                if (!File.Exists(applicationGradleBuildFilePath))
                {
                    throw new Exception("[AppLovin MAX] 找不到 applicationGradleBuildFile");
                }
                string GradleFileInfo = File.ReadAllText(applicationGradleBuildFilePath);
                if (!GradleFileInfo.Contains("applovin-quality-service"))
                {
                    throw new Exception("[AppLovin MAX] QualityService 未集成成功，建议再打一次包试试。");
                }
                DebugUtil.Log("[AppLovin MAX] QualityService 集成成功。");
#else
                if (!EditorPrefs.HasKey("AppLovinQualityServiceState") ||
                    !EditorPrefs.GetBool("AppLovinQualityServiceState"))
                {
                    throw new Exception("[AppLovin MAX] QualityService 未集成成功，建议：再打一次包试试，或者是打包机上ClashX工具没有正常工作【注意不要设置为系统代理】。");
                }
                EditorPrefs.DeleteKey("AppLovinQualityServiceState");
                DebugUtil.Log("[AppLovin MAX] QualityService 集成成功。");
#endif
            }
        }
        
        static string SUB_CHANNEL_DEFINE_SYMBOL_NAME = "SUB_CHANNEL_";
        //配置子渠道宏定义
        public static void ConfigSubChannel()
        {
            //主渠道（Apple、Google）的包需要保证没有子渠道的定义
            //子渠道宏定义标准：SUB_CHANNEL_NAME
            {
                string defineStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                string[] oldDefines = defineStr.Split(';');
                List<string> defines = new List<string>();
                foreach (var d in oldDefines)
                {
                    if (!d.StartsWith(SUB_CHANNEL_DEFINE_SYMBOL_NAME))
                    {
                        defines.Add(d);
                    }
                }
                string subChannel = "";
                string[] cmdArguments = Environment.GetCommandLineArgs();
                
                for (int count = 0; count < cmdArguments.Length; count++)
                {
                    if (cmdArguments[count] == "-subChannel")
                    {
                        subChannel = cmdArguments[count + 1];
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(subChannel))
                {
                    if (!subChannel.StartsWith(SUB_CHANNEL_DEFINE_SYMBOL_NAME))
                    {
                        throw new Exception($"sub channel define name : {subChannel} error!");
                    }
                    DebugUtil.Log($"ConfigSubChannel subChannel name : {subChannel}");
                    defines.Add(subChannel);
                }
                else
                {
                    DebugUtil.Log($"ConfigSubChannel subChannel is nil.");
                }
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines.ToArray());
            }
        }
        
        private static string GetSubChannelName()
        {
            string defineStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            string[] defines = defineStr.Split(';');
            foreach (var p in defines)
            {
                if (p.StartsWith(SUB_CHANNEL_DEFINE_SYMBOL_NAME))
                {
                    return p;
                }
            }
            return null;
        }

        private static bool CheckHasSubChannelSet()
        {
            //symbol
            string symbolName = GetSubChannelName();
            if (!string.IsNullOrEmpty(symbolName))
            {
                DebugUtil.LogError($"[CheckIsSubChannel] symbol : {symbolName}");
                return true;
            }
            
            //pem
            if (FilePathTools.GetFiles("Assets/StreamingAssets", EditorResourcePaths.FILTER_PEM_REGEX, SearchOption.AllDirectories).Length > 0)
            {
                DebugUtil.LogError("[CheckIsSubChannel] exist pem file in streamingAssetsPath");
                return true;
            }
            return false;
        }
        
        private static bool CheckIsSubChannelSet()
        {
            //symbol
            string symbolName = GetSubChannelName();
            if (string.IsNullOrEmpty(symbolName))
            {
                DebugUtil.LogError("[CheckIsSubChannel] symbol is nil");
                return false;
            }
            
            //pem
            if (FilePathTools.GetFiles("Assets/StreamingAssets", EditorResourcePaths.FILTER_PEM_REGEX, SearchOption.AllDirectories).Length == 0)
            {
                DebugUtil.LogError("[CheckIsSubChannel] not exist pem file in streamingAssetsPath");
                return false;
            }
            return true;
        }
    }
}
