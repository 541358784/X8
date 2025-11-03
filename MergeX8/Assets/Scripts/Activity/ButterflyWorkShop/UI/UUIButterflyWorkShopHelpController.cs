using UnityEngine.UI;

public class UIButterflyWorkShopHelpController:UIWindowController
{
    private Button _playBtn;
    private Button _closeBtn;
    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _closeBtn.onClick.AddListener(OnCloseBtn);
    }

    public void OnCloseBtn()
    {
        AnimCloseWindow(() =>
        {
        });
    }
}