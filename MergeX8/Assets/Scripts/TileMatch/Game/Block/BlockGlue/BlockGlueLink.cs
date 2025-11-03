using System;
using System.Collections.Generic;
using DG.Tweening;
using LayoutData;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockGlueLink : Block
    {
        private Block _linkBlock;
        public BlockGlueLink(Layer.Layer layer, LayerBlock blockData, int index): base(layer, blockData, index){}
        public override void InitData(Layer.Layer layer, LayerBlock blockData)
        {
            _blockModel = new BlockModel(layer, blockData, this);
            _blockView = new BlockView(this);
        }

        private Block LinkBlock
        {
            get
            {
                if (_linkBlock != null)
                    return _linkBlock;
            
                var neighbors = _blockModel._blockData.neighbors.Find(a => a.neighborType == (int)NeighborEnum.Left);
                var neighborsBlock = TileMatchGameManager.Instance.GetBlock(neighbors.id);

                _linkBlock = neighborsBlock;
                return _linkBlock;
            }
        }
        
        public override bool CanRemove()
        {
            if(LinkBlock._isCanRemove)
            {
                return base.CanRemove();
            }

            return base.CanRemove() && LinkBlock.IsInRemoveState();
        }
        
        public override void InitState()
        {
            base.InitState();
            
            if (LinkBlock.IsInRemoveState() || IsInRemoveState())
                ((BlockGlueView)LinkBlock._blockView).DoObstacleColor(true, false);
            else
                ((BlockGlueView)LinkBlock._blockView).DoObstacleColor(false, false);
        }

        public override void RefreshView(bool isAnim = false)
        {
            base.RefreshView(isAnim);
            
            if(_blockModel.IsCollect)
                return;
            
            if (LinkBlock.IsInRemoveState() || IsInRemoveState())
                ((BlockGlueView)LinkBlock._blockView).DoObstacleColor(true, isAnim);
            else
                ((BlockGlueView)LinkBlock._blockView).DoObstacleColor(false, isAnim);
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
            
            if (LinkBlock._isCanRemove)
            {
                base.OnPointerEnter();
                return;
            }
            
            if (LinkBlock.GetBlockState() != BlockState.Normal && LinkBlock.GetBlockState() != BlockState.InCollection)
                return;
            
            base.OnPointerEnter();
            
            LinkBlock.OnPointerEnterAnim();
        }
        
        public override void OnPointerExit(bool isRemove, Action action)
        {
            if (LinkBlock._isCanRemove)
            {
                base.OnPointerExit(isRemove, action);
                return;
            }
            
            if (isRemove)
            {
                LinkBlock.RemoveAnim(action);
            }
            else
            {
                OnPointerExitAnim();
                LinkBlock.OnPointerExitAnim();
            }
        }
        
        public override List<Block> PreparationPreprocess()
        {
            if (LinkBlock._isCanRemove)
            {
                return new List<Block>() { this};
            }
            
            return new List<Block>() {LinkBlock,this };
        }
        
        public override void Magic_BeforeRemoveBlock(List<Block> blocks)
        {
            foreach (var block in blocks)
            {
                if (block == this)
                {
                    _blockModel.IsCollect = true;
                    _isCanRemove = true;
                    LinkBlock._isCanRemove = true;
                    LinkBlock._blockModel.IsCollect = true;
                    ((BlockGlueView)(LinkBlock._blockView)).StartShuffle();
                }
                
                BeforeRemoveBlock(new List<Block>(){block});
            }
        }
        
        public override void StartMagic()
        {  
            base.StartMagic();
            
            ((BlockGlueView)(LinkBlock._blockView)).StartShuffle();
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