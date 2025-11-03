using System;
using System.Collections.Generic;
using DG.Tweening;
using Screw.UI;
using Screw.UIBinder;
using UnityEngine;

namespace Screw
{
    [ViewAsset("Screw/Prefabs/Common/ColorSlotPrefab")]
    public class OrderSlotView : Entity
    {
        [UIBinder("ColorSlotShape")] private SpriteRenderer _shapeSprite;

        private ScrewModel _screwModel;

        private ScrewShape _shape;
        private ColorType _colorType;

        public void SetUpSlot(ScrewShape shape, ColorType colorType)
        {
            _shape = shape;
            _colorType = colorType;

            _shapeSprite.gameObject.SetActive(shape != ScrewShape.Phillips);

            if (shape != ScrewShape.Phillips)
            {
                _shapeSprite.sprite = context.GetSprite($"{_colorType}_Slot_{_shape}");
            }
        }

        public Transform StorageScrew(ScrewModel model)
        {
            if (_screwModel != null)
                return null;
            if (model.ScrewShape != _shape)
                return null;

            _screwModel = model;
            return root;
        }

        public bool CheckCouldStorage(ColorType taskColor, ScrewShape shape)
        {
            if (taskColor != _colorType)
                return false;
            if (_shape != shape)
                return false;

            return _screwModel == null;
        }

        public void RefreshTaskStatus(ScrewModel inScrewModel)
        {
            if (_shapeSprite.gameObject.activeSelf && _screwModel == inScrewModel)
            {
                _shapeSprite.gameObject.SetActive(false);
            }
        }
    }
    
    [ViewAsset("Screw/Prefabs/Common/ColorSlotModule")]
    public class OrderView : Entity
    {
        [UIBinder("ColorSlotModule")] private Animator ani;
        [UIBinder("PanelRenderer", "ColorSlotModuleCover")] private SpriteRenderer spriteRendererCover;
        [UIBinder("PanelRenderer", "ColorSlotModule")] private SpriteRenderer spriteRenderer;
        [UIBinder("CoverHandle")] private SpriteRenderer spriteHandle;

        [UIBinder("SlotParent")] private Transform slotParent;

        private OrderModel _orderModel;

        private readonly float[][] showPos = new[]
        {
            new float[] { 0f },
            new float[] {-0.7f, 0.7f},
            new float[] {-1.2f, 0, 1.2f},
            new float[] {-1.8f, -0.6f, 0.6f, 1.8f},
            new float[] {-2.4f, -1.2f, 0, 1.2f, 2.4f},
            new float[] {-3f, -1.8f, -0.6f, 0.6f, 1.8f, 3f},
            new float[] {-3.6f, -2.4f, -1.2f, 0, 1.2f, 2.4f, 3.6f}
        };

        private readonly float[] showLength = new[] {3.71f, 3.71f, 4.4f, 5.35f, 6.5f, 7.5f, 8.7f};

        private List<OrderSlotView> _taskSlotViews;

        private Animator _animator;
        public void SetUpTask(OrderModel orderModel, Animator animator)
        {
            _orderModel = orderModel;
            _animator = animator;

            spriteRenderer.sprite = context.GetSprite($"{orderModel.ColorType}_Box");
            spriteRendererCover.sprite = context.GetSprite($"{orderModel.ColorType}_BoxCover");
            spriteHandle.sprite = context.GetSprite($"{orderModel.ColorType}_Handle");
            
            _taskSlotViews = new List<OrderSlotView>();
            var posArray = showPos[orderModel.Shapes.Count - 1];
            for (int i = 0; i < orderModel.Shapes.Count; i++)
            {
                var taskSlotView = LoadEntity<OrderSlotView>(slotParent, context);
                taskSlotView.SetUpSlot(orderModel.Shapes[i], orderModel.ColorType);
                taskSlotView.GetRoot().localPosition = new Vector3(posArray[i], 0.22f, 0);
                _taskSlotViews.Add(taskSlotView);
            }

            spriteRendererCover.size =
                new Vector2(showLength[orderModel.Shapes.Count - 1] + 0.4f, spriteRendererCover.size.y);
            spriteRenderer.size =  new Vector2(showLength[orderModel.Shapes.Count - 1], spriteRenderer.size.y);
        }

        public Transform StorageScrew(ScrewModel model)
        {
            if (model == null)
                return null;

            // 颜色不同不能放
            if (model.ScrewColor != _orderModel.ColorType)
                return null;

            for (int i = 0; i < _taskSlotViews.Count; i++)
            {
                var slotView = _taskSlotViews[i];
                var targetTransform = slotView.StorageScrew(model);
                if (targetTransform != null)
                {
                    return targetTransform;
                }
            }
            return null;
        }

        public bool CheckCouldStorage(ColorType taskColor, ScrewShape shape)
        {
            for (int i = 0; i < _taskSlotViews.Count; i++)
            {
                var slotView = _taskSlotViews[i];
                bool couldStorage = slotView.CheckCouldStorage(taskColor, shape);
                if (couldStorage)
                {
                    return true;
                }
            }

            return false;
        }

        public void SetSlotState(Transform target)
        {
            for (int i = 0; i < _taskSlotViews.Count; i++)
            {
                if (_taskSlotViews[i].GetRoot() == target)
                {
                    _orderModel.SetSlotState(i);
                    return;
                }
            }
        }
        public bool IsComplete()
        {
            return _orderModel.IsComplete();
        }
        
        public void SetModelSlotState(Transform target)
        {
            for (int i = 0; i < _taskSlotViews.Count; i++)
            {
                if (_taskSlotViews[i].GetRoot() == target)
                {
                    _orderModel.SetModelSlotState(i);
                    return;
                }
            }
        }

        public bool IsModelComplete()
        {
            return _orderModel.IsModelComplete();
        }

        public OrderModel GetTaskModel()
        {
            return _orderModel;
        }

        public void DoLocalMove(Vector3 targetPos, float duration, Action callback = null)
        {
            _animator.Play("Loop");
            root.DOLocalMove(targetPos, duration).SetEase(Ease.InBack).OnComplete(() =>
            {
                _animator.Play("Normal");
                callback?.Invoke();
            });
        }

        public void PlayCompleted()
        {
            if (ani.enabled)
                return;
            ScrewUtility.WaitSeconds(0.2f, () => { ScrewUtility.Vibrate(); }).Forget();
            SoundModule.PlaySfx("sfx_close_cover");
            ani.enabled = true;
        }

        public void RefreshTaskStatus(ScrewModel screwModel)
        {
            foreach (var slotView in _taskSlotViews)
            {
                slotView.RefreshTaskStatus(screwModel);
            }
        }
    }
}