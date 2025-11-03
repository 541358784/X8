
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEditor;

public static partial class CodeGen
{
    private static StringBuilder GetCamelCaseName(string fileName)
    {
        var className = new StringBuilder();
        var tmp = fileName.Replace(" ", "_");
        var isUnderline = false;
        foreach (var c in tmp)
        {
            if (c == '_')
            {
                isUnderline = true;
            }
            else if (!char.IsLetterOrDigit(c))
            {
                // skip
            }
            else if (isUnderline)
            {
                className.Append(char.ToUpper(c));
                isUnderline = false;
            }
            else
            {
                className.Append(c);
            }
        }

        return className;
    }
    
    public static string GetMd5Hash(string input)
    {
        var md5Hash = MD5.Create();
        var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sBuilder = new StringBuilder();
        foreach (var t in data)
        {
            sBuilder.Append(t.ToString("x2"));
        }

        return sBuilder.ToString().Substring(0, 8);
    }
    
    private static void SaveFile(string subDirName, string fileName, string code)
    {
        var root = $"Assets/Scripts/CodeGen/{subDirName}/";
        if (!Directory.Exists(root)) Directory.CreateDirectory(root);
        
        var path = $"{root}/{fileName}.cs";
        if (File.Exists(path)) File.Delete(path);
        
        var writer = new StreamWriter(path, true);
        writer.WriteLine(code);
        writer.Close();

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        Debug.Log($"GenCode: {path}");
    }
}