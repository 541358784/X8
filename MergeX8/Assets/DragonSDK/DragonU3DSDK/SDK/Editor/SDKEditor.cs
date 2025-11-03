using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using DragonU3DSDK;
using System.Xml;
using Facebook.Unity.Settings;
using Facebook.Unity.Editor;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Dlugin;
using Google.Android.PerformanceTuner;
using Google.Android.PerformanceTuner.Editor;

//using GoogleMobileAds.Editor;

public class SDKEditor : EditorWindow
{
    public const string PACKAGE_NAME_STR = "COM.YOUR.PACKAGE_NAME";

    #region Editor
    static SDKEditor m_EditorWindow = null;

    public Vector2 m_ScrollPosition;

    PluginConfigInfo m_PluginInfo;
    bool m_UseFirebase;
    public FirebaseConfigInfo m_FirebaseConfig;
    bool m_UseFacebook;
    public FacebookConfigInfo m_FacebookConfig;
    bool m_UseAdcolony;
    public AdColonyConfigInfo m_AdcolonyConfig;
    bool m_UseAdmob = true;
    public AdmobConfigInfo m_AdmobConfig;
    bool m_UseAudience;
    public AudienceConfigInfo m_AudienceConfig;
    bool m_UseAdjust;
    public AdjustConfigInfo m_AdjustConfig;
    bool m_UseOneSignal;
    public OneSignalConfigInfo m_OneSignalConfig;
    bool m_UseUnityAds;
    public UnityAdsConfigInfo m_UnityAdsConfig;
    bool m_UseChartboost;
    public ChartboostConfigInfo m_ChartboostConfig;
    bool m_UseIronSource;
    public IronSourceConfigInfo m_IronSourceConfig;
    bool m_UseAppLovin;
    public AppLovinConfigInfo m_AppLovinConfig;
    bool m_UseMAX;
    public MAXConfigInfo m_MAXConfig;
    bool m_UseTapjoy;
    public TapjoyConfigInfo m_TapjoyConfig;
    bool m_UseAPT;
    public APTPluginInfo m_APTConfig;
    [MenuItem("SDK/SDKSetup", false, 1)]
    public static void DluginSDKSetup()
    {
        m_EditorWindow = (SDKEditor)EditorWindow.GetWindowWithRect(typeof(SDKEditor), new Rect(0, 0, 800, 800), true, "SDK初始化");
        m_EditorWindow.Show();
        //CreateConfigurationController();
        //AndroidManifestXML();
    }
    
    private Dictionary<string, bool> foldoutDict = new Dictionary<string, bool>();

    void PropertyAllFields(SerializedProperty property)
    {
        bool useOld = false;
        if (useOld)
        {
            EditorGUILayout.PropertyField(property, true);
        }
        else
        {
            int level = EditorGUI.indentLevel;
            PropertyAllFieldsHelp(property, property.name);
            EditorGUI.indentLevel = level;
        }
    }

    void PropertyAllFieldsHelp(SerializedProperty property, string root)
    {
        int depth = property.depth;
        if (!property.hasVisibleChildren || property.type == "string")
        {
            EditorGUILayout.PropertyField(property, false);
            return;
        }

        string key = property.propertyPath;
        if (!foldoutDict.TryGetValue(key, out bool foldout))
        {
            foldout = depth < 2;
        }

        foldout = EditorGUILayout.Foldout(foldout, property.displayName);
        foldoutDict[key] = foldout;
        
        EditorGUI.indentLevel++;
        while (property.NextVisible(true))
        {
            if (!property.propertyPath.StartsWith(root))
            {
                break;
            }

            if (property.depth > depth)
            {
                if (foldout)
                {
                    PropertyAllFieldsHelp(property, root);
                }
            }
            else
            {
                EditorGUI.indentLevel = property.depth;
                PropertyAllFieldsHelp(property, root);
                return;
            }
        }
        EditorGUI.indentLevel--;
    }

