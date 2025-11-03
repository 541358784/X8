using LayoutData;

namespace TileMatch.Game.Block
{
    public class BlockGrassModel : BlockModel
    {
        public int _brokenNum;
        public BlockGrassModel(Layer.Layer layer, LayerBlock blockData, Block block) : base(layer, blockData, block)
        {
            _brokenNum = 2;
        }

        public bool IsBroken()
        {
            return _brokenNum <= 0;
        }
    }
}