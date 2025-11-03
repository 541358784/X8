#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OneLine.Editor
{
    public class OneLineNameTool : ScriptableWizard
    {
        [MenuItem("Tools/OneLine/批量重命名")]
        private static void Open()
        {
            DisplayWizard<OneLineNameTool>("一笔画资源重命名");
        }

        public Texture2D[] Textures = new Texture2D[0];

        private void OnWizardCreate()
        {
            string[] names = Textures.Select(x => x.name).ToArray();

            for (int i = 0; i < names.Length; i++)
            {
                string fileName            = names[i];
                string configPath          = $"{OneLineEditor.ConfigPath}/{fileName}";
                string templateTexturePath = $"{OneLineEditor.TemplateTexturePath}/{fileName}";
                string pathTexturePath     = $"{OneLineEditor.PathTexturePath}/{fileName}";

                string newName = $"Temp{fileName}";
                names[i]            = newName;
                configPath          = Rename(configPath, newName, "json");
                templateTexturePath = Rename(templateTexturePath, newName, "png");
                pathTexturePath     = Rename(pathTexturePath, newName, "png");
            }

            for (int i = 0; i < names.Length; i++)
            {
                string fileName            = Path.GetFileNameWithoutExtension(names[i]);
                string configPath          = $"{OneLineEditor.ConfigPath}/{fileName}";
                string templateTexturePath = $"{OneLineEditor.TemplateTexturePath}/{fileName}";
                string pathTexturePath     = $"{OneLineEditor.PathTexturePath}/{fileName}";

                string newName = (i + 1).ToString();
                names[i]            = newName;
                configPath          = Rename(configPath, newName, "json");
                templateTexturePath = Rename(templateTexturePath, newName, "png");
                pathTexturePath     = Rename(pathTexturePath, newName, "png");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static string Rename(string filePath, string newName, string suffix)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

            if (File.Exists($"{filePath}.{suffix}") == false)
            {
                if (suffix != "json")
                {
                    Debug.LogError($"丢失匹配的贴图 {filePath}");
                }

                return string.Empty;
            }

            string dir = Path.GetDirectoryName(filePath);
            try
            {
                File.Move($"{filePath}.{suffix}", $"{dir}/{newName}.{suffix}");
                File.Move($"{filePath}.{suffix}.meta", $"{dir}/{newName}.{suffix}.meta");
            }
            catch (Exception e)
            {
                throw e;
            }

            return $"{dir}/{newName}";
        }
    }
}
#endif