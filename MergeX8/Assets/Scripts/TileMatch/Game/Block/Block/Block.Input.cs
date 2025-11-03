using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public partial class Block : IOnPointerHandle
    {
        public virtual void OnPointerEnter()
        {
            if (_blockState != BlockState.Normal && _blockState != BlockState.InCollection)
                return;

            OnPointerEnterAnim();
        }
        
        public virtual void OnPointerExit(bool isRemove, Action action)
        {
            if (_blockState != BlockState.Normal && _blockState != BlockState.InCollection)
                return;
            
            if (isRemove)
            {
                ScaleNormalizeAnim();
                RemoveAnim(action);
            }
            else
            {
                OnPointerExitAnim();
            }
        }

        public void OnPointerEnterAnim()
        {
            var localPos = _blockModel.localPosition;
            localPos.y += 0.25f;
            //_blockView._root.transform.DOScale(new Vector3(1.05f, 1.05f, 1.0f), 0.07f);
            _blockView._root.transform.DOLocalMoveY(localPos.y, 0.07f).OnComplete(() =>
            {
                localPos.z -= 10f; 
                _blockView._root.transform.localPosition = localPos;
            });
        }
        
        public void ScaleNormalizeAnim(float time = 0.07f)
        {
            _blockView._root.transform.DOScale(Vector3.one, time);
        }
        
        public void OnPointerExitAnim()
        {
            ScaleNormalizeAnim();
            Vector3 localPosition = _blockView._root.transform.localPosition;
            localPosition.z = _blockModel.localPosition.z;
            _blockView._root.transform.localPosition = localPosition;
            _blockView._root.transform.DOLocalMove(_blockModel.localPosition, 0.07f);
        }
    }
}