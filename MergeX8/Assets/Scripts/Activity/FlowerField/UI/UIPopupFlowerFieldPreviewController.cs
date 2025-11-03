using DragonPlus;
using UnityEngine.UI;

public class UIPopupFlowerFieldPreviewController:UIWindowController
{
    public static UIPopupFlowerFieldPreviewController Instance;

    public static UIPopupFlowerFieldPreviewController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupFlowerFieldPreview) as UIPopupFlowerFieldPreviewController;
        return Instance;
    }

    public Button CloseBtn;
    public Button StartBtn;
    public LocalizeTextMeshProUGUI TimeText;
    
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0,1);
    }

    public void UpdateTime()
    {
        TimeText.SetText(FlowerFieldModel.Instance.Storage.GetPreheatLeftTimeText());
    }
}