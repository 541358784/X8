using System.IO;
using UnityEditor;
using UnityEngine;


public class ScaleTexture
{
    [MenuItem("Assets/图片压缩/适配缩放/Normal/缩放0.45", false, 0)]
    public static void Scale_45()
    {
        Scale(0.45f);
    }
    
    [MenuItem("Assets/图片压缩/适配缩放/Normal/缩放0.5", false, 0)]
    public static void Scale_50()
    {
        Scale(0.5f);
    }

    [MenuItem("Assets/图片压缩/适配缩放/Normal/缩放0.55", false, 0)]
    public static void Scale_55()
    {
        Scale(0.55f);
    }
    
    [MenuItem("Assets/图片压缩/适配缩放/Normal/缩放0.7", false, 0)]
    public static void Scale_7()
    {
        Scale(0.7f);
    }
    
    
    [MenuItem("Assets/图片压缩/适配缩放/Normal/缩放0.8", false, 0)]
    public static void Scale_8()
    {
        Scale(0.8f);
    }
    
    [MenuItem("Assets/图片压缩/适配缩放/Normal/缩放0.85", false, 0)]
    public static void Scale_85()
    {
        Scale(0.85f);
    }
    
    
    [MenuItem("Assets/图片压缩/适配缩放/Normal/缩放0.9", false, 0)]
    public static void Scale_9()
    {
        Scale(0.9f);
    }
    
    [MenuItem("Assets/图片压缩/适配缩放/Ignore/缩放0.1", false, 0)]
    public static void Adapt_Scale_1()
    {
        Scale(0.1f, false, true);
    }
    [MenuItem("Assets/图片压缩/适配缩放/Ignore/缩放0.2", false, 0)]
    public static void Adapt_Scale_2()
    {
        Scale(0.2f, false, true);
    }
    [MenuItem("Assets/图片压缩/适配缩放/Ignore/缩放0.25", false, 0)]
    public static void Adapt_Scale_25()
    {
        Scale(0.25f, false, true);
    }
    
    [MenuItem("Assets/图片压缩/适配缩放/Ignore/缩放0.3", false, 0)]
    public static void Adapt_Scale_3()
    {
        Scale(0.3f, false, true);
    }
    
    [MenuItem("Assets/图片压缩/适配缩放/Ignore/缩放0.35", false, 0)]
    public static void Adapt_Scale_35()
    {
        Scale(0.35f, false, true);
    }
    
    [MenuItem("Assets/图片压缩/适配缩放/Ignore/缩放0.4", false, 0)]
    public static void Adapt_Scale_4()
    {
        Scale(0.4f, false, true);
    }
    [MenuItem("Assets/图片压缩/适配缩放/Ignore/缩放0.42", false, 0)]
    public static void Adapt_Scale_42()
    {
        Scale(0.42f, false, true);
    }
    [MenuItem("Assets/图片压缩/适配缩放/Ignore/缩放0.43", false, 0)]
    public static void Adapt_Scale_43()
    {
        Scale(0.43f, false, true);
    }
    [MenuItem("Assets/图片压缩/适配缩放/Ignore/缩放0.45", false, 0)]
    public static void Adapt_Scale_45()
    {
        Scale(0.45f, false, true);
    }
    
    [MenuItem("Assets/图片压缩/适配缩放/Ignore/缩放0.5", false, 0)]
    public static void Adapt_Scale_50()
    {
        Scale(0.5f, false, true);
    }

    [MenuItem("Assets/图片压缩/适配缩放/Ignore/缩放0.55", false, 0)]
    public static void Adapt_Scale_55()
    {
        Scale(0.55f, false, true);
    }
    
     
 
    // [MenuItem("Assets/图片压缩/适配缩放/缩放0.8", false, 0)]
    // public static void Scale_80()
    // {
    //     Scale(0.8f);
    // }
    //
    // [MenuItem("Assets/图片压缩/适配缩放/缩放0.9", false, 0)]
    // public static void Scale_90()
    // {
    //     Scale(0.9f);
    // }
    
    [MenuItem("Assets/图片压缩/单独缩放/缩放0.8", false, 0)]
    public static void OnlyScale_80()
    {
        Scale(0.8f, false, false);
    }
    
    [MenuItem("Assets/图片压缩/单独缩放/缩放0.9", false, 0)]
    public static void OnlyScale_90()
    {
        Scale(0.9f, false, false);
    }


    [MenuItem("Assets/图片压缩/还原缩放", false, 0)]
    public static void RestoreScale()
    {
        foreach (var t in Selection.assetGUIDs)
        {
            var path = AssetDatabase.GUIDToAssetPath(t);
            RestoreScale(path);
        }
    }
    
