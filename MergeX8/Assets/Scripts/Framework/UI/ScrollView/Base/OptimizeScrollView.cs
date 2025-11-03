using UnityEngine;
using UnityEngine.UI;
using Framework.DataStructure;

namespace Framework.UI.ScrollView
{
    public class OptimizeScrollView : MonoBehaviour
    {
        [Header("View Object")] public ScrollRect _scrollRect;
        public RectTransform _scrollContent;
        public RectTransform _viewPortTransform;

        [Header("Cell Settings")] public int _spacing = 0;

        protected bool _isAdjusting;
        protected CircleLinkedNode<ScrollCell> _frontNode;
        protected CircleLinkedNode<ScrollCell> _backNode;
        protected CircleLinkedList<ScrollCell> _circleList = new CircleLinkedList<ScrollCell>();
        protected ScrollDataSource _dataSource = new ScrollDataSource();

        private ScrollCellTransformInfo _horizotalCellInfo;
        private ScrollCellTransformInfo _verticalCellInfo;

        #region private UpdateFrontAndBackNode

        private float __frontInViewPortPos;
        private float __frontSizeValue;
        private bool __isFrontPrevFetch;
        private bool __isFrontNextFetch;

        private float __backInViewPortPos;
        private float __backSizeValue;
        private bool __isBackPrevFetch;
        private bool __isBackNextFetch;

        private float __viewPortSizeValue;

        private float __nodeSize;
        private float __pos;

        private Vector2 __newPos;
        private Vector2 __frontPrevSizeDelta;
        private Vector2 __backNextSizeDelta;

        #endregion

        protected virtual void Awake()
        {
            if (_scrollRect == null)
            {
                Debug.LogError("scrollRect is Null !!!");
                return;
            }

            if (_scrollRect.horizontal == _scrollRect.vertical)
            {
                _scrollRect.horizontal = true;
                _scrollRect.vertical = false;
            }

            if (_scrollContent == null)
            {
                Debug.LogError("scrollContent is Null !!!");
                return;
            }

            if (_scrollRect.horizontal)
            {
                _scrollContent.anchorMax = new Vector2(0, 0.5f);
                _scrollContent.anchorMin = new Vector2(0, 0.5f);
                _scrollContent.pivot = new Vector2(0, 0.5f);
            }
            else if (_scrollRect.vertical)
            {
                _scrollContent.anchorMax = new Vector2(0.5f, 1f);
                _scrollContent.anchorMin = new Vector2(0.5f, 1f);
                _scrollContent.pivot = new Vector2(0.5f, 1f);
            }

            //for scroll cell
            _horizotalCellInfo = new ScrollCellTransformInfo
            {
                anchorMin = new Vector2(0f, 0.5f),
                anchorMax = new Vector2(0f, 0.5f),
                pivot = new Vector2(0f, 0.5f)
            };
            _verticalCellInfo = new ScrollCellTransformInfo
            {
                anchorMin = new Vector2(0.5f, 1f),
                anchorMax = new Vector2(0.5f, 1f),
                pivot = new Vector2(0.5f, 1f)
            };
        }

        //data source
        public ScrollDataSource GetDataSource()
        {
            return _dataSource;
        }

        public void SetDataSource(ScrollDataSource source)
        {
            _dataSource = source;
        }

        #region private

        private ScrollCell CreateScrollCell()
        {
            if (_scrollRect == null)
            {
                return null;
            }

            if (_scrollRect.horizontal)
            {
                return _dataSource.CreateCellAtInfo(_horizotalCellInfo);
            }

            return _dataSource.CreateCellAtInfo(_verticalCellInfo);
        }

        private void UpdateContentSize()
        {
            int count = _dataSource.NumberOfCellsInTableView();
            Vector2 cellSize;
            Vector2 size = new Vector2(0, 0);
            for (int index = 0; index < count; ++index)
            {
                cellSize = _dataSource.CellSizeForIndex(this, index);
                if (_scrollRect.horizontal)
                {
                    size.x += cellSize.x + _spacing;
                }
                else if (_scrollRect.vertical)
                {
                    size.y += cellSize.y + _spacing;
                }
            }

            size.x -= _spacing;
            size.y -= _spacing;
            if (_scrollRect.horizontal)
            {
                _scrollContent.sizeDelta = new Vector2(size.x, _scrollRect.viewport.rect.height);
            }
            else if (_scrollRect.vertical)
            {
                _scrollContent.sizeDelta = new Vector2(_scrollRect.viewport.rect.width, size.y);
            }
        }

        protected ScrollCell CreateCell(int idx)
        {
            ScrollCell cell = CreateScrollCell();
            if (cell == null)
            {
                return null;
            }

            Vector2 cellSize = _dataSource.CellSizeForIndex(this, idx);
            cell.thisRectTranform = cell.gameObject.GetComponent<RectTransform>();
            cell.thisRectTranform.SetParent(_scrollContent, false);
            cell.UpdateContentSize(cellSize);
            cell.Idx = idx;
            return cell;
        }

