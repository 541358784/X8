using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay.UI.EnergyTorrent;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupEnergyTorrentStartController : UIWindowController
{
    private Button _buttonNow;
    private Button _buttonClose;
    public override void PrivateAwake()
    {
        _buttonNow = GetItem<Button>("Root/ButtonNew");
        _buttonClose = GetItem<Button>("Root/CloseButton");
        _buttonNow.onClick.AddListener(OnBtnNow);
        _buttonClose.onClick.AddListener(OnBtnNow);
        
        var topLayer = new List<Transform>();
        topLayer.Add(transform);
        
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnergyFrenzyPop);

    }

    public override void ClickUIMask()
    {
        OnBtnNow();
    }

    private void OnBtnNow()
    {
        AnimCloseWindow(() =>
        {
            //UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyTorrentMain);
            EnergyTorrentModel.Instance.RecordShowStart();
            
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.EnergyTorrent1, null);
        });
    }
}