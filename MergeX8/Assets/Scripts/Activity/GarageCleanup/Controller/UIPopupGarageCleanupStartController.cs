using System.Collections.Generic;
using DragonPlus;
using Gameplay;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupGarageCleanupStartController : UIWindowController
{
    private LocalizeTextMeshProUGUI _timeText;
    private Button _buttonClose;
    private Button _buttonShow;
    private static string coolTimeKey = "GarageCleanupStart";
    public override void PrivateAwake()
    {
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnBtnClose);
        _buttonShow = GetItem<Button>("Root/Button");
        _buttonShow.onClick.AddListener(OnBtnShow);
        InvokeRepeating("UpdateTimeText",1,1);
        UpdateTimeText();
        
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventGarageCleanParticipate);
    }

    private void OnBtnShow()
    {
        GarageCleanupModel.Instance.StartActivity();
        AnimCloseWindow(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIGarageCleanupMain);
        });
}

    public override void ClickUIMask()
    {
        base.ClickUIMask();
        OnBtnClose();
    }

    private void OnBtnClose()
    {
        GarageCleanupModel.Instance.StartActivity();
        AnimCloseWindow(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIGarageCleanupMain);
        });
    }

    private void UpdateTimeText()
    {
        _timeText.SetText(GarageCleanupModel.Instance.GetActivityLeftTimeString());
    }
    public static bool CanShowUI()
    {

        if (!GarageCleanupModel.Instance.CanShowUI())
            return false;

        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            return false;

        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,
            CommonUtils.GetTimeStamp());

        UIManager.Instance.OpenUI(UINameConst.UIPopupGarageCleanupStart);
        return true;
    }
}