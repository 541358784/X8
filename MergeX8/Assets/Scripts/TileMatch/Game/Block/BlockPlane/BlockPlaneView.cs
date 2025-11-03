using GamePool;
using Spine.Unity;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockPlaneView : BlockView
    {
        private GameObject _planeObj;
        public BlockPlaneView(Block block) : base(block)
        {
        }
        
        public override void LoadView(Transform parent)
        {
            _obstaclePoolName = ObjectPoolName.TileMatchBlock_Plane;
            
            base.LoadView(parent);

            _planeObj = _obstacle.transform.Find("plane").gameObject;
        }
    }
}