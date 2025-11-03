#if UNITY_IOS
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace AppIconChanger.Editor
{
    public static class PostProcessor
    {
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS) return;

            var alternateIcons = AssetDatabase.FindAssets($"t:{nameof(AlternateIcon)}")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<AlternateIcon>(x))
				.Where(icon => icon != null && icon.platform == RuntimePlatform.IPhonePlayer)
                .ToArray();
            if (alternateIcons.Length == 0) return;

            var imagesXcassetsDirectoryPath = Path.Combine(pathToBuiltProject, "Unity-iPhone", "Images.xcassets");

            var iconNames = new List<string>();
            foreach (var alternateIcon in alternateIcons)
            {
                iconNames.Add(alternateIcon.iconName);
                var iconDirectoryPath =
 Path.Combine(imagesXcassetsDirectoryPath, $"{alternateIcon.iconName}.appiconset");
                Directory.CreateDirectory(iconDirectoryPath);

                var contentsJsonPath = Path.Combine(iconDirectoryPath, "Contents.json");
                var contentsJson = ContentsJsonText;
                contentsJson =
 contentsJson.Replace("iPhoneContents", PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPhoneOnly || PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPhoneAndiPad ? ContentsiPhoneJsonText : string.Empty);
                contentsJson =
 contentsJson.Replace("iPadContents", PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPadOnly || PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPhoneAndiPad ? ContentsiPadJsonText : string.Empty);
                File.WriteAllText(contentsJsonPath, contentsJson, Encoding.UTF8);

                if (PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPhoneOnly || PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPhoneAndiPad)
                {
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPhoneNotification40px, 40, Path.Combine(iconDirectoryPath, "iPhoneNotification40px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPhoneNotification60px, 60, Path.Combine(iconDirectoryPath, "iPhoneNotification60px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPhoneSettings58px, 58, Path.Combine(iconDirectoryPath, "iPhoneSettings58px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPhoneSettings87px, 87, Path.Combine(iconDirectoryPath, "iPhoneSettings87px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPhoneSpotlight80px, 80, Path.Combine(iconDirectoryPath, "iPhoneSpotlight80px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPhoneSpotlight120px, 120, Path.Combine(iconDirectoryPath, "iPhoneSpotlight120px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPhoneApp120px, 120, Path.Combine(iconDirectoryPath, "iPhoneApp120px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPhoneApp180px, 180, Path.Combine(iconDirectoryPath, "iPhoneApp180px.png"));
                }
                
                if (PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPadOnly || PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPhoneAndiPad)
                {
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPadNotifications20px, 20, Path.Combine(iconDirectoryPath, "iPadNotifications20px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPadNotifications40px, 40, Path.Combine(iconDirectoryPath, "iPadNotifications40px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPadSettings29px, 29, Path.Combine(iconDirectoryPath, "iPadSettings29px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPadSettings58px, 58, Path.Combine(iconDirectoryPath, "iPadSettings58px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPadSpotlight40px, 40, Path.Combine(iconDirectoryPath, "iPadSpotlight40px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPadSpotlight80px, 80, Path.Combine(iconDirectoryPath, "iPadSpotlight80px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPadApp76px, 76, Path.Combine(iconDirectoryPath, "iPadApp76px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPadApp152px, 152, Path.Combine(iconDirectoryPath, "iPadApp152px.png"));
                    SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.iPadProApp167px, 167, Path.Combine(iconDirectoryPath, "iPadProApp167px.png"));
                }
                
                SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.appStore1024px, 1024, Path.Combine(iconDirectoryPath, "appStore1024px.png"));
            }

            var pbxProjectPath = Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj", "project.pbxproj");
            var pbxProject = new PBXProject();
            pbxProject.ReadFromFile(pbxProjectPath);

            var targetGuid = pbxProject.GetUnityMainTargetGuid();
            var joinedIconNames = string.Join(" ", iconNames);
            pbxProject.SetBuildProperty(targetGuid, "ASSETCATALOG_COMPILER_ALTERNATE_APPICON_NAMES", joinedIconNames);

            pbxProject.WriteToFile(pbxProjectPath);
        }

        private static void SaveIcon(AlternateIconType type, Texture2D sourceTexture, Texture2D manualTexture, int size, string savePath)
        {
            if (type == AlternateIconType.AutoGenerate)
            {
                var iconTexture = new Texture2D(0, 0);
                iconTexture.LoadImage(File.ReadAllBytes(AssetDatabase.GetAssetPath(sourceTexture)));

                if (iconTexture.width != size || iconTexture.height != size)
                {
                    var pixels = sourceTexture.GetPixels(0, 0, sourceTexture.width, sourceTexture.height);
                    var resizedPixels = new Color[size * size];
                    for (var i = 0; i < size; i++)
                    {
                        for (var j = 0; j < size; j++)
                        {
                            resizedPixels[i * size + j] =
 pixels[Mathf.FloorToInt(i / (float)size * iconTexture.height) * iconTexture.width + Mathf.FloorToInt(j / (float)size * iconTexture.width)];
                        }
                    }
                    var newTexture = new Texture2D(size, size)
                    {
                        filterMode = FilterMode.Bilinear
                    };
                    newTexture.SetPixels(resizedPixels);
                    newTexture.Apply();
                    iconTexture = newTexture;
                }
                var pngBytes = iconTexture.EncodeToPNG();
                File.WriteAllBytes(savePath, pngBytes);
            }
            else
            {
                if (manualTexture == null) return;
                var path = AssetDatabase.GetAssetPath(manualTexture);
                File.Copy(path, savePath, true);
            }
        }

        private const string ContentsJsonText = @"{
  ""images"" : [
	iPhoneContents
	iPadContents
	{
	  ""filename"" : ""appStore1024px.png"",
	  ""idiom"" : ""ios-marketing"",
	  ""scale"" : ""1x"",
	  ""size"" : ""1024x1024""
	}
  ],
  ""info"" : {
    ""author"" : ""xcode"",
    ""version"" : 1
  }
}
";
	
		private const string ContentsiPhoneJsonText = @"{
	  ""filename"" : ""iPhoneNotification40px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""2x"",
	  ""size"" : ""20x20""
	},
	{
	  ""filename"" : ""iPhoneNotification60px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""3x"",
	  ""size"" : ""20x20""
	},
	{
	  ""filename"" : ""iPhoneSettings58px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""2x"",
	  ""size"" : ""29x29""
	},
	{
	  ""filename"" : ""iPhoneSettings87px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""3x"",
	  ""size"" : ""29x29""
	},
	{
	  ""filename"" : ""iPhoneSpotlight80px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""2x"",
	  ""size"" : ""40x40""
	},
	{
	  ""filename"" : ""iPhoneSpotlight120px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""3x"",
	  ""size"" : ""40x40""
	},
	{
	  ""filename"" : ""iPhoneApp120px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""2x"",
	  ""size"" : ""60x60""
	},
	{
	  ""filename"" : ""iPhoneApp180px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""3x"",
	  ""size"" : ""60x60""
	},
";

		private const string ContentsiPadJsonText = @"{
	  ""filename"" : ""iPadNotifications20px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""1x"",
	  ""size"" : ""20x20""
	},
	{
	  ""filename"" : ""iPadNotifications40px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""2x"",
	  ""size"" : ""20x20""
	},
	{
	  ""filename"" : ""iPadSettings29px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""1x"",
	  ""size"" : ""29x29""
	},
	{
	  ""filename"" : ""iPadSettings58px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""2x"",
	  ""size"" : ""29x29""
	},
	{
	  ""filename"" : ""iPadSpotlight40px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""1x"",
	  ""size"" : ""40x40""
	},
	{
	  ""filename"" : ""iPadSpotlight80px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""2x"",
	  ""size"" : ""40x40""
	},
	{
	  ""filename"" : ""iPadApp76px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""1x"",
	  ""size"" : ""76x76""
	},
	{
	  ""filename"" : ""iPadApp152px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""2x"",
	  ""size"" : ""76x76""
	},
	{
	  ""filename"" : ""iPadProApp167px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""2x"",
	  ""size"" : ""83.5x83.5""
	},
";
	}
}
#endif


