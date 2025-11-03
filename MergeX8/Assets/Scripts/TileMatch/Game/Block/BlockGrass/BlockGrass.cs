using System.Collections.Generic;
using DragonU3DSDK;
using LayoutData;
using TileMatch.Event;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockGrass : Block
    {
        private bool _isPreprocessRemove = false;
        private int _preprocessBrokenNum = 0;
        
        private BlockGrassModel _selfModel;
        private BlockGrassView _selfView;

        public BlockGrass(Layer.Layer layer, LayerBlock blockData, int index): base(layer, blockData, index){}
        public override void InitData(Layer.Layer layer, LayerBlock blockData)
        {
            _blockModel = new BlockGrassModel(layer, blockData, this);
            _blockView = new BlockGrassView(this);
            
            _selfModel = (BlockGrassModel)_blockModel;
            _selfView = (BlockGrassView)_blockView;

            _isCanRemove = false;
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
                
                if(IgnoreRemoveBlockHandle(block))
                    continue;
            
                Neighbors neighbors = _blockModel._blockData.neighbors.Find(a => a.id == block._id);
                if (neighbors == null)
                    continue;
            
                CheckUnlock();
                break;
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

            int activeNum = 0;
            foreach (var neighbors in _blockModel._blockData.neighbors)
            {
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
                                activeNum++;
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
                            if(!((BlockFunnelModel)(upNeighborsBlock._blockModel)).IsRemoveAll())
                                activeNum++;
                        }
                    }
                    continue;
                }

                filter.Add(neighbors.id);
                
                if (neighborsBlock.CanBeActive(filter))
                    activeNum++;
                
                filter.Remove(neighbors.id);
            }

            return activeNum >= (_selfModel._brokenNum - _preprocessBrokenNum);
        }
        
        private void CheckUnlock(bool ignoreState = false)
        {
            if(_isCanRemove)
                return;
            
            if (!ignoreState && _blockState != BlockState.Normal)
                return;
            
            _selfModel._brokenNum--;
            _selfView.BreakAnim();

            _isCanRemove = _selfModel._brokenNum == 0;
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
        
        public override bool CanShuffle()
        {
            if (_isCanRemove)
                return true;

            return _selfModel.IsBroken();
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
            
                if(IgnoreRemoveBlockHandle(block))
                    continue;
        
                Neighbors neighbors = _blockModel._blockData.neighbors.Find(a => a.id == block._id);
                if (neighbors == null)
                    continue;

                CheckUnlock();
                
                if (!CanBeActive(block._id, true))
                {
                    CheckUnlock(true);
                    if(_isCanRemove)
                        _blockView.RefreshNormalMask();
                    
                    DebugUtil.LogError("----------当前不可以解锁草 触发解锁草----------");
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
            
            if(_isCanRemove)
                return;

            _selfModel._brokenNum = 0;
            _selfView.BreakAnim(null);
        }

        public override void ExecutePreprocess(List<Block> blocks)
        {
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

                if (_blockState == BlockState.Normal)
                {
                    _isPreprocessRemove = _selfModel._brokenNum - 1 <= 0;
                    _preprocessBrokenNum++;
                }
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