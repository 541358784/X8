﻿using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static SomeWhere.TableView;

namespace SomeWhere
{
    public partial class Internal_TableView
    {
        enum Status
        {
            None,
            AutoMoving,
        }

        public TableView _proxy;

        public const string DEFAULT_IDENTIFIER = "default.identifier";

        /// <summary>
        /// 返回行数
        /// </summary>
        public delegate int DelegateNumberOfCells(Internal_TableView tbView);

        /// <summary>
        /// 返回第i行cell的高度（垂直），或宽度（水平）
        /// </summary>
        public delegate float DelegateSizeOfIndex(Internal_TableView tbView, int index);

        /// <summary>
        /// 返回第i行cell的RectTransform
        /// </summary>
        public delegate TableViewCell DelegateTransformOfIndex(Internal_TableView tbView, int index);

        /// <summary>
        /// 返回第i行cell的标识符
        /// </summary>
        public delegate string DelegateIdentifierOfIndex(Internal_TableView tbView, int index);

        /// <summary>
        /// 初始化cell的数据
        /// </summary>
        public delegate void DelegateOnInitCell(TableViewCell cell, int index);

        /// <summary>
        /// 滚动时回调
        /// </summary>
        public delegate void DelegateOnCellMove(TableViewCell cell, int index);

        private DelegateNumberOfCells _delegateNumberOfCells = null;
        private DelegateSizeOfIndex _delegateSizeOfIndex = null;
        private DelegateTransformOfIndex _delegateTransformOfIndex = null;
        private DelegateIdentifierOfIndex _delegateIdentifierOfIndex = null;
        private DelegateOnInitCell _delegateOnInitCell = null;
        private DelegateOnCellMove _delegateOnCellMove = null;

        private int _extraBuffer = 2;
        private Rect[] _rects;
        private Dictionary<string, TableViewGroup> _groupDict = new Dictionary<string, TableViewGroup>();
        private int _numberOfCells = 0;
        private int _minVisibleIndex = 0;
        private int _maxVisibleIndex = 0;
        private bool _centralized = false;
        private TableViewCell[] _cellsList;

        private Status _status;

        public int MinVisibleIndex => _minVisibleIndex;
        public int MaxVisibleIndex => _maxVisibleIndex;
        public Internal_TableView(TableView monoProxy, DelegateNumberOfCells delegateNumberOfCells, DelegateSizeOfIndex delegateSizeOfIndex, DelegateTransformOfIndex delegateTransformOfIndex, DelegateOnInitCell delegateOnInitCell, DelegateIdentifierOfIndex delegateIdentifierOfIndex, DelegateOnCellMove delegateOnCellMove, bool centralized)
        {
            _proxy = monoProxy;
            _proxy.OnDestroyDelegate = OnRelease;
            _proxy.onValueChanged.AddListener(OnMove);
            _proxy.OnBeginDragDelegate = OnBeginDrag;
            _proxy.OnEndDragDelegate = OnEndDrag;

            _delegateNumberOfCells = delegateNumberOfCells;
            _delegateSizeOfIndex = delegateSizeOfIndex;
            _delegateTransformOfIndex = delegateTransformOfIndex;
            _delegateIdentifierOfIndex = delegateIdentifierOfIndex;
            _delegateOnInitCell = delegateOnInitCell;
            _delegateOnCellMove = delegateOnCellMove;

            _status = Status.None;
            _centralized = centralized;
            _cellsList = new TableViewCell[delegateNumberOfCells(this)];
        }

        protected void OnRelease()
        {
            _delegateNumberOfCells = null;
            _delegateOnInitCell = null;
            _delegateSizeOfIndex = null;
            _delegateTransformOfIndex = null;
            _delegateIdentifierOfIndex = null;
        }

        bool IsIdentifierExist(string identifier)
        {
            return _groupDict.ContainsKey(identifier + "_v");
        }

        void OnMove(Vector2 value)
        {
            if (_numberOfCells == 0) return;

            UpdateMinMaxVisibleIndex();
            updateCellArc();

            onCellMove();
        }

