using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.DemiEditor;
using DragonU3DSDK.Asset;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;


public class AssetDatabaseMoveAsset
{
    private static string _atlasPath = "Assets/Export/Decoration/Worlds/World1/SpriteAtlas/";
    private static string _texturePath = "Assets/Export/Decoration/Worlds/World1/Texture/Copy/";
    
    [MenuItem("Assets/Build/创建包含文件夹", false, 0)]
    public static void MoveAsset()
    {
        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            if(!path.EndsWith(".prefab"))
                continue;
            
            var finderPath = path.Replace(".prefab", "");
            if(!Directory.Exists(finderPath))
                Directory.CreateDirectory(finderPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            AssetDatabase.MoveAsset (path, finderPath+"/"+obj.name+".prefab");
        }
    }


    [MenuItem("Assets/Build/写AssetConfig/Prefab/默认进包", false, 0)]
    public static void WriteAssetConfigInPack()
    {
        WritePrefabAssetConfig(true);
    }
    [MenuItem("Assets/Build/写AssetConfig/Prefab/下载", false, 0)]
    public static void WriteAssetConfigNotInPack()
    {
        WritePrefabAssetConfig(false);
    }
    
    
    [MenuItem("Assets/Build/写AssetConfig/Atlas/默认进包", false, 0)]
    public static void WriteAssetAtlasInPack()
    {
        WriteAtlasAssetConfig(true);
    }
    [MenuItem("Assets/Build/写AssetConfig/Atlas/下载", false, 0)]
    public static void WriteAssetAtlasNotInPack()
    {
        WriteAtlasAssetConfig(false);
    }
    
    // [MenuItem("Assets/Build/Atlas/创建Atlas", false, 0)]
    // public static void DynamicCreateAtlas()
    // {
    //     foreach (var obj in Selection.objects)
    //     {
    //         var path = AssetDatabase.GetAssetPath(obj);
    //         var subPath = AssetDatabase.GetSubFolders(path);
    //         if (subPath == null)
    //             continue;
    //
    //         foreach (var sub in subPath)
    //         {
    //             string[] files = Directory.GetFiles(sub, "*", SearchOption.TopDirectoryOnly);
    //             if(files == null || files.Length == 0)
    //                 continue;
    //
    //             List<string> prefabPath = new List<string>();
    //             foreach (var file in files)
    //             {
    //                 string filePath = file.Replace('\\', '/');
    //                 if (file.EndsWith(".meta")) 
    //                     continue;
    //
    //                 if (file.EndsWith(".spriteatlas"))
    //                 {
    //                     AssetDatabase.DeleteAsset(filePath);
    //                     continue;
    //                 }
    //                 
    //                 if(file.EndsWith(".prefab"))
    //                     prefabPath.Add(filePath);
    //             }
    //             
    //             if(prefabPath.Count == 0)
    //                 continue;
    //             
    //             foreach (var prePath in prefabPath)
    //             {
    //                 var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prePath);
    //
    //                 HashSet<Sprite> sprites = new HashSet<Sprite>();
    //                 var spriteRenderer = prefab.GetComponent<SpriteRenderer>();
    //                 if (spriteRenderer != null && spriteRenderer.sprite != null)
    //                     sprites.Add(spriteRenderer.sprite);
    //                 
    //                 var spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>();
    //                 foreach (var sr in spriteRenderers)
    //                 {
    //                     if (sr != null && sr.sprite != null)
    //                         sprites.Add(sr.sprite);
    //                 }
    //
    //                 if (sprites.Count > 0)
    //                 {
    //                     SpriteAtlas atlas = CreateAtlas(prefab.name);
    //                     atlas.Add(sprites.ToArray());
    //
    //                     string atlasPath = sub + "/" + atlas.name + ".spriteatlas";
    //                     AssetDatabase.CreateAsset(atlas, atlasPath);
    //                     AssetDatabase.SaveAssets();
    //                     AssetDatabase.Refresh();
    //                 }
    //             }
    //         }
    //     }
    // }

    [MenuItem("Assets/Build/节点/检测节点数量 > 30", false, 0)]
    public static void CheckChildNode30()
    {
        CheckChildNode(30);
    }

