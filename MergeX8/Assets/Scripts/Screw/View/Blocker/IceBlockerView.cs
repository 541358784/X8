using Screw.UI;
using Screw.UIBinder;
using UnityEngine;

namespace Screw
{
    [ViewAsset("Screw/Prefabs/Common/IceBody")]
    public class IceBlockerView : BaseBlockerView
    {
        [UIBinder("")] private Animator ani;

        public void SetUpView(ScrewModel screwModel)
        {
            root.position = screwModel.Position;
            
            var sprites = root.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sprite in sprites)
            {
                sprite.sortingOrder = screwModel.LayerId * 1000 + sprite.sortingOrder;
            }

            var particles = root.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var particle in particles)
            {
                var render = particle.GetComponent<ParticleSystemRenderer>();
                render.sortingOrder = screwModel.LayerId * 1000 + render.sortingOrder;
            }
        }

        public void BreakIceStage(IceBlockerModel model)
        {
            var aniStates = "";
            switch (model.StageCounter)
            {
                case 0:
                    aniStates = "IceCrack3";
                    break;
                case 1:
                    aniStates = "IceCrack2";
                    break;
                case 2:
                    aniStates = "IceCrack1";
                    break;
            }
            ani.Play(aniStates);
        }
    }
}