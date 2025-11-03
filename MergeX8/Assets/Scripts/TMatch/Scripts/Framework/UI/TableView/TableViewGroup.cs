using System.Collections.Generic;
using UnityEngine;

namespace SomeWhere
{
    class TableViewGroup
    {
        public Dictionary<int, TableViewCell> _visibilityCellQueueDic = new Dictionary<int, TableViewCell>();
        public List<TableViewCell> _invisibilityCellQueue = new List<TableViewCell>();

        ~TableViewGroup()
        {
            _visibilityCellQueueDic.Clear();
            _visibilityCellQueueDic = null;
            _invisibilityCellQueue.Clear();
            _invisibilityCellQueue = null;
        }

        public void QueueInvisible()
        {
            foreach (int i in _visibilityCellQueueDic.Keys)
            {
                var cell = _visibilityCellQueueDic[i];
                _invisibilityCellQueue.Add(cell);
                cell.RectTrans.gameObject.SetActive(false);
            }

            _visibilityCellQueueDic.Clear();
        }
    }
}