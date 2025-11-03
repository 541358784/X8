using System.Reflection;
using Spine.Unity;
using UnityEditor;
using UnityEngine;

public class SetDefaultMixDuration
{
    [MenuItem("Tools/Set Default Mix Duration to 0")]
    public static void SetAllSkeletonDataAssetsToZero()
    {
        // Find all SkeletonDataAsset in the project
        string[] guids = AssetDatabase.FindAssets("t:SkeletonDataAsset");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SkeletonDataAsset skeletonDataAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(path);

            if(skeletonDataAsset != null)
            {
                // Set DefaultMixDuration to 0
                skeletonDataAsset.defaultMix = 0f;
                // Mark the asset as dirty so that changes are saved
                EditorUtility.SetDirty(skeletonDataAsset);
            }
        }

        // Save changes to the assets
        AssetDatabase.SaveAssets();

        Debug.Log("All SkeletonDataAsset DefaultMixDurations have been set to 0.");
    }
}