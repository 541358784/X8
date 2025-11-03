using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ShaderTool : EditorWindow
{
    //private string materialPath = null;
    
    
    private static string AvailablePath;

    
    private List<bool> foldOut = new List<bool>();
    private Vector2 scrollViewPos;
    
    [MenuItem("Window/ShaderTool")]
    static void Init()
    {
        var window = (ShaderTool)EditorWindow.GetWindow(typeof(ShaderTool));
        window.Show();
    }

    private void OnEnable()
    {
        AvailablePath = Path.Combine(Application.dataPath, "Export");
    }


    private void OnGUI()
    {
//        if (materialPath == null)
//        {
//            materialPath = Path.Combine(Application.dataPath, "Export");
//        }
//        GUILayout.BeginHorizontal();
//        materialPath = EditorGUILayout.TextField(materialPath);
//        if (GUILayout.Button("..."))
//        {
//            materialPath = EditorUtility.OpenFolderPanel("选择材质路径", materialPath, materialPath);
//        }
//        GUILayout.EndHorizontal();
//        EditorGUILayout.Space();
        
        for (int i = 0; i < ShaderReplaceConfig.replaceShaderPair.Length; i++)
        {
            GUILayout.BeginHorizontal();
            ShaderReplaceConfig.replaceShaderPair[i][0] = EditorGUILayout.TextField("shader", ShaderReplaceConfig.replaceShaderPair[i][0]);
            ShaderReplaceConfig.replaceShaderPair[i][1] = EditorGUILayout.TextField("替换为：", ShaderReplaceConfig.replaceShaderPair[i][1]);
            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("开始替换"))
        {
            ReplaceShader();
        }

        EditorGUILayout.Space();
        
        if (GUILayout.Button("生成dummy material"))
        {
            DummyMaterialGenerator.Instance.GenDummys(AvailablePath);
        }


        scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos);

        var allMats = DummyMaterialGenerator.Instance.GetAllMaterials();
        if (allMats != null)
        {
            int index = 0;
            var e = allMats.GetEnumerator();
            while (e.MoveNext())
            {
                if (index >= foldOut.Count)
                {
                    foldOut.Add(false);
                }

                foldOut[index] = EditorGUILayout.Foldout(foldOut[index], e.Current.Key);
                if (foldOut[index])
                {
                    var mats = e.Current.Value;
                    if (mats != null)
                    {
                        for (int i = 0; i < mats.Count; i++)
                        {
                            EditorGUILayout.ObjectField(mats[i], mats[i].GetType());
                        }
                    } 
                } 
                index++;
            }     
        }
        EditorGUILayout.EndScrollView();
    }


    private void ReplaceShader()
    {
        try
        {
            EditorUtility.DisplayCancelableProgressBar("替换shader", "", 0);
            var allFiles = DummyMaterialGenerator.Instance.GetAllMaterialFiles(AvailablePath);
            if (allFiles != null)
            {
                for (int i = 0; i < allFiles.Length; i++)
                {           
                    var path = allFiles[i].FullName;
                    path = path.Substring(Application.dataPath.Length - "Assets".Length);
                    var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                    ShaderReplacer.Instance.Replace(mat);
                    EditorUtility.DisplayCancelableProgressBar("替换shader", $"{mat.name}", (float)i/allFiles.Length);
                }
                AssetDatabase.SaveAssets();
            }
            EditorUtility.ClearProgressBar();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            EditorUtility.ClearProgressBar();
        }
        
    }
  

    

    
    
    public void OnInspectorUpdate()
    {
        this.Repaint();
    }
    
    
}
