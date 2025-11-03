using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIThemeDecorationHelpController:UIWindowController
{
    private Button CloseBtn;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/Button");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }

    public static UIThemeDecorationHelpController Open(StorageThemeDecoration storageThemeDecoration)
    {
        return UIManager.Instance.OpenUI(storageThemeDecoration.GetAssetPathWithSkinName(UINameConst.UIThemeDecorationHelp), storageThemeDecoration) as
            UIThemeDecorationHelpController;
    }
}