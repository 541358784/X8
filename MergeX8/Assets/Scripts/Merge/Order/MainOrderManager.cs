using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using SomeWhere;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

namespace Merge.Order
{
    public partial class MainOrderManager : Manager<MainOrderManager>
    {
        public StorageTaskGroup StorageTaskGroup
        {
            get
            {
               return StorageManager.Instance.GetStorage<StorageGame>().TaskGroups;
            }
        }

        private bool _isInvoke = false;

        public List<StorageTaskItem> TryFillOrder(StorageTaskItem taskItem = null)
        {
            InitInvokeUpdate();
            AdaptOrder();

            List<StorageTaskItem> newOrder = new List<StorageTaskItem>();
            for (var i = SlotDefinition.Slot1; i < SlotDefinition.SlotCount; i++)
            {
                if(StorageTaskGroup.CurTasks.Find(a => a.Slot == (int)i) != null)
                    continue;

                var order = TryCreateOrder(i);
                if(order != null)
                    newOrder.Add(order);
            }

            if (StorageTaskGroup.CurTasks.Find(a => a.Slot == (int)SlotDefinition.Recomment) == null && MainOrderCreatorRecommend.CanCreate(SlotDefinition.Recomment))
            {
                var recommentOrder = MainOrderCreatorRecommend.CreateOrder(SlotDefinition.Recomment);
                if (recommentOrder != null)
                    newOrder.Add(recommentOrder);
            }

            if (MainOrderCreateRecycle.CanCreate(taskItem))
            {
                var order = MainOrderCreateRecycle.TryCreateOrder(taskItem);
                if(order != null)
                    newOrder.Add(order);
            }
            
            if (MainOrderCreateTeam.CanCreate())
            {
                var order = MainOrderCreateTeam.TryCreateOrder();
                if(order != null)
                    newOrder.Add(order);
            }
            
            var appendOrder = MainOrderCreateAppend.TryCreateOrder();
            if(appendOrder != null)
                newOrder.Add(appendOrder);
            
            var timeOrder = MainOrderCreateTime.TryCreateOrder();
            if(timeOrder != null && timeOrder.Count > 0)
                newOrder.AddRange(timeOrder);

            var orders = TryCreateOrder(taskItem);
            if (orders != null && orders.Count > 0)
                newOrder.AddRange(orders);
            
            return newOrder;
        }
        
        public bool IsCompleteOrder(int id)
        {
            return StorageTaskGroup.CompletedTaskIds.ContainsKey(id);
        }

        private void AdaptOrder()
        {
            AdaptOrderNum();
            
            if(StorageTaskGroup.AdaptOrder)
                return;

            if (StorageTaskGroup.DynamicNormalIndex > 0)
            {
                StorageTaskGroup.OrderFixIndex = OrderConfigManager.Instance.OrderFixConfigs.Count;
            }
            else
            {
                for(int i = 0; i< OrderConfigManager.Instance.OrderFixConfigs.Count; i++)
                {
                    var fixConfig = OrderConfigManager.Instance.OrderFixConfigs[i];
                    if (IsCompleteOrder(fixConfig.id))
                        StorageTaskGroup.OrderFixIndex = Math.Max(StorageTaskGroup.OrderFixIndex, i+1);

                    if (StorageTaskGroup.CurTasks.Find(a => a.Id == fixConfig.id) != null)
                        StorageTaskGroup.OrderFixIndex = Math.Max(StorageTaskGroup.OrderFixIndex, i+1);
                }
            }

            int slot = (int)SlotDefinition.Slot1;
            foreach (var storageTaskItem in StorageTaskGroup.CurTasks)
            {
                if (OrderConfigManager.Instance.OrderFixConfigs.Find(a => a.id == storageTaskItem.Id) != null)
                {
                    storageTaskItem.Slot = slot++;
                    storageTaskItem.Type = (int)MainOrderType.Fixed;
                }

                if(IsSpecialTask(storageTaskItem.Id))
                {
                    storageTaskItem.Type = (int)MainOrderType.SpecialGold;
                    storageTaskItem.Slot = (int)SlotDefinition.SpecialSlot;
                }

                if (storageTaskItem.RewardTypes.Contains((int)UserData.ResourceId.RareDecoCoin))
                {
                    storageTaskItem.Type = (int)MainOrderType.Branch;
                    storageTaskItem.Slot = (int)SlotDefinition.BranchSlot;
                }
            }

            StorageTaskGroup.AdaptOrder = true;
        }

        private void AdaptOrderNum()
        {
            if(StorageTaskGroup.AdaptOrderNum)
                return;

            StorageTaskGroup.AdaptOrderNum = true;
            StorageTaskGroup.CompleteOrderNum = StorageTaskGroup.CompletedTaskIds.Count;
            var kes = new List<int>(StorageTaskGroup.CompletedTaskIds.Keys);
            foreach (var ke in kes)
            {
                if(ke >= 100001)
                    continue;

                StorageTaskGroup.CompletedTaskIds.Remove(ke);
            }
        }
        
        private void InitInvokeUpdate()
        {
            if (_isInvoke)
                return;
            
            CancelInvoke("AutoTryFillOrder");
            InvokeRepeating("AutoTryFillOrder", 10, 30);
            _isInvoke = true;
        }
        
