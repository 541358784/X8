using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Framework.UI.ScrollView
{
    public class ListView : OptimizeScrollView
    {
        public enum VerticalFillOrder
        {
            TOP_DOWN,
            BOTTOM_UP
        };

        [Header("VerticalFillOrder Settings")] public VerticalFillOrder _verticalFill = VerticalFillOrder.TOP_DOWN;

        #region private

        private void RefreshBottomUpCellView()
        {
            ScrollCell cell;
            int startIndex = _dataSource.NumberOfCellsInTableView() - 1;
            _dataSource.currentFrontIndex = startIndex;
            _dataSource.currentBackIndex = _dataSource.currentFrontIndex;
            float viewDisplayLen =
                _scrollRect.horizontal ? _viewPortTransform.rect.width : _viewPortTransform.rect.height;
            float currentPos = 0;
            int itemIndex;
            Vector2 size;
            for (int i = _dataSource.currentFrontIndex; i >= 0; --i)
            {
                itemIndex = startIndex - i;
                _backNode = _circleList[itemIndex];
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

        #endregion

        #region oveeride

        protected override void RefreshCellView()
        {
            if (_verticalFill.Equals(VerticalFillOrder.TOP_DOWN))
            {
                base.RefreshCellView();
            }
            else
            {
                RefreshBottomUpCellView();
            }
        }

        #endregion

        #region api

        public void SetVerticalFill(VerticalFillOrder verticalFill, bool isResetPos)
        {
            if (verticalFill.Equals(_verticalFill) == false)
            {
                _verticalFill = verticalFill;
                ReloadData();
            }
        }

        #endregion
    }
}