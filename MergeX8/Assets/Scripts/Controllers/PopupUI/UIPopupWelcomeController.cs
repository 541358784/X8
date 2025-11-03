using UnityEngine;
using UnityEngine.UI;
using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using System.Collections;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupWelcomeController : UIWindowController
{
    private Button ButtonPlayButton { get; set; }


    public override void PrivateAwake()
    {
        isPlayDefaultAudio = false;

        ButtonPlayButton = transform.Find("Root/PlayButton").GetComponent<Button>();
        ButtonPlayButton.onClick.AddListener(OnButtonPlayButtonClick);

        AudioManager.Instance.PlaySound(40);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        // GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFtue1);
    }

    private void OnButtonPlayButtonClick()
    {
        CloseWindowWithinUIMgr(true);
        // GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFtue2);

        CoroutineManager.Instance.StartCoroutine(BackHomeControl.GuideLogic());
    }


    protected override void OnCloseWindow(bool destroy = false)
    {
        // GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFtue3);
        // GuideSubSystem.Instance.Trigger(GuideTrigger.Deco1, null);
    }

    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        OnButtonPlayButtonClick();
    }
}