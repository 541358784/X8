using Screw.UI;
using Screw.UIBinder;
using TMPro;
using UnityEngine;


namespace Screw
{
    [ViewAsset("Screw/Prefabs/Common/BombBody")]
    public class BombBlockerView : BaseBlockerView
    {
        [UIBinder("")] private Animator ani;
        [UIBinder("CountText")] private TextMeshPro count;
        [UIBinder("Bomb1")] private SpriteRenderer topSprite;
        [UIBinder("Bomb2")] private SpriteRenderer shapeSprite;
        [UIBinder("Sparks")] private Transform particle;

        private ScrewView _screwView;

        public void SetUpView(ScrewModel screwModel, BombBlockerModel bombBlockerModel, ScrewView screwView)
        {
            root.position = screwModel.Position;
            _screwView = screwView;

            topSprite.sprite = context.GetSprite($"{screwModel.ScrewColor}_Bomb");
            shapeSprite.sprite = context.GetSprite($"{screwModel.ScrewColor}_{screwModel.ScrewShape}");

            count.sortingOrder = screwModel.LayerId * 1000 + count.sortingOrder;

            var spriteRenderers = root.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].sortingOrder = screwModel.LayerId * 1000 + spriteRenderers[i].sortingOrder;
            }

            var particles = root.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var p in particles)
            {
                var render = p.GetComponent<ParticleSystemRenderer>();
                render.sortingOrder = screwModel.LayerId * 1000 + render.sortingOrder;
            }
            count.text = bombBlockerModel.StageCount.ToString();
            if (bombBlockerModel.StageCount <= 1)
            {
                PlayColor();
            }
            
            EnableUpdate(2);
            switch (screwModel.ScrewColor)
            {
                case ColorType.Green:
                    count.outlineColor = new Color32(47, 105, 0, 255);
                    break;
                case ColorType.Cyan:
                    count.outlineColor = new Color32(10, 71, 146, 255);
                    break;
                case ColorType.Yellow:
                    count.outlineColor = new Color32(210, 47, 22, 255);
                    break;
                case ColorType.Blue:
                    count.outlineColor = new Color32(39, 60, 167, 255);
                    break;
                case ColorType.Lilac:
                    count.outlineColor = new Color32(123, 24, 187, 255);
                    break;
                case ColorType.Red:
                    count.outlineColor = new Color32(168, 11, 22, 255);
                    break;
                case ColorType.Pink:
                    count.outlineColor = new Color32(174, 18, 111, 255);
                    break;
                case ColorType.Purple:
                    count.outlineColor = new Color32(78, 11, 170, 255);
                    break;
                case ColorType.Orange:
                    count.outlineColor = new Color32(167, 40, 3, 255);
                    break;
                case ColorType.Grey:
                    count.outlineColor = new Color32(70, 75, 97, 255);
                    break;
            }
        }

        public override void OnUpdate()
        {
            var isOverlapping = _screwView.IsOverlapping();
            if (isOverlapping && particle.gameObject.activeSelf)
                particle.gameObject.SetActive(false);

            if (!isOverlapping && !particle.gameObject.activeSelf)
                particle.gameObject.SetActive(true);
        }

        private void PlayColor()
        {
            if (ani.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Appear")
                ani.Play("Appear", -1, 0);
        }

        public bool ChangeStageCounter(BombBlockerModel model)
        {
            count.text = model.StageCount.ToString();

            if (model.StageCount <= 1)
            {
                if (model.StageCount == 0)
                {
                    ani.Play("Bomb", -1, 0);
                    return true;
                }
                else
                    PlayColor();
            }
            else
            {
                ani.Play("Jump", -1, 0);
            }

            return false;
        }
    }
}