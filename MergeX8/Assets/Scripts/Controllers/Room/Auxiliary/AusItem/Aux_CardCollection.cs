using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
using Pack = BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities.Pack;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_CardCollection : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    public static Aux_CardCollection Instance;
    private CardCollectionEntranceRedPoint _redPoint;
    protected override void Awake()
    {
        if (!Instance)
            Instance = this;
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint").gameObject.AddComponent<CardCollectionEntranceRedPoint>();
        InvokeRepeating("UpdateUI", 0, 1);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(CardCollectionModel.Instance.ShowAuxItem());
        if (!gameObject.activeSelf)
            return;
        _timeText.SetText(CardCollectionActivityModel.Instance.GetActivityLeftTimeString());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        CardCollectionModel.Instance.OpenMainPopup();
    }
    private void OnDestroy()
    {
    }
}
