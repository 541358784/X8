using System;
using System.Collections.Generic;
using System.Linq;
using ABTest;
using Activity.TimeOrder;
using DragonPlus.Config.TimeOrder;
using DragonU3DSDK.Storage;
using SomeWhere;

namespace Merge.Order
{
    public class MainOrderCreateRecycle
    {
        public static SlotDefinition _orgSlot = SlotDefinition.SecondRecycle;
        public static MainOrderType _orgType = MainOrderType.SecondRecycle;
        private static string orderRefreshTimeKey = ((int)_orgSlot).ToString();
        
        public static bool CanCreate(StorageTaskItem taskItem = null)
        {
            if (!ABTestManager.Instance.IsOpenSecondRecycleOrder())
                return false;
            
            if (taskItem == null)
                return false;

            if (_orgSlot == (SlotDefinition)taskItem.Slot)
                return false;
            
            return MainOrderCreatorRandomCommon.CanCreate(orderRefreshTimeKey, _orgType, _orgSlot, _orgSlot);
        }
        
        public static StorageTaskItem TryCreateOrder(StorageTaskItem orderItem)
        {
            Dictionary<int, int> recycleMergeLine = new Dictionary<int, int>();
            Dictionary<int, int> recycleMergeItem = new Dictionary<int, int>();
            foreach (var id in orderItem.ItemIds)
            {
                var config = OrderConfigManager.Instance.GetOrderItem(id);
                if(config == null)
                    continue;
                
                if(!config.triggerSecondRecycle)
                    continue;
                
                if(config.secondRecycleMergeLine == null || config.secondRecycleMergeLine.Length == 0)
                    continue;
                
                foreach (var line in config.secondRecycleMergeLine)
                {
                    recycleMergeLine.TryAdd(line, line);
                }
                
                if(config.secondRecycleItems == null || config.secondRecycleItems.Length == 0)
                    continue;
                
                foreach (var item in config.secondRecycleItems)
                {
                    recycleMergeItem.TryAdd(item, item);
                }
            }

            if (recycleMergeLine.Count == 0 || recycleMergeItem.Count == 0)
            {
                MainOrderCreatorRandomCommon.RestRefreshTime(orderRefreshTimeKey);
                return null;
            }
            
            var itemCodeMap = MergeManager.Instance.GetCodeCountMap(true, false, true);
            var itemCodeExistMap = MainOrderCreatorRandomCommon.GetOrderItemCount();
            foreach (var kv in itemCodeExistMap)
            {
                TableMergeItem mergeItem = GameConfigManager.Instance.GetItemConfig(kv.Key);
                if(mergeItem == null)
                    continue;

                int inLine = mergeItem.in_line;
                if (recycleMergeLine.ContainsKey(inLine))
                    recycleMergeLine.Remove(inLine);
            }

            Dictionary<int, int> itemFilter = new Dictionary<int, int>();
            foreach (var kv in itemCodeMap)
            {
                if(itemFilter.ContainsKey(kv.Key))
                    continue;
                
                if(!recycleMergeItem.ContainsKey(kv.Key))
                    continue;
                
                TableMergeItem mergeItem = GameConfigManager.Instance.GetItemConfig(kv.Key);
                if(mergeItem == null)
                    continue;
                
                if(!recycleMergeLine.ContainsKey(mergeItem.in_line))
                    continue;
                
                var config = OrderConfigManager.Instance.GetOrderItem(kv.Key);
                if(config == null)
                    continue;
                
                itemFilter.TryAdd(kv.Key, config.difficulty);
            }

            if (itemFilter.Count == 0)
            {
                MainOrderCreatorRandomCommon.RestRefreshTime(orderRefreshTimeKey);
                return null;
            }
            
            var mapDescending = itemFilter.OrderByDescending(p => p.Value).ToDictionary(p => p.Key, o => o.Value);

            List<int> keys = new List<int>(mapDescending.Keys);
            int maxNum = Math.Min(3, keys.Count);
            
            List<int> requirements = new List<int>();
            
            for (int i = 0; i < maxNum; i++)
            {
                requirements.Add(keys[i]);
            }
            
            return MainOrderManager.Instance.AddTask(MainOrderManager.Instance.StorageTaskGroup.OnlyId++, requirements.ToArray(), null, _orgType, _orgSlot);
        }
    }
}