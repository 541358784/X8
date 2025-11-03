using Framework.DataStructure;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.UI.ScrollView
{
    public class CircleListView : OptimizeScrollView
    {
        [Header("View Object")] public RectTransform _centerSelectTransform;

        [Header("Scroll Data")] public bool _enableScale = true;
        public float _moveToCenterSpeed = 10f;
        public float _selectItemScale = 1.0f;
        public float _unselectItemScale = 0.5f;
        private bool _isScrollDefalut = true;

        private float _targetPos;

        private CircleListDelegate.SelectDelegate _selectDelegate;
        private CircleLinkedNode<ScrollCell> _currentSelectNode;

        #region oveeride

        protected override void Awake()
        {
            base.Awake();
            _scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        }

        public override void ReloadData(bool resetPos = true)
        {
            base.ReloadData(resetPos);
            if (_isScrollDefalut)
            {
                _isScrollDefalut = false;
                //ScrollToIndex(0);
            }
        }

        //protected override void ProcessNode(ref CircleLinkedNode<ScrollCell> node, bool isOnlyCurrent)
        //{
        //    if (isOnlyCurrent)
        //    {
        //        node.Value.SetScale(_unselectItemScale, _unselectItemScale, _unselectItemScale);
        //        return;
        //    }
        //    node.Previous.Value.SetScale(_unselectItemScale, _unselectItemScale, _unselectItemScale);
        //}

        private float speed = 0;

        protected override void AdjustScrollContent()
        {
            Vector3 v = _scrollContent.localPosition;
            if (_scrollRect.horizontal)
            {
                //v.x = Mathf.Lerp(_scrollContent.localPosition.x, _targetPos, _moveToCenterSpeed * Time.deltaTime);
                v.x = Mathf.SmoothDamp(_scrollContent.localPosition.x, _targetPos, ref speed, _scrollRect.elasticity,
                    Mathf.Infinity, Time.unscaledDeltaTime);
                _isAdjusting &= Mathf.Abs(_scrollContent.localPosition.x - _targetPos) >= 0.001f;
                _scrollContent.localPosition = v;
            }
            else if (_scrollRect.vertical)
            {
                //v.y = Mathf.Lerp(_scrollContent.localPosition.y, _targetPos, _moveToCenterSpeed * Time.deltaTime);
                v.y = Mathf.SmoothDamp(_scrollContent.localPosition.y, _targetPos, ref speed, _scrollRect.elasticity,
                    Mathf.Infinity, Time.unscaledDeltaTime);
                _scrollContent.localPosition = v;
                _isAdjusting &= Mathf.Abs(_scrollContent.localPosition.y - _targetPos) >= 0.001f;
            }

            UpdateItemScale();
        }

        #endregion

        #region for touch event

        public void OnBeginDrag(BaseEventData baseEventData)
        {
            _scrollRect.OnBeginDrag(baseEventData as PointerEventData);
            _isAdjusting = false;
        }

        public void OnDrag(BaseEventData baseEventData)
        {
            _scrollRect.OnDrag(baseEventData as PointerEventData);
            UpdateItemScale();
        }

        public void OnEndDrag(BaseEventData baseEventData)
        {
            _scrollRect.OnEndDrag(baseEventData as PointerEventData);
            _targetPos = FindSelectNodePos();
            _isAdjusting = true;
            speed = _moveToCenterSpeed;
            TriggerSelect();
        }

        #endregion


        #region private

        private void UpdateItemScale()
        {
            if (_enableScale == false)
            {
                return;
            }

            CircleLinkedNode<ScrollCell> item = _frontNode;
            while (item != null)
            {
                item.Value.UpdateScaleTrans(_scrollContent.localPosition, _centerSelectTransform,
                    _scrollRect.horizontal, _scrollRect.vertical, _selectItemScale, _unselectItemScale);
                item = item.Next;
                if (item == _frontNode || (_backNode != null && item == _backNode.Next))
                {
                    item = null;
                }
            }
        }

        private void TriggerSelect()
        {
            if (_selectDelegate != null)
            {
                if (_currentSelectNode != null)
                {
                    _selectDelegate(_currentSelectNode.Value.Idx, _currentSelectNode.Value.gameObject);
                }
            }
        }

        private float FindSelectNodePos()
        {
            float findPos = 0;
            float centerPos = _scrollRect.horizontal
                ? _centerSelectTransform.localPosition.x
                : _centerSelectTransform.localPosition.y;
            float pos = _scrollRect.horizontal ? _scrollContent.localPosition.x : _scrollContent.localPosition.y;
            //find Select Node
            CircleLinkedNode<ScrollCell> findNode = _frontNode;
            float distance = Mathf.Infinity;
            float d;
            float itemPos;
            while (findNode != null)
            {
                if (_scrollRect.horizontal)
                {
                    itemPos = pos + findNode.Value.GetPosX() + findNode.Value.GetWidth() / 2;
                }
                else
                {
                    itemPos = pos + findNode.Value.GetPosY() - findNode.Value.GetHeight() / 2;
                }

                d = Mathf.Abs(centerPos - itemPos);
                if (d <= distance)
                {
                    distance = d;
                    findPos = centerPos - itemPos;
                    _currentSelectNode = findNode;
                }

                findNode = findNode.Next;

                if (findNode == _frontNode || (_backNode != null && findNode == _backNode.Next))
                {
                    findNode = null;
                }
            }

            return findPos + pos;
        }

        private void FindCurrentPos()
        {
            float findPos = 0;
            if (_scrollRect.horizontal)
            {
                findPos = _centerSelectTransform.localPosition.x - _currentSelectNode.Value.GetPosX() -
                          _currentSelectNode.Value.GetWidth() / 2;
            }
            else if (_scrollRect.vertical)
            {
                findPos = _centerSelectTransform.localPosition.y - _currentSelectNode.Value.GetPosY() -
                          _currentSelectNode.Value.GetHeight() / 2;
            }

            _targetPos = findPos;
        }

        #endregion

        #region api

        public void SetSelectDelegate(CircleListDelegate.SelectDelegate callback)
        {
            _selectDelegate = callback;
        }

        public void ScrollToIndex(int itemIndex)
        {
            int count = _dataSource.NumberOfCellsInTableView();

            if (itemIndex >= 0 || itemIndex < count)
            {
                float currentPos = 0;
                Vector2 size = Vector2.zero;
                float centerPos = _scrollRect.horizontal
                    ? _centerSelectTransform.localPosition.x
                    : _centerSelectTransform.localPosition.y;
                float scrollContentPos = _scrollRect.horizontal
                    ? _scrollContent.localPosition.x
                    : _scrollContent.localPosition.y;
                for (int i = 0; i < count && i != itemIndex; ++i)
                {
                    size = _dataSource.CellSizeForIndex(this, i);
                    currentPos += _scrollRect.horizontal ? size.x + _spacing : -(size.y + _spacing);
                }

                currentPos += _scrollRect.horizontal
                    ? _scrollContent.localPosition.x + size.x * 0.5f
                    : _scrollContent.localPosition.y - size.y * 0.5f;
                float findPos = centerPos - currentPos + scrollContentPos;
                _scrollContent.localPosition = _scrollRect.horizontal
                    ? new Vector3(findPos, _scrollContent.localPosition.y, _scrollContent.localPosition.z)
                    : new Vector3(_scrollContent.localPosition.x, findPos, _scrollContent.localPosition.z);
                LateUpdate();
            }

            _targetPos = FindSelectNodePos();
            _isScrollDefalut = false;
            _isAdjusting = true;
            TriggerSelect();
        }

        public void LeftScroll()
        {
            _currentSelectNode = _currentSelectNode.Next;
            _isAdjusting = true;
            FindCurrentPos();
            TriggerSelect();
        }

        public void RightScroll()
        {
            _currentSelectNode = _currentSelectNode.Previous;
            _isAdjusting = true;
            FindCurrentPos();
            TriggerSelect();
        }

        #endregion
    }
}