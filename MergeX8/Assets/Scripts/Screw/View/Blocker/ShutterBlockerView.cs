using Screw.UI;
using Screw.UIBinder;
using UnityEngine;

namespace Screw
{
    [ViewAsset("Screw/Prefabs/Common/ShutterBody")]
    public class ShutterBlockerView : BaseBlockerView
    {
        [UIBinder("")] private Animator ani;

        public void SetUpView(ScrewModel screwModel, ShutterBlockerModel shutterBlockerModel)
        {
            root.position = screwModel.Position;

            var sprites = root.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sprite in sprites)
            {
                sprite.sortingOrder = screwModel.LayerId * 1000 + sprite.sortingOrder;
            }

            ChangeShutterStatus(shutterBlockerModel);
        }

        public void ChangeShutterStatus(ShutterBlockerModel model)
        {
            ani.SetBool("on", !model.IsBlocking());
        }
    }
}