#if UNITY_ANDROID
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace AppIconChanger.Editor
{
    public class PreProcessor : IPreprocessBuildWithReport
    {
        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android) return;

            var alternateIcons = AssetDatabase.FindAssets($"t:{nameof(AlternateIcon)}")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<AlternateIcon>(x))
                .Where(icon => icon != null && icon.platform == RuntimePlatform.Android)
                .ToArray();
            if (alternateIcons.Length == 0) return;

            var imagesXcassetsDirectoryPath =
                Path.Combine(Application.dataPath, "ThirdParty/AppIconChanger/DynamicIcon/Plugins/Android",
                    "DynamicIcon.androidlib");
            foreach (var alternateIcon in alternateIcons)
            {
                var iconDirectoryPath = Path.Combine(imagesXcassetsDirectoryPath, "res");
                string iconName = alternateIcon.iconName;

                string ldpiiconDirectoryPath = Path.Combine(iconDirectoryPath, "mipmap-ldpi");
                string mdpiiconDirectoryPath = Path.Combine(iconDirectoryPath, "mipmap-mdpi");
                string hdpiiconDirectoryPath = Path.Combine(iconDirectoryPath, "mipmap-hdpi");
                string xhdpiiconDirectoryPath = Path.Combine(iconDirectoryPath, "mipmap-xhdpi");
                string xxhdpiiconDirectoryPath = Path.Combine(iconDirectoryPath, "mipmap-xxhdpi");
                string xxxhdpiiconDirectoryPath = Path.Combine(iconDirectoryPath, "mipmap-xxxhdpi");
                Directory.CreateDirectory(ldpiiconDirectoryPath);
                Directory.CreateDirectory(mdpiiconDirectoryPath);
                Directory.CreateDirectory(hdpiiconDirectoryPath);
                Directory.CreateDirectory(xhdpiiconDirectoryPath);
                Directory.CreateDirectory(xxhdpiiconDirectoryPath);
                Directory.CreateDirectory(xxxhdpiiconDirectoryPath);

                SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.androidMipmap_ldpi36, 36,
                    Path.Combine(ldpiiconDirectoryPath, $"{iconName}.png"));
                SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.androidMipmap_mdpi48, 48,
                    Path.Combine(mdpiiconDirectoryPath, $"{iconName}.png"));
                SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.androidMipmap_hdpi72, 72,
                    Path.Combine(hdpiiconDirectoryPath, $"{iconName}.png"));
                SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.androidMipmap_xhdpi96, 96,
                    Path.Combine(xhdpiiconDirectoryPath, $"{iconName}.png"));
                SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.androidMipmap_xxhdpi144, 144,
                    Path.Combine(xxhdpiiconDirectoryPath, $"{iconName}.png"));
                SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.androidMipmap_xxxhdpi192, 192,
                    Path.Combine(xxxhdpiiconDirectoryPath, $"{iconName}.png"));
            }
        }

        private static void SaveIcon(AlternateIconType type, Texture2D sourceTexture, Texture2D manualTexture, int size,
            string savePath)
        {
            if (type == AlternateIconType.AutoGenerate)
            {
                var iconTexture = new Texture2D(0, 0);
                iconTexture.LoadImage(File.ReadAllBytes(AssetDatabase.GetAssetPath(sourceTexture)));

                if (iconTexture.width != size || iconTexture.height != size)
                {
                    var pixels = sourceTexture.GetPixels(0, 0, sourceTexture.width, sourceTexture.height);
                    var resizedPixels = new Color[size * size];
                    for (var i = 0; i < size; i++)
                    {
                        for (var j = 0; j < size; j++)
                        {
                            resizedPixels[i * size + j] =
                                pixels[
                                    Mathf.FloorToInt(i / (float)size * iconTexture.height) * iconTexture.width +
                                    Mathf.FloorToInt(j / (float)size * iconTexture.width)];
                        }
                    }

                    var newTexture = new Texture2D(size, size)
                    {
                        filterMode = FilterMode.Bilinear
                    };
                    newTexture.SetPixels(resizedPixels);
                    newTexture.Apply();
                    iconTexture = newTexture;
                }

                var pngBytes = iconTexture.EncodeToPNG();
                File.WriteAllBytes(savePath, pngBytes);
            }
            else
            {
                if (manualTexture == null) return;
                var path = AssetDatabase.GetAssetPath(manualTexture);
                File.Copy(path, savePath, true);
            }
        }

        public int callbackOrder { get; }
    }
}
#endif