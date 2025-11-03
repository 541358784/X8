using System;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK;
using DragonU3DSDK.Network.ABTest;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using SomeWhere;
using UnityEngine;
using ABTestManager = ABTest.ABTestManager;

namespace Merge.Order
{
    public class AvailableItem
    {
        public int _orderItemId;
        public int _weight;
        public int _recommendedNumber;
    }
    
    public class MainOrderCreatorRandomCommon
    {
        public static CreateOrderType CreateType
        {
            get
            {
                return ABTestManager.Instance.GetCreateOrderType();
            }
        }
        
        public static bool CanCreate(string orderRefreshTimeKey, TableOrderRandom config)
        {
            long refreshTime = -1;
            if (MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.ContainsKey(orderRefreshTimeKey))
            {
                refreshTime = MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[orderRefreshTimeKey];
            }

            long serverTime = (long)APIManager.Instance.GetServerTime() / 1000;
            if (refreshTime == 0)
            {
                MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[orderRefreshTimeKey] = serverTime;
                return false;
            }

            float configRefreshTime = config.refreshLevelTime;
            if (MainOrderCreatorRandomCommon.CreateType == CreateOrderType.Difficulty)
                configRefreshTime = config.refreshDiffTime;
            
            if (configRefreshTime < 0 || serverTime - refreshTime >= configRefreshTime)
            {
                MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[orderRefreshTimeKey] = 0;
                return true;
            }

            return false;
        }
        
        public static bool CanCreate(string orderRefreshTimeKey, MainOrderType orderType, SlotDefinition slot, SlotDefinition slotNew)
        {
            if (slot != slotNew)
                return false;

            TableOrderRandom orderRandom = OrderConfigManager.Instance.GetRandomConfig((int)orderType);
            if (orderRandom.unlockLevel > ExperenceModel.Instance.GetLevel())
                return false;

            return CanCreate(orderRefreshTimeKey, orderRandom);
        }
        
        public static StorageTaskItem CreateOrder(MainOrderType orderType, SlotDefinition slot, List<AvailableItem>firstAvailableItems, List<AvailableItem>secondAvailableItems, List<AvailableItem>thirdAvailableItems)
        {
            var itemNumberWeight = OrderConfigManager.Instance._orderItemWeights;
            int level = ExperenceModel.Instance.GetLevel();
            var randomConfig = OrderConfigManager.Instance.GetRandomConfig((int)orderType);
            if (randomConfig == null)
                return null;

            TableOrderItemWeight itemWeight = null;
            for (int i = itemNumberWeight.Count - 1; i >= 0; i--)
            {
                if (level >= itemNumberWeight[i].level)
                {
                    itemWeight = itemNumberWeight[i];
                    break;
                }
            }

            if (itemWeight == null)
                return null;

            List<int> countWeightList = new List<int>();
            List<int> weightIndex = new List<int>();
            if (firstAvailableItems != null && firstAvailableItems.Count > 0)
            {
                countWeightList.Add(CreateType == CreateOrderType.Level ? itemWeight.oneItemLevelWeight : itemWeight.oneItemDiffWeight);
                weightIndex.Add(1);
            }

            if (secondAvailableItems != null && secondAvailableItems.Count > 0)
            {
                countWeightList.Add(CreateType == CreateOrderType.Level ? itemWeight.twoItemLevelWeight : itemWeight.twoItemDiffWeight);
                weightIndex.Add(2);
            }

            if (thirdAvailableItems != null && thirdAvailableItems.Count > 0)
            {
                countWeightList.Add(CreateType == CreateOrderType.Level ? itemWeight.threeItemLevelWeight : itemWeight.threeItemDiffWeight);
                weightIndex.Add(3);
            }

            int index = countWeightList.RandomIndexByWeight();
            if (index < 0)
                return null;

            List<int> requirements = new List<int>();
            List<int> seatIndex = new List<int>();
            index = weightIndex[index];
            if (index >= 1)
            {
                int itemId = RandomAvailableItem(firstAvailableItems);
                if (itemId > 0)
                {
                    requirements.Add(itemId);
                    seatIndex.Add(1);
                }
            }
            if (index >= 2)
            {
                int itemId = RandomAvailableItem(secondAvailableItems);
                if (itemId > 0)
                {
                    requirements.Add(itemId);
                    seatIndex.Add(2);
                }
            }
            if (index >= 3)
            {
                int itemId = RandomAvailableItem(thirdAvailableItems);
                if (itemId > 0)
                {
                    requirements.Add(itemId);
                    seatIndex.Add(3);
                }
            }

            if (requirements.Count == 0)
                return null;
            
            return MainOrderManager.Instance.AddTask(MainOrderManager.Instance.StorageTaskGroup.OnlyId++, requirements.ToArray(), seatIndex, orderType, slot);
        }

