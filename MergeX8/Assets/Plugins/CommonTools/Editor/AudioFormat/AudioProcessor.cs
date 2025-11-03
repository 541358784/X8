using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class AudioProcessor : AssetPostprocessor
{
    void OnPreprocessAudio()
    {
        var config = AssetDatabase.LoadAssetAtPath<AudioFormatSettings>("Assets/Plugins/CommonTools/Editor/AudioFormat/AudioFormatSettings.asset");
        if (config == null)
        {
            Debug.LogError("Audio format config is null");
            return;
        }

        var importer = assetImporter as AudioImporter;
        var fileName = GetFileName(importer.assetPath);

        if (fileName.EndsWith("_cc"))
        {
            return;
        }

        var settings = config.settings.Find(set => (set.PreFix != AudioFormatSetting.NameFormatPre.none && fileName.ToLower().StartsWith(set.PreFix.ToString().ToLower())) ||
                                                   (set.PostFix != AudioFormatSetting.NameFormatPost.none && fileName.ToLower().EndsWith(set.PostFix.ToString().ToLower())));


        // var handleAmbisonic = true;
        var handleForce2Mono = true;
        var handleLoadingInBackground = true;
        // var handlePreload = true;

        if (settings == null)
        {
            // handleAmbisonic = false;
            handleForce2Mono = false;
            handleLoadingInBackground = false;
            // handlePreload = false;
            return;
        }
        
        var psAndroid = importer.GetOverrideSampleSettings("Android");
        var psIPhone = importer.GetOverrideSampleSettings("iPhone");
        var psStandalone = importer.GetOverrideSampleSettings("Standalone");


        //if (handleAmbisonic) importer.ambisonic = settings.ambisonic;
        if (handleForce2Mono) importer.forceToMono = settings.forceToMono;
        if (handleLoadingInBackground) importer.loadInBackground = settings.loadInBackground;

        psAndroid.loadType = settings.loadType;
        psIPhone.loadType = settings.loadType;
        psStandalone.loadType = settings.loadType;
        
        psAndroid.compressionFormat = settings.compressionFormat;
        psIPhone.compressionFormat = settings.compressionFormat;
        psStandalone.compressionFormat = settings.compressionFormat;
        
        psAndroid.quality = settings.quality/100;
        psIPhone.quality = settings.quality/100;
        psStandalone.quality = settings.quality/100;

        importer.SetOverrideSampleSettings("Android",psAndroid);
        importer.SetOverrideSampleSettings("iPhone",psIPhone);
        importer.SetOverrideSampleSettings("Standalone",psStandalone);
        
        //if (handlePreload) importer.preloadAudioData = settings.preloadAudioData;

    }


    private string GetFileName(string assetPath)
    {
        var fileName = assetPath.Remove(0, assetPath.LastIndexOf('/') + 1);
        var pointIndex = fileName.LastIndexOf('.');
        fileName = fileName.Remove(pointIndex, fileName.Length - pointIndex);

        return fileName;
    }
}