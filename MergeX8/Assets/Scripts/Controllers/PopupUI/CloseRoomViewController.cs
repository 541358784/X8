using UnityEngine;
using UnityEngine.UI;
using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using System.Collections;
using DragonU3DSDK.Network.API.Protocol;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class CloseRoomViewController : UIWindowController
{
    private Button ButtonClose { get; set; }

    public Action onCloseAction = null;

    public override void PrivateAwake()
    {
        ButtonClose = transform.Find("ButtonClose").GetComponent<Button>();
        ButtonClose.onClick.AddListener(OnButtonCloseClick);
        isPlayDefaultAudio = false;
    }

    protected override void OnOpenWindow(params object[] objs)
    {
    }

    private void OnButtonCloseClick()
    {
        AudioManager.Instance.StopAllSound();
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        CloseWindowWithinUIMgr(true);

        if (onCloseAction != null)
            onCloseAction();
    }
}