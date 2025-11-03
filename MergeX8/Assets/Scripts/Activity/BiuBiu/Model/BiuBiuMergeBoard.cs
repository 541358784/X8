using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;

public class BiuBiuMergeBoard:MergeBoard
{
    public override void InitBoardId()
    {
        SetBoardID((int) MergeBoardEnum.BiuBiu);
    }

    public override void OnUseItem(int index, int id)
    {
        base.OnUseItem(index, id);
        BiuBiuModel.Instance.OnUseItem(index,id);
    }
    public override void OnNewItem(int index, int oldIndex, int id, int oldId, RefreshItemSource source)
    {
        base.OnNewItem(index, oldIndex, id, oldId, source);
        BiuBiuModel.Instance.OnNewItem(index,id,source);
    }
}