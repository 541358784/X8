using LayoutData;

namespace TileMatch.Game.Block
{
    public class BlockCurtainModel : BlockModel
    {
        public bool _isOpen = false;
        
        public BlockCurtainModel(Layer.Layer layer, LayerBlock blockData, Block block) : base(layer, blockData, block)
        {
            if (!blockData.blockParam.IsEmptyString())
                _isOpen = int.Parse(blockData.blockParam) == 1;
        }
    }
}