using System;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay.UI;
using UnityEngine.UI;

public class NewDailyPackExtraView:UIPopupExtraView
{
    public override bool CanShow()
    {
        if (UIPopupNewDailyGiftController.CanShowUIWithOutOpenWindow())
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
            UIPopupNewDailyGiftController.CanShowUIWithOpenWindow();
            // UIPopupDailyGiftController.CanShowUIWithOpenWindow();
        });
    }
}