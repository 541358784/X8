using System.Collections.Generic;

namespace Framework.UI.TableView
{
    class TableViewGroup
    {
        public Dictionary<int, ITableViewCell> VisibilityCellQueueDic = new();
        public List<ITableViewCell>            InvisibilityCellQueue  = new();

        ~TableViewGroup()
        {
            VisibilityCellQueueDic.Clear();
            VisibilityCellQueueDic = null;
            InvisibilityCellQueue.Clear();
            InvisibilityCellQueue = null;
        }

        public void QueueInvisible()
        {
            foreach (int i in VisibilityCellQueueDic.Keys)
            {
                var cell = VisibilityCellQueueDic[i];
                InvisibilityCellQueue.Add(cell);
                cell.RectTrans.gameObject.SetActive(false);
            }

            VisibilityCellQueueDic.Clear();
        }
    }
}