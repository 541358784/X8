
using UnityEngine;

namespace TileMatch.Game.Block
{
    public partial class Block : IMagic
    {
        public virtual void StartMagic()
        {  
            _blockView.DoGrayFade(0, true);
            _blockView.DoDarkFade(0, true);
        }
    }
}