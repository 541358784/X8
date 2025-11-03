using System;
using System.Collections.Generic;
using Activity.LuckyGoldenEgg;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_LuckyGoldenEgg : Aux_ItemBase
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
        gameObject.SetActive(LuckyGoldenEggModel.Instance.IsOpen());
        if (!gameObject.activeSelf)
            return;
        if (LuckyGoldenEggModel.Instance.IsPreheating())
        {
            _timeText.SetText(LuckyGoldenEggModel.Instance.GetActivityPreheatLeftTimeString());
        }
        else
        {
            _timeText.SetText(LuckyGoldenEggModel.Instance.GetActivityLeftTimeString());
        }
        _redPoint.gameObject.SetActive(LuckyGoldenEggModel.Instance.GetGoldenEgg()>0 &&!LuckyGoldenEggModel.Instance.IsPreheating() );
        _redPointLabel.gameObject.SetActive(LuckyGoldenEggModel.Instance.GetGoldenEgg()>0);
        _redPointLabel.SetText(LuckyGoldenEggModel.Instance.GetGoldenEgg().ToString());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (LuckyGoldenEggModel.Instance.LuckyGoldenEgg.IsStart)
        {
            UIManager.Instance.OpenUI(UINameConst.UILuckyGoldenEggMain);

        }
        else
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupLuckyGoldenEggStart);
        }
    }
  
}
