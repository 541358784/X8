using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static partial class CodeGen
{
    private static void GenAnimatorController()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj.GetType() != typeof(AnimatorController)) continue;
            var ac = (AnimatorController) obj;
            GenCode(ac);
        }
    }

    private static void GenCode(AnimatorController ac)
    {
        if (ac.animationClips.Length == 0) return;
        
        var className = GetCamelCaseName(ac.name);
        className.Append("AnimationName");
        
        var code = 
            @"/// <summary>
/// AnimatorController : ANIMATOR_CONTROLLER
/// </summary>
public static class CLASS_NAME
{
CLIP_NAMES}";
        code = code.Replace("ANIMATOR_CONTROLLER", ac.name);
        code = code.Replace("CLASS_NAME", className.ToString());

        var clipNames = "";
        foreach (var clip in ac.animationClips)
        {
            var memberName = GetCamelCaseName(clip.name.Replace(ac.name, ""));
            decimal i;
            if (memberName.Length == 0 || decimal.TryParse(memberName.ToString(), out i)) memberName = GetCamelCaseName(clip.name);
            clipNames += $"    /// <summary>\n";
            clipNames += $"    /// {clip.name}\n";
            clipNames += $"    /// </summary>\n";
            clipNames += $"    public const string {memberName} = \"{clip.name}\";\n";
        }
        
        code = code.Replace("CLIP_NAMES", clipNames);
        
        SaveFile("AnimationName", className.ToString(), code);
    }
}