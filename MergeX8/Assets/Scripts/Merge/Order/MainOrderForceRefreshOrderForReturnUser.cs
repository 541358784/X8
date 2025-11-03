using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using SomeWhere;

namespace Merge.Order
{
    public class MainOrderForceRefreshOrderForReturnUser
    {
        private static string _orderRefreshTimeKey = "MainOrder_ForceRefreshOrderForReturnUser";
        public static long _expiredTime = 24 * 60 * 60 * 1000; //一天的保护时间
        public static int _orderRefreshTime = 1*60*60; //2小时cd

        public static void ForceRefreshOrderForReturnUser()
        {
            if (!CanCreate())
                return;
/* 这块代码暂时不需要
            Dictionary<int, List<int>> sortedByKey = MergeManager.Instance.GetCodeLevelMap(true, true, false, false);
            if (sortedByKey == null || sortedByKey.Count == 0)
                return;

            List<int> alreadySelectedItems = new List<int>();

            var slot5 = SlotDefinition.Slot5;
            StorageTaskItem slot5Item = MainOrderManager.Instance.CurTaskList.Find(a => a.Slot == (int)slot5 && a.Type == (int)MainOrderType.Random4);
            if (slot5Item != null)
            {
                List<int> requirements = new List<int>();
                List<int> keys = sortedByKey.Keys.ToList();
                int firstKey = keys[keys.Count - 1]; //拿到最小等级物品
                int selectId = sortedByKey[firstKey].RandomPickOne();
                requirements.Add(selectId);
                alreadySelectedItems.Add(selectId);

                sortedByKey.Remove(firstKey);

                int secondId = -1;
                int secondLevelIdx = -1;
                if (sortedByKey != null && sortedByKey.Count > 0)
                {
                    foreach (var kv in sortedByKey)
                    {
                        if (kv.Key >= 7)
                            continue;

                        if (kv.Value == null || kv.Value.Count == 0)
                            continue;

                        secondId = kv.Value.RandomPickOne();
                        secondLevelIdx = kv.Key;
                        break;
                    }
                }

                if (secondId > 0)
                {
                    requirements.Add(secondId);
                    alreadySelectedItems.Add(secondId);
                    sortedByKey.Remove(secondLevelIdx);
                }

                //替换 slot require
            }

            var slot4 = SlotDefinition.Slot4;
            StorageTaskItem slot4Item = MainOrderManager.Instance.CurTaskList.Find(a => a.Slot == (int)slot4 && a.Type == (int)MainOrderType.Random3);
            if (slot4Item != null)
            {
                List<int> requirements = new List<int>();
                if (sortedByKey != null && sortedByKey.Count > 0)
                {
                    List<int> keys = sortedByKey.Keys.ToList();
                    int firstKey = keys[keys.Count - 1]; //拿到最小等级物品
                    int selectId = sortedByKey[firstKey].RandomPickOne();
                    requirements.Add(selectId);
                    alreadySelectedItems.Add(selectId);

                    sortedByKey.Remove(firstKey);
                }

                var availableItems = MainOrderCreatorRandomCommon.GetAvailableItems(slot4);
                availableItems = MainOrderCreatorRandomCommon.FilterByLevel(availableItems, 3, 5);
                availableItems = MainOrderCreatorRandomCommon.FilterByChainId(availableItems, new int[] { 20003, 20027 }); //面包链 和 咖啡链
                if (availableItems != null && availableItems.Count > 0)
                {
                    List<AvailableItem> notOnBoard = new List<AvailableItem>();
                    List<AvailableItem> notRequire = new List<AvailableItem>();
                    Dictionary<int, int> codeMap = MergeManager.Instance.GetCodeCountMap(true, false, false);
                    foreach (var kv in availableItems)
                    {
                        if (!codeMap.ContainsKey(kv._orderItemId))
                            notOnBoard.Add(kv);

                        if (!alreadySelectedItems.Contains(kv._orderItemId))
                            notRequire.Add(kv);
                    }

                    var items = availableItems;
                    if (notOnBoard.Count > 0)
                        items = notOnBoard;
                    else if (notRequire.Count > 0)
                        items = notRequire;

                    if (items.Count > 0)
                    {
                        int itemId = MainOrderCreatorRandomCommon.RandomAvailableItem(items);

                        requirements.Add(itemId);
                        alreadySelectedItems.Add(itemId);
                    }
                }

                if (requirements.Count > 0)
                    ; //替换物品
            }
*/
            var slot2 = SlotDefinition.Slot2;
            StorageTaskItem slot2Item = MainOrderManager.Instance.CurTaskList.Find(a => a.Slot == (int)slot2 && a.Type == (int)MainOrderType.Random1);
            if (slot2Item != null)
            {
                var availableItems = MainOrderCreatorRandomCommon.GetAvailableItems(slot2);
                availableItems = MainOrderCreatorRandomCommon.FilterByChainId(availableItems, new int[] { 20027 }); //咖啡链
                if (availableItems != null && availableItems.Count > 0)
                {
                    StorageTaskItem taskItem = MainOrderCreatorRandomCommon.CreateOrder(MainOrderType.Random1, slot2, availableItems);
                    RefreshTask(taskItem);
                }
            }
            
            var slot1 = SlotDefinition.Slot1;
            StorageTaskItem slot1Item = MainOrderManager.Instance.CurTaskList.Find(a => a.Slot == (int)slot1 && a.Type == (int)MainOrderType.Random1);
            if (slot1Item != null)
            {
                var availableItems = MainOrderCreatorRandomCommon.GetAvailableItems(slot1);
                availableItems = MainOrderCreatorRandomCommon.FilterByChainId(availableItems, new int[] { 20003 }); //面包
                if (availableItems != null && availableItems.Count > 0)
                {
                    StorageTaskItem taskItem = MainOrderCreatorRandomCommon.CreateOrder(MainOrderType.Random1, slot1, availableItems);
                    RefreshTask(taskItem);
                }
            }
        }

        private static bool CanCreate()
        {
            if ((long)APIManager.Instance.GetServerTime()- (long)StorageManager.Instance.GetStorage<StorageCommon>().InstalledAt < _expiredTime)
                return false;

            long refreshTime = 0;
            if (!MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.ContainsKey(_orderRefreshTimeKey))
            {
                MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.Add(_orderRefreshTimeKey, 0);
            }

            refreshTime = MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[_orderRefreshTimeKey];
            
            long serverTime = (long)APIManager.Instance.GetServerTime() / 1000;
            if (refreshTime == 0)
            {
                MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[_orderRefreshTimeKey] = serverTime;
                return false;
            }

            if (serverTime - refreshTime <= _orderRefreshTime)
                return false;

            MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[_orderRefreshTimeKey] = 0;
            return true;
        }

        private static void RefreshTask(StorageTaskItem taskItem)
        {
            if(taskItem == null)
                return;
            
            if(MergeTaskTipsController.Instance == null|| !MergeTaskTipsController.Instance.gameObject.activeInHierarchy)
                return;
            
            MergeTaskTipsController.Instance.RefreshTask(new List<StorageTaskItem>(){taskItem}, () =>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH, MergeBoardEnum.Main);
                MergeMainController.Instance?.UpdateTaskRedPoint();
            });
        }
    }
}