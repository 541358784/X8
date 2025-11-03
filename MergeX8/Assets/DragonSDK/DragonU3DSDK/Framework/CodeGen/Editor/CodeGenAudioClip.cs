using System.IO;
using UnityEditor;
using UnityEngine;

public static partial class CodeGen
{
    private static void GenAudioClip()
    {
        var code = "";
        foreach (var obj in Selection.objects)
        {
            if (obj.GetType() != typeof(AudioClip)) continue;
            var ac = (AudioClip) obj;
            code += "    " + GenCode(ac);
            code += "\n";
        }

        if (string.IsNullOrEmpty(code)) return;
        
        var classCode =         
@"public static class SoundNames
{
code}";
        
        SaveFile("Audio", "SoundNames", classCode.Replace("code", code));
    }

    private static string GenCode(AudioClip ac)
    {
        var varName = ac.name.Replace("sfx_", "").ToLower();
        return $"public const string {varName} = \"{ac.name}\";";
    }
}