using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIPopupSnakeLadderPreviewController:UIWindowController
{
    private StorageSnakeLadder Storage;
    private LocalizeTextMeshProUGUI TimeText;
    private Button CloseBtn;
    private Button PlayBtn;
    private Button HelpBtn;
    public override void PrivateAwake()
    {
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0f,1f);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        PlayBtn = GetItem<Button>("Root/Button");
        PlayBtn.onClick.AddListener(OnClickPlayBtn);
        HelpBtn = GetItem<Button>("Root/ButtonHelp");
        HelpBtn.onClick.AddListener(OnClickHelpBtn);
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }
    public void OnClickPlayBtn()
    {
        AnimCloseWindow();
    }
    public void OnClickHelpBtn()
    {
        UISnakeLadderHelpController.Open(Storage);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageSnakeLadder;
        
    }

    public void UpdateTime()
    {
        if (Storage == null)
            return;
        TimeText.SetText(Storage.GetPreheatLeftTimeText());
    }
    public static UIPopupSnakeLadderPreviewController Open(StorageSnakeLadder storageSnakeLadder)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupSnakeLadderPreview, storageSnakeLadder) as
            UIPopupSnakeLadderPreviewController;
    }
}