        private void onCellMove()
        {
            for (int i = _minVisibleIndex; i <= _maxVisibleIndex; i++)
            {
                var cell = GetCell(i);
                if (cell != null)
                {
                    _delegateOnCellMove?.Invoke(cell, i);
                }
            }
        }

        private void addCellAtIndex(int index)
        {
            if (index >= _numberOfCells || index < 0) return;
            var identifier = getIdentifierForIndex(index);

            if (_groupDict[identifier]._visibilityCellQueueDic.ContainsKey(index) == false)
            {
                var cell = _delegateTransformOfIndex(this, index);
                cell.RectTrans.gameObject.SetActive(true);
                
                _cellsList[index] = cell;
                _groupDict[identifier]._visibilityCellQueueDic.Add(index, cell);
                
                initCellTransformSettings(cell, index);
                _delegateOnInitCell(cell, index);
            }
        }

        private void updateCellArc()
        {
            if (_proxy.ArcCell)
            {
                for (int index = _minVisibleIndex; index <= _maxVisibleIndex; index++)
                {
                    var cell = _cellsList[index];
                    if (cell == null) continue;

                    var rect = _rects[index];

                    var newX = rect.x + rect.width / 2f;
                    var newY = rect.y;
                    var cellWorldPos = _proxy.content.TransformPoint(new Vector3(newX, newY, 0));
                    var radius = (_proxy.transform as RectTransform).rect.width * _proxy.ArcRadiusScale;
                    var viewCenterWorldPos = _proxy.transform.position;
                    var deltaX = viewCenterWorldPos.x - cellWorldPos.x;
                    deltaX = deltaX * UIRoot.Instance.mRootCanvas.referencePixelsPerUnit;
                    var sign = deltaX > 0 ? 1 : -1;
                    deltaX = Mathf.Abs(deltaX);
                    if (deltaX < radius)
                    {
                        var deltaY = radius - Mathf.Sqrt(radius * radius - deltaX * deltaX);
                        newY = rect.y - deltaY;
                    }
                    else
                    {
                        newY = rect.y - radius;
                    }

                    var angle = Mathf.Asin(deltaX / radius) / Mathf.PI * 180 * sign;
                    cell.RectTrans.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    cell.RectTrans.anchoredPosition = new Vector2(newX, newY);
                }
            }
        }

        private void initCellTransformSettings(TableViewCell cell, int index)
        {
            var rect = _rects[index];

            //trans.anchorMin = new Vector2(0.5f, 0.5f);
            //trans.anchorMax = new Vector2(0.5f, 0.5f);
            cell.RectTrans.SetParent(_proxy.content);
            //trans.anchoredPosition = new Vector2(x, y);
            cell.RectTrans.localScale = Vector3.one;
            cell.RectTrans.SetLocalPositionZ(0);

            if (_proxy.vertical)
            {
                cell.RectTrans.anchoredPosition = new Vector2(rect.x, rect.y - rect.height / 2f);
                cell.RectTrans.anchorMin = new Vector2(0f, 1f);
                cell.RectTrans.anchorMax = new Vector2(1f, 1f);
                cell.RectTrans.pivot = new Vector2(0.5f, 0.5f);
                var v = cell.RectTrans.offsetMin;
                v.x = 0;
                cell.RectTrans.offsetMin = v;
                v = cell.RectTrans.offsetMax;
                v.x = 0;
                cell.RectTrans.offsetMax = v;
            }
            else
            {
                var newX = rect.x + rect.width / 2f;
                var newY = rect.y;
                cell.RectTrans.anchoredPosition = new Vector2(newX, newY);
                cell.RectTrans.anchorMin = new Vector2(0f, 0.5f);
                cell.RectTrans.anchorMax = new Vector2(0f, 0.5f);
                cell.RectTrans.pivot = new Vector2(0.5f, 0.5f);
            }
        }