    public static void RestoreScale(string path)
    {
        var imp1 = TextureImporter.GetAtPath(path) as TextureImporter;
        float pxelsPerUnit = imp1.spritePixelsPerUnit;
        if (imp1.spritePixelsPerUnit == 100) 
            return;
        
        imp1.isReadable = true;
        imp1.SaveAndReimport();

        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if(tex == null)
            return;

        var newTex = ScaleTextureBilinear(tex, 1+(imp1.spritePixelsPerUnit/100f));

        File.WriteAllBytes(path, newTex.EncodeToPNG());
        AssetDatabase.SaveAssets();

        var imp = TextureImporter.GetAtPath(path) as TextureImporter;

        imp.isReadable = false;
        imp.spritePixelsPerUnit = 100;
        imp.SaveAndReimport();
    }
    
    
    private static void Scale(float scale, bool checkPixelsOne = true, bool adaptPixels = true)
    {
        foreach (var t in Selection.assetGUIDs)
        {
            var path = AssetDatabase.GUIDToAssetPath(t);
            Scale(path, scale, checkPixelsOne, adaptPixels);
        }
    }

    public static void Scale(string path, float scale, bool checkPixelsOne = true, bool adaptPixels = true)
    {
        if(scale > 1f)
            return;
        
        
        var imp1 = TextureImporter.GetAtPath(path) as TextureImporter;
        float pixelsPerUnit = imp1.spritePixelsPerUnit;

        if (checkPixelsOne)
        {
            if (imp1.spritePixelsPerUnit != 100) 
                return;
        }

        var newScale = scale;
        if (adaptPixels)
        {
            if(pixelsPerUnit/100f < scale)
                return;

            newScale = scale / (pixelsPerUnit / 100f);
        }
        
        imp1.isReadable = true;
        imp1.SaveAndReimport();

        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if(tex == null)
            return;

        // if (tex.width >= 2048 || tex.height >= 2048)
        // {
        //     Debug.LogError($"图片尺寸超大，请手动操作:{path}");
        //     return;
        // }

        var newTex = ScaleTextureBilinear(tex, newScale);

        File.WriteAllBytes(path, newTex.EncodeToPNG());
        Debug.Log($"Added texture {path} transparent outline");
        AssetDatabase.SaveAssets();

        var imp = TextureImporter.GetAtPath(path) as TextureImporter;

        imp.isReadable = false;
        if (adaptPixels)
        {
            imp.spritePixelsPerUnit = scale*100;
        }
        else
        {
            imp.spritePixelsPerUnit = pixelsPerUnit;
        }
        imp.SaveAndReimport();
    }

    public static Texture2D Scale(Texture2D tex, string path, float scale, int spritePixelsPerUnit)
    {
        if(tex == null)
            return null;
        
        // if (tex.width >= 2048 || tex.height >= 2048)
        // {
        //     Debug.LogError($"图片尺寸超大，请手动操作:{path}");
        //     return;
        // }

        var newTex = ScaleTextureBilinear(tex, scale);

        File.WriteAllBytes(path, newTex.EncodeToPNG());
        Debug.Log($"Added texture {path} transparent outline");
        AssetDatabase.SaveAssets();
        
        var imp = TextureImporter.GetAtPath(path) as TextureImporter;

        imp.isReadable = false;
        imp.spritePixelsPerUnit = spritePixelsPerUnit;
        imp.SaveAndReimport();

        return newTex;
    }
    
    private static Texture2D ScaleTextureBilinear(Texture2D originalTexture, float scaleFactor)
    {
        Texture2D newTexture = new Texture2D(Mathf.CeilToInt(originalTexture.width * scaleFactor), Mathf.CeilToInt(originalTexture.height * scaleFactor));
        float scale = 1.0f / scaleFactor;

        int maxX = originalTexture.width - 1;
        int maxY = originalTexture.height - 1;
        for (int y = 0; y < newTexture.height; y++)
        {
            for (int x = 0; x < newTexture.width; x++)
            {
                // Bilinear Interpolation
                float targetX = x * scale;
                float targetY = y * scale;
                int x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                int y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                int x2 = Mathf.Min(maxX, x1 + 1);
                int y2 = Mathf.Min(maxY, y1 + 1);

                float u = targetX - x1;
                float v = targetY - y1;
                float w1 = (1 - u) * (1 - v);
                float w2 = u * (1 - v);
                float w3 = (1 - u) * v;
                float w4 = u * v;
                Color color1 = originalTexture.GetPixel(x1, y1);
                Color color2 = originalTexture.GetPixel(x2, y1);
                Color color3 = originalTexture.GetPixel(x1, y2);
                Color color4 = originalTexture.GetPixel(x2, y2);
                Color color = new Color(Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                    Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                    Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                    Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                );
                newTexture.SetPixel(x, y, color);
            }
        }

        return newTexture;
    }
}