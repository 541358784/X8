using LayoutData;

namespace TileMatch.Game.Block
{
    public class BlockFunnelLink : Block
    {
        private BockFunnel _linkBlock;
        
        public BlockFunnelLink(Layer.Layer layer, LayerBlock blockData, int index): base(layer, blockData, index){}

        public override void InitData(Layer.Layer layer, LayerBlock blockData)
        {
            _blockModel = new BlockFunnelLinkModel(layer, blockData, this);
            _blockView = new BlockFunnelLinkView(this);
            
            _blockState = BlockState.Inactive;
        }

        public override void InitState()
        {
            if(_blockState == BlockState.Inactive)
                return;
            
            base.InitState();
        }
        
        public override bool CanShuffle()
        {
            return false;
        }
    }
}