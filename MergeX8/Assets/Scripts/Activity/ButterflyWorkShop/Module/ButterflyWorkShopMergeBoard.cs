using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;

public class ButterflyWorkShopMergeBoard:MergeBoard
{
    public override void InitBoardId()
    {
        SetBoardID((int) MergeBoardEnum.ButterflyWorkShop);
    }
    // public override TableMergeItem MergeToProductActivity(int index,TableMergeItem newConfig,TableMergeItem oldConfig)
    // {
    //     var rewardList = SummerWatermelonModel.Instance.GetRewardsOnMerge(newConfig);//合成产出奖励
    //     if (rewardList == null || rewardList.Count == 0)
    //         return null;
    //     for (var i = 0; i < rewardList.Count; i++)
    //     {
    //         var rewardId = rewardList[i];
    //         int newPos = MergeManager.Instance.FindEmptyGrid(index,(MergeBoardEnum)_boardID);
    //         // var itemConfig = GameConfigManager.Instance.GetItemConfig(rewardId);
    //         if (newPos == -1)//棋盘没位置就存入暂存奖励中
    //         {
    //             SummerWatermelonModel.Instance.UnSetRewards.Add(rewardId);
    //             SummerWatermelonModel.Instance.UnSetItemsCount++;
    //             SummerWatermelonModel.MainView?.RefreshBtnView();
    //         }
    //         else
    //         {
    //             // ProductOneItem(index, newPos, rewardId, false, RefreshItemSource.product);
    //             MergeManager.Instance.SetNewBoardItem(newPos, rewardId, 1, RefreshItemSource.product,(MergeBoardEnum)_boardID, index);
    //         }
    //         GameBIManager.Instance.SendGameEvent(
    //             BiEventAdventureIslandMerge.Types.GameEventType.GameEventSummerGetitem,data1:rewardId.ToString(),data2:"1");
    //
    //         if (!UserData.Instance.IsResource(rewardId))
    //         {
    //             GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
    //             {
    //                 MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonSummerGet,
    //                 isChange = false,
    //                 itemAId = rewardId
    //             });
    //         }
    //     }
    //     return null;
    // }

    public override void OnNewItem(int index, int oldIndex, int id, int oldId, RefreshItemSource source)
    {
        base.OnNewItem(index, oldIndex, id, oldId, source);
        ButterflyWorkShopModel.Instance.OnNewItem(index,id,source);
    }

    public override void OnUseItem(int index, int id)
    {
        base.OnUseItem(index, id);
        ButterflyWorkShopModel.Instance.OnUseItem(index,id);
    }
}