using UnityEngine.UI;

public class UIFlowerFieldStartController:UIWindowController
{
    public static UIFlowerFieldStartController Instance;

    public static UIFlowerFieldStartController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIFlowerFieldStart) as UIFlowerFieldStartController;
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
                UIFlowerFieldMainController.Open();
            });
        });
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow(() =>
            {
                UIFlowerFieldMainController.Open();
            });
        });
    }
}