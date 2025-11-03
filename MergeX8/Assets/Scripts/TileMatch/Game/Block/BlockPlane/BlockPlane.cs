using LayoutData;

namespace TileMatch.Game.Block
{
    public class BlockPlane : Block
    {
        private BlockPlaneModel _selfModel;
        private BlockPlaneView _selfView;
        
        public BlockPlane(Layer.Layer layer, LayerBlock blockData, int index) : base(layer, blockData, index)
        {
        }
        
        public override void InitData(Layer.Layer layer, LayerBlock blockData)
        {
            _blockModel = new BlockPlaneModel(layer, blockData, this);
            _blockView = new BlockPlaneView(this);

            _selfModel = (BlockPlaneModel)_blockModel;
            _selfView = (BlockPlaneView)_blockView;
        }
    }
}