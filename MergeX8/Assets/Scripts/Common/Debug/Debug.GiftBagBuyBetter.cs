using System.ComponentModel;
using DragonU3DSDK.Storage;

public partial class SROptions
{
    private const string GiftBagBuyBetter = "越买越划算礼包";
    [Category(GiftBagBuyBetter)]
    [DisplayName("显示礼包链")]
    public void ShowGiftBagBuyBetter()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UIPopupGiftBagBuyBetter);
    }

    [Category(GiftBagBuyBetter)]
    [DisplayName("重置礼包链")]
    public void ResetGiftBagBuyBetter()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().GiftBagBuyBetter.Clear();
    }

    [Category(GiftBagBuyBetter)]
    [DisplayName("设置下标")]
    public int GiftBagBuyBetterCurIndex
    {
        get
        {
            if (!GiftBagBuyBetterModel.Instance.IsInitFromServer())
                return -1;
            return GiftBagBuyBetterModel.Instance.GetCurIndex();
        }
        set
        {
            if (!GiftBagBuyBetterModel.Instance.IsInitFromServer())
                return;
            GiftBagBuyBetterModel.Instance.SetCurIndex(value);
        }
    }
    [Category(GiftBagBuyBetter)]
    [DisplayName("当前分组")]
    public int GiftBagBuyBetterCurGroup
    {
        get
        {
            if (!GiftBagBuyBetterModel.Instance.IsInitFromServer())
                return -1;
            return GiftBagBuyBetterModel.Instance.GetCurActiveId();
        }
    }
}