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

public class Aux_SummerWatermelonGift : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    // private LocalizeTextMeshProUGUI _offText;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    public static Aux_SummerWatermelonGift Instance;
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
    }

    public override void UpdateUI()
    {
        var package = SummerWatermelonModel.Instance.GetCurrentPackage();
        gameObject.SetActive(package != null);
        if (!gameObject.activeSelf)
            return;
        _timeText.SetText(SummerWatermelonModel.Instance.GetPackageLeftTimeText(package));
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        var package = SummerWatermelonModel.Instance.GetCurrentPackage();
        if (package == null)
            return;
        SummerWatermelonModel.Instance.OpenGiftPopup(package);
    }
    private void OnDestroy()
    {
    }
}
