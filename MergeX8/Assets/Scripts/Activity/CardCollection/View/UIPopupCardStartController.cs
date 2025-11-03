using System.Collections.Generic;
using ActivityLocal.CardCollection.Home;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupCardStartController : UIWindowController
{
    private Button CloseBtn;
    private Button StartBtn;
    private LocalizeTextMeshProUGUI TimeText;
    private LocalizeTextMeshProUGUI TitleText;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(OnClickStartBtn);
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        TitleText = GetItem<LocalizeTextMeshProUGUI>("Root/BGGroup/TextTitle");
        InvokeRepeating("UpdateTimeText",0f,1f);
    }

    public void UpdateTimeText()
    {
        TimeText.SetText(CurStorage.GetLeftTimeText());
    }
    private StorageCardCollectionActivity CurStorage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        CurStorage = objs[0] as StorageCardCollectionActivity;
        TitleText.SetTerm(CardCollectionModel.Instance.GetCardThemeState(CurStorage.ThemeId).NameKey);
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.CardLobbyGuideEntrance))
        {
            var topLayer = new List<Transform>();
            topLayer.Add(StartBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CardLobbyGuideEntrance,StartBtn.transform as RectTransform,topLayer:topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CardLobbyGuideEntrance, null))
            {
                // GuideSubSystem.Instance.InGuideChain = true;
                // CardCollectionModel.Instance.InGuideChain = true;
                CardCollectionModel.Instance.AddCardPackage(999999, 1, "Guide");
            }
        }
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }

    public void OnClickStartBtn()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CardLobbyGuideEntrance);
        AnimCloseWindow(() =>
        {
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home ||
                SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome)
            {
                SceneFsm.mInstance.TransitionGame();
            }
        });
    }

    public static UIPopupCardStartController Open(StorageCardCollectionActivity storage)
    {
        var theme = CardCollectionModel.Instance.GetCardThemeState(storage.ThemeId);
        var uiPath = theme.GetCardUIName(CardUIName.UIType.UIPopupCardStart);
        if (!UIManager.Instance.GetOpenedUIByPath(uiPath))
            return UIManager.Instance.OpenUI(uiPath,storage) as UIPopupCardStartController;
        return UIManager.Instance.GetOpenedUIByPath(uiPath) as UIPopupCardStartController;
    }
}