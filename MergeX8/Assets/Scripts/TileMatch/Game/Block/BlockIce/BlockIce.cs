using System;
using System.Collections.Generic;
using LayoutData;

namespace TileMatch.Game.Block
{
    public class BlockIce : Block
    {
        private BlockIceModel _selfModel;
        private BlockIceView _selfView;

        public BlockIce(Layer.Layer layer, LayerBlock blockData, int index): base(layer, blockData, index){}
        public override void InitData(Layer.Layer layer, LayerBlock blockData)
        {
            _blockModel = new BlockIceModel(layer, blockData, this);
            _blockView = new BlockIceView(this);
            
            _selfModel = (BlockIceModel)_blockModel;
            _selfView = (BlockIceView)_blockView;

            _isCanRemove = false;
        }
        public override void BeforeRemoveBlock(List<Block> blocks, bool isRefresh = true)
        {
            base.BeforeRemoveBlock(blocks, isRefresh);
            
            if(_isCanRemove)
                return;
            
            if(_blockState != BlockState.Normal)
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

                _isCanRemove = _selfModel._brokenNum == 0;
                if(_isCanRemove)
                    return;
            }
        }
        
        public override void StartShuffle()
        {
            base.StartShuffle();
            
            if(_isCanRemove || _selfModel.IsCollect)
                return;
            
            _selfView.StartShuffle();
        }

        public override void StopShuffle()
        {
            base.StopShuffle();
            
            if(_isCanRemove || _selfModel.IsCollect)
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
            
            _selfModel._brokenNum = 0;
            _selfView.BreakAnim();
        }
        
        public override FailTypeEnum CheckFailure(List<Block> blocks)
        {
            if (!IsInActiveState())
                return FailTypeEnum.None; 
            
            if(_selfModel.IsCollect)
                return FailTypeEnum.None;

            if (_isCanRemove)
                return FailTypeEnum.None;

            BlockState blockState = GetBlockState();
           
            Dictionary<int, List<int>> parentBlockIds = new Dictionary<int, List<int>>();
            bool isParent = false;
            foreach (var block in blocks)
            {
                if(block.AreaType == AreaType.SuperBanner)
                    continue;
                
                if (block._blockModel._blockData.parent != null)
                {
                    if(!parentBlockIds.ContainsKey(block._id))
                        parentBlockIds.Add(block._id, block._blockModel._blockData.parent);
                }
                
                if (!block._blockModel._blockData.parent.Contains(_id)) 
                    continue;

                isParent = true;
                
                if (!IsActiveBlock(block._id))
                    continue;

                blockState = BlockState.Normal;
            }

            if(blockState != BlockState.Normal)
                return FailTypeEnum.None;
            
            int subNum = 1;
            if (isParent)
                subNum = 0;
            
            var activeBlocks = TileMatchGameManager.Instance.GetActiveBlocks();
            activeBlocks.Remove(this);
            blocks.ForEach(a=>activeBlocks.Remove(a));

            int activeNum = 0;
            foreach (var activeBlock in activeBlocks)
            {
                if (activeBlock.GetBlockState() != BlockState.Normal)
                {
                    foreach (var kv in parentBlockIds)
                    {
                        if(!kv.Value.Contains(activeBlock._id))
                            continue;
                        
                        if(activeBlock.IsActiveBlock(kv.Key))
                            activeNum++;
                    }
                    continue;
                }

                activeNum ++;
            }
            
            if(_selfModel._brokenNum-subNum <= 0)
                return FailTypeEnum.None;
            
            if(_selfModel._brokenNum-subNum > activeNum)
                return FailTypeEnum.SpecialFail;
            
            return FailTypeEnum.None;
        }
    }
}