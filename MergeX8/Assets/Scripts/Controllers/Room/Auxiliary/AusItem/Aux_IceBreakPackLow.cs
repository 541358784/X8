using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
using Pack = BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities.Pack;

public class Aux_IceBreakPackLow : Aux_ItemBase
{
    private IEnumerator dailyIEnumerator = null;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _offText;

    protected override void Awake()
    {
        base.Awake();

        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateUI", 0, 1);
        EventDispatcher.Instance.AddEventListener(EventEnum.IceBreak_Pack_Begin, OnPackBegin);
    }


    public override void UpdateUI()
    {
        int leftTime = IcebreakingPackSecondModel.Instance.GetPackCoolTime(3) * 1000;
        gameObject.SetActive(IcebreakingPackSecondModel.Instance.IsOpen() && leftTime > 0);
        if (gameObject.activeSelf)
        {
            _timeText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
            // StopDelayWork();
            // if (leftTime > 0)
            // {
            //     dailyIEnumerator = CommonUtils.DelayWork(1, () => { UpdateUI(); });
            //     StartCoroutine(dailyIEnumerator);
            // }
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
        UIManager.Instance.OpenUI(UINameConst.UIPopupIcebreakingPackLow, "Click");
    }


    private void OnPackBegin(BaseEvent e)
    {
        UpdateUI();
    }

    private void OnDestroy()
    {
        StopDelayWork();
        EventDispatcher.Instance.RemoveEventListener(EventEnum.IceBreak_Pack_Begin, OnPackBegin);
    }
}