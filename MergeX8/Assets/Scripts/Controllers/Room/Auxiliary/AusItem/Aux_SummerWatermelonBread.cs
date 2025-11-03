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

public class Aux_SummerWatermelonBread : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    // private LocalizeTextMeshProUGUI _offText;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    public static Aux_SummerWatermelonBread Instance;
    protected override void Awake()
    {
        if (!Instance)
            Instance = this;
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        _redPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint.gameObject.SetActive(false);
        
        InvokeRepeating("UpdateUI", 0, 1);
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SummerWatermelonBreadStart, transform as RectTransform, topLayer: topLayer);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(SummerWatermelonBreadModel.Instance.IsStart);
        
        if (!gameObject.activeSelf)
            return;
        
        _timeText.SetText(SummerWatermelonBreadModel.Instance.GetActivityLeftTimeString());
        _redPoint.gameObject.SetActive(SummerWatermelonBreadModel.Instance.UnSetItemsCount > 0);
        _redPointText.gameObject.SetActive(SummerWatermelonBreadModel.Instance.UnSetItemsCount > 1);
        _redPointText.SetText(SummerWatermelonBreadModel.Instance.UnSetItemsCount.ToString());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (!SummerWatermelonBreadModel.Instance.IsStart)
            return;
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SummerWatermelonBreadStart, null);
        SummerWatermelonBreadModel.Instance.OpenMainPopup();
    }
    private void OnDestroy()
    {
    }
}
