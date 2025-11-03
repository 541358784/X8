
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Storage;

public partial class MergeManager
{
    public  void InitHGMergeBoard(MergeBoardEnum boardId)
    {
        var mergeBoadrId = (int)boardId;

        if (!storageMergeBoardDict.ContainsKey(mergeBoadrId) || GetStorageBoard(boardId).Items.Count == 0)
        {
            DebugUtil.LogError("初始化HappyGo棋盘");
            StorageMergeBoard storage = new StorageMergeBoard();
            storage.Width = BOARD_WIDTH;
            storage.Height = BOARD_HEIGHT;
            storage.BagCapacity = GameConfigManager.Instance.BagList.FindAll(x => x.CointCost <= 0).Count;
            for (int i = 0; i < storage.Width * storage.Height; i++)
            {
                StorageMergeItem storageMergeItem = new StorageMergeItem();
                storageMergeItem.Id = -1;
                storage.Items.Add(storageMergeItem);
                if (i < HappyGoModel.Instance.HappyGoBoardGridList.Count)
                {
                    var config = HappyGoModel.Instance.HappyGoBoardGridList[i];
                    storageMergeItem.Id = config.itemId;
                    storageMergeItem.State = config.state;
                    storageMergeItem.UnlockState = config.unlockstate;
#if DEBUG || DEVELOPMENT_BUILD
                    if (config.itemId != -1 && GameConfigManager.Instance.GetItemConfig(config.itemId) == null)
                    {
                        DebugUtil.LogError("Invalid MergeItemId : " + config.itemId);
                    }
#endif
                    // SetOrginalStoreCount(storageMergeItem.Id, i);
                    var mergeItem = GameConfigManager.Instance.GetItemConfig(config.itemId);
                    if (mergeItem != null && mergeItem.type ==  (int)MergeItemType.hamaster)
                        storageMergeItem.State = 3;
                }
            }

            storageMergeBoardDict[mergeBoadrId] = storage;
            InitOrzginalStoreCount(storageMergeBoardDict[mergeBoadrId],boardId);
        }
    }
}