        private void RemoveCellAtIndex(int index)
        {
            var identifier = getIdentifierForIndex(index);
            if (_groupDict.ContainsKey(identifier))
            {
                if (_groupDict[identifier]._visibilityCellQueueDic.ContainsKey(index) == true)
                {
                    var cell = _groupDict[identifier]._visibilityCellQueueDic[index];
                    _groupDict[identifier]._invisibilityCellQueue.Add(cell);
                    _groupDict[identifier]._visibilityCellQueueDic.Remove(index);
                    cell.RectTrans.gameObject.SetActive(false);
                }
            }
        }

        private int IndexAtOffset(float offset)
        {
            var minIndex = 0;
            var maxIndex = _numberOfCells - 1;
            maxIndex = maxIndex < 0 ? 0 : maxIndex;
            var index = (maxIndex + minIndex) / 2;

            if (_proxy.vertical)
            {
                while (minIndex < maxIndex)
                {
                    var indexY = _rects[index].y;
                    var nextY = _rects[index + 1].y;

                    if (indexY >= offset && nextY < offset) break;
                    else if (nextY >= offset) minIndex = index + 1;
                    else maxIndex = index;

                    index = (maxIndex + minIndex) / 2;
                }
            }
            else
            {
                while (minIndex < maxIndex)
                {
                    var indexX = -_rects[index].x;
                    var nextX = -_rects[index + 1].x;

                    if (indexX >= offset && nextX < offset) break;
                    else if (nextX >= offset) minIndex = index + 1;
                    else maxIndex = index;

                    index = (maxIndex + minIndex) / 2;
                }
            }

            return index;
        }

        private Vector2 GetMinMaxVisibleIndex()
        {
            var trans = _proxy.transform as RectTransform;
            var offset = _proxy.content.anchoredPosition;

            var viewWidth = trans.rect.width;
            var viewHeight = trans.rect.height;

            var minIndex = _proxy.vertical ? IndexAtOffset(-offset.y) : IndexAtOffset(offset.x);
            var maxIndex = _proxy.vertical ? IndexAtOffset(-offset.y - viewHeight) : IndexAtOffset(offset.x - viewWidth);

            var boundMinIndex = 0;
            var boundMaxIndex = _numberOfCells - 1; // Mathf.Max(0, numberOfCells - 1);
            minIndex = minIndex - _extraBuffer / 2;
            minIndex = minIndex < boundMinIndex ? boundMinIndex : minIndex;
            maxIndex = maxIndex + _extraBuffer / 2;
            maxIndex = maxIndex > boundMaxIndex ? boundMaxIndex : maxIndex;

            return new Vector2(minIndex, maxIndex);
        }

        public void ReAllocateCellList()
        {
            _numberOfCells = _delegateNumberOfCells(this);
            _cellsList = new TableViewCell[_numberOfCells];
        }

