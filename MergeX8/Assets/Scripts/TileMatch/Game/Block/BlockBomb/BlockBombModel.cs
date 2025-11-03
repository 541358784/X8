using LayoutData;

namespace TileMatch.Game.Block
{
    public class BlockBombModel : BlockModel
    {
        public int _brokenNum;
        public int _initBrokenNum;
        
        public BlockBombModel(Layer.Layer layer, LayerBlock blockData, Block block) : base(layer, blockData, block)
        {
            _brokenNum = 0;
            if (!blockData.blockParam.IsEmptyString())
                _brokenNum = int.Parse(blockData.blockParam);
            else
                _brokenNum = TileMatchGameManager.Instance.GetLayoutConfig(GameConst.BombKey);

            _initBrokenNum = _brokenNum;
        }
    }
}