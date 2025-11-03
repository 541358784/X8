using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Playables;

public class SpineSkinSwitcherBehaviour : PlayableBehaviour
{
    public SkeletonGraphic skeletonGraphic;
    public string[] skinsToCombine;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (skeletonGraphic == null || skinsToCombine == null || skinsToCombine.Length == 0)
        {
            return;
        }

        // 创建一个新的皮肤
        Skin combinedSkin = new Skin("combined");

        // 获取SkeletonData
        SkeletonData skeletonData = skeletonGraphic.Skeleton.Data;

        // 遍历并添加每个需要组合的皮肤
        foreach (string skinName in skinsToCombine)
        {
            Skin skin = skeletonData.FindSkin(skinName);
            if (skin != null)
            {
                combinedSkin.AddSkin(skin);
            }
            else
            {
                Debug.LogWarning($"Skin '{skinName}' not found!");
            }
        }

        // 应用组合后的皮肤
        skeletonGraphic.Skeleton.SetSkin(combinedSkin);
        
        //skeletonGraphic.AnimationState.ClearTracks();
        
        skeletonGraphic.Skeleton.SetSlotsToSetupPose();
        skeletonGraphic.AnimationState.Apply(skeletonGraphic.Skeleton);
        
        // // 最后更新骨骼世界变换
        // skeletonGraphic.Skeleton.UpdateWorldTransform();
        // skeletonGraphic.LateUpdate(); // 强制刷新显示
    }
}