using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonPlus.Haptics;
using Screw.GameLogic;
using Screw.PointerEvent;
using Screw.UI;
using Screw.UIBinder;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Screw
{
    [ViewAsset("Screw/Prefabs/Common/Screw")]
    public class ScrewView : Entity
    {
        [UIBinder("")] private CircleCollider2D circle;

        [UIBinder("ScrewBody")] private Transform body;

        [UIBinder("Pattern")] private SpriteRenderer shapeSprite;
        [UIBinder("ScrewBack")] private Transform back;
        [UIBinder("ScrewBack")] private SpriteRenderer backSprite;
        [UIBinder("Pattern")] private Transform pattern;

        [UIBinder("ScrewTop")] private SpriteRenderer top;
        [UIBinder("ScrewTop")] private CircleCollider2D topCollider2D;
        [UIBinder("Overlap")] private Transform overlap;
        
        private ScrewModel _screwModel;

        protected PointerEventHandler pointerEventHandler;

        private Collider2D[] overlapCollider = new Collider2D[10];

        private bool isClick = false;

        private Sequence sequence;

        public void SetUpScrew(ScrewModel screwModel)
        {
            _screwModel = screwModel;

            root.position = screwModel.Position;
            root.gameObject.layer = context.GetLayer(screwModel.LayerId);
            var spriteRenderers = root.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].sortingOrder = screwModel.LayerId * 1000 + spriteRenderers[i].sortingOrder;
            }

            shapeSprite.sprite = context.GetSprite($"{screwModel.ScrewColor}_{screwModel.ScrewShape}");
            top.sprite = context.GetSprite($"{screwModel.ScrewColor}_Screw");
            backSprite.sprite = context.GetSprite($"{screwModel.ScrewColor}_ScrewBack");
            
            pointerEventHandler = top.gameObject.AddComponent<PointerEventHandler>();
            pointerEventHandler.BindingPointerDown(OnPointerDown);
            pointerEventHandler.BindingPointerUp(OnPointerUp);

            if (context is ScrewMiniGameContext)
            {
                List<TableGuide> configs = GlobalConfigManager.Instance.GetGuidesByPosition((int)GuideTargetType.ScrewTouch);
                foreach (var tableGuide in configs)
                {
                    if(tableGuide.finishParam != screwModel.ScrewId.ToString())
                        continue;
                    
                    var target = root.gameObject.AddComponent<RectTransform>();
                    target.sizeDelta = new Vector2(2, 2);
                    Vector3 position = target.position;
                    target.transform.position = position;

                    root = target.transform;
                
                    GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ScrewTouch, target, targetParam:screwModel.ScrewId.ToString());
                }
                
                ScrewGuide();
            }
        }

        private void OnPointerDown(PointerEventData eventData)
        {
            // 这里判断上面层级是否遮挡
            if (eventData.pointerCurrentRaycast.gameObject == top.gameObject && 
                context.membersHandler.IsScrewDownValid(_screwModel) && !isClick && !context.SlotAreaFull())
            {
                SoundModule.PlaySfx("sfx_collect_screw");
                ScrewUtility.Vibrate();
                context.Record.AddAction(_screwModel.ScrewId);
                context.membersHandler.HandleScrewDown(_screwModel, this);
            }

            if (context is ScrewMiniGameContext)
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ScrewTouch, _screwModel.ScrewId.ToString());
            }
        }

        public bool SimulatePointerDown()
        {
            if (context.membersHandler.IsScrewDownValid(_screwModel) && !isClick && !context.SlotAreaFull())
            {
                context.membersHandler.HandleScrewDown(_screwModel, this);
                return true;
            }

            return false;
        }
        
        private void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject == top.gameObject && !IsOverlapping() &&
                context.membersHandler.IsScrewUpValid(_screwModel) && !isClick)
            {
                context.membersHandler.HandleScrewUp(_screwModel, this);
                ScrewUtility.Vibrate(HapticTypes.Warning);
            }
        }
        
        public bool IsOverlapping()
        {
            var count = Physics2D.OverlapCircleNonAlloc(overlap.position, 0.47f, overlapCollider);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if(overlapCollider[i] == null)
                        continue;
                    
                    if (overlapCollider[i] != topCollider2D && overlapCollider[i] != circle && overlapCollider[i].gameObject.layer > (ScrewGameLogic.Instance.ScrewLayer + _screwModel.LayerId))
                        return true;
                }
            }

            return false;
        }

        public bool IsMoving()
        {
            return sequence != null;
        }

        public bool IsClicked()
        {
            return isClick;
        }

        public Vector3 GetBodyPos()
        {
            return body.position;
        }

        public void DoJump()
        {
            body.DOKill();
            body.DOLocalJump(Vector3.zero, 1f, 1, 0.2f);
        }

        public async UniTask DoMove(Transform target, Action callback = null)
        {
            isClick = true;

            if (sequence != null)
                return;
            SetPointUpSortingOrder();
            context.SetModelSlotState(target);
            circle.isTrigger = true;
            UniTaskCompletionSource source = new UniTaskCompletionSource();
            sequence = DOTween.Sequence();

            root.SetParent(target);
            root.gameObject.layer = LayerMask.GetMask("Default");

            var targetPos = target.position;
            var finalPos = targetPos;
            
            targetPos.z = root.position.z;
            if (!target.name.Contains("ColorSlotPrefab"))
                finalPos.z = root.position.z;

            sequence.Append(root.DOMove(root.position + Vector3.up * 0.8f, 0.15f).OnStart(() =>
            {
                pattern.DORotate(Vector3.forward * 180, 0.14f, RotateMode.FastBeyond360);
                float v = 0.1f;
                DOTween.To(() => v, (x) =>
                {
                    back.localScale = new Vector3(1, x, 1);
                }, 1, 0.14f).OnComplete(() =>
                {
                    back.localScale = Vector3.one;
                });
            }));
            sequence.Append(root.DOMove(targetPos + Vector3.up * 0.8f, 0.25f));
            sequence.Append(root.DOMove(finalPos, 0.15f).OnStart(() =>
            {
                pattern.DORotate(Vector3.zero, 0.14f, RotateMode.FastBeyond360);
                float v = 1f;
                DOTween.To(() => v, (x) =>
                {
                    back.localScale = new Vector3(1, x, 1);
                }, 0f, 0.1f).OnComplete(() =>
                {
                    back.localScale = new Vector3(1, 0f, 1);
                });
            }));
            sequence.OnComplete( async () =>
            {
                context.SetSlotState(target);
                sequence = null;
                root.localPosition = Vector3.zero;
                context.hookContext.OnLogicEvent(LogicEvent.RefreshTaskStatus, new MoveEndParams(_screwModel));
                await ScrewUtility.WaitNFrame(2);
                callback?.Invoke();
                source.TrySetResult();
            });
           
            sequence.Play();
            await source.Task;
        }

        private void SetPointUpSortingOrder()
        {
            bool changeZ = false;
            var spriteRenderers = root.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i].sortingOrder > 9999)
                    continue;
                changeZ = true;
                spriteRenderers[i].sortingOrder = spriteRenderers[i].sortingOrder + 9999;
            }

            if (changeZ)
            {
                root.position += Vector3.back * 10;
            }
        }

        public void EnterBreakPanel()
        {
            topCollider2D.enabled = false;
        }

        public void ExitBreakPanel()
        {
            topCollider2D.enabled = true;
        }

        private async UniTask ScrewGuide()
        {
            await ScrewUtility.WaitSeconds(0.5f, false);
            
            if (context is ScrewMiniGameContext)
            {
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ScrewTouch, _screwModel.ScrewId.ToString(), _screwModel.ScrewId.ToString());
            }
        }
    }
}