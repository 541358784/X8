using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
using Pack = BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities.Pack;

public class Aux_GiftBagSend4 : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _offText;

    protected override void Awake()
    {
        base.Awake();

        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        UpdateUI();
        InvokeRepeating("UpdateUI", 0, 1);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(GiftBagSend4Model.Instance.ShowEntrance());
        if (gameObject.activeSelf)
        {
            _timeText.SetText(GiftBagSend4Model.Instance.GetActivityLeftTimeString());
        }
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UIPopupGiftBagSend4Controller.Open();
    }

}