using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System;
using DragonU3DSDK;
using DragonU3DSDK.Asset;

public static class AssemblyExportTool
{
    static string EXTERNAL_DIR = "Assets/_External/";
    static Dictionary<string, string> scriptMap = new Dictionary<string, string>();
    static HashSet<string> copiedScripts = new HashSet<string>();

    [MenuItem("Tools/AssemblyExport")]
    static void Export()
    {
        string root = Directory.GetCurrentDirectory();
        string dir = Path.GetDirectoryName(root);
        string name = Path.GetFileName(root);
        string target = dir + "Res/" + name;
        Export(target);
        DebugUtil.Log("AssemblyExport Done");
    }

    public static void Export(string target)
    {
        target = FilePathTools.NormalizeDirectory(target);
        FilePathTools.DeleteDirectory(target + EXTERNAL_DIR, true);
        System.Threading.Thread.Sleep(2000);

        CopyAsmdef(target);
        CopyAssemblies(target);
        CopyLibs(target);
        CopyResources(target);
        CopyScripts(target);
        CopyLuaFiles(target);
    }

    static void CopyAsmdef(string target)
    {
        string srcPath = "Assets/DragonSDK/DragonU3DSDK/Framework/AssemblyTool/Editor/_asmdef.temp";
        string targetPath = target + "Assets/_Main.asmdef";

        if (!File.Exists(targetPath))
        {
            FilePathTools.CopyFile(srcPath, targetPath);
        }
    }

    static void CopyFileWithMeta(string srcPath, string destPath)
    {
        if (File.Exists(srcPath))
        {
            FilePathTools.CopyFile(srcPath, destPath);
        }

        if (File.Exists(srcPath + ".meta"))
        {
            FilePathTools.CopyFile(srcPath + ".meta", destPath + ".meta");
        }
    }

    static string CustomCopyFile(string srcPath, string destPath, bool preCheck = false)
    {
        destPath = destPath.Replace("/Assets/", "/" + EXTERNAL_DIR);
        if (!preCheck)
        {
            CopyFileWithMeta(srcPath, destPath);
        }
        return destPath;
    }

    static void CopyAssemblies(string target)
    {
        List<string> assemblies = new List<string> {
            "Assembly-CSharp-firstpass",
            "Assembly-CSharp-Editor-firstpass",
#if !UNITY_2022_3_OR_NEWER
            "Assembly-CSharp",
            "Assembly-CSharp-Editor",
#endif
        };

        foreach (string path in FilePathTools.GetFiles("Assets", ".asmdef$", SearchOption.AllDirectories))
        {
            if (File.Exists(target + path))
            {
                continue;
            }

            string text = File.ReadAllText(path);
            Match match = Regex.Match(text, ".*\"name\"\\s*:\\s*\"(.*)\"");
            if (match.Success)
            {
                assemblies.Add(match.Groups[1].ToString());
            }
        }

        foreach (string assembly in assemblies)
        {
            CustomCopyFile(string.Format("Library/ScriptAssemblies/{0}.dll", assembly), string.Format("{0}Assets/Assemblies/{1}.dll", target, assembly));
            CustomCopyFile(string.Format("Library/ScriptAssemblies/{0}.pdb", assembly), string.Format("{0}Assets/Assemblies/{1}.pdb", target, assembly));
        }
#if UNITY_2022_3_OR_NEWER
        string tempDir = "Temp/CustomDlls/";
        string[] newDlls = new[] {tempDir + "Custom-CSharp.dll", tempDir + "Custom-CSharp-Editor.dll"};
        AssemblyTool.CompileAssemblies(new[] {"Assembly-CSharp.dll$", "Assembly-CSharp-Editor.dll$"}, newDlls);
        foreach (var newDll in newDlls)
        {
            string newPdb = newDll.Replace(".dll", ".pdb");
            CustomCopyFile(newDll, newDll.Replace(tempDir, string.Format("{0}Assets/Assemblies/", target)));
            CustomCopyFile(newPdb, newPdb.Replace(tempDir, string.Format("{0}Assets/Assemblies/", target)));
        }
        FilePathTools.DeleteDirectory(tempDir, true);
#endif
    }

    static void CopyLibs(string target)
    {
        HashSet<string> suffixSet = new HashSet<string>(new string[] { ".dll" });
        string root = Directory.GetCurrentDirectory();
        string[] files = FilePathTools.GetFiles("Assets", EditorResourcePaths.FILTER_META_REGEX, SearchOption.AllDirectories);
        foreach (string file in files)
        {
            if (!file.StartsWith("Assets/Plugins") && suffixSet.Contains(FilePathTools.GetFileExtension(file)))
            {
                CustomCopyFile(file, target + file);
            }
        }
    }