        public static int RandomAvailableItem(List<AvailableItem> availableItem)
        {
            if (availableItem == null || availableItem.Count == 0)
                return -1;

            List<int> weights = new List<int>();
            availableItem.ForEach(a=>weights.Add(a._weight));

            int index = weights.RandomIndexByWeight();
            if (index < 0)
                return -1;

            return availableItem[index]._orderItemId;
        }

        public static TableOrderFilter GetOrderFilterConfig(TableOrderRandom config)
        {
            int level = ExperenceModel.Instance.GetLevel();
            foreach (var filterId in config.filters)
            {
                TableOrderFilter orderFilter = OrderConfigManager.Instance.GetOrderFilter(filterId);
                if(orderFilter == null)
                    continue;
                
                if(orderFilter.playerMin > 0 && level < orderFilter.playerMin)
                    continue;
                
                if(orderFilter.playerMax > 0 && level > orderFilter.playerMax)
                    continue;

                return orderFilter;
            }

            return null;
        }

        public static StorageTaskItem CreateOrder(MainOrderType orderType, SlotDefinition slot)
        {
            MainOrderManager.Instance.RecordCreateRandomOrder();
            
            var randomConfig = OrderConfigManager.Instance.GetRandomConfig((int)orderType);
            if (randomConfig == null)
                return null;

            var selectedFilter = GetOrderFilterConfig(randomConfig);
            if (selectedFilter == null)
                return null;

            var availableItems = GetAvailableItems(slot);
            List<AvailableItem> firstAvailableItems = null;
            List<AvailableItem> secondAvailableItems = null;
            List<AvailableItem> thirdAvailableItems = null;

            switch (CreateType)
            {
                case CreateOrderType.Level:
                {
                    firstAvailableItems = FilterByLevel(availableItems, selectedFilter.firstLevelMin, selectedFilter.firstLevelMax, selectedFilter.filterLevelMergeLine);
                    secondAvailableItems = FilterByLevel(availableItems, -1, selectedFilter.secondLevelMax, selectedFilter.filterLevelMergeLine);
                    thirdAvailableItems = FilterByLevel(availableItems, -1, selectedFilter.thirdLevelMax, selectedFilter.filterLevelMergeLine);

                    break;
                }
                case CreateOrderType.Difficulty:
                {
                    firstAvailableItems = FilterByDifficulty(availableItems, selectedFilter.firstDiffMin, selectedFilter.firstDiffMax, selectedFilter.filterDiffMergeLine, slot, selectedFilter.firstFilterDiffMergeLine);
                    secondAvailableItems = FilterByDifficulty(availableItems, -1, selectedFilter.secondDiffMax, selectedFilter.filterDiffMergeLine, slot);
                    thirdAvailableItems = FilterByDifficulty(availableItems, -1, selectedFilter.thirdDiffMax, selectedFilter.filterDiffMergeLine, slot);
                    break;
                }
            }
            return CreateOrder(orderType, slot, firstAvailableItems, secondAvailableItems, thirdAvailableItems);
        }
        
