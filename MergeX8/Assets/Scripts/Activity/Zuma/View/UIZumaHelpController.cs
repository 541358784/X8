using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIZumaHelpController:UIWindowController
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

    public static UIZumaHelpController Open(StorageZuma storageZuma)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIZumaHelp, storageZuma) as
            UIZumaHelpController;
    }
}