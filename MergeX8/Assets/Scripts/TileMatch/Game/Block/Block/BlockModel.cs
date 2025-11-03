using LayoutData;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockModel : IDestroy, ICollect
    {
        public Layer.Layer _layer;
        public LayerBlock _blockData;
        public Block _block;

        public Vector3 _convertPosition = Vector3.zero;
        private bool _isCollect = false;
        public const float offsetZ = -0.0002f;
        public Vector3 localPosition
        {
            get
            {
                if (_block.GetAreaType() == AreaType.SuperBanner)
                   return new Vector3(_convertPosition.x-TileMatchGameManager.Instance.Offset.x, _convertPosition.y - TileMatchGameManager.Instance.Offset.y, _convertPosition.z);
                
                return new Vector3(_blockData.position.x, _blockData.position.y, _blockData.position.z + _block._index*offsetZ);
            } 
        }

        public Vector3 position
        {
            get
            {
                if (_block.GetAreaType() == AreaType.SuperBanner)
                    return _convertPosition;
                
                return new Vector3(localPosition.x + TileMatchGameManager.Instance.Offset.x, localPosition.y + TileMatchGameManager.Instance.Offset.y, localPosition.z);
            }
        }
        public BlockModel(Layer.Layer layer, LayerBlock blockData, Block block)
        {
            _layer = layer;
            _blockData = blockData;
            _block = block;
        }

        public virtual void Destroy()
        {
            
        }

        public bool IsCollect
        {
            get { return _isCollect; }
            set
            {
                _isCollect = value;
                if (_isCollect)
                {
                    _block._blockView.RefreshNormalMask();
                }
            }
        }
    }
}