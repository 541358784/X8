
using UnityEngine.UI;

public class MermaidHelpController : UIWindowController
{
    private Button _button;
    public override void PrivateAwake()
    {
        _button = GetItem<Button>("Root/Button");
        _button.onClick.AddListener(OnBtnClose);
    }

    private void OnBtnClose()
    {
        AnimCloseWindow();
    }
}
