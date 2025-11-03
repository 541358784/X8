/// <summary>
/// 为了在工作中方便复用，兼容各种编程方式，分理mono相关代码作为独立代理
/// </summary>

using DG.Tweening;

namespace SomeWhere
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using System;
    using static UnityEngine.UI.ScrollRect;
    using static SomeWhere.Internal_TableView;

    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [SelectionBase]
    public class TableView : UIBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public delegate void OnStopMove();

        public enum ReloadType
        {
            Start,
            ScrollTo,
            Keep, //在当前行刷新
        }

        public Action OnDestroyDelegate;
        public Action<PointerEventData> OnBeginDragDelegate;
        public Action<PointerEventData> OnEndDragDelegate;
        public bool PageScroll;
        public bool ArcCell; //弧形cell
        public float ArcRadiusScale;
        public float AppendWidthAdEnd;
        public RectOffset Offset = new RectOffset();

        #region ScrollRect proxy

        #region MovingCheck

        private Vector2 _lastContentPos;
        private bool _moving;
        private OnStopMove _onStopMove;
        private float _movingCheckDuration;

        #endregion

        public GameObject Content => content.gameObject;

        public bool vertical
        {
            get => _scrollRect.vertical;
            set => _scrollRect.vertical = value;
        }

        public RectTransform content
        {
            get => _scrollRect.content;
            set => _scrollRect.content = value;
        }

        public float horizontalNormalizedPosition
        {
            get => _scrollRect.horizontalNormalizedPosition;
            set => _scrollRect.horizontalNormalizedPosition = value;
        }

        public RectTransform viewport
        {
            get => _scrollRect.viewport;
            set => _scrollRect.viewport = value;
        }

        public Scrollbar horizontalScrollbar
        {
            get => _scrollRect.horizontalScrollbar;
            set => _scrollRect.horizontalScrollbar = value;
        }

        public Scrollbar verticalScrollbar
        {
            get => _scrollRect.verticalScrollbar;
            set => _scrollRect.verticalScrollbar = value;
        }

        public ScrollbarVisibility horizontalScrollbarVisibility
        {
            get => _scrollRect.horizontalScrollbarVisibility;
            set => _scrollRect.horizontalScrollbarVisibility = value;
        }

        public ScrollbarVisibility verticalScrollbarVisibility
        {
            get => _scrollRect.verticalScrollbarVisibility;
            set => _scrollRect.verticalScrollbarVisibility = value;
        }

        public float horizontalScrollbarSpacing
        {
            get => _scrollRect.horizontalScrollbarSpacing;
            set => _scrollRect.horizontalScrollbarSpacing = value;
        }

        public float verticalScrollbarSpacing
        {
            get => _scrollRect.verticalScrollbarSpacing;
            set => _scrollRect.verticalScrollbarSpacing = value;
        }

        public ScrollRect.ScrollRectEvent onValueChanged
        {
            get => _scrollRect.onValueChanged;
            set => _scrollRect.onValueChanged = value;
        }

        public MovementType movementType
        {
            get => _scrollRect.movementType;
            set => _scrollRect.movementType = value;
        }

        
        public float ViewportHeight()
        {
            return _scrollRect.viewport.rect.height;
        }
        public float ViewportHalfHeight()
        {
            return _scrollRect.viewport.rect.height / 2;
        }
        public float ViewportWidth()
        {
            return _scrollRect.viewport.rect.width;
        }
        public float ViewportHalfWidth()
        {
            return _scrollRect.viewport.rect.width / 2;
        }

        #endregion

        public bool ScrollEnable
        {
            get => _scrollRect.scrollSensitivity == 1;
            set => _scrollRect.scrollSensitivity = value ? 1 : 0;
        }

        public ScrollRect ScrollRect
        {
            get => _scrollRect;
            set => _scrollRect = value;
        }

        private ScrollRect _scrollRect;
        private Internal_TableView _tableView;

        protected override void Awake()
        {
            base.Awake();

            _scrollRect = GetComponent<ScrollRect>();
            if (_scrollRect != null)
            {
                if (_scrollRect.content != null)
                {
                    _scrollRect.content.anchorMax = Vector2.one;
                    _scrollRect.content.anchorMin = new Vector2(0, 1);

                    var contentSizeFitter = _scrollRect.content.GetComponent<ContentSizeFitter>();
                    if (contentSizeFitter != null) contentSizeFitter.enabled = false;

                    var verticalLayout = _scrollRect.content.GetComponent<VerticalLayoutGroup>();
                    if (verticalLayout != null) verticalLayout.enabled = false;
                }
            }

            _lastContentPos = _scrollRect.content.anchoredPosition;
        }

        #region TableView proxy

        public void Init(DelegateNumberOfCells delegateNumberOfCells, DelegateSizeOfIndex delegateSizeOfIndex, DelegateTransformOfIndex delegateTransformOfIndex, DelegateOnInitCell delegateOnInitCell, DelegateIdentifierOfIndex delegateIdentifierOfIndex, DelegateOnCellMove delegateOnCellMove, OnStopMove onStopMove, bool centralized = false)
        {
            _tableView = new Internal_TableView(this, delegateNumberOfCells, delegateSizeOfIndex, delegateTransformOfIndex, delegateOnInitCell, delegateIdentifierOfIndex, delegateOnCellMove, centralized);
            _onStopMove = onStopMove;
        }

        public void SetFloatingTransformAndIndex(DelegateFloatingCell delegateFloatingCell, DelegateFloatingIndex delegateFloatingIndex)
        {
            _tableView.SetFloatingTransformAndIndex(delegateFloatingCell, delegateFloatingIndex);
        }

        public void ReloadData(ReloadType reloadType = ReloadType.Keep, int startIndex = 0, bool indexAtCenter = true, bool clearCell = false)
        {
            _tableView.ReloadData(reloadType, startIndex, indexAtCenter, clearCell);
        }
        public void ReAllocateCellList()
        {
            _tableView.ReAllocateCellList();
        }
        public void MoveCellToTargetIndex(int moveIndex, int targetIndex, Action callback)
        {
            _tableView.MoveCellToTargetIndex(moveIndex, targetIndex, callback);
        }

        public void ScrollToTargetIndex(int currentIndex, int targetIndex, Ease easeType = Ease.OutCirc, float duration = 0.8f, System.Action onScrollFinish = null)
        {
            _scrollRect.StopMovement();
            _tableView.ScrollToTargetIndex(currentIndex, targetIndex, easeType, duration, onScrollFinish);
        }

        public Rect GetCellRect(int index)
        {
            return _tableView.GetCellRect(index);
        }
        public TableViewCell GetCell(int index)
        {
            return _tableView.GetCell(index);
        }

        public List<TableViewCell> GetVisibleCells()
        {
            return _tableView.GetVisibleCells();
        }
        
        public int MinVisibleIndex => _tableView.MinVisibleIndex;
        public int MaxVisibleIndex => _tableView.MaxVisibleIndex;

        #endregion

        protected override void OnDestroy()
        {
            OnDestroyDelegate?.Invoke();

            base.OnDestroy();
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            _scrollRect.OnBeginDrag(eventData);

            OnBeginDragDelegate?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _scrollRect.OnEndDrag(eventData);

            OnEndDragDelegate?.Invoke(eventData);
        }

        private void Update()
        {
            _tableView?.Update();
        }

        private void FixedUpdate()
        {
            checkMoving();
        }

        private void checkMoving()
        {
            _movingCheckDuration += Time.deltaTime;
            if (_movingCheckDuration <= 0.2f) return;

            _movingCheckDuration = 0f;
            if (_lastContentPos == _scrollRect.content.anchoredPosition)
            {
                if (_moving)
                {
                    _moving = false;
                    _onStopMove?.Invoke();
                }
            }
            else
            {
                _moving = true;
                _lastContentPos = _scrollRect.content.anchoredPosition;
            }
        }

        public void SetOnPageChangedDelegate(DelegateOnPageChanged delegatePageChanged)
        {
            _tableView?.SetOnPageChangedDelegate(delegatePageChanged);
        }
    }
}