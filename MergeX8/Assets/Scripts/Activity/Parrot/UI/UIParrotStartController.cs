using UnityEngine.UI;

public class UIParrotStartController:UIWindowController
{
    public static UIParrotStartController Instance;

    public static UIParrotStartController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIParrotStart) as UIParrotStartController;
        return Instance;
    }
    private Button CloseBtn;
    private Button StartBtn;
    
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow(() =>
            {
                UIParrotMainController.Open();
            });
        });
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow(() =>
            {
                UIParrotMainController.Open();
            });
        });
    }
}