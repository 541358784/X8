using System.Collections.Generic;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using SomeWhere;

namespace Merge.Order
{
    public class MainOrderCreatorRecommend
    {
        public static SlotDefinition _orgSlot = SlotDefinition.Recomment;
        public static MainOrderType _orgType = MainOrderType.Recomment;

        private static long _orderRefreshTime = 0;
        private static string orderRefreshTimeKey = ((int)_orgSlot).ToString();
        public static bool CanCreate(SlotDefinition slot)
        {
            _orderRefreshTime = 0;
            if (MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.ContainsKey(orderRefreshTimeKey))
                _orderRefreshTime = MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[orderRefreshTimeKey];
            
            return MainOrderCreatorRandomCommon.CanCreate(orderRefreshTimeKey, _orgType,_orgSlot, slot);
        }

        public static bool RefreshRecommend()
        {
            var order = MainOrderManager.Instance.StorageTaskGroup.CurTasks.Find(a => a.Slot == (int)_orgSlot);
            if (order == null)
                return false;
            
            Dictionary<int, int> orderItemCountMap = MainOrderCreatorRandomCommon.GetOrderItemCount();
            var codeCountMap = MergeManager.Instance.GetCodeCountMap(true, false, true);
            if (codeCountMap == null)
                return false;

            int needNum = 0;
            if (orderItemCountMap.ContainsKey(order.ItemIds[0]))
                needNum += orderItemCountMap[order.ItemIds[0]];
            
            int haveNum = 0;
            if (codeCountMap.ContainsKey(order.ItemIds[0]))
                haveNum = codeCountMap[order.ItemIds[0]];

            if (haveNum >= needNum)
                return false;

            long refreshTime = 0;
            if (MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.ContainsKey("Refresh_Recommend"))
                refreshTime = MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime["Refresh_Recommend"];
            
            if ((long)APIManager.Instance.GetServerTime() / 1000 - refreshTime < 24 * 60 * 60)
                return false;

            MainOrderManager.Instance.StorageTaskGroup.CurTasks.Remove(order);
            MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[orderRefreshTimeKey] = -1;

            return true;
        }
        
        public static StorageTaskItem CreateOrder(SlotDefinition slot)
        {
            var baselineOrder = CreateBaselineOrder(_orgType, slot);

            if (baselineOrder == null)
                MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[orderRefreshTimeKey] = _orderRefreshTime;
            else
            {
                MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime["Refresh_Recommend"] = (long)APIManager.Instance.GetServerTime() / 1000;
            }
            
            return baselineOrder;
        }
        
        public static StorageTaskItem CreateBaselineOrder(MainOrderType orderType, SlotDefinition slot)
        {
            var baselineAvailableItems = GetBaselineAvailableItems(slot);
            if (baselineAvailableItems == null || baselineAvailableItems.Count == 0)
                return null;

            AvailableItem availableItem = baselineAvailableItems.RandomPickOne();
            List<int> requirements = new List<int>();
            List<int> seatIndex = new List<int>();
            for (int i = 0; i < availableItem._recommendedNumber; i++)
            {
                requirements.Add(availableItem._orderItemId);
                seatIndex.Add(i+1);
            }
            
            return MainOrderManager.Instance.AddTask(MainOrderManager.Instance.StorageTaskGroup.OnlyId++, requirements.ToArray(), seatIndex, orderType, slot, isRecommended:true);
        }
        
        public static List<AvailableItem> GetBaselineAvailableItems(SlotDefinition slot)
        {
            Dictionary<int, int> orderItemCountMap = MainOrderCreatorRandomCommon.GetOrderItemCount();

            List<AvailableItem> availableItems = new List<AvailableItem>();
            
            var codeCountMap = MergeManager.Instance.GetCodeCountMap(true, false, true);
            if (codeCountMap == null)
                return null;
            
            foreach (var kv in codeCountMap)
            {
                var config = OrderConfigManager.Instance.GetOrderItem(kv.Key);
                if(config == null)
                    continue;
                
                if(config.recommendedNumber <= 0)
                    continue;
                
                if(config.recommendedSlots != null && !config.recommendedSlots.Contains((int)slot))
                    continue;

                int orderCount = 0;
                if (orderItemCountMap.ContainsKey(kv.Key))
                    orderCount = orderItemCountMap[kv.Key];

                if (config.tiggerNumber <= kv.Value - orderCount)
                {
                    AvailableItem item = new AvailableItem();
                    item._orderItemId = kv.Key;
                    item._recommendedNumber = config.recommendedNumber;
                    
                    availableItems.Add(item);
                }
            }

            return availableItems;
        }
    }
}