        public static StorageTaskItem CreateOrder(MainOrderType orderType, SlotDefinition slot, List<AvailableItem> availableItems)
        {
            if (availableItems == null)
                return null;

            var randomConfig = OrderConfigManager.Instance.GetRandomConfig((int)orderType);
            if (randomConfig == null)
                return null;

            var selectedFilter = GetOrderFilterConfig(randomConfig);
            if (selectedFilter == null)
                return null;

            List<AvailableItem> firstAvailableItems = null;
            List<AvailableItem> secondAvailableItems = null;
            List<AvailableItem> thirdAvailableItems = null;

            switch (CreateType)
            {
                case CreateOrderType.Level:
                {
                    firstAvailableItems = FilterByLevel(availableItems, selectedFilter.firstLevelMin, selectedFilter.firstLevelMax, selectedFilter.filterLevelMergeLine);
                    secondAvailableItems = FilterByLevel(availableItems, -1, selectedFilter.secondLevelMax, selectedFilter.filterLevelMergeLine);
                    thirdAvailableItems = FilterByLevel(availableItems, -1, selectedFilter.thirdLevelMax, selectedFilter.filterLevelMergeLine);

                    break;
                }
                case CreateOrderType.Difficulty:
                {
                    firstAvailableItems = FilterByDifficulty(availableItems, selectedFilter.firstDiffMin, selectedFilter.firstDiffMax, selectedFilter.filterDiffMergeLine, slot, selectedFilter.filterLevelMergeLine);
                    secondAvailableItems = FilterByDifficulty(availableItems, -1, selectedFilter.secondDiffMax, selectedFilter.filterDiffMergeLine, slot);
                    thirdAvailableItems = FilterByDifficulty(availableItems, -1, selectedFilter.thirdDiffMax, selectedFilter.filterDiffMergeLine, slot);
                    break;
                }
            }
            return CreateOrder(orderType, slot, firstAvailableItems, secondAvailableItems, thirdAvailableItems);
        }

        public static Dictionary<int, int> GetOrderItemCount()
        {
            Dictionary<int, int> orderItemCountMap = new Dictionary<int, int>();
            foreach (var storageTaskItem in MainOrderManager.Instance.StorageTaskGroup.CurTasks)
            {
                foreach (var itemId in storageTaskItem.ItemIds)
                {
                    if(!orderItemCountMap.ContainsKey(itemId))
                        orderItemCountMap.Add(itemId, 0);

                    orderItemCountMap[itemId]++;
                }
            }

            return orderItemCountMap;
        }

