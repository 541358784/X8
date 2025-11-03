using UnityEditor;

public static partial class CodeGen
{
    [MenuItem("Assets/CodeGen")]
    public static void Gen()
    {
        GenPrefab();
        GenAnimatorController();
        GenAnimationClip();
        GenAudioClip();
    }
}