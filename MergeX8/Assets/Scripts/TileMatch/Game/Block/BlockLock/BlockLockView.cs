using System;
using DragonPlus;
using GamePool;
using Spine.Unity;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockLockView : BlockView
    {
        protected SkeletonAnimation _skeletonAnimation;
        private GameObject _breakEffectObj;
        
        public BlockLockView(Block block) : base(block)
        {
        }

        public override void LoadView(Transform parent)
        {
            _obstaclePoolName = ObjectPoolName.TileMatchBlock_Rope;
            
            base.LoadView(parent);

            _skeletonAnimation = _obstacle.transform.Find("Spine").GetComponent<SkeletonAnimation>();
            
            _skeletonAnimation.AnimationState.SetAnimation(0, "idle", false);
            _skeletonAnimation.AnimationState.Update(0);
            
            _breakEffectObj = _obstacle.transform.Find("FX").gameObject;
            _breakEffectObj.gameObject.SetActive(false);
        }

        public override void BreakAnim(Action action = null)
        {
            var position =  _obstacle.transform.position;
            position.z = -30f;
            _obstacle.transform.position = position;
            _skeletonAnimation.AnimationState.SetAnimation(0, "break", false);
            _skeletonAnimation.AnimationState.Update(0);
            
            _breakEffectObj.gameObject.SetActive(true);
            
            AudioManager.Instance.PlaySound(36+TileMatchRoot.AudioDistance);
            action?.Invoke();
        }
        
        public void StartShuffle()
        {
            if(_block._blockModel.IsCollect)
                return;
            
        }

        public void StopShuffle()
        {
            if(_block._blockModel.IsCollect)
                return;
            
        }

        public override void Shake()
        {
            if(_skeletonAnimation == null)
                return;
            
            _skeletonAnimation.AnimationState.SetAnimation(0, "touch", false);
            _skeletonAnimation.AnimationState.Update(0);
        }
    }
}