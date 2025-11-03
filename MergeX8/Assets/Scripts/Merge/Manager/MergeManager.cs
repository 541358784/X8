using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Activity.BalloonRacing;
using Activity.Matreshkas.Model;
using Activity.RabbitRacing.Dynamic;
using Dlugin;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Network.API;
using Framework;
using Merge.Order;
using Newtonsoft.Json;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
public enum MergeBoardEnum
{
    None=-1,
    Main=0,
    HappyGo=1,
    SummerWatermelon=2,
    SummerWatermelonBread=3,
    Stimulate=4,
    ButterflyWorkShop=5,
    Filthy=6,
    Ditch=7,
    BiuBiu=8,
    TrainOrder=9,
}
public partial class MergeManager : Manager<MergeManager>
{
    public const int BOARD_WIDTH = 7; 
    public const int BOARD_HEIGHT = 9;
    public const int BAG_CAPACITY = 5;
    public const int BubbleCD = 60;

    public int MergeBoardID1
    {
        get { return mergeBoadrId1; }
    }
    static ClassObjectPool mergeItemPool = new ClassObjectPool(10, () => new StorageMergeItem());

    protected StorageDictionary<int, StorageMergeBoard> storageMergeBoardDict;
    protected int mergeBoadrId1 = 0;
    Dictionary<int, int> mergeItemCounts = new Dictionary<int, int>();
    public bool mergeItemCountsDirty = true;
    private MergeBoardEnum _lastGetItemBoard = MergeBoardEnum.None;

    public Dictionary<int, int> completeNeedData = new Dictionary<int, int>();

    public StorageMergeBoard storageBoard
    {
        get { return storageMergeBoardDict.GetValue(mergeBoadrId1); }
    }
    public StorageMergeBoard GetStorageBoard(MergeBoardEnum boardId)
    {
        return storageMergeBoardDict.GetValue((int)boardId);
    }

    private StorageMergeUnlockItem unlockItems
    {
        get { return StorageManager.Instance.GetStorage<StorageGame>().MergeUnlockItem; }
    }

    public StorageMergeUnlockItem UnlockItems => unlockItems;

    public Dictionary<int, int> GetMergeItemCounts(MergeBoardEnum boardId)
    {
        if (mergeItemCountsDirty|| _lastGetItemBoard != boardId)
        {
            _lastGetItemBoard = boardId;
            mergeItemCountsDirty = false;
            mergeItemCounts.Clear();
            for (int i = 0; i < GetStorageBoard(boardId).Items.Count; i++)
            {
                var mergeItem = GetStorageBoard(boardId).Items[i];
                if (!IsUnlock(mergeItem))
                    continue;

                CommonUtils.AddValue(mergeItemCounts, mergeItem.Id, 1);
            }
        }

        return mergeItemCounts;
    }
    public Dictionary<int,int> GetBagItemCounts(MergeBoardEnum boardId)
    {
        var bagItemCountDictionary = new Dictionary<int, int>();
        for (int i = 0; i < GetStorageBoard(boardId).Bags.Count; i++)
        {
            var mergeItem = GetStorageBoard(boardId).Bags[i];
            CommonUtils.AddValue(bagItemCountDictionary, mergeItem.Id, 1);
        }
        return bagItemCountDictionary;
    }
    public Dictionary<int,int> GetVipBagItemCounts(MergeBoardEnum boardId)
    {
        var bagItemCountDictionary = new Dictionary<int, int>();
        for (int i = 0; i < GetStorageBoard(boardId).VipBags.Count; i++)
        {
            var mergeItem = GetStorageBoard(boardId).VipBags[i];
            CommonUtils.AddValue(bagItemCountDictionary, mergeItem.Id, 1);
        }
        return bagItemCountDictionary;
    }

    public bool IsCompleted(Dictionary<int, int> needCounts,MergeBoardEnum boardId)
    {
        var curItemCounts = GetMergeItemCounts(boardId);
        foreach (int id in needCounts.Keys)
        {
            if (CommonUtils.GetValue(curItemCounts, id) < needCounts[id])
            {
                return false;
            }
        }

        return true;
    }

    public bool IsEnoughCount(Dictionary<int, int> needCounts,MergeBoardEnum boardId)
    {
        bool result = true;
        var curItemCounts = GetMergeItemCounts(boardId);
        foreach (int id in needCounts.Keys)
        {
            int needCount = needCounts[id];
            if (CommonUtils.GetValue(curItemCounts, id) < needCount)
            {
                result = false;
                break;
            }
        }

        return result;
    }

    public List<int> GetTaskCompleteBagItemIndex(Dictionary<int, int> needData, MergeBoardEnum boardType)
    {
        if (!storageMergeBoardDict.ContainsKey((int) boardType))
            return null;
        if (needData == null || needData.Count == 0)
            return null;
        List<int> itemIndex = new List<int>();
        if (completeNeedData != needData)
        {
            completeNeedData.Clear();
            foreach (var kv in needData)
            {
                completeNeedData.Add(kv.Key, kv.Value);
            }
        }
        for (int i = 0; i < GetStorageBoard(boardType).Bags.Count; i++)
        {
            var mergeItem = GetStorageBoard(boardType).Bags[i];
            int needCount = CommonUtils.GetValue(completeNeedData, mergeItem.Id);
            if (needCount > 0)
            {
                completeNeedData[mergeItem.Id] = needCount - 1;
                itemIndex.Add(i);

                if (completeNeedData[mergeItem.Id] <= 0)
                    completeNeedData.Remove(mergeItem.Id);
            }

            if (completeNeedData.Count == 0)
                break;
        }
        return itemIndex;
    }
    public List<int> GetTaskCompleteVipBagItemIndex(Dictionary<int, int> needData, MergeBoardEnum boardType)
    {
        if (!storageMergeBoardDict.ContainsKey((int) boardType))
            return null;
        if (needData == null || needData.Count == 0)
            return null;
        List<int> itemIndex = new List<int>();
        if (completeNeedData != needData)
        {
            completeNeedData.Clear();
            foreach (var kv in needData)
            {
                completeNeedData.Add(kv.Key, kv.Value);
            }
        }
        for (int i = 0; i < GetStorageBoard(boardType).VipBags.Count; i++)
        {
            var mergeItem = GetStorageBoard(boardType).VipBags[i];
            int needCount = CommonUtils.GetValue(completeNeedData, mergeItem.Id);
            if (needCount > 0)
            {
                completeNeedData[mergeItem.Id] = needCount - 1;
                itemIndex.Add(i);

                if (completeNeedData[mergeItem.Id] <= 0)
                    completeNeedData.Remove(mergeItem.Id);
            }

            if (completeNeedData.Count == 0)
                break;
        }
        return itemIndex;
    }
    public List<int> GetTaskCompleteItemIndex(Dictionary<int, int> needData,MergeBoardEnum boardType)
    {
        if (!storageMergeBoardDict.ContainsKey((int) boardType))
            return null;
        if (needData == null || needData.Count == 0)
            return null;

        List<int> itemIndex = new List<int>();
        completeNeedData.Clear();
        foreach (var kv in needData)
        {
            completeNeedData.Add(kv.Key, kv.Value);
        }

        // 棋盘
        for (int i = 0; i < storageMergeBoardDict[(int)boardType].Items.Count; i++)
        {
            StorageMergeItem storageMergeItem =  storageMergeBoardDict[(int)boardType].Items[i];
            if (!IsUnlock(storageMergeItem))
                continue;
            int needCount = CommonUtils.GetValue(completeNeedData, storageMergeItem.Id);
            if (needCount > 0)
            {
                completeNeedData[storageMergeItem.Id] = needCount - 1;
                itemIndex.Add(i);

                if (completeNeedData[storageMergeItem.Id] <= 0)
                    completeNeedData.Remove(storageMergeItem.Id);
            }

            if (completeNeedData.Count == 0)
                break;
        }

        return itemIndex;
    }

    public bool Consume(Dictionary<int, int> needCounts,MergeBoardEnum boardId)
    {
        if (!storageMergeBoardDict.ContainsKey((int) boardId))
            return false;
        if (needCounts == null || needCounts.Count == 0)
            return false;

        var curItemCounts = GetMergeItemCounts(boardId);
        var bagItemCount = GetBagItemCounts(boardId);
        var vipBagItemCount = GetVipBagItemCounts(boardId);
        foreach (int id in needCounts.Keys)
        {
            int needCount = needCounts[id];
            var hasCount = CommonUtils.GetValue(curItemCounts, id) +
                           CommonUtils.GetValue(bagItemCount, id) +
                           CommonUtils.GetValue(vipBagItemCount, id);
            if (hasCount < needCount)
                return false;
        }

        // 使用棋盘
        for (int i = 0; i <  storageMergeBoardDict[(int)boardId].Items.Count; i++)
        {
            StorageMergeItem storageMergeItem =  storageMergeBoardDict[(int)boardId].Items[i];
            if (!IsUnlock(storageMergeItem))
                continue;

            int needCount = CommonUtils.GetValue(needCounts, storageMergeItem.Id);
            if (needCount > 0)
            {
                needCounts[storageMergeItem.Id] = needCount - 1;
                if (needCounts[storageMergeItem.Id] <= 0)
                    needCounts.Remove(storageMergeItem.Id);
                SetBoardItem(i, -1, RefreshItemSource.remove,boardId);
            }

            if (needCounts.Count <= 0)
                break;
        }

        if (needCounts.Count > 0)
        {
            for (int i = 0; i < GetStorageBoard(boardId).Bags.Count; i++)
            {
                var mergeItem = GetStorageBoard(boardId).Bags[i];
                int needCount = CommonUtils.GetValue(needCounts, mergeItem.Id);
                if (needCount > 0)
                {
                    needCounts[mergeItem.Id] = needCount - 1;
                    if (needCounts[mergeItem.Id] <= 0)
                        needCounts.Remove(mergeItem.Id);
                    RemoveBagItem(i,boardId);
                    i--;
                }
                if (needCounts.Count <= 0)
                    break;
            }
        }
        if (needCounts.Count > 0)
        {
            for (int i = 0; i < GetStorageBoard(boardId).BuildingBags.Count; i++)
            {
                var mergeItem = GetStorageBoard(boardId).BuildingBags[i];
                int needCount = CommonUtils.GetValue(needCounts, mergeItem.Id);
                if (needCount > 0)
                {
                    needCounts[mergeItem.Id] = needCount - 1;
                    if (needCounts[mergeItem.Id] <= 0)
                        needCounts.Remove(mergeItem.Id);
                    RemoveBuildingBagItem(i,boardId);
                    i--;
                }
                if (needCounts.Count <= 0)
                    break;
            }
        }
        return true;
    }

    protected override void InitImmediately()
    {
        Refresh(MergeBoardEnum.Main);
    }

    public void Refresh(MergeBoardEnum boardId)
    {
        InitStorage();
        mergeBoadrId1 = (int) boardId;
        if(boardId==MergeBoardEnum.Main)
        {
            InitMergeBoard(boardId);
        }
        else if (boardId==MergeBoardEnum.SummerWatermelon)
        {
            InitSummerWatermelonMergeBoard(boardId);
        }
        else if (boardId==MergeBoardEnum.SummerWatermelonBread)
        {
            InitSummerWatermelonBreadMergeBoard(boardId);
        }
        else if (boardId ==MergeBoardEnum.HappyGo)
        {
            InitHGMergeBoard(boardId);
        }
        else if (boardId == MergeBoardEnum.Stimulate)
        {
            InitStimulateMergeBoard(boardId);
        }
        else if (boardId == MergeBoardEnum.Filthy)
        {
            InitFilthyMergeBoard(boardId);
        }
        else if (boardId == MergeBoardEnum.ButterflyWorkShop)
        {
            InitButterflyWorkShopMergeBoard(boardId);
        }
        else if (boardId == MergeBoardEnum.Ditch)
        {
            InitDitchMergeBoard(boardId);
        }
        else if (boardId==MergeBoardEnum.BiuBiu)
        {
            InitBiuBiuMergeBoard(boardId);
        }
        else if (boardId == MergeBoardEnum.TrainOrder)
        {
            InitTrainOrderMergeBoard(boardId);
        }
  
        ReMapBoardOldId(boardId);
        ReMapBagOldId(boardId);
        ReMapTempStorageBagOldId(boardId);
        RemoveDeleteStoreageId();
        InitRecordGetItems(boardId);
    }

    protected void InitStorage()
    {
        storageMergeBoardDict = StorageManager.Instance.GetStorage<StorageGame>().MergeBoards;
        
        if (storageMergeBoardDict.ContainsKey((int)MergeBoardEnum.HappyGo))
            storageMergeBoardDict.Remove((int)MergeBoardEnum.HappyGo);
    }

