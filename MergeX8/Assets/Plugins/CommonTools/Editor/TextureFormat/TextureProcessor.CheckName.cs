using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public partial class TextureProcessor
{
    private string CheckName(string fileName)
    {
        if (fileName.Contains("ReflectionProbe-"))
        {
            var index = fileName.LastIndexOf("-");
            fileName = fileName.Remove(index, fileName.Length - index);
            fileName += "_";
        }

        return fileName;
    }

    private string GetFileName(string assetPath)
    {
        return Path.GetFileNameWithoutExtension(assetPath);
    }
}