    [MenuItem("Assets/Build/节点/检测节点数量 > 20", false, 0)]
    public static void CheckChildNode20()
    {
        CheckChildNode(20);
    }
    
    
    [MenuItem("Assets/Build/节点/检测节点数量 > 50", false, 0)]
    public static void CheckChildNode50()
    {
        CheckChildNode(50);
    }

    private static void CheckChildNode(int num)
    {
        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            string[] files = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                continue;

            foreach (var file in files)
            {
                string filePath = file.Replace('\\', '/');
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
                var renderers = prefab.GetComponentsInChildren<SpriteRenderer>();
                if(renderers != null && renderers.Length >= num)
                    Debug.LogError("节点超过数量: " + prefab.name + "\t" + renderers.Length);
            }
        }
    }
    
    [MenuItem("Assets/Build/Atlas/创建Atlas", false, 0)]
    public static void DynamicCreateAtlas()
    {
        foreach (var obj in Selection.objects)
        {
            ClearDirectory(_atlasPath + obj.name);
            
            var path = AssetDatabase.GetAssetPath(obj);
            var subPath = AssetDatabase.GetSubFolders(path);
            if (subPath == null)
                continue;

            foreach (var sub in subPath)
            {
                string[] files = Directory.GetFiles(sub, "*", SearchOption.TopDirectoryOnly);
                if(files == null || files.Length == 0)
                    continue;

                List<string> prefabPath = new List<string>();
                foreach (var file in files)
                {
                    string filePath = file.Replace('\\', '/');
                    if (file.EndsWith(".meta")) 
                        continue;

                    if (file.EndsWith(".spriteatlas"))
                    {
                        AssetDatabase.DeleteAsset(filePath);
                        continue;
                    }
                    
                    if(file.EndsWith(".prefab"))
                        prefabPath.Add(filePath);
                }
                
                if(prefabPath.Count == 0)
                    continue;
                
                foreach (var prePath in prefabPath)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prePath);

                    HashSet<Sprite> sprites = new HashSet<Sprite>();
                    var spriteRenderer = prefab.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null && spriteRenderer.sprite != null)
                        sprites.Add(spriteRenderer.sprite);
                    
                    var spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var sr in spriteRenderers)
                    {
                        if (sr != null && sr.sprite != null)
                        {
                            if(!IsCommonAtlas(sr.sprite))
                                sprites.Add(sr.sprite);
                        }
                    }

                    if (sprites.Count > 1)
                    {
                        SpriteAtlas atlas = CreateAtlas(prefab.name);
                        atlas.Add(sprites.ToArray());

                        var spStr = prePath.Split('/');
                        string atlasPath = _atlasPath + obj.name + "/"  + spStr[spStr.Length-2];
                        if(!Directory.Exists(atlasPath))
                            Directory.CreateDirectory(atlasPath);
                        
                        AssetDatabase.CreateAsset(atlas, atlasPath + "/" + atlas.name  + ".spriteatlas");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
        }
    }

    
    [MenuItem("Assets/Build/Atlas/资源重构", false, 0)]
    public static void DynamicIsolateTexture()
    {
        foreach (var obj in Selection.objects)
        {
            ClearDirectory(_texturePath + obj.name);
            
            var path = AssetDatabase.GetAssetPath(obj);
            var subPath = AssetDatabase.GetSubFolders(path);
            if (subPath == null || subPath.Length == 0)
            {
                subPath = new[] { path };
            }

            foreach (var sub in subPath)
            {
                string[] files = Directory.GetFiles(sub, "*.prefab", SearchOption.TopDirectoryOnly);
                if(files == null || files.Length == 0)
                    continue;
                
                foreach (var prePath in files)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prePath);

                    HashSet<SpriteRenderer> sprites = new HashSet<SpriteRenderer>();
                    var spriteRenderer = prefab.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null && spriteRenderer.sprite != null)
                        sprites.Add(spriteRenderer);
                    
                    var spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var sr in spriteRenderers)
                    {
                        if (sr != null && sr.sprite != null)
                        {
                            if(!IsCommonAtlas(sr.sprite))
                                sprites.Add(sr);
                        }
                    }

                    foreach (var sprite in sprites)
                    {
                        string texturePath = AssetDatabase.GetAssetPath(sprite.sprite.texture);
                        AssetDatabase.CopyAsset(texturePath, _texturePath + obj.name + "/" + Path.GetFileName(texturePath));
                    }
                    AssetDatabase.Refresh();
                    
                    foreach (var sprite in sprites)
                    {
                        string texturePath = AssetDatabase.GetAssetPath(sprite.sprite.texture);
                        sprite.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(_texturePath + obj.name + "/" + Path.GetFileName(texturePath));
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }
    }
    private static bool IsCommonAtlas(Sprite sprite)
    {
        if (sprite == null || sprite.texture == null)
            return false;
        
        string path = AssetDatabase.GetAssetPath(sprite.texture);
        if (path.Contains("Texture/Common") || path.Contains("Texture/Glass") || path.Contains("Texture/Sand") ||
            path.Contains("Texture/Plants") || path.Contains("Texture/Stone") || path.Contains("Texture/Fog") || path.Contains("Texture/Loss"))
            return true;

        return false;
    }
    private static void ClearDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path);
            string[] directories = Directory.GetDirectories(path);

            // Delete files
            foreach (string file in files)
            {
                File.Delete(file);
            }

            // Clear subdirectories
            foreach (string directory in directories)
            {
                ClearDirectory(directory);
            }

            // Remove directory meta files
            string metaFile = path + ".meta";
            if (File.Exists(metaFile))
            {
                File.Delete(metaFile);
            }
            // Refresh asset database
            AssetDatabase.Refresh();
        }
        else
        {
            Directory.CreateDirectory(path);
        }
    }

    
    
    [MenuItem("Assets/Build/Atlas/AtlasFormat", false, 0)]
    public static void DynamicAtlasFormat()
    {
        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            string[] files = Directory.GetFiles(path, "*spriteatlas", SearchOption.AllDirectories);
            if(files == null)
                continue;
            
            foreach (var file in files)
            {
                string filePath = file.Replace('\\', '/');
                SetPlatformSettings(filePath);
            }
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }
    
    private static  SpriteAtlas CreateAtlas(string name)
    {
        SpriteAtlasPackingSettings setting = new SpriteAtlasPackingSettings();
        setting.padding = 2;
        setting.enableRotation = true;
        setting.enableTightPacking = true;
        
        SpriteAtlas atlas = new SpriteAtlas();
        atlas.name = name;
        atlas.SetPackingSettings(setting);

        
        return atlas;
    }

    private static void SetPlatformSettings(string path)
    {
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        if (textureImporter == null)
            return;
            
        TextureImporterPlatformSettings psAndroid = textureImporter.GetPlatformTextureSettings("Android");
        TextureImporterPlatformSettings psIPhone = textureImporter.GetPlatformTextureSettings("iPhone");
        
        psAndroid.overridden = true;
        psIPhone.overridden = true;
        psAndroid.format = TextureImporterFormat.ETC2_RGBA8;
        psIPhone.format = TextureImporterFormat.ASTC_6x6;
            
        textureImporter.SetPlatformTextureSettings(psAndroid);
        textureImporter.SetPlatformTextureSettings(psIPhone);
        
        textureImporter.SaveAndReimport();
        AssetDatabase.WriteImportSettingsIfDirty(path);
    }
    
    public static void WritePrefabAssetConfig(bool inPack)
    {
        BundleGroup bundleGroup = null;
        for (int i = 0; i < AssetConfigController.Instance.Groups.Length; i++)
        {
            if(!AssetConfigController.Instance.Groups[i].GroupName.Equals("Building"))
                continue;
        
            bundleGroup = AssetConfigController.Instance.Groups[i];
            break;
        }
        
        if (bundleGroup == null)
        {
            Array.Resize(ref AssetConfigController.Instance.Groups, AssetConfigController.Instance.Groups.Length + 1);
            AssetConfigController.Instance.Groups[AssetConfigController.Instance.Groups.Length - 1] = new BundleGroup();
        
            bundleGroup = AssetConfigController.Instance.Groups[AssetConfigController.Instance.Groups.Length - 1];
            bundleGroup.GroupName = "Building";
            bundleGroup.Version = "v1_0_0";
        }

        if(bundleGroup.Paths == null)
            bundleGroup.Paths = new List<BundleState>();

        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var sbPath = path.Replace("Assets/Export/", "");

            for (int i = 0; i < bundleGroup.Paths.Count; i++)
            {
                if(!bundleGroup.Paths[i].Path.Contains(sbPath))
                    continue;
                
                bundleGroup.Paths.RemoveAt(i);
                i--;
            }
        }

        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var subPath = AssetDatabase.GetSubFolders(path);
            if(subPath == null)
                continue;

            foreach (var sub in subPath)
            {
                var sbPath = sub.Replace("Assets/Export/", "");
                var bundleState = bundleGroup.Paths.Find(a => a.Path.Equals(sbPath));
                if (bundleState != null)
                {
                    bundleState.InInitialPacket = inPack;
                    continue;
                }
                
                bundleGroup.Paths.Add(new BundleState());
                bundleGroup.Paths[bundleGroup.Paths.Count - 1].Path = sbPath;
                bundleGroup.Paths[bundleGroup.Paths.Count - 1].InInitialPacket = inPack;
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        UnityEditor.EditorUtility.SetDirty(AssetConfigController.Instance);
    }
    
    
    public static void WriteAtlasAssetConfig(bool inPack)
    {
        BundleGroup bundleGroup = null;
        for (int i = 0; i < AssetConfigController.Instance.Groups.Length; i++)
        {
            if(!AssetConfigController.Instance.Groups[i].GroupName.Equals("BuildingAtlas"))
                continue;
        
            bundleGroup = AssetConfigController.Instance.Groups[i];
            break;
        }
        
        if (bundleGroup == null)
        {
            Array.Resize(ref AssetConfigController.Instance.Groups, AssetConfigController.Instance.Groups.Length + 1);
            AssetConfigController.Instance.Groups[AssetConfigController.Instance.Groups.Length - 1] = new BundleGroup();
        
            bundleGroup = AssetConfigController.Instance.Groups[AssetConfigController.Instance.Groups.Length - 1];
            bundleGroup.GroupName = "BuildingAtlas";
            bundleGroup.Version = "v1_0_0";
        }

        if(bundleGroup.Paths == null)
            bundleGroup.Paths = new List<BundleState>();
            
        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var subPath = AssetDatabase.GetSubFolders(path);
            if(subPath == null)
                continue;

            foreach (var sub in subPath)
            {
                var sbPath = sub.Replace("Assets/Export/", "");
                var bundleState = bundleGroup.Paths.Find(a => a.Path.Equals(sbPath));

                WriteAtlasAssetConfig(sbPath);
                
                if (bundleState != null)
                {
                    bundleState.InInitialPacket = inPack;
                    continue;
                }
                
                bundleGroup.Paths.Add(new BundleState());
                bundleGroup.Paths[bundleGroup.Paths.Count - 1].Path = sbPath;
                bundleGroup.Paths[bundleGroup.Paths.Count - 1].InInitialPacket = inPack;
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        UnityEditor.EditorUtility.SetDirty(AssetConfigController.Instance);
        UnityEditor.EditorUtility.SetDirty(AtlasConfigController.Instance);
    }

    public static void WriteAtlasAssetConfig(string path)
    {
        var spPath = path.Split('/');
        if(spPath == null || spPath.Length == 0)
            return;
        
        var atlasName = spPath[spPath.Length - 1];
        var atlasData = AtlasConfigController.Instance.AtlasPathNodeList.Find(a => a.AtlasName == atlasName);
        if (atlasData == null)
        {
            atlasData = new AtlasPathNode();
            AtlasConfigController.Instance.AtlasPathNodeList.Add(atlasData);
        }

        atlasData.AtlasName = atlasName;
        atlasData.HdPath = path + "/" + atlasName;
        atlasData.SdPath = path + "/" + atlasName;
    }
}