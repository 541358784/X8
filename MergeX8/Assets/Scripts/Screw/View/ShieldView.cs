using Cysharp.Threading.Tasks;
using DG.Tweening;
using Screw.UI;
using Screw.UIBinder;
using UnityEngine;

namespace Screw
{
    [ViewAsset("Screw/Prefabs/Common/Shield")]
    public class ShieldView : Entity
    {
        [UIBinder("Body")] private SpriteRenderer _bodySprite;

        private ShieldModel _shieldModel;

        public ShieldModel ShieldModel=>_shieldModel;
        
        public void SetUpBody(ShieldModel shieldModel)
        {
            _shieldModel = shieldModel;

            root.position = _shieldModel.Position;
            root.localScale = _shieldModel.Scale;
            root.localEulerAngles = _shieldModel.Rotate;
            root.gameObject.layer = context.GetLayer(_shieldModel.LayerId);

            _bodySprite.transform.localScale = _shieldModel.BodyScale;
            _bodySprite.transform.localEulerAngles = _shieldModel.BodyRotate;

            var sprite = context.GetPanelSprite(_shieldModel.BodyImageName);
            _bodySprite.sprite = sprite;
        }

        public async UniTask ActiveFalseAnim()
        {
            SoundModule.PlaySfx("sfx_special_tools");
            var sequence = DOTween.Sequence();
            sequence.Append(root.transform.DOScale(new Vector3(1.5f, 1.5f, 1f), 1.5f));
            sequence.OnComplete(() =>
            {
                Destroy();
            });

            await sequence.Play();
        }
    }
}