using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;
using DragonU3DSDK;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using System;
#endif
using System.IO;

public class IOSPostBuild : MonoBehaviour
{

    public int callbackOrder => 100;

    [PostProcessBuildAttribute(1000)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
#if UNITY_IOS
        if (target != BuildTarget.iOS)
        {
            return;
        }

        string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));
        string targetGUID = "";
#if UNITY_2019_3_OR_NEWER
        targetGUID = proj.GetUnityMainTargetGuid();
#else
        targetGUID = proj.TargetGuidByName("Unity-iPhone");
#endif
        string unityFrameworkTargetGUID = "";
#if UNITY_2019_3_OR_NEWER
        unityFrameworkTargetGUID = proj.GetUnityFrameworkTargetGuid();
#endif

        // string xcodeTargetUnityFramework = proj.TargetGuidByName("UnityFramework");
        // if (!string.IsNullOrEmpty(xcodeTargetUnityFramework))
        // {
        //     proj.AddBuildProperty(xcodeTargetUnityFramework, "OTHER_LDFLAGS", "-ld64");
        // }
        
        string projectGUID = proj.ProjectGuid();
        proj.SetBuildProperty(projectGUID,"ENABLE_BITCODE","NO");
        
        proj.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-lxml2");
        // proj.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ld64");

        // proj.AddFrameworkToProject(targetGUID, "UserNotificationsUI.framework", true);
        proj.AddFrameworkToProject(targetGUID, "iAd.framework", true);
        proj.AddFrameworkToProject(targetGUID, "AdSupport.framework", true);
        proj.AddFrameworkToProject(targetGUID, "AppTrackingTransparency.framework", true);

        // adds the AuthenticationServices.framework as an Optional framework, preventing crashes in
        // iOS versions previous to 13.0
        proj.AddFrameworkToProject(targetGUID, "AuthenticationServices.framework", true);
        if (!string.IsNullOrEmpty(unityFrameworkTargetGUID))
        {
            proj.AddFrameworkToProject(unityFrameworkTargetGUID, "AuthenticationServices.framework", true);
            proj.SetBuildProperty(unityFrameworkTargetGUID, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");   //ERROR ITMS-90206. UnityFramework contains disallowed FrameWorks

#if XCODE_OPT_LEVLE_S
            proj.SetBuildProperty(unityFrameworkTargetGUID, "GCC_OPTIMIZATION_LEVEL", "s");     //GCC -Os
#endif
        }
        proj.SetBuildProperty(targetGUID, "USYM_UPLOAD_AUTH_TOKEN", "FakeToken");
        proj.SetBuildProperty(targetGUID, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        proj.SetBuildProperty(targetGUID, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
        proj.SetBuildProperty(targetGUID,"ENABLE_BITCODE","NO");

#if XCODE_OPT_LEVLE_S
        proj.SetBuildProperty(targetGUID, "GCC_OPTIMIZATION_LEVEL", "s");     //GCC -Os
#endif

        //infoPlist.strings
        string[] localizationDirectories = Directory.GetDirectories($"{Application.dataPath}/DragonSDK/DragonU3DSDK/SDK/Editor/Localizations");
        foreach (var p in localizationDirectories)
        {
            string tempGuid = proj.AddFolderReference(p, p.Substring(p.LastIndexOf("/") + 1));
            proj.AddFileToBuild(targetGUID, tempGuid);
        }
        
        File.WriteAllText(projPath, proj.WriteToString());

        // Read plist
        var plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        DebugUtil.LogError("plist path = {0}", plistPath);

        // Update value
        PlistElementDict rootDict = plist.root;
        if (rootDict.values.ContainsKey("LSApplicationQueriesSchemes"))
        {
            rootDict.values["LSApplicationQueriesSchemes"].AsArray().AddString("fb-messenger-share-api");
        }

        // remove exit on suspend if it exists.
        string exitsOnSuspendKey = "UIApplicationExitsOnSuspend";
        if (rootDict.values.ContainsKey(exitsOnSuspendKey))
        {
            rootDict.values.Remove(exitsOnSuspendKey);
        }
        // remove NSAllowsArbitraryLoadsInWebContent if it exists.
        string appTransportSecurityKey = "NSAppTransportSecurity";
        if (rootDict.values.ContainsKey(appTransportSecurityKey))
        {
            DebugUtil.LogError("plist contains key {0}", appTransportSecurityKey);

            foreach (string key in rootDict.values[appTransportSecurityKey].AsDict().values.Keys)
            {
                DebugUtil.LogError("plist key : {0}", key);
            }

            if (rootDict.values[appTransportSecurityKey].AsDict().values.ContainsKey("NSAllowsArbitraryLoadsInWebContent"))
            {
                DebugUtil.LogError("plist contains key {0}", "NSAllowsArbitraryLoadsInWebContent");
                rootDict.values[appTransportSecurityKey].AsDict().values.Remove("NSAllowsArbitraryLoadsInWebContent");
            }
        }

        if (!rootDict.values.ContainsKey("NSUserTrackingUsageDescription"))
        {
            rootDict.SetString("NSUserTrackingUsageDescription","Tracking allows us to offer you relevant ads more often, and helps us to improve the app. If disable, the ads you see will be less relevant to you. Whichever option you choose, we're so excited to have you play the game!");}


        // Write plist
        File.WriteAllText(plistPath, plist.WriteToString());

        string entitlePath = "Unity-iPhone/" + Application.productName + ".entitlements";
#if UNITY_2019_3_OR_NEWER
        ProjectCapabilityManager projectCapabilityManager = new ProjectCapabilityManager(projPath, entitlePath, "Unity-iPhone", targetGUID);
#else
        ProjectCapabilityManager projectCapabilityManager = new ProjectCapabilityManager(projPath, entitlePath, "Unity-iPhone");
#endif
        
       try
        {
            projectCapabilityManager.AddInAppPurchase();
        }
        catch (Exception e)
        {
            DebugUtil.LogError("add purchase error " + e);
        }

        if (ConfigurationController.Instance.iOSAddSignInWithApple)
        {
            projectCapabilityManager.AddSignInWithApple();
        }
        if (ConfigurationController.Instance.iOSAddPushNotification)
        {
            bool isDebug = true;
            if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
            {
                isDebug = false;
            }

            projectCapabilityManager.AddPushNotifications(isDebug);
        }
        projectCapabilityManager.WriteToFile();
#endif
    }
}