        /// <summary>
        /// 重新加载数据
        /// </summary>
        /// <param name="startIndex">重新加载后跳转到第startIndex行</param>
        public void ReloadData(ReloadType reloadType = ReloadType.Keep, int startIndex = 0, bool indexAtCenter = true, bool clearCell = false)
        {
            if (_delegateNumberOfCells == null) return;

            //清理
            if (clearCell)
            {
                _groupDict.Clear();
                // _proxy.content.Clear();
            }
            else
            {
                foreach (string identifier in _groupDict.Keys)
                {
                    _groupDict[identifier].QueueInvisible();
                }
            }

            //创建cell分组
            System.Action<int> createIdentifierGroup = null;
            if (_delegateIdentifierOfIndex == null)
            {
                if (_groupDict.ContainsKey(DEFAULT_IDENTIFIER) == false) _groupDict[DEFAULT_IDENTIFIER] = new TableViewGroup();
            }
            else
            {
                createIdentifierGroup = (i) =>
                {
                    string identifier = _delegateIdentifierOfIndex(this, i);
                    if (_groupDict.ContainsKey(identifier) == false) _groupDict[identifier] = new TableViewGroup();
                };
            }

            //计算content的大小
            _numberOfCells = _delegateNumberOfCells(this);
            _rects = new Rect[_numberOfCells];
            var width = 0f;
            var x = (float)_proxy.Offset.left;
            var height = 0f;
            var y = (float)_proxy.Offset.top;
            var contentPos = _proxy.content.anchoredPosition;
            var contentSize = _proxy.content.sizeDelta;
            var trans = _proxy.transform as RectTransform;
            var viewWidth = trans.rect.width;
            var viewHeight = trans.rect.height;

            if (_proxy.vertical)
            {
                for (var i = 0; i < _numberOfCells; i++)
                {
                    height = _delegateSizeOfIndex(this, i);
                    _rects[i] = new Rect(0, -y, viewWidth, height);
                    y += height;
                    if (createIdentifierGroup != null) createIdentifierGroup(i);
                }

                contentSize.y = y + _proxy.Offset.bottom;

                if (_centralized) //make content centralize when few cells
                {
                    var scrollHeight = (_proxy.ScrollRect.transform as RectTransform).rect.height;
                    if (contentSize.y < scrollHeight)
                    {
                        var delta = (scrollHeight - _rects.Length * height) / 2;
                        var newY = 0f;
                        for (var j = 0; j < _rects.Length; j++)
                        {
                            _rects[j] = new Rect(0, -(delta + newY), viewWidth, height);
                            newY += height;
                        }

                        contentSize.y = scrollHeight;
                    }
                }
            }
            else
            {
                for (var i = 0; i < _numberOfCells; i++)
                {
                    width = _delegateSizeOfIndex(this, i);
                    _rects[i] = new Rect(x, 0, width, viewHeight);
                    x += width;

                    if (createIdentifierGroup != null) createIdentifierGroup(i);
                }

                if (x - viewWidth < 0)
                {
                    x = 0;
                }
                else
                {
                    x = x - viewWidth;
                }

                contentSize.x = x + _proxy.Offset.right;
                contentSize.x += _proxy.AppendWidthAdEnd;
            }

            _proxy.content.sizeDelta = contentSize;

            //设置list的偏移位置
            if (reloadType == ReloadType.ScrollTo)
            {
                var tempIndex = GetMinMaxVisibleIndex();
                _minVisibleIndex = (int) tempIndex.x;
                _maxVisibleIndex = (int) tempIndex.y;
                scrollToIndex(startIndex, indexAtCenter);
            }
            else if (reloadType == ReloadType.Start)
            {
                if (_proxy.vertical) contentPos.y = -viewHeight;
                else contentPos.x = viewWidth;
                _proxy.content.anchoredPosition = contentPos;
            }
            else if (reloadType == ReloadType.Keep)
            {
                //do nothing
            }

            var indexes = GetMinMaxVisibleIndex();
            _minVisibleIndex = (int) indexes.x;
            _maxVisibleIndex = (int) indexes.y;

            //add cells
            if (_numberOfCells > 0)
            {
                for (var i = _minVisibleIndex; i <= _maxVisibleIndex; i++)
                {
                    addCellAtIndex(i);
                }
            }

            //初始化按页滑动配置
            if (_proxy.PageScroll)
            {
                InitForPageView();
                if (reloadType == ReloadType.ScrollTo)
                    GotoPage(startIndex);
            }

            //初始化arc cell
            updateCellArc();
        }

