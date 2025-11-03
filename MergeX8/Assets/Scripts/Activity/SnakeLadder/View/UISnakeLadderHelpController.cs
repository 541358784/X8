using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UISnakeLadderHelpController:UIWindowController
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

    public static UISnakeLadderHelpController Open(StorageSnakeLadder storageSnakeLadder)
    {
        return UIManager.Instance.OpenUI(UINameConst.UISnakeLadderHelp, storageSnakeLadder) as
            UISnakeLadderHelpController;
    }
}