    static void CopyDragonSDK(string target)
    {
        string dir = "Assets/DragonSDK/DragonU3DSDK";
        FilePathTools.DeleteDirectory(target + dir, true);
        string[] files = FilePathTools.GetFiles(dir, EditorResourcePaths.FILTER_META_REGEX, SearchOption.AllDirectories);
        foreach (string file in files)
        {
            if (!file.EndsWith(".cs") && file.IndexOf("/.") == -1)
            {
                CopyFileWithMeta(file, target + file);
            }
        }
    }

    static void CopyResources(string target)
    {
        string[] dirConfigs = new string[]
        {
            "ProjectSettings", "true",              // 删除再同步项目设置目录
            "Assets/Resources/Settings", "true",    // 删除再同步设置目录
            "Assets/Resources/versionfile", "true", // 删除再同步版本目录
            "Assets/Scenes", "false",               // 增量同步场景目录
            "Assets/HomeSubmodule/Scenes", "false", // 增量同步房间场景目录
        };

        string[] fileConfigs = new string[]
        {
            "Assets/Resources", "UIRoot.prefab",
            "Assets/Resources", "PluginsConfig.txt",
            "Assets/Resources/Fonts", "CustomArial.otf",
            "Assets/Export", ".*\\.spriteatlas",
        };

        for (int i = 0; i < dirConfigs.Length; i += 2)
        {
            string dir = dirConfigs[i];
            if (dirConfigs[i + 1] == "true")
            {
                FilePathTools.DeleteDirectory(target + dir, true);
            }

            foreach (string path in FilePathTools.GetFiles(dir, EditorResourcePaths.FILTER_META_REGEX, SearchOption.AllDirectories))
            {
                CopyFileWithMeta(path, target + path);
            }
        }

        for (int i = 0; i < fileConfigs.Length; i += 2)
        {
            string dir = fileConfigs[i];
            string name = fileConfigs[i + 1];
            var files = FilePathTools.GetFiles(target + dir, "^" + name + "$", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                File.Delete(file);
                File.Delete(file + ".meta");
            }

            files = FilePathTools.GetFiles(dir, "^" + name + "$", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                CopyFileWithMeta(file, target + file);
            }
        }
    }

    static void CopyLuaFiles(string target)
    {
#if XLUA_ENABLE || HOTFIX_ENABLE
        string dir = "Assets/Export/lua";
        FilePathTools.DeleteDirectory(target + dir, true);

        string[] files = FilePathTools.GetFiles(dir, ".*.lua.txt$", SearchOption.AllDirectories);
        foreach (string path in files)
        {
            string targetPath = target + path;
            CopyFileWithMeta(path, targetPath);

            Process process = new Process();
            process.StartInfo.FileName = "Assets/DragonSDK/DragonU3DSDK/Framework/AssemblyTool/Editor/luac";
            process.StartInfo.Arguments = "-o " + targetPath + " " + targetPath;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
            process.Dispose();
        }
#endif
    }

    static void CopyScripts(string target)
    {
        Dictionary<string, string> otherScriptDict = new Dictionary<string, string>();
        string[] otherScripts = FilePathTools.GetFiles(target, ".cs$", SearchOption.AllDirectories);
        foreach (string path in otherScripts)
        {
            string guid = GetGuid(path);
            if (!string.IsNullOrEmpty(guid))
            {
                otherScriptDict[guid] = path;
            }
        }

        scriptMap.Clear();
        copiedScripts.Clear();
        string[] scripts = FilePathTools.GetFiles("Assets", ".cs$", SearchOption.AllDirectories);
        foreach (string script in scripts)
        {
            CopyScript(script, target + script, true);
        }

        foreach (string script in scripts)
        {
            if (CopyScript(script, target + script))
            {
                string otherScript;
                if (otherScriptDict.TryGetValue(GetGuid(script), out otherScript))
                {
                    File.Delete(otherScript);
                    File.Delete(otherScript + ".meta");
                }
            }
        }
    }

    static string GetGuid(string path)
    {
        string metaPath = path + ".meta";
        if (File.Exists(metaPath))
        {
            string metaText = File.ReadAllText(metaPath);
            Match match = Regex.Match(metaText, ".*guid:\\s*([\\w\\d]+.)*");
            if (match.Success)
            {
                return match.Groups[1].ToString();
            }
        }

        return "";
    }

