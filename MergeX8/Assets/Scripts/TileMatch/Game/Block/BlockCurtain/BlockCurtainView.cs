using System;
using DragonPlus;
using GamePool;
using Spine.Unity;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockCurtainView : BlockView
    {
        private static float _playAudioTime = 0;
        
        protected SkeletonAnimation _skeletonAnimation;
        
        public BlockCurtainView(Block block) : base(block)
        {
        }   
        
        public override void LoadView(Transform parent)
        {
            _obstaclePoolName = ObjectPoolName.TileMatchBlock_Curtain;
            
            base.LoadView(parent);

            _skeletonAnimation = _obstacle.transform.Find("Spine").GetComponent<SkeletonAnimation>();
            InitView();
        }

        private void InitView()
        {
            ChangeSpineAnim(false);
        }
        public override void BreakAnim(Action action = null)
        {
            if (_block._blockModel.IsCollect)
            {
                _skeletonAnimation.transform.parent.gameObject.SetActive(false);
                action?.Invoke();
                return;
            }

            if (Time.realtimeSinceStartup - _playAudioTime > 0.1f)
            {
                _playAudioTime = Time.realtimeSinceStartup;
                AudioManager.Instance.PlaySound(35+TileMatchRoot.AudioDistance);
            }
            
            ChangeSpineAnim(true);
            action?.Invoke();
        }

        private void ChangeSpineAnim(bool isAnim)
        {
            bool isOpen = ((BlockCurtainModel)_block._blockModel)._isOpen;
            string animName = !isOpen ? "close" : "open";
            
            var trackEntry = _skeletonAnimation.AnimationState.SetAnimation(0, animName, false);
            _skeletonAnimation.AnimationState.Update(0);
            trackEntry.TrackTime = isAnim ? 0 : trackEntry.Animation.Duration;
        }
        public void StartShuffle()
        {
            if(!((BlockCurtainModel)_block._blockModel)._isOpen)
                _skeletonAnimation.gameObject.SetActive(false);
        }

        public void StopShuffle()
        {
            if(!((BlockCurtainModel)_block._blockModel)._isOpen)
                _skeletonAnimation.gameObject.SetActive(true);
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