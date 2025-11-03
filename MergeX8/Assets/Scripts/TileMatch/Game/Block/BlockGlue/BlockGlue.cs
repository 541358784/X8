using System;
using System.Collections.Generic;
using DG.Tweening;
using GamePool;
using LayoutData;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public partial class BlockGlue : Block
    {
        private BlockGlueModel _selfModel;
        private BlockGlueView _selfView;

        private Block _linkBlock;
        public BlockGlue(Layer.Layer layer, LayerBlock blockData, int index): base(layer, blockData, index){}
        
        public override void InitData(Layer.Layer layer, LayerBlock blockData)
        {
            _blockModel = new BlockGlueModel(layer, blockData, this);
            _blockView = new BlockGlueView(this);
            _selfModel = (BlockGlueModel)_blockModel;
            _selfView = (BlockGlueView)_blockView;

            _isCanRemove = false;
        }
        private Block LinkBlock
        {
            get
            {
                if (_linkBlock != null)
                    return _linkBlock;
            
                var neighbors = _blockModel._blockData.neighbors.Find(a => a.neighborType == (int)NeighborEnum.Right);
                var neighborsBlock = TileMatchGameManager.Instance.GetBlock(neighbors.id);

                _linkBlock = neighborsBlock;
                return _linkBlock;
            }
        }
        public override bool CanRemove()
        {
            if(_isCanRemove)
            {
                return base.CanRemove();
            }

            return IsInRemoveState() && LinkBlock.IsInRemoveState();
        }

        public override void InitState()
        {
            base.InitState();
            
            if (LinkBlock.IsInRemoveState() || IsInRemoveState())
                _selfView.DoObstacleColor(true, false);
            else
                _selfView.DoObstacleColor(false, false);
        }

        public override void RefreshView(bool isAnim = false)
        {
            base.RefreshView(isAnim);
            
            if(_selfModel.IsCollect)
                return;
            
            if (LinkBlock.IsInRemoveState() || IsInRemoveState())
                _selfView.DoObstacleColor(true, isAnim);
            else
                _selfView.DoObstacleColor(false, isAnim);
        }
        
        public override void StartMagic()
        {  
            base.StartMagic();
            
            _selfView.StartShuffle();
        }
        
        public override void OnPointerEnter()
        {
            if (_blockState != BlockState.Normal && _blockState != BlockState.InCollection)
                return;

            if (!CanRemove())
            {
                Shake();
                return;
            }
            
            if (LinkBlock.GetBlockState() != BlockState.Normal && LinkBlock.GetBlockState() != BlockState.InCollection)
                return;

            OnPointerEnterAnim();
            LinkBlock.OnPointerEnterAnim();
        }
        
        public override void OnPointerExit(bool isRemove, Action action)
        {
            if (_isCanRemove)
            {
                base.OnPointerExit(isRemove, action);
                return;
            }

            if (!isRemove)
            {
                OnPointerExitAnim();
                LinkBlock.OnPointerExitAnim();
            }

            if (isRemove)
                RemoveAnim(action);
        }
        
        public override void RemoveAnim(Action action)
        {
            this._selfModel.IsCollect = true;
            if (_isCanRemove)
            {
                action?.Invoke();
                return;
            }
            
            _selfView.HideObstacle();
            GlueBreakAnim(() =>
            {
                _isCanRemove = true;
                LinkBlock._blockModel.IsCollect = true;
                action?.Invoke();
            });
        }
        
        public override List<Block> PreparationPreprocess()
        {
            if (_isCanRemove)
            {
                return new List<Block>() { this};
            }
            
            return new List<Block>() { this, LinkBlock };
        }
        
        public override void StartShuffle()
        {
            base.StartShuffle();
            if(_selfModel.IsCollect)
                return;
            
            _selfView.StartShuffle();
        }

        public override void StopShuffle()
        {
            base.StopShuffle();
            if(_selfModel.IsCollect)
                return;
            
            _selfView.StopShuffle();
        }  
        
        public override void Magic_BeforeRemoveBlock(List<Block> blocks)
        {
            foreach (var block in blocks)
            {
                if (block == this)
                {
                    _isCanRemove = true;
                    _blockModel.IsCollect = true;
                    _selfView.StartShuffle();
                }
                
                BeforeRemoveBlock(new List<Block>(){block});
            }
        }
        
        public override void Shake()
        {
            Vector3 localPosition = _blockView._root.transform.localPosition;
            Vector3 linkPosition = LinkBlock._blockView._root.transform.localPosition;
            _blockView.Shake();
            LinkBlock._blockView.Shake();
            
            ShakeTweener().OnUpdate(() =>
            {
                Vector3 diffValue = _blockView._root.transform.localPosition - localPosition;
                diffValue.z = 0;
                
                LinkBlock._blockView._root.transform.localPosition = linkPosition+diffValue;
            }).OnComplete(() =>
            {
                LinkBlock._blockView._root.transform.localPosition = linkPosition;
            });
        }
    }
}