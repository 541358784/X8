using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public partial class TextureProcessor : AssetPostprocessor
{
    private static TextureFormatSettings config;
    void OnPreprocessTexture()
    {
        /*
        UpdateTextureFormatSettings();
        
        if (config == null) return;

        TextureImporter textureImporter = assetImporter as TextureImporter;
        var fileName = GetFileName(textureImporter.assetPath);

        if (fileName.EndsWith(TextureFormatSettingConst.ExcludePostFix))
        {
            return;
        }

        fileName = CheckName(fileName);

        var settings = config.settings.Find(set => (
                                                       set.PreFix != TextureFormatSettingConst.NameFormatNone
                                                       && fileName.StartsWith(set.PreFix)) 
                                                   ||
               (set.PostFix != TextureFormatSettingConst.NameFormatNone
                && fileName.EndsWith(set.PostFix)));


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
            if (config == null || TextureFormatSettings.IsDirtyLock)
            {
                config =
                    AssetDatabase.LoadAssetAtPath<TextureFormatSettings>(TextureFormatSettingConst.TextureFormatSettingsPath);
                if (config == null)
                {
                    Debug.LogError("Texture format config is null");
                }
            }
        }*/
    }

    // private TextureImporterFormat GetFormat(string platForm, bool hasAlpha)
    // {
    //     if (platForm == "iPhone")
    //     {
    //         var format = TextureImporterFormat.ASTC_RGB_4x4;
    //         if (hasAlpha) format = TextureImporterFormat.ASTC_RGBA_4x4;

    //         return format;
    //     }

    //     if (platForm == "Android")
    //     {
    //         var format = TextureImporterFormat.ETC2_RGB4;
    //         if (hasAlpha) format = TextureImporterFormat.ETC2_RGBA8;

    //         return format;
    //     }

    //     return TextureImporterFormat.RGBA32;
    // }


    // [MenuItem("校验工具/Session01")]
    // static public void AutoValidate() {
    // 	//写入csv日志
    // 	StreamWriter sw = new StreamWriter("ValidateS01.csv", false, System.Text.Encoding.UTF8);
    // 	sw.WriteLine("Validate -- Session01");

    // 	string[] allAssets = AssetDatabase.GetAllAssetPaths();
    // 	foreach (string s in allAssets) {
    // 		if (s.StartsWith("Assets/")) {
    // 			Texture tex = AssetDatabase.LoadAssetAtPath(s, typeof(Texture)) as Texture;

    // 			if (tex) {
    // 				//检测纹理资源命名是否合法
    // 				if (!Regex.IsMatch(s, @"^[a-zA-Z][a-zA-Z0-9_/.]*$")) {
    // 					sw.WriteLine(string.Format("illegal texture filename,{0}", s));
    // 				}

    // 				//判断纹理尺寸是否符合四的倍数
    // 				if (((tex.width % 4) != 0) || ((tex.height % 4) != 0)) {
    // 					sw.WriteLine(string.Format("illegal texture W/H size,{0},{1},{2}", s, tex.width, tex.height));
    // 				}
    // 			}
    // 		}
    // 	}

    // 	sw.Flush();
    // 	sw.Close();
    // }
}