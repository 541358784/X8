using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupEaster2024StartController:UIWindowController
{
    public static UIPopupEaster2024StartController Open(StorageEaster2024 storageEaster2024)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupEaster2024Start, storageEaster2024) as
            UIPopupEaster2024StartController;
    }
    
    private StorageEaster2024 Storage;
    // private LocalizeTextMeshProUGUI TimeText;
    private Button CloseBtn;
    private Button PlayBtn;
    private Button HelpBtn;
    public override void PrivateAwake()
    {
        // TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        // InvokeRepeating("UpdateTime",0f,1f);
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
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Easter2024GuideStart);
        AnimCloseWindow(() => Easter2024Model.CanShowMainPopup());
    }
    public void OnClickHelpBtn()
    {
        UIEaster2024HelpController.Open(Storage);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageEaster2024;
        // if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.Easter2024GuideStart))
        // {
        //     List<Transform> topLayer = new List<Transform>();
        //     topLayer.Add(PlayBtn.transform);
        //     GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Easter2024GuideStart, PlayBtn.transform as RectTransform,
        //         topLayer: topLayer);
        //     if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Easter2024GuideStart, null))
        //     {
        //         Easter2024Model.Instance.AddEgg(5,"GuideSendBall");
        //     }
        // }
    }

    // public void UpdateTime()
    // {
    //     if (Storage == null)
    //         return;
    //     TimeText.SetText(Storage.GetLeftTimeText());
    // }
}