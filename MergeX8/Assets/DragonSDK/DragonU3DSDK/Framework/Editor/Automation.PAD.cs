#if UNITY_ANDROID && ENABLE_ASSET_DELIVERY
using UnityEditor;
using System.IO;
using DragonU3DSDK.Asset;
using Google.Android.AppBundle.Editor;

namespace DragonU3DSDK
{
    public partial class Automation
    {
        static readonly string assetRoot = FilePathTools.NormalizeDirectory(Directory.GetCurrentDirectory());
        static readonly string tempRoot = assetRoot + "Android/_Temp/";

        static bool BuildWithAssetPack(string path, BuildOptions options)
        {
            EditorCommonUtils.DeleteDirectory(tempRoot, true);
            bool result = false;

            // 如果签名文件不在项目中，则PlayerSettings.Android.keystoreName只保留了文件名
            // 导致打包找不到签名文件，所以这里复制一份来使用
            if (!string.IsNullOrEmpty(PlayerSettings.Android.keystoreName) && ConfigurationController.Instance.AndroidKeyStorePath != PlayerSettings.Android.keystoreName)
            {
                EditorCommonUtils.CopyFile(ConfigurationController.Instance.AndroidKeyStorePath, PlayerSettings.Android.keystoreName);
            }

            try
            {
                var buildPlayerOptions = AndroidBuildHelper.CreateBuildPlayerOptions(path.Replace(".apk", ".aab"));
                buildPlayerOptions.options = options;
                AssetPackConfig assetPackConfig = new AssetPackConfig();
                AddInstallTimeAssetsFolders(assetPackConfig);

                System.Threading.Thread.Sleep(1000);
                foreach (string file in FilePathTools.GetFiles(tempRoot, "\\.meta$", SearchOption.AllDirectories))
                {
                    EditorCommonUtils.DeleteFile(file);
                }

                result = Bundletool.BuildBundle(buildPlayerOptions, assetPackConfig, true);

                if (true)
                {
                    string apk = buildPlayerOptions.locationPathName.Replace(".aab", ".apk");
                    string apks = apk + "s";
                    string error = Bundletool.BuildApks(buildPlayerOptions.locationPathName, apks, BundletoolBuildMode.Universal);

                    if (string.IsNullOrEmpty(error))
                    {
                        EditorCommonUtils.StartProcess("Assets/DragonSDK/DragonU3DSDK/.Tools/extractApk", "\"" + apks + "\"");
                        EditorCommonUtils.DeleteFile(apks);
                    }
                    else
                    {
                        DebugUtil.LogError(error);
                        result = false;
                    }

                    if (path.EndsWith(".aab"))
                    {
                        EditorCommonUtils.MoveFile(apk, apk.Replace(".apk", "_Unsinged.apk"));
                    }
                    else
                    {
                        EditorCommonUtils.DeleteFile(buildPlayerOptions.locationPathName);
                    }
                }
            }
            catch (System.Exception e)
            {
                DebugUtil.LogError(e);
            }

            if (!string.IsNullOrEmpty(PlayerSettings.Android.keystoreName) && ConfigurationController.Instance.AndroidKeyStorePath != PlayerSettings.Android.keystoreName)
            {
                EditorCommonUtils.DeleteFile(PlayerSettings.Android.keystoreName);
            }

            foreach (string file in FilePathTools.GetFiles(tempRoot, ".*", SearchOption.AllDirectories))
            {
                EditorCommonUtils.MoveFile(file, file.Replace(tempRoot, assetRoot));
            }
            EditorCommonUtils.DeleteDirectory(tempRoot, true);
            return result;
        }

        static void AddAssetsFolder(AssetPackConfig assetPackConfig, string assetPackName, string assetsFolderPath, AssetPackDeliveryMode deliveryMode, bool move = true)
        {
            string assetsFolderPathTemp = assetsFolderPath.Replace(assetRoot, tempRoot);
            if (move)
            {
                EditorCommonUtils.MoveDirectory(assetsFolderPath, assetsFolderPathTemp);
            }
            assetPackConfig.AddAssetsFolder(assetPackName, assetsFolderPathTemp, deliveryMode);
        }

        static void AddInstallTimeAssetsFolders(AssetPackConfig assetPackConfig)
        {
            AddAssetsFolder(assetPackConfig, ResourcesManager.INSTALL_TIME_ASSETPACK_NAME, FilePathTools.streamingAssetsPath_Platform, AssetPackDeliveryMode.InstallTime);
        }
    }
}
#endif