using System;
using DG.Tweening;
using GamePool;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockGlueView : BlockView
    {
        private Vector3 _obstacleOrgPosition = Vector3.zero;
        private SpriteRenderer _spriteRenderer;
        
        public BlockGlueView(Block block) : base(block)
        {
        }
        
        public override void LoadView(Transform parent)
        {
            _obstaclePoolName = ObjectPoolName.TileMatchBlock_Glue;
            base.LoadView(parent);
            
            _obstacleOrgPosition = _obstacle.transform.localPosition;
            _spriteRenderer = _obstacle.transform.Find("GlueRenderer").GetComponent<SpriteRenderer>();

            ChangeObstacleOrder();
        }

        public void ChangeObstacleOrder()
        {
            _obstacle.transform.localPosition = new Vector3(_obstacleOrgPosition.x, _obstacleOrgPosition.y, _obstacleOrgPosition.z + BlockModel.offsetZ - 0.000095f);
        }

        public override void BreakAnim(Action action = null)
        {
            action?.Invoke();
        }
        
        public void StartShuffle()
        {
            _spriteRenderer.DOFade(0, 0.2f);
        }

        public void StopShuffle()
        {
            _spriteRenderer.DOFade(1, 0.2f);
        }

        public void DoObstacleColor(bool isShow, bool isAnim = true)
        {
            DOColor(isShow ? Color.white : Color.grey, isAnim);
        }
        
        private void DOColor(Color color, bool isAnim)
        {
            _spriteRenderer.DOKill();
            if (isAnim)
            {
                _spriteRenderer.DOColor(color, 0.2f).SetEase(Ease.Linear);
            }
            else
            {
                _spriteRenderer.color = color;
            }
        }
        
        public override void Destroy()
        {
            base.Destroy();
        }
    }
}