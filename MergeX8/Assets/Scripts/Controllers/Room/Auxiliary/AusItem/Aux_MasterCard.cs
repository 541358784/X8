using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
using Pack = BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities.Pack;

public class Aux_MasterCard : Aux_ItemBase
{
    private IEnumerator dailyIEnumerator = null;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _offText;

    protected override void Awake()
    {
        base.Awake();

        MasterCardModel.Instance.InitMasterCard();

        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _timeText.SetText(LocalizationManager.Instance.GetLocalizedString("UI_master_card_title"));
        InvokeRepeating("UpdateUI", 0, 1);
        UpdateUI();
    }


    public override void UpdateUI()
    {
        gameObject.SetActive(MasterCardModel.Instance.IsOpen());
        if (gameObject.activeSelf)
        {
            _timeText.SetText(MasterCardModel.Instance.GetMasterCareLeftTime());
            // StopDelayWork();
            // dailyIEnumerator = CommonUtils.DelayWork(1, () => { UpdateUI(); });
            // StartCoroutine(dailyIEnumerator);
        }
    }

    private void StopDelayWork()
    {
        if (dailyIEnumerator == null)
            return;

        StopCoroutine(dailyIEnumerator);
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UIManager.Instance.OpenUI(UINameConst.UIMasterCard, "Click");
    }


    private void OnPackBegin(BaseEvent e)
    {
        UpdateUI();
    }

    private void OnDestroy()
    {
        StopDelayWork();
    }
}