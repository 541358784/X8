using DragonPlus;
using UnityEngine.UI;

public class UIPopupParrotPreviewController:UIWindowController
{
    public static UIPopupParrotPreviewController Instance;

    public static UIPopupParrotPreviewController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupParrotPreview) as UIPopupParrotPreviewController;
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
        TimeText.SetText(ParrotModel.Instance.Storage.GetPreheatLeftTimeText());
    }
}