        private StorageTaskItem TryCreateOrder(SlotDefinition slot)
        {
            var order = MainOrderCreatorFix.CreateOrder(slot);
            if (order == null && MainOrderCreatorReturnFreeOrder.CanCreate(slot))
                order = MainOrderCreatorReturnFreeOrder.CreateOrder(slot);

            if (order == null)
                order = MainOrderCreatorRandomCommon.CreateRandomOrders(slot);
            
            return order;
        }

        public void AutoTryFillOrder()
        {
            MainOrderForceRefreshOrderForReturnUser.ForceRefreshOrderForReturnUser();
            
            List<StorageTaskItem> taskItems = TryFillOrder();
            if(taskItems == null)
                return;
            
            if(MergeTaskTipsController.Instance == null || !MergeTaskTipsController.Instance.gameObject.activeInHierarchy)
                return;
            
            MergeTaskTipsController.Instance.RefreshTask(taskItems, () =>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main);
                MergeMainController.Instance?.UpdateTaskRedPoint();
            });
        }

        public StorageTaskItem AddTask(int id, int[] requirements, List<int> seatIndex, MainOrderType type, SlotDefinition slot, int headId = 0, bool isRecommended = false, int[] rewardIds = null, int[] rewardNums = null, int[] extraRewardIds = null, int[] extraRewardNums = null, int coinFactor = 0)
        {
            StorageTaskItem taskItem = new StorageTaskItem();
            taskItem.Id = id < 0 ? MainOrderManager.Instance.StorageTaskGroup.OnlyId++ : id;
            taskItem.HeadIndex = headId > 0 ? headId : RandomHeadIndex();
            taskItem.Type = (int)type;
            taskItem.Slot = (int)slot;

            if (slot == SlotDefinition.BranchSlot)
            {
                taskItem.Type = (int)MainOrderType.Branch;
            }

            int price = 0;
            for (int i = 0; i < requirements.Length; i++)
            {
                if(requirements[i] <= 0)
                    continue;
                
                taskItem.ItemIds.Add(requirements[i]);
                taskItem.ItemNums.Add(1);

                price += OrderConfigManager.Instance.GetItemPrice(requirements[i]);
            }

            if (coinFactor > 0)
            {
                price = (int)(price * (coinFactor / 100f));
                price = Math.Max(1, price);
            }
            
            if (seatIndex != null)
            {
                for (int i = 0; i < seatIndex.Count; i++)
                    taskItem.ItemSeatIndex.Add(seatIndex[i]);
            }

            taskItem.DogCookiesNum = GetDogCoin(price);

            if (extraRewardIds != null)
            {
                foreach (var rewardId in extraRewardIds)
                {
                    taskItem.ExtraRewardTypes.Add(rewardId);
                }
            }
            if (extraRewardNums != null)
            {
                foreach (var num in extraRewardNums)
                {
                    taskItem.ExtraRewardNums.Add(num);
                }
            }
            
            if (rewardIds != null && rewardIds.Length > 0)
            {
                foreach (var rewardId in rewardIds)
                {
                    taskItem.RewardTypes.Add(rewardId);
                }
                
                foreach (var rewardId in rewardNums)
                {
                    taskItem.RewardNums.Add(rewardId);
                }
            }
            else
            {
                taskItem.RewardTypes.Add(taskItem.Slot == (int)SlotDefinition.BranchSlot ? (int)UserData.ResourceId.RareDecoCoin : (int)UserData.ResourceId.Coin);
                taskItem.RewardNums.Add(price);
            }

            taskItem.OrgId = taskItem.Id;
            taskItem.IsHard = false;
            
            if (isRecommended)
                taskItem.IsHard = true;
            
            StorageTaskGroup.CurTasks.Add(taskItem);
            EventDispatcher.Instance.SendEventImmediately(new EventCreateMergeOrder(taskItem));
    
         string biParam = "";
         for(int i = 0; i < taskItem.ItemIds.Count; i++)
             biParam += taskItem.ItemIds[i] + ":" + taskItem.ItemNums[i] + " ";
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventAppearTask,taskItem.OrgId + "_" + taskItem.Type, biParam);
        
            return taskItem;
        }

        public void RemoveOrder(int id)
        {
            for (var i = 0; i < StorageTaskGroup.CurTasks.Count; i++)
            {
                if(StorageTaskGroup.CurTasks[i].OrgId != id)
                    continue;
                
                StorageTaskGroup.CurTasks.RemoveAt(i);
                i--;
            }

            EventDispatcher.Instance.DispatchEvent(EventEnum.ORDER_REMOVE_REFRESH,id);
        }

        public void RemoveOrderByComplete(int id)
        {
            StorageTaskGroup.CompletedTaskIds.Remove(id);
        }
        
        public void RemoveOrder(MainOrderType type)
        {
            int id = -1;
            for (var i = 0; i < StorageTaskGroup.CurTasks.Count; i++)
            {
                if(StorageTaskGroup.CurTasks[i].Type != (int)type)
                    continue;
                
                id = StorageTaskGroup.CurTasks[i].Id;
                StorageTaskGroup.CurTasks.RemoveAt(i);
                i--;
            }

            EventDispatcher.Instance.DispatchEvent(EventEnum.ORDER_REMOVE_REFRESH, id);
        }
        
        public int GetDogCoin(int price)
        {
            int coin = ((price/20)+1)*2;
            coin = Math.Min(coin, 30);
            return coin;
        }
    
        public bool IsUnlockMergeItem(int mergeId)
        {
            return MergeManager.Instance.IsGetItem(mergeId);
        }
    }
}