using System.IO;
using UnityEditor;
using UnityEngine;

namespace Decoration.Common.Editor
{
    public class TextureTools : AssetPostprocessor
    {
        private readonly string[] startsPath = new[]
        {
            "Assets/Export/Decoration/Worlds/World1/Texture/",
            "Assets/Export/Decoration/Worlds/World2/Texture/",
        };
            
        private readonly string[] filtePath = new[]
        {
            "Assets/Export/Decoration/Worlds/World1/Texture/Common",
            "Assets/Export/Decoration/Worlds/World1/Texture/Fog",
            "Assets/Export/Decoration/Worlds/World1/Texture/Glass",
            "Assets/Export/Decoration/Worlds/World1/Texture/Sand",
            "Assets/Export/Decoration/Worlds/World1/Texture/Sea",
            "Assets/Export/Decoration/Worlds/World1/Texture/SeaImage",
            "Assets/Export/Decoration/Worlds/World1/Texture/Stone",
            "Assets/Export/Decoration/Worlds/World1/Texture/Plants",
            "Assets/Export/Decoration/Worlds/World1/Texture/Loss",
            "Assets/Export/Decoration/Worlds/World1/Texture/BuildingIcons",
            "Assets/Export/Decoration/Worlds/World1/Spine/",
        };

        private bool StartsWith(string assetPath)
        {
            foreach (var s in startsPath)
            {
                if(assetPath.StartsWith(s))
                    return true;
            }

            return false;
        }
        
        private bool Contains(string assetPath)
        {
            foreach (var s in filtePath)
            {
                if(assetPath.StartsWith(s))
                    return true;
            }

            return false;
        }
        
        
        private void OnPreprocessTexture()
        {
            //return;
            TextureImporter importer = (TextureImporter) assetImporter;
            if (!StartsWith(importer.assetPath))
                return;
            
            importer.textureType = TextureImporterType.Sprite;

            if (Contains(importer.assetPath))
            {
                //importer.isReadable = false;
            }
            else
            {
                importer.isReadable = true;
            }
        
            TextureImporterPlatformSettings psAndroid = importer.GetPlatformTextureSettings("Android");
            TextureImporterPlatformSettings psIPhone = importer.GetPlatformTextureSettings("iPhone");
        
            psAndroid.overridden = true;
            psIPhone.overridden = true;
            psAndroid.format = TextureImporterFormat.ETC2_RGBA8;
            psIPhone.format = TextureImporterFormat.ASTC_6x6;
            
            importer.SetPlatformTextureSettings(psAndroid);
            importer.SetPlatformTextureSettings(psIPhone);
            
            AssetDatabase.SaveAssets();
        }
        
        private void OnPostprocessTexture(Texture2D texture)
        {
            //return;
            
            TextureImporter importer = (TextureImporter) assetImporter;

            if (!StartsWith(importer.assetPath))
                return;
            
            importer.textureType = TextureImporterType.Sprite;

            if (importer.spritePixelsPerUnit == 100 && !importer.assetPath.Contains("BuildingIcons") &&
                !importer.assetPath.Contains("Sea"))
            {
                texture = ScaleTexture.Scale(texture, importer.assetPath, 0.45f, 45);
                
                importer = TextureImporter.GetAtPath(importer.assetPath) as TextureImporter;
            }
            
            if (Contains(importer.assetPath))
                return;

            importer.isReadable = true;
            
            TextureImporterPlatformSettings psAndroid = importer.GetPlatformTextureSettings("Android");
            TextureImporterPlatformSettings psIPhone = importer.GetPlatformTextureSettings("iPhone");
            
            psAndroid.overridden = true;
            psIPhone.overridden = true;
            psAndroid.format = TextureImporterFormat.ETC2_RGBA8;
            psIPhone.format = TextureImporterFormat.ASTC_6x6;
            
            importer.SetPlatformTextureSettings(psAndroid);
            importer.SetPlatformTextureSettings(psIPhone);
            
            AssetDatabase.SaveAssets();
             
            if (!DetectionFormat(texture))
                FixTextureFormat(texture, importer.assetPath);
        }

        private static bool IsTextureHasTransparentOutline(Texture2D texture)
        {
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    if (x == 0 || x == texture.width - 1 || y == 0 || y == texture.height - 1)
                    {
                        Color color = texture.GetPixel(x, y);
                        if (color.a != 0f)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool IsFourMultiple(Texture2D texture)
        {
            return texture.width % 4 == 0 && texture.height % 4 == 0;
        }
        
        private static bool IsFourMultiple(string path)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            return IsFourMultiple(texture);
        }
    
        private static bool IsTextureHasTransparentOutline(string path)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            return IsTextureHasTransparentOutline(texture);
        }

        private static bool DetectionFormat(string path)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            return DetectionFormat(texture);
        }
        
        private static bool DetectionFormat(Texture2D texture)
        {
            if (IsFourMultiple(texture))
                return true;

            return false;
        }

        private static void FixTextureFormat(string path)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if(texture == null)
                return;

            FixTextureFormat(texture, path);
        }
        
        private static void FixTextureFormat(Texture2D texture, string path)
        {
            if(texture == null)
                return;
            
            int w = texture.width % 4;
            int h = texture.height % 4;

            w = w > 0 ? 4 - w : 0;
            h = h > 0 ? 4 - h : 0;
            
            int aw = w > 0 ? (w + 1) / 2 : 0;
            int ah = h > 0 ? (h + 1) / 2 : 0;

            int width = texture.width + w;
            int height = texture.height + h;
            Texture2D newTexture = new Texture2D(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if(x < aw || x >= width-(w-aw) || y < ah || y >= height-(h-ah))
                    {
                        newTexture.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        newTexture.SetPixel(x, y, texture.GetPixel(x - aw, y - ah));
                    }
                }
            }
            
            File.WriteAllBytes(path, newTexture.EncodeToPNG());
            AssetDatabase.SaveAssets();
        }

        [MenuItem("TextureTool/修改图片尺寸 被4整除")]
        public static void AddTransparentOutline()
        {
            foreach (var t in Selection.assetGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(t);
                if (!DetectionFormat(path))
                    FixTextureFormat(path);
            }
            
            AssetDatabase.Refresh();
        }
    }
}
