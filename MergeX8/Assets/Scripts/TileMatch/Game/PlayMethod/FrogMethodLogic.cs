using System;
using DG.Tweening;
using GamePool;
using Spine.Unity;
using UnityEngine;

namespace TileMatch.Game.PlayMethod
{
    public class FrogMethodLogic : MonoBehaviour
    {
        public Block.Block _block;

        private float _offsetZ = 0.009f;

        public bool _isJumping = false;

        private SkeletonAnimation _skeletonAnimation;
        private void Awake()
        {
            _skeletonAnimation = transform.Find("icon").GetComponent<SkeletonAnimation>();
            
            _skeletonAnimation.AnimationState.SetAnimation(0, "normal", true);
            _skeletonAnimation.AnimationState.Update(0);
        }

        private void OnEnable()
        {
            if (_skeletonAnimation != null)
            {
                _skeletonAnimation.AnimationState.SetAnimation(0, "normal", true);
                _skeletonAnimation.AnimationState.Update(0);
            }
        }

        public void Init(Block.Block block)
        {
            _block = block;

            CommonUtils.AddChild(block._blockView._root.transform, transform);
            transform.localPosition = new Vector3(0,0,block._blockModel.localPosition.z-_offsetZ);
            block._isCanRemove = false;
            
            _isJumping = false;
            transform.DOKill();
        }

        public void Jump(Block.Block jumpBlock, Action jumpAction)
        {
            jumpBlock._isCanRemove = false;
            
            Vector3 formPos =_block._blockModel.position;
            formPos.z = -20;

            Vector3 endPos = jumpBlock._blockModel.position;
            endPos.z = -20;
            
            transform.transform.SetParent(jumpBlock._blockView._root.transform);
            
            transform.position = formPos;
            transform.localScale = Vector3.one;

            _isJumping = true;
            
            float angle = CalculationAngle(formPos, endPos);
            _skeletonAnimation.AnimationState.SetAnimation(0, "jump", false);
            _skeletonAnimation.AnimationState.Update(0);

            this.transform.DOLocalRotate(new Vector3(0, 0, angle), 0.5f * 0.5f);
            transform.DOMove(endPos, 0.5f).OnComplete(() =>
            {
                transform.localPosition = new Vector3(0,0,jumpBlock._blockModel.localPosition.z-_offsetZ);
                _isJumping = false;
                
                _skeletonAnimation.AnimationState.SetAnimation(0, "normal", true);
                _skeletonAnimation.AnimationState.Update(0);
                
                jumpAction?.Invoke();
            });

            _block = jumpBlock;
        }

        public void FrogDie()
        {
            _block._isCanRemove = true;
            
            Vector3 formPos =_block._blockModel.position;
            formPos.z = -50;

            Vector3 endPos = formPos;
            endPos.y -= 15f;
            endPos.z = -50;
            
            transform.SetParent(null);
            
            float dis = Vector3.Distance(formPos, endPos);
            float angle = CalculationAngle(formPos, endPos);
            
            float jumpSpeed = 3f;
            float height = 1.2f;
            float moveTime = dis/jumpSpeed;

            moveTime = Mathf.Min(moveTime, 0.5f);
            moveTime = Mathf.Max(moveTime, 1f);
            
            Vector3[] path = new Vector3[3];
            path[0] = formPos;
            path[1] = formPos + (Vector3.up * height);
            path[2] = endPos;

            transform.position = formPos;
            transform.localScale = Vector3.one;
            
            _skeletonAnimation.AnimationState.SetAnimation(0, "start", false);
            _skeletonAnimation.AnimationState.Update(0);

            StartCoroutine(CommonUtils.DelayWork(0.16f, () =>
            {
                _skeletonAnimation.AnimationState.SetAnimation(0, "end", true);
                _skeletonAnimation.AnimationState.Update(0);
            }));
            
            float changeTime = 0.1f;
            var sq = DOTween.Sequence();
            sq.Insert(0, transform.DOMove(path[1], changeTime));
            //sq.Insert(0,transform.DOScale(2f, changeTime));
            sq.Insert(changeTime, transform.DOLocalRotate(new Vector3(0,0,angle), 0.5f));
            sq.Insert(changeTime, transform.DOMove(path[2], moveTime));
            sq.SetEase(Ease.InQuad);
            sq.OnComplete(() =>
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock_Frog, gameObject);
                _isJumping = false;
            });
        }
        
        private float CalculationAngle(Vector3 starPos, Vector3 endPos)
        {
            var dirVector = endPos-starPos;

            float angle = Mathf.Atan2(dirVector.y, dirVector.x) * Mathf.Rad2Deg+270;

            return angle;
        }

        public bool Unbind(Block.Block block)
        {
            if(block != _block)
                return false;
            
            block._isCanRemove = true;
            transform.SetParent(null);
            Vector3 formPos =_block._blockModel.position;
            transform.position = formPos;
            transform.localScale = Vector3.one;

            return true;
        }
    }
}