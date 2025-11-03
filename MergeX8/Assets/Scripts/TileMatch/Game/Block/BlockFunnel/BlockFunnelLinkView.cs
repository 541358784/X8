using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockFunnelLinkView : BlockView
    {
        public BlockFunnelLinkView(Block block) : base(block)
        {
        }
        
        public override void LoadView(Transform parent)
        {
            base.LoadView(parent);
            
            _root.gameObject.SetActive(false);
        }
    }
}