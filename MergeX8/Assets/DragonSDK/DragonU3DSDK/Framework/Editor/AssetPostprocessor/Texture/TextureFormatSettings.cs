using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DragonU3DSDK
{
    [System.Serializable]
    public class TextureFormatSettings : ScriptableObject
    {
        public List<TextureFormatSetting> settings = new List<TextureFormatSetting>();
        public static bool IsDirtyLock
        {
            set => _isDirtyLock = value;
            get
            {
                var r = _isDirtyLock;
                _isDirtyLock = false;
                return r;
            }
        }

        private static bool _isDirtyLock = true;

        [MenuItem("Assets/ResourceImportSettings/Create/TextureFormatSettings")]
        public static void CreateAssetFile()
        {
            TextureFormatSettings ac = ScriptableObject.CreateInstance<TextureFormatSettings>();
            AssetDatabase.CreateAsset(ac, "Assets/TextureFormatSettings.asset");
            AssetDatabase.SetLabels(ac, new []{"TextureFormatSettings"});
            AssetDatabase.SaveAssets();
        }
    }

    [System.Serializable]
    public class RegexsAnd
    {
        public List<string> Regex = new List<string>();
        public bool InspectorFoldOut = true;

        public bool IsMatch(string input)
        {
            foreach (var p in Regex)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(input, p, RegexOptions.IgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }

    [System.Serializable]
    public class TextureFormatSetting
    {
        public string Name;

        public List<RegexsAnd> RegexsOR = new List<RegexsAnd>();

        public TextureImporterType ImporterType;
        public TextureImporterShape ImporterShape = TextureImporterShape.Texture2D;
        public bool GeneratePhysicShape;
        public int MaxSize;
        public int StandaloneMaxSize;
        public bool MipMap;
        public bool ForceAlphaSetting = true;
        public TextureImporterAlphaSource Alpha;

        public TextureImporterFormat FormatIos = TextureImporterFormat.RGB24;
        public TextureImporterFormat FormatIosAlpha = TextureImporterFormat.RGBA32;
        public TextureImporterFormat FormatAndroid = TextureImporterFormat.RGB24;
        public TextureImporterFormat FormatAndroidAlpha = TextureImporterFormat.RGBA32;
        public TextureImporterFormat FormatStandalone = TextureImporterFormat.RGB24;
        public TextureImporterFormat FormatStandaloneAlpha = TextureImporterFormat.RGBA32;

        public bool InspectorFoldOut = false;

        public TextureImporterFormat GetFormatIos(bool hasAlpha)
        {
            return hasAlpha ? FormatIosAlpha : FormatIos;
        }

        public TextureImporterFormat GetFormatAndroid(bool hasAlpha)
        {
            return hasAlpha ? FormatAndroidAlpha : FormatAndroid;
        }
    
        public TextureImporterFormat GetFormatStandalone(bool hasAlpha)
        {
            return hasAlpha ? FormatStandaloneAlpha : FormatStandalone;
        }
    }
}