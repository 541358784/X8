using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
using Pack = BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities.Pack;

public class Aux_GiftBagLink : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _offText;

    protected override void Awake()
    {
        base.Awake();

        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        UpdateUI();
        EventDispatcher.Instance.AddEventListener(EventEnum.GIFTBAGLINK_OPEN_REFRESH, Open_Refresh);
        InvokeRepeating("UpdateUI", 0, 1);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(GiftBagLinkModel.Instance.ShowEntrance());
        if (gameObject.activeSelf)
        {
            _timeText.SetText(GiftBagLinkModel.Instance.GetActivityLeftTimeString());
        }
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UIGiftBagLinkController.Open();
    }


    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GIFTBAGLINK_OPEN_REFRESH, Open_Refresh);
    }

    private void Open_Refresh(BaseEvent e)
    {
        UpdateUI();
    }
}