using LayoutData;

namespace TileMatch.Game.Block
{
    public class BlockGoldModel : BlockModel
    {
        public int _brokenNum;
        public BlockGoldModel(Layer.Layer layer, LayerBlock blockData, Block block) : base(layer, blockData, block)
        {
            _brokenNum = 3;
        }

        public bool IsBroken()
        {
            return _brokenNum <= 0;
        }
    }
}