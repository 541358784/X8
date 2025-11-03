using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupThemeDecorationStartController:UIWindowController
{
    public static UIPopupThemeDecorationStartController Open(StorageThemeDecoration storageThemeDecoration)
    {
        return UIManager.Instance.OpenUI(storageThemeDecoration.GetAssetPathWithSkinName(UINameConst.UIPopupThemeDecorationStart), storageThemeDecoration) as
            UIPopupThemeDecorationStartController;
    }
    
    private StorageThemeDecoration Storage;
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
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ThemeDecorationStart);
        AnimCloseWindow(() => ThemeDecorationModel.CanShowShopUI());
    }
    // public void OnClickHelpBtn()
    // {
    //     UIThemeDecorationHelpController.Open(Storage);
    // }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageThemeDecoration;
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ThemeDecorationStart))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(PlayBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ThemeDecorationStart, PlayBtn.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ThemeDecorationStart, null))
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