        private Vector3 GetFrontPrevPos()
        {
            if (_scrollRect.horizontal)
            {
                return new Vector3(_frontNode.Value.transform.localPosition.x - _spacing - __frontPrevSizeDelta.x,
                    _frontNode.Value.transform.localPosition.y, _frontNode.Value.transform.localPosition.z);
            }

            return new Vector3(_frontNode.Value.transform.localPosition.x,
                _frontNode.Value.transform.localPosition.y + _spacing + __frontPrevSizeDelta.y,
                _frontNode.Value.transform.localPosition.z);
        }

        private Vector3 GetBackNextPos()
        {
            if (_scrollRect.horizontal)
            {
                return new Vector3(_backNode.Value.transform.localPosition.x + _spacing + __backNextSizeDelta.x,
                    _backNode.Value.transform.localPosition.y, _backNode.Value.transform.localPosition.z);
            }

            return new Vector3(_backNode.Value.transform.localPosition.x,
                _backNode.Value.transform.localPosition.y - _spacing - __backNextSizeDelta.y,
                _backNode.Value.transform.localPosition.z);
        }

        private void UpdateFrontAndBackFetchState()
        {
            if (_scrollRect.horizontal)
            {
                //前
                __frontInViewPortPos = _frontNode.Value.GetPosX() + _scrollContent.localPosition.x;
                __frontSizeValue = _frontNode.Value.GetWidth();
                //后
                __backInViewPortPos = _backNode.Value.GetPosX() + _scrollContent.localPosition.x;
                __backSizeValue = _backNode.Value.GetWidth();
                //viewport
                __viewPortSizeValue = _viewPortTransform.rect.width;
            }
            else if (_scrollRect.vertical)
            {
                //前
                __frontInViewPortPos = -_frontNode.Value.GetPosY() - _scrollContent.localPosition.y;
                __frontSizeValue = _frontNode.Value.GetHeight();
                //后
                __backInViewPortPos = -_backNode.Value.GetPosY() - _scrollContent.localPosition.y;
                __backSizeValue = _backNode.Value.GetHeight();
                //viewport
                __viewPortSizeValue = _viewPortTransform.rect.height;
            }

            __isFrontNextFetch = __frontInViewPortPos + __frontSizeValue + _spacing < 0;
            __isFrontPrevFetch = __frontInViewPortPos > 0;
            __isBackNextFetch = __backInViewPortPos + __backSizeValue < __viewPortSizeValue;
            __isBackPrevFetch = __backInViewPortPos - _spacing > __viewPortSizeValue;
        }

        private bool IsInViewPort(Vector2 vectorPos, ScrollCell cell)
        {
            if (_scrollRect.horizontal)
            {
                __pos = vectorPos.x + _scrollContent.localPosition.x;
                __viewPortSizeValue = _viewPortTransform.rect.width;
                __nodeSize = cell.GetWidth();
            }
            else
            {
                __pos = -vectorPos.y - _scrollContent.localPosition.y;
                __viewPortSizeValue = _viewPortTransform.rect.height;
                __nodeSize = cell.GetHeight();
            }

            return __pos > -(__nodeSize * 1.5 + _spacing) &&
                   __pos < (__viewPortSizeValue + _spacing + __nodeSize * 0.5);
        }

        #endregion


        #region public api

        public bool FrontNodePreviousFetch()
        {
            _dataSource.FetchPrevFrontIndex(this);
            if (_frontNode.Value.Idx != _dataSource.currentFrontIndex)
            {
                __frontPrevSizeDelta = _dataSource.CellSizeForIndex(this, _dataSource.currentFrontIndex);
                __newPos = GetFrontPrevPos();
                _frontNode = _frontNode.Previous;
                if (_frontNode == _backNode)
                {
                    _circleList.InsertNext(_frontNode, CreateCell(_dataSource.currentFrontIndex));
                    _frontNode = _frontNode.Next;
                }

                _frontNode.Value.transform.localPosition = __newPos;
                _frontNode.Value.UpdateContentSize(__frontPrevSizeDelta);
                ProcessNode(ref _frontNode, false);
                if (IsInViewPort(__newPos, _frontNode.Value))
                {
                    _dataSource.UpdateItemAtIndex(_frontNode.Value, _dataSource.currentFrontIndex);
                }

                return true;
            }

            return false;
        }

        public void FrontNodeNextFetch()
        {
            _dataSource.FetchNextFrontIndex(this);
            if (_frontNode.Value.Idx != _dataSource.currentFrontIndex)
            {
                _frontNode = _frontNode.Next;
                ProcessNode(ref _frontNode, true);
            }
        }

        public void BackNodePreviousFetch()
        {
            _dataSource.FetchPrevBackIndex(this);
            if (_backNode.Value.Idx != _dataSource.currentBackIndex)
            {
                _backNode = _backNode.Previous;
                ProcessNode(ref _backNode, true);
            }
        }

