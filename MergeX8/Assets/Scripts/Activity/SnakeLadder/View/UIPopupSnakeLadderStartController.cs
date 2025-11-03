using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupSnakeLadderStartController:UIWindowController
{
    public static UIPopupSnakeLadderStartController Open(StorageSnakeLadder storageSnakeLadder)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupSnakeLadderStart, storageSnakeLadder) as
            UIPopupSnakeLadderStartController;
    }
    
    private StorageSnakeLadder Storage;
    // private LocalizeTextMeshProUGUI TimeText;
    private Button CloseBtn;
    private Button PlayBtn;
    // private Button HelpBtn;
    public override void PrivateAwake()
    {
        // TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        // InvokeRepeating("UpdateTime",0f,1f);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        PlayBtn = GetItem<Button>("Root/Button");
        PlayBtn.onClick.AddListener(OnClickPlayBtn);
        // HelpBtn = GetItem<Button>("Root/ButtonHelp");
        // HelpBtn.onClick.AddListener(OnClickHelpBtn);
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }
    public void OnClickPlayBtn()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SnakeLadderGuideStart);
        AnimCloseWindow(() => SnakeLadderModel.CanShowMainPopup());
    }
    // public void OnClickHelpBtn()
    // {
    //     UISnakeLadderHelpController.Open(Storage);
    // }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageSnakeLadder;
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SnakeLadderGuideStart))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(PlayBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SnakeLadderGuideStart, PlayBtn.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SnakeLadderGuideStart, null))
            {
                // SnakeLadderModel.Instance.AddTurntable(1,"GuideSend");
            }
        }
    }

    // public void UpdateTime()
    // {
    //     if (Storage == null)
    //         return;
    //     TimeText.SetText(Storage.GetLeftTimeText());
    // }
}