        public static List<AvailableItem> GetAvailableItems(SlotDefinition slot)
        {
            //获取棋盘 背包 内的可用物品
            var itemCodeNumByMapAndInventory = MergeManager.Instance.GetCodeCountMap(true, false, true);
            var itemCodeExistMap = GetOrderItemCount();//当前任务需要的物品及其个数[itemcode]=[num]
            var itemCodeChainMinLevel = new Dictionary<int, int>();//存储当前棋盘和背包内物品合成链出现的最小等级[chinde]=level
            foreach (var kv in itemCodeNumByMapAndInventory)//计算最小等级
            {
                var itemConfig = OrderConfigManager.Instance.GetOrderItem(kv.Key);
                if(itemConfig == null || itemConfig.weightMultiple != 1)
                    continue;

                TableMergeItem mergeItem = GameConfigManager.Instance.GetItemConfig(itemConfig.id);
                if(mergeItem == null)
                    continue;

                if (!itemCodeChainMinLevel.ContainsKey(mergeItem.in_line))
                    itemCodeChainMinLevel[mergeItem.in_line] = mergeItem.level;
                else
                {
                    if(mergeItem.level < itemCodeChainMinLevel[mergeItem.in_line])
                        itemCodeChainMinLevel[mergeItem.in_line] = mergeItem.level;
                }
            }

            if (ABTestManager.Instance.IsOpenOrderABTest())
            {
                var itemCodeNumInventory = MergeManager.Instance.GetCodeCountMap(true, false, true, true);
                
                foreach (var orderExtend in OrderConfigManager.Instance._orderExtends)
                {
                    if(!itemCodeNumInventory.ContainsKey(orderExtend.id))
                        continue;

                    foreach (var mergeLine in orderExtend.extendMergeLines)
                    {
                        if(itemCodeChainMinLevel.ContainsKey(mergeLine))
                            continue;

                        itemCodeChainMinLevel[mergeLine] = 0;
                    }
                }
            }

            Dictionary<int, bool> itemChainState = new Dictionary<int, bool>();//只有在任务种出现的item state才会为true
            Dictionary<int, List<int>> itemChainMultipleListCode = new Dictionary<int, List<int>>();//存放该合成链 所有的物品
            foreach (var kv in OrderConfigManager.Instance._orderItems)//计算state listcode
            {
                var itemCode = kv.Key;
                var config = kv.Value;
                
                TableMergeItem mergeItem = GameConfigManager.Instance.GetItemConfig(itemCode);
                if(mergeItem == null)
                    continue;

                if (!itemChainState.ContainsKey(mergeItem.in_line))
                    itemChainState[mergeItem.in_line] = false;
                
                if(config.weightMultiple == 1 && itemCodeExistMap.ContainsKey(itemCode))
                    itemChainState[mergeItem.in_line] = true;

                if (!itemChainMultipleListCode.ContainsKey(mergeItem.in_line))
                    itemChainMultipleListCode[mergeItem.in_line] = new List<int>();
                
                itemChainMultipleListCode[mergeItem.in_line].Add(itemCode);
            }

            var itemChainMultiple = new Dictionary<int, int>();
            var itemCodeMultiple = new Dictionary<int, int>();
            foreach (var kv in itemChainState)
            {
                int chainId = kv.Key;
                bool bState = kv.Value;

                if (bState)//计算物品链权重 首先在任务种出现的物品 个数 <= 棋盘物品个数 那么该物品权重改为100 否则 1
                {
                    bool bAllFinish = true;
                    foreach (var itemCode in itemChainMultipleListCode[chainId])
                    {
                        int nCurBoardNum = 0;
                        if (itemCodeNumByMapAndInventory.ContainsKey(itemCode))
                            nCurBoardNum = itemCodeNumByMapAndInventory[itemCode];

                        int nOrderNum = 0;
                        if(itemCodeExistMap.ContainsKey(itemCode))
                            nOrderNum = itemCodeExistMap[itemCode];
                        
                        if (nCurBoardNum < nOrderNum)
                        {
                            bAllFinish = false;
                            break;
                        }
                    }

                    if (bAllFinish)
                        itemChainMultiple[chainId] = (int) EItemMultipleTypeOrderState.ExitMapFinishEnough;
                    else
                        itemChainMultiple[chainId] = (int) EItemMultipleTypeOrderState.NoMultipleExitMap;
                }
                else
                {
                    itemChainMultiple[chainId] = (int) EItemMultipleTypeOrderState.NoExitMap; //当前物品不需要 该条链出现的权重
                    foreach (var itemCode in itemChainMultipleListCode[chainId]) //计算单独物品出现的权重 和最小等级比对
                    {
                        TableMergeItem mergeItem = GameConfigManager.Instance.GetItemConfig(itemCode);
                        if(mergeItem == null)
                            continue;
                        
                        int nCurLevel = mergeItem.level;
                        if (itemCodeChainMinLevel.ContainsKey(chainId) && itemCodeChainMinLevel[chainId] < nCurLevel)
                            itemCodeMultiple[itemCode] = (int) EItemMultipleTypeOrderState.NoExitMapLevelUp;
                    }
                }
            }

            StorageTaskItem lastFinishOrder = null;
            if (MainOrderManager.Instance.StorageTaskGroup.LastFinishOrder.ContainsKey((int) slot))
                lastFinishOrder = MainOrderManager.Instance.StorageTaskGroup.LastFinishOrder[(int) slot];

            List<AvailableItem> availableItems = new List<AvailableItem>();
            foreach (var kv in OrderConfigManager.Instance._orderItems)
            {
                var itemCode = kv.Key;
                var itemConfig = kv.Value;

                if (itemConfig.progressUnlock == 1 && !MainOrderManager.Instance.IsUnlockMergeItem(itemCode))
                    continue;

                if(itemConfig.isRecycle)
                    continue;
                
                if (itemConfig.completeOrderId > 0)
                {
                    if(!MainOrderManager.Instance.IsCompleteOrder(itemConfig.completeOrderId))
                        continue;
                }
                
                //有code权重 用code  否则用链的权重
                int weight = CalculateWeight(itemCode, itemConfig, itemCodeExistMap, itemChainMultiple, itemCodeMultiple, slot, lastFinishOrder);
                if (weight > 0)
                {
                    AvailableItem availableItem = new AvailableItem();
                    availableItem._orderItemId = itemCode;
                    availableItem._weight = weight;
                    availableItems.Add(availableItem);
                }
            }

            return availableItems;
        }

