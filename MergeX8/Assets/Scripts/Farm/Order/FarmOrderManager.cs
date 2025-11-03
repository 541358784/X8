using System;
using System.Collections.Generic;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Storage;
using Farm.Model;
using Farm.Order.Creator;
using Farm.View;
using Gameplay;
using SomeWhere;
using UnityEngine.Serialization;

namespace Farm.Order
{
    public partial class FarmOrderManager : Manager<FarmOrderManager>
    {
        public StorageFarmOrder FarmOrder
        {
            get
            {
                return FarmModel.Instance.storageFarm.Order;
            }
        }

        private const string _orderKey = "Key_{0}_{1}";
        
        private List<Creator.Creator> _procedureMain = new List<Creator.Creator>()
        {
            new Creator.Creator(Creator_Fix.CanCreate, Creator_Fix.TryCreateOrder),
            new Creator.Creator(Creator_Common.CanCreate, Creator_Common.TryCreateOrder),
        };

        public List<StorageFarmOrderItem> TryCreateOrder(StorageFarmOrderItem finishOrder = null)
        {
            List<StorageFarmOrderItem> createOrders = new List<StorageFarmOrderItem>();
            foreach (var slot in Enum.GetValues(typeof(OrderSlot)))
            {
                if (FarmOrder.Orders.Find(a => a.Slot == (int)slot) != null)
                    continue;

                if(!IsSlotUnLock((OrderSlot)slot))
                    continue;
                    
                var order = TryCreateOrder((OrderSlot)slot, finishOrder);
                if (order == null)
                    continue;

                createOrders.Add(order);
            }

            return createOrders;
        }
        
        private StorageFarmOrderItem TryCreateOrder(OrderSlot slot, StorageFarmOrderItem finishOrder = null)
        {
            foreach (var creator in _procedureMain)
            {
                if (!creator._canCreate.Invoke(slot))
                    continue;

                var order = creator._tryCreate(slot, finishOrder);
                if (order == null)
                    continue;

                return order;
            }

            return null;
        }

        private bool IsSlotUnLock(OrderSlot slot)
        {
            if (slot == OrderSlot.SlotFix)
                return true;
            
            var config = FarmConfigManager.Instance.TableFarmOrderSlotList.Find(a => a.Id == (int)slot);
            if (config == null)
                return false;

            if (config.UnlockLevel < 0)
                return false;
            
            return FarmModel.Instance.GetLevel() >= config.UnlockLevel;
        }
        
        
        public StorageFarmOrderItem CreatorOrder(int id, List<int> requirementIds, List<int> requirementsNums, OrderSlot slot, List<int> rewardIds = null, List<int> rewardNums = null)
        {
            StorageFarmOrderItem orderItem = new StorageFarmOrderItem();
            orderItem.Id = id < 0 ? string.Format(_orderKey, ++FarmOrder.OnlyId, FarmOrder.OnlyId) : string.Format(_orderKey, ++FarmOrder.OnlyId, id);
            orderItem.Slot = (int)slot;
            orderItem.OrgId = id > 0 ? id.ToString() : "";
            orderItem.HeadIndex = RandomHeadIndex();
            
            int price = 0;
            for (int i = 0; i < requirementIds.Count; i++)
            {
                int needId = requirementIds[i];
                int needNum = requirementsNums[i];
                if (needId <= 0)
                    continue;

                orderItem.NeedItemIds.Add(needId);
                orderItem.NeedItemNums.Add(needNum);

                price += FarmConfigManager.Instance.GetFarmProductPrice(needId)*needNum;
            }

            if (rewardIds != null && rewardIds.Count > 0)
            {
                foreach (var rewardId in rewardIds)
                {
                    orderItem.RewardIds.Add(rewardId);
                }

                foreach (var rewardId in rewardNums)
                {
                    orderItem.RewardNums.Add(rewardId);
                }
            }
            else
            {
                orderItem.RewardIds.Add((int)UserData.ResourceId.Farm_Exp);
                orderItem.RewardNums.Add(price);
            }

            FarmOrder.Orders.Add(orderItem);
            // string biParam = "";
            // for (int i = 0; i < orderItem.NeedItemIds.Count; i++)
            //     biParam += orderItem.NeedItemIds[i] + ":" + orderItem.NeedItemIds[i] + " ";
            // GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventNewAppearTask, orderItem.OrgId + "_" + orderItem.Type, biParam);

            FarmModel.Instance.SendBI_CreateOrder(orderItem);
            
            return orderItem;
        }

        public void RemoveOrder(int slot)
        {
            List<string> ids = new List<string>();
            for (var i = 0; i < FarmOrder.Orders.Count; i++)
            {
                if(FarmOrder.Orders[i].Slot != slot)
                    continue;

                ids.Add(FarmOrder.Orders[i].Id);
                FarmOrder.Orders.RemoveAt(i);
                i--;
            }
            
            EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_ORDER_REMOVE, ids);
        }

        private List<int> _headIndexList = new List<int>();
        public int RandomHeadIndex()
        {
            _headIndexList.Clear();
            foreach (var spine in OrderConfigManager.Instance.GetHeadSpines(OrderConfigManager.SpineType.Normal))
            {
                _headIndexList.Add(spine.id);
            }

            for (int i = 0; i < FarmOrder.Orders.Count; i++)
            {
                if (FarmOrder.Orders[i].HeadIndex <= 0)
                    continue;

                if (!_headIndexList.Contains(FarmOrder.Orders[i].HeadIndex))
                    continue;

                _headIndexList.Remove(FarmOrder.Orders[i].HeadIndex);
            }

            int hdIndex = 0;
            if (_headIndexList.Count > 0)
                hdIndex = _headIndexList[UnityEngine.Random.Range(0, _headIndexList.Count)];
            else
                hdIndex = OrderConfigManager.Instance.GetHeadSpines(OrderConfigManager.SpineType.Normal).RandomPickOne().id;

            return hdIndex;
        }
    }
}