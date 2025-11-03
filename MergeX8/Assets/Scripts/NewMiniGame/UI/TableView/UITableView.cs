using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.UI.TableView
{
    public partial class UITableView : UIScrollRectHolder
    {
        public delegate int DelegateNumberOfCells(UITableView tbView);

        public delegate float DelegateSizeOfIndex(UITableView tbView, int index);

        public delegate ITableViewCell DelegateTransformOfIndex(UITableView tbView, int index);

        public delegate string DelegateIdentifierOfIndex(UITableView tbView, int index);

        public delegate void DelegateOnInitCell(ITableViewCell cell, int index);

        public delegate void DelegateOnCellMove(ITableViewCell cell, int index);

        protected DelegateNumberOfCells     NumberOfCells      = null;
        private   DelegateSizeOfIndex       _sizeOfIndex       = null;
        private   DelegateTransformOfIndex  _transformOfIndex  = null;
        private   DelegateIdentifierOfIndex _identifierOfIndex = null;
        private   DelegateOnInitCell        _onInitCell        = null;
        private   DelegateOnCellMove        _onCellMove        = null;


        protected OnStopMove _onStopMove;

        protected enum Status
        {
            None,
            AutoMoving,
        }

        public const string DEFAULT_IDENTIFIER = "default.identifier";

        private   int                                _extraBuffer = 2;
        protected Rect[]                             _rects;
        private   Dictionary<string, TableViewGroup> _groupDict       = new();
        protected int                                _numberOfCells   = 0;
        protected int                                _minVisibleIndex = 0;
        protected int                                _maxVisibleIndex = 0;
        private   bool                               _centralized     = false;
        protected ITableViewCell[]                   _cellsList;

        protected Status _status;

        public delegate void OnStopMove();

        public enum ReloadType
        {
            Start,
            ScrollTo,
            Keep, //在当前行刷新
            End,
        }


        public float AppendWidthAdEnd;
        public float AppendWidthAdStart;
        public bool  Reverse;


        public bool ScrollEnable
        {
            set => _scrollRect.scrollSensitivity = value ? 1 : 0;
        }

        public void Init(DelegateNumberOfCells delegateNumberOfCells
            , DelegateSizeOfIndex delegateSizeOfIndex
            , DelegateTransformOfIndex delegateTransformOfIndex
            , DelegateOnInitCell delegateOnInitCell
            , DelegateIdentifierOfIndex delegateIdentifierOfIndex
            , DelegateOnCellMove delegateOnCellMove = null
            , OnStopMove onStopMove = null
            , bool centralized = false
        )
        {
            NumberOfCells = delegateNumberOfCells;
            _sizeOfIndex = delegateSizeOfIndex;
            _transformOfIndex = delegateTransformOfIndex;
            _identifierOfIndex = delegateIdentifierOfIndex;
            _onInitCell = delegateOnInitCell;
            _onCellMove = delegateOnCellMove;

            _status = Status.None;
            _centralized = centralized;
            _cellsList = new ITableViewCell[delegateNumberOfCells(this)];

            _onStopMove = onStopMove;
            OnEndDragSlider = EndMove;
        }

        private void EndMove()
        {
            if (_onStopMove == null) return;
            _onStopMove();
        }

        protected internal override void OnRemove()
        {
            base.OnRemove();

            NumberOfCells = null;
            _onInitCell = null;
            _sizeOfIndex = null;
            _transformOfIndex = null;
            _identifierOfIndex = null;
        }

        protected override void OnMove(Vector2 value)
        {
            base.OnMove(value);

            if (_numberOfCells == 0) return;

            UpdateMinMaxVisibleIndex();
            UpdateFading();
            UpdateCellIndex();

            OnCellMove();
        }

        private void OnCellMove()
        {
            for (int i = _minVisibleIndex; i <= _maxVisibleIndex; i++)
            {
                var cell = GetCell(i);
                if (cell != null)
                {
                    _onCellMove?.Invoke(cell, i);
                }
            }
        }


        private void UpdateCellIndex()
        {
            return;

            for (var index = _cellsList.Length - 1; index >= 0; index--)
            {
                var cell = _cellsList[index];
                if (cell != null)
                {
                    var siblingIndex = _numberOfCells - 1 - index;
                    cell.RectTrans.SetSiblingIndex(siblingIndex);
                }
            }
        }

        private void AddCellAtIndex(int index)
        {
            if (index >= _numberOfCells || index < 0) return;
            var identifier = GetIdentifierForIndex(index);

            if (_groupDict[identifier].VisibilityCellQueueDic.ContainsKey(index) == false)
            {
                var cell = _transformOfIndex(this, index);
#if UNITY_EDITOR
                cell.Transform.gameObject.name = index.ToString();
#endif
                _cellsList[index] = cell;
                InitCellTransformSettings(cell, index);
                _onInitCell(cell, index);

                _groupDict[identifier].VisibilityCellQueueDic.Add(index, cell);

                cell.RectTrans.gameObject.SetActive(true);
            }
        }

        private void InitCellTransformSettings(ITableViewCell cell, int index)
        {
            var rect = _rects[index];

            cell.RectTrans.SetParent(content);
            cell.RectTrans.localScale = Vector3.one;
            cell.RectTrans.SetLocalPositionZ(0);

            if (_scrollRect.vertical)
            {
                #region 原逻辑

                // cell.RectTrans.anchoredPosition = new Vector2(rect.x, rect.y - rect.height / 2f);
                // cell.RectTrans.anchorMin = new Vector2(0f, 1f);
                // cell.RectTrans.anchorMax = new Vector2(1f, 1f);
                // cell.RectTrans.pivot = new Vector2(0.5f, 0.5f);
                // var v = cell.RectTrans.offsetMin;
                // v.x = 0;
                // cell.RectTrans.offsetMin = v;
                // v = cell.RectTrans.offsetMax;z
                // v.x = 0;
                // cell.RectTrans.offsetMax = v;

                #endregion

                cell.RectTrans.anchoredPosition = new Vector2(rect.x, rect.y - rect.height / 2f);
                cell.RectTrans.anchorMin = new Vector2(0.5f, 1f);
                cell.RectTrans.anchorMax = new Vector2(0.5f, 1f);
                cell.RectTrans.pivot = new Vector2(0.5f, 0.5f);
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
            var identifier = GetIdentifierForIndex(index);
            if (_groupDict.ContainsKey(identifier) &&
                _groupDict[identifier].VisibilityCellQueueDic.ContainsKey(index) == true)
            {
                var cell = _groupDict[identifier].VisibilityCellQueueDic[index];
                _groupDict[identifier].InvisibilityCellQueue.Add(cell);
                _groupDict[identifier].VisibilityCellQueueDic.Remove(index);
                cell.RectTrans.gameObject.SetActive(false);
            }
        }

        private int IndexAtOffset(float offset)
        {
            var minIndex = 0;
            var maxIndex = _numberOfCells - 1;
            maxIndex = maxIndex < 0 ? 0 : maxIndex;
            var index = (maxIndex + minIndex) / 2;

            if (_scrollRect.vertical)
            {
                while (minIndex < maxIndex)
                {
                    var indexY = _rects[index].y;
                    var nextY = _rects[index + 1].y;

                    if (indexY >= offset && nextY < offset) break;

                    if (nextY >= offset) minIndex = index + 1;
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

                    if (nextX >= offset) minIndex = index + 1;
                    else maxIndex = index;

                    index = (maxIndex + minIndex) / 2;
                }
            }

            return index;
        }

        private Vector2 GetMinMaxVisibleIndex()
        {
            var trans = Transform as RectTransform;
            var offset = content.anchoredPosition;

            var viewWidth = trans.rect.width;
            var viewHeight = trans.rect.height;

            var minIndex = _scrollRect.vertical ? IndexAtOffset(-offset.y) : IndexAtOffset(offset.x);
            var maxIndex = _scrollRect.vertical
                ? IndexAtOffset(-offset.y - viewHeight)
                : IndexAtOffset(offset.x - viewWidth);

            var boundMinIndex = 0;
            var boundMaxIndex = _numberOfCells - 1; // Mathf.Max(0, numberOfCells - 1);
            minIndex = minIndex - _extraBuffer / 2;
            minIndex = minIndex < boundMinIndex ? boundMinIndex : minIndex;
            maxIndex = maxIndex + _extraBuffer / 2;
            maxIndex = maxIndex > boundMaxIndex ? boundMaxIndex : maxIndex;

            return new Vector2(minIndex, maxIndex);
        }

        private int GetCellCountForOneScreen()
        {
            var trans = Transform as RectTransform;
            // var offset = content.anchoredPosition;

            var viewWidth = trans.rect.width;
            var viewHeight = trans.rect.height;


            var count = (_scrollRect.vertical ? viewHeight : viewWidth) / _sizeOfIndex(this, 0);
            return (int)(count + 0.5f);
        }

        public void ReAllocateCellList()
        {
            _numberOfCells = NumberOfCells(this);
            foreach (var tableViewCell in _cellsList)
            {
                tableViewCell?.RemoveFromParent();
            }

            _groupDict.Clear();

            _cellsList = new ITableViewCell[_numberOfCells];
        }

        /// <summary>
        /// 重新加载数据
        /// </summary>
        /// <param name="startIndex">重新加载后跳转到第startIndex行</param>
        public virtual void ReloadData(ReloadType reloadType = ReloadType.Keep, int startIndex = 0,
            bool indexAtCenter = true, bool clearCell = false)
        {
            if (NumberOfCells == null) return;

            //清理
            if (clearCell)
            {
                _groupDict.Clear();
                // _proxy.content.Clear();
                Utils.DestroyAllChildren(content);
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
            if (_identifierOfIndex == null)
            {
                if (_groupDict.ContainsKey(DEFAULT_IDENTIFIER) == false)
                    _groupDict[DEFAULT_IDENTIFIER] = new TableViewGroup();
            }
            else
            {
                createIdentifierGroup = (i) =>
                {
                    string identifier = _identifierOfIndex(this, i);
                    if (_groupDict.ContainsKey(identifier) == false) _groupDict[identifier] = new TableViewGroup();
                };
            }

            //计算content的大小
            _numberOfCells = NumberOfCells(this);
            _rects = new Rect[_numberOfCells];
            var width = 0f;
            var x = 0f;
            var height = 0f;
            var y = 0f;
            var contentPos = content.anchoredPosition;
            var contentSize = content.sizeDelta;
            var trans = Transform as RectTransform;
            var scrollViewWidth = trans.rect.width;
            var scrollViewHeight = trans.rect.height;

            if (_scrollRect.vertical)
            {
                y += AppendWidthAdStart;

                for (var i = 0; i < _numberOfCells; i++)
                {
                    height = _sizeOfIndex(this, i);
                    _rects[i] = new Rect(0, -y, scrollViewWidth, height);
                    y += height;
                    if (createIdentifierGroup != null) createIdentifierGroup(i);
                }

                contentSize.y = y;

                if (_centralized) //make content centralize when few cells
                {
                    var scrollHeight = (ScrollRect.transform as RectTransform).rect.height;
                    if (contentSize.y < scrollHeight)
                    {
                        var delta = (scrollHeight - _rects.Length * height) / 2;
                        var newY = 0f;
                        for (var j = 0; j < _rects.Length; j++)
                        {
                            _rects[j] = new Rect(0, -(delta + newY), scrollViewWidth, height);
                            newY += height;
                        }

                        contentSize.y = scrollHeight;
                    }
                }

                contentSize.y += AppendWidthAdEnd;
            }
            else
            {
                x += AppendWidthAdStart;

                for (var i = 0; i < _numberOfCells; i++)
                {
                    width = _sizeOfIndex(this, i);
                    _rects[i] = new Rect(x, 0, width, scrollViewHeight);
                    x += width;

                    if (createIdentifierGroup != null) createIdentifierGroup(i);
                }

                if (x < scrollViewWidth)
                {
                    x = 0;
                }
                else
                {
                    x = x - scrollViewWidth;
                }

                x += AppendWidthAdEnd;

                contentSize.x = x;
            }

            content.sizeDelta = contentSize;

            switch (reloadType)
            {
                //设置list的偏移位置
                case ReloadType.ScrollTo:
                {
                    var minMax = GetMinMaxVisibleIndex();
                    _minVisibleIndex = (int)minMax.x;
                    _maxVisibleIndex = (int)minMax.y;
                    ScrollToIndex(startIndex, indexAtCenter);
                    break;
                }
                case ReloadType.Start:
                {
                    if (_scrollRect.vertical) contentPos.y = -scrollViewHeight;
                    else contentPos.x = scrollViewWidth;
                    content.anchoredPosition = contentPos;
                    break;
                }
                case ReloadType.Keep:
                    //do nothing
                    break;
                case ReloadType.End:
                {
                    var minMax = GetMinMaxVisibleIndex();
                    _minVisibleIndex = (int)minMax.x;
                    _maxVisibleIndex = (int)minMax.y;
                    ScrollToIndex(_numberOfCells - 1, indexAtCenter);
                }
                    break;
                default:
                    throw new Exception("类型未处理");
            }

            var indexes = GetMinMaxVisibleIndex();
            _minVisibleIndex = (int)indexes.x;
            _maxVisibleIndex = (int)indexes.y;

            //add cells
            if (_numberOfCells > 0)
            {
                for (var i = _minVisibleIndex; i <= _maxVisibleIndex; i++)
                {
                    AddCellAtIndex(i);
                }
            }

            //初始化cell的siblingIndex
            UpdateCellIndex();

            UpdateFading();
        }

        private void ScrollToIndex(int targetIndex, bool indexAtCenter)
        {
            var contentPos = GetTargetContentPos(targetIndex, indexAtCenter);

            content.anchoredPosition = contentPos;
        }

        public Vector2 GetTargetContentPos(int targetIndex, bool indexAtCenter)
        {
            var contentPos = content.anchoredPosition;

            var newTargetIndex = targetIndex;
            newTargetIndex = Mathf.Clamp(newTargetIndex, 0, NumberOfCells(this) - 1);
            // Debug.LogError($"targetIndex before:{targetIndex}");
            if (indexAtCenter)
            {
                targetIndex = ConvertIndexToCenter(targetIndex);
            }

            // Debug.LogError($"targetIndex after:{targetIndex}");


            if (_scrollRect.vertical)
            {
                var selfHeight = _rects[newTargetIndex].height; //自身高度

                var areaHeight = 0f; //滑动可视区域高度
                if (indexAtCenter)
                    areaHeight = (ScrollRect.transform as RectTransform)!.rect.height;

                //如果考虑显示区域根据AppendWidthAdEnd减少用这行
                //areaHeight = ((ScrollRect.transform as RectTransform)!.rect.height -AppendWidthAdEnd);

                var totalHeight = 0f; //从起点到目标点高度
                for (int i = 0; i <= newTargetIndex; i++)
                    totalHeight += _rects[i].height;

                float y;

                if (Reverse)
                {
                    y = totalHeight - (areaHeight * 0.5f) - (selfHeight * 0.5f) + AppendWidthAdStart;
                }
                else
                {
                    y = totalHeight - (areaHeight * 0.5f) - (selfHeight * 0.5f) + AppendWidthAdStart;
                    // y = -_rects[targetIndex].y;

                    // + _scrollRect.viewport.rect.height / 2f; // - _rects[targetIndex].height / 2 + AppendWidthAdStart;

                    // if (-_rects[targetIndex].y < _scrollRect.viewport.rect.height / 2f) y = 0f;
                }

                var maxY = _scrollRect.content.sizeDelta.y - _scrollRect.viewport.rect.height;

                contentPos.y = Mathf.Clamp(y, 0, maxY);
            }
            else
            {
                contentPos.x = _rects[targetIndex].x - _scrollRect.viewport.rect.width;
            }

            return contentPos;
        }


        private string GetIdentifierForIndex(int index)
        {
            return _identifierOfIndex == null ? DEFAULT_IDENTIFIER : _identifierOfIndex(this, index);
        }

        public ITableViewCell DequeueReusabelCell(int index)
        {
            var identifier = GetIdentifierForIndex(index);
            var invisibilityCellQueue = _groupDict[identifier].InvisibilityCellQueue;
            var count = invisibilityCellQueue.Count;
            if (count > 0)
            {
                var cell = invisibilityCellQueue[count - 1];
                invisibilityCellQueue.RemoveAt(count - 1);
                return cell;
            }

            return null;
        }

        public ITableViewCell GetCell(int index)
        {
            var identifier = GetIdentifierForIndex(index);
            if (_groupDict.ContainsKey(identifier))
            {
                var group = _groupDict[identifier];
                group.VisibilityCellQueueDic.TryGetValue(index, out var cell);
                return cell;
            }

            return null;
        }

        public List<ITableViewCell> GetVisibleCells()
        {
            var result = new List<ITableViewCell>();
            for (var i = _minVisibleIndex; i <= _maxVisibleIndex; i++)
            {
                result.Add(GetCell(i));
            }

            return result;
        }

        // private int _visibleDeltaCount = 0;

        private int ConvertIndexToCenter(int index)
        {
            // if (_visibleDeltaCount == 0) Mathf.Abs(_visibleDeltaCount = _maxVisibleIndex - _minVisibleIndex);

            var visibleDeltaCount = GetCellCountForOneScreen();
            var half = visibleDeltaCount / 2;

            if (Reverse)
            {
                index += half;
            }
            else
            {
                index -= half;
            }

            index = Mathf.Clamp(index, 0, NumberOfCells(this) - 1);

            return index;
        }

        public void ScrollToTargetIndex(int currentIndex, int targetIndex, float delta, System.Action onScrollFinish,
            Ease easyType = Ease.InSine)
        {
            ScrollEnable = false;
            _status = Status.AutoMoving;

            var contentPos = GetTargetContentPos(targetIndex, true);

            var deltaIndex = Mathf.Abs(targetIndex - currentIndex);
            var duration = deltaIndex * delta;

            var scaleDuration = 0.1f;

            //移动列表
            var sequence2 = DOTween.Sequence();
            sequence2.AppendInterval(scaleDuration);
            sequence2.Append(content.DOAnchorPos(contentPos, duration).SetEase(easyType)).onComplete =
                () => onScrollFinish?.Invoke();
        }

        public void MoveCellToTargetIndex(int moveIndex, int targetIndex, System.Action onMoveFinish)
        {
            if (moveIndex == targetIndex)
            {
                onMoveFinish?.Invoke();
                return;
            }

            ScrollEnable = false;
            _status = Status.AutoMoving;

            var centerPosIndex = ConvertIndexToCenter(targetIndex);

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
            sequence.Append(floatingCell.DOAnchorPosY(_rects[targetIndex].y - _rects[targetIndex].size.y / 2f, duration)
                .SetEase(easyType));
            sequence.Append(floatingCell.DOScale(Vector3.one, scaleDuration));
            sequence.onUpdate = () => floatingCell.SetAsLastSibling();
            sequence.onComplete = () =>
            {
                ReloadData();
                GameObject.Destroy(floatingCell.gameObject);

                _status = Status.None;
                onMoveFinish?.Invoke();
                ScrollEnable = true;
            };

            //移动列表
            var sequence2 = DOTween.Sequence();
            sequence2.AppendInterval(scaleDuration);


            sequence2.Append(content.DOAnchorPosY(-_rects[centerPosIndex].y, duration).SetEase(easyType));
            // sequence2.Append(content.DOAnchorPosY(-_rects[centerPosIndex].y, duration).SetEase(easyType));
        }

        /// <summary>
        /// 首位进出的cell和进出的百分比，可用于制作淡出效果
        /// </summary>
        public delegate void DelegateCellsFading(UITableView uiTableView, int minVisibleIndex, float minVisiblePercent,
            int maxVisibleIndex, float maxVisiblePercent);

        private DelegateCellsFading _delegateCellsFading = null;


        public void SetCellFadingDelegate(DelegateCellsFading delegateCellFading)
        {
            _delegateCellsFading = delegateCellFading;
        }

        private void UpdateMinMaxVisibleIndex()
        {
            var oldRange = new Vector2(_minVisibleIndex, _maxVisibleIndex);
            var newRange = GetMinMaxVisibleIndex();

            // Debug.LogError($"{oldRange} : {newRange}");

            var newMinIndex = (int)newRange.x;
            var newMaxIndex = (int)newRange.y;

            var minAddIndex = -1;
            var maxAddIndex = -1;
            var minRemoveIndex = -1;
            var maxRemoveIndex = -1;

            if (oldRange != newRange)
            {
                // // for remove index
                // // -------------minV-------------maxV--------
                // // ------minI----------maxI------------------
                // if (intersectRange.x == _minVisibleIndex)
                // {
                //     minRemoveIndex = (int) (intersectRange.x + intersectRange.y + 1);
                //     maxRemoveIndex = _maxVisibleIndex;
                // }
                // else
                // {
                //     minRemoveIndex = _minVisibleIndex;
                //     maxRemoveIndex = (int) (intersectRange.x - 1);
                // }
                //
                // // for add index
                // // -------------minI-------------maxI--------
                // // ------minV----------maxV------------------
                // if (intersectRange.x == newMinIndex)
                // {
                //     minAddIndex = (int) (intersectRange.x + intersectRange.y + 1);
                //     maxAddIndex = newMaxIndex;
                // }
                // else
                // {
                //     minAddIndex = newMinIndex;
                //     maxAddIndex = (int) (intersectRange.x - 1);
                // }
                if (newRange.x > oldRange.x) //新的左边界大于左边界,左边需要移除
                {
                    minRemoveIndex = (int)oldRange.x;
                    maxRemoveIndex = (int)(newRange.x - 1);
                }
                else if (newRange.x < oldRange.x) //新的左边界小于左边界,左边需要加
                {
                    minAddIndex = (int)(newRange.x);
                    maxAddIndex = (int)(oldRange.x - 1);
                }

                if (newRange.y > oldRange.y) //新的右边界大于右边界,右边需要加
                {
                    minAddIndex = (int)(oldRange.y + 1);
                    maxAddIndex = (int)newRange.y;
                }
                else if (newRange.y < oldRange.y) //新的右边界小于右边界，右边需要移除
                {
                    minRemoveIndex = (int)(newRange.y + 1);
                    maxRemoveIndex = (int)oldRange.y;
                }
            }

            _minVisibleIndex = newMinIndex;
            _maxVisibleIndex = newMaxIndex;

            for (var i = minRemoveIndex; i <= maxRemoveIndex; i++)
            {
                if (i >= 0) RemoveCellAtIndex(i);
            }

            for (var i = minAddIndex; i <= maxAddIndex; i++)
            {
                if (i >= 0) AddCellAtIndex(i);
            }
        }

        private void UpdateFading()
        {
            if (_delegateCellsFading != null)
            {
                float minVisiblePercent = 0f;
                float maxVisiblePercent = 0f;
                int minIndex = 0;
                int maxIndex = 0;

                var rectT = (Transform as RectTransform)!;
                float offsetMin = _scrollRect.vertical ? content.anchoredPosition.y : -content.anchoredPosition.x;
                float offsetMax = _scrollRect.vertical
                    ? (content.anchoredPosition.y + rectT.rect.size.y)
                    : (-content.anchoredPosition.x + rectT.rect.size.x);

                offsetMin = Mathf.Max(0, offsetMin);
                offsetMax = Mathf.Max(0, offsetMax);

                for (int i = 0; i < _numberOfCells; i++)
                {
                    if (offsetMin >= 0)
                    {
                        offsetMin -= _scrollRect.vertical ? _rects[i].size.y : _rects[i].size.x;
                        minIndex = i;
                    }

                    if (offsetMax >= 0)
                    {
                        offsetMax -= _scrollRect.vertical ? _rects[i].size.y : _rects[i].size.x;
                        maxIndex = i;
                    }
                    else
                    {
                        break;
                    }
                }

                minVisiblePercent = Mathf.Abs(offsetMin) /
                                    (_scrollRect.vertical ? _rects[minIndex].size.y : _rects[minIndex].size.x);
                maxVisiblePercent = 1f - Mathf.Abs(offsetMax) /
                    (_scrollRect.vertical ? _rects[maxIndex].size.y : _rects[maxIndex].size.x);

                _delegateCellsFading?.Invoke(this, minIndex, minVisiblePercent, maxIndex, maxVisiblePercent);
            }
        }

        public float GetCurrentPos()
        {
            return content.anchoredPosition.y;
        }

        public void SetPos(float y)
        {
            var pos = content.anchoredPosition;
            pos.y = y;
            content.anchoredPosition = pos;
        }

        public Vector2 GetCellPos(int index)
        {
            return _rects[index].position;
        }
    }
}