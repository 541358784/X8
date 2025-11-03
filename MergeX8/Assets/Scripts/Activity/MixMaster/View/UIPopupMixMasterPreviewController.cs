using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIPopupMixMasterPreviewController:UIWindowController
{
    public static UIPopupMixMasterPreviewController Open(StorageMixMaster storage)
    {
        var openWindow =
            UIManager.Instance.GetOpenedUIByPath<UIPopupMixMasterPreviewController>(UINameConst.UIPopupMixMasterPreview);
        if (openWindow)
        {
            openWindow.CloseWindowWithinUIMgr(true);
        }
        return UIManager.Instance.OpenUI(UINameConst.UIPopupMixMasterPreview, storage) as UIPopupMixMasterPreviewController;
    }
    public override void PrivateAwake()
    {
        ClsoeBtn = GetItem<Button>("Root/ButtonClose");
        ClsoeBtn.onClick.AddListener(() =>
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
    private StorageMixMaster Storage;
    private Button ClsoeBtn;
    private Button StartBtn;
    private LocalizeTextMeshProUGUI TimeText;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageMixMaster;
    }

    public void UpdateTime()
    {
        if (Storage.GetPreheatTime() <= 0)
        {
            AnimCloseWindow();
        }
        TimeText.SetText(Storage.GetPreheatTimeText());
    }
}