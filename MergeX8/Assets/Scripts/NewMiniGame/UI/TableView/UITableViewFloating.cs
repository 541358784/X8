using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Framework.UI.TableView
{
    public class UITableViewFloating : UITableView
    {
        public delegate int DelegateFloatingIndex();

        private DelegateFloatingIndex _delegateFloatingIndex;

        private ITableViewCell _minFloatingCell;
        private ITableViewCell _maxFloatingCell;

        public void SetFloatingTransformAndIndex(ITableViewCell min, ITableViewCell max, DelegateFloatingIndex delegateFloatingIndex)
        {
            _minFloatingCell = min;
            _maxFloatingCell = max;

            _delegateFloatingIndex = delegateFloatingIndex;
        }

        public override void ReloadData(ReloadType reloadType = ReloadType.Keep, int startIndex = 0, bool indexAtCenter = true, bool clearCell = false)
        {
            base.ReloadData(reloadType, startIndex, indexAtCenter, clearCell);

            UpdateFloatingCells();
        }

        protected override void OnMove(Vector2 value)
        {
            base.OnMove(value);

            UpdateFloatingCells();
        }

        private void UpdateFloatingCells()
        {
            if (_delegateFloatingIndex == null) return;

            var playerIndex = _delegateFloatingIndex();

            var viewRect = (content.parent as RectTransform)!;
            var playerDownBoarder = content.TransformPoint(new Vector3(_rects[playerIndex].x, _rects[playerIndex].y - _rects[playerIndex].height)).y;
            var playerUpBoarder = content.TransformPoint(new Vector3(_rects[playerIndex].x, _rects[playerIndex].y)).y;
            var minDownBoarder = viewRect.TransformPoint(new Vector3(_minFloatingCell.RectTrans.rect.x, -viewRect.rect.height + _minFloatingCell.RectTrans.rect.height)).y;
            var maxUpBoarder = viewRect.TransformPoint(new Vector3(_maxFloatingCell.RectTrans.rect.x, _maxFloatingCell.RectTrans.rect.y - _maxFloatingCell.RectTrans.rect.height / 2f)).y;

            // Debug.DrawLine(new Vector3(0, playerDownBoarder), new Vector3(10, playerDownBoarder), Color.red, 0.1f);
            // Debug.DrawLine(new Vector3(0, playerUpBoarder), new Vector3(10, playerUpBoarder), Color.green, 0.1f);
            // Debug.DrawLine(new Vector3(0, minDownBoarder), new Vector3(10, minDownBoarder), Color.blue, 0.1f);
            // Debug.DrawLine(new Vector3(0, maxUpBoarder), new Vector3(10, maxUpBoarder), Color.yellow, 0.1f);

            if (playerDownBoarder >= maxUpBoarder)
            {
                _minFloatingCell.RectTrans.gameObject.SetActive(_status != Status.AutoMoving);
            }
            else
            {
                _minFloatingCell.RectTrans.gameObject.SetActive(false);
            }

            if (playerUpBoarder <= minDownBoarder)
            {
                _maxFloatingCell.RectTrans.gameObject.SetActive(_status != Status.AutoMoving);
            }
            else
            {
                _maxFloatingCell.RectTrans.gameObject.SetActive(false);
            }


            // var screenPos = UIManager.Instance.Camera.WorldToScreenPoint(content.TransformPoint(_rects[playerIndex].position));
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, screenPos, UIManager.Instance.Camera, out var anchoredPosition);
            // var playerRect = new Rect(anchoredPosition.x, anchoredPosition.y, _rects[playerIndex].width, _rects[playerIndex].height);

            // var delta = playerRect.position.y - playerRect.size.y / 2f;
            // if (delta >= _minFloatingCell.RectTrans.rect.yMin)
            // {
            //     _minFloatingCell.RectTrans.gameObject.SetActive(_status != Status.AutoMoving);
            // }
            // else
            // {
            //     _minFloatingCell.RectTrans.gameObject.SetActive(false);
            // }
            //
            // delta = playerRect.position.y + playerRect.size.y / 2f;
            // if (delta <= _maxFloatingCell.RectTrans.rect.yMax)
            // {
            //     _maxFloatingCell.RectTrans.gameObject.SetActive(_status != Status.AutoMoving);
            // }
            // else
            // {
            //     _maxFloatingCell.RectTrans.gameObject.SetActive(false);
            // }
        }
    }
}