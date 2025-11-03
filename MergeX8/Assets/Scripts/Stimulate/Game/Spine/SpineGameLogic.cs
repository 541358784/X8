using Spine.Unity;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Stimulate.Model.Spine
{
    public class SpineGameLogic : MonoBehaviour
    {
        private SkeletonAnimation _skeletonAnimation;

        public SkeletonAnimation skeletonAnimation
        {
            get { return _skeletonAnimation; }
        }
        
        public void OnInit(TableStimulateSpine config)
        {
            _skeletonAnimation = transform.Find(config.spinePath).GetComponent<SkeletonAnimation>();
            
            if(config.defaultAnim.IsEmptyString())
                return;
            
            _skeletonAnimation.AnimationState.SetAnimation(0,config.defaultAnim, true);
            _skeletonAnimation.Update(0);
        }
    }
}