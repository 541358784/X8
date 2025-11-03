using Screw.UI;
using Screw.UIBinder;
using UnityEngine;

namespace Screw
{
    [ViewAsset("Screw/Prefabs/Common/Hole")]
    public class HoleView : Entity
    {
        [UIBinder("")] private SpriteMask _mask;

        private HoleModel _holeModel;

        public void SetUpHole(HoleModel holeModel, int layerId)
        {
            _holeModel = holeModel;

            var transform = GetRoot();
            transform.position = holeModel.Position;
            _mask.frontSortingOrder = layerId * 1000 + _mask.frontSortingOrder;
            _mask.backSortingOrder = layerId * 1000 + _mask.backSortingOrder;
        }
    }
}