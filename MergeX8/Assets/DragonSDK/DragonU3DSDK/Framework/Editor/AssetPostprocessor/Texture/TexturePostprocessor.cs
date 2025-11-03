using UnityEditor;
using UnityEngine;

public class TexturePostprocessor : AssetPostprocessor
{
    private static DragonU3DSDK.TextureFormatSettings config;
    void OnPreprocessTexture()
    {
        UpdateTextureFormatSettings();
    
        if (config == null) return;

        DragonU3DSDK.TextureFormatSetting settings = null;
        foreach (var set in config.settings)
        {
            foreach (var or in set.RegexsOR)
            {
                if (or.IsMatch(assetImporter.assetPath))
                {
                    settings = set;
                    break;
                }
            }

            if (settings != null)
            {
                break;
            }
        }

        TextureImporter textureImporter = assetImporter as TextureImporter;
    
        var handleSize = true;
        var handleType = true;
        var handleShape = true;
        var handlePhysicsShape = true;
        var handleAlpha = true;
        var handleFormat = true;

        if (settings == null)
        {
            handleSize = false;
            handleType = false;
            handleShape = false;
            handlePhysicsShape = false;
            handleAlpha = false;
            handleFormat = false;
            return;
        }

        //size
        var maxSize = settings.MaxSize;

        //Type
        if (handleType) textureImporter.textureType = settings.ImporterType;

        //Shape
        if (handleShape) textureImporter.textureShape = settings.ImporterShape;

        //Physics Shape
        if (handlePhysicsShape)
        {
            if (!settings.GeneratePhysicShape)
            {
                var importerSettings = new TextureImporterSettings();
                textureImporter.ReadTextureSettings(importerSettings);
                importerSettings.spriteGenerateFallbackPhysicsShape = false;

                textureImporter.SetTextureSettings(importerSettings);
            }
        }


        //Alpha
        if (handleAlpha)
        {
            if (settings.ForceAlphaSetting)
            {
                textureImporter.alphaSource = settings.Alpha;
            }
            else
            {
                if (textureImporter.DoesSourceTextureHaveAlpha())
                {
                    textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
                }
                else
                {
                    textureImporter.alphaSource = TextureImporterAlphaSource.None;
                }
            }
        }


        //Format & MaxSize
        TextureImporterPlatformSettings psAndroid = textureImporter.GetPlatformTextureSettings("Android");
        TextureImporterPlatformSettings psIPhone = textureImporter.GetPlatformTextureSettings("iPhone");
        TextureImporterPlatformSettings psStandalone = textureImporter.GetPlatformTextureSettings("Standalone");
        psAndroid.overridden = true;
        psIPhone.overridden = true;
        psStandalone.overridden = true;

        if (handleSize)
        {
            psIPhone.maxTextureSize = maxSize;
            psAndroid.maxTextureSize = maxSize;
            psStandalone.maxTextureSize = settings.StandaloneMaxSize;
        }

        if (handleFormat)
        {
            var hasAlpha = textureImporter.DoesSourceTextureHaveAlpha();
            psAndroid.format = settings.GetFormatAndroid(hasAlpha);
            psIPhone.format = settings.GetFormatIos(hasAlpha);
            psStandalone.format = settings.GetFormatStandalone(hasAlpha);
        }

        textureImporter.SetPlatformTextureSettings(psAndroid);
        textureImporter.SetPlatformTextureSettings(psIPhone);
        textureImporter.SetPlatformTextureSettings(psStandalone);


        // //设置Read/Write Enabled开关,不勾选
        // textureImporter.isReadable = false;

        //mipmap
        textureImporter.mipmapEnabled = settings.MipMap;

        void UpdateTextureFormatSettings()
        {
            if (config == null || DragonU3DSDK.TextureFormatSettings.IsDirtyLock)
            {
                var assetGuids = AssetDatabase.FindAssets("l:TextureFormatSettings");
                if (assetGuids.Length > 0)
                {
                    config =
                        AssetDatabase.LoadAssetAtPath<DragonU3DSDK.TextureFormatSettings>(AssetDatabase.GUIDToAssetPath(assetGuids[0]));
                    if (config == null)
                    {
                        Debug.LogError("Texture format config is null");
                    }
                }
            }
        }
    }
}