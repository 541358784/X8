using System;
using System.Collections.Generic;
using DragonU3DSDK;
using LayoutData;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockCurtain : Block
    {
        private BlockCurtainModel _selfModel;
        private BlockCurtainView _selfView;

        public BlockCurtain(Layer.Layer layer, LayerBlock blockData, int index): base(layer, blockData, index){}
        
        public override void InitData(Layer.Layer layer, LayerBlock blockData)
        {
            _blockModel = new BlockCurtainModel(layer, blockData, this);
            _blockView = new BlockCurtainView(this);
            
            _selfModel = (BlockCurtainModel)_blockModel;
            _selfView = (BlockCurtainView)_blockView;

            _isCanRemove = _selfModel._isOpen;
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
                    if (_blockModel._blockData.children != null && _blockModel._blockData.children.Contains(block._id))
                    {
                        CheckForceOpen();
                        continue;
                    }
                }

                if (_selfModel._isOpen)
                {
                    List<Block> activeBlocks = TileMatchGameManager.Instance.GetActiveBlocks();
                    activeBlocks.Remove(this);
                    foreach (var removeBlock in blocks)
                    {
                        activeBlocks.Remove(removeBlock);
                    }

                    bool canOpen = false;
                    foreach (var activeBlock in activeBlocks)
                    {
                        if (activeBlock._blockModel._blockData.blockType != (int)BlockTypeEnum.Curtain)
                        {
                            canOpen = true;
                            break;
                        }

                        if (activeBlock.IsActiveBlock() && !((BlockCurtainModel)(activeBlock._blockModel))._isOpen)
                        {
                            foreach (var removeBlock in blocks)
                            {
                                if (activeBlock._blockModel._blockData.children != null && activeBlock._blockModel._blockData.children.Contains(removeBlock._id))
                                {
                                    DebugUtil.LogError("--------帘子 当前解不开了 不做状态改变 特---------------");
                                    continue;
                                }
                                else
                                {
                                    canOpen = true;
                                    break;
                                }
                            }
                            
                           if(canOpen)
                                break;
                        }
                    }

                    if (!canOpen)
                    {
                        DebugUtil.LogError("--------帘子 当前解不开了 不做状态改变---------------");
                        return;
                    }
                }
                _selfModel._isOpen = !_selfModel._isOpen;
                _isCanRemove = _selfModel._isOpen;
                _selfView.BreakAnim();
                break;
            }
        }

        public override void BeforeRecoverBlock(List<Block> blocks, Action action)
        {
            base.BeforeRecoverBlock(blocks, action);
            if(_blockState != BlockState.Normal)
                return;
            
            if(_selfModel.IsCollect)
                return;

            foreach (var block in blocks)
            {
                if (IgnoreRemoveBlockHandle(block))
                    continue;

                if (block.AreaType != AreaType.SuperBanner)
                {
                    if (_blockModel._blockData.children != null && _blockModel._blockData.children.Contains(block._id))
                        continue;
                }

                _selfModel._isOpen = !_selfModel._isOpen;
                _isCanRemove = _selfModel._isOpen;
                _selfView.BreakAnim();
                break;
            }
        }
        
        private void CheckForceOpen()
        {
            if(_isCanRemove)
                return;
            
            if(_blockState != BlockState.Normal)
                return;
            
            if(_selfModel.IsCollect)
                return;
            
            if(_selfModel._isOpen)
                return;
            
            List<Block> activeBlocks = TileMatchGameManager.Instance.GetActiveBlocks();
            activeBlocks.Remove(this);
            bool canOpen = false;
            foreach (var activeBlock in activeBlocks)
            {
                if(!activeBlock.IsActiveBlock())
                    continue;
                    
                if(activeBlock._blockModel._blockData.blockType != (int)BlockTypeEnum.Curtain)
                    return;
                
                if (!((BlockCurtainModel)(activeBlock._blockModel))._isOpen)
                    return;
            }
            
            _selfModel._isOpen = true;
            _isCanRemove = _selfModel._isOpen;
            _selfView.BreakAnim();
            DebugUtil.LogError("--------帘子 当前解不开了 强制解开帘子---------------");

        }

        public override void RemoveAnim(Action action)
        {
            base.RemoveAnim(action);
            
            _selfView.BreakAnim();
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
                BeforeRemoveBlock(new List<Block>(){block});
            }
        }

        public override void Magic_AfterRemoveBlock(List<Block> blocks)
        {
            foreach (var block in blocks)
            {
                AfterRemoveBlock(new List<Block>(){block});
            }
        }
        
        public override void OnPointerEnter()
        {
            if (_blockState != BlockState.Normal)
                return;

            if (!_isCanRemove)
            {
                Shake();
                return;
            }
            
            base.OnPointerEnter();
        }
        
        public override void StartMagic()
        {  
            base.StartMagic();
            
            if(_selfModel.IsCollect)
                return;

            _selfModel._isOpen = true;
            _isCanRemove = _selfModel._isOpen;
            _selfView.BreakAnim();
        }
    }
}