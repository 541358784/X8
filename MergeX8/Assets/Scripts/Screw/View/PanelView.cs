using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DragonPlus.Haptics;
using Screw.Module;
using Screw.PointerEvent;
using Screw.UI;
using Screw.UIBinder;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Screw
{
    [ViewAsset("Screw/Prefabs/Common/Panel")]
    public class PanelView : Entity
    {
        [UIBinder("")] private PolygonCollider2D _collider;

        [UIBinder("Body")] private SpriteRenderer _bodySprite;
        [UIBinder("Body")] private SpriteMask _bodyMask;
        [UIBinder("Body")] private ColorProvider _bodyBreak;

        [UIBinder("Shadow")] private SpriteRenderer _bodyShadowSprite;

        private PanelBodyModel _panelBodyModel;
        private PointerEventHandler pointerEventHandler;

        private Vector3 lastPos;
        private Vector3 initPos;
        private bool isMoving;
        private List<Vector2> points = new List<Vector2>();

        public PanelBodyModel PanelBodyModel => _panelBodyModel;
        
        public void SetUpBody(PanelBodyModel panelBodyModel)
        {
            _panelBodyModel = panelBodyModel;

            root.position = panelBodyModel.Position;
            root.localScale = panelBodyModel.Scale;
            root.localEulerAngles = panelBodyModel.EulerAngles;
            root.gameObject.layer = context.GetLayer(panelBodyModel.LayerId);

            lastPos = root.position;
            if (context is ScrewMiniGameContext)
            {
                initPos = lastPos;
            }
                
            _bodySprite.transform.localScale = panelBodyModel.BodyScale;
            _bodySprite.transform.localEulerAngles = panelBodyModel.BodyRotate;

            _bodyShadowSprite.transform.localScale = panelBodyModel.BodyScale;
            // _bodyShadowSprite.transform.position = panelBodyModel.ShadowPos;
            _bodyShadowSprite.GetComponent<ShadowFollow>().SetOriginalPos(GetShadowOffset(panelBodyModel.Position));

            var sprite = context.GetPanelSprite(panelBodyModel.BodyRes);
            _bodySprite.sprite = sprite;
            _bodyShadowSprite.sprite = sprite;
            _bodyMask.sprite = sprite;
            
            // 根据 panelBodyModel.PanelColor 变换颜色
            var colorProviders = root.GetComponentsInChildren<ColorProvider>();
            for (int i = 0; i < colorProviders.Length; i++)
            {
                if (colorProviders[i].gameObject.name.Contains("Shadow"))
                    colorProviders[i].SetColor(panelBodyModel.PanelColor, ColorReceiverType.PanelShadow, context);
                else
                    colorProviders[i].SetColor(panelBodyModel.PanelColor, ColorReceiverType.Panel, context, panelBodyModel.LayerId);
            }

            foreach (var holeModels in panelBodyModel.HoleModels.Values)
            {
                var holeView = LoadEntity<HoleView>(GetRoot(), context);
                holeView.SetUpHole(holeModels, panelBodyModel.LayerId);
            }
            
            var spriteRenderers = root.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].sortingOrder = panelBodyModel.LayerId * 1000 + spriteRenderers[i].sortingOrder;
            }

            SetColliderPath(spriteRenderers);

            _bodyMask.frontSortingOrder = panelBodyModel.LayerId * 1000 + _bodyMask.frontSortingOrder;
            _bodyMask.backSortingOrder = panelBodyModel.LayerId * 1000 + _bodyMask.backSortingOrder;

            pointerEventHandler = root.gameObject.AddComponent<PointerEventHandler>();
            pointerEventHandler.BindingPointerUp(OnPointerUp);
            
            EnableUpdate(1);
        }

        private void SetColliderPath(SpriteRenderer[] spriteRenderers)
        {
            var count = 0;
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i].name.Contains("Shadow"))
                    continue;
                var currentShapeCount = spriteRenderers[i].sprite.GetPhysicsShapeCount();
                for (int j = 0; j < currentShapeCount; j++)
                {
                    count++;
                }
            }
            _collider.pathCount = count;
            
            count = 0;
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i].name.Contains("Shadow"))
                    continue;
                var spriteRenderer = spriteRenderers[i];
                var shapeCount = spriteRenderer.sprite.GetPhysicsShapeCount();
                for (int j = 0; j < shapeCount; j++)
                {
                    spriteRenderer.sprite.GetPhysicsShape(j, points);
                    var simplifiedPoints = points;
                    for (int k = 0; k < simplifiedPoints.Count; k++)
                    {
                        simplifiedPoints[k] = simplifiedPoints[k] * spriteRenderer.transform.localScale +
                                              (Vector2) spriteRenderer.transform.localPosition;
                    }
                    _collider.SetPath(count, simplifiedPoints);
                    count++;
                }
            }
        }
        private Vector3 GetShadowOffset(Vector3 pos)
        {
            // Quaternion quaternion = Quaternion.Euler(root.localEulerAngles);
            return context.ShadowOffset + pos;
        }

        public bool IsMoving()
        {
            return isMoving;
        }

        public override void OnUpdate()
        {
            if (context is ScrewMiniGameContext)
            {
                if (Mathf.Abs(Mathf.Abs(initPos.y) - Mathf.Abs(root.position.y)) > 7)
                {
                    _panelBodyModel.IsActive = false;
                    context.hookContext.OnLogicEvent(LogicEvent.RefreshShield, null);
                }
            }
            
            if (Mathf.Abs(Mathf.Abs(initPos.y) - Mathf.Abs(root.position.y)) > 20)
            {
                Destroy();
                
                if (context.gameState != ScrewGameState.Fail && context.gameState != ScrewGameState.Win)
                {
                    context.hookContext.OnLogicEvent(LogicEvent.BlockCheckFail, null);
                    if (context.gameState == ScrewGameState.Fail)
                    {
                        context.hookContext.OnLogicEvent(LogicEvent.ActionFinish, null);
                    }
                }
                return;
            }
            if (lastPos != root.position)
            {
                lastPos = root.position;
                isMoving = true;
            }
            else
            {
                if (isMoving)
                {
                    isMoving = false;
                    if (context.gameState != ScrewGameState.Fail && context.gameState != ScrewGameState.Win)
                    {
                        context.hookContext.OnLogicEvent(LogicEvent.BlockCheckFail, null);
                        if (context.gameState == ScrewGameState.Fail)
                        {
                            context.hookContext.OnLogicEvent(LogicEvent.ActionFinish, null);
                        }
                    }
                }
            }
        }

        private async void OnPointerUp(PointerEventData eventData)
        {
            if (root == null)
                return;
            if (context.gameState == ScrewGameState.InUseBooster)
            {
                UIModule.Instance.EnableEventSystem = false;
                // 消耗booster
                context.boostersHandler.GetHandler<BreakBodyBoosterHandler>(BoosterType.BreakBody).ConsumeHammerBooster();
                var color = _bodyBreak.GetColor();
                context.PlayHammerAni(Input.mousePosition);
                SoundModule.PlaySfx("sfx_useItem_glass");
                await UniTask.Delay(700);
                _bodyBreak.BreakPanel();
                ScrewUtility.Vibrate(HapticTypes.Success);
                await ScrewUtility.WaitSeconds(0.3f, false, root.GetCancellationTokenOnDestroy());
                var effect = AssetModule.Instance.LoadAsset<GameObject>("Screw/Prefabs/Particle/PaneLShatterParticle");
                effect.transform.position = root.transform.position;
                var main = effect.GetComponent<ParticleSystem>().main;
                main.startColor = color;

                UIModule.Instance.EnableEventSystem = true;
                Destroy();
                context.gameState = ScrewGameState.InProgress;
                context.hookContext.OnLogicEvent(LogicEvent.ExitBreakPanel, null);
                GameObject.Destroy(effect, 1);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            isMoving = false;
        }
    }
}