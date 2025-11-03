using System;
using System.Collections.Generic;
using DragonU3DSDK.Storage;
using Farm.Order;
using UnityEngine;

namespace Farm.View
{
    public partial class UIFarmMain_Order : MonoBehaviour, IInitContent
    {
        private GameObject _itemContent;
        private GameObject _item;

        public List<OrderCell> _orderCells = new List<OrderCell>();
        private Queue<OrderCell> _freeOrderCells = new Queue<OrderCell>();
        
        private void Awake()
        {
            _itemContent = transform.Find("Viewport/Content").gameObject;
            _item = transform.Find("Viewport/Content/Task").gameObject;
            _item.gameObject.SetActive(false);

            RegisterEvent();
        }

        public void InitContent(object content)
        {
        }

        public void UpdateData(params object[] param)
        {
        }

        private void OnDestroy()
        {
            UnRegisterEvent();

            foreach (var orderCell in _orderCells)
            {
                Destroy(orderCell.gameObject);
            }
            
            foreach (var freeOrderCell in _freeOrderCells)
            {
                Destroy(freeOrderCell.gameObject);
            }
            
            _orderCells.Clear();
            _freeOrderCells.Clear();
        }

        private void OnEnable()
        {
            if(_itemContent == null)
                return;
            
            RestOrderCell();

            foreach (var storageFarmOrderItem in FarmOrderManager.Instance.FarmOrder.Orders)
            {
                InitOrderCell(storageFarmOrderItem);
            }
            UpdateSiblingIndex();
        }

        private void RestOrderCell()
        {
            foreach (var orderCell in _orderCells)
            {
                orderCell.Recycle();
                _freeOrderCells.Enqueue(orderCell);
                orderCell.gameObject.SetActive(false);
            }
            
            _orderCells.Clear();
        }

        private OrderCell InitOrderCell(StorageFarmOrderItem orderItem)
        {
            OrderCell mono = null;
            if (_freeOrderCells.Count > 0)
            {
                mono = _freeOrderCells.Dequeue();
            }
            else
            {
                GameObject item = Instantiate(_item, _itemContent.transform);
                mono = item.gameObject.AddComponent<OrderCell>();
                mono.gameObject.SetActive(true);
                mono.InitContent(orderItem);
            }
            
            mono.gameObject.SetActive(false);
            mono.gameObject.SetActive(true);
            mono.UpdateData(orderItem);

            _orderCells.Add(mono);
            return mono;
        }

        private void UpdateSiblingIndex()
        {
            var activityTimeOrder = _orderCells.Find(a => a._storage.Slot == (int)OrderSlot.Activity_TimeOrder);
            if(activityTimeOrder != null)
                activityTimeOrder.transform.SetAsFirstSibling();
            
            for (var i = _orderCells.Count-1; i >= 0; i--)
            {
                if(!_orderCells[i].IsFinish())
                    continue;
                
                _orderCells[i].transform.SetAsFirstSibling();
            }
        }
    }
}