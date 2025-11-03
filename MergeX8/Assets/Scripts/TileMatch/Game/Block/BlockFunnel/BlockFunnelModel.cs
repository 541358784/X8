using System.Collections.Generic;
using LayoutData;

namespace TileMatch.Game.Block
{
    public class BlockFunnelModel : BlockModel
    {
        public List<int> _tileIds = new List<int>();
        public int _index;
        public List<Block> _newBlocks = new List<Block>();
        
        public int _residueCount
        {
            get { return _tileIds.Count - _index; }
        }

        public bool IsRemoveAll()
        {
            if (_residueCount > 0)
                return false;

            foreach (var block in _newBlocks)
            {
                if (block.IsActive())
                    return false;
            }

            return true;
        }
        
        public BlockFunnelModel(Layer.Layer layer, LayerBlock blockData, Block block) : base(layer, blockData, block)
        {
            var spStr = blockData.blockParam.Split(',');
            foreach (var id in spStr)
            {
                _tileIds.Add(int.Parse(id));
            }

            _index = 0;
        }
        
        
        public int GetNewBlockId()
        {
            if (_index > _tileIds.Count - 1)
                return -1;

            return _tileIds[_index++];
        }
        
        public override void Destroy()
        {
            foreach (var block in _newBlocks)
            {
                block.Destroy();
            }
            _newBlocks.Clear();
        }

        public int BlockNum()
        {
            return _tileIds.Count;
        }
    }
}