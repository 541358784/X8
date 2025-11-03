using System;
using System.Collections.Generic;
using Activity.TreasureHuntModel;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_TreasureHunt : Aux_ItemBase
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
        InvokeRepeating("UpdateUI", 0, 1);
    }

    public override void UpdateUI()
    {
        if (!this || !gameObject)
            return;
        gameObject.SetActive(TreasureHuntModel.Instance.IsOpen());
        if (!gameObject.activeSelf)
            return;
        if (TreasureHuntModel.Instance.IsPreheating())
        {
            _timeText.SetText(TreasureHuntModel.Instance.GetActivityPreheatLeftTimeString());
        }
        else
        {
            _timeText.SetText(TreasureHuntModel.Instance.GetActivityLeftTimeString());
        }
        _redPoint.gameObject.SetActive(TreasureHuntModel.Instance.GetHammer()>0 &&!TreasureHuntModel.Instance.IsPreheating() );
        _redPointLabel.gameObject.SetActive(TreasureHuntModel.Instance.GetHammer()>0);
        _redPointLabel.SetText(TreasureHuntModel.Instance.GetHammer().ToString());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (TreasureHuntModel.Instance.TreasureHunt.IsStart)
        {
            UIManager.Instance.OpenUI(UINameConst.UITreasureHuntMain);

        }
        else
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupTreasureHuntStart);
        }
    }
  
}
