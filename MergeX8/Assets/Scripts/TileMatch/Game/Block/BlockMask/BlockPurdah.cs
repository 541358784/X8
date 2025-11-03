using System;
using System.Collections.Generic;
using LayoutData;

namespace TileMatch.Game.Block
{
    public class BlockPurdah : Block
    {
        private BlockPurdahModel _selfModel;
        private BlockPurdahView _selfView;
        
        public BlockPurdah(Layer.Layer layer, LayerBlock blockData, int index) : base(layer, blockData, index)
        {
            _isCanRemove = false;
        }
        
        public override void InitData(Layer.Layer layer, LayerBlock blockData)
        {
            _blockModel = new BlockPurdahModel(layer, blockData, this);
            _blockView = new BlockPurdahView(this);

            _selfModel = (BlockPurdahModel)_blockModel;
            _selfView = (BlockPurdahView)_blockView;
            
            _isCanRemove = false;
        }
        
        public override void OnPointerEnter()
        {
        }

        public override void OnPointerExit(bool isRemove, Action action)
        {
        }

        public override void BeforeRemoveBlock(List<Block> blocks, bool isRefresh = true)
        {
        }

        public override void AfterRemoveBlock(List<Block> blocks)
        {
        }

        public override void AfterRecoverBlock(List<Block> blocks)
        {
        }
        
        public override void StartMagic()
        {  
        }
        
        public override bool CanShuffle()
        {
            return false;
        }
    }
}