    public virtual void InitMergeBoard(MergeBoardEnum boardId)
    {
        var mergeBoardId = (int)boardId;
        if (!storageMergeBoardDict.ContainsKey(mergeBoardId) || GetStorageBoard(boardId).Items.Count == 0)
        {
            DebugUtil.LogError("初始化棋盘");
            StorageMergeBoard storageMergeBoard = new StorageMergeBoard();
            storageMergeBoard.Width = BOARD_WIDTH;
            storageMergeBoard.Height = BOARD_HEIGHT;
            storageMergeBoard.BagCapacity = GameConfigManager.Instance.BagList.FindAll(x => x.CointCost <= 0).Count;
            for (int i = 0; i < storageMergeBoard.Width * storageMergeBoard.Height; i++)
            {
                StorageMergeItem storageMergeItem = new StorageMergeItem();
                storageMergeItem.Id = -1;
                if (i < GameConfigManager.Instance.BoardGrid.Count)
                {
                    var config = GameConfigManager.Instance.BoardGrid[i];
                    storageMergeItem.Id = config.itemId;
                    storageMergeItem.State = config.state;
#if DEBUG || DEVELOPMENT_BUILD
                    if (config.itemId != -1 && GameConfigManager.Instance.GetItemConfig(config.itemId) == null)
                    {
                        DebugUtil.LogError("Invalid MergeItemId : "+config.id+"   "+ config.itemId);
                    }
#endif
                    // SetOrginalStoreCount(storageMergeItem.Id, i);
                }

                storageMergeBoard.Items.Add(storageMergeItem);
            }

            storageMergeBoardDict[mergeBoardId] = storageMergeBoard;
            InitOrzginalStoreCount(storageMergeBoardDict[mergeBoardId],boardId);
        }
    }
    public virtual void InitSummerWatermelonMergeBoard(MergeBoardEnum boardId)
    {
        var mergeBoadrId = (int)boardId;
        if (!storageMergeBoardDict.ContainsKey(mergeBoadrId) || GetStorageBoard(boardId).Items.Count == 0)
        {
            DebugUtil.LogError("初始化夏日西瓜棋盘");
            StorageMergeBoard storageMergeBoard = new StorageMergeBoard();
            storageMergeBoard.Width = SummerWatermelonModel.BOARD_WIDTH;
            storageMergeBoard.Height = SummerWatermelonModel.BOARD_HEIGHT;
            storageMergeBoard.BagCapacity = 0;
            for (int i = 0; i < storageMergeBoard.Width * storageMergeBoard.Height; i++)
            {
                StorageMergeItem storageMergeItem = new StorageMergeItem();
                storageMergeItem.Id = -1;
                storageMergeItem.State = 1;
                storageMergeBoard.Items.Add(storageMergeItem);
            }

            storageMergeBoardDict[mergeBoadrId] = storageMergeBoard;
            InitOrzginalStoreCount(storageMergeBoardDict[mergeBoadrId],boardId);
        }
    }
    public virtual void InitSummerWatermelonBreadMergeBoard(MergeBoardEnum boardId)
    {
        var mergeBoadrId = (int)boardId;
        if (!storageMergeBoardDict.ContainsKey(mergeBoadrId) || GetStorageBoard(boardId).Items.Count == 0)
        {
            DebugUtil.LogError("初始化夏日西瓜棋盘");
            StorageMergeBoard storageMergeBoard = new StorageMergeBoard();
            storageMergeBoard.Width = SummerWatermelonBreadModel.BOARD_WIDTH;
            storageMergeBoard.Height = SummerWatermelonBreadModel.BOARD_HEIGHT;
            storageMergeBoard.BagCapacity = 0;
            for (int i = 0; i < storageMergeBoard.Width * storageMergeBoard.Height; i++)
            {
                StorageMergeItem storageMergeItem = new StorageMergeItem();
                storageMergeItem.Id = -1;
                storageMergeItem.State = 1;
                storageMergeBoard.Items.Add(storageMergeItem);
            }

            storageMergeBoardDict[mergeBoadrId] = storageMergeBoard;
            InitOrzginalStoreCount(storageMergeBoardDict[mergeBoadrId],boardId);
        }
    }
    public virtual void InitButterflyWorkShopMergeBoard(MergeBoardEnum boardId)
    {
        var mergeBoadrId = (int)boardId;
        if (!storageMergeBoardDict.ContainsKey(mergeBoadrId) || GetStorageBoard(boardId).Items.Count == 0)
        {
            DebugUtil.LogError("初始化蝴蝶棋盘");
            StorageMergeBoard storageMergeBoard = new StorageMergeBoard();
            storageMergeBoard.Width = ButterflyWorkShopModel.BOARD_WIDTH;
            storageMergeBoard.Height = ButterflyWorkShopModel.BOARD_HEIGHT;
            storageMergeBoard.BagCapacity = 0;
            for (int i = 0; i < storageMergeBoard.Width * storageMergeBoard.Height; i++)
            {
                StorageMergeItem storageMergeItem = new StorageMergeItem();
                storageMergeItem.Id = -1;
                storageMergeItem.State = 1;
                storageMergeBoard.Items.Add(storageMergeItem);
            }
            storageMergeBoardDict[mergeBoadrId] = storageMergeBoard;
            InitOrzginalStoreCount(storageMergeBoardDict[mergeBoadrId],boardId);
        }
    }
    public virtual void InitBiuBiuMergeBoard(MergeBoardEnum boardId)
    {
        var mergeBoadrId = (int)boardId;
        if (!storageMergeBoardDict.ContainsKey(mergeBoadrId) || GetStorageBoard(boardId).Items.Count == 0)
        {
            DebugUtil.LogError("初始化飞镖棋盘");
            StorageMergeBoard storageMergeBoard = new StorageMergeBoard();
            storageMergeBoard.Width = BiuBiuModel.BOARD_WIDTH;
            storageMergeBoard.Height = BiuBiuModel.BOARD_HEIGHT;
            storageMergeBoard.BagCapacity = 0;
            for (int i = 0; i < storageMergeBoard.Width * storageMergeBoard.Height; i++)
            {
                StorageMergeItem storageMergeItem = new StorageMergeItem();
                storageMergeItem.Id = -1;
                storageMergeItem.State = 1;
                storageMergeBoard.Items.Add(storageMergeItem);
            }
            storageMergeBoardDict[mergeBoadrId] = storageMergeBoard;
            InitOrzginalStoreCount(storageMergeBoardDict[mergeBoadrId],boardId);
        }
    }
    
    public  void InitTrainOrderMergeBoard(MergeBoardEnum boardId)
    {
        var mergeBoardId = (int)boardId;
        if (!storageMergeBoardDict.ContainsKey(mergeBoardId) || GetStorageBoard(boardId).Items.Count == 0)
        {
            DebugUtil.LogError("初始化火车订单棋盘");
            StorageMergeBoard storageMergeBoard = new StorageMergeBoard();
            storageMergeBoard.Width = Activity.TrainOrder.TrainOrderModel.BOARD_WIDTH;
            storageMergeBoard.Height = Activity.TrainOrder.TrainOrderModel.BOARD_HEIGHT;
            storageMergeBoard.BagCapacity = 0;
            for (int i = 0; i < storageMergeBoard.Width * storageMergeBoard.Height; i++)
            {
                StorageMergeItem storageMergeItem = new StorageMergeItem();
                storageMergeItem.Id = -1;
                storageMergeItem.State = 1;
                storageMergeBoard.Items.Add(storageMergeItem);
            }

            storageMergeBoardDict[mergeBoardId] = storageMergeBoard;
            InitOrzginalStoreCount(storageMergeBoardDict[mergeBoardId], boardId);
        }
    }
    