        public bool BackNodeNextFetch()
        {
            _dataSource.FetchNextBackIndex(this);
            if (_backNode.Value.Idx != _dataSource.currentBackIndex)
            {
                __backNextSizeDelta = _dataSource.CellSizeForIndex(this, _dataSource.currentBackIndex);
                __newPos = GetBackNextPos();
                _backNode = _backNode.Next;
                if (_backNode == _frontNode)
                {
                    _circleList.InsertPrevious(_backNode, CreateCell(_dataSource.currentFrontIndex));
                    _backNode = _backNode.Previous;
                }

                _backNode.Value.UpdateContentSize(__backNextSizeDelta);
                _backNode.Value.transform.localPosition = __newPos;
                ProcessNode(ref _backNode, false);
                if (IsInViewPort(__newPos, _backNode.Value))
                {
                    _dataSource.UpdateItemAtIndex(_backNode.Value, _dataSource.currentBackIndex);
                }

                return true;
            }

            return false;
        }

        private void ResetScrollContentPos()
        {
            if (_scrollRect.horizontal)
            {
                _scrollContent.localPosition =
                    new Vector3(0.0f, _scrollContent.localPosition.y, _scrollContent.localPosition.z);
            }
            else
            {
                _scrollContent.localPosition =
                    new Vector3(_scrollContent.localPosition.x, 0.0f, _scrollContent.localPosition.z);
            }
        }

        #endregion


        #region virtual api

        public virtual void ReloadData(bool resetPos = true)
        {
            if (resetPos)
            {
                ResetScrollContentPos();
            }

            UpdateContentSize();
            RefreshCellView();

            if (_circleList.Count > 0)
            {
                _frontNode = _circleList.headNode;
            }
        }


        protected virtual void RefreshCellView()
        {
            ScrollCell cell;
            int count = _dataSource.NumberOfCellsInTableView();
            _dataSource.currentFrontIndex = 0;
            _dataSource.currentBackIndex = 0;
            float viewDisplayLen =
                _scrollRect.horizontal ? _viewPortTransform.rect.width : _viewPortTransform.rect.height;
            float currentPos = 0;
            Vector2 size;
            for (int i = 0; i < count; ++i)
            {
                _backNode = _circleList[i];
                if (_backNode == null)
                {
                    cell = CreateCell(_dataSource.currentBackIndex);
                    _circleList.Add(cell);
                    _backNode = _circleList.tailNode;
                }

                cell = _backNode.Value;
                size = _dataSource.CellSizeForIndex(this, i);
                cell.UpdateContentSize(size);
                _dataSource.UpdateItemAtIndex(cell, _dataSource.currentBackIndex);
                if (_scrollRect.horizontal)
                {
                    cell.thisRectTranform.localPosition = new Vector3(currentPos, 0, 0);
                    currentPos += size.x + _spacing;
                }
                else if (_scrollRect.vertical)
                {
                    cell.thisRectTranform.localPosition = new Vector3(0, -currentPos, 0);
                    currentPos += size.y + _spacing;
                }

                if (currentPos > viewDisplayLen)
                {
                    break;
                }

                _dataSource.FetchNextBackIndex(this);
            }
        }


        protected virtual void UpdateFrontAndBackNode()
        {
            if (_frontNode == null || _backNode == null || _frontNode == _backNode)
            {
                return;
            }

            UpdateFrontAndBackFetchState();

            if (__isFrontNextFetch || __isBackNextFetch)
            {
                //向前滑动
                while (__isFrontNextFetch || __isBackNextFetch)
                {
                    //后哨兵节点
                    if (__isBackNextFetch && BackNodeNextFetch() == false)
                    {
                        //向前滑动
                        break;
                    }

                    //前哨兵节点
                    if (__isFrontNextFetch)
                    {
                        //向前滑动
                        FrontNodeNextFetch();
                    }

                    UpdateFrontAndBackFetchState();
                }
            }
            else if (__isFrontPrevFetch || __isBackPrevFetch)
            {
                //向后滑动
                while (__isFrontPrevFetch || __isBackPrevFetch)
                {
                    //前哨兵节点
                    if (__isFrontPrevFetch && FrontNodePreviousFetch() == false)
                    {
                        break;
                    }

                    //后哨兵节点
                    if (__isBackPrevFetch)
                    {
                        BackNodePreviousFetch();
                    }

                    UpdateFrontAndBackFetchState();
                }
            }
        }

        protected virtual void LateUpdate()
        {
            if (_scrollRect == null || _scrollRect.content == null) return;
            UpdateFrontAndBackNode();

            //adjust
            if (_isAdjusting)
            {
                AdjustScrollContent();
            }
        }

        protected virtual void AdjustScrollContent()
        {
        }

        protected virtual void ProcessNode(ref CircleLinkedNode<ScrollCell> node, bool isOnlyCurrent)
        {
        }

        #endregion
    }
}