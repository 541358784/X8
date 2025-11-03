using Spine.Unity;
using Spine.Unity.Playables;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MiniGame
{
    public partial class MiniGameModel
    {
        public void SetGenericBindingBySpine(PlayableDirector director)
        {
            if(director == null)
                return;
            
            TimelineAsset timelineAsset = director.playableAsset as TimelineAsset;
            if (timelineAsset == null) 
                return;
            
            foreach (var track in timelineAsset.GetOutputTracks()) 
            {
                if (track is SpineAnimationStateGraphicTrack spineTrack) 
                {
                    string trackName = ((SpineAnimationStateGraphicTrack)track).trackName;
                    if(trackName.Equals("Filter"))
                        continue;
     
                    SkeletonGraphic skeletonGraphic =  director.GetComponentInChildren<SkeletonGraphic>();
                    if (skeletonGraphic == null) 
                        continue;
                    
                    director.SetGenericBinding(spineTrack, skeletonGraphic);
                }
            }
        }

        public void SetGenericBindingByAnimator(PlayableDirector director)
        {
            if(director == null)
                return;
            
            TimelineAsset timelineAsset = director.playableAsset as TimelineAsset;
            if (timelineAsset == null) 
                return;
     
            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track is AnimationTrack)
                {
                    if(!track.name.Contains("GB_"))
                        continue;
        
                    var names = track.name.Split("_");
                    if(names.Length < 2)
                        continue;
                    
                    var child = director.transform.Find("Root/"+names[1]);
                    if(child == null)
                        continue;
        
                    var animator = child.gameObject.GetComponent<Animator>();
                    if(animator == null)
                        continue;
                    
                    director.SetGenericBinding(track, animator);
                }
            }
        }
    }
}