    /// <summary>
    /// 用于初始化棋牌初始建筑产量
    /// </summary>
    /// <param name="board"></param>
    public void InitOrzginalStoreCount(StorageMergeBoard board,MergeBoardEnum boardId)
    {
        if (board == null || board.Items == null || board.Items.Count < 0)
            return;
        for (int i = 0; i < board.Items.Count; i++)
        {
            SetOrginalStoreCount(board.Items[i].Id, i,boardId);
        }
    }
    public int GetBoardWidth(MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).Width;
    }

    public int GetBoardHeight(MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).Height;
    }

    public int GetBoardCount(MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).Width * GetStorageBoard(boardId).Height;
    }

    public bool IsOpen(StorageMergeItem storageMergeItem)
    {
        if (storageMergeItem == null)
            return false;
        return storageMergeItem.State == 1 || storageMergeItem.State == 3;
    }

    public bool IsOpen(int index,MergeBoardEnum boardId)
    {
        var itemStorage = GetBoardItem(index,boardId);
        if (itemStorage == null)
            return false;
        return itemStorage.State == 1 || itemStorage.State == 3;
    }

    public bool IsBox(StorageMergeItem storageMergeItem)
    {
        return storageMergeItem.State == -1;
    }

    public bool IsActiveItem(int index,MergeBoardEnum boardId)
    {
        var itemStorage = GetBoardItem(index,boardId);
        return itemStorage.State == 3;
    }

    public bool IsActiveItem(StorageMergeItem itemStorage)
    {
        return itemStorage.State == 3;
    }

    public bool IsBox(int index,MergeBoardEnum boardId)
    {
        var itemStorage = GetBoardItem(index,boardId);
        return itemStorage.State == -1;
    }

    //box || 固定产出
    public bool IsBox(TableMergeItem config)
    {
        if (config == null)
            return false;

        return config.type == (int) MergeItemType.box || config.type == (int)MergeItemType.flashsaleBox || config.type == (int)MergeItemType.dailyBox;
    }
    public bool IsChoiceBox(TableMergeItem config)
    {
        if (config == null)
            return false;

        return config.type == (int) MergeItemType.choiceChest;
    }

    public MergeItemStatus GetMergeItemStatus(StorageMergeItem storageMergeItem)
    {
        return (MergeItemStatus) storageMergeItem.State;
    }

    public bool IsUnlock(StorageMergeItem storageMergeItem)
    {
        return storageMergeItem.State == 1 || storageMergeItem.State == 3;
    }

    public bool IsLockWeb(int index,MergeBoardEnum boardId) //是否是蛛网
    {
        var itemStorage = GetBoardItem(index,boardId);
        return itemStorage.State == 0;
    }

    public StorageMergeItem GetEmptyItem()
    {
        var item = (StorageMergeItem) mergeItemPool.Pop();
        item.Id = -1;
        item.Clear();
        ResetStorageMergeBoardStatus(item);
        return item;
    }

    // public StorageMergeItem GetBoardItem(int index)
    // {
    //     if (GetStorageBoard(boardId) == null)
    //         return null;
    //     if (index < 0 || index >= GetStorageBoard(boardId).Items.Count)
    //         return null;
    //     return GetStorageBoard(boardId).Items[index];
    // }

    public StorageMergeItem GetBoardItem(int index, MergeBoardEnum boardId)
    {
        if (GetStorageBoard(boardId) == null)
            return null;
        if (index < 0 || index >= GetStorageBoard(boardId).Items.Count)
            return null;
        return GetStorageBoard(boardId).Items[index];
    }

    public void SetBoardItem(int index, StorageMergeItem storageMergeItem, RefreshItemSource source,MergeBoardEnum boardId,
        bool isDispatchEvent = true)
    {
        StorageMergeItem temp = GetStorageBoard(boardId).Items[index];
        if (temp != storageMergeItem)
        {
            mergeItemPool.Push(temp);
            GetStorageBoard(boardId).Items[index] = storageMergeItem;
        }

        mergeItemCountsDirty = true;
        if (isDispatchEvent)
            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_REFRESH, (int)boardId,index, -1, source, storageMergeItem.Id);
        AddShowedItemId(storageMergeItem.Id);
        RecordGetItem(storageMergeItem);
        SetOrginalStoreCount(GetStorageBoard(boardId).Items[index], source,boardId);
    }

    public void SetBoardItem(int index, int id, RefreshItemSource source,MergeBoardEnum boardId, int oldIndex = -1,
        bool isDispatchEvent = true, int oldId = -1)
    {
        if (boardId != MergeBoardEnum.HappyGo)
        {
            EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_TASK_REFRESH, TaskType.MergerItem, id, 1);
            EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_2_TASK_REFRESH, TaskType.MergerItem, id, 1);
        }
       
        GetStorageBoard(boardId).Items[index].Id = id;
        mergeItemCountsDirty = true;

        if (source == RefreshItemSource.mergeOk || source == RefreshItemSource.mergeOk_omnipoten)
        {
            ResetStorageMergeBoardStatus(GetStorageBoard(boardId).Items[index]);
            if (boardId == MergeBoardEnum.Main)
            {
                BalloonRacingModel.Instance.AddMergeScore(id, MergeMainController.Instance.MergeBoard.GetGridByIndex(index).board.transform.position, "2");
                RabbitRacingModel.Instance.AddMergeScore(id, MergeMainController.Instance.MergeBoard.GetGridByIndex(index).board.transform.position, "2");
            }
        }

        var config = GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Items[index].Id);
        if (MergeConfigManager.Instance.IsTimeProductItem(GetStorageBoard(boardId).Items[index].Id) &&
            GetStorageBoard(boardId).Items[index].TimProductTime <= 0 && config.time_original_count<=0)
            GetStorageBoard(boardId).Items[index].TimProductTime = APIManager.Instance.GetServerTime() / 1000;
        SetOrginalStoreCount(GetStorageBoard(boardId).Items[index], source,boardId);
        if (isDispatchEvent)
            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_REFRESH, (int)boardId,index, oldIndex, source, id, oldId);
        AddShowedItemId(id);
        RecordGetItem(GetStorageBoard(boardId).Items[index]);
    }

    public void SetNewBoardItem(int index, int id, int state, RefreshItemSource source,MergeBoardEnum boardId,
        int oldIndex = -1, bool isDispatchEvent = true, int oldId = -1) // 新增 新的merge物品
    {
        int type = (int) MergeConfigManager.Instance.GetActiveCostType(id);
        if (type >= 1 && type != 3)
            state = 3;
        if (type == 3)
            GetStorageBoard(boardId).Items[index].ActiveTime = APIManager.Instance.GetServerTime() / 1000;
        else if (type == 4)
            GetStorageBoard(boardId).Items[index].ActiveTime = 0;
        GetStorageBoard(boardId).Items[index].State = state;
        GetStorageBoard(boardId).Items[index].Id = id;
        ResetStorageMergeBoardStatus(GetStorageBoard(boardId).Items[index]);
        SetBoardItem(index, id, source,boardId, oldIndex, isDispatchEvent, oldId);
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_NEW_ITEM, (int)boardId,index, oldIndex, source, id, oldId);
        MergeFtueBiManager.Instance.SendFtueBi(MergeFtueBiManager.SendType.Product, id);
    }

    public void SetProductCount(int index, int count,MergeBoardEnum boardId)
    {
        if (index == -1)
            return;
        var boardItem = GetBoardItem(index,boardId);
        boardItem.ProductCount = count;
    }

    public void SetBoardItem(int index, int id, int state, RefreshItemSource source,MergeBoardEnum boardId)
    {
        GetStorageBoard(boardId).Items[index].State = state;
        if (source == RefreshItemSource.mergeOk || source == RefreshItemSource.mergeOk_omnipoten)
        {
            ResetStorageMergeBoardStatus(GetStorageBoard(boardId).Items[index]);
        }

        // if (MergeConfigManager.Instance.IsTimeProductItem(id) && GetStorageBoard(boardId).Items[index].ProductTime <= 0)
        //     GetStorageBoard(boardId).Items[index].ProductTime = APIManager.Instance.GetServerTime() / 1000;
        SetBoardItem(index, id, source,boardId);
        SetOrginalStoreCount(GetStorageBoard(boardId).Items[index], source,boardId);
    }

    #region 产出

    public void RecordProductInfo(int index, int productId,MergeBoardEnum boardId, RefreshItemSource source = RefreshItemSource.notDeal)
    {
        if (source == RefreshItemSource.timeProduct)
        {
            int count = GetStorageBoard(boardId).Items[index].TimeProductCount;
            count += 1;
            GetStorageBoard(boardId).Items[index].TimProductTime = APIManager.Instance.GetServerTime() / 1000;
            GetStorageBoard(boardId).Items[index].TimeProductCount = count;
            GetStorageBoard(boardId).Items[index].TimeStoreMax -= 1;
            DebugUtil.Log("产出物产出次数+1:" + GetStorageBoard(boardId).Items[index].Id + "-----产出次数：" + count +
                          "--TimeStoreMax:" + GetStorageBoard(boardId).Items[index].TimeStoreMax);
            RecordMergeLineProductInfo(index,boardId);
        }
        else
        {
            int count = GetStorageBoard(boardId).Items[index].ProductCount;
            bool isEnergyProduct= MergeConfigManager.Instance.IsEnergyProductItem(GetStorageBoard(boardId).Items[index].Id);
            bool isDeath = MergeConfigManager.Instance.IsLimitNoCdProductItem(GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Items[index].Id));
            if (!IsInUnlimitedProduct(boardId) || !isEnergyProduct ||isDeath)
            {
                count += 1;
                GetStorageBoard(boardId).Items[index].ProductTime = APIManager.Instance.GetServerTime() / 1000;
                GetStorageBoard(boardId).Items[index].ProductCount = count;
                //if(boardId !=MergeBoardEnum.TrainOrder)
                GetStorageBoard(boardId).Items[index].StoreMax -= 1;
                
                DebugUtil.Log("产出物产出次数+1:" + GetStorageBoard(boardId).Items[index].Id + "-----产出次数：" + count + "--storeMax:" +
                              GetStorageBoard(boardId).Items[index].StoreMax);
            }

            if(GetStorageBoard(boardId).Items[index].StoreMax ==0)
                DebugUtil.Log("StoreMax ==0  ProductCount"+TimeUtils.GetDateTime((long)GetStorageBoard(boardId).Items[index].ProductTime).ToString());
            RecordMergeLineProductInfo(index,boardId);
            // if (source != RefreshItemSource.exp)
            //     RecordDropInterval(index, productId,boardId);
        }
    }

    public  void RecordDropInterval(int index, int productId,MergeBoardEnum boardId) // 保底次数记录
    {
        var storageItem = GetBoardItem(index,boardId);
        if (storageItem == null)
            return;
        var itemConfig = GameConfigManager.Instance.GetItemConfig(storageItem.Id);
        if (itemConfig == null)
            return;
        if (itemConfig.output == null)
            return;
        if (!itemConfig.output.Contains(productId)) // 可能会产出output意外的物品 --- （按照合成产出来配置） 
            return;
        for (int i = 0; i < itemConfig.output.Length; i++)
        {
            int maxDrop = itemConfig.Max_Drop_Interval != null &&
                          itemConfig.Max_Drop_Interval.Length > 0 &&
                          itemConfig.Max_Drop_Interval.Length > i
                ? itemConfig.Max_Drop_Interval[i]
                : 0;
            if (itemConfig.output[i] != productId && maxDrop > 0)
            {
                int intervalCount = 0;
                storageItem.DropIntervalDic.TryGetValue(itemConfig.output[i], out intervalCount);
                storageItem.DropIntervalDic[itemConfig.output[i]] = intervalCount + 1;
            }
        }

        storageItem.DropIntervalDic[productId] = 0;
    }

    public void RecordMergeLineProductInfo(int index,MergeBoardEnum boardId) // 记录合成连产出次数
    {
        var storeItem = GetBoardItem(index,boardId);
        if (storeItem == null)
            return;
        int line = MergeConfigManager.Instance.GetMergeLineById(storeItem.Id);
        int productCount = 0;
        if (line == 0)
            return;
        GetStorageBoard(boardId).LineProducts.TryGetValue(line, out productCount);
        GetStorageBoard(boardId).LineProducts[line] = productCount + 1;
    }

    public int GetMergeLineProductCount(int index,MergeBoardEnum boardId)
    {
        var storeItem = GetBoardItem(index,boardId);
        if (storeItem == null)
            return 0;
        int line = MergeConfigManager.Instance.GetMergeLineById(storeItem.Id);
        int productCount = 0;
        GetStorageBoard(boardId).LineProducts.TryGetValue(line, out productCount);
        return productCount;
    }

    public int GetMergeLineProductItemId(int index,MergeBoardEnum boardId)
    {
        var storeItem = GetBoardItem(index,boardId);
        if (storeItem == null)
            return 0;
        int prodcutCount = GetMergeLineProductCount(index,boardId);
        int outputId = MergeConfigManager.Instance.GetMergeLineProductItemId(storeItem.Id, prodcutCount);
        return outputId;
    }

    public int GetDropIntervalOutputId(int index,MergeBoardEnum boardId) //获取保底次数id 
    {
        int outputId = 0;
        var storageItem = GetBoardItem(index,boardId);
        if (storageItem == null)
            return outputId;
        var level=ExperenceModel.Instance.GetLevel();
        foreach (var item in storageItem.DropIntervalDic)
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(storageItem.Id);
            if (itemConfig != null)
            {
                int i = itemConfig.output.IndexOfEx(item.Key);
                //有产出等级限制不产出保底
                if (itemConfig.outputLimit != null && itemConfig.outputLimit.Length > 0 && itemConfig.outputLimit.Length==itemConfig.output.Length)
                {
                    if(level< itemConfig.outputLimit[i])
                        continue;
                }
                int maxDrop =
                    i >= 0 &&
                    itemConfig.Max_Drop_Interval != null &&
                    itemConfig.Max_Drop_Interval.Length > 0 &&
                    itemConfig.Max_Drop_Interval.Length > i
                        ? itemConfig.Max_Drop_Interval[i]
                        : 0;
                if (maxDrop > 0)
                {
                    if (item.Value >= maxDrop ) // 已经触发保底次数
                    {
                        ///对回收物品加一个处理，回收后不再产出保底
                        if (GameConfigManager.Instance.RecoveryList != null &&
                            GameConfigManager.Instance.RecoveryList.Count > 0)
                        {
                            foreach (var recovery in GameConfigManager.Instance.RecoveryList)
                            {
                                if (recovery.id == item.Key)
                                {
                                    storageItem.DropIntervalDic[item.Key] = 0;
                                    return 0;
                                }
                            }
                        }
                        outputId = item.Key;
                        storageItem.DropIntervalDic[item.Key] = 0;
                        break;
                    }
                }
            }
        }

        return outputId;
    }

    public void RecordTimeProductItem(int index, int id, TableMergeItem itemConfig,MergeBoardEnum boardId) // 记录时间产出建筑的产出物品
    {
        if (!IsTimeProductMaxCount(index, itemConfig,boardId))
        {
            GetStorageBoard(boardId).Items[index].ProductItems.Add(id);
            RecordProductInfo(index, -1, boardId,RefreshItemSource.timeProduct);
        }
    }

    public List<int> GetTimeProductItems(int index,MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).Items[index].ProductItems;
    }

    public void FreeTimeProductItems(int index, int id,MergeBoardEnum boardId)
    {
        GetStorageBoard(boardId).Items[index].ProductItems.Remove(id);
    }

    public bool IsTimeProductMaxCount(int index, TableMergeItem itemConfig,MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).Items[index].ProductItems.Count >= GetTimeMaxOutputAmount(itemConfig);
    }

    public int GetTimeProductTimes(int index,MergeBoardEnum boardId)
    {
        return 3 - GetStorageBoard(boardId).Items[index].ProductItems.Count;
    }


    public bool IsTimeProductInCD(int index,MergeBoardEnum boardId) //时间产出建筑是否处于cd内
    {
        if (GetStorageBoard(boardId).Items[index].Id == -1)
            return false;
        bool result = true;
        var config = GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Items[index].Id);
        if (config == null)
            throw new System.Exception("配置表中没找到id:" + GetStorageBoard(boardId).Items[index].Id);
        var offset = (int) (APIManager.Instance.GetServerTime() / 1000 - GetStorageBoard(boardId).Items[index].TimProductTime);
        result = offset < GetTimeProductCdTime(config);
        if (GetStorageBoard(boardId).Items[index].TimeProductCount <= 0 && config.time_original_count>0)
        {
            GetStorageBoard(boardId).Items[index].TimeStoreMax =
                GetStorageBoard(boardId).Items[index].TimeStoreMax > 0
                    ? GetStorageBoard(boardId).Items[index].TimeStoreMax
                    :  config.time_original_count; 
        }
        else
        {
            if (GetStorageBoard(boardId).Items[index].TimProductTime <= 0)
            {
                GetStorageBoard(boardId).Items[index].TimeStoreMax =
                    GetStorageBoard(boardId).Items[index].TimeStoreMax > 0
                        ? GetStorageBoard(boardId).Items[index].TimeStoreMax
                        : GetTimeOutputAmount(config); 
            }
            else
            {
                float cdTime = GetTimeProductCdTime(config);
                if (cdTime != 0)
                {
                    int recoverCount = Mathf.FloorToInt(offset / cdTime);
                    if (recoverCount > 0)
                    {
                        GetStorageBoard(boardId).Items[index].TimeStoreMax += recoverCount * GetTimeOutputAmount(config);
                        if (GetStorageBoard(boardId).Items[index].TimeStoreMax > GetTimeMaxOutputAmount(config))
                            GetStorageBoard(boardId).Items[index].TimeStoreMax = GetTimeMaxOutputAmount(config);
                        if (config.onelife > 0)
                        {
                            int leftCout = GetTimeOutputAmount(config) * config.onelife -
                                           GetStorageBoard(boardId).Items[index].TimeProductCount;
                            if (leftCout>0 && GetStorageBoard(boardId).Items[index].TimeStoreMax > leftCout  )
                            {
                                GetStorageBoard(boardId).Items[index].TimeStoreMax = leftCout;
                            }
                        }
                    }
                }
            }
        }
        

        return result;
    }

    public int GetTimeProductCount(int index,MergeBoardEnum boardId)
    {
        if (GetStorageBoard(boardId).Items[index] == null)
            return 0;
        return GetStorageBoard(boardId).Items[index].TimeStoreMax;
    }

    public int GetTimeProductCd(int index, ref float percent,MergeBoardEnum boardId)
    {
        var config = GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Items[index].Id);
        if (config == null)
            return 0;

        return GetTimeProductCd(index, config, ref percent,boardId);
    }

    public int GetTimeProductCd(int index, TableMergeItem tableMergeItem, ref float percent,MergeBoardEnum boardId)
    {
        var boardItem = GetStorageBoard(boardId).Items[index];
        if (boardItem.Id == -1)
            return 0;

        if (tableMergeItem == null)
            return 0;

        bool isProductItem = MergeConfigManager.Instance.IsTimeProductItem(tableMergeItem);
        if (!isProductItem)
            return 0;
        // if(boardItem.IsPause)
        //      return boardItem.PauseCDTime;
        //异常处理 --调时间导致越界
        if (APIManager.Instance.GetServerTime() / 1000 < GetStorageBoard(boardId).Items[index].TimProductTime)
            GetStorageBoard(boardId).Items[index].TimProductTime = APIManager.Instance.GetServerTime() / 1000;
        ulong offset = APIManager.Instance.GetServerTime() / 1000 - GetStorageBoard(boardId).Items[index].TimProductTime;
        ulong cds = offset >= (ulong) GetTimeProductCdTime(tableMergeItem)
            ? 0
            : (ulong) GetTimeProductCdTime(tableMergeItem) - offset;
        percent = offset / (float) GetTimeProductCdTime(tableMergeItem);
        return (int) cds;
    }

    public int GetLeftProductCount(int index,MergeBoardEnum boardId)
    {
        if (index < 0 || index >= GetStorageBoard(boardId).Items.Count)
            return 0;
        var config = GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Items[index].Id);
        if (config == null)
        {
            DebugUtil.Log("-----------GetStorageBoard(boardId).Items[index].Id-------------" +
                          GetStorageBoard(boardId).Items[index].Id);
            return 0;
        }

        return GetLeftProductCount(index, config,boardId);
    }

    //剩余产出次数
    public int GetLeftProductCount(int index, TableMergeItem config,MergeBoardEnum boardId)
    {
        int result = 0;
        if (config == null)
            return 0;
        if (GetStorageBoard(boardId) == null)
            return 0;
        ResetProductCount(index, config,boardId);
        bool isStoreItem = MergeConfigManager.Instance.IsStoreItem(config);
        if (!isStoreItem)
            result = GetOutputAmount(config) - Math.Max(0, GetStorageBoard(boardId).Items[index].ProductCount);
        else
            result = GetStorageBoard(boardId).Items[index].StoreMax;
        return result;
    }

    public int GetLeftProductCountInBag(int index,MergeBoardEnum boardId)
    {
        if (index < 0 || index >= GetBagCount(boardId))
            return 0;

        StorageMergeItem mergeItem = GetBagItem(index,boardId);
        if (mergeItem == null)
            return 0;

        TableMergeItem config = GameConfigManager.Instance.GetItemConfig(mergeItem.Id);
        if (config == null)
            return 0;

        bool isStoreItem = MergeConfigManager.Instance.IsStoreItem(config);
        if (!isStoreItem)
            return GetOutputAmount(config) - Math.Max(0, GetStorageBoard(boardId).Items[index].ProductCount);
        else
            return GetStorageBoard(boardId).Items[index].StoreMax;
    }

    public int GetLeftActiveTime(int index, ref float percent,MergeBoardEnum boardId) // 获取剩余激活时间
    {
        int result = 0;
        var itemStorage = MergeManager.Instance.GetBoardItem(index,boardId);
        var itemConfig = GameConfigManager.Instance.GetItemConfig(itemStorage.Id);

        return GetLeftActiveTime(itemStorage, itemConfig, ref percent);
    }

    public int GetLeftActiveTime(int index,MergeBoardEnum boardId) // 获取剩余激活时间
    {
        int result = 0;
        var itemStorage = GetBoardItem(index,boardId);
        if (itemStorage == null)
            return result;

        var itemConfig = GameConfigManager.Instance.GetItemConfig(itemStorage.Id);

        float percent = 0;
        return GetLeftActiveTime(itemStorage, itemConfig, ref percent);
    }

    //剩余激活时间
    public int GetLeftActiveTime(StorageMergeItem itemStorage, TableMergeItem mergeItem, ref float percent)
    {
        int result = 0;

        if (itemStorage == null || mergeItem == null)
            return result;
        //
        // if (itemStorage.IsPause)
        //     return itemStorage.PauseCDTime;

        ActiveCostType type = GetActiveCostType(mergeItem);
        if (type == ActiveCostType.time_active || type == ActiveCostType.time_inactive)
        {
            ulong severTime = APIManager.Instance.GetServerTime() / 1000;
            ulong activeTime = itemStorage.ActiveTime;
            int cdTime = mergeItem.active_cost[1] * 60;
            float offset = (float) (severTime - activeTime);
            result = offset >= cdTime ? 0 : cdTime - (int) offset;

            percent = offset / cdTime;
        }

        return result;
    }

    public ActiveCostType GetActiveCostType(TableMergeItem mergeItem)
    {
        ActiveCostType type = ActiveCostType.none;
        if (mergeItem == null)
            return type;

        if (mergeItem.active_cost == null)
            return type;

        if (mergeItem.active_cost.Length <= 1)
            return type;
        type = (ActiveCostType) mergeItem.active_cost[0];
        return type;
    }

    public int GetTimeProductCdTime(TableMergeItem config)
    {
        int cd = 0;
        if (config != null && config.timeOutput_cost != null && config.timeOutput_cost.Length > 0)
            cd = config.timeOutput_cost[1];
        if (MasterCardModel.Instance.IsBuyMasterCard)
        {
            var reduceCdPre = MasterCardModel.Instance.GetReduceCDPre();
            cd = cd * (100 - reduceCdPre) / 100;
        }

        return cd;
    }

    public  int[] GetCdSpeedCost(TableMergeItem config, StorageMergeItem item)
    {
        int[] cd_speed_cost = config.cdspeed_cost;
        if (!string.IsNullOrEmpty(config.big_smale_cdspeed))
        {
            var cost= config.big_smale_cdspeed.Split(';');
            int index = item.BsIndex % cost.Length;
            var strArr=cost[index].Split(',');
            for (int i = 0; i < strArr.Length; i++)
            {
                cd_speed_cost[i] = int.Parse(strArr[i]);
            }
        }

        return cd_speed_cost;
    }

    public int GetProductCdTime(TableMergeItem config,StorageMergeItem item, MergeBoardEnum boardId)
    {
        int cd = 0;
        
        // if (boardId== MergeBoardEnum.TrainOrder)
        //     return cd;
        
        if (config != null)
            cd = Mathf.CeilToInt(config.cd_time * 60);
        if (config.big_smale_cd != null && config.big_smale_cd.Length > 0)
        {
            int index = item.BsIndex % config.big_smale_cd.Length;
            cd = Mathf.CeilToInt(config.big_smale_cd[index]*60);
            if (item.BsRefreshTime == 0)
                item.BsRefreshTime = APIManager.Instance.GetServerTime();
        }

        return cd;
    }

    public int GetLeftProductTime(int index,MergeBoardEnum boardId)
    {
        float percent = 0;
        return GetLeftProductTime(index, ref percent,boardId);
    }

    public int GetLeftProductTime(int index, ref float percent,MergeBoardEnum boardId) // 获取建筑产出剩余时间
    {
        var item = GetBoardItem(index,boardId);
        int time = 0;
        if (item == null)
            return time;
        var itemConfig = GameConfigManager.Instance.GetItemConfig(item.Id);
        if (itemConfig == null)
            return time;

        if (MergeConfigManager.Instance.IsEnergyProductItem(itemConfig) &&
            GetLeftProductCount(index, itemConfig,boardId) <= 0 && !MergeConfigManager.Instance.IsDeathBoxItem(item.Id, item))
        {
            // if (item.IsPause)
            //     return item.PauseCDTime;
            // 刷新倒计时
            ulong severTime = APIManager.Instance.GetServerTime() / 1000;
            ulong lastProductTime = item.ProductTime;
            int cdTime = GetProductCdTime(itemConfig,item,boardId);

            if (MergeConfigManager.Instance.IsStoreItem(itemConfig) && !itemConfig.is_product_all_cd)
            {
                lastProductTime = item.InCdTime;
            }

            float offset = (int) (severTime - lastProductTime);
            time = offset >= cdTime ? 0 : cdTime - (int) offset;
            percent = offset / cdTime;
        }

        return time;
    }

    public int GetLeftProductTime(int index, TableMergeItem config, ref float percent,MergeBoardEnum boardId) // 获取建筑产出剩余时间
    {
        var item = GetBoardItem(index,boardId);
        int time = 0;
        if (item == null)
            return time;

        if (config == null)
            return time;

        if (MergeConfigManager.Instance.IsEnergyProductItem(config) && GetLeftProductCount(index, config,boardId) <= 0 &&
            !MergeConfigManager.Instance.IsDeathBoxItem(config, item))
        {
            // if (item.IsPause)
            //     return item.PauseCDTime;
            // 刷新倒计时
            ulong severTime = APIManager.Instance.GetServerTime() / 1000;
            ulong lastProductTime = item.ProductTime;
            int cdTime = GetProductCdTime(config,item,boardId);

            if (MergeConfigManager.Instance.IsStoreItem(config) && !config.is_product_all_cd)
            {
                lastProductTime = item.InCdTime;
            }

            float offset = (int) (severTime - lastProductTime);
            if (lastProductTime > severTime)
                Debug.LogError("时间异常 " + offset);
            time = offset >= cdTime ? 0 : cdTime - (int) offset;
            percent = offset / cdTime;
        }

        return time;
    }


    private void ResetProductCount(int index, TableMergeItem itemConfig,MergeBoardEnum boardId)
    {
        if (itemConfig == null)
            return;
        if (index < 0 || index >= GetStorageBoard(boardId).Items.Count)
            return;
        if (GetStorageBoard(boardId) == null || GetStorageBoard(boardId).Items[index] == null)
            return;
        ulong lastProductTime = GetStorageBoard(boardId).Items[index].ProductTime;

        if (MergeConfigManager.Instance.IsStoreItem(itemConfig) && !itemConfig.is_product_all_cd)
        {
            lastProductTime = GetStorageBoard(boardId).Items[index].InCdTime;
        }
        if (GetStorageBoard(boardId).Items.Count > index &&
            !MergeConfigManager.Instance.IsDeathProductItem(itemConfig, GetStorageBoard(boardId).Items[index]) &&
            !MergeConfigManager.Instance.IsDeathBoxItem(itemConfig, GetStorageBoard(boardId).Items[index]))
        {
            var offset = (int) (APIManager.Instance.GetServerTime() / 1000 -
                                lastProductTime)+1;
            
            if (offset > GetProductCdTime(itemConfig,GetStorageBoard(boardId).Items[index],boardId) &&
                GetStorageBoard(boardId).Items[index].ProductCount >= GetOutputAmount(itemConfig))
            {
                GetStorageBoard(boardId).Items[index].ProductCount = 0;
                
                RefreshMaxStoreCount(itemConfig.id, index, itemConfig,boardId);
            }
        }
    }

    /// <summary>
    /// 大小cd记录一次cd恢复
    /// </summary>
    /// <param name="boardItem"></param>
    public void ResumeBuildCD(StorageMergeItem boardItem)
    {
        boardItem.BsIndex ++;
        if (!CommonUtils.IsSameDayWithToday(boardItem.BsRefreshTime))
        {
            boardItem.BsIndex =0;
            boardItem.BsRefreshTime = APIManager.Instance.GetServerTime();
        }
    }
    public void ResetProductCD(int index,MergeBoardEnum boardId)
    {
        if (index == -1)
            return;
        var itemConfig = GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Items[index].Id);
        if (itemConfig == null)
            return;
        // if(isRv && itemConfig.rv_speed_count >0)
        //     GetStorageBoard(boardId).Items[index].ProductCount = itemConfig.output_amount - itemConfig.rv_speed_count;
        // else
        GetStorageBoard(boardId).Items[index].ProductCount = 0;
        GetStorageBoard(boardId).Items[index].ProductTime = 0;
    }

    public void ResetActiveCD(int index,MergeBoardEnum boardId)
    {
        if (index == -1)
            return;
        GetStorageBoard(boardId).Items[index].ActiveTime = 0;
    }

    public void ResetTimeProductCD(int index,MergeBoardEnum boardId)
    {
        if (index == -1)
            return;
        GetStorageBoard(boardId).Items[index].TimProductTime = 0;
        GetStorageBoard(boardId).Items[index].PlayRvNum = 0;
    }

    #endregion 产出

 
    public void RemoveBoardItem(int index, out StorageMergeItem result, string sources, MergeBoardEnum boardId,bool sendEvent = true)
    {
        result = GetStorageBoard(boardId).Items[index];
        var item = (StorageMergeItem) mergeItemPool.Pop();
        int removeId = result.Id;
        item.Id = -1;
        item.State = 1;
        ResetStorageMergeBoardStatus(item);
        GetStorageBoard(boardId).Items[index] = item;
        mergeItemCountsDirty = true;

        if (sendEvent)
            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_REFRESH, (int)boardId,index, -1,
                RefreshItemSource.remove, removeId);
        
        GameBIManager.Instance.SendMergeEvent((new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemChangeRemoveItem,
            itemAId =removeId,
            data1 = ((int)boardId).ToString(),
            data2 = (index).ToString(),
            data3 = sources,
        }));
    }

    public void ResetStorageMergeBoardStatus(StorageMergeItem item) // 2021-12-21  当通过任何途径获得新建筑时（包括合成），新建筑会有一个初始库存值，该值在mergeItem表配置.
    {
        if (item == null)
            return;
        item.ProductCount = 0;
        item.TimeProductCount = 0;
        item.ProductItems.Clear();
        item.ProductTime = 0;
        item.TimProductTime = 0;
        item.OpenTime = 0;
        item.InCdTime = 0;
        item.StoreMax = 0;
        item.DropIntervalDic.Clear();
        item.IsPause = false;
        item.ProductWheel = 0;
        item.TimeStoreMax = 0;
        item.EatBuildingDic.Clear();
        
        if (item.Id != -1)
        {
            var config = GameConfigManager.Instance.GetItemConfig(item.Id);
            if (config != null)
            {
                if (config.original_count > 0) // 恢复初始量
                {
                    item.ProductCount = GetOutputAmount(config) - config.original_count;
                }
            }
        }
    }
    public void RemoveAllItemByType(MergeItemType itemType,MergeBoardEnum boardId, string sources)
    {
        if (MergeMainController.Instance != null && MergeMainController.Instance.MergeBoard != null)
            MergeMainController.Instance.MergeBoard.RestoreInput();
        UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergePackage)?.CloseWindowWithinUIMgr(true);
        for (int i = 0; i < GetStorageBoard(boardId).Items.Count; i++)
        {
            if (GetStorageBoard(boardId).Items[i].Id<=0)
                continue;
            var config = GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Items[i].Id);
            if (config!=null && config.type ==(int) itemType)
            {
                RemoveBoardItem(i,boardId,sources);
            }
        }
        for (int i = GetStorageBoard(boardId).Rewards.Count-1; i >=0; i--)
        {
            if (GetStorageBoard(boardId).Rewards[i].Id<=0)
                continue;
            var config = GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Rewards[i].Id);
            if (config!=null && config.type ==(int) itemType)
            {
                GetStorageBoard(boardId).Rewards.Remove(GetStorageBoard(boardId).Rewards[i]);
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
            }
        }    
        for (int i = GetStorageBoard(boardId).Bags.Count-1; i >=0; i--)
        {
            if (GetStorageBoard(boardId).Bags[i].Id<=0)
                continue;
            var config = GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Bags[i].Id);
            if (config!=null && config.type ==(int) itemType)
            {
                GetStorageBoard(boardId).Bags.Remove(GetStorageBoard(boardId).Bags[i]);
            }
        }
   

    }
    
    
    private int[] easterLines= new[] { 22101, 22111,22112 };
    /// <summary>
    /// 复活节相关替换
    /// </summary>
    public void ReplaceEaster(MergeBoardEnum boardId)
    {
        foreach (var itemLine in easterLines)
        {
            RePlaceAllItemByLine(itemLine,boardId,"EasterReplace");
        }
    }

    private int[] sealLines = new[] { 21101, 21111 };
    private int[] dolphinLines = new[] { 23101, 23111 };
    private int[] capybaraLines = new[] { 24101, 24111 };
    /// <summary>
    /// 海豹相关替换
    /// </summary>
    public void ReplaceSeal(MergeBoardEnum boardId)
    {
        foreach (var itemLine in sealLines)
        {
            RePlaceAllItemByLine(itemLine,boardId,"SealReplace");
        }
    }

    public void ReplaceCapybara(MergeBoardEnum boardId)
    {
        foreach (var itemLine in capybaraLines)
        {
            RePlaceAllItemByLine(itemLine,boardId,"CapybaraReplace");
        }
    }
    
    public void ReplaceDolphin(MergeBoardEnum boardId)
    {
        foreach (var itemLine in dolphinLines)
        {
            RePlaceAllItemByLine(itemLine,boardId,"DolphinReplace");
        }
    }
    
    public void CheckSealReplaceItem(MergeBoardEnum boardId)
    {
        foreach (var itemLine in sealLines)
        {
            if (HaveItemByLine(itemLine,boardId))
            {
                UICommonTpsController.OpenCommonTips("ui_item_recycle_special_item", () =>
                {
                    ReplaceSeal(boardId);
                });
                break;
            }
        }
    }
    
    public void CheckCapybaraReplaceItem(MergeBoardEnum boardId)
    {
        foreach (var itemLine in capybaraLines)
        {
            if (HaveItemByLine(itemLine,boardId))
            {
                UICommonTpsController.OpenCommonTips("ui_item_recycle_special_capybara", () =>
                {
                    ReplaceCapybara(boardId);
                });
                break;
            }
        }
    }
    
    public void CheckDolphinReplaceItem(MergeBoardEnum boardId)
    {
        foreach (var itemLine in dolphinLines)
        {
            if (HaveItemByLine(itemLine,boardId))
            {
                UICommonTpsController.OpenCommonTips("ui_item_recycle_special_dolphin", () =>
                {
                    ReplaceDolphin(boardId);
                });
                break;
            }
        }
    }

    public void CheckEasterReplaceItem(MergeBoardEnum boardId)
    {
        foreach (var itemLine in easterLines)
        {
            if (HaveItemByLine(itemLine,boardId))
            {
                UICommonTpsController.OpenCommonTips("ui_item_recycle_event_item", () =>
                {
                    ReplaceEaster(boardId);
                });
                break;
            }
        }
    }

    public int GetProductCdByLine(int line,MergeBoardEnum boardId)
    {
        if (GetStorageBoard(boardId) == null || GetStorageBoard(boardId).Items == null)
            return 0;
        
        for (int i = 0; i < GetStorageBoard(boardId).Items.Count; i++)
        {
            if (GetStorageBoard(boardId).Items[i].Id <= 0)
                continue;
            var config = GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Items[i].Id);
            if(config == null)
                continue;

            if (config.in_line == line)
            {
                return GetLeftProductTime(i,boardId);
            }
        }

        return 0;

    }
    public int GetTimeProductCdByLine(int line,MergeBoardEnum boardId)
    {
        if (GetStorageBoard(boardId) == null || GetStorageBoard(boardId).Items == null)
            return 0;
        
        for (int i = 0; i < GetStorageBoard(boardId).Items.Count; i++)
        {
            if (GetStorageBoard(boardId).Items[i].Id <= 0)
                continue;
            var config = GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Items[i].Id);
            if(config == null)
                continue;

            if (config.in_line == line&&  MergeConfigManager.Instance.IsTimeProductItem(config))
            {
                float pre = 0;
                return GetTimeProductCd(i,ref pre,boardId);
            }
              
        }

        return 0;

    }

    public bool HaveItemByLine(int line,MergeBoardEnum boardId)
    {
        for (int i = 0; i < GetStorageBoard(boardId).Items.Count; i++)
        {
            if (GetStorageBoard(boardId).Items[i].Id<=0)
                continue;
            var config=GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Items[i].Id);

            if (config.in_line == line)
                return true;
        }
        
        for (int i = GetStorageBoard(boardId).Rewards.Count-1; i >=0; i--)
        {
            if (GetStorageBoard(boardId).Rewards[i].Id<=0)
                continue;
            var config=GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Rewards[i].Id);

            if (config.in_line == line)
                return true;
        }    
        
        for (int i = GetStorageBoard(boardId).Bags.Count-1; i >=0; i--)
        {
            if (GetStorageBoard(boardId).Bags[i].Id<=0)
                continue;
            var config=GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Bags[i].Id);

            if (config.in_line == line)
                return true;
        }

        return false;
    }
    public void RePlaceAllItemByLine(int line,MergeBoardEnum boardId,string sources)
    {
        if (MergeMainController.Instance != null && MergeMainController.Instance.MergeBoard != null)
            MergeMainController.Instance.MergeBoard.RestoreInput();
        UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergePackage)?.CloseWindowWithinUIMgr(true);
        for (int i = 0; i < GetStorageBoard(boardId).Items.Count; i++)
        {
            if (GetStorageBoard(boardId).Items[i].Id<=0)
                continue;
            var config=GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Items[i].Id);

            if (config.in_line == line)
            {
                int index = i;
                MergeBoard.Grid grid =MergeMainController.Instance.MergeBoard.GetGridByIndex(index);
                UIRoot.Instance.EnableEventSystem = false;
                CoroutineManager.Instance.StartCoroutine(CommonUtils.PlayAnimation(grid.board.Animator, "small",
                    null,
                    () =>
                    {
                        grid.board.PlayRemoveEffect();
                        
                        CommonUtils.DelayedCall(0.5f, () =>
                        {
                            UIRoot.Instance.EnableEventSystem = true;
                            if (config.recovery_item > 0)
                            {
                                SetBoardItem(index, config.recovery_item, 1, RefreshItemSource.notDeal,boardId);
                            }
                            else
                            {
                                RemoveBoardItem(index,boardId,sources);
                            }
                        });
                    }));
            }

        }
        for (int i = GetStorageBoard(boardId).Rewards.Count-1; i >=0; i--)
        {
            if (GetStorageBoard(boardId).Rewards[i].Id<=0)
                continue;
            var config=GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Rewards[i].Id);

            if (config.in_line == line)
            {
                if (config.recovery_item > 0)
                {
                    GetStorageBoard(boardId).Rewards[i].Id = config.recovery_item;
                }
                else
                {
                    GetStorageBoard(boardId).Rewards.Remove(GetStorageBoard(boardId).Rewards[i]);
                }
            }
                
            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
        }    
        for (int i = GetStorageBoard(boardId).Bags.Count-1; i >=0; i--)
        {
            if (GetStorageBoard(boardId).Bags[i].Id<=0)
                continue;
            var config=GameConfigManager.Instance.GetItemConfig(GetStorageBoard(boardId).Bags[i].Id);

            if (config.in_line == line)
            {
                if (config.recovery_item > 0)
                {
                    GetStorageBoard(boardId).Bags[i].Id = config.recovery_item;
                }
                else
                {
                    GetStorageBoard(boardId).Bags.Remove(GetStorageBoard(boardId).Bags[i]);
                }
            }
             
        }
   

    }
    public void RemoveBoardItem(int index,MergeBoardEnum boardId, string sources, bool sendEvent = true)
    {
        StorageMergeItem result;
        RemoveBoardItem(index, out result, sources, boardId, sendEvent);
        mergeItemPool.Push(result);
    }

    public bool IsBoardItemExist(int index,MergeBoardEnum boardId)
    {
        if (index < 0 || index >= GetStorageBoard(boardId).Items.Count)
            return false;

        return GetStorageBoard(boardId).Items[index].Id > 0;
    }

    public void SwapBoardItem(int a, int b,MergeBoardEnum boardId)
    {
        StorageMergeItem temp = GetStorageBoard(boardId).Items[a];
        GetStorageBoard(boardId).Items[a] = GetStorageBoard(boardId).Items[b];
        GetStorageBoard(boardId).Items[b] = temp;
    }

    public int FindEmptyGrid(int index,MergeBoardEnum boardId,bool ignoreSelf = false)
    {
        return FindEmptyGridView(index,boardId,ignoreSelf);
    }

    public bool MergeBoardIsFull(MergeBoardEnum boardId)
    {
        for (int i = 0; i < GetStorageBoard(boardId).Items.Count; i++)
        {
            if (GetStorageBoard(boardId).Items[i].Id == -1)
                return false;
        }
        
        return true;
    }

    public int GetLeftEmptyGridCount(MergeBoardEnum boardId)
    {
        var storage = storageMergeBoardDict.GetValue((int)boardId);
        if (storage == null)
            return -1;
        var leftCount = 0;
        for (int i = 0; i < storage.Items.Count; i++)
        {
            if (storage.Items[i].Id == -1)
                leftCount++;
        }
        return leftCount;
    }
    public int GetBagGridCount(MergeBoardEnum boardId)
    {
        var storage = storageMergeBoardDict.GetValue((int)boardId);
        if (storage == null)
            return -1;
        return storage.BagCapacity;
    }    
    
    public int GetBuildBagGridCount(MergeBoardEnum boardId)
    {
        var storage = storageMergeBoardDict.GetValue((int)boardId);
        if (storage == null)
            return -1;
        return storage.BuildingBagCapacity;
    }
    public int GetLeftBagEmptyGridCount(MergeBoardEnum boardId)
    {
        var storage = storageMergeBoardDict.GetValue((int)boardId);
        if (storage == null)
            return -1;
        var leftCount = storage.BagCapacity - storage.Bags.Count - storage.VipBags.Count;
        return leftCount;
    }    
    public int GetLeftBuildBagEmptyGridCount(MergeBoardEnum boardId)
    {
        var storage = storageMergeBoardDict.GetValue((int)boardId);
        if (storage == null)
            return -1;
        var leftCount = storage.BuildingBagCapacity - storage.BuildingBags.Count - storage.VipBags.Count;
        return leftCount;
    }

    //九宫遍历 上 右上 右 右下 左
    public int FindEmptyGridView(int index,MergeBoardEnum boardId,bool ignoreSelf=false)
    {
        //检测自己是否是空位置
        StorageMergeItem boardStorage = GetBoardItem(index,boardId);
        if (!ignoreSelf && boardStorage != null && boardStorage.Id == -1)
            return index;

        int boardWidth = GetBoardWidth(boardId);
        int boardHeight = GetBoardHeight(boardId);

        int curX = index % boardWidth;
        int curY = index / boardWidth;

        int xLoop = Math.Max(boardWidth - curX, curX);
        int yLoop = Math.Max(boardHeight - curY, curY);
        int loopNum = Math.Max(xLoop, yLoop);

        int emptyIndex = -1;

        for (int i = 1; i <= loopNum; i++)
        {
            //top right
            int y = curY + i;
            if (y < boardHeight)
            {
                for (int x1 = curX; x1 <= Math.Min(curX + i, boardWidth - 1); x1++)
                {
                    emptyIndex = x1 + y * boardWidth;
                    StorageMergeItem storage = GetBoardItem(emptyIndex,boardId);
                    if (storage != null && storage.Id == -1)
                        return emptyIndex;
                }
            }

            //left
            int x = curX + i;
            if (x < boardWidth)
            {
                for (int y1 = Math.Max(curY + i - 1, 0); y1 >= curY - i; y1--)
                {
                    emptyIndex = x + y1 * boardWidth;
                    StorageMergeItem storage = GetBoardItem(emptyIndex,boardId);
                    if (storage != null && storage.Id == -1)
                        return emptyIndex;
                }
            }

            //down
            y = curY - i;
            if (y >= 0)
            {
                for (int x1 = Math.Min(curX + i - 1, boardWidth - 1); x1 >= Math.Max(curX - i, 0); x1--)
                {
                    emptyIndex = x1 + y * boardWidth;
                    StorageMergeItem storage = GetBoardItem(emptyIndex,boardId);
                    if (storage != null && storage.Id == -1)
                        return emptyIndex;
                }
            }

            //Right
            x = curX - i;
            if (i >= 0)
            {
                for (int y1 = curY - i; y1 <= Math.Min(curY + i, boardHeight - 1); y1++)
                {
                    emptyIndex = x + y1 * boardWidth;
                    StorageMergeItem storage = GetBoardItem(emptyIndex,boardId);
                    if (storage != null && storage.Id == -1)
                        return emptyIndex;
                }
            }

            //top Right
            y = curY + i;
            if (y < boardHeight)
            {
                for (int x1 = curX - i; x1 <= curX; x1++)
                {
                    emptyIndex = x1 + y * boardWidth;
                    StorageMergeItem storage = GetBoardItem(emptyIndex,boardId);
                    if (storage != null && storage.Id == -1)
                        return emptyIndex;
                }
            }
        }
        if (ignoreSelf && boardStorage != null && boardStorage.Id == -1)
            return index;
        return -1;
    }

    public int GetSudokuEmptIndex(int m_index,MergeBoardEnum boardId) //获取该位置九宫格区域的空位
    {
        int boardWidth = GetBoardWidth(boardId);
        int boardHeight = GetBoardHeight(boardId);
        int result = -1;
        int x = m_index % boardWidth;
        int y = m_index / boardWidth;
        int index = -1;
        for (int i = Mathf.Max(x - 1, 0); i <= Mathf.Min(x + 1, boardWidth - 1); i++)
        {
            for (int j = Mathf.Max(y - 1, 0); j <= Mathf.Min(y + 1, boardHeight - 1); j++)
            {
                index = i + j * boardWidth;
                var storage = GetBoardItem(index,boardId);
                if (storage.Id == -1)
                {
                    result = index;
                    break;
                }
            }
        }

        return result;
    }

    #region bag

    public int GetBagCount(MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).Bags.Count;
    }

    public int GetBagCapacity(MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).BagCapacity;
    }

    public int GetBuildingBagCapacity(MergeBoardEnum boardId)
    {
        int count = GetStorageBoard(boardId).BuildingBagCapacity;
        if (count <= 0)
        {
            GetStorageBoard(boardId).BuildingBagCapacity=GameConfigManager.Instance.BagBuildingList.FindAll(x => x.CointCost <= 0).Count;
        }
        return GetStorageBoard(boardId).BuildingBagCapacity;
    }

    public bool IsBagFull(MergeBoardEnum boardId)
    {
        return GetBagCount(boardId) >= GetBagCapacity(boardId);
    }

    public int GetLeftBagCount(MergeBoardEnum boardId)
    {
        int leftCount = GetBagCapacity(boardId) - MergeManager.Instance.GetBagCount(boardId);
        leftCount = Mathf.Max(0, leftCount);

        return leftCount;
    }
    
    public int GetEmptBoardItemCount(MergeBoardEnum boardId) //获取剩余棋盘数量
    {
        var boardItems = GetStorageBoard(boardId).Items.FindAll(x => x.Id <= -1);
        if (boardItems == null)
            return 0;
        return boardItems.Count;
    }

    public int GetStorBoardItemCount(MergeBoardEnum boardId) //获取暂存物品数量
    {
        return GetStorageBoard(boardId).Rewards.Count;
    }


    public StorageMergeItem GetBagItem(int index,MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).Bags[index];
    }

    public void AddBagItem(StorageMergeItem storageMergeItem,MergeBoardEnum boardId)
    {
        GetStorageBoard(boardId).Bags.Add(storageMergeItem);

        mergeItemCountsDirty = true;
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BAG_REFRESH);
        SendMovetoBagBi(storageMergeItem.Id);
    }


    public int GetBuildingBagCount(MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).BuildingBags.Count;
    }

    public StorageMergeItem GetBuildingBagItem(int index,MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).BuildingBags[index];
    }

    public void AddBuildingBagItem(StorageMergeItem storageMergeItem,MergeBoardEnum boardId)
    {
        
        GetStorageBoard(boardId).BuildingBags.Add(storageMergeItem);
        mergeItemCountsDirty = true;
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BAG_REFRESH);
        SendMovetoBagBi(storageMergeItem.Id);
    }

    public void RemoveBagItem(int index, out StorageMergeItem result,MergeBoardEnum boardId)
    {
        result = GetStorageBoard(boardId).Bags[index];
        GetStorageBoard(boardId).Bags.RemoveAt(index);
        mergeItemCountsDirty = true;
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BAG_REFRESH);
    }

    public void RemoveBuildingBagItem(int index, out StorageMergeItem result,MergeBoardEnum boardId)
    {
        result = GetStorageBoard(boardId).BuildingBags[index];
        GetStorageBoard(boardId).BuildingBags.RemoveAt(index);
        mergeItemCountsDirty = true;
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BAG_REFRESH);
    }

    public void RemoveBagItem(int index,MergeBoardEnum boardId)
    {
        StorageMergeItem result;
        RemoveBagItem(index, out result,boardId);
        mergeItemPool.Push(result);
    }
    public void RemoveBuildingBagItem(int index,MergeBoardEnum boardId)
    {
        StorageMergeItem result;
        RemoveBuildingBagItem(index, out result,boardId);
        mergeItemPool.Push(result);
    }

    public void UseBgItem(int index, int emptyIndex,MergeBoardEnum boardId)
    {
        int bagCount = GetBagCount(boardId);
        if (bagCount <= 0)
            return;

        if (emptyIndex < 0)
            return;

        StorageMergeItem item;
        RemoveBagItem(index, out item,boardId);
        SetBoardItem(emptyIndex, item, RefreshItemSource.bag, boardId,false);
        ResumeCd(emptyIndex,boardId);
        SendUseBagBi(item.Id);
    }

    public void UseBuildingBgItem(int index, int emptyIndex,MergeBoardEnum boardId)
    {
        int bagCount = GetStorageBoard(boardId).BuildingBags.Count;
        if (bagCount <= 0)
        {
            return;
        }

        if (emptyIndex < 0)
            return;

        StorageMergeItem item;
        RemoveBuildingBagItem(index, out item,boardId);
        SetBoardItem(emptyIndex, item, RefreshItemSource.bag, boardId,false);
        ResumeCd(emptyIndex,boardId);
        SendUseBagBi(item.Id);
    }

    private void SendUseBagBi(int id)
    {
        var product = GameConfigManager.Instance.GetItemConfig(id);
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemMoveBagToGame,
            itemAId = product.id,
            ItemALevel = product.level,
            isChange = false,
            extras = new Dictionary<string, string>
            {
                {"from", "bag"},
                {"to", "game"},
            }
        });
    }


    private void SendMovetoBagBi(int id)
    {
        if (id < 0)
            return;

        var product = GameConfigManager.Instance.GetItemConfig(id);
        if (product == null)
            return;

        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemMoveGameToBag,
            itemAId = product.id,
            ItemALevel = product.level,
            isChange = false,
            extras = new Dictionary<string, string>
            {
                {"from", "game"},
                {"to", "bag"},
            }
        });
    }

    #endregion bag

    public int GetRewardCount(MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).Rewards.Count;
    }

    public StorageMergeItem GetRewardItem(int index,MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).Rewards[index];
    }

    public void AddRewardItem(StorageMergeItem storageMergeItem,MergeBoardEnum boardId, int cout = 1, bool inFirst = false)
    {
        for (int i = 0; i < cout; i++)
        {
            if(!inFirst && ExperenceModel.Instance.GetLevel()< GlobalConfigManager.Instance.GetNumValue("reward_infirst_level"))
                GetStorageBoard(boardId).Rewards.Add(storageMergeItem);
            else
            {
                GetStorageBoard(boardId).Rewards.Insert(0, storageMergeItem);
            }
            mergeItemCountsDirty = true;
            AddShowedItemId(storageMergeItem.Id);
            RecordGetItem(storageMergeItem);
        }
    }

    public void RemoveRewardItem(int index, out StorageMergeItem result,MergeBoardEnum boardId, bool isSendEvent = true)
    {
        result = GetStorageBoard(boardId).Rewards[index];
        GetStorageBoard(boardId).Rewards.RemoveAt(index);
        mergeItemCountsDirty = true;
        if (isSendEvent)
            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
    }

    public void RemoveRewardItem(int index,MergeBoardEnum boardId)
    {
        StorageMergeItem result;
        RemoveRewardItem(index, out result,boardId);
        mergeItemPool.Push(result);
    }

    public bool IsShowedItemId(int id) //是否出现过item
    {
        bool result = false;
        if (unlockItems == null || unlockItems.UnlockIds == null)
            return result;
        result = unlockItems.UnlockIds.Contains(id);
        return result;
    }

    public bool IsItemGetGalleryAwards(int id)
    {
        return unlockItems.GalleryAwarded.Contains(id);
    }

    public void RecordItemGalleryAwards(int id)
    {
        if (!unlockItems.GalleryAwarded.Contains(id))
            unlockItems.GalleryAwarded.Add(id);
    }

    public void RecordGetItem(StorageMergeItem item)
    {
        if(item == null || item.Id < 0)
            return;
        
        if(item.State != 1)
            return;
        
        if(unlockItems.GetIds.ContainsKey(item.Id))
            return;
        
        if(item.Id == 100004 && !unlockItems.GetIds.ContainsKey(100104))
            unlockItems.GetIds.Add(100104, 100104);
            
        unlockItems.GetIds.Add(item.Id, item.Id);
    }

    public void InitRecordGetItems(MergeBoardEnum boardId)
    {
        if(unlockItems.GetIds.Count > 0)
            return;

        foreach (var storageItem in GetStorageBoard(boardId).Items)
        {
            if (storageItem.State == 1)
                RecordGetItem(storageItem);
        }
        
        foreach (var storageMergeItem in GetStorageBoard(boardId).Bags)
        {
            RecordGetItem(storageMergeItem);
        }
        
        foreach (var storageMergeReward in GetStorageBoard(boardId).Rewards)
        {
            RecordGetItem(storageMergeReward);
        }
    }

    public bool IsGetItem(int id)
    {
        return unlockItems.GetIds.ContainsKey(id);
    }
    

    public bool IsGalleryShowRedPoint()
    {
        int showCount = 0;
        for (int i = 0; i < unlockItems.UnlockIds.Count; i++)
        {
            if (MergeConfigManager.Instance.IsHaveGalleryAwards(unlockItems.UnlockIds[i]))
                showCount += 1;
        }

        return unlockItems.GalleryAwarded.Count != showCount;
    }

    public void AddShowedItemId(int id)
    {
        if (unlockItems == null || unlockItems.UnlockIds == null)
            return;
        if (!unlockItems.UnlockIds.Contains(id))
            unlockItems.UnlockIds.Add(id);
        
        if (id == 100004)//假工具箱做个特殊处理
        {
            if (!unlockItems.UnlockIds.Contains(100104))
                unlockItems.UnlockIds.Add(100104);
        }
        var config = GameConfigManager.Instance.GetItemConfig(id);
        if (config != null)
            if (!unlockItems.UnlockLines.Contains(config.in_line))
                unlockItems.UnlockLines.Add(config.in_line);
    }
    
    /// <summary>
    /// 判断合成链是否解锁 
    /// </summary>
    /// <param name="mergeLineID"></param>
    /// <returns></returns>
    public bool IsUnlockLines(int mergeLineID)
    {
        if (unlockItems == null || unlockItems.UnlockLines == null)
            return false;
        return unlockItems.UnlockLines.Contains(mergeLineID);
    }

    #region 存贮量 建筑

    public int GetMaxOutputAmount(TableMergeItem itemConfig)
    {
        if (itemConfig == null)
            return 0;
        if (MasterCardModel.Instance.IsBuyMasterCard && itemConfig.type ==  (int) MergeItemType.item)
        {
            var productAddPre = MasterCardModel.Instance.GetProductAddPre();
            int amount = Mathf.CeilToInt(1f * (100 + productAddPre) * itemConfig.max_output_amount / 100f);
            return amount;
        }

        return itemConfig.max_output_amount;
    }

    public int GetOutputAmount(TableMergeItem itemConfig)
    {
        if (itemConfig == null)
            return 0;
        if (MasterCardModel.Instance.IsBuyMasterCard && itemConfig.type == (int) MergeItemType.item)
        {
            var productAddPre = MasterCardModel.Instance.GetProductAddPre();
            int amount = Mathf.CeilToInt(1f * (100 + productAddPre) * itemConfig.output_amount / 100f);
            return amount;
        }

        return itemConfig.output_amount;
    }

    public int GetTimeMaxOutputAmount(TableMergeItem itemConfig)
    {
        if (itemConfig == null)
            return 0;
        if (MasterCardModel.Instance.IsBuyMasterCard && itemConfig.type == (int) MergeItemType.item)
        {
            var productAddPre = MasterCardModel.Instance.GetProductAddPre();
            int amount = Mathf.CeilToInt(1f * (100 + productAddPre) * itemConfig.time_max_output_amount / 100f);
            return amount;
        }

        return itemConfig.time_max_output_amount;
    }

    public int GetTimeOutputAmount(TableMergeItem itemConfig)
    {
        if (itemConfig == null)
            return 0;
        if (MasterCardModel.Instance.IsBuyMasterCard && itemConfig.type == (int) MergeItemType.item)
        {
            var productAddPre = MasterCardModel.Instance.GetProductAddPre();
            int amount = Mathf.CeilToInt(1f * (100 + productAddPre) * itemConfig.time_output_amount / 100f);
            return amount;
        }

        return itemConfig.time_output_amount;
    }

    public void RefreshMaxStoreCount(int id, int index, TableMergeItem itemConfig,MergeBoardEnum boardId) // 刷新物品的最大存贮量
    {
        if (itemConfig == null)
            return;

        bool isStoreItem = MergeConfigManager.Instance.IsStoreItem(itemConfig);
        if (!isStoreItem)
            return;

        var itemStorage = GetBoardItem(index,boardId);
        if (itemStorage == null || itemStorage.State != 1)
            return;
        ulong severTime = APIManager.Instance.GetServerTime() / 1000;

        //兼容处理 有可能之前建筑没有产出，改为了产出建筑
        if (itemStorage.StoreMax <= 0 && itemStorage.ProductTime == 0 && itemStorage.InCdTime == 0)
        {
            itemStorage.ProductTime =severTime;
            itemStorage.InCdTime = severTime;
            itemStorage.StoreMax = GetMaxOutputAmount(itemConfig);
        }
        
        ulong lastCdTime =itemConfig.is_product_all_cd?itemStorage.ProductTime: itemStorage.InCdTime;
        if (lastCdTime <= 0)
            return;
        if(itemConfig.type==99 &&itemStorage.StoreMax>0 )//防止花荼未完成产出重置
            return;
        
        float cdTime = GetProductCdTime(itemConfig,itemStorage,boardId);
        ulong offset = severTime - lastCdTime;
        if (cdTime == 0)
            return;
        int recoverCount = Mathf.FloorToInt(offset / cdTime);
        // if (id == 102104)
        //      DebugUtil.LogError("cdTime:" + cdTime + "offset:" + offset + "recoverCount:" + recoverCount + "--severTime:" + severTime + "-----lastCdTime:" + lastCdTime);
        if (recoverCount > 0)
        {
            if (itemStorage.StoreMax < 0)
                itemStorage.StoreMax = 0;
            if (itemConfig.is_product_all_cd && itemStorage.StoreMax > 0)
            {
                itemStorage.InCdTime = severTime;
            }
            else
            {
                itemStorage.StoreMax += recoverCount * GetOutputAmount(itemConfig);
                if (itemStorage.StoreMax > GetMaxOutputAmount(itemConfig) || itemStorage.StoreMax < 0)
                    itemStorage.StoreMax = GetMaxOutputAmount(itemConfig);
                itemStorage.InCdTime = severTime;
            }
            ResumeBuildCD(GetStorageBoard(boardId).Items[index]);
        }
    }

    public void RefreshInCdTime(int id, int index,MergeBoardEnum boardId)
    {
        bool isInCd = IsInStoreCd(id, index,boardId);
        if (!isInCd)
        {
            GetBoardItem(index,boardId).InCdTime = APIManager.Instance.GetServerTime() / 1000;
        }
    }

    //单个建筑加速
    public void SpeedUpOneItem(StorageMergeItem item, ulong time)
    {
        if (item.ProductTime > time)
            item.ProductTime -= time;
        else
            item.ProductTime = 0;
        if (item.TimProductTime > time)
            item.TimProductTime -= time;
        else
            item.TimProductTime = 0;
        if (item.InCdTime > time)
            item.InCdTime -= time;
        else
            item.InCdTime = 0;
        if (item.ActiveTime > time)
            item.ActiveTime -= time;
        else
            item.ActiveTime = 0;
    }

    public void SpeedUpAllItem(ulong time,MergeBoardEnum boardId) // 加速所有产出建筑
    {
        for (int i = 0; i < GetStorageBoard(boardId).Items.Count; i++)
        {
            var item = GetStorageBoard(boardId).Items[i];
            SpeedUpOneItem(item, time);
        }
    }

    public void SetOrginalStoreCount(int id, int index, MergeBoardEnum boardId, int extraProduct = 0, int stack = 0) // 得到存贮量 建筑设置最初的产出建筑存贮量
    {
        var itemConfig = GameConfigManager.Instance.GetItemConfig(id);
        if (itemConfig == null)
            return;
        var itemStorage = GetBoardItem(index,boardId);
        if (itemStorage == null)
            return;

        if (itemConfig.canStacking)
        {
           if(stack > 0)
               itemStorage.StackNum = stack;
           else
               itemStorage.StackNum = itemConfig.defaultStackNum;
        }
        
        bool isStoreItem = MergeConfigManager.Instance.IsStoreItem(itemConfig);
        if (!isStoreItem)
            return;
       
        if (index < 0 || index >= GetStorageBoard(boardId).Items.Count)
            return;
        if (itemConfig.type == (int) MergeItemType.eatBuild)
        {
            itemStorage.State = 3;
            itemStorage.StoreMax = 0;
            return;
        }
        if (itemStorage.State != 1)
            return;
        if (itemConfig.original_count > 0)
            itemStorage.StoreMax = itemConfig.original_count + extraProduct;
        else
            itemStorage.StoreMax = GetOutputAmount(itemConfig) + extraProduct;

        itemStorage.InCdTime = APIManager.Instance.GetServerTime() / 1000;
    }

    public void SetOrginalStoreCount(StorageMergeItem item, bool isUseRv = false) // 得到存贮量 建筑设置最初的产出建筑存贮量
    {
        if (item == null)
            return;
        var itemStorage = item;
        if (itemStorage == null || itemStorage.State != 1)
            return;

        var itemConfig = GameConfigManager.Instance.GetItemConfig(item.Id);
        if (itemConfig == null)
            return;

        bool isStoreItem = MergeConfigManager.Instance.IsStoreItem(itemConfig);
        if (!isStoreItem)
            return;
        // if(isUseRv && itemConfig.rv_speed_count > 0)
        //     itemStorage.StoreMax = itemConfig.rv_speed_count;
        // else
        itemStorage.StoreMax = GetOutputAmount(itemConfig);

        itemStorage.InCdTime = APIManager.Instance.GetServerTime() / 1000;
    }

    public void SetTimeProductStoreCount(StorageMergeItem item, bool isUseRv = false) // x
    {
        if (item == null)
            return;
        var itemStorage = item;
        if (itemStorage == null || itemStorage.State != 1)
            return;

        var itemConfig = GameConfigManager.Instance.GetItemConfig(item.Id);
        if (itemConfig == null)
            return;

        bool isStoreItem = MergeConfigManager.Instance.IsTimeStoreItem(itemConfig);
        if (!isStoreItem)
            return;
        if (isUseRv && itemConfig.rv_speed_count > 0)
        {
            int amount = itemConfig.rv_speed_count;
            if (MasterCardModel.Instance.IsBuyMasterCard && itemConfig.type == (int) MergeItemType.item)
            {
                var productAddPre = MasterCardModel.Instance.GetProductAddPre();
                amount = Mathf.CeilToInt(1f * (100 + productAddPre) * itemConfig.rv_speed_count / 100f);
            }

            itemStorage.TimeStoreMax = amount;
        }
        else
        {
            itemStorage.TimeStoreMax = GetTimeOutputAmount(itemConfig);
        }
    }

    public void SetOrginalStoreCount(StorageMergeItem item, RefreshItemSource source,MergeBoardEnum boardId) // 得到存贮量 建筑设置最初的产出建筑存贮量
    {
        switch (source)
        {
            case RefreshItemSource.product:
            case RefreshItemSource.mergeOk:
            case RefreshItemSource.mergeOk_omnipoten:
            case RefreshItemSource.timeProduct:
            case RefreshItemSource.rewards:
            case RefreshItemSource.webUnlock:
            case RefreshItemSource.mergeBubble:
                int index = GetStorageBoard(boardId).Items.IndexOf(item);
                GetBoardItem(index,boardId).OpenTime = APIManager.Instance.GetServerTime() / 1000;
                SetOrginalStoreCount(item.Id, index,boardId);
                break;
        }
    }

    public bool IsInStoreCd(int id, int index,MergeBoardEnum boardId)
    {
        bool result = false;
        var itemConfig = GameConfigManager.Instance.GetItemConfig(id);
        var itemStorage = GetBoardItem(index,boardId);
        if (itemConfig == null)
            return result;

        if (GetMaxOutputAmount(itemConfig) > 0)
        {
            result = itemStorage.StoreMax < GetMaxOutputAmount(itemConfig);
        }

        return result;
    }

    #endregion


    #region 气泡产出

    public void AddMergeCount(int id, int newId,MergeBoardEnum boardId)
    {
        int mergeCount = 0;

        if (GetStorageBoard(boardId).MergeCounts.ContainsKey(id))
        {
            GetStorageBoard(boardId).MergeCounts.TryGetValue(id, out mergeCount);
            mergeCount += 1;
            GetStorageBoard(boardId).MergeCounts[id] = mergeCount;
        }
        else
        {
            mergeCount += 1;
            GetStorageBoard(boardId).MergeCounts.Add(id, mergeCount);
        }

        GetStorageBoard(boardId).MergeCount += 1;
    }

    public int GetMergeCount(MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).MergeCount;
    }

    public void AddFinishTaskCount(MergeBoardEnum boardId)
    {
        GetStorageBoard(boardId).FinishTaskCount += 1;
    }

    public int GetFinishTaskCount(MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).FinishTaskCount;
    }

    public void AddDecoCount(MergeBoardEnum boardId)
    {
        GetStorageBoard(boardId).DecoCount += 1;
    }

    public int GetDecoCount(MergeBoardEnum boardId)
    {
        return GetStorageBoard(boardId).DecoCount;
    }

    public void ClearMergeCount(int id,MergeBoardEnum boardId)
    {
        if (GetStorageBoard(boardId).MergeCounts.ContainsKey(id))
        {
            GetStorageBoard(boardId).MergeCounts.Remove(id);
        }
    }

    public void PauseAllCdTime(MergeBoardEnum boardId) // 暂停所有cd
    {
        for (int i = 0; i < GetStorageBoard(boardId).Items.Count; i++)
        {
            int leftBubbleTime = GetBubbleLeftCdTime(i,boardId);
            if (leftBubbleTime > 0)
                PauseCd(i,boardId);
        }
    }

    public void ResumAllCdTime(MergeBoardEnum boardId) // 恢复所有cd
    {
        for (int i = 0; i < GetStorageBoard(boardId).Items.Count; i++)
        {
            int leftBubbleTime = GetBubbleLeftCdTime(i,boardId);
            if (leftBubbleTime > 0)
                ResumeCd(i,boardId);
        }
    }

    public void PauseCd(int index,MergeBoardEnum boardId)
    {
        var item = GetBoardItem(index,boardId);
        if (item == null)
            return;
        var config = GameConfigManager.Instance.GetItemConfig(item.Id);
        if (config == null)
            return;
        if (item.IsPause)
            return;
        if (item.State == 1)
        {
            int leftCd = GetLeftActiveTime(index,boardId); // 剩余激活时间
            if (leftCd > 0)
            {
                item.PauseCDTime = leftCd;
            }
            else
            {
                bool isTimeProduct = MergeConfigManager.Instance.IsTimeProductItem(config);
                if (isTimeProduct)
                {
                    float pp = 0;
                    leftCd = GetTimeProductCd(index, ref pp,boardId);
                    if (leftCd > 0)
                        item.PauseCDTime = leftCd;
                }
                else
                {
                    float p = 0;
                    leftCd = GetLeftProductTime(index, ref p,boardId);
                    if (leftCd > 0)
                        item.PauseCDTime = leftCd;
                }
            }
        }
        else if (item.State == 2)
        {
            item.PauseCDTime = GetBubbleLeftCdTime(index,boardId);
        }

        item.IsPause = true;
    }

    public void ResumeCd(int index,MergeBoardEnum boardId)
    {
        var item = GetBoardItem(index,boardId);
        if (item == null)
            return;
        if (!item.IsPause)
            return;


        var itemConfig = GameConfigManager.Instance.GetItemConfig(item.Id);
        if (item.State == 1)
        {
            ActiveCostType type = GetActiveCostType(itemConfig);
            if (type == ActiveCostType.time_active || type == ActiveCostType.time_inactive)
            {
                int leftTime = GetLeftActiveTime(index,boardId); // 剩余激活时间
                if (leftTime > 0)
                {
                    int cdTime = itemConfig.active_cost[1] * 60;
                    if ((int) (APIManager.Instance.GetServerTime() / 1000) > (cdTime - leftTime))
                        item.ActiveTime = (APIManager.Instance.GetServerTime() / 1000) - (ulong) (cdTime - leftTime);
                    else
                        item.ActiveTime = 0;
                }
            }

            else
            {
                bool isTimeProduct = MergeConfigManager.Instance.IsTimeProductItem(itemConfig);
                if (isTimeProduct)
                {
                    float pp = 0;
                    int  leftCd = GetTimeProductCd(index, ref pp,boardId);
                    if (leftCd > 0)
                    {
                        int cdTime = GetTimeProductCdTime(itemConfig);
                        if ((int) (APIManager.Instance.GetServerTime() / 1000) > (cdTime - leftCd))
                            item.TimProductTime = (APIManager.Instance.GetServerTime() / 1000) - (ulong) (cdTime - leftCd);
                        else
                            item.TimProductTime = 0;
                    }
                       
                }
                else
                {
                    float p = 0;
                    int leftTime = GetLeftProductTime(index, ref p,boardId); // 剩余时间
                    if (leftTime > 0)
                    {
                        int cdTime = GetProductCdTime(itemConfig,item,boardId);
                        if ((int) (APIManager.Instance.GetServerTime() / 1000) > (cdTime - leftTime))
                            item.InCdTime = (APIManager.Instance.GetServerTime() / 1000) - (ulong) (cdTime - leftTime);
                        else
                            item.InCdTime = 0;
                    }
                }
               
            }
        }
        else if (item.State == 2)
        {
            int leftTime = GetBubbleLeftCdTime(index,boardId); // 剩余激活时间
            if ((int) (APIManager.Instance.GetServerTime() / 1000) > (BubbleCD - leftTime))
                item.OpenTime = APIManager.Instance.GetServerTime() / 1000 - (ulong) (BubbleCD - leftTime);
            else
                item.OpenTime = 0;
        }

        item.IsPause = false;
    }

    #endregion

    public bool IsCanSold(int id) 
    {
      
        var itemcConfig = GameConfigManager.Instance.GetItemConfig(id);
        if (itemcConfig != null && itemcConfig.sold_gold < 0)
            return false;
        if (itemcConfig != null && itemcConfig.type == (int) MergeItemType.easter)
        {
            if (EasterModel.Instance.IsOpened() && !EasterModel.Instance.IsMax())
                return false;
        }
        return true;
    }

    Dictionary<int, List<TableMergeItem>> productItems = new Dictionary<int, List<TableMergeItem>>();

    private void FindAllProductBoardItem(MergeBoardEnum boardId)
    {
        productItems.Clear();
        for (int i = 0; i < GetStorageBoard(boardId).Items.Count; i++)
        {
            var item = GetStorageBoard(boardId).Items[i];
            var itemConfig = GameConfigManager.Instance.GetItemConfig(item.Id);
            if (itemConfig == null)
                continue;
            if (MergeConfigManager.Instance.IsCanProductItem(item.Id) && item.State == 1)
            {
                if (!productItems.ContainsKey(itemConfig.in_line))
                    productItems[itemConfig.in_line] = new List<TableMergeItem>();
                productItems[itemConfig.in_line].Add(itemConfig);
            }
        }

        //排序
        foreach (var item in productItems)
        {
            item.Value.Sort((x, y) => y.level - x.level);
        }
    }

    #region 改动物品id对应 --- 暂定全部变成金币(最大等级)

    public void ReMapBoardOldId(MergeBoardEnum boardId) //重新映射棋盘删除物品
    {
        var maxCoinConfig = MergeConfigManager.Instance.GetLevelItem(6, -1);
        if (maxCoinConfig == null)
            return;
        int reMapId = maxCoinConfig.id; // 可以变换为其他id
        for (int i = 0; i < GetStorageBoard(boardId).Items.Count; i++)
        {
            var item = GetStorageBoard(boardId).Items[i];
            if (item.Id > 0)
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(item.Id);
                if (itemConfig == null) // 有之前老的存档记录 变为金币
                {
                    item.Id = reMapId;
                }
            }
        }
    }

    public void ReMapBagOldId(MergeBoardEnum boardId) //映射背包已删除的物品id
    {
        var maxCoinConfig = MergeConfigManager.Instance.GetLevelItem(6, -1);
        if (maxCoinConfig == null)
            return;
        int reMapId = maxCoinConfig.id; // 可以变换为其他id
        for (int i = 0; i < GetStorageBoard(boardId).Bags.Count; i++)
        {
            var item = GetStorageBoard(boardId).Bags[i];
            if (item.Id > 0)
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(item.Id);
                if (itemConfig == null) // 有之前老的存档记录 变为金币
                {
                    item.Id = reMapId;
                }
            }
        }
    }

    public void ReMapTempStorageBagOldId(MergeBoardEnum boardId) //映射临时仓库已删除的物品id
    {
        var maxCoinConfig = MergeConfigManager.Instance.GetLevelItem(6, -1);
        if (maxCoinConfig == null)
            return;
        int reMapId = maxCoinConfig.id; // 可以变换为其他id
        for (int i = 0; i < GetStorageBoard(boardId).Rewards.Count; i++)
        {
            var item = GetStorageBoard(boardId).Rewards[i];
            if (item.Id > 0)
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(item.Id);
                if (itemConfig == null) // 有之前老的存档记录 变为金币
                {
                    item.Id = reMapId;
                }
            }
        }
    }

    protected void RemoveDeleteStoreageId() //删除合成记录 和解锁物品
    {
        if(unlockItems == null)
            return;
        
        if(unlockItems.UnlockIds == null)
            return;
        
        if(GameConfigManager.Instance.MergeLineList == null)
            return;
        
        for (int i = 0; i < unlockItems.UnlockIds.Count; i++)
        {
            var config = GameConfigManager.Instance.GetItemConfig(unlockItems.UnlockIds[i]);
            if (config == null)
            {
                unlockItems.UnlockIds.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < unlockItems.UnlockLines.Count; i++)
        {
            var config = GameConfigManager.Instance.MergeLineList.Find(x => x.id == unlockItems.UnlockLines[i]);
            if (config == null)
            {
                unlockItems.UnlockLines.RemoveAt(i);
                i--;
            }
        }
    }

    public int GetBoardItemCountByType(MergeItemStatus status,MergeBoardEnum boardId)
    {
        var list = GetStorageBoard(boardId).Items.FindAll(x => x.State == (int) status);
        if (list == null)
            return 0;
        return list.Count;
    }

    #endregion 改动物品id对应

    // ---------------debug
    public void ClearMerBoard(MergeBoardEnum boardId)
    {
        var board=storageMergeBoardDict.GetValue((int)boardId);
        if (board!=null)
        {
            board.Clear();
        }
    }

    private static Dictionary<MergeBoardEnum,int> _leftEmptyGridCountDic = new Dictionary<MergeBoardEnum, int>();
    private static Dictionary<MergeBoardEnum,int> _leftBagEmptyGridCountDic = new Dictionary<MergeBoardEnum, int>();
    private static Dictionary<MergeBoardEnum,int> _bagGridCountDic = new Dictionary<MergeBoardEnum, int>();
    public static int LeftEmptyGridCount(MergeBoardEnum boardId)
    {
        if (IsDestroy)
            return _leftEmptyGridCountDic.TryGetValue(boardId,out var value)?value:-1;
        else
            return MergeManager.Instance.GetLeftEmptyGridCount(boardId);
    }
    public static int LeftBagEmptyGridCount(MergeBoardEnum boardId)
    {
        if (IsDestroy)
            return _leftBagEmptyGridCountDic.TryGetValue(boardId,out var value)?value:-1;
        else
            return MergeManager.Instance.GetLeftBagEmptyGridCount(boardId);
    }   
    public static int LeftBuildBagEmptyGridCount(MergeBoardEnum boardId)
    {
            return MergeManager.Instance.GetLeftBuildBagEmptyGridCount(boardId);
    }
    public static int BagGridCount(MergeBoardEnum boardId)
    {
        if (IsDestroy)
            return _bagGridCountDic.TryGetValue(boardId,out var value)?value:-1;
        else
            return MergeManager.Instance.GetBagGridCount(boardId);
    }    
    public static int BuildBagGridCount(MergeBoardEnum boardId)
    { 
        return MergeManager.Instance.GetBuildBagGridCount(boardId);
    }
    public static bool IsDestroy;
    private void OnDestroy()
    {
        IsDestroy = true;
        foreach (MergeBoardEnum value in (MergeBoardEnum[])Enum.GetValues(typeof(MergeBoardEnum)))
        {
            _leftEmptyGridCountDic[value] = MergeManager.Instance.GetLeftEmptyGridCount(value);
            _leftBagEmptyGridCountDic[value] = MergeManager.Instance.GetLeftBagEmptyGridCount(value);
            _bagGridCountDic[value] = MergeManager.Instance.GetBagGridCount(value);
        }
    }
    
    
    public Dictionary<int, int> GetCodeCountMap(bool includeBoard, bool includeCache, bool includeBag, bool ignoreOrder = false)
    {
        Dictionary<int, int> countMap = new Dictionary<int, int>();

        if (includeBoard)
        {
            for (int i = 0; i < storageBoard.Items.Count; i++)
            {
                var mergeItem = storageBoard.Items[i];
                if (!IsUnlock(mergeItem))
                    continue;
                
                if(MainOrderManager.Instance.IsWaitRemoving(i))
                    continue;
                
                if(!ignoreOrder && !OrderConfigManager.Instance._orderItems.ContainsKey(storageBoard.Items[i].Id))
                    continue;
                
                if(!countMap.ContainsKey(storageBoard.Items[i].Id))
                    countMap.Add(storageBoard.Items[i].Id, 0);

                countMap[storageBoard.Items[i].Id]++;
            }
        }

        if (includeCache)
        {
            foreach (var storageBoardReward in storageBoard.Rewards)
            {
                var a = storageBoardReward;
                if(!ignoreOrder && !OrderConfigManager.Instance._orderItems.ContainsKey(a.Id))
                   continue;
                
                if(!countMap.ContainsKey(a.Id))
                    countMap.Add(a.Id, 0);

                countMap[a.Id]++;
            }
        }

        if (includeBag)
        {
            foreach (var storageMergeItem in storageBoard.Bags)
            {
                var a = storageMergeItem;
                if (!ignoreOrder && !OrderConfigManager.Instance._orderItems.ContainsKey(a.Id))
                    continue;
                
                if(!countMap.ContainsKey(a.Id))
                    countMap.Add(a.Id, 0);

                countMap[a.Id]++;
            }
        }

        return countMap;
    }

    public Dictionary<int, List<int>> GetCodeLevelMap(bool isDescending, bool includeBoard, bool includeCache, bool includeBag, int filterId = -1)
    {
        Dictionary<int, List<int>> levelMap = new Dictionary<int, List<int>>();
        Dictionary<int, int> codeMap = GetCodeCountMap(includeBoard, includeCache, includeBag);
        foreach (var kv in codeMap)
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(kv.Key);
            if(itemConfig == null)
                continue;
                
            if(!OrderConfigManager.Instance._orderItems.ContainsKey(itemConfig.id))
                continue;

            if (filterId > 0)
            {
                if(itemConfig.in_line == filterId)
                    continue;
            }
            
            int level = itemConfig.level;
            if(!levelMap.ContainsKey(level))
                levelMap.Add(level, new List<int>());
                
            levelMap[level].Add(itemConfig.id);
        }
        
        if(isDescending)
            return levelMap.OrderByDescending(p => p.Key).ToDictionary(p => p.Key, o => o.Value);

        return levelMap.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);
    }
    
    public bool IsEatAllFood(StorageMergeItem storageMergeItem)
    {
        var itemConfig = GameConfigManager.Instance.GetItemConfig(storageMergeItem.Id);
        var produceCost = MergeManager.Instance.GetProduceCost(itemConfig);
        if (itemConfig != null && produceCost!=null)
        {
            foreach (var mergeId in produceCost)
            {
                if (!storageMergeItem.EatBuildingDic.ContainsKey(mergeId))
                    return false;
            }

            return true;
        }
        return false;
    }
    public void BuildEat(StorageMergeItem storageMergeItem,int mergeId)
    {
        if (storageMergeItem.EatBuildingDic.ContainsKey(mergeId))
            storageMergeItem.EatBuildingDic[mergeId]++;
        else
        {
            storageMergeItem.EatBuildingDic.Add(mergeId,1);
        }
    }

    public float GetEatProgress(StorageMergeItem storageMergeItem,TableMergeItem mergeItem)
    {
        float value = 0;
        var produceCost = MergeManager.Instance.GetProduceCost(mergeItem);
        if (produceCost == null || produceCost.Length <= 0)
            return value;
        
        value=1f*storageMergeItem.EatBuildingDic.Count/produceCost.Length;
        return value;
    }
    public bool IsBuildEat(StorageMergeItem storageMergeItem,int mergeId)
    {
        if (storageMergeItem == null)
            return false;
        return storageMergeItem.EatBuildingDic.ContainsKey(mergeId);
    }

    public void SendMergeBoardBI(MergeBoardEnum boardId)
    {
        var boardData = GetMergeBoardDataStruct(boardId);
        var jsonString = JsonConvert.SerializeObject(boardData);
        GameBIManager.Instance.SendGameEvent(
            BiEventCooking.Types.GameEventType.GameEventCheckerboard2,
            data1:boardData.EmptyBoardGridCount.ToString(),
            data2:boardData.EmptyBagGridCount.ToString(),
            data3:boardData.UnlockBagGridCount.ToString(),
            extras:new Dictionary<string, string>()
            {
                {"data1",jsonString},
                {"buildBagCount",boardData.UnlockBuildBagGridCount.ToString()},
                {"buildBagLeft",boardData.EmptyBuildBagGridCount.ToString()},
            });
    }

    public MergeBoardDataStruct GetMergeBoardDataStruct(MergeBoardEnum boardId)
    {
        var data = new MergeBoardDataStruct();
        data.EmptyBoardGridCount = MergeManager.LeftEmptyGridCount(boardId);
        data.EmptyBagGridCount = MergeManager.LeftBagEmptyGridCount(boardId);
        data.UnlockBagGridCount = MergeManager.BagGridCount(boardId);
        data.EmptyBuildBagGridCount = MergeManager.LeftBuildBagEmptyGridCount(boardId);
        data.UnlockBuildBagGridCount = MergeManager.BuildBagGridCount(boardId);
        data.BoardItems = new Dictionary<int, int>();
        data.BagItems = new Dictionary<int, int>();
        data.BuildBagItems = new Dictionary<int, int>();
        var storage = storageMergeBoardDict.GetValue((int)boardId);
        if (storage == null)
        {
            return data;
        }

        foreach (var item in storage.Items)
        {
            if (item.State > 0 && item.Id > 0)
            {
                if (!data.BoardItems.ContainsKey(item.Id))
                {
                    data.BoardItems.Add(item.Id,0);
                }
                data.BoardItems[item.Id]++;
            }
        }
        foreach (var item in storage.Bags)
        {
            if (item.State > 0 && item.Id > 0)
            {
                if (!data.BagItems.ContainsKey(item.Id))
                {
                    data.BagItems.Add(item.Id,0);
                }
                data.BagItems[item.Id]++;
            }
        }   
        foreach (var item in storage.BuildingBags)
        {
            if (item.State > 0 && item.Id > 0)
            {
                if (!data.BuildBagItems.ContainsKey(item.Id))
                {
                    data.BuildBagItems.Add(item.Id,0);
                }
                data.BuildBagItems[item.Id]++;
            }
        }
        return data;
    }
    public class MergeBoardDataStruct
    {
        public int EmptyBoardGridCount;
        public int EmptyBagGridCount;
        public int UnlockBagGridCount;
        public int EmptyBuildBagGridCount;
        public int UnlockBuildBagGridCount;
        public Dictionary<int, int> BoardItems;
        public Dictionary<int, int> BagItems;
        public Dictionary<int, int> BuildBagItems;
    }

    public void ResetBuilding(MergeBoardEnum boardId)
    {
        var board= GetStorageBoard(boardId);
        for (int i = 0; i < board.Items.Count; i++)
        {
            if (MergeConfigManager.Instance.IsEnergyProductItem(board.Items[i].Id))
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(board.Items[i].Id);
                bool isDeath = MergeConfigManager.Instance.IsLimitNoCdProductItem( itemConfig );
                if(isDeath)
                    continue;
                MergeManager.Instance.SetOrginalStoreCount( board.Items[i], false);
                MergeManager.Instance.ResetProductCD(i,boardId);
                MergeManager.Instance.ResumeBuildCD(board.Items[i]);
                MergeMainController.Instance.MergeBoard.GetGridByIndex(i).board.PlaySpeedUpAnimator(false, () => { });
                
            }
        }
  
                        
    }

    public void AddUnlimitedProductTime(long time,MergeBoardEnum boardId)
    {
        var board= GetStorageBoard(boardId);
        board.UnlimtProductEndTime = UnlimitedProductLeftTime(boardId) + time + (long)APIManager.Instance.GetServerTime();
        ResetBuilding(boardId);
    }
    public long UnlimitedProductLeftTime(MergeBoardEnum boardId)
    {
        var board= GetStorageBoard(boardId);

        long left = board.UnlimtProductEndTime - (long)APIManager.Instance.GetServerTime();
        return left > 0L ? (int) left : 0;
    }
    bool IsInUnlimitedProductState { get; set; }
    public bool IsInUnlimitedProduct(MergeBoardEnum boardId)
    {
        bool isUnlimited= UnlimitedProductLeftTime(boardId) > 0;
        if (isUnlimited != IsInUnlimitedProductState)
        {
            IsInUnlimitedProductState = isUnlimited;
            EventDispatcher.Instance.DispatchEvent(MergeEvent.MERGE_BORAD_REFRESH_UNLIMITEDPRODUCT);
        }
        return isUnlimited;
    }

    public string GetUnlimitedProductTimeStr(MergeBoardEnum boardId)
    {
        return CommonUtils.FormatLongToTimeStr(UnlimitedProductLeftTime(boardId));
    }

    public bool RemoveBoardItems(MergeBoardEnum boardId, List<int> items, bool isSendEvent = true, string sources = "")
    {
        bool isRemove = false;
        StorageMergeBoard board = GetStorageBoard(boardId);
        if (board == null)
            return false;
        
        for (int i = 0; i < board.Items.Count; i++)
        {
            int index = i;
            int id = GetStorageBoard(boardId).Items[i].Id;
            if (id <= 0)
                continue;
            
            if(!items.Contains(id))
                continue;

            if (MergeMainController.Instance != null && MergeMainController.Instance.MergeBoard != null)
            {
                MergeBoard.Grid grid =MergeMainController.Instance.MergeBoard.GetGridByIndex(i);
                grid.board.PlayRemoveEffect();  
    
                CommonUtils.DelayedCall(0.3f, () =>
                {
                    RemoveBoardItem(index, boardId, sources);
                });
            }
            else
            {
                RemoveBoardItem(index, boardId, sources);
            }

            isRemove = true;
                
            if (isSendEvent)
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
        }
        
        for (int i = GetStorageBoard(boardId).Rewards.Count-1; i >=0; i--)
        {
            int id = GetStorageBoard(boardId).Rewards[i].Id;
            if (id <= 0)
                continue;
            
            if(!items.Contains(id))
                continue;
            
            GetStorageBoard(boardId).Rewards.Remove(GetStorageBoard(boardId).Rewards[i]);
            if (isSendEvent)
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
            isRemove = true;
        }    
        
        for (int i = GetStorageBoard(boardId).Bags.Count-1; i >=0; i--)
        {
            int id = GetStorageBoard(boardId).Bags[i].Id;
            if (id <= 0)
                continue;
            
            if(!items.Contains(id))
                continue;
            
            GetStorageBoard(boardId).Bags.Remove(GetStorageBoard(boardId).Bags[i]);
            isRemove = true;
        }

        return isRemove;
    }

    public int[] GetProduceCost(TableMergeItem config)
    {
        if (config == null)
            return null;
        
        switch (config.subType)
        {
            case (int)SubType.Matreshkas:
            {
                return MatreshkasModel.Instance.GetProduceCost();
            }
        }

        return config.produce_cost;
    }
    
    public void AdaptBoard()
    {
        InitStorage();
        int BagCapacity = 0;
        if (!storageMergeBoardDict.ContainsKey((int)MergeBoardEnum.Main))
        {
            InitMergeBoard((int)MergeBoardEnum.Main);
            return;
        }

        storageMergeBoardDict.Remove((int)MergeBoardEnum.Main);
        InitMergeBoard((int)MergeBoardEnum.Main);

        Refresh(MergeBoardEnum.Main);
        
        MergeMainController.Instance.MergeBoard.RefreshInitGrids();
    }
}