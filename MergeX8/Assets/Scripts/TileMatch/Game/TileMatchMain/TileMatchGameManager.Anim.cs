
using System;
using System.Collections.Generic;
using DG.Tweening;
using Framework;
using GamePool;
using UnityEngine;

namespace TileMatch.Game
{
    public partial class TileMatchGameManager
    {
        private Vector2 _layoutShowPosition = new Vector2(15, 0);
        private Vector3 _bannerShowPosition = new Vector3(0, -10f, 0);
        private float _bornTime = 0.5f;
        
        private void PlayShowAnim(Action action)
        {
            UIRoot.Instance.EnableEventSystem = false;

            TileMatchMainController controller = UIManager.Instance.GetOpenedUIByPath<TileMatchMainController>(UINameConst.TileMatchMain);
            if(controller != null)
                controller.AppearAnim();
            
            _layoutRoot.transform.localPosition = _offset + _layoutShowPosition;
            _collectBanner.transform.position = _collectBannerWordPos + _bannerShowPosition;
            
            _layoutRoot.transform.DOLocalMove(_offset, _bornTime).SetEase(Ease.OutBack);
            _collectBanner.transform.DOMove(_collectBannerWordPos, _bornTime).SetEase(Ease.OutBack).OnComplete(() =>
            {
                UIRoot.Instance.EnableEventSystem = true;
                action?.Invoke();
            });
        }

        public void PlayHideAnim(Action action)
        {
            UIRoot.Instance.EnableEventSystem = false;
            
            foreach (var block in _collectBannerList)
            {
                block._blockView._root.transform.SetParent(_collectBanner.transform);
            }
            
            TileMatchMainController controller = UIManager.Instance.GetOpenedUIByPath<TileMatchMainController>(UINameConst.TileMatchMain);
            if(controller != null)
                controller.DisappearAnim();
            
            _layoutRoot.transform.DOLocalMove(_offset + _layoutShowPosition, _bornTime).SetEase(Ease.InBack);
            _collectBanner.transform.DOMove(_collectBannerWordPos + _bannerShowPosition, _bornTime).SetEase(Ease.InBack).OnComplete(() =>
            {
                UIRoot.Instance.EnableEventSystem = true;
                action?.Invoke();
            });
        }

        private void WaitGameEndAnim(List<Block.Block> blocks, Action action)
        {
            _waitGameEndAnim = true;
            foreach (var block in blocks)
            {
                block._blockView._root.transform.DOKill();
            }

            Vector3 targetPosition = blocks[0]._blockView._root.transform.position;
            
            var seq = DOTween.Sequence();
            seq.Insert(0, blocks[0]._blockView._root.transform.DOMove(targetPosition+Vector3.up*3f, 0.25f).SetLoops(2, LoopType.Yoyo));
            seq.Insert(0, blocks[0]._blockView._root.transform.DORotate(new Vector3(0,0,-360), 0.3f, RotateMode.FastBeyond360));
            seq.Insert(0.15f, blocks[2]._blockView._root.transform.DOMove(targetPosition, 0.4f).SetEase(Ease.InBack));
            seq.Insert(0.15f, blocks[2]._blockView._root.transform.DOScale(new Vector3(1.2f, 1), 0.1f));
            seq.Insert(0.25f, blocks[1]._blockView._root.transform.DOMove(targetPosition, 0.2f).SetEase(Ease.InBack));
            seq.Insert(0.25f, blocks[1]._blockView._root.transform.DOScale(new Vector3(1.2f, 1), 0.1f));
            seq.onComplete = () =>
            {
                GameObject effect = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.TileMatchBlock_Blast);
                effect.transform.position = targetPosition;

                foreach (var block in blocks)
                {
                    block._blockView._root.gameObject.SetActive(false);
                }
                
                CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1f, () =>
                {
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock_Blast, effect);
                }));
                
                action?.Invoke();
            };
        }
    }
}