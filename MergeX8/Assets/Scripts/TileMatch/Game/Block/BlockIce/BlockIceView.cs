using System;
using System.Collections.Generic;
using DragonPlus;
using Framework;
using GamePool;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockIceView : BlockView
    {
        private List<Animator> _stageAnimators = new List<Animator>();
        private List<GameObject> _stageBase = new List<GameObject>();
        private Animator _animator;
        private Coroutine _coroutine;
        
        public BlockIceView(Block block) : base(block)
        {
        }
        
        public override void LoadView(Transform parent)
        {
            _obstaclePoolName = ObjectPoolName.TileMatchBlock_Ice;
            
            base.LoadView(parent);

            for (int i = 1; i <= 4; i++)
            {
                _stageAnimators.Add(_obstacle.transform.Find("IceStage" + i).GetComponent<Animator>());
                _stageBase.Add(_obstacle.transform.Find("BaseAnimator/BaseGrp/Base"+i).gameObject);
            }

            _animator = _obstacle.transform.Find("BaseAnimator").GetComponent<Animator>();
            InitView();
        }

        private void InitView()
        {
            _stageAnimators.ForEach(a=>a.gameObject.SetActive(false));
            _stageBase.ForEach(a=>a.gameObject.SetActive(false));
            _stageBase[0].gameObject.SetActive(true);
        }
        
        public override void BreakAnim(Action action = null)
        { 
            int orgBrokenNum = ((BlockIceModel)_block._blockModel)._orgBrokenNum;
            int brokenNum = ((BlockIceModel)_block._blockModel)._brokenNum;

            _stageAnimators.ForEach(a=>a.gameObject.SetActive(false));
            _stageBase.ForEach(a=>a.gameObject.SetActive(false));

            int num = orgBrokenNum - brokenNum;
            
            int stageIndex = 0;
            switch (num)
            {
                case 1:
                {
                    stageIndex = 0;
                    AudioManager.Instance.PlaySound(31+TileMatchRoot.AudioDistance);
                    break;
                }
                case 2:
                {
                    stageIndex = 1;
                    AudioManager.Instance.PlaySound(32+TileMatchRoot.AudioDistance);
                    break;
                }
                case 3:
                {
                    stageIndex = 2;
                    AudioManager.Instance.PlaySound(33+TileMatchRoot.AudioDistance);
                    break;
                }
                case 4:
                {
                    stageIndex = 3;
                    AudioManager.Instance.PlaySound(33+TileMatchRoot.AudioDistance);
                    break;
                }
            }

            if (brokenNum == 0 && orgBrokenNum == 3)
            {
                _stageAnimators[stageIndex+1].gameObject.SetActive(true);
            }
            else
            {
                if(stageIndex+1 < _stageBase.Count)
                    _stageBase[stageIndex+1].gameObject.SetActive(true);
            }
            _stageAnimators[stageIndex].gameObject.SetActive(true);
            
            action?.Invoke();
        }
        
        public void StartShuffle()
        {
        }

        public void StopShuffle()
        {
        }
        
        public override void Shake()
        {
            if(_animator == null)
                return;

            _animator.Play("BaseGrpStage1Anim", 0, 0);
        }
    }
}