using System.Collections.Generic;
using LayoutData;

namespace TileMatch.Game.Block
{
    public class BlockWrapper : Block
    {
        private BlockWrapperModel _selfModel;
        private BlockWrapperView _selfView;

        public BlockWrapper(Layer.Layer layer, LayerBlock blockData, int index): base(layer, blockData, index){}
        public override void InitData(Layer.Layer layer, LayerBlock blockData)
        {
            _blockModel = new BlockWrapperModel(layer, blockData, this);
            _blockView = new BlockWrapperView(this);
            
            _selfModel = (BlockWrapperModel)_blockModel;
            _selfView = (BlockWrapperView)_blockView;
            _isCanRemove = false;
            
        }
        public override void BeforeRemoveBlock(List<Block> blocks, bool isRefresh = true)
        {
            base.BeforeRemoveBlock(blocks, isRefresh);
            
            if(_blockState != BlockState.Normal)
                return;
            
            if(_isCanRemove)
                return;
            
            foreach (var block in blocks)
            {
                if (block.AreaType != AreaType.SuperBanner)
                {
                    if(_blockModel._blockData.children != null && !_blockModel._blockData.children.Contains(block._id))
                        continue;
                }
            
                _selfView.BreakAnim();
                _isCanRemove = true;
                return;
            }
        }
        
        public override bool CanShuffle()
        {
            if (_selfModel.IsCollect)
                return true;

            return _isCanRemove;
        }
        
         
        public override void StartMagic()
        {  
            base.StartMagic();
            
            if(_selfModel.IsCollect)
                return;
            
            _selfView.BreakAnim();
        }
    }
}