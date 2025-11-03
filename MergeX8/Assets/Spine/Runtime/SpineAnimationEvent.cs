using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace Spine.Unity
{
    public class SpineAnimationEvent : MonoBehaviour
    {
        private SkeletonAnimation _SkeletonAnimation;
    
        // Start is called before the first frame update
        void Awake()
        {
            _SkeletonAnimation = GetComponent<SkeletonAnimation>();

        }

        public void PlaySpineAnimation(string animationname)
        {
            if (_SkeletonAnimation)
            {
                _SkeletonAnimation.loop = false;
                _SkeletonAnimation.AnimationName = null;
                _SkeletonAnimation.AnimationName = animationname;
                //_SkeletonAnimation.state.SetAnimation(0, animationname, false);
            }
        }
    
    
        public void PlaySpineAnimationLoop(string animationname)
        {
            if (_SkeletonAnimation)
            {
                _SkeletonAnimation.loop = true;
                _SkeletonAnimation.AnimationName = null;
                _SkeletonAnimation.AnimationName = animationname;
                //_SkeletonAnimation.state.SetAnimation(0, animationname, true);
            }
        }
    }    
    
}

