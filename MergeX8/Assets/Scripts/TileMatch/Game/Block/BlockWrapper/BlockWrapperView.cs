using System;
using GamePool;
using Spine.Unity;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockWrapperView : BlockView
    {
        protected SkeletonAnimation _skeletonAnimation;
        protected GameObject _staticImage;
        protected Animator _animator;
        
        public BlockWrapperView(Block block) : base(block)
        {
        }
        
        public override void LoadView(Transform parent)
        {
            _obstaclePoolName = ObjectPoolName.TileMatchBlock_Bag;
            
            base.LoadView(parent);

            _staticImage = _obstacle.transform.Find("bag").gameObject;
            _animator = _obstacle.transform.Find("UncoverAnimParent").GetComponent<Animator>();
            _skeletonAnimation = _obstacle.transform.Find("UncoverAnimParent/Spine").GetComponent<SkeletonAnimation>();

            InitView();
        }
        
        public void InitView()
        {
            _staticImage.gameObject.SetActive(true);
        }

        public override void BreakAnim(Action action = null)
        {
            base.BreakAnim(action);
            
            _staticImage.gameObject.SetActive(false);
            _animator.Play("Tearing_Particle", 0, 0);
            _skeletonAnimation.AnimationState.SetAnimation(0, "Open", false);
            _skeletonAnimation.AnimationState.Update(0);
        }
    }
}