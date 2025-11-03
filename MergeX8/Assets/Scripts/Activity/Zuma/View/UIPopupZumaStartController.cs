using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupZumaStartController:UIWindowController
{
    public static UIPopupZumaStartController Open(StorageZuma storageZuma)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupZumaStart, storageZuma) as
            UIPopupZumaStartController;
    }
    
    private StorageZuma Storage;
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
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ZumaStart);
        AnimCloseWindow(() => ZumaModel.CanShowMainPopup());
    }
    // public void OnClickHelpBtn()
    // {
    //     UIZumaHelpController.Open(Storage);
    // }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageZuma;
        // if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ZumaGuideStart))
        // {
        //     List<Transform> topLayer = new List<Transform>();
        //     topLayer.Add(PlayBtn.transform);
        //     GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ZumaGuideStart, PlayBtn.transform as RectTransform,
        //         topLayer: topLayer);
        //     if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ZumaGuideStart, null))
        //     {
        //     }
        // }
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ZumaStart))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(PlayBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ZumaStart, PlayBtn.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ZumaStart, null))
            {
                
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