using System;
using System.Collections.Generic;
using ASMR;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine.UI;

public partial class UIFishCultureMainController : UIWindowController
{
    public FishCultureModel Model => FishCultureModel.Instance;
    public StorageFishCulture Storage => Model.CurStorageFishCultureWeek;

    public override void PrivateAwake()
    {
    }

    public static UIFishCultureMainController Instance;

    public static UIFishCultureMainController Open()
    {
        if (!Instance)
        {
            Instance = UIManager.Instance.OpenUI(UINameConst.UIFishCultureMain) as UIFishCultureMainController;
        }

        return Instance;
    }

    private Button CloseBtn;
    private LocalizeTextMeshProUGUI TimeText;

    public void UpdateTime()
    {
        TimeText.SetText(Storage.GetLeftPreEndTimeText());
        if (Storage.GetLeftPreEndTime() <= 0 && !Storage.IsEnd)
        {
            AnimCloseWindow();
            CancelInvoke("UpdateTime");
        }
    }
    private LocalizeTextMeshProUGUI ScoreText;
    public void OnScoreChange(EventFishCultureScoreChange evt)
    {
        ScoreText.SetText(evt.NewValue.ToString());
    }
    public List<Action> DestroyActions = new List<Action>();
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        CloseBtn = transform.Find("Root/CloseButton").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            if (Storage.IsEnd)
            {
                UIPopupFishExitController.Open((b) =>
                {
                    if (b)
                        AnimCloseWindow();
                });
                // CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                // {
                //     DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_quit_game_tips_text"),
                //     OKCallback = () =>
                //     {
                //         AnimCloseWindow();
                //     },
                //     OKButtonText = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_ok"),
                //     CancelButtonText = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_cancel"),
                //     HasCancelButton = true,
                // });
            }
            else
            {
                AnimCloseWindow();   
            }
        });
        TimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateTime",0,1);
        ScoreText = transform.Find("Root/NumGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        ScoreText.SetText(Storage.CurScore.ToString());
        EventDispatcher.Instance.AddEvent<EventFishCultureScoreChange>(OnScoreChange);
        DestroyActions.Add(() =>
        {
            EventDispatcher.Instance.RemoveEvent<EventFishCultureScoreChange>(OnScoreChange);
        });
        
        InitShopEntrance();
        InitLeaderBoardEntrance();
        InitLevelGroup();
        InitFishNode();
        GuideFishShop();
    }

    private void OnDestroy()
    {
        foreach (var action in DestroyActions)
        {
            action?.Invoke();
        }
    }

    public List<Action<Action>> PerformList = new List<Action<Action>>();
    public bool isPlaying = false;
    public bool IsPlaying() => isPlaying;

    public void PushPerformAction(Action<Action> performAction)
    {
        PerformList.Add(performAction);
        if (!isPlaying)
        {
            isPlaying = true;
            XUtility.WaitFrames(1, PlayPerform);
        }
    }

    public void PlayPerform()
    {
        if (PerformList.Count > 0)
        {
            var performAction = PerformList[0];
            PerformList.RemoveAt(0);
            performAction(PlayPerform);
        }
        else
        {
            isPlaying = false;
        }
    }
}