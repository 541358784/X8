using UnityEngine.UI;

public class UITurntableHelpController: UIWindowController
{
    public override void PrivateAwake()
    {
        var closeButton = transform.Find("Root/CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(()=>
        {
            AnimCloseWindow();
        });
    }
}