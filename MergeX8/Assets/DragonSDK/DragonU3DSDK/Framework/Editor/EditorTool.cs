using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DragonU3DSDK.Asset;
using DragonU3DSDK;

namespace DragonU3DSDK
{
    public class EditorTool
    {
        [MenuItem("Tools/SetTMPFont")]
        static void SetTMPFont()
        {
            var pathArrays = new string[][]
            {
            FilePathTools.GetFiles("Assets/Resources", ".prefab$", System.IO.SearchOption.AllDirectories),
            FilePathTools.GetFiles("Assets/Export", ".prefab$", System.IO.SearchOption.AllDirectories),
            };
            SetTMPFont(pathArrays[0], pathArrays[1]);
        }

        public static void SetTMPFont(string[] resourcesList, string[] exportList)
        {
            var fontDirs = new string[]
            {
            "Assets/Resources/Export/Fonts/En/",
            "Assets/Export/Fonts/En/",
            };

            var pathArrays = new string[][] { resourcesList, exportList };

            for (int i = 0; i < pathArrays.Length; i++)
            {
                string fontDir = fontDirs[i];
                if (!System.IO.Directory.Exists(fontDir))
                {
                    fontDir = fontDirs[0];
                }
                string[] paths = pathArrays[i];
                var defaultFont = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(fontDir + "LocaleFont_En SDF.asset");

                foreach (string path in paths)
                {
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    bool dirty = false;

                    foreach (var tmp in go.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
                    {
                        if (tmp.font != defaultFont)
                        {
                            try
                            {
                                Match match = match = Regex.Match(tmp.fontMaterial.name.Replace("(Instance)", ""), "LocaleFont_.*SDF(.*)");
                                if (match.Success)
                                {
                                    tmp.font = defaultFont;
                                    string materialName = string.Format("LocaleFont_En SDF {0}", match.Groups[1].ToString().Trim());
                                    Material material = AssetDatabase.LoadAssetAtPath<Material>(fontDir + materialName + ".mat");
                                    if (material)
                                    {
                                        tmp.fontMaterial = material;
                                    }
                                }
                                else
                                {
                                    tmp.font = defaultFont;
                                }
                            }
                            catch
                            {
                                tmp.font = defaultFont;
                                Debug.LogError(path + " (" + tmp.name + ") " + tmp.font.name);
                            }
                            dirty = true;
                        }
                    }

                    foreach (var subMeshUI in go.GetComponentsInChildren<TMPro.TMP_SubMeshUI>(true))
                    {
                        if (subMeshUI.fontAsset != defaultFont)
                        {
                            subMeshUI.fontAsset = defaultFont;
                            dirty = true;
                        }
                    }

                    if (dirty)
                    {
                        PrefabUtility.SavePrefabAsset(go);
                    }
                }
            }
        }
    }
}

public class AssetSetting : AssetPostprocessor
{
    const string PREFAB_DIRTY_LIST = "EDITOR_PREFAB_DIRTY_LIST";
    static int count = 0;

    static AssetSetting()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    private void OnPreprocessTexture()
    {
        TextureImporter importer = (TextureImporter)assetImporter;
        string[] dirs = new string[] { "Assets/Export/", "Assets/Res/", "Assets/Resources/" };
        foreach (string dir in dirs)
        {
            if (importer.assetPath.StartsWith(dir) && importer.assetPath.IndexOf("Textures/") == -1)
            {
                //EditorTool.SetDefaultFormat(importer);
                break;
            }
        }
    }

    private void OnPreprocessAsset()
    {
#if UNITY_2022_1_OR_NEWER
        if (count++ > 20)
        {
            System.GC.Collect();
            count = 0;
        }
#endif
        
#if AUTO_SET_TMP
        string assetPath = assetImporter.assetPath;
        if (assetPath.EndsWith("prefab"))
        {
            string key = assetPath + "#";
            string content = PlayerPrefs.GetString(PREFAB_DIRTY_LIST);
            if (content.IndexOf(key) == -1)
            {
                PlayerPrefs.SetString(PREFAB_DIRTY_LIST, content + key);
            }
        }
#endif

#if AUTO_DELETE_EMPTY_DIRS
        int now = (int)(Utils.GetTimeStampSeconds() / 3600);
        if (now > PlayerPrefs.GetInt("DELETE_EMPTY_DIRS_TIME"))
        {
            PlayerPrefs.SetInt("DELETE_EMPTY_DIRS_TIME", now);
            EditorTextureTool.DelEmptyFolders();
            Automation.FindAndRemove();
        }
#endif
    }

    static void OnHierarchyChanged()
    {
#if AUTO_SET_TMP
        HashSet<string> whiteSet = new HashSet<string>();
        foreach (var o in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            GameObject go = o as GameObject;
            if (PrefabUtility.GetPrefabAssetType(go) == PrefabAssetType.NotAPrefab && go.transform.parent != null && go.transform.parent.name == "Canvas (Environment)")
            {
                whiteSet.Add(go.name);
            }
        }

        HashSet<string> filter = new HashSet<string>();
        foreach (var o in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            GameObject go = o as GameObject;
            if (whiteSet.Contains(go.name) || PrefabUtility.GetPrefabInstanceStatus(go) == PrefabInstanceStatus.Connected)
            {
                filter.Add(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go));
            }
        }

        string content = PlayerPrefs.GetString(PREFAB_DIRTY_LIST);
        string[] list = content.Split(new char[] { '#' }, System.StringSplitOptions.RemoveEmptyEntries);
        List<string> resourcesList = new List<string>();
        List<string> exportList = new List<string>();
        content = "";
        foreach (string path in list)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go && !filter.Contains(path))
            {
                if (path.StartsWith("Assets/Resources/"))
                {
                    resourcesList.Add(path);
                }
                else if (path.StartsWith("Assets/Export/"))
                {
                    exportList.Add(path);
                }
            }
            else
            {
                content += path + "#";
            }
        }

        EditorTool.SetTMPFont(resourcesList.ToArray(), exportList.ToArray());
        PlayerPrefs.SetString(PREFAB_DIRTY_LIST, content);
#endif
    }
}