/*-------------------------------------------------------------------------------------------
// Copyright (C) 2019 北京，天龙互娱
//
// 模块名：EditorTextureTool
// 创建日期：2020-4-9
// 创建者：guomeng.lu
// 模块描述：编辑器图片处理
//-------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using DragonU3DSDK.Asset;

namespace DragonU3DSDK
{
    public class EditorTextureTool
    {
#if UNITY_2019_1_OR_NEWER
        const TextureImporterFormat IOS_NEW_RGBA = TextureImporterFormat.ASTC_4x4;
        const TextureImporterFormat IOS_NEW_RGB = TextureImporterFormat.ASTC_4x4;
#else
        const TextureImporterFormat IOS_NEW_RGBA = TextureImporterFormat.ASTC_RGBA_4x4;
        const TextureImporterFormat IOS_NEW_RGB = TextureImporterFormat.ASTC_RGB_4x4;
#endif

        const int MAX_TEXTURE_SIZE = 2048;
        const int COMPRESSION_QUALITY = 100;
        const int ATLAS_TYPE_NUMBER = 2;
        const string LOW_ATLAS_SUFFIX = ".SD.spriteatlas";
        static string[] PLATFORMS = { "Default", "Standalone", "Android", "iPhone" };

        static Dictionary<string, List<string>> GetReferences()
        {
            var referencesDict = new Dictionary<string, List<string>>();
            foreach (var prefab in EditorResourcePaths.GetAllPrefabFiles())
            {
                foreach (string dependency in AssetDatabase.GetDependencies(prefab, false))
                {
                    List<string> list;
                    if (!referencesDict.TryGetValue(dependency, out list))
                    {
                        list = new List<string>();
                        referencesDict.Add(dependency, list);
                    }
                    list.Add(prefab);
                }
            }
            return referencesDict;
        }

        [MenuItem("TextureTool/FindReferences (Select)")]
        static void FindReferencesBySelect()
        {
            var dependenciesDict = new Dictionary<string, HashSet<string>>();
            foreach (var prefab in EditorResourcePaths.GetAllPrefabFiles())
            {
                dependenciesDict[prefab] = new HashSet<string>(AssetDatabase.GetDependencies(prefab, false));
            }

            HashSet<string> fileSet = new HashSet<string>(EditorResourcePaths.GetFileSelections());
            foreach (string file in fileSet)
            {
                foreach (string prefab in dependenciesDict.Keys)
                {
                    if (!fileSet.Contains(prefab) && dependenciesDict[prefab].Contains(file))
                    {
                        DebugUtil.LogWarning(file + " Referenced By " + prefab);
                    }
                }
            }
        }

        [MenuItem("TextureTool/FindExternalDependencies (Select)")]
        static void FindExternalDependenciesBySelect()
        {
            List<string> selections = EditorResourcePaths.GetAllSelections(SelectionMode.DeepAssets);
            Dictionary<string, List<string>> referenceDict = new Dictionary<string, List<string>>();
            EditorResourcePaths.GetExternalDependencies(selections, selections, null, referenceDict);
            foreach (string file in referenceDict.Keys)
            {
                Debug.LogWarning("Depend On " + file);
                foreach (string reference in referenceDict[file])
                {
                    Debug.Log(" In " + reference);
                }
            }
        }

        [MenuItem("TextureTool/FindAtlas (Select)")]
        static void FindAtlasBySelect()
        {
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            foreach (string atlasPath in EditorResourcePaths.GetAllAtlasFiles())
            {
                if (IsLowSpriteatlas(atlasPath))
                {
                    continue;
                }

                SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
                foreach (Object o in SpriteAtlasExtensions.GetPackables(spriteAtlas))
                {
                    string file = AssetDatabase.GetAssetPath(o);
                    List<string> list;
                    if (!dict.TryGetValue(file, out list))
                    {
                        list = new List<string>();
                        dict.Add(file, list);
                    }
                    list.Add(spriteAtlas.name + ".spriteatlas");
                }
            }

            List<string> files = EditorResourcePaths.GetAllSelections(SelectionMode.DeepAssets);
            files.Sort();
            foreach (string file in files)
            {
                string path = file;
                do
                {
                    path = System.IO.Path.GetDirectoryName(path);
                    if (dict.ContainsKey(path))
                    {
                        break;
                    }
                } while (!string.IsNullOrEmpty(path));
                bool existed = !string.IsNullOrEmpty(path);

                List<string> list;
                if (dict.TryGetValue(file, out list))
                {
                    string content = file + " In " + EditorCommonUtils.ToString(list.ToArray());
                    if (list.Count > 1 || existed)
                    {
                        DebugUtil.LogError(content);
                    }
                    else
                    {
                        DebugUtil.LogWarning(content);
                    }
                }
                else if (!existed)
                {
                    if (AssetDatabase.LoadAssetAtPath<Sprite>(file))
                    {
                        DebugUtil.LogError(file + " Not In Atlas");
                    }
                }
            }
        }

        [MenuItem("TextureTool/CheckPrefab (Select)")]
        static void CheckPrefabBySelect()
        {
            var atlasSprites = EditorResourcePaths.GetAllSpriteFilesInAtlas();
            foreach (string file in EditorResourcePaths.GetFileSelections())
            {
                if (file.EndsWith(".prefab"))
                {
                    var sprites = EditorResourcePaths.GetDependencies(new string[] { file }, EditorResourcePaths.PNG_REGEX);
                    foreach (var sprite in sprites)
                    {
                        if (!atlasSprites.Contains(sprite))
                        {
                            DebugUtil.LogWarning(file + " 包含散图 " + sprite);
                        }
                    }
                }
            }
        }

        [MenuItem("TextureTool/CheckAlpha (Select)")]
        static void CheckAlphaBySelect()
        {
            foreach (string file in EditorResourcePaths.GetTextureFiles(EditorResourcePaths.GetFileSelections()))
            {
                Texture2D texture = EditorCommonUtils.ReadTexture(file);
                if (ContainAlpha(texture))
                {
                    DebugUtil.LogWarning("该图片包含透明通道: " + file);
                }
            }
        }

        [MenuItem("TextureTool/FilterAlpha (Select)")]
        static void FilterAlphaBySelect()
        {
            foreach (string file in EditorResourcePaths.GetTextureFiles(EditorResourcePaths.GetFileSelections()))
            {
                FilterTextureAlpha(file);
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("TextureTool/ToBytes (Select)")]
        static void ToBytesBySelect()
        {
            foreach (string file in EditorResourcePaths.GetFileSelections())
            {
                string newFile = file.Replace(EditorCommonUtils.GetExtension(file), ".bytes");
                EditorCommonUtils.MoveFile(file, newFile);
            }
        }

        [MenuItem("TextureTool/ShowAtlas (Select) &s")]
        static void ShowAtlasBySelect()
        {
            Object[] selections = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            System.Array.Sort(selections, (a, b) => { return string.Compare(a.name, b.name); });
            foreach (Object selection in selections)
            {
                if (selection.GetType() == typeof(SpriteAtlas))
                {
                    Selection.activeObject = selection;
                    break;
                }
            }
        }

        [MenuItem("TextureTool/ClearTextureSettings (Select)")]
        static void ClearTextureSettings()
        {
            HandleTextureFormat(EditorResourcePaths.GetTextureFiles(EditorResourcePaths.GetFileSelections()), (importer) =>
            {
                importer.isReadable = false;
                importer.mipmapEnabled = false;
                importer.streamingMipmaps = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                for (int i = 1; i < PLATFORMS.Length; i++)
                {
                    var platformSettings = importer.GetPlatformTextureSettings(PLATFORMS[i]);
                    platformSettings.overridden = false;
                    platformSettings.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.SetPlatformTextureSettings(platformSettings);
                }
            });
        }

        [MenuItem("TextureTool/DelEmptyFolders")]
        public static void DelEmptyFolders()
        {
            string[] dirs = FilePathTools.GetDirectories("Assets", ".*", System.IO.SearchOption.AllDirectories);
            System.Array.Sort(dirs);
            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i];
                string nextDir = i < dirs.Length - 1 ? dirs[i + 1] : "";
                if (nextDir.StartsWith(dir) || !System.IO.Directory.Exists(dir))
                {
                    continue;
                }

                string path = dir;
                string emptyPath = "";
                while (FilePathTools.GetFiles(path, EditorResourcePaths.FILTER_META_REGEX, System.IO.SearchOption.AllDirectories).Length == 0)
                {
                    emptyPath = path;
                    path = System.IO.Path.GetDirectoryName(path);
                }

                if (!string.IsNullOrEmpty(emptyPath) && System.IO.Directory.Exists(emptyPath))
                {
                    System.IO.Directory.Delete(emptyPath, true);
                    System.IO.File.Delete(emptyPath + ".meta");
                }
            }
        }

        [MenuItem("TextureTool/CheckMaterial")]
        static void CheckMaterial()
        {
            var referencesDict = GetReferences();
            string[] files = FilePathTools.GetFiles("Assets", ".*\\.mat$", System.IO.SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string[] dependencies = AssetDatabase.GetDependencies(new string[] { file });
                foreach (string dependency in dependencies)
                {
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(dependency);
                    if (sprite)
                    {
                        var references = referencesDict[dependency];
                        string info = "";
                        foreach (var reference in references)
                        {
                            if (reference.EndsWith(".spriteatlas"))
                            {
                                info += "\n" + reference;
                            }
                        }

                        if (!string.IsNullOrEmpty(info))
                        {
                            DebugUtil.LogWarning(dependency + "  Referenced By " + file + info);
                        }
                    }
                }
            }
        }

        [MenuItem("TextureTool/CheckExternal")]
        static void CheckExternal()
        {
            string rootDir = System.IO.Directory.GetCurrentDirectory().Replace('\\', '/');
            Dictionary<string, string> md5Dict = new Dictionary<string, string>();
            string[] all = FilePathTools.GetFiles("Assets", EditorResourcePaths.FILTER_META_REGEX, System.IO.SearchOption.AllDirectories);
            string[] others = FilePathTools.GetFiles(rootDir + "/_External", EditorResourcePaths.FILTER_META_REGEX, System.IO.SearchOption.AllDirectories);
            DebugUtil.Log("All " + all.Length + " External " + others.Length);

            foreach (string other in others)
            {
                string md5 = EditorCommonUtils.GetFileMD5(other);
                md5Dict[md5] = other.Replace(rootDir, "..");
            }

            foreach (string file in all)
            {
                string md5 = EditorCommonUtils.GetFileMD5(file);
                if (md5Dict.ContainsKey(md5))
                {
                    DebugUtil.LogError(file + " <---> " + md5Dict[md5]);
                }
            }
        }

        [MenuItem("TextureTool/CheckETC2")]
        public static void CheckETC2()
        {
            foreach (string file in EditorResourcePaths.GetSpriteFiles())
            {
                Texture2D texture = EditorCommonUtils.ReadTexture(file);
                if (texture.width % 4 != 0 || texture.height % 4 != 0)
                {
                    DebugUtil.LogWarning("该图片不满足Android的压缩格式 : " + file);
                }
            }
            DebugUtil.Log("CheckETC2 Done!");
        }

        [MenuItem("TextureTool/FindRGBA")]
        public static void FindRGBA()
        {
            TextureImporterFormat[] formatConfig = new TextureImporterFormat[]
            {
                TextureImporterFormat.RGBA32,
                TextureImporterFormat.RGB24,
                TextureImporterFormat.ARGB32,
            };

            HashSet<TextureImporterFormat> filter = new HashSet<TextureImporterFormat>(formatConfig);
            foreach (string atlasFile in EditorResourcePaths.GetAllAtlasFiles())
            {
                if (IsLowSpriteatlas(atlasFile))
                {
                    continue;
                }

                SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasFile);
                for (int i = 2; i < PLATFORMS.Length; i++)
                {
                    TextureImporterPlatformSettings patformSettings = SpriteAtlasExtensions.GetPlatformSettings(spriteAtlas, PLATFORMS[i]);
                    if (patformSettings.overridden && filter.Contains(patformSettings.format))
                    {
                        DebugUtil.LogWarning("高清图集 : " + atlasFile);
                        break;
                    }
                }
            }

            DebugUtil.Log("FindRGBA Done!");
        }

        //[MenuItem("TextureTool/SetTMPFont")]
        static void SetTMPFont()
        {
            var fontDirs = new string[]
            {
                "Assets/Resources/Export/Fonts/En/",
                "Assets/Export/Fonts/En/",
            };

            var pathArrays = new string[][]
            {
                FilePathTools.GetFiles("Assets/Resources", ".prefab$", System.IO.SearchOption.AllDirectories),
                FilePathTools.GetFiles("Assets/Export", ".prefab$", System.IO.SearchOption.AllDirectories),
            };

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

        [MenuItem("TextureTool/CheckCircular")]
        static void CheckCircular()
        {
            List<string> list = new List<string>();
            foreach (var group in AssetConfigController.Instance.Groups)
            {
                foreach (var path in group.Paths)
                {
                    if (!path.FromOutside || System.IO.Directory.Exists(path.Path))
                    {
                        list.Add(path.Path);
                    }
                }
            }
            list.AddRange(AssetConfigController.Instance.ActivityResPaths);

            string root = "Assets/Export/";
            Dictionary<string, string> fileResDict = new Dictionary<string, string>();
            Dictionary<string, string[]> resFilesDict = new Dictionary<string, string[]>();
            Dictionary<string, HashSet<string>> fileDependencies = new Dictionary<string, HashSet<string>>();
            Dictionary<string, HashSet<string>> resDependencies = new Dictionary<string, HashSet<string>>();

            foreach (string name in list)
            {
                string res = root + name;
                string[] files = FilePathTools.GetFiles(res, "^(?!.*\\.(meta))", System.IO.SearchOption.AllDirectories);
                resFilesDict[res] = files;
                foreach (string file in files)
                {
                    fileResDict[file] = res;
                }
            }

            foreach (string res in resFilesDict.Keys)
            {
                string[] files = resFilesDict[res];
                HashSet<string> resSet = new HashSet<string>();
                foreach (string file in files)
                {
                    HashSet<string> fileSet = new HashSet<string>();
                    foreach (string dependency in AssetDatabase.GetDependencies(file, true))
                    {
                        string dependencyRes;
                        if (fileResDict.TryGetValue(dependency, out dependencyRes) && res != dependencyRes)
                        {
                            resSet.Add(dependencyRes);
                            fileSet.Add(dependency);
                        }
                    }
                    fileDependencies[file] = fileSet;
                }
                resDependencies[res] = resSet;
            }

            HashSet<string> filter = new HashSet<string>();
            List<string> queue = new List<string>();
            foreach (string key in resDependencies.Keys)
            {
                CheckCircular(queue, key, resFilesDict, fileResDict, resDependencies, fileDependencies, filter);
            }
        }

        static void CheckCircular(List<string> chain, string res, Dictionary<string, string[]> resFilesDict, Dictionary<string, string> fileResDict, Dictionary<string, HashSet<string>> resDependencies, Dictionary<string, HashSet<string>> fileDependencies, HashSet<string> filter)
        {
            chain.Push(res);
            foreach (string key in resDependencies[res])
            {
                if (chain.Contains(key))
                {
                    if (chain[0] == key)
                    {
                        string id = GetUniqueId(chain);
                        if (!filter.Contains(id))
                        {
                            filter.Add(id);
                            string output = "Circular : ";
                            foreach (string item in chain)
                            {
                                output += item + "-->";
                            }
                            output += "[head: " + key + "]";
                            Debug.LogError(output);

                            for (int i = 0; i < chain.Count; i++)
                            {
                                string res1 = chain[i];
                                string res2 = chain[(i + 1) % chain.Count];
                                //Debug.LogWarning(" ========== " + res1 + "-->" + res2 + " ========== :");

                                bool flag = false;
                                foreach (string file in resFilesDict[res1])
                                {
                                    foreach (string dependency in fileDependencies[file])
                                    {
                                        if (fileResDict[dependency] == res2)
                                        {
                                            flag = true;
                                            Debug.LogWarning(file + " --> " + dependency);
                                        }
                                    }
                                }
                                if (flag)
                                {
                                    Debug.LogWarning(" \n ");
                                }
                            }
                        }
                    }
                }
                else
                {
                    CheckCircular(chain, key, resFilesDict, fileResDict, resDependencies, fileDependencies, filter);
                }
            }
            chain.Pop();
        }

        static string GetUniqueId(List<string> list)
        {
            string maxValue = "";
            int maxValueIndex = 0;
            for (int i = 0; i < list.Count; i++)
            {
                string value = list[i];
                if (string.Compare(value, maxValue) > 0)
                {
                    maxValue = value;
                    maxValueIndex = i;
                }
            }

            string key = "";
            for (int i = 0; i < list.Count; i++)
            {
                key += list[(maxValueIndex + i) % list.Count];
            }
            return key;
        }

        static void CheckWrongReferences(List<List<string>> groups)
        {
            var dependenciesDict = new Dictionary<string, HashSet<string>>();
            foreach (var prefab in EditorResourcePaths.GetAllPrefabFiles())
            {
                dependenciesDict[prefab] = new HashSet<string>(AssetDatabase.GetDependencies(prefab, true));
            }

            string content = "";
            foreach (var group in groups)
            {
                HashSet<string> fileSet = new HashSet<string>();
                foreach (var name in group)
                {
                    string dir = "Assets/Export/" + name;
                    string[] files = FilePathTools.GetFiles(dir, ".*", System.IO.SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        if (file.EndsWith(".meta"))
                        {
                            continue;
                        }

                        fileSet.Add(file);

                        if (file.EndsWith(".spriteatlas"))
                        {
                            fileSet.UnionWith(AssetDatabase.GetDependencies(file, false));
                        }
                    }
                }

                foreach (string file in fileSet)
                {
                    foreach (string prefab in dependenciesDict.Keys)
                    {
                        if (prefab.IndexOf("Assets/Export/") != -1 && !fileSet.Contains(prefab) && dependenciesDict[prefab].Contains(file))
                        {
                            string text = file + " Referenced By " + prefab;
                            DebugUtil.LogWarning(text);
                            content += text + "\n";
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(content))
            {
                DebugUtil.LogWarning("汇总: \n" + content);
            }
        }

        static void CheckWrongReferences(bool all)
        {
            HashSet<string> filter = new HashSet<string>();
            List<string> list = new List<string>();
            foreach (var group in AssetConfigController.Instance.Groups)
            {
                foreach (var path in group.Paths)
                {
                    if (!path.FromOutside || System.IO.Directory.Exists(path.Path))
                    {
                        list.Add(path.Path);
                    }

                    if (path.InInitialPacket || path.FromOutside)
                    {
                        filter.Add(path.Path);
                    }
                }
            }
            list.AddRange(AssetConfigController.Instance.ActivityResPaths);
            list.Sort((a, b) => a.Length != b.Length ? a.Length - b.Length : a.CompareTo(b));
            var groups = EditorResourcePaths.ReadGroupConfig();
            var dict = new Dictionary<string, List<string>>();

            bool isSuccess = true;
            foreach (var path in list)
            {
                if (!all && filter.Contains(path))
                {
                    continue;
                }

                bool isMatch = false;
                for (int index = 0; index < groups.Count; index++)
                {
                    var group = groups[index];
                    foreach (var pattern in group)
                    {
                        var match = Regex.Match(path, "^" + pattern + "$", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            string key = index + "_";
                            if (match.Groups.Count > 1)
                            {
                                key += match.Groups[1];
                            }

                            List<string> temp;
                            if (!dict.TryGetValue(key, out temp))
                            {
                                temp = new List<string>();
                                dict.Add(key, temp);
                            }
                            temp.Add(path);

                            isMatch = true;
                            break;
                        }
                    }
                }

                if (!isMatch && !filter.Contains(path))
                {
                    isSuccess = false;
                    DebugUtil.LogWarning(path + " Not Found!");
                }
            }

            if (!isSuccess)
            {
                DebugUtil.LogError("请先修改本地的分组配置：" + EditorResourcePaths.GROUP_CONFIG_PATH);
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<TextAsset>(EditorResourcePaths.GROUP_CONFIG_PATH));
                return;
            }

            List<List<string>> groupList = new List<List<string>>();
            DebugUtil.Log("======================== 请检查下列分组是否符合预期:");
            foreach (string key in dict.Keys)
            {
                string text = "[";
                var group = dict[key];
                for (int i = 0; i < group.Count; i++)
                {
                    text += (i > 0 ? ", \"" : "\"") + group[i] + "\"";
                }
                text += "]";
                DebugUtil.Log(text);
                groupList.Add(group);
            }
            DebugUtil.Log("======================== 请检查上面分组是否符合预期!");
            CheckWrongReferences(groupList);
        }

        [MenuItem("TextureTool/CheckWrongReferences (All)")]
        static void CheckWrongReferencesOfAll()
        {
            Debug.Log("检测匹配的所有资源");
            CheckWrongReferences(true);
        }

        [MenuItem("TextureTool/CheckWrongReferences (Download)")]
        static void CheckWrongReferencesOfDownload()
        {
            Debug.Log("检测匹配的包外资源");
            CheckWrongReferences(false);
        }

        [MenuItem("TextureTool/SetTextureFormat")]
        public static void SetTextureFormat()
        {
            HandleTextureFormat(EditorResourcePaths.GetSpriteFiles(), SetResourcesFormat);
            HandleTextureFormat(EditorResourcePaths.GetRawTextureFiles(), SetRawTextureFormat);
            HandleSpriteAtlasFormat(EditorResourcePaths.GetAtlasFiles());
            AssetDatabase.SaveAssets();
            DebugUtil.Log("SetTextureFormat Done!");
        }

        [MenuItem("TextureTool/SetAtlasMaxSize")]
        public static void SetAtlasMaxSize()
        {
            MethodInfo getPreviewTexturesMI = typeof(SpriteAtlasExtensions).GetMethod("GetPreviewTextures", BindingFlags.Static | BindingFlags.NonPublic);
            foreach (string atlasFile in EditorResourcePaths.GetAllAtlasFiles())
            {
                if (IsLowSpriteatlas(atlasFile))
                {
                    continue;
                }

                SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasFile);
                List<Sprite> sprites = GetSpritesByAtlas(spriteAtlas);

                // 统计散图面积之和
                int total = 0;
                int minSize = 128;
                foreach (Sprite sprite in sprites)
                {
                    Rect rect = sprite.textureRect;
                    int width = Mathf.CeilToInt(rect.width);
                    int height = Mathf.CeilToInt(rect.height);
                    minSize = Mathf.Max(minSize, width, height);
                    total += width * height;
                }

                // 估算maxTextureSize值
                float guessArea = EditorCommonUtils.ToLargerPOT(total, 4);
                int guessSize = Mathf.RoundToInt(Mathf.Sqrt(guessArea));

                // 如果填充率太低，将估值尺寸减半
                if (total <= guessArea * 0.625f)
                {
                    guessSize /= 2;
                }

                // 用估值尺寸尝试打图集
                guessSize = Mathf.Max(guessSize, Mathf.RoundToInt(EditorCommonUtils.ToLargerPOT(minSize, 2)));
                foreach (string platform in PLATFORMS)
                {
                    TextureImporterPlatformSettings patformSettings = SpriteAtlasExtensions.GetPlatformSettings(spriteAtlas, platform);
                    if (patformSettings.maxTextureSize != guessSize)
                    {
                        patformSettings.maxTextureSize = guessSize;
                        SpriteAtlasExtensions.SetPlatformSettings(spriteAtlas, patformSettings);
                    }
                }
                SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { spriteAtlas }, EditorUserBuildSettings.activeBuildTarget);

                // 计算图集的实际面积
                Texture2D[] atlasTextures = (Texture2D[])getPreviewTexturesMI.Invoke(null, new object[] { spriteAtlas });
                int area = 0;
                for (int i = 0; i < atlasTextures.Length; i++)
                {
                    area += atlasTextures[i].width * atlasTextures[i].height;
                }

                // 如果估值尺寸效果一般，将其翻倍
                if (area > guessSize * guessSize * 2.5f)
                {
                    foreach (string platform in PLATFORMS)
                    {
                        TextureImporterPlatformSettings patformSettings = SpriteAtlasExtensions.GetPlatformSettings(spriteAtlas, platform);
                        patformSettings.maxTextureSize = Mathf.Min(guessSize * 2, MAX_TEXTURE_SIZE);
                        SpriteAtlasExtensions.SetPlatformSettings(spriteAtlas, patformSettings);
                    }
                    SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { spriteAtlas }, EditorUserBuildSettings.activeBuildTarget);
                }
                else if (area > guessSize * guessSize * 1.5f && area <= guessSize * guessSize * 2)
                {
                    TextureImporterPlatformSettings patformSettings = SpriteAtlasExtensions.GetPlatformSettings(spriteAtlas, "Android");
                    patformSettings.maxTextureSize = Mathf.Min(guessSize * 2, MAX_TEXTURE_SIZE);
                    SpriteAtlasExtensions.SetPlatformSettings(spriteAtlas, patformSettings);
                    SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { spriteAtlas }, EditorUserBuildSettings.activeBuildTarget);
                }

                SpriteAtlas lowSpriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(ToLowSpriteatlas(atlasFile));
                if (lowSpriteAtlas != null)
                {
                    foreach (string platform in PLATFORMS)
                    {
                        TextureImporterPlatformSettings patformSettings = SpriteAtlasExtensions.GetPlatformSettings(spriteAtlas, platform);
                        TextureImporterPlatformSettings lowPatformSettings = SpriteAtlasExtensions.GetPlatformSettings(lowSpriteAtlas, platform);
                        lowPatformSettings.maxTextureSize = patformSettings.maxTextureSize;
                        SpriteAtlasExtensions.SetPlatformSettings(lowSpriteAtlas, lowPatformSettings);
                    }
                }
            }
            AssetDatabase.SaveAssets();
            DebugUtil.Log("SetAtlasMaxSize Done!");
        }

        static bool IsLowSpriteatlas(string path)
        {
            return path.EndsWith(LOW_ATLAS_SUFFIX) || path.IndexOf("/Sd/") >= 0;
        }

        static string ToLowSpriteatlas(string path)
        {
            if (path.IndexOf("/Hd/") >= 0)
            {
                return path.Replace("/Hd/", "/Sd/");
            }
            else
            {
                return path.Replace(EditorResourcePaths.ATLAS_SUFFIX, LOW_ATLAS_SUFFIX);
            }
        }

        static Dictionary<string, object> GetTextureImporterValues(TextureImporter importer)
        {
            var dict = EditorCommonUtils.GetValues(importer);
            EditorCommonUtils.GetValues(importer.GetDefaultPlatformTextureSettings(), dict, "Default");
            EditorCommonUtils.GetValues(importer.GetPlatformTextureSettings("Standalone"), dict, "Standalone");
            EditorCommonUtils.GetValues(importer.GetPlatformTextureSettings("Android"), dict, "Android");
            EditorCommonUtils.GetValues(importer.GetPlatformTextureSettings("iPhone"), dict, "iPhone");
            return dict;
        }

        static void HandleTextureFormat(HashSet<string> files, System.Action<TextureImporter> setDelegate)
        {
            foreach (string file in files)
            {
                TextureImporter importer = AssetImporter.GetAtPath(file) as TextureImporter;
                var originalDict = GetTextureImporterValues(importer);
                setDelegate(importer);
                var newDict = GetTextureImporterValues(importer);

                foreach (string key in originalDict.Keys)
                {
                    if (originalDict[key].ToString() != newDict[key].ToString())
                    {
                        importer.SaveAndReimport();
                        break;
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        static Dictionary<string, object> GetSpriteAtlasSettingsValues(SpriteAtlasTextureSettings[] textureSettingsArray, Dictionary<string, TextureImporterPlatformSettings>[] platformSettingsDictArray)
        {
            var dict = new Dictionary<string, object>();
            for (int i = 0; i < ATLAS_TYPE_NUMBER; i++)
            {
                EditorCommonUtils.GetValues(textureSettingsArray[i], dict, "textureSettings" + i);
                foreach (string platform in PLATFORMS)
                {
                    EditorCommonUtils.GetValues(platformSettingsDictArray[i][platform], dict, platform + i);
                }
            }
            return dict;
        }

        static void HandleSpriteAtlasFormat(HashSet<string> atlasFiles)
        {
            string[] atlasFileArray = new string[ATLAS_TYPE_NUMBER];
            SpriteAtlas[] spriteAtlasArray = new SpriteAtlas[ATLAS_TYPE_NUMBER];
            SpriteAtlasPackingSettings[] packingSettingsArray = new SpriteAtlasPackingSettings[ATLAS_TYPE_NUMBER];
            SpriteAtlasTextureSettings[] textureSettingsArray = new SpriteAtlasTextureSettings[ATLAS_TYPE_NUMBER];
            Dictionary<string, TextureImporterPlatformSettings>[] platformSettingsDictArray = new Dictionary<string, TextureImporterPlatformSettings>[ATLAS_TYPE_NUMBER];
            platformSettingsDictArray[0] = new Dictionary<string, TextureImporterPlatformSettings>();
            platformSettingsDictArray[1] = new Dictionary<string, TextureImporterPlatformSettings>();

            foreach (string file in atlasFiles)
            {
                if (IsLowSpriteatlas(file))
                {
                    continue;
                }

                atlasFileArray[0] = file;
                atlasFileArray[1] = ToLowSpriteatlas(file);

                for (int i = 0; i < ATLAS_TYPE_NUMBER; i++)
                {
                    spriteAtlasArray[i] = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasFileArray[i]);
                    if (spriteAtlasArray[i] == null)
                    {
                        continue;
                    }

                    packingSettingsArray[i] = SpriteAtlasExtensions.GetPackingSettings(spriteAtlasArray[i]);
                    textureSettingsArray[i] = SpriteAtlasExtensions.GetTextureSettings(spriteAtlasArray[i]);
                    foreach (string platform in PLATFORMS)
                    {
                        platformSettingsDictArray[i][platform] = SpriteAtlasExtensions.GetPlatformSettings(spriteAtlasArray[i], platform);
                    }
                }

                List<Sprite> spriteList = GetSpritesByAtlas(spriteAtlasArray[0]);
                bool containAlpha = false;
                foreach (Sprite sprite in spriteList)
                {
                    Texture2D texture = EditorCommonUtils.ReadTexture(AssetDatabase.GetAssetOrScenePath(sprite));
                    if (ContainAlpha(texture))
                    {
                        containAlpha = true;
                        break;
                    }
                }

                for (int i = 0; i < ATLAS_TYPE_NUMBER; i++)
                {
                    if (spriteAtlasArray[i] == null)
                    {
                        continue;
                    }

                    packingSettingsArray[i].enableRotation = false;
                    packingSettingsArray[i].enableTightPacking = false;
                    SetDefaultTextureSettings(ref textureSettingsArray[i]);
                }

                if (containAlpha)
                {
                    //SetPlatformTextureSettings(platformSettingsDictArray[0]["Default"], "Default", TextureImporterFormat.RGBA32);
                    SetPlatformTextureSettings(platformSettingsDictArray[0]["Standalone"], "Standalone", TextureImporterFormat.DXT5);
                    SetPlatformTextureSettings(platformSettingsDictArray[0]["Android"], "Android", new TextureImporterFormat[] { TextureImporterFormat.ETC2_RGBA8, IOS_NEW_RGBA });
                    SetPlatformTextureSettings(platformSettingsDictArray[0]["iPhone"], "iPhone", IOS_NEW_RGBA);

                    if (spriteAtlasArray[1] != null)
                    {
                        //SetPlatformTextureSettings(platformSettingsDictArray[1]["Default"], "Default", TextureImporterFormat.RGBA32);
                        SetPlatformTextureSettings(platformSettingsDictArray[1]["Standalone"], "Standalone", TextureImporterFormat.DXT5Crunched);
                        SetPlatformTextureSettings(platformSettingsDictArray[1]["Android"], "Android", TextureImporterFormat.ETC2_RGBA8Crunched);
                        SetPlatformTextureSettings(platformSettingsDictArray[1]["iPhone"], "iPhone", TextureImporterFormat.PVRTC_RGBA4);
                    }
                }
                else
                {
                    //SetPlatformTextureSettings(platformSettingsDictArray[0]["Default"], "Default", TextureImporterFormat.RGB24);
                    SetPlatformTextureSettings(platformSettingsDictArray[0]["Standalone"], "Standalone", TextureImporterFormat.DXT1);
                    SetPlatformTextureSettings(platformSettingsDictArray[0]["Android"], "Android", TextureImporterFormat.ETC2_RGB4);
                    SetPlatformTextureSettings(platformSettingsDictArray[0]["iPhone"], "iPhone", TextureImporterFormat.PVRTC_RGB4);

                    if (spriteAtlasArray[1] != null)
                    {
                        //SetPlatformTextureSettings(platformSettingsDictArray[1]["Default"], "Default", TextureImporterFormat.RGB24);
                        SetPlatformTextureSettings(platformSettingsDictArray[1]["Standalone"], "Standalone", TextureImporterFormat.DXT1Crunched);
                        SetPlatformTextureSettings(platformSettingsDictArray[1]["Android"], "Android", TextureImporterFormat.ETC_RGB4Crunched);
                        SetPlatformTextureSettings(platformSettingsDictArray[1]["iPhone"], "iPhone", TextureImporterFormat.PVRTC_RGB4);
                    }
                }

                for (int i = 0; i < ATLAS_TYPE_NUMBER; i++)
                {
                    if (spriteAtlasArray[i] == null)
                    {
                        continue;
                    }

                    SpriteAtlasExtensions.SetIncludeInBuild(spriteAtlasArray[i], false);
                    SpriteAtlasExtensions.SetPackingSettings(spriteAtlasArray[i], packingSettingsArray[i]);
                    SpriteAtlasExtensions.SetTextureSettings(spriteAtlasArray[i], textureSettingsArray[i]);
                    foreach (string platform in PLATFORMS)
                    {
                        SpriteAtlasExtensions.SetPlatformSettings(spriteAtlasArray[i], platformSettingsDictArray[i][platform]);
                    }
                }

                if (spriteAtlasArray[1] != null)
                {
                    SpriteAtlasExtensions.SetIsVariant(spriteAtlasArray[1], false);
                    SpriteAtlasExtensions.Remove(spriteAtlasArray[1], SpriteAtlasExtensions.GetPackables(spriteAtlasArray[1]));
                    SpriteAtlasExtensions.Add(spriteAtlasArray[1], SpriteAtlasExtensions.GetPackables(spriteAtlasArray[0]));
                }
            }
            AssetDatabase.Refresh();
        }

        static void SetDefaultFormat(TextureImporter importer)
        {
            importer.textureType = TextureImporterType.Default;
            importer.textureShape = TextureImporterShape.Texture2D;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.sRGBTexture = true;
            importer.isReadable = false;
            importer.mipmapEnabled = false;
            importer.streamingMipmaps = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.maxTextureSize = MAX_TEXTURE_SIZE;
            importer.anisoLevel = 0;
            importer.spritePackingTag = "";
            importer.allowAlphaSplitting = false;
            importer.compressionQuality = COMPRESSION_QUALITY;
        }

        static void SetSpriteFormat(TextureImporter importer)
        {
            SetDefaultFormat(importer);
            importer.textureType = TextureImporterType.Sprite;
        }

        static void SetPlatformTextureSettings(TextureImporterPlatformSettings patformSettings, string platform, TextureImporterFormat textureFormat, bool allowsAlphaSplit = false)
        {
            patformSettings.overridden = true;
            patformSettings.name = platform;
            patformSettings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
            patformSettings.format = textureFormat;
            patformSettings.compressionQuality = COMPRESSION_QUALITY;
            patformSettings.allowsAlphaSplitting = allowsAlphaSplit;
            patformSettings.androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings;
        }

        static void SetPlatformTextureSettings(TextureImporterPlatformSettings patformSettings, string platform, TextureImporterFormat[] textureFormats, bool allowsAlphaSplit = false)
        {
            TextureImporterFormat textureFormat = textureFormats[0];
            foreach (var format in textureFormats)
            {
                if (format == patformSettings.format)
                {
                    textureFormat = format;
                    break;
                }
            }

            SetPlatformTextureSettings(patformSettings, platform, textureFormat, allowsAlphaSplit);
        }

        static void SetPlatformTextureImporterSettings(TextureImporter importer, string platform, TextureImporterFormat textureFormat, bool allowsAlphaSplit = false)
        {
            TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
            SetPlatformTextureSettings(settings, platform, textureFormat, allowsAlphaSplit);
            settings.maxTextureSize = MAX_TEXTURE_SIZE;
            importer.SetPlatformTextureSettings(settings);
        }

        static void SetTextureImporterFormat(TextureImporter importer)
        {
            Texture2D texture = EditorCommonUtils.ReadTexture(importer.assetPath);

            if (ContainAlpha(texture))
            {
                SetPlatformTextureImporterSettings(importer, "Default", TextureImporterFormat.RGBA32);
                SetPlatformTextureImporterSettings(importer, "Standalone", TextureImporterFormat.DXT5);
                SetPlatformTextureImporterSettings(importer, "Android", TextureImporterFormat.ETC2_RGBA8);
                SetPlatformTextureImporterSettings(importer, "iPhone", IOS_NEW_RGBA);
            }
            else
            {
                SetPlatformTextureImporterSettings(importer, "Default", TextureImporterFormat.RGB24);
                SetPlatformTextureImporterSettings(importer, "Standalone", TextureImporterFormat.DXT1);
                SetPlatformTextureImporterSettings(importer, "Android", TextureImporterFormat.ETC2_RGB4);

                bool pvrtc;
                if (importer.npotScale == TextureImporterNPOTScale.None)
                {
                    pvrtc = texture.width == texture.height && EditorCommonUtils.IsPOT(texture.width, 2) && EditorCommonUtils.IsPOT(texture.height, 2);
                }
                else
                {
                    Texture2D temp = AssetDatabase.LoadAssetAtPath<Texture2D>(importer.assetPath);
                    pvrtc = temp.width == temp.height;
                }

                if (pvrtc)
                {
                    SetPlatformTextureImporterSettings(importer, "iPhone", TextureImporterFormat.PVRTC_RGB4);
                }
                else
                {
                    SetPlatformTextureImporterSettings(importer, "iPhone", IOS_NEW_RGB);
                }
            }
        }

        static void SetResourcesFormat(TextureImporter importer)
        {
            SetSpriteFormat(importer);
            SetTextureImporterFormat(importer);
        }

        static void SetRawTextureFormat(TextureImporter importer)
        {
            SetDefaultFormat(importer);
            SetTextureImporterFormat(importer);
        }

        static void SetDefaultTextureSettings(ref SpriteAtlasTextureSettings textureSettings)
        {
            textureSettings.anisoLevel = 0;
            textureSettings.filterMode = FilterMode.Bilinear;
            textureSettings.generateMipMaps = false;
            textureSettings.readable = false;
            textureSettings.sRGB = true;
        }

        static bool ContainAlpha(Texture2D readableTexture)
        {
            for (int i = 0; i < readableTexture.width; i++)
            {
                for (int j = 0; j < readableTexture.height; j++)
                {
                    Color color = readableTexture.GetPixel(i, j);
                    if (!color.a.Equals(1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void FilterTextureAlpha(string path)
        {
            Texture2D texture = EditorCommonUtils.ReadTexture(path);
            for (int i = 0; i < texture.width; i++)
            {
                for (int j = 0; j < texture.height; j++)
                {
                    Color color = texture.GetPixel(i, j);
                    color.a = 1;
                    texture.SetPixel(i, j, color);
                }
            }
            EditorCommonUtils.WriteBytes(path, texture.EncodeToPNG());
        }

        public static List<Sprite> GetSpritesByFile(string path)
        {
            List<Sprite> sprites = new List<Sprite>();
            foreach (Object o in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                Sprite sprite = o as Sprite;
                if (sprite)
                {
                    sprites.Add(sprite);
                }
            }
            return sprites;
        }

        public static List<Sprite> GetSpritesByAtlas(SpriteAtlas spriteAtlas)
        {
            List<Sprite> sprites = new List<Sprite>();
            foreach (Object asset in SpriteAtlasExtensions.GetPackables(spriteAtlas))
            {
                string path = AssetDatabase.GetAssetOrScenePath(asset);
                if (System.IO.Directory.Exists(path))
                {
                    foreach (string filePath in System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories))
                    {
                        sprites.AddRange(GetSpritesByFile(filePath));
                    }
                }
                else
                {
                    sprites.AddRange(GetSpritesByFile(path));
                }
            }
            return sprites;
        }
    }
}