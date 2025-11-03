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

public class Aux_SummerWatermelon : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    // private LocalizeTextMeshProUGUI _offText;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        _redPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint.gameObject.SetActive(false);
        
        InvokeRepeating("UpdateUI", 0, 1);
        InvokeRepeating("UpdateTime", 0, 1);
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SummerWatermelonStart, transform as RectTransform, topLayer: topLayer);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(SummerWatermelonModel.Instance.IsStart);
        
        if (!gameObject.activeSelf)
            return;
        
        _timeText.SetText(SummerWatermelonModel.Instance.GetActivityLeftTimeString());
        _redPoint.gameObject.SetActive(SummerWatermelonModel.Instance.UnSetItemsCount > 0);
        _redPointText.gameObject.SetActive(SummerWatermelonModel.Instance.UnSetItemsCount > 1);
        _redPointText.SetText(SummerWatermelonModel.Instance.UnSetItemsCount.ToString());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (!SummerWatermelonModel.Instance.IsStart)
            return;
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SummerWatermelonStart, null);
        SummerWatermelonModel.Instance.OpenMainPopup();
    }
    public void UpdateTime()
    {
        SummerWatermelonModel.Instance.UpdateTime();
    }
    private void OnDestroy()
    {
    }
}
