using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = System.Object;



    public class DummyMaterialGenerator
    {

        private static string dummyPathInAssets = "Export/DummyMaterial";
        private readonly string dummyPath = "Assets/" + dummyPathInAssets;
        private Dictionary<string, Material> dummyMaterials = new Dictionary<string, Material>();
        private Dictionary<string, List<Material>> allMaterials = new Dictionary<string, List<Material>>();


        public string[] GetDummyShaderAssets()
        {
            var fullPath = Path.Combine(Application.dataPath, dummyPathInAssets);
            var theFolder = new DirectoryInfo(fullPath);
            var fileInfos = theFolder.GetFiles("*.*", SearchOption.AllDirectories);

            if (fileInfos != null && fileInfos.Length > 0)
            {
                var names = new string[fileInfos.Length];
                for (int i = 0; i < fileInfos.Length; i++)
                {
                    var path = fileInfos[i].FullName;
                    path = path.Substring(Application.dataPath.Length - "Assets".Length);
                    names[i] = path;
                }

                return names;
            }

            return null;
        }
        
        public void GenDummys(string fullPath)
        {
            try
            {
                Clear();
                EditorUtility.DisplayCancelableProgressBar("生成dummy materials", "收集所有materials", 0f);
                var allFiles = GetAllMaterialFiles(fullPath);
                if (allFiles != null)
                {
                    for (int i = 0; i < allFiles.Length; i++)
                    {     
                        
                        var path = allFiles[i].FullName;
                        path = path.Substring(Application.dataPath.Length - "Assets".Length);
                        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                        GenDymmyMaterial(mat);
                        EditorUtility.DisplayCancelableProgressBar("生成dummy materials", $"处理 {mat.name}", (float)i/allFiles.Length);
                    }
                    SaveDymmyMaterial();
                }
                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                EditorUtility.ClearProgressBar();
            }
        
        }
        
        public FileInfo[] GetAllMaterialFiles(string path)
        {
            if (Directory.Exists(path))
            {
                var theFolder = new DirectoryInfo(path);
                return theFolder.GetFiles("*.mat", SearchOption.AllDirectories);
            }

            return null;
        }
        
        public string GenDummyFileKeyName(Material material)
        {
            var keywords = material.shaderKeywords;
            var name = $"{material.shader.name}#";
            
            for (int i = 0; i < keywords.Length; i++)
            {
                name += $"{keywords[i]}-";
            }

            name += ".mat";
            return name;
        }

        void GenDymmyMaterial(Material material)
        {
            if (material)
            {
                var key = GenDummyFileKeyName(material);
                Material dummyMaterial;
                if (!dummyMaterials.TryGetValue(key, out dummyMaterial))
                {
                    dummyMaterial = UnityEngine.Object.Instantiate(material);
                    dummyMaterials.Add(key, dummyMaterial);
                    Debug.Log($"{GetType()}: dummy material generated, name = {key}");
                }

                List<Material> mats;
                if (allMaterials.TryGetValue(key, out mats))
                {        
                }
                else
                {
                    mats = new List<Material>();
                    allMaterials.Add(key, mats);
                }
                mats.Add(material);
            } 
        }

        void SaveDymmyMaterial()
        {
            var dummyFullPath = Path.Combine(Application.dataPath, dummyPathInAssets);
            if (!Directory.Exists(dummyFullPath))
            {
                Directory.CreateDirectory(dummyFullPath);
            }
            var e = dummyMaterials.GetEnumerator();
            while (e.MoveNext())
            {
                var mat = e.Current.Value;
                var innerPath = Path.GetDirectoryName(e.Current.Key);
                var fullPath = Path.Combine(dummyFullPath, innerPath);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
                var path = Path.Combine(dummyPath, e.Current.Key);
                if (!File.Exists(path))
                {
                    AssetDatabase.CreateAsset(mat, path);
                    Debug.Log($"{GetType()}: asset created, path = {path}"); 
                }
                EditorUtility.DisplayCancelableProgressBar("生成dummy materials", $"生成asset {path}", 0.999f);
            }
        }

        public void Clear()
        {
            var e = dummyMaterials.GetEnumerator();
            while (e.MoveNext())
            {
                UnityEngine.GameObject.DestroyImmediate(e.Current.Value);
            }
            dummyMaterials.Clear();
            allMaterials.Clear();
        }

        public Dictionary<string, List<Material>> GetAllMaterials()
        {
            return allMaterials;
        }
        
        
        private static DummyMaterialGenerator _instance;

        public static DummyMaterialGenerator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DummyMaterialGenerator();
                }

                return _instance;
            }
        }
    }