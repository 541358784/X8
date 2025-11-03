using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK;
using LayoutData;
using TileMatch.Event;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockLock : Block
    {
        private bool _isPreprocessRemove = false;
        private BlockLockModel _selfModel;
        private BlockLockView _selfView;
        
        public BlockLock(Layer.Layer layer, LayerBlock blockData, int index): base(layer, blockData, index){}
        public override void InitData(Layer.Layer layer, LayerBlock blockData)
        {
            _blockModel = new BlockLockModel(layer, blockData, this);
            _blockView = new BlockLockView(this);

            _selfModel = (BlockLockModel)_blockModel;
            _selfView = (BlockLockView)_blockView;
            
            _isCanRemove = false;
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
        
        public override void BeforeRemoveBlock(List<Block> blocks, bool isRefresh = true)
        {
            base.BeforeRemoveBlock(blocks, isRefresh);
            
            if (!IsInActiveState())
                return;

            if (_isCanRemove)
                return;
            
            foreach (var block in blocks)
            {
                if(block.AreaType == AreaType.SuperBanner)
                    continue;
                
                if (_blockModel._blockData.neighbors.Find(a => (a.neighborType == (int)NeighborEnum.Left || a.neighborType == (int)NeighborEnum.Right) && a.id == block._id) == null)
                    continue;
            
                CheckUnlock();
            }
        }
        
        private bool CanBeActive(int filterId, bool ignoreCanRemove = false)
        {
            List<int> filter = new List<int>();
            filter.Add(filterId);
            
            return CanBeActive(filter, ignoreCanRemove);
        }

        public override bool CanBeActive(List<int> filter, bool ignoreCanRemove = false)
        {
            if (!IsInActiveState())
                return false;
            
            if (!ignoreCanRemove && (_isCanRemove || _isPreprocessRemove))
                return true;
            
            foreach (var neighbors in _blockModel._blockData.neighbors)
            {
                if(neighbors.neighborType != (int)NeighborEnum.Left && neighbors.neighborType != (int)NeighborEnum.Right)
                    continue;

                var neighborsBlock = TileMatchGameManager.Instance.GetBlock(neighbors.id);
                if (filter.Contains(neighbors.id))
                {
                    var upNeighbors = neighborsBlock._blockModel._blockData.neighbors.Find(a => a.neighborType == (int)NeighborEnum.Up);
                    if (upNeighbors != null)
                    {
                        var upNeighborsBlock = TileMatchGameManager.Instance.GetBlock(upNeighbors.id);
                        if (upNeighborsBlock._blockModel._blockData.blockType == (int)BlockTypeEnum.Funnel)
                        {
                            if (((BlockFunnelModel)(upNeighborsBlock._blockModel))._residueCount >= 1)
                                return true;
                        }
                    }
                    continue;
                }

                if (!neighborsBlock.IsInActiveState())
                {
                    var upNeighbors = neighborsBlock._blockModel._blockData.neighbors.Find(a => a.neighborType == (int)NeighborEnum.Up);
                    if (upNeighbors != null)
                    {
                        var upNeighborsBlock = TileMatchGameManager.Instance.GetBlock(upNeighbors.id);
                        if (upNeighborsBlock._blockModel._blockData.blockType == (int)BlockTypeEnum.Funnel)
                        {
                            if (!((BlockFunnelModel)(upNeighborsBlock._blockModel)).IsRemoveAll())
                                return true;
                        }
                    }
                    continue;
                }

                filter.Add(neighbors.id);
                if (neighborsBlock.CanBeActive(filter))
                {
                    filter.Remove(neighbors.id);
                    return true;
                }
                
                filter.Remove(neighbors.id);
            }

            return false;
        }

        private void CheckUnlock(bool ignoreState = false)
        {
            if(_isCanRemove)
                return;
            
            if (!ignoreState && _blockState != BlockState.Normal)
                return;

            _isCanRemove = true;
            _blockView.BreakAnim();
        }
        
        
        public override void StartShuffle()
        {
            base.StartShuffle();
            
            if(_isCanRemove)
                return;
            
            _selfView.StartShuffle();
        }

        public override void StopShuffle()
        {
            base.StopShuffle();
            
            if(_isCanRemove)
                return;
            
            _selfView.StopShuffle();
        }
        
        public override void Magic_BeforeRemoveBlock(List<Block> blocks)
        {
            foreach (var block in blocks)
            {
                base.BeforeRemoveBlock(new List<Block>(){block});
            
                if (!IsInActiveState())
                    return;

                if (_isCanRemove)
                    continue;
            
                if (_blockModel._blockData.neighbors.Find(a => (a.neighborType == (int)NeighborEnum.Left || a.neighborType == (int)NeighborEnum.Right) && a.id == block._id) == null)
                    continue;
        
                CheckUnlock();
                
                if (!CanBeActive(block._id, true))
                {
                    _blockView.RefreshNormalMask();
                    CheckUnlock(true);                    
                    DebugUtil.LogError("----------当前不可以解锁锁链 触发解锁锁链----------");
                }
            }
        }

        public override void Magic_AfterRemoveBlock(List<Block> blocks)
        {
            foreach (var block in blocks)
            {
                base.AfterRemoveBlock(new List<Block>(){block});
            }
        }
        
        public override void StartMagic()
        {  
            base.StartMagic();
            
            if(_selfModel.IsCollect)
                return;
            
            _selfView.BreakAnim();
        }
        
        public override void ExecutePreprocess(List<Block> blocks)
        {
            base.BeforeRemoveBlock(blocks, false);
            
            if (!IsInActiveState())
                return; 

            if (_isCanRemove)
                return;

            foreach (var block in blocks)
            {
                if (IgnoreRemoveBlockHandle(block))
                    continue;

                if (block.AreaType == AreaType.SuperBanner)
                    continue;
                
                Neighbors neighbors = _blockModel._blockData.neighbors.Find(a => a.id == block._id);
                if (neighbors == null)
                    continue;
                
                if(_blockState == BlockState.Normal)
                    _isPreprocessRemove = true;
            }
        }
        
        public override FailTypeEnum CheckFailure(List<Block> blocks)
        {
            if (!IsInActiveState())
                return FailTypeEnum.None; 

            if (_isCanRemove)
                return FailTypeEnum.None; 

            foreach (var block in blocks)
            {
                if (IgnoreRemoveBlockHandle(block))
                    continue;
                
                if (block.AreaType == AreaType.SuperBanner)
                    continue;
                
                Neighbors neighbors = _blockModel._blockData.neighbors.Find(a => a.id == block._id);
                if (neighbors == null)
                    continue;
                
                if (!CanBeActive(block._id))
                    return FailTypeEnum.Special; 
            }

            return FailTypeEnum.None; 
        }
    }
}