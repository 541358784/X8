using UnityEditor;
using UnityEngine;


public class PsbBuild
{
    [MenuItem("Assets/图片压缩/Psb/Prefab", false, 0)]
    public static void BuildPrefab()
    {
        foreach (var t in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(t);
            if(!path.EndsWith(".prefab"))
                continue;
            
            string directory = System.IO.Path.GetDirectoryName(path);
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        
            if (asset == null)
                return;
            
            var prefab = PrefabUtility.InstantiatePrefab(asset) as GameObject;
            var spriteRenderer = prefab.GetComponent<SpriteRenderer>();
            BuildSprite(spriteRenderer, directory);
            
            var spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in spriteRenderers)
            {
                BuildSprite(sr, directory);
            }
            
            PrefabUtility.ApplyPrefabInstance(prefab, InteractionMode.AutomatedAction);
            
            Object.DestroyImmediate(prefab);
        }
        
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/图片压缩/Psb/Psb", false, 0)]
    public static void BuildPsb()
    {
        foreach (var t in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(t);
            if(!path.EndsWith(".psb"))
                continue;
            
            string directory = System.IO.Path.GetDirectoryName(path);
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        
            if (asset == null)
                return;
            
            var prefab = PrefabUtility.InstantiatePrefab(asset) as GameObject;
            PrefabUtility.UnpackPrefabInstance(prefab, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            var spriteRenderer = prefab.GetComponent<SpriteRenderer>();
            BuildSprite(spriteRenderer, directory);
            
            var spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in spriteRenderers)
            {
                BuildSprite(sr, directory);
            }

            PrefabUtility.CreatePrefab(directory + "/" + t.name + ".prefab", prefab);
            
            AssetDatabase.DeleteAsset(path);
            Object.DestroyImmediate(prefab);
        }
        
        AssetDatabase.Refresh();
    }
    private static void BuildSprite(SpriteRenderer sr, string path)
    {
        if(sr == null)
            return;
        
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path+"/"+sr.gameObject.name+".png");
    }
}