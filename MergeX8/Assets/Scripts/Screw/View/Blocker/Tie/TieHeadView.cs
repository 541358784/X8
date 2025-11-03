using Screw.UI;
using Screw.UIBinder;
using UnityEngine;

namespace Screw
{
    [ViewAsset("Screw/Prefabs/Common/ChainBody")]
    public class TieHeadView : Entity
    {
        [UIBinder("pieces")] private Transform container;

        private Animator ani;
        private int aniIndex = 0;
        public void SetUpView(int length, int layerId)
        {
            aniIndex = 0;
            for (int i = 0; i < container.childCount; i++)
            {
                var child = container.GetChild(i);
                child.gameObject.SetActive(child.name == length.ToString());
                if (child.name == length.ToString())
                {
                    ani = child.GetComponent<Animator>();
                    var sprites = child.GetComponentsInChildren<SpriteRenderer>(true);
                    foreach (var sprite in sprites)
                    {
                        sprite.sortingOrder = layerId * 1000 + sprite.sortingOrder;
                    }
                    var particles = child.GetComponentsInChildren<ParticleSystem>(true);
                    foreach (var particle in particles)
                    {
                        var render = particle.GetComponent<ParticleSystemRenderer>();
                        render.sortingOrder = layerId * 1000 + render.sortingOrder;
                    }
                }
            }
        }

        public void PlayAni()
        {
            aniIndex++;
            ScrewUtility.PlayAnimation(ani, aniIndex.ToString());
        }
    }
}