        private void scrollToIndex(int targetIndex, bool indexAtCenter)
        {
            var contentPos = _proxy.content.anchoredPosition;

            if (indexAtCenter)
            {
                targetIndex = _convertIndexToCenter(targetIndex);
            }

            if (_proxy.vertical)
            {
                contentPos.y = -_rects[targetIndex].yMin; //+ _proxy.ViewportHalfHeight();
                contentPos.y = Mathf.Clamp(contentPos.y, 0,
                    _proxy.content.rect.height - _proxy.ViewportHeight());
            }
            else
            {
                contentPos.x = -_rects[targetIndex].center.x + _proxy.ViewportHalfWidth();
                contentPos.x = Mathf.Clamp(contentPos.x, 
                    -_proxy.content.rect.width + _proxy.ViewportWidth(), 
                    0);
            }
                // -_proxy.ViewportHalfWidth());
            _proxy.content.anchoredPosition = contentPos;
            
            // var width = 0f;
            // var height = 0f;
            // var x = (float)_proxy.Offset.left;
            // var y = (float)_proxy.Offset.top;
            // var offsetX = 0f;
            // var offsetY = 0f;
            // var trans = _proxy.transform as RectTransform;
            // var viewWidth = trans.rect.width;
            // var viewHeight = trans.rect.height;
            //
            // if (_proxy.vertical)
            // {
            //     for (var i = 0; i < _numberOfCells; i++)
            //     {
            //         height = _delegateSizeOfIndex(this, i);
            //         y += height;
            //         if (i < targetIndex) offsetY += height;
            //     }
            // }
            // else
            // {
            //     for (var i = 0; i < _numberOfCells; i++)
            //     {
            //         width = _delegateSizeOfIndex(this, i);
            //         _rects[i] = new Rect(x, 0, width, viewHeight);
            //         x += width;
            //         if (i < targetIndex) offsetX -= width;
            //         if (i == targetIndex) offsetX -= width / 2f;
            //         if (targetIndex == 0) offsetX = 0;
            //     }
            // }
            //
            // if (_proxy.vertical) contentPos.y = offsetY;
            // else contentPos.x = offsetX;
            // _proxy.content.anchoredPosition = contentPos;
        }

        private string getIdentifierForIndex(int index)
        {
            return _delegateIdentifierOfIndex == null ? DEFAULT_IDENTIFIER : _delegateIdentifierOfIndex(this, index);
        }

        public TableViewCell DequeueReusabelCell(int index)
        {
            var identifier = getIdentifierForIndex(index);
            var invisibilityCellQueue = _groupDict[identifier]._invisibilityCellQueue;
            var count = invisibilityCellQueue.Count;
            if (count > 0)
            {
                var cell = invisibilityCellQueue[count - 1];
                invisibilityCellQueue.RemoveAt(count - 1);
                return cell;
            }

            return null;
        }

        public Rect GetCellRect(int index)
        {
            return _rects[index];
        }
        
        public TableViewCell GetCell(int index)
        {
            var identifier = getIdentifierForIndex(index);
            var group = _groupDict[identifier];
            group._visibilityCellQueueDic.TryGetValue(index, out var cell);
            return cell;
        }

        public List<TableViewCell> GetVisibleCells()
        {
            var result = new List<TableViewCell>();
            for (var i = _minVisibleIndex; i <= _maxVisibleIndex; i++)
            {
                result.Add(GetCell(i));
            }

            return result;
        }

        private int _convertIndexToCenter(int index)
        {
            // DebugUtil.LogError("startIndex:" + startIndex);
            // index -= Mathf.Abs(_maxVisibleIndex - _minVisibleIndex) / 2;
            // DebugUtil.LogError("startIndex:" + startIndex);
            index = Mathf.Clamp(index, 0, _delegateNumberOfCells(this) - 1);
            // DebugUtil.LogError("startIndex:" + startIndex);
            return index;
        }

