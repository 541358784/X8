using DragonPlus.Config.TileMatch;
using DragonU3DSDK.Asset;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockPurdahView : BlockView
    {
        public BlockPurdahView(Block block) : base(block)
        {
        }

        public override void LoadView(Transform parent)
        {
            base.LoadView(parent);

            Vector3 localPosition = _block._blockModel.localPosition;
            localPosition.z -= 4;
            _root.transform.localPosition = localPosition;
        }

        public override void RefreshIcon()
        {
            string imageName = "purdah" + _block._blockModel._blockData.blockParam;
            if(_icon.sprite.name == imageName)
                return;

            _icon.sprite = null;
            _icon.sprite = ResourcesManager.Instance.GetSpriteVariant("SpriteAtlas/TileMatchAtlas", imageName);
            _icon.sprite.name = imageName;
        }
    }
}