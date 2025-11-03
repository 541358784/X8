using UnityEngine;

namespace Framework.UI.TableView
{
    public class UITableViewCircle : UITableView
    {
        public float ArcRadiusScale;

        public override void ReloadData(ReloadType reloadType = ReloadType.Keep, int startIndex = 0, bool indexAtCenter = true, bool clearCell = false)
        {
            base.ReloadData(reloadType, startIndex, indexAtCenter, clearCell);

            if (NumberOfCells == null) return;

            UpdateCell();
        }

        protected override void OnMove(Vector2 value)
        {
            base.OnMove(value);

            UpdateCell();
        }

        private void UpdateCell()
        {
            var rectT = (Transform as RectTransform)!;
            for (int index = _minVisibleIndex; index <= _maxVisibleIndex; index++)
            {
                var cell = _cellsList[index];
                if (cell == null) continue;

                var rect = _rects[index];

                var newX = rect.x + rect.width / 2f;
                var newY = rect.y;
                var cellWorldPos = content.TransformPoint(new Vector3(newX, newY, 0));
                var radius = rectT.rect.width * ArcRadiusScale;
                var viewCenterWorldPos = Transform.position;
                var deltaX = viewCenterWorldPos.x - cellWorldPos.x;
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
}