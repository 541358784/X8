using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIPillowWheelHelpController:UIWindowController
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

    public static UIPillowWheelHelpController Open(StoragePillowWheel storagePillowWheel)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPillowWheelHelp, storagePillowWheel) as
            UIPillowWheelHelpController;
    }
}