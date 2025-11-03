using System;
using DragonPlus;
using Framework;
using GamePool;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockBombView : BlockView
    {
        private TextMeshPro _countText;
        private SkeletonAnimation _skeletonAnimation;
        private GameObject _staticImage;
        private Animator _animator;
        
        public BlockBombView(Block block) : base(block)
        {
        }
        
        public override void LoadView(Transform parent)
        {
            _obstaclePoolName = ObjectPoolName.TileMatchBlock_Bomb_Break;
            base.LoadView(parent);

            _animator = _obstacle.gameObject.GetComponent<Animator>();
            _skeletonAnimation = _obstacle.transform.Find("Parent/BombAnim/Spine GameObject (TB Blocker Frog and bomb_MergedC)").GetComponent<SkeletonAnimation>();
            _staticImage = _obstacle.transform.Find("Parent/StaticImg").gameObject;
            _countText = _obstacle.transform.Find("Parent/CounterParent/Counter").GetComponent<TextMeshPro>();

            UpdateView(_block.GetBlockState(), true);
        }

        public void UpdateView(BlockState state, bool isInit = false)
        {
            if(_block._blockModel.IsCollect)
                return;
            
            switch (state)
            {
                case BlockState.Overlap:
                {
                    _staticImage.gameObject.SetActive(true);
                    _skeletonAnimation.gameObject.SetActive(false);
                    PlayAnimation("BombIdle");
                    break;
                }
                case BlockState.Normal:
                {
                    if (isInit)
                    {
                        _staticImage.gameObject.SetActive(false);
                    }
                    else
                    {
                        CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.1f, () =>
                        {
                            _staticImage.gameObject.SetActive(false);
                        }));
                    }
                    _skeletonAnimation.gameObject.SetActive(true);
                    _skeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
                    _skeletonAnimation.AnimationState.Update(0);
                    break;
                }
            }
            
            int brokenNum = ((BlockBombModel)_block._blockModel)._brokenNum;
            _countText.text = brokenNum.ToString();
        }
        
        public void UpdateView()
        {
            int brokenNum = ((BlockBombModel)_block._blockModel)._brokenNum;
            _countText.text = brokenNum.ToString();

            if (brokenNum == 1)
            {
                _skeletonAnimation.AnimationState.SetAnimation(0, "Jumping", true);
                _skeletonAnimation.AnimationState.Update(0);
            }
        }

        public void Bomb(Action action)
        {
            UIRoot.Instance.EnableEventSystem = false;
            
            _skeletonAnimation.AnimationState.SetAnimation(0, "Burst", false);
            _skeletonAnimation.AnimationState.Update(0);

            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.08f, () =>
            {
                PlayAnimation("BombAnimation");

                CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1f, () =>
                {
                    AudioManager.Instance.PlaySound(37+TileMatchRoot.AudioDistance);
                    _icon.gameObject.SetActive(false);
                }));
                
                CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(2f, () =>
                {
                    UIRoot.Instance.EnableEventSystem = true;
                    action?.Invoke();
                }));
            }));
        }
        public void StartShuffle()
        {
            // _bombObj.gameObject.SetActive(false);
        }

        public void StopShuffle()
        {     
            // _bombObj.gameObject.SetActive(true);
        }

        public void ShowIcon()
        {
            _icon.gameObject.SetActive(true);
        }
        public void HideBomb()
        {
            PlayAnimation("Out");
            
            _staticImage.gameObject.SetActive(false);
            
            int brokenNum = ((BlockBombModel)_block._blockModel)._brokenNum;
            string spineName = "IdleOut";
            if (brokenNum == 1)
                spineName = "JumpingOut";
            
            _skeletonAnimation.AnimationState.SetAnimation(0, spineName, false);
            _skeletonAnimation.AnimationState.Update(0);
        }
        public void PlayAnimation(string animName, Action action = null)
        {
            if (action == null)
            {
                _animator.Play(animName, 0, 0);
            }
            {
                CoroutineManager.Instance.StartCoroutine(CommonUtils.PlayAnimation(_animator, animName, "", action));
            }
        }
    }
}