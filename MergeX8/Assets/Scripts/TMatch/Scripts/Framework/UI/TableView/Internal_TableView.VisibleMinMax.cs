using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SomeWhere
{
    public partial class Internal_TableView
    {
        /// <summary>
        /// 获取浮动cell
        /// </summary>
        public delegate TableViewCell DelegateFloatingCell();

        /// <summary>
        /// 浮动cell的index
        /// </summary>
        /// <returns></returns>
        public delegate int DelegateFloatingIndex();

        /// <summary>
        /// 首位进出的cell和进出的百分比，可用于制作淡出效果
        /// </summary>
        public delegate void DelegateCellsFading(Internal_TableView tableView, int minVisibleIndex, float minVisiblePercent, int maxVisibleIndex, float maxVisiblePercent);

        private DelegateCellsFading _delegateCellsFading = null;
        private DelegateFloatingCell _delegateFloatingCell = null;
        private DelegateFloatingIndex _delegateFloatingIndex;

        /// <summary>
        /// 浮动在最小值的cell
        /// </summary>
        private TableViewCell _minFloatingCell;

        /// <summary>
        /// 浮动在最大值的cell
        /// </summary>
        private TableViewCell _maxFloatingCell;

        public void SetFloatingTransformAndIndex(DelegateFloatingCell delegateFloatingCell, DelegateFloatingIndex delegateFloatingIndex)
        {
            _delegateFloatingCell = delegateFloatingCell;
            _delegateFloatingIndex = delegateFloatingIndex;

            _proxy.movementType = ScrollRect.MovementType.Clamped;
        }

        public void SetCellFadingDelegate(DelegateCellsFading delegateCellFading)
        {
            _delegateCellsFading = delegateCellFading;
        }

        private void UpdateMinMaxVisibleIndex()
        {
            var oldRange = new Vector2(_minVisibleIndex, _maxVisibleIndex);
            var newRange = GetMinMaxVisibleIndex();

            // Debug.LogError($"{oldRange} : {newRange}");

            var newMinIndex = (int) newRange.x;
            var newMaxIndex = (int) newRange.y;

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
                    minRemoveIndex = (int) oldRange.x;
                    maxRemoveIndex = (int) (newRange.x - 1);
                }
                else if (newRange.x < oldRange.x) //新的左边界小于左边界,左边需要加
                {
                    minAddIndex = (int) (newRange.x);
                    maxAddIndex = (int) (oldRange.x - 1);
                }

                if (newRange.y > oldRange.y) //新的右边界大于右边界,右边需要加
                {
                    minAddIndex = (int) (oldRange.y + 1);
                    maxAddIndex = (int) newRange.y;
                }
                else if (newRange.y < oldRange.y) //新的右边界小于右边界，右边需要移除
                {
                    minRemoveIndex = (int) (newRange.y + 1);
                    maxRemoveIndex = (int) oldRange.y;
                }
            }

            _minVisibleIndex = newMinIndex;
            _maxVisibleIndex = newMaxIndex;

            for (var i = minRemoveIndex; i <= maxRemoveIndex; i++)
            {
                RemoveCellAtIndex(i);
            }

            for (var i = minAddIndex; i <= maxAddIndex; i++)
            {
                addCellAtIndex(i);
            }

            if (_delegateCellsFading != null || _delegateFloatingIndex != null)
            {
                float minVisiblePercent = 0f;
                float maxVisiblePercent = 0f;
                int minDataIndex = 0;
                int maxDataIndex = 0;

                float offsetMin = _proxy.vertical ? _proxy.content.anchoredPosition.y : -_proxy.content.anchoredPosition.x;
                float offsetMax = _proxy.vertical ? (_proxy.content.anchoredPosition.y + (_proxy.transform as RectTransform).rect.size.y) : (-_proxy.content.anchoredPosition.x + (_proxy.transform as RectTransform).rect.size.x);

                offsetMin = Mathf.Max(0, offsetMin);
                offsetMax = Mathf.Max(0, offsetMax);

                for (int i = 0; i < _numberOfCells; i++)
                {
                    if (offsetMin >= 0)
                    {
                        offsetMin -= _proxy.vertical ? _rects[i].size.y : _rects[i].size.x;
                        minDataIndex = i;
                    }

                    if (offsetMax >= 0)
                    {
                        offsetMax -= _proxy.vertical ? _rects[i].size.y : _rects[i].size.x;
                        maxDataIndex = i;
                    }
                    else
                    {
                        break;
                    }
                }

                minVisiblePercent = Mathf.Abs(offsetMin) / (_proxy.vertical ? _rects[minDataIndex].size.y : _rects[minDataIndex].size.x);
                maxVisiblePercent = 1f - Mathf.Abs(offsetMax) / (_proxy.vertical ? _rects[maxDataIndex].size.y : _rects[maxDataIndex].size.x);

                // minVisiblePercent = Mathf.Clamp(minVisiblePercent, 0f, 1f);
                // maxVisiblePercent = Mathf.Clamp(maxVisiblePercent, 0f, 1f);

                if (_delegateFloatingIndex != null)
                {
                    if (minDataIndex >= _delegateFloatingIndex()) //上部切换到了需要浮动的cell
                    {
                        if (_minFloatingCell == null)
                        {
                            var templateCell = _delegateFloatingCell();
                            var pos = new Vector2(0, (_proxy.transform as RectTransform).rect.y + (_proxy.transform as RectTransform).rect.size.y / 2 - templateCell.RectTrans.sizeDelta.y / 2f);
                            _minFloatingCell = createFloatingCell(pos, templateCell);
                            updateFloatingData();
#if UNITY_EDITOR
                            _minFloatingCell.RectTrans.name = "MinFloatingCell";
#endif
                        }

                        _minFloatingCell.RectTrans.gameObject.SetActive(_status != Status.AutoMoving);
                    }
                    else
                    {
                        _minFloatingCell?.RectTrans.gameObject.SetActive(false);
                    }

                    if (maxDataIndex <= _delegateFloatingIndex()) //下部切换到了需要浮动的cell
                    {
                        if (_maxFloatingCell == null)
                        {
                            var templateCell = _delegateFloatingCell();
                            var pos = new Vector2(0, (_proxy.transform as RectTransform).rect.y - (_proxy.transform as RectTransform).rect.size.y / 2 + templateCell.RectTrans.sizeDelta.y / 2f);
                            _maxFloatingCell = createFloatingCell(pos, templateCell);
                            updateFloatingData();
#if UNITY_EDITOR
                            _maxFloatingCell.RectTrans.name = "MaxFloatingCell";
#endif
                        }

                        _maxFloatingCell.RectTrans.gameObject.SetActive(_status != Status.AutoMoving);
                    }
                    else
                    {
                        _maxFloatingCell?.RectTrans.gameObject.SetActive(false);
                    }
                }

                _delegateCellsFading?.Invoke(this, minDataIndex, minVisiblePercent, maxDataIndex, maxVisiblePercent);
            }
        }

        private void updateFloatingData()
        {
            if (_delegateFloatingIndex != null && _delegateOnInitCell != null)
            {
                var floatingIndex = _delegateFloatingIndex();
                if (_minFloatingCell != null) _delegateOnInitCell(_minFloatingCell, floatingIndex);
                if (_maxFloatingCell != null) _delegateOnInitCell(_maxFloatingCell, floatingIndex);
            }
        }

        private TableViewCell createFloatingCell(Vector2 pos, TableViewCell templateCell)
        {
            var rectTransform = GameObject.Instantiate(templateCell.RectTrans);
            var cell = rectTransform.gameObject.AddComponent<TableViewCell>();
            initCellTransformSettings(cell, _delegateFloatingIndex());
            rectTransform.SetParent(_proxy.transform);
            rectTransform.Reset();
            rectTransform.anchoredPosition = pos;

            return cell;
        }
    }
}