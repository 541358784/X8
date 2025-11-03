using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Storage;

namespace Merge.Order
{
    public partial class MainOrderManager
    {
        private Func<StorageTaskItem, List<StorageTaskItem>>[] _createFunction1 = new Func<StorageTaskItem, List<StorageTaskItem>>[]
        {
            MainOrderCreateAppend.TryCreateOrder,
            MainOrderCreateTime.TryCreateOrder,
        };

        
        private Func<StorageTaskItem, StorageTaskItem>[] _createFunction2 = new Func<StorageTaskItem, StorageTaskItem>[]
        {
            MainOrderCreateLimitLine.TryCreateOrder,
            MainOrderCreateCraze.TryCreateOrder,
        };

        private List<StorageTaskItem> TryCreateOrder(StorageTaskItem orderItem)
        {
            List<StorageTaskItem> orders = new List<StorageTaskItem>();
            if (orderItem == null)
                return orders;
            
            foreach (var func in _createFunction1)
            {
                var funcOrders = func(orderItem);
                if(funcOrders != null && funcOrders.Count > 0)
                    orders.AddRange(funcOrders);
            }
            
            foreach (var func in _createFunction2)
            {
                var funcOrder = func(orderItem);
                if(funcOrder != null)
                    orders.Add(funcOrder);
            }

            return orders;
        }
        
        
        public void LocationOrder(MainOrderType type)
        {
            if(type == MainOrderType.None)
                return;
        
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemByType(type);
            if (tipsItem == null)
                return;
        
            MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-tipsItem.transform.localPosition.x+280, 0);
        }
    }
}