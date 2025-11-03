using UnityEngine.UI;

public class UIParrotHelpController:UIWindowController
{
    public static UIParrotHelpController Instance;

    public static UIParrotHelpController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIParrotHelp) as UIParrotHelpController;
        return Instance;
    }

    private Button CloseBtn;
    public override void PrivateAwake()
    {
        CloseBtn = transform.Find("Root/CloseButton").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
    }        
}