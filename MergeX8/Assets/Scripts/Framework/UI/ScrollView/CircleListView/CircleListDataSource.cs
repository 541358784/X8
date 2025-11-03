using UnityEngine;

namespace Framework.UI.ScrollView
{
    public class CircleListDataSource : ScrollDataSource
    {
        public CircleListDataSource(bool isNeedCircle = true)
        {
            IsNeedCircle = isNeedCircle;
        }

        public override ScrollCell CreateCellAtInfo(ScrollCellTransformInfo info)
        {
            GameObject obj = new GameObject("CircleListCell");
            if (info != null)
            {
                RectTransform rect = obj.AddComponent<RectTransform>();
                rect.anchorMax = info.anchorMax;
                rect.anchorMin = info.anchorMin;
                rect.pivot = info.pivot;
            }

            return obj.AddComponent<CircleListCell>();
        }
    }
}