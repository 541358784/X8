using UnityEngine;

public static class AnimatorUtils
{
    static public AnimationClip GetClipInfo(Animator animator, string clipName)
    {
        var clips = animator?.runtimeAnimatorController?.animationClips;
        if (clips != null)
        {
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name.Equals(clipName))
                {
                    return clips[i];
                }
            }
        }

        return default(AnimationClip);
    }
}