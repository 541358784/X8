using System;
using System.Collections.Generic;
using Activity.TreasureHuntModel;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_NoAdsGiftBag : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointLabel;
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        _redPointLabel = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI", 0, 1);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(NoAdsGiftBagModel.Instance.IsActive());
        if (!gameObject.activeSelf)
            return;
        _timeText.SetText(NoAdsGiftBagModel.Instance.LeftTimeString());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UIPopupNoADSController.Open();
    }
  
}
