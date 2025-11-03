using UnityEngine;

namespace Framework.UI.ScrollView
{
    public class ScrollDataSource
    {
        private bool _isNeedCircle = true;

        public bool IsNeedCircle
        {
            get { return _isNeedCircle; }
            set { _isNeedCircle = value; }
        }

        public int currentFrontIndex { get; set; }
        public int currentBackIndex { get; set; }

        public virtual int FetchPrevFrontIndex(OptimizeScrollView optimizeScrollView)
        {
            currentFrontIndex--;
            if (IsNeedCircle)
            {
                int count = NumberOfCellsInTableView();
                currentFrontIndex = currentFrontIndex < 0 ? count - 1 : currentFrontIndex;
                return currentFrontIndex;
            }

            currentFrontIndex = currentFrontIndex < 0 ? 0 : currentFrontIndex;
            return currentFrontIndex;
        }

        public virtual int FetchNextFrontIndex(OptimizeScrollView optimizeScrollView)
        {
            currentFrontIndex++;
            int count = NumberOfCellsInTableView();
            if (IsNeedCircle)
            {
                currentFrontIndex = currentFrontIndex >= count ? 0 : currentFrontIndex;
                return currentFrontIndex;
            }

            currentFrontIndex = currentFrontIndex >= count ? count - 1 : currentFrontIndex;
            return currentFrontIndex;
        }

        public virtual int FetchPrevBackIndex(OptimizeScrollView optimizeScrollView)
        {
            currentBackIndex--;
            if (IsNeedCircle)
            {
                int count = NumberOfCellsInTableView();
                currentBackIndex = currentBackIndex < 0 ? count - 1 : currentBackIndex;
                return currentBackIndex;
            }

            currentBackIndex = currentBackIndex < 0 ? 0 : currentBackIndex;
            return currentBackIndex;
        }

        public virtual int FetchNextBackIndex(OptimizeScrollView optimizeScrollView)
        {
            currentBackIndex++;
            int count = NumberOfCellsInTableView();
            if (IsNeedCircle)
            {
                currentBackIndex = currentBackIndex >= count ? 0 : currentBackIndex;
                return currentBackIndex;
            }

            currentBackIndex = currentBackIndex >= count ? count - 1 : currentBackIndex;
            return currentBackIndex;
        }

        #region virtual for use

        public virtual ScrollCell CreateCellAtInfo(ScrollCellTransformInfo info)
        {
            GameObject obj = new GameObject("ScrollCell");
            if (info != null)
            {
                RectTransform rect = obj.AddComponent<RectTransform>();
                rect.anchorMax = info.anchorMax;
                rect.anchorMin = info.anchorMin;
                rect.pivot = info.pivot;
            }

            return obj.AddComponent<ScrollCell>();
        }

        public virtual Vector2 CellSizeForIndex(OptimizeScrollView optimizeScrollView, int idx)
        {
            return Vector2.zero;
        }

        public virtual void UpdateItemAtIndex(ScrollCell cell, int idx = 0)
        {
            cell.Idx = idx;
        }

        public virtual int NumberOfCellsInTableView()
        {
            return 0;
        }

        #endregion
    }
}