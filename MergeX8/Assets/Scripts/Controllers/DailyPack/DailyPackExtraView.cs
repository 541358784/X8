using System;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay.UI;
using UnityEngine.UI;

public class DailyPackExtraView:UIPopupExtraView
{
    public override bool CanShow()
    {
        if (UIDailyPack2Controller.CanShowUIWithOutOpenWindow())
            return true;
        return false;
    }

    private bool _isInit = false;
    private Button _buyButton;
    public override void Init()
    {
        if (_isInit)
            return;
        _isInit = true;
        _buyButton = transform.Find("Button").GetComponent<Button>();
        _buyButton.onClick.AddListener(() =>
        {
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpclickLifeUseup,data1:"0");
            UIDailyPack2Controller.CanShowUIWithOpenWindow();
        });
    }
}