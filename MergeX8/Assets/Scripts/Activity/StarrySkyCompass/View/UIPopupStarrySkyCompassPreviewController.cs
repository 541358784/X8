using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIPopupStarrySkyCompassPreviewController:UIWindowController
{
    private StorageStarrySkyCompass Storage;
    private LocalizeTextMeshProUGUI TimeText;
    private Button CloseBtn;
    private Button PlayBtn;
    public override void PrivateAwake()
    {
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0f,1f);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        PlayBtn = GetItem<Button>("Root/Button");
        PlayBtn.onClick.AddListener(OnClickPlayBtn);
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }
    public void OnClickPlayBtn()
    {
        AnimCloseWindow();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageStarrySkyCompass;
        
    }

    public void UpdateTime()
    {
        if (Storage == null)
            return;
        TimeText.SetText(Storage.GetPreheatTimeText());
        if (Storage.GetPreheatTime() == 0)
            AnimCloseWindow();
    }
    public static UIPopupStarrySkyCompassPreviewController Open(StorageStarrySkyCompass storageStarrySkyCompass)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupStarrySkyCompassPreview, storageStarrySkyCompass) as
            UIPopupStarrySkyCompassPreviewController;
    }
}