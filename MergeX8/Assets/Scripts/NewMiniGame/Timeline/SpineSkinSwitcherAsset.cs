using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Spine.Unity;

[System.Serializable]
public class SpineSkinSwitcherAsset : PlayableAsset, ITimelineClipAsset
{
    public SpineSkinSwitcherBehaviour template = new SpineSkinSwitcherBehaviour();
    public ExposedReference<SkeletonGraphic> skeletonGraphic;
    public string[] skinsToCombine;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SpineSkinSwitcherBehaviour>.Create(graph, template);
        SpineSkinSwitcherBehaviour clone = playable.GetBehaviour();
        clone.skeletonGraphic = skeletonGraphic.Resolve(graph.GetResolver());
        clone.skinsToCombine = skinsToCombine;
        return playable;
    }
}