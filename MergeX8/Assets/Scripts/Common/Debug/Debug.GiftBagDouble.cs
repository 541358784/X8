using System.ComponentModel;
using DragonU3DSDK.Storage;

public partial class SROptions
{
    private const string GiftBagDouble = "俩礼包";
    [Category(GiftBagDouble)]
    [DisplayName("重置")]
    public void ResetGiftBagDouble()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().GiftBagDouble.Clear();
        if (GiftBagDoubleModel.Instance.IsInitFromServer())
        {
            GiftBagDoubleModel.Instance.InitStorage();
        }
    }
    [Category(GiftBagDouble)]
    [DisplayName("当前分组")]
    public int GiftBagDoubleCurGroup
    {
        get
        {
            if (GiftBagDoubleModel.Instance.Storage == null)
                return -1;
            return GiftBagDoubleModel.Instance.Storage.GroupId;
        }
    }
}