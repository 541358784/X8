using System.Collections.Generic;
using DragonPlus;
using Gameplay;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIEasterHelpController : UIWindowController
{
    private Button _buttonClose;
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnBtnClose);
    }
    private void OnBtnClose()
    {
        AnimCloseWindow();
    }

}