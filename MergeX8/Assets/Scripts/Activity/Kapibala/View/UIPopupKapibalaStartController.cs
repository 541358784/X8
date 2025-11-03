
using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIPopupKapibalaStartController : UIWindowController
{
    private Button CloseBtn;
    private Button StartBtn;
    // private LocalizeTextMeshProUGUI TimeText;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(OnClickStartBtn);
        // TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        // InvokeRepeating("UpdateTimeText",0f,1f);
    }

    private bool AutoClose = false;
    // public void UpdateTimeText()
    // {
    //     TimeText.SetText(CurStorage.GetLeftTimeText());
    //     if (CurStorage.GetLeftTime() <= 0 && !AutoClose)
    //     {
    //         AutoClose = true;
    //         AnimCloseWindow();
    //     }
    // }
    private StorageKapibala CurStorage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        CurStorage = objs[0] as StorageKapibala;
        // if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.CardLobbyGuideEntrance))
        // {
        //     var topLayer = new List<Transform>();
        //     topLayer.Add(StartBtn.transform);
        //     GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CardLobbyGuideEntrance,StartBtn.transform as RectTransform,topLayer:topLayer);
        //     if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CardLobbyGuideEntrance, null))
        //     {
        //         // GuideSubSystem.Instance.InGuideChain = true;
        //         // CardCollectionModel.Instance.InGuideChain = true;
        //         CardCollectionModel.Instance.AddCardPackage(999999, 1, "Guide");
        //     }
        // }
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }

    public void OnClickStartBtn()
    {
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CardLobbyGuideEntrance);
        AnimCloseWindow(() =>
        {
            UIKapibalaMainController.Open(KapibalaModel.Instance.Storage);
        });
    }

    public static UIPopupKapibalaStartController Open(StorageKapibala storage)
    {
        var uiPath = UINameConst.UIPopupKapibalaStart;
        if (!UIManager.Instance.GetOpenedUIByPath(uiPath))
            return UIManager.Instance.OpenUI(uiPath,storage) as UIPopupKapibalaStartController;
        return UIManager.Instance.GetOpenedUIByPath(uiPath) as UIPopupKapibalaStartController;
    }
}