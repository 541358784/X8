#if ENABLE_INJECTFIX
using UnityEditor;
using System.IO;
using System.Reflection;

namespace DragonU3DSDK.Asset
{
    public class IFixBuild
    {
        public static void CreatePatches()
        {
            string rootDir = FilePathTools.NormalizeDirectory(Directory.GetCurrentDirectory());
            string patchDir = "Assets/Export/" + IFixManager.ASSET_PATH + "/";
            foreach (string file in FilePathTools.GetFiles(rootDir, ".patch.bytes$", SearchOption.AllDirectories))
            {
                File.Delete(file);
            }

            var type = typeof(IFixBuild).Assembly.GetType("IFix.Editor.IFixEditor");
            BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic;
#if UNITY_ANDROID
            type.InvokeMember("CompileToAndroid", bindingFlags, null, null, null);
#elif UNITY_IOS
            type.InvokeMember("CompileToIOS", bindingFlags, null, null, null);
#else
            type.InvokeMember("Patch", bindingFlags, null, null, null);
#endif
            string text = "";
            foreach (string file in FilePathTools.GetFiles(rootDir, ".patch.bytes$"))
            {
                string name = file.Replace(rootDir, "");
                EditorCommonUtils.MoveFile(file, patchDir + name);
                text += name.Replace(".bytes", "") + "\n";
            }
            EditorCommonUtils.WriteText(patchDir + IFixManager.PATCH_LIST + ".txt", text);
            AssetDatabase.Refresh();

            DebugUtil.Log("CreatePatches Done");
        }
    }
}

#endif
