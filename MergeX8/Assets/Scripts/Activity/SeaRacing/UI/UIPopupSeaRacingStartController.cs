using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupSeaRacingStartController:UIWindowController
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
        InvokeRepeating("UpdateTime", 0f, 1.0f);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SeaRacingStartUIBtn, StartBtn.transform as RectTransform, topLayer: new List<Transform>()
        {
            StartBtn.transform
        });
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }

    public void OnClickStartBtn()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSearacingEnter);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SeaRacingStartUIBtn, null);
        if (Storage != null)
        {
            AnimCloseWindow(() =>
            {
                SeaRacingModel.CanShowMainPopup();
            });
        }
    }

    public void UpdateTime()
    {
        if (Storage == null)
            return;
        if (Storage.GetLeftTime() <= 0)
        {
            TimeText.SetText(Storage.GetLeftTimeText());
            CancelInvoke("UpdateTime");
            AnimCloseWindow();
        }
        else
        {
            TimeText.SetText(Storage.GetLeftTimeText());   
        }
    }

    private StorageSeaRacingRound Storage;
    public void BindStorage(StorageSeaRacingRound storage)
    {
        if (storage != null)
        {
            Storage = storage;
            TitleText.SetTermFormats(storage.GetRoundString());
        }
        else
        {
            Debug.LogError("开始弹窗绑定round为null");
        }
    }

    public void CheckGuide()
    {
        if (!GuideSubSystem.Instance.IsShowingGuide() && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SeaRacingStartUIDes))
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SeaRacingStartUIDes, null);
            return;
        }
    }
    public static UIPopupSeaRacingStartController Open(StorageSeaRacingRound storage)
    {
        var popup = UIManager.Instance.OpenUI(UINameConst.UIPopupSeaRacingStart) as UIPopupSeaRacingStartController;
        storage.IsStart = true;
        var curTime = (long)APIManager.Instance.GetServerTime();
        storage.StartTime = curTime;
        foreach (var robot in storage.RobotList)
        {
            robot.UpdateScoreTime = curTime;
        }
        popup.BindStorage(storage);
        popup.CheckGuide();
        return popup;
    }
}