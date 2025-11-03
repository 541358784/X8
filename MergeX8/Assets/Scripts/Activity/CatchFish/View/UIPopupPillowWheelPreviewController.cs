using DragonPlus;
using UnityEngine.UI;

public class UIPopupCatchFishPreviewController:UIWindowController
{
    public static UIPopupCatchFishPreviewController Instance;

    public static UIPopupCatchFishPreviewController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupCatchFishPreview) as UIPopupCatchFishPreviewController;
        return Instance;
    }

    public Button CloseBtn;
    public Button StartBtn;
    public LocalizeTextMeshProUGUI TimeText;
    
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/CloseButton");
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
        TimeText.SetText(CatchFishModel.Instance.Storage.GetPreheatLeftTimeText());
    }
}