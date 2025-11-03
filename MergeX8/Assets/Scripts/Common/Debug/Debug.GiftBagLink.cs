using System.ComponentModel;
using DragonU3DSDK.Storage;

public partial class SROptions
{
    private const string GiftBagLink = "礼包链";
    [Category(GiftBagLink)]
    [DisplayName("显示礼包链")]
    public void ShowGiftLink()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UIGiftBagLink);
    }

    [Category(GiftBagLink)]
    [DisplayName("重置礼包链")]
    public void ResetGiftLink()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().GiftBagLink.Clear();
    }

    [Category(GiftBagLink)]
    [DisplayName("设置下标")]
    public int GiftBagLinkCurIndex
    {
        get
        {
            if (!GiftBagLinkModel.Instance.IsInitFromServer())
                return -1;
            return GiftBagLinkModel.Instance.GetCurIndex();
        }
        set
        {
            if (!GiftBagLinkModel.Instance.IsInitFromServer())
                return;
            GiftBagLinkModel.Instance.SetCurIndex(value);
        }
    }
    [Category(GiftBagLink)]
    [DisplayName("当前分组")]
    public int GiftBagLinkCurGroup
    {
        get
        {
            if (!GiftBagLinkModel.Instance.IsInitFromServer())
                return -1;
            return GiftBagLinkModel.Instance.GetCurActiveId();
        }
    }
}