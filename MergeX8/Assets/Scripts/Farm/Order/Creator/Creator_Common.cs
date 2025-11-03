using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Storage;
using Farm.Model;
using SomeWhere;
using UnityEngine;

namespace Farm.Order.Creator
{
    public class Creator_Common
    {
        public static bool CanCreate(OrderSlot slot)
        {
            if (slot == OrderSlot.SlotFix)
                return false;
            
            return true;
        }
        
        public static StorageFarmOrderItem TryCreateOrder(OrderSlot slot, StorageFarmOrderItem finishOrder = null)
        {
            var orderItems = FarmConfigManager.Instance.GetFarmOrderItemByLevel(FarmModel.Instance.GetLevel());
            var orderWeight = GetOrderWeightConfig();
            
            
            List<int> weightList = new List<int>();
            weightList.Add(orderWeight.OneItemWeight);
            weightList.Add(orderWeight.TwoItemWeight);
            weightList.Add(orderWeight.ThreeItemWeight);
            
            int index = weightList.RandomIndexByWeight();
            if (index < 0)
                return null;

            index += 1;
            List<int> requirementIds = new List<int>();
            List<int> requirementsNums  = new List<int>();
        
            var copyItems = new List<TableFarmOrderItem>(orderItems);
            List<int> itemWeightList = new List<int>();

            int startIndex = 0;
            //first
            {
                List<TableFarmOrderItem> firstItem = new List<TableFarmOrderItem>();
                for (int i = 0; i < copyItems.Count; i++)
                {
                    if(!copyItems[i].FirstSlot.Contains((int)slot))
                        continue;
                    
                    firstItem.Add(copyItems[i]);
                }

                if (firstItem.Count > 0)
                {
                    itemWeightList.Clear();
                    foreach (var config in firstItem)
                    {
                        bool isSame = false;
                        bool isLink = false;
                        foreach (var orderItem in FarmOrderManager.Instance.FarmOrder.Orders)
                        {
                            foreach (var itemId in orderItem.NeedItemIds)
                            {
                                if (config.Id == itemId)
                                    isSame = true;

                                if (config.LinkItem != null && config.LinkItem.Contains(itemId))
                                    isLink = true;
                            }
                        }

                        if (isSame)
                        {
                            itemWeightList.Add((int)(config.Weight*config.SameReduceWeight));
                            
                            if(FarmModel.Instance.Debug_OpenFram)
                                Debug.LogError($"Farm: 物品1 相同物品 减少权重 ID{config.Id}");
                        }
                        else if (isLink)
                        {
                            itemWeightList.Add((int)(config.Weight*config.LinkReduceWeight));
                            
                            if(FarmModel.Instance.Debug_OpenFram)
                                Debug.LogError($"Farm: 物品1 关联的物品 减少权重 ID{config.Id}");
                        }
                        else
                        {
                            itemWeightList.Add(config.Weight);
                        }
                    }

                    int randomIndex = itemWeightList.RandomIndexByWeight();
                    if (randomIndex >= 0)
                    {
                        var randomConfig = firstItem[randomIndex];
                        requirementIds.Add(randomConfig.Id);
                        requirementsNums.Add(UnityEngine.Random.Range(randomConfig.NumMin, randomConfig.NumMax+1));
                    
                        copyItems.Remove(randomConfig);

                        startIndex = 1;
                    }
                }
            }
            
            for (int i = startIndex; i < index; i++)
            {
                itemWeightList.Clear();
                foreach (var config in copyItems)
                {
                    bool isSame = false;
                    bool isLink = false;
                    foreach (var orderItem in FarmOrderManager.Instance.FarmOrder.Orders)
                    {
                        foreach (var itemId in orderItem.NeedItemIds)
                        {
                            if (config.Id == itemId)
                                isSame = true;

                            if (config.LinkItem != null && config.LinkItem.Contains(itemId))
                                isLink = true;
                        }
                    }

                    if (isSame)
                    {
                        itemWeightList.Add((int)(config.Weight*config.SameReduceWeight));
                        if(FarmModel.Instance.Debug_OpenFram)
                            Debug.LogError($"Farm: 物品 关联的物品 减少权重 ID{config.Id}");
                    }
                    else if (isLink)
                    {
                        itemWeightList.Add((int)(config.Weight*config.LinkReduceWeight));
                        
                        if(FarmModel.Instance.Debug_OpenFram)
                            Debug.LogError($"Farm: 物品 关联的物品 减少权重 ID{config.Id}");
                    }
                    else
                        itemWeightList.Add(config.Weight);
                }
                
                int randomIndex = itemWeightList.RandomIndexByWeight();
                if (randomIndex < 0)
                    continue;

                var randomConfig = copyItems[randomIndex];
                requirementIds.Add(randomConfig.Id);
                requirementsNums.Add(UnityEngine.Random.Range(randomConfig.NumMin, randomConfig.NumMax+1));
                copyItems.RemoveAt(randomIndex);
            }

            if (requirementIds.Count == 0)
                return null;
            
            return FarmOrderManager.Instance.CreatorOrder(-1, requirementIds, requirementsNums, slot);

        }
        
        public static TableFarmOrderWeight GetOrderWeightConfig()
        {
            int level = FarmModel.Instance.GetLevel();
            var orderWeightList = FarmConfigManager.Instance.TableFarmOrderWeightList;

            var orderWeight = FarmConfigManager.Instance.TableFarmOrderWeightList[FarmConfigManager.Instance.TableFarmOrderWeightList.Count-1];
            for (int i = orderWeightList.Count - 1; i >= 0; i--)
            {
                if (level >= orderWeightList[i].Level)
                {
                    orderWeight = orderWeightList[i];
                    break;
                }
            }

            return orderWeight;
        }
    }
}