using ActivityLocal.CardCollection.Home;
using UnityEngine.UI;

public class UICardHelpController:UIWindowController
{
    private Button CloseButton;
    public override void PrivateAwake()
    {
        CloseButton = GetItem<Button>("Root/CloseButton");
        CloseButton.onClick.AddListener(OnClickCloseButton);
    }

    public void OnClickCloseButton()
    {
        AnimCloseWindow();
    }
    public static void Open(CardCollectionCardThemeState themeState)
    {
        var uiPath = themeState.GetCardUIName(CardUIName.UIType.UICardHelp);
        if (!UIManager.Instance.GetOpenedUIByPath(uiPath))
            UIManager.Instance.OpenUI( uiPath);
    }
}