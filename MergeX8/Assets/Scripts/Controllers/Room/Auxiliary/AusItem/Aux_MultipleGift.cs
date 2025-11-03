using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
using Pack = BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities.Pack;

public class Aux_MultipleGift : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    protected override void Awake()
    {
        base.Awake();

        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateUI", 0, 1);
    }


    public override void UpdateUI()
    {
        gameObject.SetActive(MultipleGift.MultipleGiftModel.Instance.IsOpened());
        if (gameObject.activeSelf)
        {
            _timeText.SetText(MultipleGift.MultipleGiftModel.Instance.GetActivityRewardLeftTimeString());
         
        }
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UIManager.Instance.OpenUI(UINameConst.UIPopupMultipleGift);
    }


    private void OnDestroy()
    {
    }
}