using UnityEngine.UI;

public class UIFlowerFieldHelpController:UIWindowController
{
    public static UIFlowerFieldHelpController Instance;

    public static UIFlowerFieldHelpController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIFlowerFieldHelp) as UIFlowerFieldHelpController;
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