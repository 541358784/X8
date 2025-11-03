using System;
using System.Collections.Generic;
using DragonPlus;
using Framework;
using GamePool;
using Spine.Unity;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockGrassView : BlockView
    {
        protected SkeletonAnimation _skeletonAnimation;
        
        private List<GameObject> _particles = new List<GameObject>();
        private List<GameObject> _stages = new List<GameObject>();
        
        public BlockGrassView(Block block) : base(block)
        {
        }
        
        public override void LoadView(Transform parent)
        {
            _obstaclePoolName = ObjectPoolName.TileMatchBlock_Leaves;
            
            base.LoadView(parent);

            _skeletonAnimation = _obstacle.transform.Find("Spine").GetComponent<SkeletonAnimation>();
            _skeletonAnimation.gameObject.SetActive(true);

            for (int i = 1; i <= 2; i++)
            {
                _particles.Add(_obstacle.transform.Find("GrassParticleParent"+i).gameObject);
                _stages.Add(_obstacle.transform.Find("Stage"+i).gameObject);
            }
            
            InitView();
        }

        public void InitView()
        {
            _particles.ForEach(a=>a.gameObject.SetActive(false));
            _stages.ForEach(a=>a.gameObject.SetActive(false));
            _stages[0].gameObject.SetActive(true);
        }

        public override void BreakAnim(Action action = null)
        {
            int brokenNum = ((BlockGrassModel)_block._blockModel)._brokenNum;
            int index = brokenNum == 1 ? 0 : 1;
            
            if(index == 0)
                AudioManager.Instance.PlaySound(28+TileMatchRoot.AudioDistance);
            else
                AudioManager.Instance.PlaySound(29+TileMatchRoot.AudioDistance);
            
            _particles.ForEach(a=>a.gameObject.SetActive(false));
            _stages.ForEach(a=>a.gameObject.SetActive(false));
            _particles[index].gameObject.SetActive(true);
            if(brokenNum == 1)
                _stages[index+1].gameObject.SetActive(true);

            ChangeSpine();
            action?.Invoke();
        }
        
        public override void Shake()
        {
            if(_skeletonAnimation == null)
                return;

            ChangeSpine();
        }

        private void ChangeSpine(bool isAnim = true)
        {
            int brokenNum = ((BlockGrassModel)_block._blockModel)._brokenNum;
            string animName = brokenNum == 2 ? "select1" : "select2";
            
            var trackEntry = _skeletonAnimation.AnimationState.SetAnimation(0, animName, false);
            _skeletonAnimation.AnimationState.Update(0);
            
            if(brokenNum == 0)
                _skeletonAnimation.gameObject.SetActive(false);
        }
        
        public void StartShuffle()
        {
        }

        public void StopShuffle()
        {
        }
    }
}