using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static partial class CodeGen
{
    private static void GenAnimationClip()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj.GetType() != typeof(AnimationClip)) continue;
            var ac = (AnimationClip) obj;
            GenCode(ac);
        }
    }

    private static void GenCode(AnimationClip ac)
    {
        if (ac.events.Length == 0) return;
        
        var code = 
@"/// <summary>
/// AnimationClipName
/// </summary>
interface interfaceName : CodeGenAnimationEvent
{
events}";
        code = code.Replace("AnimationClipName", ac.name);
        
        var interfaceName = GetCamelCaseName(ac.name);
        interfaceName.Append("AnimationEvent");
        code = code.Replace("interfaceName", interfaceName.ToString());

        var events = "";
        foreach (var e in ac.events)
        {
            events += $"    /// <summary>\n";
            events += $"    /// {ac.name} *{e.functionName}*\n";
            events += $"    /// </summary>\n";
            events += $"    void {e.functionName}();\n";
        }
        code = code.Replace("events", events);
        
        SaveFile("AnimationEvent", interfaceName.ToString(), code);
    }
}