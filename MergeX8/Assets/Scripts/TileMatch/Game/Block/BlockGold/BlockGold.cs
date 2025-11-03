using System;
using System.Collections.Generic;
using DragonPlus;
using LayoutData;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockGold : Block
    {
        private BlockGoldModel _selfModel;
        private BlockGoldView _selfView;

        public BlockGold(Layer.Layer layer, LayerBlock blockData, int index): base(layer, blockData, index){}
        public override void InitData(Layer.Layer layer, LayerBlock blockData)
        {
            _blockModel = new BlockGoldModel(layer, blockData, this);
            _blockView = new BlockGoldView(this);
            
            _selfModel = (BlockGoldModel)_blockModel;
            _selfView = (BlockGoldView)_blockView;
        }
        public override void BeforeRemoveBlock(List<Block> blocks, bool isRefresh = true)
        {
            base.BeforeRemoveBlock(blocks, isRefresh);
            
            if(_blockState != BlockState.Normal)
                return;
            
            if(_selfModel.IsCollect)
                return;
            
            foreach (var block in blocks)
            {
                if(IgnoreRemoveBlockHandle(block))
                    continue;

                if (block.AreaType != AreaType.SuperBanner)
                {
                    if(_blockModel._blockData.children != null && _blockModel._blockData.children.Contains(block._id))
                        continue;
                }

                _selfModel._brokenNum--;
                _selfView.BreakAnim();

                _selfModel.IsCollect = _selfModel._brokenNum <= 0;
                break;
            }
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
        
        public override void RemoveAnim(Action action)
        {
            if (_blockModel.IsCollect)
            {
                base.RemoveAnim(action);
                return;
            }
            
            if (_selfModel._brokenNum > 0)
            {
                _selfModel._brokenNum = 0;
                _selfView.BreakAnim();
                _selfView.PlayAddCoinEffect();
                
                TileMatchLevelModel.Instance.CollectCoin++;
                
                //Debug.LogError("-----------黄金牌  增加金币-----------");
            }
            else
            {
                //Debug.LogError("-----------黄金牌破碎  不在增加金币-----------");
            }
            base.RemoveAnim(action);
        }
        
        public override void Magic_BeforeRemoveBlock(List<Block> blocks)
        {
            foreach (var block in blocks)
            {
                BeforeRemoveBlock(new List<Block>(){block});
            }
        }
        
        public override bool CanShuffle()
        {
            return false;
        }
        
        public override void StartMagic()
        {  
            base.StartMagic();
            
            if(_selfModel.IsCollect)
                return;
            
            _selfModel._brokenNum = 0;
            _selfView.BreakAnim();
        }
    }
}