using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SomeWhere
{
    public partial class Internal_TableView
    {
        public delegate void DelegateOnPageChanged(int pageIndex);


        /// <summary>
        /// 按页滑动模式，换页回调
        /// </summary>
        private DelegateOnPageChanged _delegatePageChanged = null;

        /// ------------------------------------------
        /// ------------------------------------------
        private float _startDragHorizontal;
        private bool _isDrag = false; //是否拖拽结束  
        private bool _moving = false;
        private float _startTime;
        public float _smooting = 4; //滑动速度  
        private float _targethorizontal = 0; //滑动的起始坐标  
        private List<float> _posList = new List<float>(); //求出每页的临界角，页索引从0开始  
        public float _dragMaxDeltaOfScreen = 0.3f; //最大滑动距离(超出后按一页)
        private int _currentPageIndex = -1;

        public void SetOnPageChangedDelegate(DelegateOnPageChanged delegatePageChanged)
        {
            _delegatePageChanged = delegatePageChanged;
        }

        private void InitForPageView()
        {
            _posList.Clear();
            for (int i = 0; i < _numberOfCells; i++)
            {
                _posList.Add(i * (1f / (_numberOfCells - 1)));
            }
        }

        private void updatePageScroll()
        {
            if (!_proxy.PageScroll) return;

            if (!_isDrag && _moving)
            {
                _startTime += 0.05f;
                float t = _startTime * _smooting;
                _proxy.horizontalNormalizedPosition = Mathf.Lerp(_proxy.horizontalNormalizedPosition, _targethorizontal, t);
                if (t >= 1)
                    _moving = false;
            }
        }

        void OnBeginDrag(PointerEventData eventData)
        {
            if (!_proxy.PageScroll) return;

            _moving = true;
            _isDrag = true;
            _startDragHorizontal = _proxy.horizontalNormalizedPosition;
        }

        void OnEndDrag(PointerEventData eventData)
        {
            if (!_proxy.PageScroll) return;

            float posX = _proxy.horizontalNormalizedPosition;

            var delta = _proxy.horizontalNormalizedPosition - _startDragHorizontal;
            var sign = Mathf.Sign(delta);
            delta = Mathf.Abs(delta);

            delta *= _posList.Count;
            var targetIndex = _currentPageIndex;
            if (delta >= _dragMaxDeltaOfScreen)
            {
                targetIndex += (int)sign;
            }

            GotoPage(targetIndex);
        }

        private void GotoPage(int index)
        {
            if (_currentPageIndex != index)
            {
                if (index < 0 || index >= _posList.Count)
                    return;
                _targethorizontal = _posList[index];
                _currentPageIndex = index;
                _delegatePageChanged?.Invoke(index);
            }
            
            // fix bug: It doesn't move When drag a little.
            _moving = true;
            _isDrag = false;
            _startTime = 0;
        }
    }
}