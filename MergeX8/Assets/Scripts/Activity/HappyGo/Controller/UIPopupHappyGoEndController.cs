
using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupHappyGoEndController : UIWindowController
{
    private Button _closeBtn;
    private Button _playBtn;
    private Button _helpBtn;
    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _playBtn = GetItem<Button>("Root/PlayButton");
        _closeBtn.onClick.AddListener(OnClose);
        _playBtn.onClick.AddListener(OnPlayBtn);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventHgVdEnd,HappyGoModel.Instance.storageHappy.Exp.ToString());
    }
    
   
    private void OnPlayBtn()
    {
        AnimCloseWindow();
        HappyGoModel.Instance.CheckReissue(HappyGoModel.Instance.storageHappy);
    }
   
    private void OnClose()
    {
        AnimCloseWindow();
        HappyGoModel.Instance.CheckReissue(HappyGoModel.Instance.storageHappy);
    }
    

}
