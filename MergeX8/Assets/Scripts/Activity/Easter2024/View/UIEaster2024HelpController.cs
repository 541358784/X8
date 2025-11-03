using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIEaster2024HelpController:UIWindowController
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

    public static UIEaster2024HelpController Open(StorageEaster2024 storageEaster2024)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIEaster2024Help, storageEaster2024) as
            UIEaster2024HelpController;
    }
}