        public static int CalculateWeight(int itemCode, TableOrderItem itemConfig, Dictionary<int, int> itemCodeExistMap,
            Dictionary<int, int> itemChainMultiple, Dictionary<int, int> itemCodeMultiple, SlotDefinition slot, StorageTaskItem lastFinishOrder)
        {
            if (lastFinishOrder != null && lastFinishOrder.ItemIds.Contains(itemCode))
            {
                //DebugUtil.Log("当前item在上一个完成的任务中出现过 不在生成");
                return 0;
            }

            if (itemCodeExistMap != null && itemCodeExistMap.ContainsKey(itemCode))
            {
                //DebugUtil.Log("当前item在当前订单中存在");
                return 0;
            }

            if (itemConfig != null && ExperenceModel.Instance.GetLevel() < itemConfig.unlockLevel)
            {
                //DebugUtil.Log("当前item不满足解锁等级 " + itemConfig.unlockLevel);
                return 0;
            }

            if (itemConfig != null && itemConfig.forbiddenSlot != null && itemConfig.forbiddenSlot.Contains((int)slot))
            {
                //DebugUtil.Log("当前item不能在这个槽位生成");
                return 0;
            }

            if (itemConfig.generators != null && itemConfig.generators.Length > 0)
            {
                bool isUnlock = false;
                for (int i = 0; i < itemConfig.generators.Length; i++)
                {
                    if (MainOrderManager.Instance.IsUnlockMergeItem(itemConfig.generators[i]))
                    {
                        isUnlock = true;
                        break;
                    }
                }
                
                if(!isUnlock)
                    return 0;
            }

            if (ABTestManager.Instance.IsOpenOrderABTest() && itemConfig.discardBuilds != null && itemConfig.discardBuilds.Length > 0)
            {
                bool isHave = false;
                for (int i = 0; i < itemConfig.discardBuilds.Length; i++)
                {
                    if (MainOrderManager.Instance.IsUnlockMergeItem(itemConfig.discardBuilds[i]))
                    {
                        isHave = true;
                        break;
                    }
                }
                
                if(isHave)
                    return 0;
            }
            
            int nMultiple = 1;
            int inLine = GameConfigManager.Instance.GetItemConfig(itemCode).in_line;
            if (itemCodeMultiple.ContainsKey(itemCode))
                nMultiple = itemCodeMultiple[itemCode];
            else if (itemChainMultiple.ContainsKey(inLine))
                nMultiple = itemChainMultiple[inLine];

            int weight = CreateType == CreateOrderType.Level ? itemConfig.levelWeight : itemConfig.diffWeight;
            return weight * nMultiple;
        }

        public static List<AvailableItem> GetAvailableDefaultItems(SlotDefinition slot)
        {
            StorageTaskItem lastFinishOrder = null;
            if (MainOrderManager.Instance.StorageTaskGroup.LastFinishOrder.ContainsKey((int) slot))
                lastFinishOrder = MainOrderManager.Instance.StorageTaskGroup.LastFinishOrder[(int) slot];

            List<AvailableItem> availableItems = new List<AvailableItem>();
            foreach (var kv in OrderConfigManager.Instance._orderItems)
            {
                var itemCode = kv.Key;
                var itemConfig = kv.Value;

                if (itemConfig.progressUnlock == 1 && !MainOrderManager.Instance.IsUnlockMergeItem(itemCode))
                    continue;

                if(itemConfig.isRecycle)
                    continue;
                
                if (itemConfig.completeOrderId > 0)
                {
                    if(!MainOrderManager.Instance.IsCompleteOrder(itemConfig.completeOrderId))
                        continue;
                }
                
                int weight = CalculateDefaultWeight(itemCode, itemConfig, slot, lastFinishOrder);
                if (weight > 0)
                {
                    AvailableItem availableItem = new AvailableItem();
                    availableItem._orderItemId = itemCode;
                    availableItem._weight = weight;
                    availableItems.Add(availableItem);
                }
            }

            return availableItems;
        }
        
        
        public static int CalculateDefaultWeight(int itemCode, TableOrderItem itemConfig, SlotDefinition slot, StorageTaskItem lastFinishOrder)
        {
            if (lastFinishOrder != null && lastFinishOrder.ItemIds.Contains(itemCode))
            {
                //DebugUtil.Log("当前item在上一个完成的任务中出现过 不在生成");
                return 0;
            }

            if (itemConfig != null && ExperenceModel.Instance.GetLevel() < itemConfig.unlockLevel)
            {
                //DebugUtil.Log("当前item不满足解锁等级 " + itemConfig.unlockLevel);
                return 0;
            }

            if (itemConfig != null && itemConfig.forbiddenSlot != null && itemConfig.forbiddenSlot.Contains((int)slot))
            {
                //DebugUtil.Log("当前item不能在这个槽位生成");
                return 0;
            }

            if (itemConfig.generators != null && itemConfig.generators.Length > 0)
            {
                bool isUnlock = false;
                for (int i = 0; i < itemConfig.generators.Length; i++)
                {
                    if (MainOrderManager.Instance.IsUnlockMergeItem(itemConfig.generators[i]))
                    {
                        isUnlock = true;
                        break;
                    }
                }
                
                if(!isUnlock)
                    return 0;
            }
            
            if (ABTestManager.Instance.IsOpenOrderABTest() && itemConfig.discardBuilds != null && itemConfig.discardBuilds.Length > 0)
            {
                bool isHave = false;
                for (int i = 0; i < itemConfig.discardBuilds.Length; i++)
                {
                    if (MainOrderManager.Instance.IsUnlockMergeItem(itemConfig.discardBuilds[i]))
                    {
                        isHave = true;
                        break;
                    }
                }
                
                if(isHave)
                    return 0;
            }
            
            int weight = CreateType == CreateOrderType.Level ? itemConfig.levelWeight : itemConfig.diffWeight;
            return weight;
        }
 
