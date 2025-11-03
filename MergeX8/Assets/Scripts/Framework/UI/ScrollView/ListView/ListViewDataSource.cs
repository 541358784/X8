using UnityEngine;

namespace Framework.UI.ScrollView
{
    public class ListViewDataSource : ScrollDataSource
    {
        #region override

        public override ScrollCell CreateCellAtInfo(ScrollCellTransformInfo info)
        {
            return base.CreateCellAtInfo(info);
        }

        public override int FetchPrevFrontIndex(OptimizeScrollView optimizeScrollView)
        {
            if (IsUpDown(optimizeScrollView))
            {
                return base.FetchPrevFrontIndex(optimizeScrollView);
            }

            return base.FetchNextFrontIndex(optimizeScrollView);
        }

        public override int FetchNextFrontIndex(OptimizeScrollView optimizeScrollView)
        {
            if (IsUpDown(optimizeScrollView))
            {
                return base.FetchNextFrontIndex(optimizeScrollView);
            }

            return base.FetchPrevFrontIndex(optimizeScrollView);
        }

        public override int FetchPrevBackIndex(OptimizeScrollView optimizeScrollView)
        {
            if (IsUpDown(optimizeScrollView))
            {
                return base.FetchPrevBackIndex(optimizeScrollView);
            }

            return base.FetchNextBackIndex(optimizeScrollView);
        }

        public override int FetchNextBackIndex(OptimizeScrollView optimizeScrollView)
        {
            if (IsUpDown(optimizeScrollView))
            {
                return base.FetchNextBackIndex(optimizeScrollView);
            }

            return base.FetchPrevBackIndex(optimizeScrollView);
        }

        #endregion

        #region private

        private bool IsUpDown(OptimizeScrollView optimizeScrollView)
        {
            ListView listView = optimizeScrollView as ListView;
            if (listView != null)
            {
                return listView._verticalFill.Equals(ListView.VerticalFillOrder.TOP_DOWN);
            }

            return false;
        }

        #endregion
    }
}