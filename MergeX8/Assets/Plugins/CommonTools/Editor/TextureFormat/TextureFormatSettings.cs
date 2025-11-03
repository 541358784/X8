using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "TextureFormatSettings", menuName = "ResourceImportSettings/TextureFormatSettings", order = 0)]
public class TextureFormatSettings : ScriptableObject
{
    public List<TextureFormatSetting> settings;
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
}

[System.Serializable]
public class TextureFormatSetting
{
    public string Name;
    public string PreFix;
    public string PostFix;

    public TextureImporterType ImporterType;
    public TextureImporterShape ImporterShape = TextureImporterShape.Texture2D;
    public bool GeneratePhysicShape;
    public int MaxSize;
    public int StandaloneMaxSize;
    public bool MipMap;
    public bool ForceAlphaSetting = true;
    public TextureImporterAlphaSource Alpha;
    
    

    public TextureImporterFormat FormatIos = TextureImporterFormat.ASTC_5x5;
    public TextureImporterFormat FormatAndroid = TextureImporterFormat.ASTC_5x5;
    public TextureImporterFormat FormatIosAlpha = TextureImporterFormat.ASTC_5x5;
    public TextureImporterFormat FormatAndroidAlpha = TextureImporterFormat.ASTC_5x5;
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
