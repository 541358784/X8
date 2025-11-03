using System.Collections.Generic;
using DragonPlus.Config.WeeklyChallenge;
using DragonU3DSDK.Storage;

namespace Farm.View
{
    public partial class UIFarmMain_Order
    {
        private void RegisterEvent()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.FARM_ORDER_REFRESH, Event_OrderRefresh);
            EventDispatcher.Instance.AddEventListener(EventEnum.FARM_REFRESH_PRODUCT, Event_RefreshProduct);
            EventDispatcher.Instance.AddEventListener(EventEnum.FARM_REFRESH_WAREHOUSE, Event_RefreshProduct);
            EventDispatcher.Instance.AddEventListener(EventEnum.FARM_ORDER_REMOVE, Event_RemoveRefresh);
            
        }

        private void UnRegisterEvent()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.FARM_ORDER_REFRESH, Event_OrderRefresh);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.FARM_REFRESH_PRODUCT, Event_RefreshProduct);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.FARM_REFRESH_WAREHOUSE, Event_RefreshProduct);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.FARM_ORDER_REMOVE, Event_RemoveRefresh);
            
        }

        private void Event_RefreshProduct(BaseEvent e)
        {
            _orderCells.ForEach(a=>a.UpdateData());
            UpdateSiblingIndex();
        }
        private void Event_OrderRefresh(BaseEvent e)
        {
            if (e != null && e.datas != null && e.datas.Length > 0)
            {
                OrderCell cell = (OrderCell)e.datas[0];
                List<StorageFarmOrderItem> orders = (List<StorageFarmOrderItem>)e.datas[1];

                if (cell != null)
                {
                    if (_orderCells.Contains(cell))
                    {
                        cell.Recycle();
                        cell.gameObject.SetActive(false);
                        _orderCells.Remove(cell);
                    }
                    _freeOrderCells.Enqueue(cell);
                }

                if (orders != null && orders.Count > 0)
                {
                    foreach (var order in orders)
                    {
                        InitOrderCell(order);
                    }
                }
                
                _orderCells.ForEach(a=>a.UpdateData());
                UpdateSiblingIndex();
            }
            else
            {
                _orderCells.ForEach(a=>a.UpdateData());
                UpdateSiblingIndex();
            }
        }

        private void Event_RemoveRefresh(BaseEvent e)
        {
            if(e == null || e.datas == null || e.datas.Length == 0)
                return;

            List<string> ids = (List<string>)e.datas[0];
            if(ids == null || ids.Count == 0)
                return;
            
            for (var i = 0; i < _orderCells.Count; i++)
            {
                if (_orderCells[i]._storage != null && ids.Contains(_orderCells[i]._storage.Id))
                {
                    _orderCells[i].Recycle();
                    _orderCells[i].gameObject.SetActive(false);
                    _freeOrderCells.Enqueue(_orderCells[i]);
                    _orderCells.Remove(_orderCells[i]);
                    i--;
                }
            }
        }
    }
}