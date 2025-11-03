using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using System.Collections.Generic;
using System.IO;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using SystemAssembly = System.Reflection.Assembly;

public static class AssemblyTool
{
    private static Dictionary<string, string> ReplacementDict = new Dictionary<string, string>();
    private static bool CrossFlag = false;

    [MenuItem("Automation/Assembly/CompileDll (Select)")]
    static void CompileDll()
    {
        List<string> files = new List<string>();
        string name = null;
        foreach (Object selection in Selection.GetFiltered(typeof(Object), SelectionMode.TopLevel))
        {
            string path = AssetDatabase.GetAssetPath(selection);
            name = name == null ? path : EditorCommonUtils.SameStart(name, path);

            if (Directory.Exists(path))
            {
                files.AddRange(FilePathTools.GetFiles(path, ".cs$", SearchOption.AllDirectories));
            }
            else if (path.EndsWith(".cs"))
            {
                files.Add(path);
            }
        }

        if (!File.Exists(name) && !Directory.Exists(name))
        {
            name = Path.GetDirectoryName(name);
        }
        name = Path.GetFileName(EditorCommonUtils.TrimEnd(name, "/"));
        name = EditorCommonUtils.TrimExtension(name) + ".dll";

        string dir = EditorCommonUtils.GetCurrentDirectory() + EditorUserBuildSettings.activeBuildTarget + "/DLL/";
        string target = dir + name;
        Compile("((Assembly-CSharp)|(Main)).dll$", files.ToArray(), target, false);
        DebugUtil.Log("CompileDll " + target);
    }

    [MenuItem("Automation/Assembly/CompileDragonSDK")]
    static void CompileDragonSDK()
    {
        string rootDir = EditorCommonUtils.GetCurrentDirectory();
        string source = "Assets/DragonSDK/";
        string targetDir = rootDir + EditorUserBuildSettings.activeBuildTarget + "/DLL/";
        string targetDll = targetDir + "DragonSDK.dll";
        string targetRefDir = targetDir + "DragonSDK_Referenced/";

        string[] scripts = FilePathTools.GetFiles(source, ".cs$", SearchOption.AllDirectories);
        Compile("((Assembly-CSharp)|(Main)).dll$", scripts, targetDll, false);

        string[] files = FilePathTools.GetFiles(source, "^(?!.*\\.cs(.meta)?$)", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            EditorCommonUtils.CopyFile(file, targetRefDir + file);
        }
    }

    public static void CompileAssembly(string source, string target, bool release = false, bool sync = true)
    {
        Compile(source, null, target, release, sync);
    }

    public static void CompileAssemblies(string[] sources, string[] targets, bool release = false, bool sync = true)
    {
        CrossFlag = true;
        ReplacementDict.Clear();
        for (int i = 0; i < sources.Length; i++)
        {
            Compile(sources[i], null, targets[i], release, sync);
        }
        ReplacementDict.Clear();
        CrossFlag = false;
    }

    public static void Compile(string assemblyRegex, string[] files, string target, bool release = false, bool sync = true)
    {
        EditorCommonUtils.WriteBytes(target, new byte[] { });
        EditorCommonUtils.DeleteFile(target);
        Assembly assembly = EditorCommonUtils.Select(CompilationPipeline.GetAssemblies(), assemblyRegex, (a) => a.outputPath)[0];
        files = files != null ? files : assembly.sourceFiles;
        AssemblyBuilder builder = new AssemblyBuilder(target, release ? EditorCommonUtils.Select(files, "^(?!.*/Editor/)").ToArray() : files);
        builder.additionalDefines = release ? EditorCommonUtils.Select(assembly.defines, "^(?!.*((Editor)|(Debug)))").ToArray() : assembly.defines;
        List<string> allReferences = new List<string>(assembly.allReferences);
        if (!release)
        {
            List<Assembly> assemblyList = EditorCommonUtils.Select(CompilationPipeline.GetAssemblies(), assembly.outputPath.Replace(".dll", ".*Editor.dll"), (a) => a.outputPath);
            if (assemblyList.Count > 0)
            {
                allReferences.AddRange(assemblyList[0].allReferences);
            }
        }

        if (CrossFlag)
        {
            ReplacementDict[assembly.outputPath] = target;
            for (int i = 0; i < allReferences.Count; i++)
            {
                if (allReferences[i] != assembly.outputPath && ReplacementDict.TryGetValue(allReferences[i], out var replacement))
                {
                    allReferences[i] = replacement;
                }
            }
        }

        builder.additionalReferences = allReferences.ToArray();
        builder.excludeReferences = new string[] { assembly.outputPath };
        builder.compilerOptions = assembly.compilerOptions;
        builder.flags = release ? AssemblyBuilderFlags.None : AssemblyBuilderFlags.DevelopmentBuild;
        builder.buildFinished += (path, compilerMessages) =>
        {
            foreach (CompilerMessage compilerMessage in compilerMessages)
            {
                if (compilerMessage.type == CompilerMessageType.Error)
                {
                    DebugUtil.LogError(compilerMessage.message);
                }
                else
                {
                    DebugUtil.LogWarning(compilerMessage.message);
                }
            }

            if (File.Exists(path))
            {
                Dictionary<string, SystemAssembly> allAssemblyDict = new Dictionary<string, SystemAssembly>();
                foreach (var tempAssembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    allAssemblyDict.Add(tempAssembly.GetName().Name, tempAssembly);
                }

                string projectDir = EditorCommonUtils.GetCurrentDirectory();
                string exportDir = EditorCommonUtils.GetDirectoryName(path) + "/" + EditorCommonUtils.TrimExtension(EditorCommonUtils.GetFileName(path)) + "_Referenced/";
                EditorCommonUtils.DeleteDirectory(exportDir, true);
                var targetAssembly = SystemAssembly.LoadFrom(path);
                foreach (var referencedAssembly in GetReferencedAssemblies(targetAssembly, allAssemblyDict).Values)
                {
                    string location = EditorCommonUtils.TrimStart(referencedAssembly.Location, projectDir);
                    if (location.StartsWith("/"))
                    {
                        EditorCommonUtils.CopyFile(location, exportDir + "UnityEngine/" + EditorCommonUtils.GetFileName(location));
                    }
                    else
                    {
                        EditorCommonUtils.CopyFile(location, exportDir + location);
                    }
                }
            }
        };
        builder.Build();

        if (sync)
        {
            while (builder.status != AssemblyBuilderStatus.Finished)
            {
                System.Threading.Thread.Sleep(10);
            }
        }
    }

    static void GainReferencedAssemblies(SystemAssembly assembly, Dictionary<string, SystemAssembly> allAssemblyDict, Dictionary<string, SystemAssembly> referencedAssemblyDict)
    {
        foreach (var assemblyName in assembly.GetReferencedAssemblies())
        {
            SystemAssembly referencedAssembly;
            if (!referencedAssemblyDict.ContainsKey(assemblyName.Name) && allAssemblyDict.TryGetValue(assemblyName.Name, out referencedAssembly))
            {
                referencedAssemblyDict.Add(assemblyName.Name, referencedAssembly);
                GainReferencedAssemblies(referencedAssembly, allAssemblyDict, referencedAssemblyDict);
            }
        }
    }

    static Dictionary<string, SystemAssembly> GetReferencedAssemblies(SystemAssembly assembly, Dictionary<string, SystemAssembly> allAssemblyDict)
    {
        var referencedAssemblyDict = new Dictionary<string, SystemAssembly>();
        GainReferencedAssemblies(assembly, allAssemblyDict, referencedAssemblyDict);
        return referencedAssemblyDict;
    }
}