    void OnGUI()
    {
        m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, GUILayout.Height(800));
        GUILayout.Label("使用流程：\n" +
        "1，导入UnityAds 和 Unity IAP 两个内置插件。\n" +
        "2，将Firebase 控制台生成的 google-service.json 和 GoogleService-Info.plist文件引入Assets文件夹内任意位置，注意每个文件只应该有一个副本。\n" +
        "3，填写Facebook ID 和Admob ID(如果使用的话)。\n" +
        "4，点击Set Up 按钮。 \n" +
        "5，在Assets/Resources/Settings/ConfigurationController里面配置其他参数。\n", EditorStyles.boldLabel);


        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("LoadConfig", GUILayout.Width(80)))
        {
            LoadWholeEncrypt();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(1);
        GUILayout.Label("FackBook", EditorStyles.boldLabel);
        m_UseFacebook = GUILayout.Toggle(m_UseFacebook, "是否使用Facebook");
        if (m_UseFacebook)
        {
            SerializedProperty Property = so.FindProperty("m_FacebookConfig");
            PropertyAllFields(Property);
            so.ApplyModifiedProperties();
        }

        GUILayout.Space(1);
        GUILayout.Label("Firebase", EditorStyles.boldLabel);
        m_UseFirebase = GUILayout.Toggle(m_UseFirebase, "是否使用Firebase");
        if (m_UseFirebase)
        {
            if (GUILayout.Button("Setup", GUILayout.Width(80)))
            {
                SetFirebase();
            }
        }

        GUILayout.Space(1);
        GUILayout.Label("Adcolony", EditorStyles.boldLabel);
        m_UseAdcolony = GUILayout.Toggle(m_UseAdcolony, "是否使用Adcolony");
        if (m_UseAdcolony)
        {
            SerializedProperty Property = so.FindProperty("m_AdcolonyConfig");
            PropertyAllFields(Property);
            so.ApplyModifiedProperties();
        }

        GUILayout.Space(1);
        GUILayout.Label("Audience", EditorStyles.boldLabel);
        m_UseAudience = GUILayout.Toggle(m_UseAudience, "是否使用Audience");
        if (m_UseAudience)
        {
            SerializedProperty Property = so.FindProperty("m_AudienceConfig");
            PropertyAllFields(Property);
            so.ApplyModifiedProperties();
        }

        GUILayout.Space(1);
        GUILayout.Label("UnityAds", EditorStyles.boldLabel);
        m_UseUnityAds = GUILayout.Toggle(m_UseUnityAds, "是否使用UnityAds");
        if (m_UseUnityAds)
        {
            SerializedProperty Property = so.FindProperty("m_UnityAdsConfig");
            PropertyAllFields(Property);
            so.ApplyModifiedProperties();
        }

        GUILayout.Space(1);
        GUILayout.Label("IronSource", EditorStyles.boldLabel);
        m_UseIronSource = GUILayout.Toggle(m_UseIronSource, "是否使用IronSource");
        if (m_UseIronSource)
        {
            SerializedProperty Property = so.FindProperty("m_IronSourceConfig");
            PropertyAllFields(Property);
            so.ApplyModifiedProperties();
        }

        GUILayout.Space(1);
        GUILayout.Label("Chartboost", EditorStyles.boldLabel);
        m_UseChartboost = GUILayout.Toggle(m_UseChartboost, "是否使用Chartboost");
        if (m_UseChartboost)
        {
            SerializedProperty Property = so.FindProperty("m_ChartboostConfig");
            PropertyAllFields(Property);
            so.ApplyModifiedProperties();
        }

        GUILayout.Space(1);
        GUILayout.Label("Adjust", EditorStyles.boldLabel);
        m_UseAdjust = GUILayout.Toggle(m_UseAdjust, "是否使用Adjust");
        if (m_UseAdjust)
        {
            SerializedProperty Property = so.FindProperty("m_AdjustConfig");
            PropertyAllFields(Property);
            so.ApplyModifiedProperties();
        }

        //GUILayout.Space(1);
        //GUILayout.Label("OneSignal", EditorStyles.boldLabel);
        //m_UseOneSignal = GUILayout.Toggle(m_UseOneSignal, "是否使用OneSignal");
        //if (m_UseOneSignal)
        //{
        //    SerializedProperty Property = so.FindProperty("m_OneSignalConfig");
        //    PropertyAllFields(Property);
        //    so.ApplyModifiedProperties();
        //}

        GUILayout.Space(1);
        GUILayout.Label("AppLovin", EditorStyles.boldLabel);
        m_UseAppLovin = GUILayout.Toggle(m_UseAppLovin, "是否使用AppLovin");
        if (m_UseAppLovin)
        {
            SerializedProperty Property = so.FindProperty("m_AppLovinConfig");
            PropertyAllFields(Property);
            so.ApplyModifiedProperties();
        }

        GUILayout.Space(1);
        GUILayout.Label("MAX", EditorStyles.boldLabel);
        m_UseMAX = GUILayout.Toggle(m_UseMAX, "是否使用MAX");
        if (m_UseMAX)
        {
            SerializedProperty Property = so.FindProperty("m_MAXConfig");
            PropertyAllFields(Property);
            so.ApplyModifiedProperties();
        }
        
        GUILayout.Space(1);
        GUILayout.Label("Tapjoy", EditorStyles.boldLabel);
        m_UseTapjoy = GUILayout.Toggle(m_UseTapjoy, "是否使用Tapjoy");
        if (m_UseTapjoy)
        {
            SerializedProperty Property = so.FindProperty("m_TapjoyConfig");
            PropertyAllFields(Property);
            so.ApplyModifiedProperties();
        }
        
        GUILayout.Space(1);
        GUILayout.Label("AndroidPerformanceTuner", EditorStyles.boldLabel);
        m_UseAPT = GUILayout.Toggle(m_UseAPT, "是否使用AndroidPerformanceTuner");
        if (m_UseAPT)
        {
            SerializedProperty Property = so.FindProperty("m_APTConfig");
            PropertyAllFields(Property);
            so.ApplyModifiedProperties();
        }

        GUILayout.Label("Admob", EditorStyles.boldLabel);
        GUILayout.Label("设置admobID,目前一定要配置admob，否则打包会报错，设置admob同时设置包名 ");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Admob Android APP ID : ");
        GUILayout.EndHorizontal();

        SerializedProperty admobConfig = so.FindProperty("m_AdmobConfig");
        PropertyAllFields(admobConfig);
        so.ApplyModifiedProperties();


        if (GUILayout.Button("Create config json", GUILayout.Width(200)))
        {
            AndroidManifestXML();

            if (m_PluginInfo == null)
            {
                m_PluginInfo = new PluginConfigInfo();
            }

            if (m_UseFacebook)
            {
                SetFacebook(m_FacebookConfig.AppID, m_FacebookConfig.ClientToken);
                m_PluginInfo.m_Map[Constants.FaceBook] = m_FacebookConfig;
            }
            else
            {
                m_PluginInfo.m_Map.Remove(Constants.FaceBook);
            }

            if (m_UseFirebase)
            {
                SetFirebase();
                m_PluginInfo.m_Map[Constants.FireBase] = m_FirebaseConfig;
            }
            else
            {
                m_PluginInfo.m_Map.Remove(Constants.FireBase);
            }

            if (m_UseAdcolony)
            {
                m_PluginInfo.m_Map[Constants.AdColony] = m_AdcolonyConfig;
            }
            else
            {
                m_PluginInfo.m_Map.Remove(Constants.AdColony);
            }

            if (m_UseAudience)
            {
                m_PluginInfo.m_Map[Constants.Audience] = m_AudienceConfig;
            }
            else
            {
                m_PluginInfo.m_Map.Remove(Constants.Audience);
            }

            if (m_UseUnityAds)
            {
                m_PluginInfo.m_Map[Constants.UnityAds] = m_UnityAdsConfig;
            }
            else
            {
                m_PluginInfo.m_Map.Remove(Constants.UnityAds);
            }

            if (m_UseChartboost)
            {
                m_PluginInfo.m_Map[Constants.Chartboost] = m_ChartboostConfig;
            }
            else
            {
                m_PluginInfo.m_Map.Remove(Constants.Chartboost);
            }

            if (m_UseIronSource)
            {
                m_PluginInfo.m_Map[Constants.IronSource] = m_IronSourceConfig;
            }
            else
            {
                m_PluginInfo.m_Map.Remove(Constants.IronSource);
            }

            if (m_UseAdjust)
            {
                m_PluginInfo.m_Map[Constants.Adjust] = m_AdjustConfig;
            }
            else
            {
                m_PluginInfo.m_Map.Remove(Constants.Adjust);
            }

            //if (m_UseOneSignal)
            //{
            //    m_PluginInfo.m_Map[Constants.OneSignal] = m_OneSignalConfig;
            //}
            //else
            //{
            //    m_PluginInfo.m_Map.Remove(Constants.OneSignal);
            //}

            if (m_UseAppLovin)
            {
                SetAppLovin(m_AppLovinConfig.SDKKey);
                m_PluginInfo.m_Map[Constants.AppLovin] = m_AppLovinConfig;
            }
            else
            {
                m_PluginInfo.m_Map.Remove(Constants.AppLovin);
            }

            if (m_UseMAX)
            {
                m_PluginInfo.m_Map[Constants.MAX] = m_MAXConfig;
            }
            else
            {
                m_PluginInfo.m_Map.Remove(Constants.MAX);
            }
            
            if (m_UseTapjoy)
            {
                m_PluginInfo.m_Map[Constants.Tapjoy] = m_TapjoyConfig;
            }
            else
            {
                m_PluginInfo.m_Map.Remove(Constants.Tapjoy);
            }
            
            if (m_UseAPT)
            {
                m_PluginInfo.m_Map[Constants.APT] = m_APTConfig;
            }
            else
            {
                m_PluginInfo.m_Map.Remove(Constants.APT);
            }

            if (m_UseAdmob)
            {
                //GoogleMobileAdsSettings.Instance.IsAdMobEnabled = true;
                //GoogleMobileAdsSettings.Instance.AdMobAndroidAppId = m_AdmobConfig.AndroidAppID;
                //GoogleMobileAdsSettings.Instance.AdMobIOSAppId = m_AdmobConfig.iOSAppID;

                SetAdmob(m_AdmobConfig.AndroidAppID);
                m_PluginInfo.m_Map[Constants.Admob] = m_AdmobConfig;
            }
            else
            {
                m_PluginInfo.m_Map.Remove(Constants.Admob);
            }

            PluginsInfoManager.SaveToResourceJson(m_PluginInfo);

            DragonU3DSDK.DebugUtil.Log("Setup Complete!!");
        }
        GUILayout.EndScrollView();
    }

    #endregion

    void LoadWholeEncrypt()
    {
        m_PluginInfo = PluginsInfoManager.LoadPluginConfig();
        LoadDataFromPluginInfo();
    }

    void LoadDataFromPluginInfo()
    {
        if (m_PluginInfo != null)
        {
            if (m_PluginInfo.m_Map.ContainsKey(Constants.FaceBook))
            {
                m_UseFacebook = true;
                m_FacebookConfig = m_PluginInfo.m_Map[Constants.FaceBook] as FacebookConfigInfo;
            }

            if (m_PluginInfo.m_Map.ContainsKey(Constants.FireBase))
            {
                m_UseFirebase = true;
                m_FirebaseConfig = m_PluginInfo.m_Map[Constants.FireBase] as FirebaseConfigInfo;
            }

            if (m_PluginInfo.m_Map.ContainsKey(Constants.AdColony))
            {
                m_UseAdcolony = true;
                m_AdcolonyConfig = m_PluginInfo.m_Map[Constants.AdColony] as AdColonyConfigInfo;
            }

            if (m_PluginInfo.m_Map.ContainsKey(Constants.Audience))
            {
                m_UseAudience = true;
                m_AudienceConfig = m_PluginInfo.m_Map[Constants.Audience] as AudienceConfigInfo;
            }

            if (m_PluginInfo.m_Map.ContainsKey(Constants.Adjust))
            {
                m_UseAdjust = true;
                m_AdjustConfig = m_PluginInfo.m_Map[Constants.Adjust] as AdjustConfigInfo;
            }

            if (m_PluginInfo.m_Map.ContainsKey(Constants.Admob))
            {
                m_UseAdmob = true;
                m_AdmobConfig = m_PluginInfo.m_Map[Constants.Admob] as AdmobConfigInfo;
            }

            if (m_PluginInfo.m_Map.ContainsKey(Constants.Chartboost))
            {
                m_UseChartboost = true;
                m_ChartboostConfig = m_PluginInfo.m_Map[Constants.Chartboost] as ChartboostConfigInfo;
            }

            if (m_PluginInfo.m_Map.ContainsKey(Constants.IronSource))
            {
                m_UseIronSource = true;
                m_IronSourceConfig = m_PluginInfo.m_Map[Constants.IronSource] as IronSourceConfigInfo;
            }

            if (m_PluginInfo.m_Map.ContainsKey(Constants.UnityAds))
            {
                m_UseUnityAds = true;
                m_UnityAdsConfig = m_PluginInfo.m_Map[Constants.UnityAds] as UnityAdsConfigInfo;
            }


            if (m_PluginInfo.m_Map.ContainsKey(Constants.AppLovin))
            {
                m_UseAppLovin= true;
                m_AppLovinConfig = m_PluginInfo.m_Map[Constants.AppLovin] as AppLovinConfigInfo;
            }

            if (m_PluginInfo.m_Map.ContainsKey(Constants.MAX))
            {
                m_UseMAX = true;
                m_MAXConfig = m_PluginInfo.m_Map[Constants.MAX] as MAXConfigInfo;
            }

            if (m_PluginInfo.m_Map.ContainsKey(Constants.Tapjoy))
            {
                m_UseTapjoy = true;
                m_TapjoyConfig = m_PluginInfo.m_Map[Constants.Tapjoy] as TapjoyConfigInfo;
            }
            

            if (m_PluginInfo.m_Map.ContainsKey(Constants.APT))
            {
                m_UseAPT = true;
                m_APTConfig = m_PluginInfo.m_Map[Constants.APT] as APTPluginInfo;
            }

        }
    }

    static void CreateConfigurationController()
    {
        ConfigurationController controller = AssetDatabase.LoadAssetAtPath<ConfigurationController>("Assets/Resources/" + ConfigurationController.ConfigurationControllerPath + ".asset");
        if (controller != null)
        {
            DragonU3DSDK.DebugUtil.Log("ConfigurationController 已经存在，请注意ConfigurationController 应该只存在一个副本，位于Assets/Resources/setting/ConfigurationController.asset");
        }
        else
        {
            Automation.CreateConfigrationAsset();
        }
    }


    #region AndroidXML
    static void AndroidManifestXML()
    {
        CopyDefaultModule();
        //XMLMerge();
    }

    static void CopyDefaultModule()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath);
        FileInfo[] files = dir.GetFiles("DluginModule_AndroidManifest.xml", SearchOption.AllDirectories);
        if (files.Length > 1)
        {
            MoreThanOneFileError(files, "DluginModule_AndroidManifest.xml");
        }
        else if (files.Length == 1)
        {
            string manifestPath = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
            if (File.Exists(manifestPath))
                File.Delete(manifestPath);
            File.Copy(files[0].FullName, manifestPath);
        }
        else
        {
            DebugUtil.LogError("没有找到 DluginModule_AndroidManifest.xml 文件, 请检查是否拉取SDK module 成功");
        }

        FileInfo[] gradleFiles = dir.GetFiles("DluginModule_mainTemplate.gradle", SearchOption.AllDirectories);
        if (gradleFiles.Length > 1)
        {
            MoreThanOneFileError(gradleFiles, "DluginModule_mainTemplate.gradle");
        }
        else if (gradleFiles.Length == 1)
        {
            string manifestPath = Path.Combine(Application.dataPath, "Plugins/Android/mainTemplate.gradle");
            if (File.Exists(manifestPath))
                File.Delete(manifestPath);
            File.Copy(gradleFiles[0].FullName, manifestPath);
        }
        else
        {
            DebugUtil.LogError("没有找到 DluginModule_mainTemplate.gradle 文件, 请检查是否拉取SDK module 成功");
        }
        AssetDatabase.Refresh();
    }

    static void XMLMerge()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath);
        FileInfo[] files = dir.GetFiles("*AndroidManifest.xml", SearchOption.AllDirectories);
        List<TextAsset> txtList = new List<TextAsset>();
        TextAsset mainManifest = null;

        foreach (FileInfo file in files)
        {
            if (file.Name.EndsWith("AndroidManifest.xml"))
            {
                if (file.FullName.Contains("Assets/Plugins/Android/AndroidManifest.xml"))
                {
                    mainManifest = AssetDatabase.LoadAssetAtPath<TextAsset>(GetRelativePath(file.FullName));
                }
                else
                {
                    txtList.Add(AssetDatabase.LoadAssetAtPath<TextAsset>(GetRelativePath(file.FullName)));
                }
            }
        }


        if (mainManifest != null)
        {
            XmlDocument mainManifestXml = new XmlDocument();
            mainManifestXml.LoadXml(mainManifest.text);
            XmlNodeList nodes = mainManifestXml.ChildNodes;

            foreach (XmlNode node in nodes)
            {
                DebugUtil.Log(node.Name);
                DebugUtil.Log(node.ToString());
            }
        }
    }

    static string GetRelativePath(string path)
    {
        return path.Replace('\\', '/').Replace(Application.dataPath, "Assets");
    }

    static void MergeXml(XmlDocument destXML, XmlDocument sourceXML)
    {
        XmlNodeList nodes = destXML.ChildNodes;
        XmlNodeList list = destXML.SelectNodes("manifest/application");

        XmlNode activity = destXML.ImportNode(sourceXML.DocumentElement.LastChild, true);
        destXML.DocumentElement.AppendChild(activity);
    }
    #endregion

    #region Facebook setting
    public static void SetFacebook(string facebookID, string token)
    {
#if UNITY_STANDALONE || UNITY_STANDALONE_OSX
        //PC或Mac 平台不支持
        return;
#endif
        FacebookSettings.AppIds = new List<string> { facebookID };
        FacebookSettings.ClientTokens=new List<string> { token };
        FacebookSettings.SelectedAppIndex = 0;
        ManifestMod.GenerateManifest();
        AssetDatabase.Refresh();
    }
    #endregion

    #region Firebase setting
    private static string FirebaseAndroidOutPutPath = "Plugins/Android/res/values/google-services.xml";
    public static void SetFirebase()
    {
#if UNITY_2022_3_OR_NEWER
        FirebaseAndroidOutPutPath = "Plugins/Android/main_res.androidlib/res/values/google-services.xml";
#endif
        
#if UNITY_STANDALONE || UNITY_STANDALONE_OSX
        //PC或Mac 平台不支持Firebase，所以打包不需要设置Firebase相关的配置
        return;
#endif
        FileInfo originFile = null;
        string finalPath = Path.Combine(Application.dataPath, FirebaseAndroidOutPutPath);
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath);

        #region Set Android xml
        FileInfo[] files = dir.GetFiles("google-services.json", SearchOption.AllDirectories);
        if (files.Length > 1 || files.Length == 0)
        {
            if (files.Length > 0)
            {
                MoreThanOneFileError(files, "google-services.json");
            }
            else
            {
                DebugUtil.LogError("没有找到 google-services.json 文件");
            }
        }
        else
        {
            originFile = files[0];
            FileInfo[] cmdPath = dir.GetFiles("generate_xml_from_google_services_json.py", SearchOption.AllDirectories);
            if (files.Length > 1)
            {
                MoreThanOneFileError(files, "generate_xml_from_google_services_json.py");
            }
            else if(files.Length == 0)
            {
                DebugUtil.LogError("没有找到 generate_xml_from_google_services_json.py 文件");
            }
            string command = cmdPath[0].FullName + " -i " + files[0].FullName + " -o " + finalPath;
            ProcessCommand("python", command);
        }

        //find the trans sh
        FileInfo[] shFiles = dir.GetFiles("trans_google_services.sh", SearchOption.AllDirectories);
        if (shFiles.Length > 1 || shFiles.Length == 0)
        {
            if (shFiles.Length > 0)
            {
                MoreThanOneFileError(shFiles, "trans_google_services.sh");
            }
            else
            {
                DebugUtil.LogError("没有找到 trans_google_services.sh 文件");
            }
        }
        else
        {
#if UNITY_EDITOR_WIN
            string command = "bash " + shFiles[0].FullName + " " + originFile.FullName + " " + finalPath;
            ProcessCommand("wsl", command);
#else
            string command = shFiles[0].FullName + " " + originFile.FullName + " " + finalPath;
            ProcessCommand("/bin/sh", command);
#endif
        }
        #endregion

        #region set ios plist
        FileInfo[] plistFiles = dir.GetFiles("GoogleService-Info.plist", SearchOption.AllDirectories);
        if (plistFiles.Length > 1 || plistFiles.Length == 0)
        {
            if (plistFiles.Length > 0)
            {
                MoreThanOneFileError(plistFiles, "GoogleService-Info.plist");
            }
            else
            {
                DebugUtil.LogError("没有找到 GoogleService-Info.plist 文件");
            }
        }
        else
        {
            File.Move(plistFiles[0].FullName, Path.Combine(Application.dataPath, "Plugins/iOS/") + "GoogleService-Info.plist");
        }
        #endregion

        #region m2repository
        //AppDependencies
        FileInfo[] appdependenciesFiles = dir.GetFiles("*Dependencies.xml", SearchOption.AllDirectories);
        if (appdependenciesFiles.Length > 0)
        {
            foreach (FileInfo file in appdependenciesFiles)
            {
                if (file.FullName.Contains("FirebasePlugin"))
                {
                    XmlDocument appdependency = new XmlDocument();
                    TextAsset manifestXML = AssetDatabase.LoadAssetAtPath<TextAsset>(GetRelativePath(file.FullName));
                    appdependency.LoadXml(manifestXML.text);

                    XmlNode node = appdependency.SelectSingleNode("dependencies/androidPackages/androidPackage/repositories/repository");

                    if (node == null)
                    {
                        continue;
                    }

                    string relativePath = file.FullName.Replace(Application.dataPath, "Assets");
                    string theFinalPath = relativePath.Replace("Editor/" + file.Name, "m2repository");

                    node.InnerText = theFinalPath;
                    //node.Attributes["repository"].Value = theFinalPath;
                    DebugUtil.Log(file.Name);

                    appdependency.Save(file.FullName);
                }
            }
        }
        else
        {
            DebugUtil.LogError("Did not find any AppDenpendencies.xml, firebase plugin is not complete!!");
        }
        #endregion
    }

    public static void ProcessCommand(string command, string argument)
    {
        ProcessStartInfo start = new ProcessStartInfo(command);
        start.Arguments = argument;
        start.CreateNoWindow = false;
        start.ErrorDialog = true;
        start.UseShellExecute = true;
        if (start.UseShellExecute)
        {
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;
            start.RedirectStandardInput = false;
        }
        else
        {
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.StandardOutputEncoding = UTF8Encoding.UTF8;
            start.StandardErrorEncoding = UTF8Encoding.UTF8;
        }
        Process p = Process.Start(start);
        if (!start.UseShellExecute)
        {
            DebugUtil.Log(p.StandardOutput);
            DebugUtil.Log(p.StandardError);
        }
        p.WaitForExit();
        p.Close();

    }
    #endregion

    public static void SetAppLovin(string sdkKey)
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Plugins/Android/");
        FileInfo[] files = dir.GetFiles("AndroidManifest.xml", SearchOption.TopDirectoryOnly);
        if (files.Length > 1 || files.Length == 0)
        {
            if (files.Length > 0)
            {
                MoreThanOneFileError(files, "AndroidManifest.xml");
            }
            else
            {
                DebugUtil.LogError("AndroidManifest.xml没有找到。");
            }
        }
        else
        {
            XmlDocument manifest = new XmlDocument();
            TextAsset manifestXML = AssetDatabase.LoadAssetAtPath<TextAsset>(GetRelativePath(files[0].FullName));
            string newText = ReplacePackageName(manifestXML.text);
            manifest.LoadXml(newText);

            string xmlContent = "<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\" package=\"" + PlayerSettings.applicationIdentifier + "\">"  +
                                    "<application>" +
                                        "<meta-data android:name =\"applovin.sdk.key\" android:value=\"" + sdkKey + "\"/>" +
                                    "</application>" +
                                "</manifest>";
            XmlDocument appLovinNode = new XmlDocument();
            appLovinNode.LoadXml(xmlContent);
            XmlNode newNode = appLovinNode.DocumentElement;

            foreach (XmlNode node in appLovinNode.SelectNodes("manifest/application/meta-data"))
            {
                XmlNode importNode = manifest.ImportNode(node, true);
                manifest.SelectSingleNode("manifest/application").AppendChild(importNode);
            }

            manifest.Save(files[0].FullName);
            AssetDatabase.Refresh();
            //XmlNode node = ne
        }
    }

    #region AndroidPerformanceTuner
    public static void SetAPT(bool debug)
    {
        APTPluginInfo m_ConfigInfo = PluginsInfoManager.Instance.GetPluginConfig<APTPluginInfo>(Constants.APT);
        if (m_ConfigInfo != null)
        {
            var SetupConfig =  AssetDatabase.LoadAssetAtPath<SetupConfig>(Paths.configPath);
            if(SetupConfig == null) throw new System.Exception("can not find Android PerformanceTuner SetupConfig.asset");
            
            SetupConfig.pluginEnabled = true;
            SetupConfig.useAdvancedAnnotations = Initializer.devDescriptor.annotationEnumSizes.Count > 1;
            Initializer.projectData.apiKey = m_ConfigInfo.APIKey;
            
            var pacingProperty = typeof(PlayerSettings.Android).GetProperty("optimizedFramePacing", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            pacingProperty.SetValue(null, true);
            
            EditorUtility.SetDirty(SetupConfig);
            
            //推荐配置
#if !APT_RECOMMEND_SET_BAN
            EditorStatePrefs<InstrumentationSettingsEditor.EditorState> m_EditorStatePrefs =
                new EditorStatePrefs<InstrumentationSettingsEditor.EditorState>("instrumentation-settings", new InstrumentationSettingsEditor.EditorState()
                {
                    useAdvanced = false,
                    jsonSettings = SettingsUtil.defaultSettings.ToString()
                });
            InstrumentationSettingsEditor.EditorState m_EditorState = m_EditorStatePrefs.Get();
            Settings m_AdvancedSettings = Initializer.projectData.ResetSettingsToDefault();
            m_AdvancedSettings.AggregationStrategy.IntervalmsOrCount = debug ? (int)(0.5 * 60 * 1000) : (int)(10 * 60 * 1000);
            
            m_EditorState.jsonSettings = m_AdvancedSettings.ToString();
            m_EditorStatePrefs.Set(m_EditorState);
            
            Initializer.projectData.SetSettings(m_AdvancedSettings);
#endif
        }
        else
        {
            var SetupConfig =  AssetDatabase.LoadAssetAtPath<SetupConfig>(Paths.configPath);
            if (SetupConfig != null)
            {
                SetupConfig.pluginEnabled = false;
                EditorUtility.SetDirty(SetupConfig);
            }
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }
    #endregion
    

    #region Admob setting
    public static void SetAdmob(string admobID)
    {

        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Plugins/Android/");
        FileInfo[] files = dir.GetFiles("AndroidManifest.xml", SearchOption.TopDirectoryOnly);
        if (files.Length > 1 || files.Length == 0)
        {
            if (files.Length > 0)
            {
                MoreThanOneFileError(files, "AndroidManifest.xml");
            }
            else
            {
                DebugUtil.LogError("AndroidManifest.xml没有找到。");
            }
        }
        else
        {
            XmlDocument manifest = new XmlDocument();
            TextAsset manifestXML = AssetDatabase.LoadAssetAtPath<TextAsset>(GetRelativePath(files[0].FullName));
            string newText = ReplacePackageName(manifestXML.text);
            manifest.LoadXml(newText);

            string xmlContent = "<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\">" +
                                    "<application>" +
                                        "<meta-data android:name=\"com.google.android.gms.ads.APPLICATION_ID\" android:value=\"" + admobID + "\"/>" +
                                    "</application>" +
                                "</manifest>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            XmlNode newNode = doc.DocumentElement;

            foreach (XmlNode node in doc.SelectNodes("manifest/application/meta-data"))
            {
                XmlNode importNode = manifest.ImportNode(node, true);
                manifest.SelectSingleNode("manifest/application").AppendChild(importNode);
            }

            manifest.Save(files[0].FullName);
            //XmlNode node = ne
        }
    }
    #endregion

    static string ReplacePackageName(string mainfestText)
    {
        string newText = mainfestText.Replace(PACKAGE_NAME_STR, PlayerSettings.applicationIdentifier);
        return newText;
    }


    #region General
    static void MoreThanOneFileError(FileInfo[] files, string fileName)
    {
        DebugUtil.LogError("多个" + fileName + "被发现, 此文件副本应该只存在一个");
        foreach (FileInfo file in files)
        {
            DragonU3DSDK.DebugUtil.Log(file.FullName);
        }
    }

    #endregion
}
