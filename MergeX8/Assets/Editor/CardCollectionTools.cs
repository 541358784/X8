using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
using System.IO;
public class CardCollectionTools : EditorWindow
{
    private string folderID = "";
    private string basePath = "Assets/Resources/Atlas/"; // 基础路径

    [MenuItem("Tools/卡册生成")]
    public static void ShowWindow()
    {
        GetWindow<CardCollectionTools>("卡册生成");
    }

    void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Create Sprite Atlas Folders", EditorStyles.boldLabel);

        GUILayout.Space(10);
        folderID = EditorGUILayout.TextField("Folder ID:", folderID);

        GUILayout.Space(20);
        if (GUILayout.Button("Generate Atlas Structure", GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(folderID))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a valid Folder ID", "OK");
                return;
            }

            CreateFolderStructure();
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("This will create:\n" +
                                basePath + folderID + "/Hd\n" +
                                basePath + folderID + "/Sd\n" +
                                "With Sprite Atlas assets in each",
            MessageType.Info);
    }

    void CreateFolderStructure()
    {
        string mainFolder = Path.Combine(basePath, folderID);
        string hdFolder = Path.Combine(mainFolder, "Hd");
        string sdFolder = Path.Combine(mainFolder, "Sd");

        // 创建主文件夹
        if (!AssetDatabase.IsValidFolder(mainFolder))
        {
            Directory.CreateDirectory(mainFolder);
            AssetDatabase.Refresh();
        }

        // 创建HD文件夹
        if (!AssetDatabase.IsValidFolder(hdFolder))
        {
            AssetDatabase.CreateFolder(mainFolder, "Hd");
        }

        // 创建SD文件夹
        if (!AssetDatabase.IsValidFolder(sdFolder))
        {
            AssetDatabase.CreateFolder(mainFolder, "Sd");
        }

        // 创建Sprite Atlas资源
        CreateSpriteAtlas(hdFolder, folderID + "_HD_Atlas");
        CreateSpriteAtlas(sdFolder, folderID + "_SD_Atlas");

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", "Atlas structure created successfully!", "OK");
    }

    void CreateSpriteAtlas(string folderPath, string atlasName)
    {
        // 创建Sprite Atlas实例
        SpriteAtlas atlas = new SpriteAtlas();

        // 配置Atlas基本设置
        SpriteAtlasPackingSettings packingSettings = new SpriteAtlasPackingSettings()
        {
            padding = 8,
            enableRotation = false,
            enableTightPacking = true
        };
        atlas.SetPackingSettings(packingSettings);

        SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings()
        {
            readable = false,
            generateMipMaps = false,
            sRGB = true,
            filterMode = FilterMode.Bilinear
        };
        atlas.SetTextureSettings(textureSettings);

        // 保存Atlas资源
        string assetPath = Path.Combine(folderPath, atlasName + ".spriteatlas");
        AssetDatabase.CreateAsset(atlas, assetPath);
    }
}