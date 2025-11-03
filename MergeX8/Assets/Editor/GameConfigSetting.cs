using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using BestHTTP.Extensions;
using DragonPlus;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class GameConfigSetting
{
    private const string localSpriteAtlas = "LocalSpriteAtlas";
    private const string atlasConfigPath = "Assets/Resources/Settings/AtlasConfigController.asset";
    private const string assetConfigPath = "Assets/Resources/Settings/AssetConfigController.asset";
    private const string space2 = "  ";
    private const string space4 = "    ";
    private const string space6 = "      ";
    
    [MenuItem ("Assets/资源配置/AtlasConfig", false, 0)]
    static public void GameCofig_Atlas() 
    {
        string[] filePaths = GetFilePaths();
        if(filePaths == null || filePaths.Length == 0)
            return;

        foreach (var value in AtlasConfigController.Instance.AtlasPathNodeList)
        {
            if (value.AtlasName.Equals(filePaths[0]))
            {  
                UnityEditor.EditorUtility.DisplayDialog("AtlasFile 【Error】", "图集文件中已经存在 [ " + filePaths[0] + " ] 请手动检查 \n", "确定");
                
                return;
            }
        }
        
        AtlasPathNode pathNode = new AtlasPathNode();
        pathNode.AtlasName = filePaths[0];
        pathNode.HdPath = filePaths[1];
        pathNode.SdPath = filePaths[2];
        
        AtlasConfigController.Instance.AtlasPathNodeList.Add(pathNode);
        
        EditorUtility.SetDirty(AtlasConfigController.Instance);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        UnityEditor.EditorUtility.DisplayDialog("AtlasConfig 【Success】",  filePaths[0] + "\n" + filePaths[1] + "\n" + filePaths[2], "确定");
    }
    

    [MenuItem ("Assets/资源配置/AssetConfig-Activity", false, 1)]
    static public void GameCofig_Asset_Activity() 
    {
        string[] filePaths = GetFilePaths();
        if(filePaths == null || filePaths.Length == 0)
            return;

        foreach (string path in AssetConfigController.Instance.ActivityResPaths)
        {
            if (path.Equals(filePaths[1]))
            {  
                UnityEditor.EditorUtility.DisplayDialog("Activity 【Error】", "图集文件中已经存在 [ " + filePaths[1] + " ] 请手动检查 \n", "确定");
                return;
            }
            else if (path.Equals(filePaths[2]))
            {  
                UnityEditor.EditorUtility.DisplayDialog("Activity 【Error】", "图集文件中已经存在 [ " + filePaths[2] + " ] 请手动检查 \n", "确定");
                return;
            }
        }

        List<string> tempList = new List<string>(AssetConfigController.Instance.ActivityResPaths);
        tempList.Add(SubString(filePaths[1], "Hd"));
        tempList.Add(SubString(filePaths[2], "Sd"));

        AssetConfigController.Instance.ActivityResPaths = tempList.ToArray();
        
        EditorUtility.SetDirty(AssetConfigController.Instance);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        UnityEditor.EditorUtility.DisplayDialog("Activity 【Success】",  filePaths[1] + "\n" + filePaths[2] , "确定");
    }
    
    [MenuItem ("Assets/资源配置/AssetConfig-Activity-Prefab", false, 2)]
    static public void GameCofig_Asset_Prefab_Activity() 
    {
        if (Selection.assetGUIDs == null || Selection.assetGUIDs.Length != 1)
        {
            UnityEditor.EditorUtility.DisplayDialog("GameCofig_Asset_Prefab_Activity 【Error】", "选择一个文件夹", "确定");
            return;
        }

        string selectionPath = AssetDatabase.GUIDToAssetPath (Selection.assetGUIDs[0]);
        selectionPath = selectionPath.Replace("Assets/Export/", "");
        
        List<string> tempList = new List<string>(AssetConfigController.Instance.ActivityResPaths);
        tempList.Add(selectionPath);
        
        AssetConfigController.Instance.ActivityResPaths = tempList.ToArray();
        
        EditorUtility.SetDirty(AssetConfigController.Instance);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        UnityEditor.EditorUtility.DisplayDialog("Activity 【Success】",  selectionPath, "确定");
    }
    
    [MenuItem ("Assets/资源配置/AssetConfig-LocalSpriteAtlas", false, 3)]
    static public void GameCofig_Asset_LocalSpriteAtlas() 
    {     
        string[] filePaths = GetFilePaths();
        if(filePaths == null || filePaths.Length == 0)
            return;

        BundleGroup bundleGroup = null;
        foreach (BundleGroup group in AssetConfigController.Instance.Groups)
        {
            if(!group.GroupName.Equals(localSpriteAtlas))
                continue;

            bundleGroup = group;
            break;
        }

        if (bundleGroup == null)
        {
            UnityEditor.EditorUtility.DisplayDialog("LocalSpriteAtlas 【Error】", "数组索引不存在 [ " + localSpriteAtlas + " ] 请手动检查 \n", "确定");
            return;
        }
        
        
        foreach (BundleState state in bundleGroup.Paths)
        {
            if (state.Path.Equals(filePaths[1]))
            {  
                UnityEditor.EditorUtility.DisplayDialog("LocalSpriteAtlas 【Error】", "图集文件已经存在 [ " + filePaths[1] + " ] 请手动检查 \n", "确定");
                return;
            }
            else if (state.Path.Equals(filePaths[2]))
            {  
                UnityEditor.EditorUtility.DisplayDialog("LocalSpriteAtlas 【Error】", "图集文件已经存在 [ " + filePaths[2] + " ] 请手动检查 \n", "确定");
                return;
            }
        }

        for (int i = 1; i < filePaths.Length; i++)
        {
            BundleState state = new BundleState();
            state.Path = SubString(filePaths[i], i==1 ? "Hd" : "Sd");
            
            bundleGroup.Paths.Add(state);
        }
        
        EditorUtility.SetDirty(AssetConfigController.Instance);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        UnityEditor.EditorUtility.DisplayDialog("LocalSpriteAtlas 【Success】",  filePaths[1] + "\n" + filePaths[2] , "确定");
    }

    #region MyRegion
    /// <summary>
    /// 暂时保留写文件的方式
    /// </summary>
    [MenuItem ("Assets/资源配置/WriteFile/AtlasConfig")]
    static public void GameCofig_Atlas_File() 
    {
        string[] atlasContent = GetAtlasFilePath(atlasConfigPath);
        if(atlasContent == null)
            return;
        
        atlasContent[0] = space2 + "- AtlasName: " + atlasContent[0];
        atlasContent[1] = space4 + "HdPath: " + atlasContent[1];
        atlasContent[2] = space4 + "SdPath: " + atlasContent[2];
        
        if (WriteFile(atlasConfigPath, atlasContent))
        {
            UnityEditor.EditorUtility.DisplayDialog("AtlasConfig 【Success】",  atlasContent[0] + "\n" + atlasContent[1] + "\n" + atlasContent[2], "确定");
            AssetDatabase.Refresh();
        }
        else
        {
            UnityEditor.EditorUtility.DisplayDialog("AtlasConfig 【Error】", atlasContent[0] + "\n" + atlasContent[1] + "\n" + atlasContent[2] + "\n ErrorCode: 7", "确定");
        }
    }
    
    /// <summary>
    /// 暂时保留写文件的方式
    /// </summary>
    [MenuItem ("Assets/资源配置/WriteFile/AssetConfig-Activity")]
    static public void GameCofig_Asset_Activity_File() 
    {
        string[] atlasContent = GetAtlasFilePath(assetConfigPath, "ActivityResPaths:", "ProjectPretreatmentDelegate: {fileID: 0}");
        if(atlasContent == null)
            return;
        string[] writeContent = {"", ""};
        writeContent[0] = space2 + "- " + SubString(atlasContent[1], "Hd");
        writeContent[1] = space2 + "- " + SubString(atlasContent[2], "Sd");
        
            
        if (WriteFile(assetConfigPath, writeContent, "ActivityResPaths:"))
        {
            UnityEditor.EditorUtility.DisplayDialog("AtlasConfig 【Success】",  writeContent[0] + "\n" + writeContent[1] , "确定");
            AssetDatabase.Refresh();
        }
        else
        {
            UnityEditor.EditorUtility.DisplayDialog("AtlasConfig 【Error】", writeContent[0] + "\n" + writeContent[1] + "\n ErrorCode: 7", "确定");
        }
    }
    
    /// <summary>
    /// 暂时保留写文件方式
    /// </summary>
    [MenuItem ("Assets/资源配置/WriteFile/AssetConfig-LocalSpriteAtlas")]
    static public void GameCofig_Asset_LocalSpriteAtlas_File() 
    {
        string[] atlasContent = GetAtlasFilePath(assetConfigPath, "- GroupName: LocalSpriteAtlas", "- GroupName: RemoteSpriteAtlas");
        if(atlasContent == null)
            return;

        int arrayLength = 10;
        string[] writeContent = new string[arrayLength];

        writeContent[0] = space4 + "- Path: " + SubString(atlasContent[1], "Hd");
        writeContent[1] = space6 + "InInitialPacket: 1";
        writeContent[2] = space6 + "NameOutside: ";
        writeContent[3] = space6 + "GroupOutside: ";
        writeContent[4] = space6 + "PathOutside: ";
        
        writeContent[5] = space4 + "- Path: " + SubString(atlasContent[2], "Sd");
        writeContent[6] = space6 + "InInitialPacket: 1";
        writeContent[7] = space6 + "NameOutside: ";
        writeContent[8] = space6 + "GroupOutside: ";
        writeContent[9] = space6 + "PathOutside: ";
        
        if (WriteFile(assetConfigPath, writeContent, "- GroupName: LocalSpriteAtlas", 3))
        {
            UnityEditor.EditorUtility.DisplayDialog("AtlasConfig 【Success】",  writeContent[0] + "\n" + writeContent[5] , "确定");
            AssetDatabase.Refresh();
        }
        else
        {
            UnityEditor.EditorUtility.DisplayDialog("AtlasConfig 【Error】", writeContent[0] + "\n" + writeContent[5] + "\n ErrorCode: 7", "确定");
        }
    }
    #endregion
    
    private static string[] GetAtlasFilePath(string filePath, string chunkBegin = null, string chunkEnd = null)
    {
        string[] filePaths = GetFilePaths();
        if (filePaths == null || filePaths.Length == 0)
            return null;
        
        string atlasName = filePaths[0];
        string[] fileContent = GetFileContent(filePath, chunkBegin, chunkEnd);
        if (ContainsString(fileContent, atlasName))
        {
            UnityEditor.EditorUtility.DisplayDialog("GetAtlasFilePath 【Error】", "图集文件中已经存在 [ " + atlasName + " ] 请手动检查 \n ErrorCode: 6", "确定");
            return null;
        }

        return filePaths;
    }

    private static string[] GetFilePaths()
    {
         if (Selection.assetGUIDs == null || Selection.assetGUIDs.Length != 1)
        {
            UnityEditor.EditorUtility.DisplayDialog("GetAtlasFilePath 【Error】", "选择一个图集文件夹", "确定");
            return null;
        }

        string selectionPath = AssetDatabase.GUIDToAssetPath (Selection.assetGUIDs[0]);
        int childCount = Directory.GetDirectories(selectionPath).Length;
        if (childCount != 2)
        {
            UnityEditor.EditorUtility.DisplayDialog("GetAtlasFilePath 【Error】", "图集路径选择错误，该文件夹下必须包含Hd Sd \n ErrorCode: 2", "确定");
            return null;
        }

        bool haveHd = false;
        bool haveSd = false;
        string[] hsFilePath = {"", ""};
        foreach (string fileDir in Directory.GetDirectories(selectionPath) )
        {
            if (fileDir.EndsWith("Hd"))
            {
                haveHd = true;
                hsFilePath[0] = fileDir;
            }

            if (fileDir.EndsWith("Sd"))
            {
                haveSd = true;
                hsFilePath[1] = fileDir;
            }
        }

        if (!haveHd || !haveSd)
        {
            UnityEditor.EditorUtility.DisplayDialog("GetAtlasFilePath 【Error】", "图集路径选择错误，该文件夹下必须包含Hd Sd \n ErrorCode: 3", "确定");
            return null;
        }

        string[] spPath = selectionPath.Split('/');
        if (spPath == null || spPath.Length == 0)
        {
            UnityEditor.EditorUtility.DisplayDialog("GetAtlasFilePath 【Error】", "图集路径选择错误，该文件夹下必须包含Hd Sd \n ErrorCode: 4", "确定");
            return null;
        }
        string atlasName = spPath[spPath.Length-1];
        int index = 0;
        for (int i = 0; i < hsFilePath.Length; i++)
        {
            foreach (string fileName in Directory.GetFiles(hsFilePath[i]))
            {
                if(!fileName.EndsWith(".spriteatlas"))
                    continue;

                hsFilePath[i] = fileName;
                index = i;
            }
        }

        if (index != hsFilePath.Length-1)
        { 
            UnityEditor.EditorUtility.DisplayDialog("GetAtlasFilePath 【Error】", "图集路径选择错误，该文件夹下必须包含Hd Sd \n ErrorCode: 5", "确定");
            return null;
        }

        for (int i = 0; i < hsFilePath.Length; i++)
        {
            hsFilePath[i] = hsFilePath[i].Replace("Assets/Export/", "");
            hsFilePath[i] = hsFilePath[i].Replace(".spriteatlas", "");
        }

        string[] filePaths = {"", "", ""};
        filePaths[0] = atlasName;
        filePaths[1] = hsFilePath[0];
        filePaths[2] = hsFilePath[1];

        return filePaths;
    }
    
    private static string[] GetFileContent(string filePath, string chunkBegin = null, string chunkEnd = null)
    {
        try
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
        
            byte[] _bytes = new byte[fileStream.Length];
            int count = fileStream.Read(_bytes, 0, (int)fileStream.Length);
            fileStream.Close();
            fileStream.Dispose();
        
            string encodingStr = Encoding.Default.GetString(_bytes, 0, count);
        
            string[] fileContent = encodingStr.Split('\n');
            if (chunkBegin == null || chunkBegin.IsEmptyString() || chunkBegin == null && chunkEnd == null)
                return fileContent;

            bool isBegin = false;
            bool isEnd = false;

            List<string> chunkContent = new List<string>();
            for (int i = 0; i < fileContent.Length; i++)
            {
                if(!isBegin)
                {
                    if (fileContent[i].Contains(chunkBegin))
                    {
                        isBegin = true;
                        continue;
                    }
                }
                
                if(!isBegin)
                    continue;
                
                if(fileContent[i].Contains(chunkEnd))
                    break;
                
                chunkContent.Add(fileContent[i]);
            }

            return chunkContent.ToArray();
        }
        catch (Exception e)
        {
            UnityEditor.EditorUtility.DisplayDialog("打开文件失败", filePath + "\n" + e.Message, "确定");
            throw;
        }
    }

    private static bool WriteFile(string filePath, string[] writeContent, string insertBegin = null, int offset = 0)
    {
        if (writeContent == null || writeContent.Length == 0)
            return false;
        
        try
        {
            string[] allLines = File.ReadAllLines(filePath);
            List<string> listAllLines = new List<string>(allLines);
            
            if(insertBegin == null || insertBegin.IsEmptyString())
            {
                for (int i = 0; i < writeContent.Length; i++)
                {
                    listAllLines.Add(writeContent[i]);
                }
                File.WriteAllLines(filePath, listAllLines.ToArray());

                return true;
            }
            
            int insertIndex = listAllLines.Count;
            for (int i = 0; i < listAllLines.Count; i++)
            {
                if (listAllLines[i].Contains(insertBegin))
                {
                    insertIndex = i + 1;
                    break;
                }
            }

            insertIndex += offset;
            for (int i = 0; i < writeContent.Length; i++)
                listAllLines.Insert(insertIndex+i, writeContent[i]);
            
            File.WriteAllLines(filePath, listAllLines.ToArray());

            return true;
        }
        catch (Exception e)
        {
            UnityEditor.EditorUtility.DisplayDialog("写入文件失败", filePath + "\n" + e.Message, "确定");
            throw;
        }
    }
    
    private static bool ContainsString(string[] content, string str)
    {
        for (int i = 0; i < content.Length; i++)
        {
            if (content[i].Contains(str))
                return true;
        }

        return false;
    }

    private static string SubString(string orgStr, string subStr)
    {
       int index = orgStr.IndexOf(subStr);
       if (index < 0)
           return orgStr;

       string sub = orgStr.Substring(0, index + subStr.Length);

       return sub;
    }
}
