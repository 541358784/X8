using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIGiftBagProgressHelpController:UIWindowController
{
    private Button CloseBtn;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/CloseButton");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }

    public static UIGiftBagProgressHelpController Open(StorageGiftBagProgress storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIGiftBagProgressHelp, storage) as
            UIGiftBagProgressHelpController;
    }
}