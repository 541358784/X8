using DragonU3DSDK;
using DragonU3DSDK.Storage;

public partial class MergeManager
{
    private const int D_BOARD_WIDTH = 4;
    private const int D_BOARD_HEIGHT = 5;
    
    public void InitDitchMergeBoard(MergeBoardEnum boardId)
    {
        var mergeBoardId = (int)boardId;
        if (!storageMergeBoardDict.ContainsKey(mergeBoardId) || GetStorageBoard(boardId).Items.Count == 0)
        {
            DebugUtil.LogError("初始化Ditch棋盘");
            StorageMergeBoard storageMergeBoard = new StorageMergeBoard();
            storageMergeBoard.Width = D_BOARD_WIDTH;
            storageMergeBoard.Height = D_BOARD_HEIGHT;
            storageMergeBoard.BagCapacity = 0;
            for (int i = 0; i < storageMergeBoard.Width * storageMergeBoard.Height; i++)
            {
                StorageMergeItem storageMergeItem = new StorageMergeItem();
                storageMergeItem.Id = -1;
                storageMergeItem.State = 1;
                storageMergeBoard.Items.Add(storageMergeItem);
            }

            storageMergeBoardDict[mergeBoardId] = storageMergeBoard;
            InitOrzginalStoreCount(storageMergeBoardDict[mergeBoardId],boardId);
        }
    }
}