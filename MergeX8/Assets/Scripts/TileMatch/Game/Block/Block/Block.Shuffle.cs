using UnityEngine;

namespace TileMatch.Game.Block
{
    public partial class Block : IShuffle
    {
        public virtual void StartShuffle()
        {
            if (!IsValidState())
                return;
            
            _blockView.DoGrayFade(0, true);

            if (_blockState == BlockState.Black)
            {
                _blockView.DoDarkFade(1, false);
                _blockView.DoDarkColor(Color.white, true);
            }
        }

        public virtual void ShuffleRefresh()
        {
            _blockView.RefreshIcon();
        }
        
        public virtual void StopShuffle()
        {
            RefreshView(true);
        }

        public virtual bool CanShuffle()
        {
            if (_blockState == BlockState.Black)
                return false;
            
            return true;
        }
        
        public virtual void Shuffle(Block block)
        {
            int blockId = _blockId;
            _blockId = block._blockId;
            block._blockId = blockId;
        }
    }
}