using System;
using System.Collections;
using System.Collections.Generic;
using Activity.Turntable.Model;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
using Pack = BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities.Pack;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_Turntable : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    public static Aux_Turntable Instance;
    private GameObject _redPoint;
    protected override void Awake()
    {
        if (!Instance)
            Instance = this;
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint").gameObject;
        InvokeRepeating("UpdateUI", 0, 1);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(TurntableModel.Instance.IsOpened());
        _redPoint.gameObject.SetActive(TurntableModel.Instance.GetCoin() > 0);
        
        if (!gameObject.activeSelf)
            return;
        _timeText.SetText(TurntableModel.Instance.GetActivityLeftTimeString());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UIManager.Instance.OpenUI(UINameConst.UIPopupTurntableMain);
    }
    private void OnDestroy()
    {
    }
}