        public static List<AvailableItem> FilterByDifficulty(List<AvailableItem> items, int minDiff, int maxDiff, int[] filterMergeLine, SlotDefinition slot, int[] firstFilterDiffMergeLine = null)
        {
            List<AvailableItem> filter = new List<AvailableItem>();
            foreach (var item in items)
            {
                TableMergeItem mergeItem = GameConfigManager.Instance.GetItemConfig(item._orderItemId);
                if(mergeItem == null)
                    continue;

                var orderConfig = OrderConfigManager.Instance.GetOrderItem(item._orderItemId);
                if(orderConfig == null)
                    continue;
                    
                if(minDiff >= 0 && orderConfig.difficulty < minDiff)
                    continue;
                
                if(maxDiff >= 0 && orderConfig.difficulty > maxDiff)
                    continue;

                if(filterMergeLine != null && filterMergeLine.Contains(mergeItem.in_line))
                    continue;
                
                if(firstFilterDiffMergeLine != null && firstFilterDiffMergeLine.Contains(mergeItem.in_line))
                    continue;
                
                AvailableItem availableItem = new AvailableItem();
                availableItem._orderItemId = item._orderItemId;
                availableItem._weight = item._weight;
                availableItem._recommendedNumber = item._recommendedNumber;
                filter.Add(availableItem);
            }

            if (slot != SlotDefinition.BranchSlot && filter.Count == 0)
                return FilterDefaultByDifficulty(minDiff, maxDiff, filterMergeLine, slot);
            
            return filter;
        }

        public static List<AvailableItem> FilterByLevel(List<AvailableItem> items, int minLevel, int maxLevel, int[] filterMergeLine)
        {
            List<AvailableItem> filter = new List<AvailableItem>();
            foreach (var item in items)
            {
                TableMergeItem mergeItem = GameConfigManager.Instance.GetItemConfig(item._orderItemId);
                if(mergeItem == null)
                    continue;
                
                if(minLevel >= 0 && mergeItem.level < minLevel)
                    continue;
                
                if(maxLevel >= 0 && mergeItem.level > maxLevel)
                    continue;

                if(filterMergeLine != null && filterMergeLine.Contains(mergeItem.in_line))
                    continue;
                
                AvailableItem availableItem = new AvailableItem();
                availableItem._orderItemId = item._orderItemId;
                availableItem._weight = item._weight;
                availableItem._recommendedNumber = item._recommendedNumber;
                filter.Add(availableItem);
            }

            return filter;
        }

        
        private static List<AvailableItem> FilterDefaultByDifficulty(int minDiff, int maxDiff, int[] filterMergeLine, SlotDefinition slot)
        {
            List<AvailableItem> filter = new List<AvailableItem>();
            var items = GetAvailableDefaultItems(slot);
            foreach (var item in items)
            {
                TableMergeItem mergeItem = GameConfigManager.Instance.GetItemConfig(item._orderItemId);
                if(mergeItem == null)
                    continue;

                var orderConfig = OrderConfigManager.Instance.GetOrderItem(item._orderItemId);
                if(orderConfig == null)
                    continue;
                    
                if(minDiff >= 0 && orderConfig.difficulty < minDiff)
                    continue;
                
                if(maxDiff >= 0 && orderConfig.difficulty > maxDiff)
                    continue;

                if(filterMergeLine != null && filterMergeLine.Contains(mergeItem.in_line))
                    continue;
                
                AvailableItem availableItem = new AvailableItem();
                availableItem._orderItemId = item._orderItemId;
                availableItem._weight = item._weight;
                availableItem._recommendedNumber = item._recommendedNumber;
                filter.Add(availableItem);
            }

            return filter;
        }
        public static List<AvailableItem> FilterByChainId(List<AvailableItem> items, int[] chainIdList)
        {
            if (chainIdList == null || chainIdList.Length == 0)
                return items;
            
            List<AvailableItem> filter = new List<AvailableItem>();
            foreach (var item in items)
            {
                TableMergeItem mergeItem = GameConfigManager.Instance.GetItemConfig(item._orderItemId);
                if (mergeItem == null)
                    continue;
                
                if(!chainIdList.Contains(mergeItem.in_line))
                    continue;
                
                AvailableItem availableItem = new AvailableItem();
                availableItem._orderItemId = item._orderItemId;
                availableItem._weight = item._weight;
                availableItem._recommendedNumber = item._recommendedNumber;
                filter.Add(availableItem);
            }

            return filter;
        }