        public void ScrollToTargetIndex(int currentIndex, int targetIndex, Ease easeType = Ease.OutCirc, float duration = 0.8f, System.Action onScrollFinish = null)
        {
            _proxy.ScrollEnable = false;
            _status = Status.AutoMoving;

            var centerPosIndex = _convertIndexToCenter(targetIndex);

            // var deltaIndex = Mathf.Abs(targetIndex - currentIndex);
            // if (deltaIndex <= 3) duration = deltaIndex * 0.3f;
            // else if (deltaIndex <= 6) duration = deltaIndex * 0.2f;
            // else duration = deltaIndex * 0.05f;

            // var scaleDuration = 0.1f;
            // var easyType = Ease.OutCirc;

            //移动列表
            var sequence2 = DOTween.Sequence();
            // sequence2.AppendInterval(scaleDuration);
            if (_proxy.vertical)
            {
                var targetY = -_rects[centerPosIndex].center.y - _proxy.ViewportHalfHeight();
                targetY = Mathf.Clamp(targetY,0,
                    _proxy.content.rect.height - _proxy.ViewportHeight());
                //     -_proxy.ViewportHalfWidth());
                sequence2.Append(_proxy.content.DOAnchorPosY(targetY, duration).SetEase(easeType)).OnComplete(() => onScrollFinish?.Invoke());
            }
            else
            {
                var targetX = -_rects[centerPosIndex].center.x + _proxy.ViewportHalfWidth();
                targetX = Mathf.Clamp(targetX,
                    -_proxy.content.rect.width + _proxy.ViewportWidth(),
                    0);
                //     -_proxy.ViewportHalfWidth());
                sequence2.Append(_proxy.content.DOAnchorPosX(targetX, duration).SetEase(easeType)).OnComplete(() => onScrollFinish?.Invoke());
                // sequence2.Append(_proxy.content.DOAnchorPosX(0, duration).SetEase(easyType)).OnComplete(() => onScrollFinish?.Invoke());
            }
        }

        public void MoveCellToTargetIndex(int moveIndex, int targetIndex, System.Action onMoveFinish)
        {
            if (moveIndex == targetIndex)
            {
                onMoveFinish?.Invoke();
                return;
            }

            _proxy.ScrollEnable = false;
            _status = Status.AutoMoving;

            var centerPosIndex = _convertIndexToCenter(targetIndex);

            var deltaIndex = Mathf.Abs(targetIndex - moveIndex);
            var duration = 0.1f;
            if (deltaIndex <= 3) duration = deltaIndex * 0.3f;
            else if (deltaIndex <= 6) duration = deltaIndex * 0.2f;
            else duration = deltaIndex * 0.1f;

            var scaleDuration = 0.1f;
            var moveCell = GetCell(moveIndex);
            var floatingCell = GameObject.Instantiate(moveCell.RectTrans, moveCell.RectTrans.parent);
            var easyType = Ease.InSine;

            //移动自己cell
            moveCell.RectTrans.gameObject.SetActive(false);
            var sequence = DOTween.Sequence();
            sequence.Append(floatingCell.DOScale(Vector3.one * 1.03f, scaleDuration));
            if (_proxy.vertical)
            {
                sequence.Append(floatingCell.DOAnchorPosY(_rects[targetIndex].y - _rects[targetIndex].size.y / 2f, duration).SetEase(easyType));
            }
            else
            {
                sequence.Append(floatingCell.DOAnchorPosX(-_rects[targetIndex].x + _rects[targetIndex].size.x / 2f, duration).SetEase(easyType));
            }

            sequence.Append(floatingCell.DOScale(Vector3.one, scaleDuration));
            sequence.OnUpdate(() => floatingCell.SetAsLastSibling());
            sequence.OnComplete(() =>
            {
                updateFloatingData();
                ReloadData();
                GameObject.Destroy(floatingCell.gameObject);

                _status = Status.None;
                onMoveFinish?.Invoke();
                _proxy.ScrollEnable = true;
                var cell = GetCell(targetIndex);
                cell?.ShowComplete();
            });

            //移动中间所有cell

            //移动列表
            var sequence2 = DOTween.Sequence();
            sequence2.AppendInterval(scaleDuration);
            if (_proxy.vertical)
            {
                sequence2.Append(_proxy.content.DOAnchorPosY(-_rects[centerPosIndex].y, duration).SetEase(easyType));
            }
            else
            {
                sequence2.Append(_proxy.content.DOAnchorPosX(-_rects[centerPosIndex].x, duration).SetEase(easyType));
            }
            
            var sequence3 = DOTween.Sequence();
            sequence3.AppendInterval(scaleDuration);
            sequence3.OnComplete(() => { floatingCell.GetComponent<TableViewCell>().OnUpdate(moveIndex, targetIndex, duration); });
        }

        public void Update()
        {
            updatePageScroll();
        }
    }
}