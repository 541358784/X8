using System;
using DG.Tweening;
using DragonPlus;
using Framework;
using GamePool;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public partial class BlockGlue
    {
        protected GameObject _breakAnimObject;
        protected Coroutine _breakAnimCoroutine;
        protected Coroutine _breakAnimEndCoroutine;
        private void GlueBreakAnim(Action action)
        {
            _blockView.SetColliderEnable(false);
            LinkBlock._blockView.SetColliderEnable(false);
            
            _blockView._root.transform.DOKill();
            LinkBlock._blockView._root.transform.DOKill();
            
            _breakAnimObject = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.TileMatchBlock_Glue_Break);
            CommonUtils.AddChild(_blockView._root.transform.parent, _breakAnimObject.transform);

            Vector3 animPos = _blockView._root.transform.localPosition;
            animPos.z = -40;
            
            _breakAnimObject.gameObject.transform.localPosition = animPos;

            AudioManager.Instance.PlaySound(34+TileMatchRoot.AudioDistance);
            CommonUtils.AddChild(_breakAnimObject.transform.Find("TileView L/Parent"), _blockView._root.transform);
            CommonUtils.AddChild(_breakAnimObject.transform.Find("TileView R/Parent"), LinkBlock._blockView._root.transform);

            _breakAnimCoroutine = CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(CommonUtils.GetAnimTime(_breakAnimObject.GetComponent<Animator>(), "GlueBreakAnimation"), () =>
            {
                _blockView._root.transform.parent = _breakAnimObject.transform.parent;
                LinkBlock._blockView._root.transform.parent = _breakAnimObject.transform.parent;

                _breakAnimCoroutine = null;
                PlayBreakEffect();
                
                action?.Invoke();
            }));

            _breakAnimEndCoroutine = CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(2f, () =>
            {
                if (_breakAnimObject != null)
                {
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock_Glue_Break, _breakAnimObject);
                    _breakAnimObject = null;
                }
                _breakAnimEndCoroutine = null;
            }));
        }

        public override void Destroy()
        {
            if (_breakAnimObject != null)
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock_Glue_Break, _breakAnimObject);
                _blockView._root.transform.parent = _breakAnimObject.transform.parent;
                _linkBlock._blockView._root.transform.parent = _breakAnimObject.transform.parent;
            }

            if (_breakAnimCoroutine != null)
                CoroutineManager.Instance.StopCoroutine(_breakAnimCoroutine);
            
            if (_breakAnimEndCoroutine != null)
                CoroutineManager.Instance.StopCoroutine(_breakAnimEndCoroutine);
            
            _breakAnimCoroutine = null;
            _breakAnimEndCoroutine = null;
            _breakAnimObject = null;
            base.Destroy();
        }

        private void PlayBreakEffect()
        {
            var leftEffect = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.TileMatchBlock_Glue_BreakEffect);
            var rightEffect = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.TileMatchBlock_Glue_BreakEffect);
            
            leftEffect.transform.Find("TileView L").gameObject.SetActive(true);
            rightEffect.transform.Find("TileView R").gameObject.SetActive(true);
            
            CommonUtils.AddChild(_blockView._root.transform, leftEffect.transform);
            CommonUtils.AddChild(LinkBlock._blockView._root.transform, rightEffect.transform);
            
            CoroutineManager.Instance.StartCoroutine(CommonUtils.PlayAnimation(leftEffect.transform.Find("TileView L").gameObject.GetComponent<Animator>(), "GlueBreakLeft", "", ()=>
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock_Glue_BreakEffect, leftEffect);
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock_Glue_BreakEffect, rightEffect);
            }));
        }
    }
}