    static bool CopyScript(string srcPath, string targetPath, bool preCheck = false)
    {
        Type type = GetType(srcPath);
        if (type == null)
        {
            return false;
        }

        if (!type.IsSubclassOf(typeof(UnityEngine.Object)))
        {
            return true;
        }

        if (type.IsAbstract)
        {
            return true;
        }

        if (!type.IsVisible)
        {
            return false;
        }
        
        string key = type.Namespace + "." + type;
        if (copiedScripts.Contains(key))
        {
            return false;
        }

        bool useInherit = false;
        ConstructorInfo[] constructorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (ConstructorInfo constructorInfo in constructorInfos)
        {
            if (!constructorInfo.IsPrivate && constructorInfo.GetParameters().Length == 0)
            {
                useInherit = true;
            }
        }
        useInherit = useInherit && !type.IsSealed;

        if (!useInherit)
        {
            targetPath = targetPath.Replace(".cs", "Bridge.cs");
        }

        targetPath = CustomCopyFile(srcPath, targetPath, preCheck);
        string targetName = Path.GetFileName(targetPath).Replace(".cs", "");

        if (preCheck)
        {
            scriptMap[type.FullName] = string.Format("{0}_ASM_Bridge.{1}", type.Namespace, targetName);
            return false;
        }
        
        copiedScripts.Add(key);
        using (StringWriter stringReader = new StringWriter())
        {
            stringReader.WriteLine(string.Format("namespace {0}_ASM_Bridge {1}", type.Namespace, "{\n"));
            if (type.IsSubclassOf(typeof(Editor)))
            {
                foreach (var attribute in type.CustomAttributes)
                {
                    stringReader.Write("[");
                    stringReader.Write(attribute.AttributeType);
                    stringReader.Write("(");
                    bool firstArgument = true;
                    for (int i = 0; i < attribute.ConstructorArguments.Count; i++)
                    {
                        var argument = attribute.ConstructorArguments[i];
                        if (argument.Value == null)
                        {
                            continue;
                        }

                        if (!firstArgument)
                        {
                            stringReader.Write(", ");
                        }
                        firstArgument = false;

                        if (argument.ArgumentType == typeof(Type))
                        {
                            var valueType = (Type)argument.Value;
                            string fullname = valueType.FullName;
                            fullname = scriptMap.ContainsKey(fullname) ? scriptMap[fullname] : fullname;
                            stringReader.Write(string.Format("typeof({0})", fullname));
                        }
                        else if (argument.ArgumentType == typeof(Boolean))
                        {
                            stringReader.Write(argument.Value.ToString().ToLower());
                        }
                        else if (argument.ArgumentType == typeof(string))
                        {
                            stringReader.Write("\"" + argument.Value + "\"");
                        }
                        else
                        {
                            stringReader.Write(argument.Value);
                        }
                    }
                    stringReader.WriteLine(")]");
                }
            }
            if (useInherit)
            {
                string typerefix = string.IsNullOrEmpty(type.Namespace) ? "global::" : "";
                stringReader.WriteLine(string.Format("public class {0} : {1}", targetName, typerefix + type + " {\n\n}"));
            }
            else
            {
                FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                stringReader.WriteLine(string.Format("public class {0} : MonoBehaviourBridge {1}", targetName, "{"));
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if (fieldInfo.IsInitOnly)
                    {
                        continue;
                    }

                    stringReader.WriteLine(string.Format("\tpublic {0} {1};", GetTypeShowName(fieldInfo.FieldType), fieldInfo.Name));
                }
                stringReader.WriteLine("\n\tpublic override UnityEngine.Component AddTargetComponent() {");
                stringReader.WriteLine(string.Format("\t\tvar component = gameObject.AddComponent<{0}>();", type));
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if (fieldInfo.IsInitOnly)
                    {
                        continue;
                    }

                    stringReader.WriteLine(string.Format("\t\tif ({0} != default) component.{0} = {0};", fieldInfo.Name));
                }
                stringReader.WriteLine(string.Format("\t\treturn component;", type));
                stringReader.WriteLine("\t}\n}");
            }
            stringReader.WriteLine("\n}");

            File.WriteAllText(targetPath, stringReader.ToString());
        }

        return true;
    }

    static Type GetType(string path)
    {
        string name = Path.GetFileName(path).Replace(".cs", "");

        Type type = null;

        foreach (string line in File.ReadLines(path))
        {
            int index = line.IndexOf("namespace");
            if (index == -1)
            {
                continue;
            }

            Match match = Regex.Match(line, ".*namespace\\s+([^\\s{]+)\\s*{?.*");
            name = match.Groups[1].ToString() + "." + name;
            break;
        }

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            if (!assembly.FullName.StartsWith("Assembly-CSharp"))
            {
                continue;
            }

            type = assembly.GetType(name);
            if (type != null)
            {
                break;
            }
        }

        return type;
    }

    static string GetTypeShowName(Type type)
    {
        string name = type.ToString();
        name = name.Replace("+", ".");
        return GetShowName(name);
    }

    static string GetShowName(string name)
    {
        Match match = Regex.Match(name, "(.+)`\\d+\\[([^\\[\\]]+)\\](.*)");
        if (match.Success)
        {
            name = GetShowName(string.Format("{0}<{1}>{2}", match.Groups[1], match.Groups[2], match.Groups[3]));
        }

        return name;
    }
}