        public static bool IsRandomOrder(StorageTaskItem item)
        {
            if (MainOrderManager.Instance.IsSpecialTask(item.Id)
             || item.Slot == (int)SlotDefinition.BranchSlot || item.Slot == (int)SlotDefinition.Append
             || item.Slot == (int)SlotDefinition.Time || item.Slot == (int)SlotDefinition.Limit 
             || item.Slot == (int)SlotDefinition.Craze|| item.Slot == (int)SlotDefinition.KeepPet)
                return false;

            return true;
        }
        
        public class RandomCreator
        {
            public Func<SlotDefinition, bool> _canCreate;
            public Func<SlotDefinition, StorageTaskItem> _createOrder;
            
            public RandomCreator(Func<SlotDefinition, bool> canCreate, Func<SlotDefinition, StorageTaskItem> createOrder)
            {
                _canCreate = canCreate;
                _createOrder = createOrder;
            }
        }

        public static RandomCreator[] RandomCreators = new[]
        {
            new RandomCreator(MainOrderCreatorRandom1.CanCreate, MainOrderCreatorRandom1.CreateOrder),
            new RandomCreator(MainOrderCreatorRandom2.CanCreate, MainOrderCreatorRandom2.CreateOrder),
            new RandomCreator(MainOrderCreatorRandom3.CanCreate, MainOrderCreatorRandom3.CreateOrder),
            new RandomCreator(MainOrderCreatorRandom4.CanCreate, MainOrderCreatorRandom4.CreateOrder),
            new RandomCreator(MainOrderCreatorRandom5.CanCreate, MainOrderCreatorRandom5.CreateOrder),
            new RandomCreator(MainOrderCreatorBranch.CanCreate, MainOrderCreatorBranch.CreateOrder),
            new RandomCreator(MainOrderCreatorRandom6.CanCreate, MainOrderCreatorRandom6.CreateOrder),
        };

        public static StorageTaskItem CreateRandomOrders(SlotDefinition slot)
        {
            foreach (var randomCreator in RandomCreators)
            {
                if (randomCreator._canCreate(slot))
                    return randomCreator._createOrder(slot);
            }

            return null;
        }
        
        public static void RestRefreshTime(string key)
        {
            if(!MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.ContainsKey(key))
                return;
            
            MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[key] = 1;
        }

        public static bool HavenEnoughMerge(int mergeId, int num)
        {
            Dictionary<int, int> mergeItemCounts = MergeManager.Instance.GetMergeItemCounts(MergeBoardEnum.Main);
            Dictionary<int, int> bagItemCounts = MergeManager.Instance.GetBagItemCounts(MergeBoardEnum.Main);
            Dictionary<int, int> vipBagItemCounts = MergeManager.Instance.GetVipBagItemCounts(MergeBoardEnum.Main);
            
            var count = 0;
            if (mergeItemCounts.TryGetValue(mergeId, out var count1))
                count += count1;
            
            if (bagItemCounts.TryGetValue(mergeId,out var count2))
                count += count2;
            
            if (vipBagItemCounts.TryGetValue(mergeId,out var count3))
                count += count3;

            return count >= num;
        }
    }
   
}