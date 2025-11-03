using DG.Tweening;
using Screw.UI;
using Screw.UIBinder;
using UnityEngine;

namespace Screw
{
    [ViewAsset("Screw/Prefabs/Common/KeyBody")]
    public class KeyBodyView : BaseBlockerView
    {
        [UIBinder("Ring")] public SpriteRenderer Ring;
        [UIBinder("Key")] public SpriteRenderer Key;

        public void SetUpView(int targetKeyScrewId)
        {
            Ring.gameObject.SetActive(true);
            Key.gameObject.SetActive(false);
            var keyModel = context.GetScrewModel(targetKeyScrewId);
            root.position = keyModel.Position + Vector3.back * keyModel.LayerId;
            Ring.sortingOrder = keyModel.LayerId * 1000 + Ring.sortingOrder;
            Key.sortingOrder = keyModel.LayerId * 1000 + Key.sortingOrder;
        }
    }

    [ViewAsset("Screw/Prefabs/Common/LockBody")]
    public class LockBlockerView : BaseBlockerView
    {
        [UIBinder("")] private Animator ani;

        private KeyBodyView _keyBodyView;

        public void SetUpView(ScrewModel screwModel, LockBlockerModel lockBlockerModel, int targetKeyScrewId)
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

            _keyBodyView = LoadEntity<KeyBodyView>(root, context);
            _keyBodyView.SetUpView(targetKeyScrewId);
        }

        public void UnLock()
        {
            var keyRoot = _keyBodyView.Key.transform;
            _keyBodyView.Ring.gameObject.SetActive(false);
            _keyBodyView.Key.gameObject.SetActive(true);
            var position = root.position;

            SoundModule.PlaySfx("sfx_obstacle_lock");
            keyRoot.DOJump(position, -3,1, 1f).SetDelay(0.14f).OnUpdate(() =>
            {
                Vector3 normalizedTargetDirection = (position - keyRoot.position).normalized;
                float angle = Vector3.Angle(Vector3.down, normalizedTargetDirection);
                keyRoot.eulerAngles = (normalizedTargetDirection.x < 0 ? Vector3.back : Vector3.forward) * angle;
            }).OnComplete(() =>
            {
                _keyBodyView.Destroy();
                ani.Play("open");
            });
        }
    }
}